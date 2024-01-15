using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;

//海底控制器
public class RainbowSeabedController : BaseSyncController, ISingleton, IExtInterface
{
    //有下潜装备后 关闭范围检测
    public bool HasDivingEquipment;
    //当前所在宝箱的Id
    public int CurBoxId;
    protected SubModel model;

    public void Init()
    {

    }

    public override void Enter()
    {
        //加载出生点
        LoadTreasureDiggingBirthPoint();
        //加载控制器池
        LoadPlayerControllerImpPool();
        //创建角色
        CreateUserInfo(RoomData.UserList);
        OnReproduceScene();
        //Debug.Log($"11111111111111111 RoomData.UserList =  {RoomData.UserList} ");
        MessageCenter.AddMsgListener(SeabedRewardBox.Event_PlayerChanged, OnSeabedRewardBoxHandler);

        MoveController moveController = PlayerCtrlManager.Instance().MoveController();
        moveController.SetOnDrags(OnDragStart, OnDraging, OnDragEnd);

        //CharacterManager.Instance().playerItem.OtherExtAExtInterface = this;
        //Debug.Log($"11111111111111111 RoomData.Boxs =  {RoomData.Boxs} ");
        UpdateSeabedBox(RoomData.Boxs);
    }



    public override void Leave()
    {
        UserInfos = null;

        TakeOffDivingEquipment(SelfUserId);

        ClearOtherPlayerModel();
        ClearPlayerControllerImpPool();

        //CharacterManager.Instance().playerItem.OtherExtAExtInterface = null;
        MoveController moveController = PlayerCtrlManager.Instance().MoveController();
        moveController.SetOnDrags(null, null, null);
    }

    public void UpdateSeabedBox(RepeatedField<uint> boxs)
    {
        //刷新宝箱
        foreach (uint boxIndex in boxs)
        {
            if (SeabedRewardBox.DicBoxs.TryGetValue((int)boxIndex, out var boxInfo))
            {
                boxInfo.ShowBox();
            }
        }
        //隐藏未刷新宝箱
        foreach (var boxsValue in SeabedRewardBox.DicBoxs.Values)
        {
            if (!boxs.Contains((uint)boxsValue.BoxIndex))
            {
                boxsValue.HideBox();
            }
        }
    }


    public override void LoadTreasureDiggingBirthPoint()
    {
        if (PosGo) return;
        GameObject posInt = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/HaiDiXingKongBeginPos", false);
        posInt.name = "[HaiDiXingKongBeginPos]";
        PosGo = posInt.GetComponent<RoomBirthPoint>();

        GameObject rangeGo = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/SeabedSafeRange", false);
        rangeGo.name = "[SeabedSafeRange]";
    }

    public override void ClearOtherPlayerModel()
    {
        foreach (var imp in SyncPlayerList.Values)
        {
            if (imp.IsSelf)
            {
                imp.SyncEnable(false);
            }
            else
            {
                imp.UnUseHelmet();
                ulong userId = imp.UserInfo.UserId;
                PlayerItem playerItem = imp.playerItem;
                GameObject.Destroy(imp.gameObject);
                DelHudPanel(playerItem.gameObject);
                AvatarManager.Instance().RecyclePlayerPreFun(userId);
            }
        }
        SyncPlayerList.Clear();
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

    //还原玩家的状态
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
        //还原面罩
        if (userInfo.OxygenMaskStart > 0 && userInfo.OxygenMaskEnd > 0)
        {
            int curTime = ManageMentClass.DataManagerClass.CurTime;
            if (curTime > userInfo.OxygenMaskStart && curTime < userInfo.OxygenMaskEnd)
            {
                uint sce = userInfo.OxygenMaskEnd - userInfo.OxygenMaskStart;
                //带上面罩
                DrewDivingEquipment(userInfo.UserId, sce > 65 ? 40002u : 40001u);
                //如果是我自己
                if (imp && imp.IsSelf)
                {
                    Timer.RegisterRealTimeNoLoop(1, () =>
                    {
                        //显示倒计时
                        OpenUI(FormConst.RAINBOWDEEPTIMER);
                        RainbowDeepTimer deepTimer = UIManager.GetInstance().GetUIForm<RainbowDeepTimer>(FormConst.RAINBOWDEEPTIMER);
                        deepTimer.StartTimerByEndtime((int)userInfo.OxygenMaskEnd);
                    });
                }
            }
        }
        else
        {
            TakeOffDivingEquipment(userInfo.UserId);
        }
    }

