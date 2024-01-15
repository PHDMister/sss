using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;
using Random = UnityEngine.Random;

public class RainbowIocnController : BaseSyncController, ISingleton
{
    public Room RoomData = new Room();
    public RoomUserInfo UserInfo
    {
        get
        {
            foreach (var userInfo in RoomData.UserList)
            {
                if (userInfo.UserId == SelfUserId) return userInfo;
            }
            return null;
        }
    }

    protected Timer MoveTimer;
    protected float MoveTime = 0;

    protected Dictionary<ulong, GameObject> BoatGos = new Dictionary<ulong, GameObject>();
    public void Init()
    {

    }

    public override void SetRoomData(Room data)
    {
        RoomData = data;
    }
    public override void SetRoomUser(IEnumerable<RoomUserInfo> list)
    {
        RoomData.UserList.Clear();
        RoomData.UserList.AddRange(list);
    }
    //进入退出的总方法
    public override void Enter()
    {
        //加载出生点
        LoadTreasureDiggingBirthPoint();
        //加载控制器池
        LoadPlayerControllerImpPool();
        //创建角色
        CreateUserInfo(RoomData.UserList);
        OnReproduceScene();
    }
    public override void Leave()
    {
        UserInfos = null;
        ClearBoatModel();
        ClearOtherPlayerModel();
        ClearPlayerControllerImpPool();

        MoveTimer?.Cancel();
        MoveTimer = null;

        MoveController moveController = PlayerCtrlManager.Instance().MoveController();
        moveController.SetOnDrags(null, null, null);
    }

    //加载必要组件
    public override void LoadTreasureDiggingBirthPoint()
    {
        if (PosGo) return;
        GameObject posInt = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/ShenMiHaiWanBeginPos", true);
        posInt.name = "[ShenMiHaiWanBeginPos]";
        PosGo = posInt.GetComponent<RoomBirthPoint>();
    }
    public override void UpdateSelfPlayerControllerImp(GameObject go)
    {
        if (RoomData == null) return;
        PlayerControllerImp newImp = AddComp<PlayerControllerImp>(go);
        RoomUserInfo newUserInfo = UserInfo;
        if (newUserInfo == null)
        {
            Debug.Log("RainbowIocnController  RoomUserInfo  is  null  ");
            return;
        }
        newImp.IsSelf = true;
        newImp.SyncEnable(true);
        newImp.UserInfo = newUserInfo;
        newImp.LookFollowHud = AddHudPanel(go, newUserInfo, true, newUserInfo.NetworkState);
        newImp.playerItem = go.GetComponent<PlayerItem>();
        SyncPlayerList[ManageMentClass.DataManagerClass.userId] = newImp;
        //将船绑定到新的角色
        if (BoatGos.TryGetValue(ManageMentClass.DataManagerClass.userId, out var boat))
        {
            //GameObject boat = BoatGos[ManageMentClass.DataManagerClass.userId];
            BoatController boatController = boat.GetComponent<BoatController>();
            boatController.UnBind();
            boatController.BindPlayer(newImp.playerItem);
        }
    }
    public override void CreateUserInfo(RoomUserInfo userInfo)
    {
        base.CreateUserInfo(userInfo);
        GameObject boatGo = ResourcesMgr.GetInstance().LoadAsset("Prefabs/Fish/Boat", true);
        BoatGos[userInfo.UserId] = boatGo;
        BoatController controller = boatGo.GetComponent<BoatController>();
        PlayerControllerImp imp = Players[userInfo.UserId];
        controller.BindPlayer(imp.playerItem);
    }
    public override void CreateUserInfo(ulong userId, RepeatedField<long> avatarIds)
    {
        if (SyncPlayerList.TryGetValue(userId, out var imp))
        {
            if (userId == ManageMentClass.DataManagerClass.userId)
            {
                GameObject go = CharacterManager.Instance().GetPlayerObj();
                imp.playerItem = go.GetComponent<PlayerItem>();
                imp.LookFollowHud = AddHudPanel(go, imp.UserInfo, imp.IsSelf);
                //处理新模型与船的绑定
                BoatController boatController = BoatGos[userId].GetComponent<BoatController>();
                boatController.UnBind();
                boatController.BindPlayer(imp.playerItem);
            }
            else
            {
                GameObject go = AvatarManager.Instance().ChangeOtherAvatarFun(userId, avatarIds);
                go.name = "player_" + userId;
                CharacterManager.Instance().PlayOtherPlayerSpecialEffect(go);
                imp.playerItem = go.GetComponent<PlayerItem>();
                imp.moveController = go.GetComponent<MoveControllerImp>();
                imp.LookFollowHud = AddHudPanel(go, imp.UserInfo, imp.IsSelf, imp.UserInfo.NetworkState);
                imp.RefPipeline();
                //处理新模型与船的绑定
                BoatController boatController = BoatGos[userId].GetComponent<BoatController>();
                boatController.UnBind();
                boatController.BindPlayer(imp.playerItem);
            }
        }
    }
    public void ClearBoatModel()
    {
        foreach (var boatGo in BoatGos.Values)
        {
            BoatController boatController = boatGo.GetComponent<BoatController>();
            boatController.UnBind("Idle");
            GameObject.Destroy(boatGo);
        }
        BoatGos.Clear();
    }
    public override void RemoveRoomPlayer(ulong userId)
    {
        base.RemoveRoomPlayer(userId);
        if (BoatGos.TryGetValue(userId, out var boatGo))
        {
            BoatController boatController = boatGo.GetComponent<BoatController>();
            boatController.UnBind("Idle");
            BoatGos.Remove(userId);
            GameObject.Destroy(boatGo);
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
            MoveController moveController = PlayerCtrlManager.Instance().MoveController();
            moveController.SetOnDrags(OnDragDown, null, OnDragUp);
        }
        if (imp)
        {
            imp.playerItem.SelfAnimator.ResetTrigger("Idle");
            imp.playerItem.SelfAnimator.Play("SitDown", 0, 1);
        }
    }
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

