using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Treasure;
using UnityEngine;
using UnityTimer;


public class PlayerControllerImp : MonoBehaviour, IExtInterface
{
    public enum SyncProcesState
    {
        Idle, //进程空闲中
        MoveStart, //移动开始 对应玩家按压在摇杆上
        MoveSpline, //移动插值中
        MoveEnd, //移动结束接管moving插值 直接向目标为止移动
        RotaSpline, //摄像机引起的角色旋转同步
        RotaEnd, //摄像机引起的角色旋转同步结束
        WaitMoveEndDoAction, //等待移动旋转等动作的结束  之后执行动作
        WaitActionEnd, //等待动作结束
    }
    public enum SyncPipelineState
    {
        None,
        Idle, //进程空闲中
        GetMessage, //获取一条信息缓存
        MsgExecute,
        DoneToIdle,
    }

    public LookFollowHud LookFollowHud;
    public MoveControllerImp moveController;
    public PlayerItem playerItem;
    public RoomUserInfo UserInfo;
    public bool PauseImp = false;
    public string IsInteractive = "";
    public string Equip = "";

    //已执行的同步包Index
    private uint lastSyncIndex = 0;
    public uint LastSyncExecuteIndex => lastSyncIndex;
    public uint CurMaxIndex
    {
        get
        {
            uint maxIndex = 0;
            foreach (var msgCtrl in msgCtrls)
            {
                if (msgCtrl.MaxIndex() > maxIndex)
                    maxIndex = msgCtrl.MaxIndex();
            }
            return maxIndex;
        }
    }
    public bool IsSelf = false;
    //同步消息的列表
    protected List<ISyncMsgCtrl> msgCtrls = new List<ISyncMsgCtrl>();
    protected MoveSyncPipeline moveSyncPipeline;
    protected DoActionSyncPipeline doActionSyncPipeline;
    protected OtherChangeSkinPipeline otherChangeSkinPipeline;
    protected SyncRoomPlayerStatePipeline syncRoomPlayerStatePipeline;
    protected SyncRainbowPlayerStatePipeline rainbowPlayerStatePipeline;
    protected DoInteractiveSyncPipeline doInteractiveSyncPipeline;


    protected ISyncMsgCtrl CurMsgCtrl;
    protected SyncPipelineState PipelineState = SyncPipelineState.Idle;
    protected float cdRt = 0;

    protected void Awake()
    {
        if (moveController == null) moveController = GetComponent<MoveControllerImp>();
        if (playerItem == null) playerItem = GetComponent<PlayerItem>();
    }
    protected void Start()
    {
        if (IsSelf) return;
        if (msgCtrls.Count == 0)
        {
            doActionSyncPipeline = new DoActionSyncPipeline(OnSetMainPipelineState, moveController, playerItem);
            otherChangeSkinPipeline = new OtherChangeSkinPipeline(OnSetMainPipelineState, moveController, playerItem);
            syncRoomPlayerStatePipeline = new SyncRoomPlayerStatePipeline(OnSetMainPipelineState, moveController, playerItem);
            rainbowPlayerStatePipeline = new SyncRainbowPlayerStatePipeline(OnSetMainPipelineState, moveController, playerItem);
            moveSyncPipeline = new MoveSyncPipeline(OnSetMainPipelineState, moveController, playerItem);
            doInteractiveSyncPipeline = new DoInteractiveSyncPipeline(OnSetMainPipelineState, moveController, playerItem);

            //添加的顺序代表权重  越靠前越先执行
            msgCtrls.Add(doActionSyncPipeline);
            msgCtrls.Add(otherChangeSkinPipeline);
            msgCtrls.Add(syncRoomPlayerStatePipeline);

            //Move消息
            msgCtrls.Add(moveSyncPipeline);
            //交互动作 权重在move之后
            msgCtrls.Add(doInteractiveSyncPipeline);
            msgCtrls.Add(rainbowPlayerStatePipeline);
        }
    }
    protected void OnDestroy()
    {
        UserInfo = null;
        moveController = null;
        playerItem = null;
        LookFollowHud = null;
        msgCtrls.Clear();
    }

