using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TreasureDiggingMainMenu : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_TeamVerticalScroll;
    public Text m_TextGas;
    public Text m_TextTicket;
    public Text m_TextRoomName;
    public Image m_ImageAct;
    public Image m_ImageRed;
    public Button m_BtnArrow;
    public Button m_BtnInvite;
    public Button m_BtnLeave;
    public GameObject m_BtnsObj;
    public Button m_BtnShow;
    public Button m_BtnHide;
    private bool bShowTeam = true;
    private item m_SelectActItemData;
    private List<RoomUserInfo> m_TeamUserList = new List<RoomUserInfo>();
    private bool bOtherRoomInvited = false;//是否其他房间邀请进来的
    private float lastSendActionTime = 0;

    private float timer = 0f;
    private float clickInterval = 1f;//换形象商城点击间隔
    private bool bShopClicked = false;
    private bool bSetClicked = false;
    private bool bShopCanClick = true;
    private bool bSetCanClick = true;
    private bool bClickRoomList = false;
    private int requestNum = 0;
    private bool bFirstGuide = false;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnShop", p =>
        {
            bShopClicked = true;
            timer = 0;
            if (!bShopCanClick)
            {
                return;
            }
            if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
            {
                ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
                return;
            }
            OpenUIForm(FormConst.SHOPNEWUIFORM);
        });

        RigisterButtonObjectEvent("BtnChangeImg", p =>
        {
            bSetClicked = true;
            timer = 0;
            if (!bSetCanClick)
            {
                return;
            }
            if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
            {
                ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
                return;
            }
            OpenUIForm(FormConst.PERSONALDATAPANEL);
            SendMessage("OpenPersonDataPanelRefreshUI", "Success", (ulong)0);
        });

        RigisterButtonObjectEvent("BtnRecord", p =>
         {
             if (PlayerPrefs.GetInt("RecordRedDot") > 0)
             {
                 PlayerPrefs.SetInt("RecordRedDot", 0);
                 SetRecordRedDotState();
             }
             OpenUIForm(FormConst.TREASUREDIGGINGRECORD);
         });


        RigisterButtonObjectEvent("BtnChangeAct", p =>
        {
            if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
            {
                ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
                return;
            }
            GameObject actButton = UnityHelper.FindTheChildNode(this.gameObject, "BtnAction").gameObject;
            if (actButton != null)
            {
                actButton.gameObject.SetActive(false);
            }
            OpenUIForm(FormConst.SETACTION_UIFORM);
        });

        RigisterButtonObjectEvent("BtnAction", p =>
        {
            if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
            {
                ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
                return;
            }
            if (Singleton<TreasuringController>.Instance.IsSelfTreasureReady())
            {
                ToastManager.Instance.ShowNewToast("当前挖宝准备中，请稍后再试~", 3);
                return;
            }
            if (m_SelectActItemData != null)
            {
                int itemId = m_SelectActItemData.item_id;
                animation m_Animation = ManageMentClass.DataManagerClass.GetAnimationTableFun(itemId);
                if (m_Animation != null)
                {
                    string actClipName = m_Animation.animation_model;
                    PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
                    if (playerItem != null)
                    {
                        if (playerItem.IsHaveState(actClipName))
                        {
                            OnPlaySelectAct(actClipName);
                            //在挖宝房间内发送消息同步
                            if (ManageMentClass.DataManagerClass.SceneID == 4 
                                || ManageMentClass.DataManagerClass.SceneID == 1
                                 && Time.realtimeSinceStartup - lastSendActionTime > 3)
                            {
                                lastSendActionTime = Time.realtimeSinceStartup;
                                DoActionReq doAction = new DoActionReq();
                                doAction.Index = WebSocketAgent.Ins.NetView.GetCode;
                                doAction.UserId = ManageMentClass.DataManagerClass.userId;
                                doAction.Action = (uint)itemId;
                                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.DoActionReq, doAction);
                            }
                        }
                        else
                        {
                            ToastManager.Instance.ShowNewToast(string.Format("暂未拥有{0}动作，敬请期待！", m_Animation.animation_name), 5f);
                        }
                    }
                }
            }
        });

        RigisterButtonObjectEvent("BtnReturn", p =>
        {
            OnClickReturn();
        });

        RigisterButtonObjectEvent("BtnHelp", p =>
         {
             OpenUIForm(FormConst.DIGFORTREASUREHELP);
         });

        RigisterButtonObjectEvent("BtnExpand", p =>
         {
             bClickRoomList = true;
             if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
             {
                 ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
                 return;
             }
             MessageManager.GetInstance().SendRoomList();
         });

        m_BtnArrow.onClick.AddListener(OnClickArrow);
        m_BtnInvite.onClick.AddListener(OnClickInvite);
        m_BtnLeave.onClick.AddListener(OnClickLeave);
        m_BtnHide.onClick.AddListener(OnClickHideOtherPlayer);
        m_BtnShow.onClick.AddListener(OnClickShowOtherPlayer);
        m_BtnHide.gameObject.SetActive(true);
        m_BtnShow.gameObject.SetActive(false);

        ReceiveMessage("UpdataGasValue", p =>
         {
             m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
         });

        ReceiveMessage("SetActionItemSuccess", p =>
        {
            m_SelectActItemData = p.Values as item;
            if (m_SelectActItemData != null)
            {
                SetItemIcon(m_SelectActItemData.item_icon);
            }
        });

        ReceiveMessage("SetActionClose", p =>
        {
            GameObject actButton = UnityHelper.FindTheChildNode(this.gameObject, "BtnAction").gameObject;
            if (actButton != null)
            {
                actButton.gameObject.SetActive(true);
            }
        });

        ReceiveMessage("TreasureLoadEnd", p =>
        {
            object[] arg = p.Values as object[];
            if (arg != null)
            {
                if (arg.Length > 1)
                {
                    uint errorCode = (uint)arg[0];
                    bool bRoomFull = (bool)arg[1];
                    if (errorCode == 230011)
                    {
                        if (!bRoomFull)
                        {
                            ToastManager.Instance.ShowNewToast("对方队伍已满", 5f);
                        }
                        else
                        {
                            ToastManager.Instance.ShowNewToast("对方房间人数已满，已为你分配新的房间", 5f);
                        }
                    }
                }
                else
                {
                    uint errorCode = (uint)arg[0];
                    if (errorCode == 230009)
                    {
                        ToastManager.Instance.ShowNewToast("对方房间人数已满，已为你分配新的房间", 5f);
                    }
                }
            }
            bOtherRoomInvited = true;
            SetRoomInfo();
            //切换房间断开连接等回调回来再初始化聊天室建立连接
            ChatMgr.Instance.DisconnectChatRoom();
        });

        ReceiveMessage("TreasureExchangeTicket", p =>
        {
            MessageManager.GetInstance().RequestGetTicketCount(() =>
            {
                m_TextTicket.text = ManageMentClass.DataManagerClass.ticket.ToString();
            });
        });

        ReceiveMessage("TreasureCostTicket", p =>
         {
             MessageManager.GetInstance().RequestGetTicketCount(() =>
             {
                 m_TextTicket.text = ManageMentClass.DataManagerClass.ticket.ToString();
             });
         });

        ReceiveMessage("RefreshPlayerName", p =>
        {
            ManageMentClass.DataManagerClass.selfPersonData.login_name = p.Values.ToString();
            string newName = p.Values.ToString();
            Debug.Log("RefreshPlayerName    newName = " + newName);

            foreach (var player in Singleton<TreasuringController>.Instance.Players)
            {
                if (player.Key == ManageMentClass.DataManagerClass.userId)
                {
                    player.Value.UserInfo.UserName = newName;
                    player.Value.LookFollowHud.SetPlayerName(newName, true);
                    break;
                }
            }
            
            ChatMgr.Instance.ImUpdateUserInfo();

            foreach (var player in TreasureModel.Instance.RoomData.UserList)
            {
                if (player.UserId == ManageMentClass.DataManagerClass.userId)
                {
                    player.UserName = newName;
                    break;
                }
            }

            foreach (var player in TreasureModel.Instance.UserList)
            {
                if (player.UserId == ManageMentClass.DataManagerClass.userId)
                {
                    player.UserName = newName;
                    break;
                }
            }

            foreach (var player in TreasureModel.Instance.TeamUserList)
            {
                if (player.UserId == ManageMentClass.DataManagerClass.userId)
                {
                    player.UserName = newName;
                    break;
                }
            }

            foreach (var player in TreasureModel.Instance.FriendList)
            {
                if ((ulong)player.user_id == ManageMentClass.DataManagerClass.userId)
                {
                    player.login_name = newName;
                    break;
                }
            }

            KeyValuesUpdate kvs = new KeyValuesUpdate("", newName);
            MessageCenter.SendMessage("UpdatePlayerName", kvs);
        });

        ReceiveMessage("UpdatePlayerName", p =>
        {
            Debug.Log("TreasureDiggingMainMenu    newName = " + p.Values.ToString());
            SortTeamList();
            SetTeamInfo();
        });
        
        ReceiveMessage("OpenChatUI", p =>
        {
            openChatUI();
        });

        RegistMsgEvent();
    }

    private void RegistMsgEvent()
    {
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.CreateRoomResp, OnCreateRoomResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.EnterTreasureResp, OnEnterTreasureResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RoomListResp, OnRoomListResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.JoinRoomResp, OnJoinRoomResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherInviteJoinTeamPush, OnOtherInviteJoinTeamPush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherInvitePush, OnOtherInvitePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RefuseInvitePush, OnRefuseInvitePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RefuseNearInvitePush, OnRefuseNearInvitePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.TeamUpdatePush, OnTeamUpdatePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherEnterTreasurePush, OnOtherEnterTreasurePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherLeaveTreasurePush, OnOtherLeaveTreasurePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.NoticePush, OnNoticePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RewardPush, OnRewardPush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.OtherUserStatePush, OnOtherUserStatePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.FriendStatePush, OnFriendStatePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.StartTreasurePush, OnTreasureStartPush);
    }

    private void UnRegistMsgEvent()
    {
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.CreateRoomResp, OnCreateRoomResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.EnterTreasureResp, OnEnterTreasureResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.RoomListResp, OnRoomListResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.JoinRoomResp, OnJoinRoomResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.OtherInviteJoinTeamPush, OnOtherInviteJoinTeamPush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.OtherInvitePush, OnOtherInvitePush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.TeamUpdatePush, OnTeamUpdatePush);
    }
    /// <summary>
    /// 创建房间返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnCreateRoomResp(uint clientCode, ByteString data)
    {
        CreateRoomResp createRoomResp = CreateRoomResp.Parser.ParseFrom(data);
        if (createRoomResp.StatusCode == 230015)
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }

        if (createRoomResp.Room == null || createRoomResp.Room.UserList == null)
            return;

        ManageMentClass.DataManagerClass.roomId = createRoomResp.Room.RoomId;
        TreasureModel.Instance.RoomData.ChatRoomId = createRoomResp.Room.ChatRoomId;
        TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
        TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(createRoomResp.Room.ChatRoomAddr);
        
        MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

        //角色刷新
        TreasureModel.Instance.UpdateRoomPlayer(createRoomResp.Room.UserList, true);
        //队伍刷新
        TreasureModel.Instance.RemoveTeamUserListExceptSelf(createRoomResp.Room.RoomId);
        UpdateTeamUserInfoState(createRoomResp.Room.UserList);
        m_TeamUserList = TreasureModel.Instance.TeamUserList;
        SortTeamList();
        SetTeamInfo();

        UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGROOMLIST);
        TreasureLoadManager.Instance().Load(LoadType.Create);
        SetRoomInfo();
        
        MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);
    }

    /// <summary>
    /// 进入大厅返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnEnterTreasureResp(uint clientCode, ByteString data)
    {
        EnterTreasureResp enterTreasureResp = EnterTreasureResp.Parser.ParseFrom(data);
        ManageMentClass.DataManagerClass.roomId = enterTreasureResp.RoomInfo.RoomId;
        TreasureModel.Instance.RoomData = enterTreasureResp.RoomInfo;
        TreasureModel.Instance.UserList = enterTreasureResp.RoomInfo.UserList;
        SetRoomInfo();
    }
    /// <summary>
    /// 房间列表返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnRoomListResp(uint clientCode, ByteString data)
    {
        RoomListResp roomListResp = RoomListResp.Parser.ParseFrom(data);
        if (bClickRoomList)
        {
            if (!UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGROOMLIST))
            {
                OpenUIForm(FormConst.TREASUREDIGGINGROOMLIST);
                MessageManager.GetInstance().SendMessage("RoomListResp", "Success", roomListResp);
            }
            bClickRoomList = false;
        }

        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGROOMLIST))
        {
            TreasureModel.Instance.bReqRoomList = true;
            TreasureDiggingRoomList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGROOMLIST) as TreasureDiggingRoomList;
            if (uiForm != null)
            {
                uiForm.UpdateRoomData(roomListResp);
                uiForm.SetRoomListInfo(roomListResp);
            }
        }
    }
    /// <summary>
    /// 加入房间返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnJoinRoomResp(uint clientCode, ByteString data)
    {
        JoinRoomResp joinRoomResp = JoinRoomResp.Parser.ParseFrom(data);
        if (joinRoomResp.StatusCode == 230015)
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }

        ManageMentClass.DataManagerClass.roomId = joinRoomResp.RoomId;
        TreasureModel.Instance.RoomData.ChatRoomId = joinRoomResp.ChatRoomId;
        TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
        TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(joinRoomResp.ChatRoomAddr);
        
        MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

        //队伍刷新
        TreasureModel.Instance.RemoveTeamUserListExceptSelf(joinRoomResp.RoomId);
        m_TeamUserList = TreasureModel.Instance.TeamUserList;
        SortTeamList();
        SetTeamInfo();

        //角色刷新
        TreasureModel.Instance.UpdateRoomPlayer(joinRoomResp.UserList, true);
        UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGROOMLIST);
        TreasureLoadManager.Instance().Load(LoadType.Join);
        UpdateTeamUserInfoState(joinRoomResp.UserList);

        SetRoomInfo();
        
        MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);
    }

    /// <summary>
    /// 附近邀请推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnOtherInviteJoinTeamPush(uint clientCode, ByteString data)
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            //挖宝中禁掉邀请弹框
            return;
        }
        OtherInviteJoinTeamPush otherInvitePush = OtherInviteJoinTeamPush.Parser.ParseFrom(data);
        InvitePushData invitePushData = new InvitePushData((int)TreasureDiggingInviteTeamList.PageType.Near, otherInvitePush, CalcTools.GetTimeStamp());
        if (!TreasureModel.Instance.InvitePushList.Exists((x) => x.PushData.FromUserInfo.UserId == otherInvitePush.FromUserInfo.UserId))
        {
            TreasureModel.Instance.InvitePushList.Add(invitePushData);
        }
        if (!UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
        {
            OpenUIForm(FormConst.TREASUREDIGGINGINVITEPUSH);
        }
        else
        {
            TreasureDiggingInvitePush pushUIFrom = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITEPUSH) as TreasureDiggingInvitePush;
            if (pushUIFrom != null)
            {
                pushUIFrom.UpdateInviteList();
            }
        }
    }

    /// <summary>
    /// 好友邀请推送
    /// </summary>
    /// <param name="cientCode"></param>
    /// <param name="data"></param>
    private void OnOtherInvitePush(uint cientCode, ByteString data)
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            //挖宝中禁掉邀请弹框
            return;
        }

        OtherInvitePush otherInvitePush = OtherInvitePush.Parser.ParseFrom(data);
        OtherInviteJoinTeamPush pushData = new OtherInviteJoinTeamPush();
        pushData.FromUserInfo = otherInvitePush.FromUserInfo;
        pushData.ToUserId = otherInvitePush.ToUserId;
        TreasureModel.Instance.InviteFromUserInfo = pushData.FromUserInfo;
        InvitePushData invitePushData = new InvitePushData((int)TreasureDiggingInviteTeamList.PageType.Friend, pushData, CalcTools.GetTimeStamp());
        if (!TreasureModel.Instance.InvitePushList.Exists((x) => x.PushData.FromUserInfo.UserId == otherInvitePush.FromUserInfo.UserId))
        {
            TreasureModel.Instance.InvitePushList.Add(invitePushData);
        }
        if (!UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
        {
            OpenUIForm(FormConst.TREASUREDIGGINGINVITEPUSH);
        }
        else
        {
            TreasureDiggingInvitePush pushUIFrom = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITEPUSH) as TreasureDiggingInvitePush;
            if (pushUIFrom != null)
            {
                pushUIFrom.UpdateInviteList();
            }
        }
    }

    /// <summary>
    /// 好友拒绝推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnRefuseInvitePush(uint clientCode, ByteString data)
    {
        RefuseInvitePush refuseInvitePush = RefuseInvitePush.Parser.ParseFrom(data);
        ToastManager.Instance.ShowNewToast(string.Format("【{0}】已拒绝了你的组队邀请", TextTools.setCutAddString(refuseInvitePush.FromUserName, 8, "...")), 3f);
        //邀请状态刷新
        for (int i = TreasureModel.Instance.OptInviteFriendList.Count - 1; i >= 0; i--)
        {
            if ((ulong)TreasureModel.Instance.OptInviteFriendList[i].user_id == refuseInvitePush.FromUserId)
            {
                TreasureModel.Instance.OptInviteFriendList.RemoveAt(i);
                break;
            }
        }
        UpdateFriendPlayerState(refuseInvitePush);
    }

    /// <summary>
    /// 附近玩家拒绝推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnRefuseNearInvitePush(uint clientCode, ByteString data)
    {
        RefuseNearInvitePush refuseInvitePush = RefuseNearInvitePush.Parser.ParseFrom(data);
        ToastManager.Instance.ShowNewToast(string.Format("【{0}】已拒绝了你的组队邀请", TextTools.setCutAddString(refuseInvitePush.FromUserName, 8, "...")), 3f);

        //记录的邀请状态刷新
        for (int i = TreasureModel.Instance.OptInviteNearList.Count - 1; i >= 0; i--)
        {
            if ((ulong)TreasureModel.Instance.OptInviteNearList[i].UserId == refuseInvitePush.FromUserId)
            {
                TreasureModel.Instance.OptInviteNearList.RemoveAt(i);
                break;
            }
        }

        //刷新拒绝人的状态
        MessageManager.GetInstance().RequestRoomInfo((roomInfoResp) =>
        {
            RoomUserInfo userInfo = roomInfoResp.RoomInfo.UserList.ToList().Find((x) => x.UserId == refuseInvitePush.FromUserId);
            if (userInfo != null)
            {
                for (int i = 0; i < TreasureModel.Instance.UserList.Count; i++)
                {
                    if (TreasureModel.Instance.UserList[i].UserId == refuseInvitePush.FromUserId)
                    {
                        TreasureModel.Instance.UserList[i].State = userInfo.State;
                        break;
                    }
                }
            }
            //邀请状态刷新
            UpdateNearPlayerState(refuseInvitePush);
        });


    }

    /// <summary>
    /// 队伍更新推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnTeamUpdatePush(uint clientCode, ByteString data)
    {
        TeamInfoResp teamUpdatePush = TeamInfoResp.Parser.ParseFrom(data);
        TreasureModel.Instance.TeamUserList = teamUpdatePush.List.ToList();
        if (teamUpdatePush.List.Count > m_TeamUserList.Count)//加入
        {
            for (int i = 0; i < teamUpdatePush.List.Count; i++)
            {
                if (!m_TeamUserList.Exists(x => x.UserId == teamUpdatePush.List[i].UserId))
                {
                    if (!bOtherRoomInvited)
                    {
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITEPUSH);
                        }

                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }

                        ToastManager.Instance.ShowNewToast(string.Format("你已与[{0}]组队", TextTools.setCutAddString(teamUpdatePush.List[i].UserName, 8, "...")), 5f);
                        AddTeamUpdateListener();
                        break;
                    }
                    else
                    {
                        bOtherRoomInvited = false;
                    }
                }
            }
        }
        else if (teamUpdatePush.List.Count < m_TeamUserList.Count)//退出队伍
        {
            List<RoomUserInfo> newRoomUserInfo = teamUpdatePush.List.ToList();
            if (!CheckTreasureState(newRoomUserInfo))
            {
                ToastManager.Instance.ShowNewToast("队伍已解散", 5f);
                AddTeamUpdateListener();
            }
        }

        m_TeamUserList = teamUpdatePush.List.ToList();
        SortTeamList();
        SetTeamInfo();
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
        {
            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
        }
    }

    private bool CheckTreasureState(List<RoomUserInfo> teamUpdatePush)
    {
        RoomUserInfo userInfo = teamUpdatePush.Find(user => user.UserId == ManageMentClass.DataManagerClass.userId);
        if (userInfo != null && userInfo.State == 5 && teamUpdatePush.Count == 1)
        {
            OpenUIFormCheckOpen(FormConst.UITREASUREPARTNERLEAVETIP);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 他人加入房间推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnOtherEnterTreasurePush(uint clientCode, ByteString data)
    {
        OtherEnterTreasurePush enterTreasurePush = OtherEnterTreasurePush.Parser.ParseFrom(data);
        if (!TreasureModel.Instance.UserList.ToList().Exists(x => x.UserId == enterTreasurePush.NewUserInfo.UserId))
        {
            TreasureModel.Instance.UserList.Add(enterTreasurePush.NewUserInfo);
        }

        SetRoomInfo();
        UpdateNearPlayerList();
        UpdateSelfRoomListInfo();
    }


    /// <summary>
    /// 他人离开房间推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnOtherLeaveTreasurePush(uint clientCode, ByteString data)
    {
        OtherLeaveTreasurePush otherLeaveTreasurePush = OtherLeaveTreasurePush.Parser.ParseFrom(data);
        for (int i = TreasureModel.Instance.UserList.Count - 1; i >= 0; i--)
        {
            if (TreasureModel.Instance.UserList[i].UserId == otherLeaveTreasurePush.FromUserId)
            {
                TreasureModel.Instance.UserList.Remove(TreasureModel.Instance.UserList[i]);
            }
        }

        SetRoomInfo();
        UpdateNearPlayerList();
        UpdateSelfRoomListInfo();
    }

    /// <summary>
    /// 奖励消息推送
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnNoticePush(uint clientCode, ByteString data)
    {
        NoticePush noticePush = NoticePush.Parser.ParseFrom(data);
        ChatData chatData = new ChatData();
        chatData.fromAccId = string.Format("system_{0}", CalcTools.GetTimeStamp());
        chatData.fromNick = "系统通知";
        chatData.msg = noticePush.Msg;
        chatData.bNotice = true;
        chatData.flow = "in";
        ChatMgr.Instance.AddChatMsg(ChatType.All, chatData.fromAccId, chatData);

        KeyValuesUpdate kvs = new KeyValuesUpdate("", null);
        MessageCenter.SendMessage("NoticeMsgPush", kvs);

        if (!TreasureModel.Instance.bRecNoticePush)
        {
            Singleton<NoticeController>.Instance.Init();
            TreasureModel.Instance.bRecNoticePush = true;
        }
        Singleton<NoticeController>.Instance.AddMessage(noticePush.Msg);
    }

    private void OnRewardPush(uint clientCode, ByteString data)
    {
        MessageManager.GetInstance().RequestGasValue();
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGRECORD))
        {
            TreasureDiggingRecord uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGRECORD) as TreasureDiggingRecord;
            if (uiForm != null)
            {
                if (uiForm.GetPageType() == TreasureDiggingRecord.PageType.Record)
                {
                    PlayerPrefs.SetInt("RecordRedDot", 0);
                    uiForm.ShowRecordList();
                    SetRecordRedDotState();
                }
            }
        }
        else
        {
            PlayerPrefs.SetInt("RecordRedDot", 1);
            SetRecordRedDotState();
        }
    }

    /// <summary>
    /// 房间其他人状态变化同步
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnOtherUserStatePush(uint clientCode, ByteString data)
    {
        OtherUserStatePush otherUserStatePush = OtherUserStatePush.Parser.ParseFrom(data);
        if (TreasureModel.Instance.OptInviteNearList.Exists((x) => x.UserId == otherUserStatePush.FromUserId))
        {
            KeyValuesUpdate kvs = new KeyValuesUpdate("", new object[] { otherUserStatePush.FromUserId, (int)TreasureDiggingInviteTeamList.PageType.Near });
            MessageCenter.SendMessage("OtherUserStatePush", kvs);
        }

        //如果有队伍清除缓存的邀请推送列表里的数据
        for (int i = TreasureModel.Instance.InvitePushList.Count - 1; i >= 0; i--)
        {
            if (TreasureModel.Instance.InvitePushList[i].PushData.FromUserInfo.UserId == otherUserStatePush.FromUserId)
            {
                if (otherUserStatePush.State == (int)PlayerState.Types.Enum.Grouping)
                {
                    TreasureModel.Instance.InvitePushList.RemoveAt(i);
                }
            }
        }

        for (int i = 0; i < TreasureModel.Instance.UserList.Count; i++)
        {
            if (TreasureModel.Instance.UserList[i].UserId == otherUserStatePush.FromUserId)
            {
                TreasureModel.Instance.UserList[i].State = otherUserStatePush.State;
            }
        }

        if (TreasureModel.Instance.FriendList != null)
        {
            for (int i = 0; i < TreasureModel.Instance.FriendList.Count; i++)
            {
                if ((ulong)TreasureModel.Instance.FriendList[i].user_id == otherUserStatePush.FromUserId)
                {
                    TreasureModel.Instance.FriendList[i].state = (int)otherUserStatePush.State;
                }
            }
        }

        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
        {
            TreasureDiggingInviteTeamList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITETEAM) as TreasureDiggingInviteTeamList;
            if (uiForm != null)
            {
                if (uiForm.m_PageType == TreasureDiggingInviteTeamList.PageType.Near)
                {
                    uiForm.ShowNearInviteList();
                }
                else
                {
                    uiForm.ShowFriendInviteList();
                }
            }
        }

        if (TreasureModel.Instance.InvitePushList.Count <= 0)
        {
            if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
            {
                UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITEPUSH);
            }
        }
        else
        {
            if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
            {
                TreasureDiggingInvitePush uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITEPUSH) as TreasureDiggingInvitePush;
                if (uiForm != null)
                {
                    uiForm.m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
                    uiForm.UpdateInviteList();
                }
            }
        }
    }

    /// <summary>
    /// 好友状态刷新
    /// </summary>
    private void OnFriendStatePush(uint clientCode, ByteString data)
    {
        FriendStatePush friendStatePush = FriendStatePush.Parser.ParseFrom(data);
        if (TreasureModel.Instance.OptInviteFriendList.Exists((x) => (ulong)x.user_id == friendStatePush.FromUserId))
        {
            KeyValuesUpdate kvs = new KeyValuesUpdate("", new object[] { friendStatePush.FromUserId, (int)TreasureDiggingInviteTeamList.PageType.Friend });
            MessageCenter.SendMessage("OtherUserStatePush", kvs);
        }

        //如果有队伍清除缓存的邀请推送列表里的数据
        for (int i = TreasureModel.Instance.InvitePushList.Count - 1; i >= 0; i--)
        {
            if (TreasureModel.Instance.InvitePushList[i].PushData.FromUserInfo.UserId == friendStatePush.FromUserId)
            {
                if (friendStatePush.State == (int)PlayerState.Types.Enum.Grouping)
                {
                    TreasureModel.Instance.InvitePushList.RemoveAt(i);
                }
            }
        }

        if (TreasureModel.Instance.FriendList != null)
        {
            for (int i = 0; i < TreasureModel.Instance.FriendList.Count; i++)
            {
                if ((ulong)TreasureModel.Instance.FriendList[i].user_id == friendStatePush.FromUserId)
                {
                    TreasureModel.Instance.FriendList[i].state = (int)friendStatePush.State;
                }
            }
            if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
            {
                TreasureDiggingInviteTeamList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITETEAM) as TreasureDiggingInviteTeamList;
                if (uiForm != null)
                {
                    if (uiForm.m_PageType == TreasureDiggingInviteTeamList.PageType.Friend)
                    {
                        uiForm.SetFriendInviteTeamListInfo();
                    }
                    else
                    {
                        uiForm.SetNearInviteTeamListInfo();
                    }
                }
            }
        }

        if (TreasureModel.Instance.InvitePushList.Count <= 0)
        {
            if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
            {
                UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITEPUSH);
            }
        }
        else
        {
            if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITEPUSH))
            {
                TreasureDiggingInvitePush uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITEPUSH) as TreasureDiggingInvitePush;
                if (uiForm != null)
                {
                    uiForm.m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
                    uiForm.UpdateInviteList();
                }
            }
        }

    }

    private void OnTreasureStartPush(uint clientCode, ByteString data)
    {
        MessageManager.GetInstance().RequestGetTicketCount(() =>
        {
            m_TextTicket.text = ManageMentClass.DataManagerClass.ticket.ToString();
        });
    }

    /// <summary>
    /// 刷新队伍中角色的圈信息
    /// </summary>
    /// <param name="userInfos"></param>
    private void UpdateTeamUserInfoState(RepeatedField<RoomUserInfo> userInfos)
    {
        //查找
        RoomUserInfo selfRoomUser = null;
        foreach (var roomUser in userInfos)
        {
            if (roomUser.UserId == ManageMentClass.DataManagerClass.userId)
            {
                selfRoomUser = roomUser;
                break;
            }
        }

        //更新队伍中的圈
        if (TreasureModel.Instance.TeamUserList != null)
        {
            ulong userId = ManageMentClass.DataManagerClass.userId;
            RoomUserInfo userInfo = TreasureModel.Instance.TeamUserList.Find(tUser => tUser.UserId == userId);
            if (userInfo != null && selfRoomUser != null)
            {
                if (selfRoomUser.CircleIndex != userInfo.CircleIndex)
                    userInfo.CircleIndex = selfRoomUser.CircleIndex;
                if (selfRoomUser.State != userInfo.State)
                    userInfo.State = selfRoomUser.State;
                if (selfRoomUser.YunxinAccid != userInfo.YunxinAccid)
                    userInfo.YunxinAccid = selfRoomUser.YunxinAccid;
            }
            else if (userInfo == null && selfRoomUser != null)
            {
                TreasureModel.Instance.TeamUserList.Add(selfRoomUser);
            }
        }

        foreach (var item in userInfos)
        {
            RoomUserInfo userInfo = TreasureModel.Instance.OptInviteNearList.Find((x => (ulong)x.UserId == item.UserId));
            if (userInfo != null)
            {
                TreasureModel.Instance.OptInviteNearList.Remove(userInfo);
            }

            TreasureFriendListRecData friendInfo = TreasureModel.Instance.OptInviteFriendList.Find((x => (ulong)x.user_id == item.UserId));
            if (friendInfo != null)
            {
                TreasureModel.Instance.OptInviteFriendList.Remove(friendInfo);
            }
        }
    }

    private void OnTreasureStartHandler(KeyValuesUpdate kv)
    {
        MessageManager.GetInstance().RequestGasValue();
    }

    public override void Display()
    {
        base.Display();
        SetCurActIcon();
        SetRoomInfo();
        ShowTeamInfo();
        RequestRewardPreviewData();
        SetRecordRedDotState();
        requestNum = 0;
        StartCoroutine(WaitOpenChat());
    }

    private IEnumerator WaitOpenChat()
    {
        SetGasAndTicket();
        CheckShowGuide();
        while (requestNum < 2)
        {
            yield return null;
        }

        if (!bFirstGuide && !UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGTICKETEXCHANGE))
        {
            openChatUI();
        }
    }

    private void openChatUI()
    {
        if (UIManager.GetInstance().IsOpend(FormConst.UICHAT))
        {
            return;
        }
        OpenUIForm(FormConst.UICHAT);
        var param = new UIChatParam
        {
            ChatTypes = new List<ChatType> { ChatType.All , ChatType.Room, ChatType.Team},
            ChatModel = TreasureModel.Instance,
            Controller = Singleton<TreasuringController>.Instance,
        };
        MessageCenter.SendMessage("OnOpenUIChat", "param", param);
    }

    public void SetGasAndTicket()
    {
        MessageManager.GetInstance().RequestGetTicketCount(() =>
        {
            m_TextTicket.text = ManageMentClass.DataManagerClass.ticket.ToString();
        });
        MessageManager.GetInstance().RequestGasValue(() =>
        {
            m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
            requestNum++;
        });
    }

    public void SetRoomInfo()
    {
        m_TextRoomName.text = string.Format("寻宝空间·{0}", ManageMentClass.DataManagerClass.roomId);
        m_TextRoomName.color = (TreasureModel.Instance.RoomData != null && TreasureModel.Instance.RoomData.UserList.Count >= 6) ? new Color(255f / 255f, 32f / 255f, 95f / 255f) : new Color(63f / 255f, 100f / 255f, 230f / 255f);
    }

    public void SortTeamList()
    {
        if (m_TeamUserList == null)
            return;
        m_TeamUserList.Sort((a, b) =>
        {
            bool bSelfA = a.UserId == ManageMentClass.DataManagerClass.userId;
            bool bSelfB = b.UserId == ManageMentClass.DataManagerClass.userId;
            if (bSelfA.CompareTo(bSelfB) != 0)
            {
                return -(bSelfA.CompareTo(bSelfB));
            }
            return 1;
        });
    }

    public void SetTeamInfo()
    {
        int count = 2;
        m_TeamVerticalScroll.Init(InitTeamInfoCallBack);
        m_TeamVerticalScroll.ShowList(count);
        m_TeamVerticalScroll.ResetScrollRect();
        m_BtnInvite.gameObject.SetActive(m_TeamUserList.Count < 2);
        m_BtnLeave.gameObject.SetActive(m_TeamUserList.Count >= 2);
    }

    public void InitTeamInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        TeamItem teamItem = cell.transform.GetComponent<TeamItem>();
        if (teamItem != null)
        {
            teamItem.SetEmptyState(index > m_TeamUserList.Count && index <= 2);
            if (index <= m_TeamUserList.Count)
            {
                RoomUserInfo roomUserInfo = m_TeamUserList[index - 1];
                if (roomUserInfo != null)
                {
                    teamItem.SetPlayerVisable(true);
                    teamItem.SetUserName(roomUserInfo.UserName);
                    teamItem.SetUserNameColor(roomUserInfo.UserId == ManageMentClass.DataManagerClass.userId);
                    teamItem.SetUserAvatar(roomUserInfo);
                    teamItem.SetUserData(roomUserInfo);
                }
            }
        }
    }

    public override void Hiding()
    {
        base.Hiding();
        //UnRegistMsgEvent();
    }
    void OnClickReturn()
    {
        try
        {
            SetTools.SetPortraitModeFun();
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
    public void SetCurActIcon()
    {
        int m_SelectActId = PlayerPrefs.GetInt("CurUseActId");
        if (m_SelectActId <= 0)
        {
            m_SelectActId = 2003;//默认问候
            PlayerPrefs.SetInt("CurUseActId", m_SelectActId);
        }
        m_SelectActItemData = ManageMentClass.DataManagerClass.GetItemTableFun(m_SelectActId);
        if (m_SelectActItemData != null)
        {
            string spriteName = m_SelectActItemData.item_icon;
            SetItemIcon(spriteName);
        }
    }

    public void SetItemIcon(string spriteName)
    {
        /*var atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
        Sprite sprite = atlas.GetSprite(spriteName);*/
        m_ImageAct.sprite = Resources.Load("UIRes/UISprite/Icon/" + spriteName, typeof(Sprite)) as Sprite;
    }

    public void OnPlaySelectAct(string triggerName)
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator(triggerName);
        }
    }

    public void ShowTeamInfo()
    {
        m_TeamUserList = TreasureModel.Instance.TeamUserList;
        SortTeamList();
        SetTeamInfo();
        SetTeamVisable();
    }

    public void SetTeamVisable()
    {
        m_TeamVerticalScroll.gameObject.SetActive(bShowTeam);
        m_BtnsObj.SetActive(bShowTeam);

        if (bShowTeam)
        {
            m_BtnArrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            m_BtnArrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 180f);
        }
    }

    public void OnClickArrow()
    {
        bShowTeam = !bShowTeam;
        SetTeamVisable();
    }

    /// <summary>
    /// 邀请组队
    /// </summary>
    public void OnClickInvite()
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }
        OpenUIForm(FormConst.TREASUREDIGGINGINVITETEAM);
    }

    /// <summary>
    /// 离开队伍
    /// </summary>
    public void OnClickLeave()
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3f);
            return;
        }
        OpenUIForm(FormConst.TREASUREDIGGINGQUITTEAMTIPS);
        //LeaveTeamReq leaveTeamReq = new LeaveTeamReq();
        //leaveTeamReq.UserId = ManageMentClass.DataManagerClass.userId;

        //WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.LeaveTeamReq, leaveTeamReq, (code, bytes) =>
        //{
        //    if (code != 0) return;
        //    LeaveRoomResp leaveRoomResp = LeaveRoomResp.Parser.ParseFrom(bytes);
        //    if (leaveRoomResp.StatusCode == 0)
        //    {
        //        Debug.Log($"[WebSocket] LeaveRoomResp Success");
        //    }
        //});
    }

    /// <summary>
    /// 新手引导Tips
    /// </summary>
    public void CheckShowGuide()
    {
        CheckBeginLaunchReq req = new CheckBeginLaunchReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.CheckBeginLaunchReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            CheckBeginLaunchResp checkBeginLaunchResp = CheckBeginLaunchResp.Parser.ParseFrom(bytes);
            if (checkBeginLaunchResp.StatusCode == 0)
            {
                if ((int)checkBeginLaunchResp.IsComplete == 0)
                {
                    bFirstGuide = true;
                    OpenUIForm(FormConst.TREASUREGUIDEPANEL);
                }
            }
            requestNum++;
        });
    }
    /// <summary>
    /// 好友邀请状态刷新
    /// </summary>
    /// <param name="refuseInvitePush"></param>
    private void UpdateFriendPlayerState(RefuseInvitePush refuseInvitePush)
    {
        //邀请状态刷新
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
        {
            TreasureDiggingInviteTeamList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITETEAM) as TreasureDiggingInviteTeamList;
            if (uiForm != null)
            {
                uiForm.ShowFriendInviteList();
            }
        }
    }
    /// <summary>
    /// 附近玩家邀请状态刷新
    /// </summary>
    /// <param name="refuseInvitePush"></param>
    public void UpdateNearPlayerState(RefuseNearInvitePush refuseInvitePush)
    {
        //邀请状态刷新
        for (int i = 0; i < TreasureModel.Instance.UserList.Count; i++)
        {
            if ((ulong)TreasureModel.Instance.UserList[i].UserId == refuseInvitePush.FromUserId)
            {
                if (TreasureModel.Instance.UserList[i].Invites.Contains(refuseInvitePush.FromUserId))
                {
                    TreasureModel.Instance.UserList[i].Invites.Remove(refuseInvitePush.FromUserId);
                }
            }
        }
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
        {
            TreasureDiggingInviteTeamList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITETEAM) as TreasureDiggingInviteTeamList;
            if (uiForm != null)
            {
                if (uiForm.m_PageType == TreasureDiggingInviteTeamList.PageType.Near)
                {
                    uiForm.ShowNearInviteList();
                }
            }
        }
    }

    /// <summary>
    /// 更新附近玩家列表
    /// </summary>
    private void UpdateNearPlayerList()
    {
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
        {
            TreasureDiggingInviteTeamList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGINVITETEAM) as TreasureDiggingInviteTeamList;
            if (uiForm != null)
            {
                if (uiForm.m_PageType == TreasureDiggingInviteTeamList.PageType.Near)
                {
                    uiForm.ShowNearInviteList();
                }
            }
        }
    }

    /// <summary>
    /// 更新房间列表状态
    /// </summary>
    private void UpdateSelfRoomListInfo()
    {
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGROOMLIST))
        {
            TreasureDiggingRoomList uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGROOMLIST) as TreasureDiggingRoomList;
            if (uiForm != null)
            {
                RoomListReq roomListReq = new RoomListReq();
                roomListReq.Start = 1;
                roomListReq.PageSize = 1000;

                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.RoomListReq, roomListReq, (code, datas) =>
                {
                    RoomListResp resp = RoomListResp.Parser.ParseFrom(datas);
                    uiForm.UpdateRoomListData(resp);
                    uiForm.SetRoomListInfo(resp);
                });
            }
        }
    }

    /// <summary>
    /// 请求奖励预览数据
    /// </summary>
    private void RequestRewardPreviewData()
    {
        MessageManager.GetInstance().RequestTreasureReward();
    }

    public void SetRecordRedDotState()
    {
        if (m_ImageRed != null)
        {
            int state = PlayerPrefs.GetInt("RecordRedDot");
            m_ImageRed.gameObject.SetActive(state > 0);
        }
    }

    private void Update()
    {
        if (bShopClicked)
        {
            if (timer < clickInterval)
            {
                timer += Time.deltaTime;
                bSetCanClick = false;
            }

            if (timer >= clickInterval)
            {
                timer = 0;
                bSetCanClick = true;
                bShopCanClick = true;
                bShopClicked = false;
            }
        }

        if (bSetClicked)
        {
            if (timer < clickInterval)
            {
                timer += Time.deltaTime;
                bShopCanClick = false;
            }

            if (timer >= clickInterval)
            {
                timer = 0;
                bShopCanClick = true;
                bSetCanClick = true;
                bSetClicked = false;
            }
        }
    }

    public void OnClickHideOtherPlayer()
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }

        m_BtnHide.gameObject.SetActive(false);
        m_BtnShow.gameObject.SetActive(true);
        Singleton<TreasuringController>.Instance.ShowOrHideOtherRoomPlayer(false);
    }

    public void OnClickShowOtherPlayer()
    {
        if (Singleton<TreasuringController>.Instance.IsSelfTreasuring())
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }

        m_BtnHide.gameObject.SetActive(true);
        m_BtnShow.gameObject.SetActive(false);
        Singleton<TreasuringController>.Instance.ShowOrHideOtherRoomPlayer(true);
    }

    public void AddTeamUpdateListener()
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", null);
        MessageCenter.SendMessage("TeamUpdate", kvs);
    }
}
