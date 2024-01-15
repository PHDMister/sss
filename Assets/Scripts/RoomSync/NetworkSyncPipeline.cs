using System;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UnityEngine;

/// <summary>
/// 网络同步 消息流水线
/// </summary>
/// <typeparam name="T">同步协议</typeparam>
public abstract class SyncCmdPipeline<T> : ISyncMsgCtrl
{
    protected PlayerControllerImp.SyncProcesState SyncProcesState;
    protected Action<PlayerControllerImp.SyncPipelineState> OnSetState;
    protected MoveControllerImp MoveController;
    protected PlayerItem PlayerItem;
    protected T CurMsg;
    protected List<T> MsgList = new List<T>(8);


    protected uint CurMaxIndex;
    protected bool IsOverSetLastIndex = true;
    protected float PopTimeStart = 0;
    protected uint ExeIndex = 0;

    protected SyncCmdPipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem)
    {
        this.OnSetState = onSetState;
        MoveController = imp;
        PlayerItem = pItem;
    }

    #region 必要实现函数  每一个子类必须实现的函数
    public abstract uint CurIndex();
    public abstract void Push(IMessage msg);
    public abstract void OnUpdate(float time);
    public abstract bool CheckMsgIndex(uint lastIndex);
    public virtual void InsidePop()
    {
        T data = MsgList[MsgList.Count - 1];
        MsgList.RemoveAt(MsgList.Count - 1);
        CurMsg = data;
    } 
    #endregion



    public virtual int GetCount()
    {
        return MsgList.Count;
    }
    public virtual uint MaxIndex()
    {
        return CurMaxIndex;
    }
    public virtual bool CheckType(IMessage msg)
    {
        return msg is T;
    }
    public virtual void Reset()
    {
        CurMsg = default(T);
    }
    public virtual void UpdateRef(MoveControllerImp move, PlayerItem item)
    {
        MoveController = move;
        PlayerItem = item;
    }
    public virtual bool OverSetLastIndex()
    {
        return IsOverSetLastIndex;
    }
    public virtual PlayerControllerImp.SyncProcesState GetStartState()
    {
        return SyncProcesState;
    }
    public virtual void SetStartState()
    {
        SyncProcesState = PlayerControllerImp.SyncProcesState.Idle;
    }
    public virtual bool CheckPipelineBlock()
    {
        if (Time.realtimeSinceStartup - PopTimeStart > 3)
        {
            return true;
        }
        return false;
    }
    public virtual void ClearForRestart()
    {
        CurMaxIndex = 0;
        SyncProcesState = PlayerControllerImp.SyncProcesState.Idle;
        CurMsg = default(T);
        MsgList.Clear();
    }
}