    public void AttachMoveMsg(Move data)
    {
        if (PauseImp) return;
        CheckNetWorkMsgIndex(data.Index);
        moveSyncPipeline?.Push(data);
    }
    public void AttachActionMsg(OtherDoActionPush data)
    {
        if (PauseImp) return;
        CheckNetWorkMsgIndex(data.Index);
        doActionSyncPipeline?.Push(data);
    }
    public void AttachChangeSkinMsg(OtherChangeSkinPush data)
    {
        if (PauseImp) return;
        CheckNetWorkMsgIndex(data.Index);
        otherChangeSkinPipeline?.Push(data);
    }
    public void AttachPlayerStateMsg(OtherUserStatePush data)
    {
        if (PauseImp) return;
        CheckNetWorkMsgIndex(data.Index);
        syncRoomPlayerStatePipeline?.Push(data);
    }
    public void AttachRainbowPlayerStateMsg(RoomRewardPush data)
    {
        if (PauseImp) return;
        CheckNetWorkMsgIndex(data.Index);
        rainbowPlayerStatePipeline?.Push(data);
    }
    public void AttachInteractiveStateMsg(SitdownPush data)
    {
        CheckNetWorkMsgIndex(data.Index);
        doInteractiveSyncPipeline?.Push(data);
    }



    private void CheckNetWorkMsgIndex(uint msgIndex)
    {
        if (lastSyncIndex == 0 && msgIndex > 0) lastSyncIndex = msgIndex - 1;
        //有玩家 重连或者重启  同步下标重置过
        if (msgIndex < 2 && lastSyncIndex >= 2) lastSyncIndex = 0;
    }


    public void AttachMsg(IMessage msg)
    {
        if (PauseImp) return;
        //根据move.UserId找到对应玩家的PlayerController设置摇杆方向和坐标
        //排除自己的消息
        //move.Dir有值时 move.Pos为起点坐标
        //move.Dir无值时 move.Pos为终点坐标
        foreach (ISyncMsgCtrl msgCtrl in msgCtrls)
        {
            if (msgCtrl.CheckType(msg))
            {
                msgCtrl.Push(msg);
                break;
            }
        }
    }
    public void SyncEnable(bool enable)
    {
        if (IsSelf)
        {
            MoveController moveController = PlayerCtrlManager.Instance().MoveController();
            if (moveController) moveController.IsStartUpSyncMove = enable;
            return;
        }
        if (moveController)
        {
            moveController.IsStartUpSyncMove = enable;
        }
    }
    public Vector3 GetPosition()
    {
        return playerItem.transform.position;
    }
    public Quaternion GetRota()
    {
        return playerItem.transform.rotation;
    }
    public void DestroyModel()
    {
        LookFollowHud = null;
        GameObject.Destroy(playerItem.gameObject);
    }
    public void RefPipeline()
    {
        msgCtrls.ForEach(msgLine => msgLine.UpdateRef(moveController, playerItem));
    }
    public void ResetForStart()
    {
        lastSyncIndex = 0;
        msgCtrls.ForEach(msgCtrl => msgCtrl.ClearForRestart());
    }
    private void Update()
    {
        if (IsSelf) PipelineState = SyncPipelineState.None;

        if (PipelineState == SyncPipelineState.Idle)
        {
            foreach (ISyncMsgCtrl msgCtrl in msgCtrls)
            {
                if (msgCtrl.CheckMsgIndex(lastSyncIndex))
                {
                    CurMsgCtrl = msgCtrl;
                    PipelineState = SyncPipelineState.GetMessage;
                    break;
                }
            }
            UpdateMsgCountSlerpFact();
        }

        if (PipelineState == SyncPipelineState.GetMessage)
        {
            CurMsgCtrl.InsidePop();
            CurMsgCtrl.SetStartState();
            PipelineState = SyncPipelineState.MsgExecute;
        }

        if (PipelineState == SyncPipelineState.MsgExecute)
        {
            CurMsgCtrl.OnUpdate(Time.deltaTime);
        }

        if (PipelineState == SyncPipelineState.DoneToIdle)
        {
            if (CurMsgCtrl.OverSetLastIndex())
                lastSyncIndex = CurMsgCtrl.CurIndex();
            CurMsgCtrl.Reset();
            PipelineState = SyncPipelineState.Idle;
        }

    }
    private void UpdateMsgCountSlerpFact()
    {
        int count = 0;
        foreach (ISyncMsgCtrl msgCtrl in msgCtrls)
            count += msgCtrl.GetCount();
        moveController?.SetMsgCountSlerpFact(count);
    }
    private void OnSetMainPipelineState(SyncPipelineState ps)
    {
        PipelineState = ps;
    }
    public float Distance(Vector3 tar)
    {
        return Vector3.Distance(playerItem.transform.position, tar);
    }


