using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class TreasureDiggingInviteTeamList : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_InviteVerticalScroll;
    public Toggle m_Toggle_Friend;
    public Toggle m_Toggle_Near;
    public Text m_Toggle_Friend_Txt;
    public Text m_Toggle_Near_Txt;
    public GameObject m_NoneFriendObj;
    public GameObject m_NoneNearObj;

    private List<InviteTeamItem> m_NearInviteTeamItems = new List<InviteTeamItem>();
    private List<InviteTeamItem> m_FriendInviteItems = new List<InviteTeamItem>();
    private Timer m_OptInviteNearTimer;
    private Timer m_OptInviteFriendTimer;

    public enum PageType
    {
        Friend = 1,
        Near = 2,
    }
    public PageType m_PageType;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        m_NoneFriendObj.SetActive(false);
        m_NoneNearObj.SetActive(false);

        m_Toggle_Friend.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Friend;
                SetPageToggleTextColor(m_PageType);
                ShowFriendInviteList();
            }
        });

        m_Toggle_Near.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Near;
                SetPageToggleTextColor(m_PageType);
                ShowNearInviteList();
            }
        });

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnInviteFirend", p =>
        {
            SwitchToFriendListPage();
        });

        ReceiveMessage("InviteJoinTeamRespCb", p =>
         {
             object[] param = p.Values as object[];
             if (param == null || param.Length < 2)
                 return;
             int inviteType = (int)param[0];
             ulong inviteId = (ulong)param[1];
             if (inviteType == (int)TreasureDiggingInviteTeamList.PageType.Near)
             {
                 SetNearPlayerInvited(inviteId);
                 if (!TreasureModel.Instance.OptInviteNearList.Exists((x => (ulong)x.UserId == inviteId)))
                 {
                     RoomUserInfo data = TreasureModel.Instance.UserList.ToList().Find(x => (ulong)x.UserId == inviteId);
                     if (data != null)
                     {
                         data.State = (int)PlayerState.Types.Enum.Invited;
                         TreasureModel.Instance.OptInviteNearList.Add(data);
                         m_OptInviteNearTimer = Timer.RegisterRealTimeNoLoop(30f, OnOptInviteNearComplete);
                     }
                 }
             }
             else
             {
                 SetFriendPlayerInvited(inviteId);
                 if (!TreasureModel.Instance.OptInviteFriendList.Exists((x => (ulong)x.user_id == inviteId)))
                 {
                     TreasureFriendListRecData data = TreasureModel.Instance.FriendList.Find(x => (ulong)x.user_id == inviteId);
                     if (data != null)
                     {
                         data.state = (int)PlayerState.Types.Enum.Invited;
                         TreasureModel.Instance.OptInviteFriendList.Add(data);
                         m_OptInviteFriendTimer = Timer.RegisterRealTimeNoLoop(30f, OnOptInviteFriendComplete);
                     }
                 }
             }
         });

        ReceiveMessage("OtherUserStatePush", p =>
         {
             object[] param = p.Values as object[];
             ulong userId = (ulong)param[0];
             int pageType = (int)param[1];
             if (pageType == (int)PageType.Near)
             {
                 CancelNearTimer();
             }
             else
             {
                 CancelFrriendTimer();
             }
         });

        ReceiveMessage("UpdatePlayerName", p =>
        {
            Debug.Log("UpdatePlayerName    newName = " + p.Values.ToString() + " m_PageType  = " + (int)m_PageType);
            if (m_PageType == PageType.Friend)
            {
                SetFriendInviteTeamListInfo();
            }
            else
            {
                ShowNearInviteList();
            }
        });
    }

    private void OnOptInviteFriendComplete()
    {
        TreasureModel.Instance.OptInviteFriendList.Clear();
        ShowFriendInviteList();
    }

    private void OnOptInviteNearComplete()
    {
        TreasureModel.Instance.OptInviteNearList.Clear();
        MessageManager.GetInstance().RequestRoomInfo((roomInfoResp) =>
        {
            TreasureModel.Instance.UserList = roomInfoResp.RoomInfo.UserList;
            ShowNearInviteList();
        });
    }

    private void SwitchToFriendListPage()
    {
        m_Toggle_Friend.isOn = true;
        m_Toggle_Near.isOn = false;

        m_PageType = PageType.Friend;
        SetPageToggleTextColor(m_PageType);
        ShowFriendInviteList();
    }

    private void SetPageToggleTextColor(PageType pageType)
    {
        switch (pageType)
        {
            case PageType.Friend:
                if (m_Toggle_Friend_Txt != null)
                    m_Toggle_Friend_Txt.color = new Color(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
                if (m_Toggle_Near_Txt != null)
                    m_Toggle_Near_Txt.color = new Color(80f / 255f, 255f / 255f, 251f / 255f, 229f / 255f);
                break;
            case PageType.Near:
                if (m_Toggle_Friend_Txt != null)
                    m_Toggle_Friend_Txt.color = new Color(80f / 255f, 255f / 255f, 251f / 255f, 229f / 255f);
                if (m_Toggle_Near_Txt != null)
                    m_Toggle_Near_Txt.color = new Color(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
                break;
        }
    }

    public void ShowFriendInviteList()
    {
        m_NoneFriendObj.SetActive(true);
        MessageManager.GetInstance().RequestFriendList((p) =>
        {
            TreasureModel.Instance.FriendList = p;
            SetFriendInviteTeamListInfo();
        });
    }

    public void ShowNearInviteList()
    {
        SetNearInviteTeamListInfo();
    }


    public void SetFriendInviteTeamListInfo()
    {
        if (TreasureModel.Instance.FriendList == null)
        {
            m_NoneFriendObj.SetActive(true);
            m_NoneNearObj.SetActive(false);
            return;
        }
        m_FriendInviteItems.Clear();
        int count = TreasureModel.Instance.FriendList.Count;
        m_InviteVerticalScroll.Init(InitFriendListInfoCallBack);
        m_InviteVerticalScroll.ShowList(count);
        m_InviteVerticalScroll.ResetScrollRect();
        m_NoneFriendObj.SetActive(count <= 0);
        m_NoneNearObj.SetActive(false);
    }

    public void InitFriendListInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        cell.SetActive(true);
        InviteTeamItem inviteTeamItem = cell.transform.GetComponent<InviteTeamItem>();
        if (inviteTeamItem != null)
        {
            TreasureFriendListRecData data = TreasureModel.Instance.FriendList[index - 1];
            if (data == null)
                return;
            inviteTeamItem.SetName(data.login_name);
            inviteTeamItem.SetID(data.code);
            inviteTeamItem.SetSex(data.gender);
            inviteTeamItem.SetAvatar(data.user_pic_url);
            inviteTeamItem.SetTag(data.age, data.constell);
            TreasureFriendListRecData cacheData = TreasureModel.Instance.OptInviteFriendList.Find(x => x.user_id == data.user_id);
            if (cacheData != null)
            {
                int state = cacheData.state;
                inviteTeamItem.SetInviteState(state);
            }
            else
            {
                inviteTeamItem.SetInviteState(data.state);
            }

            RoomUserInfo optCacheData = TreasureModel.Instance.OptInviteNearList.Find(x => (int)x.UserId == data.user_id);//既是好友又是附近的人
            if (optCacheData != null)
            {
                int state = (int)optCacheData.State;
                inviteTeamItem.SetInviteState(state);
            }

            inviteTeamItem.SetPageType((int)m_PageType);
            inviteTeamItem.SetInviteFriendData(data);
            m_FriendInviteItems.Add(inviteTeamItem);
        }
    }

    public void SetNearInviteTeamListInfo()
    {
        if (TreasureModel.Instance.UserList == null)
        {
            m_NoneNearObj.SetActive(true);
            m_NoneFriendObj.SetActive(false);
            return;
        }
        //筛除自己数据
        for (int i = TreasureModel.Instance.UserList.Count - 1; i >= 0; i--)
        {
            if (TreasureModel.Instance.UserList[i].UserId == ManageMentClass.DataManagerClass.userId)
            {
                TreasureModel.Instance.UserList.Remove(TreasureModel.Instance.UserList[i]);
            }
        }
        m_NearInviteTeamItems.Clear();
        int count = TreasureModel.Instance.UserList.Count;
        m_InviteVerticalScroll.Init(InitNearListInfoCallBack);
        m_InviteVerticalScroll.ShowList(count);
        m_InviteVerticalScroll.ResetScrollRect();
        m_NoneNearObj.SetActive(count <= 0);
        m_NoneFriendObj.SetActive(false);
    }

    public void InitNearListInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        InviteTeamItem inviteTeamItem = cell.transform.GetComponent<InviteTeamItem>();
        if (inviteTeamItem != null)
        {
            RoomUserInfo data = TreasureModel.Instance.UserList[index - 1];
            if (data == null)
                return;
            inviteTeamItem.SetInvitePlayerData(data);
            inviteTeamItem.SetName(data.UserName);
            inviteTeamItem.SetID(data.Code);
            inviteTeamItem.SetSex(data.Gender);
            inviteTeamItem.SetAvatar(data.PicUrl);
            inviteTeamItem.SetTag(data.Age, data.Constell);
            inviteTeamItem.SetPageType((int)m_PageType);
            TreasureFriendListRecData cacheData = TreasureModel.Instance.OptInviteFriendList.Find(x => (ulong)x.user_id == data.UserId);
            if (cacheData != null)//既是好友又是附近的人
            {
                int state = cacheData.state;
                inviteTeamItem.SetInviteState(state);
            }
            else
            {
                RoomUserInfo optCacheData = TreasureModel.Instance.OptInviteNearList.Find(x => x.UserId == data.UserId);
                if (optCacheData != null)
                {
                    int state = (int)optCacheData.State;
                    inviteTeamItem.SetInviteState(state);
                }
                else
                {
                    int inviteState = (int)PlayerState.Types.Enum.None;
                    if (data.Invites.Count > 0)
                    {
                        inviteState = (int)PlayerState.Types.Enum.Invited;
                    }
                    else
                    {
                        inviteState = (int)data.State;
                    }
                    inviteTeamItem.SetInviteState(inviteState);
                }
            }
            m_NearInviteTeamItems.Add(inviteTeamItem);
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        SetDefaultSelectPage();
        ShowFriendInviteList();
    }

    public void SetDefaultSelectPage()
    {
        m_PageType = PageType.Friend;
        m_Toggle_Friend.isOn = true;
        m_Toggle_Near.isOn = false;
        SetPageToggleTextColor(m_PageType);
    }
    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        SetDefaultSelectPage();
    }

    public void SetNearPlayerInvited(ulong playerId)
    {
        foreach (var item in m_NearInviteTeamItems)
        {
            if (item.GetInvitePlayerData() != null)
            {
                if (item.GetInvitePlayerData().UserId == playerId)
                {
                    item.SetInviteState((int)PlayerState.Types.Enum.Invited);
                }
            }
        }
    }

    public void SetFriendPlayerInvited(ulong playerId)
    {
        foreach (var item in m_FriendInviteItems)
        {
            if (item.GetInviteFriendData() != null)
            {
                if ((ulong)item.GetInviteFriendData().user_id == playerId)
                {
                    item.SetInviteState((int)PlayerState.Types.Enum.Invited);
                }
            }
        }
    }

    private void CancelNearTimer()
    {
        m_OptInviteNearTimer?.Cancel();
        m_OptInviteNearTimer = null;

    }

    public void CancelFrriendTimer()
    {
        m_OptInviteFriendTimer?.Cancel();
        m_OptInviteFriendTimer = null;
    }
}