/// <summary>
/// 移动同步
/// </summary>
public class MoveSyncPipeline : SyncCmdPipeline<Move>
{
    public MoveSyncPipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem)
        : base(onSetState, imp, pItem)
    {

    }

    public override void InsidePop()
    {
        base.InsidePop();
        ExeIndex = CurMsg.Index;
    }
    public override void Push(IMessage msg)
    {
        Move move = msg as Move;
        if (move.Index >= CurMaxIndex) CurMaxIndex = move.Index;
        MsgList.Add(move);
        MsgList.Sort((a, b) => b.Index.CompareTo(a.Index));
        if (MsgList.Count > 20) Debug.Log("111111111  userId =" + PlayerItem.gameObject.name + "  syncState=" + SyncProcesState);
    }
    public override uint CurIndex()
    {
        if (CurMsg == null) return 0;
        return CurMsg.Index;
    }
    public override bool CheckMsgIndex(uint lastIndex)
    {
        //乱序 或者 当前执行的包的Index小于已执行的Index 舍弃
        if (MsgList.Count == 0) return false;
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            Move move = MsgList[i];
            if (move.Index <= lastIndex)
                MsgList.RemoveAt(i);
            else
                return true;
        }
        return false;
    }
    public override void SetStartState()
    {
        switch (CurMsg.State)
        {
            case 1: SyncProcesState = PlayerControllerImp.SyncProcesState.MoveStart; break;
            case 2: SyncProcesState = PlayerControllerImp.SyncProcesState.MoveSpline; break;
            case 3: SyncProcesState = PlayerControllerImp.SyncProcesState.MoveEnd; break;
            case 11:
            case 12: SyncProcesState = PlayerControllerImp.SyncProcesState.RotaSpline; break;
        }
    }
    public override void OnUpdate(float time)
    {
        //移动开始立即执行一次
        if (SyncProcesState == PlayerControllerImp.SyncProcesState.MoveStart)
        {
            //Debug.Log("1111111  MoveStart =" + moveController.CurSeizingState);
            UpdateMoveStart(CurMsg);
            OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
        }
        //当前是移动消息  进行消息插值
        else if (SyncProcesState == PlayerControllerImp.SyncProcesState.MoveSpline)
        {
            //等效插值
            if (UpdateMoving(CurMsg) || MoveController.CurSeizingState == 1)
            {
                //Debug.Log("1111111 MoveSpline =" + moveController.CurSeizingState);
                OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
            }
        }
        //移动结束 进行坐标对比 执行一次或者插值执行
        else if (SyncProcesState == PlayerControllerImp.SyncProcesState.MoveEnd)
        {
            //结束的同步
            if (UpdateMoveEnd(CurMsg) || MoveController.CurSeizingState == 1)
            {
                //Debug.Log("1111111 MoveEnd =" + moveController.CurSeizingState);
                OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
            }
        }
        //摄像机引起的角色旋转同步
        else if (SyncProcesState == PlayerControllerImp.SyncProcesState.RotaSpline)
        {
            //Debug.Log("1111111 RotaSpline =" + moveController.CurSeizingState);
            UpdateRotaSpline(CurMsg);
            OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
        }
    }

    private void UpdateMoveStart(Move move)
    {
        float nowDis = move.Dir.Z / 1000.0f;
        float ry = move.Dir.Y / 1000.0f;
        Vector3 pos = new Vector3(move.Pos.X / 1000.0f, move.Pos.Y / 1000.0f, move.Pos.Z / 1000.0f);
        MoveController.SyncMoving(ry, pos, nowDis);
    }
    private bool UpdateMoving(Move move)
    {
        float nowDis = move.Dir.Z / 1000.0f;
        float ry = move.Dir.Y / 1000.0f;
        Vector3 pos = new Vector3(move.Pos.X / 1000.0f, move.Pos.Y / 1000.0f, move.Pos.Z / 1000.0f);
        return MoveController.SyncMoving(ry, pos, nowDis);
    }
    private bool UpdateMoveEnd(Move move)
    {
        float nowDis = move.Dir.Z / 1000.0f;
        float ry = move.Dir.Y / 1000.0f;
        Vector3 pos = new Vector3(move.Pos.X / 1000.0f, move.Pos.Y / 1000.0f, move.Pos.Z / 1000.0f);
        return MoveController.SyncMoving(ry, pos, nowDis);
    }
    private bool UpdateRotaSpline(Move move)
    {
        float rotaY = move.Dir.Y / 1000.0f;
        Vector3 pos = new Vector3(move.Pos.X / 1000.0f, move.Pos.Y / 1000.0f, move.Pos.Z / 1000.0f);
        return MoveController.SyncRotaing(rotaY, pos);
    }
}

