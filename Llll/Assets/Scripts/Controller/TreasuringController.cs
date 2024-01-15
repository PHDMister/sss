using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;
using Google.Protobuf.Collections;


public class TreasuringController : BaseSyncController, ISingleton
{
    public const string Event_Open = "TreasureOpen";
    public const int CircleNum = 7;
    public static bool EnterCircleShowTip = true;
    protected const string PlayerPrefsKey = "EnterCircleShowTip";
    public static bool ReadyShowNextCheck = true;
    public const string PlayerPrefs_ShowNextCheck = "PlayerPrefs_ShowNextCheck";

    public void Init()
    {
        //WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.StartTreasurePush, OnTreasureStartPush);
        //WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RewardPush, OnRewardPush);
        //WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherUserStatePush, OnOtherUserStatePushHandle);
        //WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.TeamUpdatePush, OnTeamUpdatePush);

    }
    private void InitPlayerPrefsData()
    {
        string circleShowTip = PlayerPrefs.GetString(PlayerPrefsKey, "");
        if (string.IsNullOrEmpty(circleShowTip))
            EnterCircleShowTip = true;
        else if (int.TryParse(circleShowTip, out int timestamp))
            EnterCircleShowTip = CalcTools.IsNextOrDoubleDay(TreasureModel.Instance.CurTime, timestamp);

        string tipTime = PlayerPrefs.GetString(PlayerPrefs_ShowNextCheck, "");
        if (string.IsNullOrEmpty(tipTime))
            ReadyShowNextCheck = true;
        else if (int.TryParse(tipTime, out int timestamp))
            ReadyShowNextCheck = CalcTools.IsNextOrDoubleDay(TreasureModel.Instance.CurTime, timestamp);
    }


    //进入退出的总方法
    public override void Enter()
    {
        enterRoomCount++;
        RewardPush = null;
        LoadTreasureDiggingBirthPoint();
        LoadPlayerControllerImpPool();
        CloseCircle();
        AddEvent();
        InitPlayerPrefsData();
    }
    public override void Leave()
    {
        UserInfos = null;
        ClearRoomBirthPoint();
        ClearOtherPlayerModel();
        ClearPlayerControllerImpPool();
        DelEvent();
        ClearSelfMoveCheck();
        enterRoomCount = 0;
    }