    protected void OnDragDown()
    {
        MoveTime = Random.Range(5.0f, 10.0f);
        Debug.Log("111111111   RainbowIocnController  moveTime=" + MoveTime);
        MoveTimer = Timer.RegisterRealTimeNoLoop(MoveTime, OnDragIngComplete);
        GameObject obj = BoatGos[ManageMentClass.DataManagerClass.userId];
        obj.GetComponent<BoatController>().PlayEffect();
    }
    protected void OnDragIngComplete()
    {
        SetMoveNormal(false);
        OpenUI(FormConst.RAINBOWIOCNCONFIRM);

        GameObject obj = BoatGos[ManageMentClass.DataManagerClass.userId];
        obj.GetComponent<BoatController>().StopEffect();
    }
    protected void OnDragUp()
    {
        MoveTimer?.Cancel();
        MoveTimer = null;
        MoveTime = 0;
        GameObject obj = BoatGos[ManageMentClass.DataManagerClass.userId];
        obj.GetComponent<BoatController>().StopEffect();
    }
    //钓鱼的过程
    public void Fishing(BeachShellResp resp)
    {
        _isWorking = true;
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        playerItem.StartCoroutine(WaitAnimOneFrame(playerItem, resp));
    }
    public IEnumerator WaitAnimOneFrame(PlayerItem playerItem, BeachShellResp resp)
    {
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = false;
        string rod = GetFishingRod(resp.ToolId);
        GameObject model = resp.RewardType == 4 ? GetWasteModel() : GetFishModel(resp.RewardId);
        //子模型和动画
        SubModel subModel = playerItem.GetSubModel(rod);
        subModel.SetActive(true);
        subModel.Play(rod);
        model.SetActive(false);
        subModel.AddModel("FishPoint", model);

        //自身动画
        float otherTime = 2;
        playerItem.SelfAnimator.Play(rod, 0, 0);
        float time = playerItem.GetCurAnimLength(rod);
        yield return new WaitForSeconds(otherTime);
        model.SetActive(true);
        yield return new WaitForSeconds(time - otherTime);
        _isWorking = false;
        subModel.SetActive(false);
        subModel.RemoveModel();

        playerItem.SelfAnimator.ResetTrigger("Idle");
        SetMoveNormal(true);
        playerItem.SelfAnimator.Play("SitDown", 0, 1);
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = true;
        //4=海洋垃圾
        if (resp.RewardType == 4) OpenUI(FormConst.RAINBOWIOCNWASTE);
        //123 正常奖励
        else OpenUI(FormConst.RAINBOWIOCNREWARD);
    }
    public string GetFishingRod(uint id)
    {
        if (id == 30001) return "NormalRod";
        if (id == 30002) return "SuperRod";
        return "NormalRod";
    }
    public GameObject GetFishModel(uint id)
    {
        island_fish fish = ManageMentClass.DataManagerClass.GetIsLandFishTable((int)id);
        string path = "Prefabs/Fish/" + fish.fish_modle;
        return ResourcesMgr.GetInstance().LoadAsset(path, false);
    }
    public GameObject GetWasteModel()
    {
        string path = "Prefabs/Fish/waste";
        return ResourcesMgr.GetInstance().LoadAsset(path, false);
    }

    public override void ShowOrHideOtherRoomPlayer(bool bShow)
    {
        base.ShowOrHideOtherRoomPlayer(bShow);
        foreach (var boat in BoatGos)
        {
            if (boat.Key != ManageMentClass.DataManagerClass.userId)
            {
                boat.Value.SetActive(bShow);
            }
        }
    }
}
