using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;

public class RainbowBeachController : BaseSyncController, ISingleton
{
    public float MoveTime = 0;
    //当前是否在挖贝壳 改isWorking
    
    protected Timer MoveTimer;
    protected Timer ExcavateShellTimer;
    public BeachShellResp RewardInfo;
    public int LastUserItemId;
    private const string ShovelShellsAnim = "PickingShells";
    private string ShovelShellAnim = "Shovel";

    public void Init()
    {
        
    }

    //进入退出的总方法
    public override void Enter()
    {
        //加载出生点
        LoadTreasureDiggingBirthPoint();
        //加载控制器池
        LoadPlayerControllerImpPool();
        AddEvent();

        //Debug.Log($"111111111 RainbowBeachSyncNetView RoomData = {RoomData}   ");
        CreateUserInfo(RoomData.UserList);
        OnReproduceScene();
    }
    public override void Leave()
    {
        UserInfos = null;
        ClearOtherPlayerModel();
        ClearPlayerControllerImpPool();
        DelEvent();
        MoveTimer?.Cancel();
        MoveTimer = null;
        ExcavateShellTimer?.Cancel();
        ExcavateShellTimer = null;

        MoveController moveController = PlayerCtrlManager.Instance().MoveController();
        moveController.SetOnDrags(null, null, null);
    }

    //事件
    public void AddEvent()
    {
        
    }
    public void DelEvent()
    {
        
    }
    

    //加载必要组件
    public override void LoadTreasureDiggingBirthPoint()
    {
        if (PosGo) return;
        GameObject posInt = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/RainbowBeachBeginPos", true);
        posInt.name = "[RainbowBeachBeginPos]";
        PosGo = posInt.GetComponent<RoomBirthPoint>();
    }
    public override void UpdateSelfPlayerControllerImp(GameObject go)
    {
        if (RoomData == null) return;
        PlayerControllerImp newImp = AddComp<PlayerControllerImp>(go);
        RoomUserInfo newUserInfo = UserInfo;
        if (newUserInfo == null)
        {
            Debug.Log("ParlorController  RoomUserInfo  is  null  ");
            return;
        }
        newImp.IsSelf = true;
        newImp.SyncEnable(true);
        newImp.UserInfo = newUserInfo;
        newImp.LookFollowHud = AddHudPanel(go, newUserInfo, true, newUserInfo.NetworkState);
        newImp.playerItem = go.GetComponent<PlayerItem>();
        SyncPlayerList[ManageMentClass.DataManagerClass.userId] = newImp;
    }

    //设置出生点
    protected override void SetPos(GameObject go, Move move = null)
    {
        if (move != null && move.Pos != null)
        {
            go.transform.position = new Vector3(move.Pos.X / 1000f, move.Pos.Y / 1000f, move.Pos.Z / 1000f);
            go.transform.rotation = Quaternion.Euler(0, move.Dir.Y / 1000f, 0);
        }
        else
        {
            Transform trans = PosGo.GetPoint(SyncPlayerList.Count - 1);
            go.transform.position = trans.position;
            go.transform.localEulerAngles = trans.localEulerAngles;
        }
    }
    public override void OnReproduceScene()
    {
        //还原所有其他角色的状态
        foreach (var imp in SyncPlayerList.Values)
        {
            ReproduceUserState(imp.UserInfo, imp);
        }
    }
    public override void ReproduceUserState(ulong userId, bool igRefCircle = true)
    {
        if (SyncPlayerList.TryGetValue(userId, out var pcImp))
        {
            ReproduceUserState(pcImp.UserInfo, pcImp, igRefCircle);
        }
    }
    public override void ReproduceUserState(RoomUserInfo userInfo, PlayerControllerImp imp = null, bool igRefCircle = false)
    {
        if (imp && imp.IsSelf)
        {
            imp.playerItem.SetAnimator("Idle");
            MoveController moveController = PlayerCtrlManager.Instance().MoveController();
            moveController.SetOnDrags(OnDragDown, null, OnDragUp);
        }
    }

    protected void OnDragDown()
    {
        MoveTime = UnityEngine.Random.Range(5.0f, 10.0f);
        Debug.Log("111111111   RainbowBeachController  moveTime=" + MoveTime);
        MoveTimer = Timer.RegisterRealTimeNoLoop(MoveTime, OnDragIngComplete);
    }
    protected void OnDragIngComplete()
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        playerItem.SetAnimator("Idle");
        Singleton<RainbowBeachController>.Instance.SetMoveNormal(false);
        //触发一次 挖贝壳弹窗
        OpenUI(FormConst.RAINBOWSHALLCONFIRM);
    }
    protected void OnDragUp()
    {
        MoveTimer?.Cancel();
        MoveTimer = null;
        MoveTime = 0;
    }
    public void ExcavateShell(BeachShellResp resp)
    {
        _isWorking = true;
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        //playerItem.SetAnimator("Idle");
        playerItem.StartCoroutine(WaitAnimOneFrame(playerItem, resp));
    }
    protected IEnumerator WaitAnimOneFrame(PlayerItem playerItem, BeachShellResp resp)
    {
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = false;
        //子模型动画
        SubModel subModel = GetSubModel((uint)LastUserItemId, playerItem);
        subModel.SetActive(true);
        subModel.Play(ShovelShellAnim);
        //自身动画
        playerItem.SetAnimator(ShovelShellsAnim);
        playerItem.SelfAnimator.Play(ShovelShellAnim, 0, 0);
        float time = playerItem.GetCurAnimLength(ShovelShellAnim);
        yield return new WaitForSeconds(time);
        _isWorking = false;
        ExcavateShellTimer = null;
        subModel.SetActive(false);
        playerItem.SetAnimator("Idle");
        Singleton<RainbowBeachController>.Instance.SetMoveNormal(true);
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = true;
        //4=海洋垃圾
        if (resp.RewardType == 4) OpenUI(FormConst.RAINBOWBEACHWASTE);
        //123 正常奖励
        else OpenUI(FormConst.RAINBOWBEACHREWARD);
    }

    public SubModel GetSubModel(uint id, PlayerItem item)
    {
        string endWith = "_Iron";
        switch (id)
        {
            case 10001: endWith = "_Iron"; break;
            case 10002: endWith = "_Silver"; break;
            case 10003: endWith = "_Gold"; break;
        }
        return item.GetSubModel(endWith);
    }

}