    //事件
    public void AddEvent()
    {
        MessageCenter.AddMsgListener(Event_Open, OnTreasureOpenHandle);
        MessageCenter.AddMsgListener(TreasureCircle.Event_PlayerChanged, OnTreasureCirclePlayerChanged);

        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.StartTreasurePush, OnTreasureStartPush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RewardPush, OnRewardPush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherUserStatePush, OnOtherUserStatePushHandle);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.TeamUpdatePush, OnTeamUpdatePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.TreasureActivityEndPush, OnRoomActivityEndHandle);
    }
    public void DelEvent()
    {
        MessageCenter.RemoveMsgListener(Event_Open, OnTreasureOpenHandle);
        MessageCenter.RemoveMsgListener(TreasureCircle.Event_PlayerChanged, OnTreasureCirclePlayerChanged);

        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.StartTreasurePush, OnTreasureStartPush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.RewardPush, OnRewardPush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.OtherUserStatePush, OnOtherUserStatePushHandle);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.TeamUpdatePush, OnTeamUpdatePush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.TreasureActivityEndPush, OnRoomActivityEndHandle);
    }

    //挖宝圈
    public void OpenCircle()
    {
        for (int i = 1; i <= CircleNum; i++)
        {
            //MessageCenter.SendMessage(TreasureCircle.Event_Enable, new KeyValuesUpdate(i.ToString(), 1));
            MessageCenter.SendMessage(TreasureCircle.Event_Enable, i.ToString(), 1);
        }
    }
    public void CloseCircle()
    {
        for (int i = 1; i <= CircleNum; i++)
        {
            //MessageCenter.SendMessage(TreasureCircle.Event_Enable, new KeyValuesUpdate(i.ToString(), 0));
            MessageCenter.SendMessage(TreasureCircle.Event_Enable, i.ToString(), 0);
        }
    }
    public void OpenCircleExcept(uint circle)
    {
        for (int i = 1; i <= CircleNum; i++)
        {
            //MessageCenter.SendMessage(TreasureCircle.Event_Enable, new KeyValuesUpdate(i.ToString(), i == circle ? 1 : 0));
            MessageCenter.SendMessage(TreasureCircle.Event_Enable, i.ToString(), i == circle ? 1 : 0);
        }
    }

    //推送及事件处理
    private void OnTreasureOpenHandle(KeyValuesUpdate kv)
    {
        MessageCenter.RemoveMsgListener(Event_Open, OnTreasureOpenHandle);
        RoomUserInfo userInfo = GetSelfUserInfo();
        Debug.Log($"111111111  OnTreasureOpenHandle  data={userInfo}");
        if (userInfo != null) OpenCircleExcept(userInfo.CircleIndex);
    }
    private void OnTreasureCirclePlayerChanged(KeyValuesUpdate kv)
    {
        int circleId = Convert.ToInt32(kv.Values);
        RoomUserInfo userInfo = GetSelfUserInfo();
        //Debug.Log($"111111111  OnTreasureCirclePlayerChanged  data={userInfo}   key:{kv.Key}");
        if (userInfo != null && circleId == userInfo.CircleIndex)
        {
            if (kv.Key == "enter" || kv.Key == "stay")
            {
                //显示UI
                if (UIManager.GetInstance().IsOpend(FormConst.SHOPNEWUIFORM)) return;
                if (UIManager.GetInstance().IsOpend(FormConst.APPEARANCEEDITORUIFORM)) return;
                OpenUI(FormConst.TREASUREREADYPANEL);
            }
            else if (kv.Key == "leave")
            {
                //关闭UI
                CloseUI(FormConst.TREASUREREADYPANEL);
            }
        }
    }
    private void OnTreasureStartPush(uint code, ByteString dataBytes)
    {
        uint timeStamp = (uint)TreasureModel.Instance.CurTime;
        TreasureModel.Instance.TeamUserList.ForEach(info => info.Start = timeStamp);
        //设置状态
        //SetTeamPlayerState((uint)PlayerState.Types.Enum.Treasureing);
        SetPlayerState(0, (uint)PlayerState.Types.Enum.Treasureing);
        //本队开始播放挖掘动画
        SetTeamPlayerAnim(0, "W_Mine");
        //添加移动拦截
        SetSelfMoveCheck();
        //UI表现更新
        MessageCenter.SendMessage(UITreasureReadyPanel.Event_OnTreasureStart, KeyValuesUpdate.Empty);
    }
    private void OnRewardPush(uint code, ByteString dataBytes)
    {
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = true;
        RewardPush rewardPush = RewardPush.Parser.ParseFrom(dataBytes);
        Debug.Log($"11111111111   OnRewardPush   {rewardPush}");
        RewardPush = rewardPush;
        //关闭UI面板
        CloseUI(FormConst.TREASUREREADYPANEL);
        CloseUI(FormConst.UITREASUREPARTNERLEAVETIP);
        //设置全队
        SetTeamPlayerState((uint)PlayerState.Types.Enum.Idle);
        //重置模型动作
        SetTeamPlayerAnim("W_Done");
        var plyerImp = SyncPlayerList[ManageMentClass.DataManagerClass.userId];
        float delay = plyerImp.playerItem.GetCurAnimLength("W_Done");
        UnityTimer.Timer.RegisterRealTimeNoLoop(delay * 0.9f, () =>
          {
              foreach (RoomUserInfo userInfo in TreasureModel.Instance.TeamUserList)
              {
                  if (SyncPlayerList.TryGetValue(userInfo.UserId, out var imp))
                  {
                      if (imp.UserInfo != null && imp.playerItem != null)
                      {
                          if (imp.UserInfo.State == 1 && !imp.playerItem.IsPlaying("Idle"))
                              imp.playerItem.SetAnimator("Idle");
                          else if (imp.UserInfo.State == 4 && !imp.playerItem.IsPlaying("W_Idle"))
                              imp.playerItem.SetAnimator("W_Idle");
                          else if (imp.UserInfo.State == 5 && !imp.playerItem.IsPlaying("W_Mine"))
                              imp.playerItem.SetAnimator("W_Mine");
                      }
                  }
              }
          });
        //取消摇杆拦截
        ClearSelfMoveCheck();
        //显示宝箱
        //MessageCenter.SendMessage(TreasureCircle.Event_RewardBoxHandle, "show", ManageMentClass.DataManagerClass.userId);
        MessageCenter.SendMessage(TreasureCircle.Event_AppointRewardBoxHandle, "show", rewardPush.CircleIndex);
    }
    private void OnSendSyncCheckHandle()
    {
        CloseUI(FormConst.UITREASUREPARTNERLEAVETIP);
        OpenUI(FormConst.TREASUREHIATUSTIP);
    }
    private void OnReadyMoveCheckHandle()
    {
        ClearSelfMoveCheck();
        Debug.Log("111111  玩家发起了   取消准备 ");
        CancelReadyTreasureReq cancelReady = new CancelReadyTreasureReq();
        cancelReady.UserId = ManageMentClass.DataManagerClass.userId;
        ulong userId = cancelReady.UserId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.CancelReadyTreasureReq, cancelReady, (code, datas) =>
        {
            CancelReadyTreasureResp resp = CancelReadyTreasureResp.Parser.ParseFrom(datas);
            Debug.Log("111111    取消准备的响应    respone :" + resp.StatusCode);
            if (resp.StatusCode == 0)
            {
                SetPlayerState(userId, (uint)PlayerState.Types.Enum.Idle);
                SetTeamPlayerAnim(userId, "Idle");
                MessageCenter.SendMessage(UITreasureReadyPanel.Event_OnUpdateReadyList, KeyValuesUpdate.Empty);
            }
        });
    }
    private void OnOtherUserStatePushHandle(uint code, ByteString dataBytes)
    {
        OtherUserStatePush userState = OtherUserStatePush.Parser.ParseFrom(dataBytes);
        if (SyncPlayerList.TryGetValue(userState.FromUserId, out var imp))
        {
            Debug.Log($"111111111111  OtherUserStatePush:{userState}   ");
            if (userState.FromUserId == ManageMentClass.DataManagerClass.userId)
            {
                //if (imp.UserInfo.State == userState.State) return;
                //imp.UserInfo.State = userState.State;
                //ReproduceUserState(imp.UserInfo, imp, true);
                ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = userState.State != (uint)PlayerState.Types.Enum.Treasureing;
                return;
            }
            userState.Index = imp.CurMaxIndex + 1;
            imp.AttachPlayerStateMsg(userState);
        }
    }
    private void OnTeamUpdatePush(uint code, ByteString dataBytes)
    {
        TeamInfoResp teamUpdatePush = TeamInfoResp.Parser.ParseFrom(dataBytes);
        TreasureModel.Instance.TeamUserList = teamUpdatePush.List.ToList();
        RoomUserInfo selfUserInfo = GetSelfUserInfo();
        if (selfUserInfo != null)
        {
            if (SyncPlayerList.TryGetValue(ManageMentClass.DataManagerClass.userId, out var imp))
            {
                ReproduceUserState(selfUserInfo, imp);
                MessageCenter.SendMessage(UITreasureReadyPanel.Event_OnUpdateReadyList, KeyValuesUpdate.Empty);
            }
        }
    }
    private void OnRoomActivityEndHandle(uint code, ByteString dataBytes)
    {
        TreasureActivityEndPush roomEndPush = TreasureActivityEndPush.Parser.ParseFrom(dataBytes);
        Debug.Log($"1111111  OnRoomActivityEndHandle:{roomEndPush}");
        //关闭寻宝相关UI
        CloseUI(FormConst.TREASUREREADYPANEL);
        CloseUI(FormConst.TREASUREREWARDPANEL);
        CloseUI(FormConst.TREASUREHIATUSTIP);

        MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

        //房间刷新
        ManageMentClass.DataManagerClass.roomId = roomEndPush.Room.RoomId;
        //角色刷新
        TreasureModel.Instance.UpdateRoomPlayer(roomEndPush.Room.UserList, true);
        //起loading
        TreasureLoadManager.Instance().Load(LoadType.TreasureEnd);

        //重新检测倒计时
        TreasureModel.Instance.TreasureEndHandle(roomEndPush);
        
        MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);
    }


    //加载必要组件
    public override void LoadTreasureDiggingBirthPoint()
    {
        if (PosGo) return;
        GameObject posInt = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/TreasureHuntBeginPos", true);
        posInt.name = "[TreasureHuntBeginPos]";
        PosGo = posInt.GetComponent<RoomBirthPoint>();
    }




    //创建房间角色
    public override void CreateUserInfo(RepeatedField<RoomUserInfo> userInfos)
    {
        UserInfos = userInfos;
        foreach (RoomUserInfo userInfo in userInfos)
        {
            CreateUserInfo(userInfo);
        }
    }
    public override void CreateUserInfo(RoomUserInfo userInfo)
    {
        if (Players.ContainsKey(userInfo.UserId)) return;
        if (userInfo.AvatarIds.Count == 0) return;
        if (userInfo.UserId == ManageMentClass.DataManagerClass.userId)
        {
            GameObject go = CharacterManager.Instance().GetPlayerObj();
            PlayerControllerImp imp = AddComp<PlayerControllerImp>(go);
            imp.IsSelf = true;
            imp.SyncEnable(true);
            imp.LookFollowHud = AddHudPanel(go, userInfo, imp.IsSelf, userInfo.NetworkState);
            imp.UserInfo = userInfo;
            imp.playerItem = go.GetComponent<PlayerItem>();
            Players[userInfo.UserId] = imp;
            SetPos(go, userInfo.CurPos);
            //if (userInfo.CurPos == null)
            //    RoomSyncNetView.Ins.SendMove(go.transform.localEulerAngles.y, go.transform.position, 0, 1);
        }
        else
        {
            GameObject go = AvatarManager.Instance().GetOtherPlayerPreFun(userInfo);
            go.name = "player_" + userInfo.UserId;
            CharacterManager.Instance().PlayOtherPlayerSpecialEffect(go);
            PlayerControllerImp imp = CreateControllerImp(userInfo.UserId.ToString());
            imp.playerItem = go.GetComponent<PlayerItem>();
            imp.moveController = go.GetComponent<MoveControllerImp>();
            imp.UserInfo = userInfo;
            imp.IsSelf = false;
            imp.LookFollowHud = AddHudPanel(go, userInfo, imp.IsSelf, userInfo.NetworkState);
            imp.ResetForStart();
            //SetNavigationInfo(imp.playerItem.gameObject);
            Players[userInfo.UserId] = imp;
            if (!go.activeSelf) go.SetActive(true);
            SetPos(go, userInfo.CurPos);
            imp.moveController.StopSyncValue();
        }
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
            }
        }
    }
    public override void UpdateSelfPlayerControllerImp(GameObject go)
    {
        PlayerControllerImp newImp = AddComp<PlayerControllerImp>(go);
        RoomUserInfo newUserInfo = GetSelfUserInfo();
        newImp.IsSelf = true;
        newImp.SyncEnable(true);
        newImp.UserInfo = newUserInfo;
        newImp.LookFollowHud = AddHudPanel(go, newUserInfo, true, newUserInfo.NetworkState);
        newImp.playerItem = go.GetComponent<PlayerItem>();
        SyncPlayerList[ManageMentClass.DataManagerClass.userId] = newImp;
    }
    public override PlayerControllerImp CreateControllerImp(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = PcImpPool.transform;
        go.transform.localPosition = Vector3.zero;
        return go.AddComponent<PlayerControllerImp>();
    }
    protected override void SetPos(GameObject go, Move move = null)
    {
        if (move != null && move.Pos != null)
        {
            if (move.Pos.Y <= -100)
            {
                Transform trans = PosGo.GetPoint(SyncPlayerList.Count - 1);
                go.transform.position = trans.position;
                go.transform.localEulerAngles = trans.localEulerAngles;
            }
            else
            {
                go.transform.position = new Vector3(move.Pos.X / 1000f, move.Pos.Y / 1000f, move.Pos.Z / 1000f);
                go.transform.rotation = Quaternion.Euler(0, move.Dir.Y / 1000f, 0);
            }
        }
        else
        {
            Transform trans = PosGo.GetPoint(SyncPlayerList.Count - 1);
            go.transform.position = trans.position;
            go.transform.localEulerAngles = trans.localEulerAngles;
        }
    }

    public void SetTeamPlayerAnim(string animName)
    {
        foreach (var info in TreasureModel.Instance.TeamUserList)
        {
            if (SyncPlayerList.TryGetValue(info.UserId, out var imp))
            {
                imp.playerItem.SetAnimator(animName);
            }
        }
    }
    public void SetTeamPlayerAnim(ulong userId, string animName)
    {
        if (userId == 0) userId = ManageMentClass.DataManagerClass.userId;
        if (SyncPlayerList.TryGetValue(userId, out var imp))
        {
            imp.playerItem.SetAnimator(animName);
        }
    }
    public void SetTeamPlayerState(uint state)
    {
        TreasureModel.Instance.TeamUserList.ForEach(info => info.State = state);
        foreach (var user in TreasureModel.Instance.TeamUserList)
        {
            if (SyncPlayerList.TryGetValue(user.UserId, out var imp))
            {
                imp.UserInfo = user;
            }
        }
    }
    public RoomUserInfo SetPlayerState(ulong userId, uint state)
    {
        if (userId == 0) userId = ManageMentClass.DataManagerClass.userId;
        RoomUserInfo userInfo = TreasureModel.Instance.TeamUserList.Find(info => info.UserId == userId);
        var imp = SyncPlayerList[userId];
        if (userInfo == null) userInfo = imp.UserInfo;
        userInfo.State = state;
        imp.UserInfo = userInfo;
        return userInfo;
    }
    public void SetSelfMoveCheck(Action cb = null)
    {
        if (cb != null)
        {
            PlayerCtrlManager.Instance().MoveController().SetSendBeforeCheck(cb);
            return;
        }

        PlayerCtrlManager.Instance().MoveController().SetSendBeforeCheck(OnSendSyncCheckHandle);
    }
    public void ClearSelfMoveCheck()
    {
        PlayerCtrlManager.Instance().MoveController().ClearSendBeforeCheck();
    }


    public bool IsSelfTreasuring()
    {
        RoomUserInfo userInfo = GetSelfUserInfo();
        if (userInfo != null) return userInfo.State == (uint)PlayerState.Types.Enum.Treasureing;
        return false;
    }
    public bool IsSelfTreasureReady()
    {
        RoomUserInfo userInfo = GetSelfUserInfo();
        if (userInfo != null) return userInfo.State == (uint)PlayerState.Types.Enum.Ready;
        return false;
    }
    //存储数据
    public static void SaveShowTipTime()
    {
        PlayerPrefs.SetString(PlayerPrefsKey, TreasureModel.Instance.CurTime.ToString());
        PlayerPrefs.Save();
    }


    //还原挖宝流程中刚进入房间，所有玩家此时的状态
    public override void OnReproduceScene()
    {
        //还原房间内所有的状态
        foreach (var imp in SyncPlayerList)
        {
            //Debug.Log($"1111111111   userId:{imp.Value.UserInfo.UserId}  num:{imp.Value.UserInfo.State}");
            ReproduceUserState(imp.Value.UserInfo, imp.Value, true);
        }
        //还原我能看到的圈
        RoomUserInfo userInfo = SyncPlayerList[ManageMentClass.DataManagerClass.userId].UserInfo;
        if (userInfo != null && TreasureModel.Instance.IsTreasureOpen())
            OpenCircleExcept(userInfo.CircleIndex);
    }
    public override void ReproduceUserState(RoomUserInfo userInfo, PlayerControllerImp imp = null, bool igRefCircle = false)
    {
        if (userInfo == null)
        {
            Debug.LogError("1111111  self  userInfo  is  null ");
            return;
        }
        PlayerState.Types.Enum state = (PlayerState.Types.Enum)userInfo.State;
        if (imp == null) imp = SyncPlayerList[userInfo.UserId];
        imp.UserInfo = userInfo;

        switch (state)
        {
            case PlayerState.Types.Enum.Idle:
                if (imp.IsSelf) ClearSelfMoveCheck();
                if (imp.IsSelf) CloseUI(FormConst.TREASUREREADYPANEL);
                if (imp.playerItem.IsPlaying("W_Done")) Debug.Log("11111111 Idle  play anim  W_Done");
                else if (!imp.playerItem.IsPlaying("Idle")) imp.playerItem.SetAnimator("Idle");
                break;
            case PlayerState.Types.Enum.None:
            case PlayerState.Types.Enum.Grouping:
            case PlayerState.Types.Enum.Invited:
                if (imp.IsSelf) ClearSelfMoveCheck();
                if (imp.IsSelf) CloseUI(FormConst.TREASUREREADYPANEL);
                if (!imp.playerItem.IsPlaying("Idle")) imp.playerItem.SetAnimator("Idle");
                break;
            case PlayerState.Types.Enum.Ready:
                //打开准备UI  显示倒计时
                if (imp.IsSelf) OpenUI(FormConst.TREASUREREADYPANEL);
                if (imp.IsSelf) SetSelfMoveCheck(OnReadyMoveCheckHandle);
                if (!imp.playerItem.IsPlaying("W_Idle")) imp.playerItem.SetAnimator("W_Idle");
                break;
            case PlayerState.Types.Enum.Treasureing:
                //打开准备UI  显示挖宝进度
                if (imp.IsSelf) OpenUI(FormConst.TREASUREREADYPANEL);
                if (imp.IsSelf) SetSelfMoveCheck();
                if (!imp.playerItem.IsPlaying("W_Mine")) imp.playerItem.SetAnimator("W_Mine");
                break;
        }

        //还原我能看到的圈
        if (igRefCircle) return;
        if (imp.IsSelf && TreasureModel.Instance.IsTreasureOpen())
            OpenCircleExcept(userInfo.CircleIndex);
    }
    public override void ReproduceUserState(ulong userId, bool igRefCircle = true)
    {
        if (TryGetPlayerImp(userId, out var pcImp))
        {
            ReproduceUserState(pcImp.UserInfo, pcImp, igRefCircle);
        }
    }
    public void ReproduceSelfUserOnLeaveTeam()
    {
        //设置状态
        RoomUserInfo userInfo = SetPlayerState(0, (uint)PlayerState.Types.Enum.Idle);

        //关闭挖宝准备界面
        CloseUI(FormConst.TREASUREREADYPANEL);

        //清除拦截
        ClearSelfMoveCheck();

        //还原状态
        ReproduceUserState(userInfo);
    }
    public void CheckPartnerUpdateUI(RoomUserInfo userInfo)
    {
        ulong userId = userInfo.UserId;
        if (TreasureModel.Instance.TeamUserList.Exists(ui => ui.UserId == userId))
        {
            //UI表现更新
            MessageCenter.SendMessage(UITreasureReadyPanel.Event_OnTreasureStart, KeyValuesUpdate.Empty);
        }
    }
    public RepeatedField<RoomUserInfo> GetTreasurRoomInfo()
    {
        return UserInfos;

    }
}