    //带上下潜设备
    public void DrewDivingEquipment(ulong userId, uint id)
    {
        bool isSelf = userId == ManageMentClass.DataManagerClass.userId;
        if (isSelf)
        {
            HasDivingEquipment = true;
            _isWorking = true;
        }
        string subModelName = id == 40001 ? "DNormalDiving" : "DSuperDiving";
        if (TryGetPlayerImp(userId, out var imp))
        {
            PlayerItem playerItem = imp.playerItem;
            GameObject go = imp.playerItem.gameObject;
            imp.Equip = subModelName;
            if (go.name == "KLS_T")
            {
                Transform yanjing = go.transform.Find("Glass");
                if (yanjing) yanjing.gameObject.SetActive(false);
            }

            SubModel modeltemp = playerItem.GetSubModel(subModelName);
            modeltemp.SetActive(true);
            if (isSelf) model = modeltemp;
        }
    }
    //脱掉下潜设备
    public void TakeOffDivingEquipment(ulong useId)
    {
        bool isSelf = useId == ManageMentClass.DataManagerClass.userId;
        if (isSelf)
        {
            HasDivingEquipment = false;
            _isWorking = false;
        }
        if (TryGetPlayerImp(useId, out var imp))
        {
            GameObject go = imp.playerItem.gameObject;
            if (go.name == "KLS_T")
            {
                Transform yanjing = go.transform.Find("Glass");
                if (yanjing) yanjing.gameObject.SetActive(true);
            }
            if (isSelf)
            {
                model?.SetActive(false);
                model = null;
                imp.Equip = "";
                imp.playerItem.OtherExtAExtInterface = null;
            }
            else if (!string.IsNullOrEmpty(imp.Equip))
            {
                SubModel subModel = imp.playerItem.GetSubModel(imp.Equip);
                subModel.SetActive(false);
                imp.Equip = "";
                imp.playerItem.OtherExtAExtInterface = null;
            }
        }
    }


    private void OnSeabedRewardBoxHandler(KeyValuesUpdate kv)
    {
        if (kv.Key == "enter")
        {
            PlayerItem playerItem = CharacterManager.Instance().playerItem;
            playerItem.SetAnimator("Idle");
            if (model != null) model.SetTrigger("Idle");
            SetMoveNormal(false);

            CurBoxId = Convert.ToInt32(kv.Values);
            BaseUIForm.AddBlackData("RewardBox_CurBoxId", CurBoxId);

            OpenUI(FormConst.RAINBOWSDKEYCONFIRM);
        }
        else if (kv.Key == "leave")
        {
            CurBoxId = 0;
            SetMoveNormal(true);
            CloseUI(FormConst.RAINBOWSDKEYCONFIRM);
        }
    }
    public void PlayerTriggerTreasure()
    {
        PlayerItem playerItem = CharacterManager.Instance().playerItem;
        playerItem.SetAnimator("Idle");
        if (model != null) model.SetTrigger("Idle");


        SetMoveNormal(false);
        OpenUI(FormConst.RAINBOWSDHELMETCONFIRM);
    }
    public void ForwardBackPosition()
    {
        Vector3 center = SeabedSafeRange.Ins.transform.position;
        GameObject go = CharacterManager.Instance().GetPlayerObj();
        Vector3 dir = (center - go.transform.position).normalized;
        go.transform.position = go.transform.position + dir * 2;
    }
    public void ResetBirthPos(ulong userId)
    {
        if (TryGetPlayerImp(userId, out var imp))
        {
            if (userId != SelfUserId) imp.moveController.StopSyncValue();
            imp.playerItem.SetAnimator("Idle");
            int index = GetSelfIndexInRoom(userId);
            Transform trans = PosGo.GetPoint(index);
            imp.playerItem.transform.position = trans.position;
            imp.playerItem.transform.rotation = trans.rotation;
        }
    }
    public int GetSelfIndexInRoom(ulong userId)
    {
        int index = -1;
        foreach (var roomUserInfo in RoomData.UserList)
        {
            index++;
            if (roomUserInfo.UserId == userId) break;
        }
        if (index == -1) index = SyncPlayerList.Count - 1;
        return index;
    }


    protected void OnDragStart()
    {
        if (model != null) model.SetTrigger("Walk");
    }
    protected void OnDraging()
    {
        if (model != null) model.SetTrigger("Walk");
    }
    protected void OnDragEnd()
    {
        if (model != null) model.SetTrigger("Idle");
    }

    //穿戴设备时 的潜水设备同步
    public void SetAnimator(string trigger, float speed = 1)
    {
        if (model != null) model.SetTrigger(trigger, speed);
    }
}