    //海底
    public void UseHelmet(uint toolId)
    {
        playerItem.OtherExtAExtInterface = this;
        if (!string.IsNullOrEmpty(Equip))
        {
            SubModel model = playerItem.GetSubModel(Equip);
            model.SetActive(true);
        }
    }
    public void UnUseHelmet()
    {
        playerItem.OtherExtAExtInterface = null;
        if (!string.IsNullOrEmpty(Equip))
        {
            SubModel model = playerItem.GetSubModel(Equip);
            model.SetActive(false);
        }
    }
    public void SetAnimator(string trigger, float speed = 1)
    {
        if (!string.IsNullOrEmpty(Equip))
        {
            SubModel model = playerItem.GetSubModel(Equip);
            model.SetTrigger(trigger, speed);
        }
    }



#if UNITY_EDITOR
    [ContextMenu("DeepInfo")]
    protected void OnDeepInfoExecute()
    {
        BuildPlayerImpInfo(UserInfo, this);
    }
    protected void BuildPlayerImpInfo(RoomUserInfo userInfo, PlayerControllerImp imp)
    {
        if (imp.gameObject.name == "DefaultChar") return;
        StringBuilder sb = new StringBuilder();
        sb.Append("1111  SyncCmdPipeline  useid:" + userInfo.UserId + "   state:" + userInfo.State);
        sb.Append($"  lastIndex:{lastSyncIndex} \r\n");
        sb.AppendLine($"                  MoveSyncPipeline   count:{imp.moveSyncPipeline.GetCount()}   curIndex:{imp.moveSyncPipeline.CurIndex()}   maxIndex:{imp.moveSyncPipeline.MaxIndex()}");
        sb.AppendLine($"             DoActionSyncPipeline   count:{imp.doActionSyncPipeline.GetCount()}   curIndex:{imp.doActionSyncPipeline.CurIndex()}   maxIndex:{imp.doActionSyncPipeline.MaxIndex()}");
        sb.AppendLine($"       OtherChangeSkinPipeline   count:{imp.otherChangeSkinPipeline.GetCount()}   curIndex:{imp.otherChangeSkinPipeline.CurIndex()}   maxIndex:{imp.otherChangeSkinPipeline.MaxIndex()}");
        sb.AppendLine($"SyncRoomPlayerStatePipeline   count:{imp.syncRoomPlayerStatePipeline.GetCount()}   curIndex:{imp.syncRoomPlayerStatePipeline.CurIndex()}   maxIndex:{imp.syncRoomPlayerStatePipeline.MaxIndex()}");
        Debug.Log(sb);
    }
    [ContextMenu("DeepInfoAllPlayer")]
    protected void OnDeepInfoAllExecute()
    {
        Dictionary<ulong, PlayerControllerImp> dic = Singleton<TreasuringController>.Instance.Players;
        foreach (var imp in dic)
        {
            BuildPlayerImpInfo(imp.Value.UserInfo, imp.Value);
        }
    }
#endif
}