/// <summary>
/// 表情动作同步
/// </summary>
public class DoActionSyncPipeline : SyncCmdPipeline<OtherDoActionPush>
{
    private float rtNoCacheWaitRestateTime;
    public DoActionSyncPipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem)
        : base(onSetState, imp, pItem)
    {
    }

    public override void InsidePop()
    {
        base.InsidePop();
        ExeIndex = CurMsg.Index;
    }
    public override void Push(IMessage msg)
    {
        OtherDoActionPush doActionPush = msg as OtherDoActionPush;
        if (doActionPush.Index >= CurMaxIndex) CurMaxIndex = doActionPush.Index;
        MsgList.Add(doActionPush);
        MsgList.Sort((a, b) => b.Index.CompareTo(a.Index));
    }
    public override void Reset()
    {
        CurMsg = null;
    }
    public override uint CurIndex()
    {
        if (CurMsg == null) return 0;
        return CurMsg.Index;
    }
    public override bool CheckMsgIndex(uint lastIndex)
    {
        //乱序 或者 当前执行的包的Index小于已执行的Index 舍弃
        if (MsgList.Count == 0) return false;
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            OtherDoActionPush doAction = MsgList[i];
            if (doAction.Index <= lastIndex)
                MsgList.RemoveAt(i);
            //客户端重启过下标重置了 执行过的下标只会大于等于lastIndex
            else if (lastIndex < ExeIndex)
                return true;
            else
                return doAction.Index > lastIndex && (doAction.Index - lastIndex) <= 1;
        }
        return false;
    }
    public override void SetStartState()
    {
        SyncProcesState = PlayerControllerImp.SyncProcesState.WaitMoveEndDoAction;
        rtNoCacheWaitRestateTime = Time.realtimeSinceStartup;
    }
    public override void OnUpdate(float time)
    {
        //等待移动旋转等动作的结束  之后执行动作
        if (SyncProcesState == PlayerControllerImp.SyncProcesState.WaitMoveEndDoAction)
        {
            if (!MoveController.IsIdle) return;
            if (DoAction_ExecuteAction(out float animLength))
            {
                //syncProcesState = PlayerControllerImp.SyncProcesState.WaitActionEnd;
                //Timer.Register(animLength, () =>
                //{
                //    playerItem.SetAnimator("Idle");
                //    OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
                //});
                OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
            }
            else
            {
                //动作执行失败 
                OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
            }
        }
        else if (SyncProcesState == PlayerControllerImp.SyncProcesState.WaitActionEnd)
        {
            if (Time.realtimeSinceStartup - rtNoCacheWaitRestateTime >= 5)
            {
                OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
            }
        }
    }
    private bool DoAction_ExecuteAction(out float animLength)
    {
        animLength = 0;
        int itemId = (int)CurMsg.Action;
        animation m_Animation = ManageMentClass.DataManagerClass.GetAnimationTableFun(itemId);
        if (m_Animation != null)
        {
            string actClipName = m_Animation.animation_model;
            if (PlayerItem.IsHaveState(actClipName))
            {
                animLength = PlayerItem.GetCurAnimLength(actClipName);
                PlayerItem.SetAnimator(actClipName);
                return true;
            }
            ToastManager.Instance.ShowNewToast(string.Format("暂未拥有{0}动作，敬请期待！", m_Animation.animation_name), 5f);
            return false;
        }
        return false;
    }
}

/// <summary>
/// 换装同步
/// </summary>
public class OtherChangeSkinPipeline : SyncCmdPipeline<OtherChangeSkinPush>
{
    public OtherChangeSkinPipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem)
        : base(onSetState, imp, pItem)
    {
    }

    public override void Push(IMessage msg)
    {
        OtherChangeSkinPush skinPush = msg as OtherChangeSkinPush;
        if (skinPush.Index >= CurMaxIndex) CurMaxIndex = skinPush.Index;
        MsgList.Add(skinPush);
        MsgList.Sort((a, b) => b.Index.CompareTo(a.Index));
    }
    public override void InsidePop()
    {
        OtherChangeSkinPush data = MsgList[MsgList.Count - 1];
        MsgList.RemoveAt(MsgList.Count - 1);
        CurMsg = data;
        ExeIndex = CurMsg.Index;
    }
    public override uint CurIndex()
    {
        if (CurMsg == null) return 0;
        return CurMsg.Index;
    }
    public override bool CheckMsgIndex(uint lastIndex)
    {
        //乱序 或者 当前执行的包的Index小于已执行的Index 舍弃
        if (MsgList.Count == 0) return false;
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            OtherChangeSkinPush doAction = MsgList[i];
            if (doAction.Index <= lastIndex)
                MsgList.RemoveAt(i);
            //客户端重启过下标重置了 执行过的下标只会大于等于lastIndex
            else if (lastIndex < ExeIndex)
                return true;
            else
                return doAction.Index > lastIndex && (doAction.Index - lastIndex) <= 1;
        }
        return false;
    }
    public override void SetStartState()
    {
        SyncProcesState = PlayerControllerImp.SyncProcesState.WaitMoveEndDoAction;
    }
    public override void OnUpdate(float time)
    {
        if (!MoveController.IsIdle) return;
        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.TreasureDigging)
        {
            Singleton<TreasuringController>.Instance.CreateUserInfo(CurMsg.FromUserId, CurMsg.AvatarIds);
            Singleton<TreasuringController>.Instance.ReproduceUserState(CurMsg.FromUserId);
        }
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.parlorScene)
        {
            Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative();
            Singleton<ParlorController>.Instance.CreateUserInfo(CurMsg.FromUserId, CurMsg.AvatarIds);
            Singleton<ParlorController>.Instance.ReproduceUserState(CurMsg.FromUserId);
        }
        OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
    }
}

