using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TreasureDiggingInvitePush : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_InviteVerticalScroll;
    public Text m_NameText;
    public Text m_IDText;
    public Image m_SexImg;
    public Image m_AvatorImg;
    public Text m_TagText;
    public Text m_InviteType;
    public Text m_TimerText;
    public RectTransform m_NameRect;

    public InvitePushData m_SelectUserInfo;
    private List<InvitePushItem> m_InvitePushItems = new List<InvitePushItem>();
    private int seconds = 0;
    private InvitePushData m_EarlyUserInfo;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnJoin", p =>
        {
            if (m_SelectUserInfo.PageType == (int)TreasureDiggingInviteTeamList.PageType.Near)
            {
                AgreeJoinTeamReq agreeJoinTeamReq = new AgreeJoinTeamReq();
                agreeJoinTeamReq.FromUserId = m_SelectUserInfo.PushData.FromUserInfo.UserId;
                agreeJoinTeamReq.ToUserId = m_SelectUserInfo.PushData.ToUserId;
                agreeJoinTeamReq.RoomId = m_SelectUserInfo.PushData.RoomId;

                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.AgreeJoinTeamReq, agreeJoinTeamReq, (code, bytes) =>
                {
                    if (code != 0) return;
                    AgreeJoinTeamResp agreeJoinTeamResp = AgreeJoinTeamResp.Parser.ParseFrom(bytes);
                    if (agreeJoinTeamResp.StatusCode == 230010)
                    {
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                        ToastManager.Instance.ShowNewToast("你已在队伍中", 5f);
                        return;
                    }

                    if (agreeJoinTeamResp.StatusCode == 230012)
                    {
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                        ToastManager.Instance.ShowNewToast("对方已不在当前空间~", 5f);
                        return;
                    }

                    if (agreeJoinTeamResp.StatusCode == 230014)
                    {
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                        ToastManager.Instance.ShowNewToast("当前无法接受邀请哦~", 5f);
                        return;
                    }

                    if (agreeJoinTeamResp.StatusCode == 0)
                    {
                        Debug.Log($"[WebSocket] AgreeJoinTeamResp Success");
                        TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                        CloseUIForm();


                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                    }
                });
            }
            else
            {
                AgreeInviteReq agreeInviteReq = new AgreeInviteReq();
                agreeInviteReq.FromUserId = m_SelectUserInfo.PushData.FromUserInfo.UserId;
                agreeInviteReq.ToUserId = m_SelectUserInfo.PushData.ToUserId;

                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.AgreeInviteReq, agreeInviteReq, (code, bytes) =>
                {
                    if (code != 0) return;
                    AgreeInviteResp agreeInviteResp = AgreeInviteResp.Parser.ParseFrom(bytes);

                    if (agreeInviteResp.StatusCode == 230003)
                    {
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                        ToastManager.Instance.ShowNewToast("对方房间已满", 5f);
                        return;
                    }

                    if (agreeInviteResp.StatusCode == 230009 || agreeInviteResp.StatusCode == 230011)
                    {
                        if (agreeInviteResp.Room == null)
                            return;
                        TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }

                        bool bFull = (TreasureModel.Instance.InviteFromUserInfo != null && TreasureModel.Instance.InviteFromUserInfo.RoomId != agreeInviteResp.Room.RoomId);
                        //不同房间邀请
                        if (ManageMentClass.DataManagerClass.roomId != agreeInviteResp.Room.RoomId)
                        {
                            MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

                            TreasureModel.Instance.UpdateRoomPlayer(agreeInviteResp.Room.UserList, true);

                            ManageMentClass.DataManagerClass.roomId = agreeInviteResp.Room.RoomId;
                            TreasureModel.Instance.RoomData.ChatRoomId = agreeInviteResp.Room.ChatRoomId;
                            TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
                            TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(agreeInviteResp.Room.ChatRoomAddr);
                            
                            MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);

                            if (agreeInviteResp.StatusCode == 230009)
                            {
                                object[] args = new object[] { agreeInviteResp.StatusCode };
                                TreasureLoadManager.Instance().Load(LoadType.Join, args);
                            }
                            else if (agreeInviteResp.StatusCode == 230011)
                            {
                                object[] args = new object[] { agreeInviteResp.StatusCode, bFull };
                                TreasureLoadManager.Instance().Load(LoadType.Join, args);
                            }
                        }
                        else
                        {
                            if (agreeInviteResp.StatusCode == 230011)
                            {
                                TreasureModel.Instance.UpdateRoomPlayer(agreeInviteResp.Room.UserList, false);
                                object[] args = new object[] { agreeInviteResp.StatusCode, bFull };
                                MessageManager.GetInstance().SendMessage("TreasureLoadEnd", "Success", args);
                            }
                        }
                    }

                    if (agreeInviteResp.StatusCode == 230014)
                    {
                        CloseUIForm();
                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }
                        ToastManager.Instance.ShowNewToast("当前无法接受邀请哦~", 5f);
                        return;
                    }

                    if (agreeInviteResp.StatusCode == 0)
                    {
                        if (agreeInviteResp.Room == null)
                            return;

                        TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                        CloseUIForm();


                        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGINVITETEAM))
                        {
                            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGINVITETEAM);
                        }

                        //不同房间邀请
                        if (ManageMentClass.DataManagerClass.roomId != agreeInviteResp.Room.RoomId)
                        {
                            TreasureModel.Instance.RoomData.ChatRoomId = agreeInviteResp.Room.ChatRoomId;
                            TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
                            TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(agreeInviteResp.Room.ChatRoomAddr);
                            
                            MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

                            TreasureModel.Instance.UpdateRoomPlayer(agreeInviteResp.Room.UserList, true);
                            ManageMentClass.DataManagerClass.roomId = agreeInviteResp.Room.RoomId;

                            TreasureLoadManager.Instance().Load(LoadType.Join);
                            
                            MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);
                        }
                        else
                        {
                            TreasureModel.Instance.UpdateRoomPlayer(agreeInviteResp.Room.UserList, false);
                        }
                    }
                });
            }
        });

        RigisterButtonObjectEvent("BtnRefuse", p =>
        {
            if (m_SelectUserInfo.PageType == (int)TreasureDiggingInviteTeamList.PageType.Near)
            {
                RefuseNearInviteReq refuseNearInviteReq = new RefuseNearInviteReq();
                refuseNearInviteReq.FromUserId = m_SelectUserInfo.PushData.ToUserId;
                refuseNearInviteReq.ToUserId = m_SelectUserInfo.PushData.FromUserInfo.UserId;

                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.RefuseNearInviteReq, refuseNearInviteReq, (code, bytes) =>
                {
                    if (code != 0) return;
                    RefuseNearInviteResp refuseNearInviteResp = RefuseNearInviteResp.Parser.ParseFrom(bytes);
                    if (refuseNearInviteResp.StatusCode == 0)
                    {
                        Debug.Log($"[WebSocket] RefuseNearInviteResp Success");
                        TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                        if (TreasureModel.Instance.InvitePushList.Count == 0)//多个邀请时不关闭弹框
                        {
                            CloseUIForm();
                        }
                        else
                        {
                            m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
                            SetSelectInviteInfo();
                        }
                        ToastManager.Instance.ShowNewToast(string.Format("你已拒绝了【{0}】的组队邀请", TextTools.setCutAddString(m_SelectUserInfo.PushData.FromUserInfo.UserName, 8, "...")), 5f);
                    }
                });
            }
            else
            {
                RefuseInviteReq refuseInviteReq = new RefuseInviteReq();
                refuseInviteReq.FromUserId = m_SelectUserInfo.PushData.ToUserId;
                refuseInviteReq.ToUserId = m_SelectUserInfo.PushData.FromUserInfo.UserId;

                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.RefuseInviteReq, refuseInviteReq, (code, bytes) =>
                {
                    if (code != 0) return;
                    RefuseInviteResp refuseInviteResp = RefuseInviteResp.Parser.ParseFrom(bytes);
                    if (refuseInviteResp.StatusCode == 0)
                    {
                        Debug.Log($"[WebSocket] RefuseInviteResp Success");
                        TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                        if (TreasureModel.Instance.InvitePushList.Count == 0)//多个邀请时不关闭弹框
                        {
                            CloseUIForm();
                        }
                        else
                        {
                            m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
                            SetSelectInviteInfo();
                        }
                        ToastManager.Instance.ShowNewToast(string.Format("你已拒绝了【{0}】的组队邀请", TextTools.setCutAddString(m_SelectUserInfo.PushData.FromUserInfo.UserName, 8, "...")), 5f);
                    }
                });
            }
        });

        ReceiveMessage("InvitePushItemClick", (p) =>
         {
             InvitePushData data = p.Values as InvitePushData;
             m_SelectUserInfo = data;
             SetSelectInvitePlayerInfo(m_SelectUserInfo.PushData.FromUserInfo);
             SetSelectState();
         });
    }


    public void ShowInviteList()
    {
        if (TreasureModel.Instance.InvitePushList == null)
            return;
        int count = TreasureModel.Instance.InvitePushList.Count;
        m_InviteVerticalScroll.Init(InitInviteListInfoCallBack);
        m_InviteVerticalScroll.ShowList(count);
        m_InviteVerticalScroll.ResetScrollRect();
    }

    public void InitInviteListInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        InvitePushItem invitePushItem = cell.transform.GetComponent<InvitePushItem>();
        if (invitePushItem != null)
        {
            InvitePushData data = TreasureModel.Instance.InvitePushList[index - 1];
            if (data == null)
                return;
            if (data == null || data.PushData == null || data.PushData.FromUserInfo == null)
                return;
            invitePushItem.SetName(data.PushData.FromUserInfo.UserName);
            invitePushItem.SetAvatar(data.PushData.FromUserInfo.PicUrl);
            invitePushItem.SetSelected(m_SelectUserInfo.PushData.FromUserInfo.UserId == data.PushData.FromUserInfo.UserId);
            invitePushItem.SetInvitePlayerData(data);
            if (!m_InvitePushItems.Contains(invitePushItem))
            {
                m_InvitePushItems.Add(invitePushItem);
            }
        }
    }

    public void SetSelectState()
    {
        foreach (var item in m_InvitePushItems)
        {
            if (m_SelectUserInfo.PushData.FromUserInfo.UserId == item.GetInvitePlayerData().PushData.FromUserInfo.UserId)
            {
                item.SetSelected(true);
            }
            else
            {
                item.SetSelected(false);
            }
        }
    }

    public void SetSelectInvitePlayerInfo(RoomUserInfo data)
    {
        m_NameText.text = TextTools.setCutAddString(data.UserName, 8, "...");
        m_IDText.text = string.Format("ID:{0}", data.Code);
        seconds = 30 - (int)(CalcTools.GetTimeStamp() - m_SelectUserInfo.ReceiveTimestamp);
        SetSex(data.Gender);
        SetTag(data.Age, data.Constell);
        SetInviteType(m_SelectUserInfo.PageType);
        SetAvatar(data.PicUrl);
        SetTimer(seconds);
    }

    public void SetSex(string sex)
    {
        if (string.IsNullOrEmpty(sex))
        {
            m_SexImg.gameObject.SetActive(false);
            return;
        }

        m_SexImg.gameObject.SetActive(true);
        string spriteName = "";
        switch (sex)
        {
            case "男":
                spriteName = string.Format("{0}", "icon-boy");
                break;
            case "女":
                spriteName = string.Format("{0}", "icon-gril");
                break;
        }
        Sprite sprite = Resources.Load<Sprite>(string.Format("UIRes/UISprite/Common/{0}", spriteName));
        if (sprite != null)
        {
            m_SexImg.sprite = sprite;
        }

        if (m_NameRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_NameRect);
        }
    }

    private void SetInviteType(int inviteType)
    {
        switch (inviteType)
        {
            case (int)TreasureDiggingInviteTeamList.PageType.Near:
                m_InviteType.text = "附近";
                break;
            case (int)TreasureDiggingInviteTeamList.PageType.Friend:
                m_InviteType.text = "好友";
                break;
        }
    }

    public void SetTag(string age, string constell)
    {
        if (string.IsNullOrEmpty(age) && string.IsNullOrEmpty(constell))
        {
            m_TagText.text = string.Format("{0}", "未知");
            return;
        }
        if (string.IsNullOrEmpty(age) && !string.IsNullOrEmpty(constell))
        {
            m_TagText.text = string.Format("{0}", constell);
        }
        else if (!string.IsNullOrEmpty(age) && string.IsNullOrEmpty(constell))
        {
            m_TagText.text = string.Format("{0}", age);
        }
        else
        {
            m_TagText.text = string.Format("{0} {1}", age, constell);
        }
    }

    public void SetAvatar(string avatar)
    {
        if (string.IsNullOrEmpty(avatar))
        {
            Sprite sprite = InterfaceHelper.GetDefaultAvatarFun();
            m_AvatorImg.sprite = sprite;
        }
        else
        {
            MessageManager.GetInstance().DownLoadAvatar(avatar, (sprite) =>
            {
                m_AvatorImg.sprite = sprite;
            });
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
        SetSelectInviteInfo();
        InvokeRepeating("CheckItemTimer", 0f, 1f);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }

    private void Update()
    {
        seconds = 30 - (int)(CalcTools.GetTimeStamp() - m_SelectUserInfo.ReceiveTimestamp);
        if (seconds >= 0)
        {
            SetTimer(seconds);
        }

        m_EarlyUserInfo = GetEarlyReceiveTimestampItem();
        if (m_EarlyUserInfo != null)
        {
            int earlyTimer = 30 - (int)(CalcTools.GetTimeStamp() - m_EarlyUserInfo.ReceiveTimestamp);
            if (earlyTimer == 0)
            {
                TreasureModel.Instance.RemoveInvitePushList(m_EarlyUserInfo.PushData.FromUserInfo.UserId);
                SetSelectInviteInfo();
            }
        }
    }

    public void SetTimer(int second)
    {
        if (second > 0)
        {
            m_TimerText.text = string.Format("拒绝（{0}s）", seconds);
        }
        else
        {
            if (seconds == 0)
            {
                m_TimerText.text = string.Format("{0}", "拒绝");
                TreasureModel.Instance.RemoveInvitePushList(m_SelectUserInfo.PushData.FromUserInfo.UserId);
                if (TreasureModel.Instance.InvitePushList.Count > 0)//邀请列表还有其他人的邀请
                {
                    m_SelectUserInfo = TreasureModel.Instance.InvitePushList[0];
                    SetSelectInviteInfo();
                }
                else
                {
                    CloseUIForm();
                }
                //ToastManager.Instance.ShowNewToast(string.Format("你已拒绝了【{0}】的组队邀请", TextTools.setCutAddString(m_SelectUserInfo.PushData.FromUserInfo.UserName, 8, "...")), 5f);
            }
        }
    }

    public void UpdateInviteList()
    {
        ShowInviteList();
        SetSelectInvitePlayerInfo(m_SelectUserInfo.PushData.FromUserInfo);
    }

    public void SetSelectInviteInfo()
    {
        ShowInviteList();
        seconds = 30 - (int)(CalcTools.GetTimeStamp() - m_SelectUserInfo.ReceiveTimestamp);
        SetSelectInvitePlayerInfo(m_SelectUserInfo.PushData.FromUserInfo);
        SetTimer(seconds);
    }

    public InvitePushData GetEarlyReceiveTimestampItem()
    {
        TreasureModel.Instance.InvitePushList = TreasureModel.Instance.InvitePushList.OrderBy(t => (t.ReceiveTimestamp)).ToList();
        if (TreasureModel.Instance.InvitePushList.Count > 0)
        {
            return TreasureModel.Instance.InvitePushList.First();
        }
        return null;
    }

    public void CheckItemTimer()
    {
        if (TreasureModel.Instance.InvitePushList == null)
            return;
        for (int i = TreasureModel.Instance.InvitePushList.Count - 1; i >= 0; i--)
        {
            InvitePushData data = TreasureModel.Instance.InvitePushList[i];
            int time = 30 - (int)(CalcTools.GetTimeStamp() - data.ReceiveTimestamp);
            if (time == 0)
            {
                TreasureModel.Instance.InvitePushList.RemoveAt(i);
            }
        }
    }
}