/// <summary>
/// 房间内角色状态同步
/// </summary>
public class SyncRoomPlayerStatePipeline : SyncCmdPipeline<OtherUserStatePush>
{
    public SyncRoomPlayerStatePipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem) 
        : base(onSetState, imp, pItem)
    {
        IsOverSetLastIndex = false;
    }

    public override uint CurIndex()
    {
        if (CurMsg == null) return 0;
        return CurMsg.Index;
    }
    public override void Push(IMessage msg)
    {
        MsgList.Add(msg as OtherUserStatePush);
        MsgList.Sort((a, b) => b.Index.CompareTo(a.Index));
    }
    public override bool CheckMsgIndex(uint lastIndex)
    {
        if (MsgList.Count == 0) return false;
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            OtherUserStatePush userState = MsgList[i];
            if (userState.Index <= lastIndex)
                MsgList.RemoveAt(i);
            //客户端重启过下标重置了 执行过的下标只会大于等于lastIndex
            else if (lastIndex < ExeIndex)
                return true;
            else
                return userState.Index > lastIndex && (userState.Index - lastIndex) <= 1;
        }
        return false;
    }
    public override void InsidePop()
    {
        base.InsidePop();
        ExeIndex = CurMsg.Index;
        PopTimeStart = Time.realtimeSinceStartup;
    }
    public override void OnUpdate(float time)
    {
        if (!CheckPipelineBlock())
        {
            if (!MoveController.IsIdle) return;
            if (CurMsg.State == 4 && !PlayerItem.IsPlaying("Idle")) return;
            if (CurMsg.State == 5 && !PlayerItem.IsPlaying("Idle") && !PlayerItem.IsPlaying("W_Idle")) return;
        }
        RoomUserInfo userInfo = Singleton<TreasuringController>.Instance.SetPlayerState(CurMsg.FromUserId, CurMsg.State);
        Singleton<TreasuringController>.Instance.ReproduceUserState(userInfo);
        Singleton<TreasuringController>.Instance.CheckPartnerUpdateUI(userInfo);
        Debug.Log($"1111111111  SyncRoomPlayerStatePipeline   userId: {CurMsg.FromUserId }   state: {CurMsg.State}");
        //动作执行完成
        OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
    }
    public override bool CheckPipelineBlock()
    {
        if (Time.realtimeSinceStartup - PopTimeStart > 5)
        {
            return true;
        }
        return false;
    }
}

/// <summary>
/// 交互动作同步
/// </summary>
public class DoInteractiveSyncPipeline : SyncCmdPipeline<SitdownPush>
{
    public DoInteractiveSyncPipeline(Action<PlayerControllerImp.SyncPipelineState> onSetState
        , MoveControllerImp imp, PlayerItem pItem)
        : base(onSetState, imp, pItem)
    {
    }
    public override uint CurIndex()
    {
        if (CurMsg == null) return 0;
        return CurMsg.Index;
    }
    public override void Push(IMessage msg)
    {
        SitdownPush move = msg as SitdownPush;
        if (move.Index >= CurMaxIndex) CurMaxIndex = move.Index;
        MsgList.Add(move);
        MsgList.Sort((a, b) => b.Index.CompareTo(a.Index));
        if (MsgList.Count > 5) Debug.Log("111111111  userId =" + PlayerItem.gameObject.name + "  syncState=" + SyncProcesState);
    }
    public override bool CheckMsgIndex(uint lastIndex)
    {
        if (MsgList.Count == 0) return false;
        for (int i = MsgList.Count - 1; i >= 0; i--)
        {
            SitdownPush userState = MsgList[i];
            if (userState.Index <= lastIndex)
                MsgList.RemoveAt(i);
            //客户端重启过下标重置了 执行过的下标只会大于等于lastIndex
            else if (lastIndex < ExeIndex)
                return true;
            else
                return userState.Index > lastIndex && (userState.Index - lastIndex) <= 1;
        }
        return false;
    }
    public override void InsidePop()
    {
        base.InsidePop();
        ExeIndex = CurMsg.Index;
        PopTimeStart = Time.realtimeSinceStartup;
    }
    public override void OnUpdate(float time)
    {
        if (!CheckPipelineBlock())
        {
            if (!MoveController.IsIdle) return;
        }

        if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.parlorScene)
        {
            //执行交互或者取消交互
            if (CurMsg.IsLeave == false)
                Singleton<ParlorController>.Instance.DoInterativeAction(CurMsg);
            else
                Singleton<ParlorController>.Instance.CancelInteractiveAction(CurMsg);
        }
      
        //动作执行完成
        OnSetState?.Invoke(PlayerControllerImp.SyncPipelineState.DoneToIdle);
    }
}