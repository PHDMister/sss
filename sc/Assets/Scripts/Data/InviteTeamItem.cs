using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;

public class InviteTeamItem : BaseUIForm
{
    public Button m_BtnInvite;
    public Image m_ImgSex;
    public Image m_ImgAvatar;
    public Text m_NameText;
    public Text m_IdText;
    public Text m_TagText;
    public Transform m_InvitingTrans;
    public Transform m_InvitedTrans;
    public Transform m_ReadyTrans;
    public Transform m_TreasureingTrans;
    public RectTransform m_NameRect;

    private TreasureFriendListRecData m_InviteFriendData;
    private RoomUserInfo m_InvitePlayerData;
    private int m_PageType;
    public enum Sex
    {
        Boy = 1,
        Girl = 2,
    }

    void Awake()
    {
        m_BtnInvite.onClick.AddListener(() =>
        {
            if (m_PageType == (int)TreasureDiggingInviteTeamList.PageType.Friend)
            {
                if (m_InviteFriendData == null)
                    return;
                MessageManager.GetInstance().SendInviteFriendReq(ManageMentClass.DataManagerClass.userId, (ulong)m_InviteFriendData.user_id);
            }
            else
            {
                if (m_InvitePlayerData == null)
                    return;
                MessageManager.GetInstance().SendInviteJoinTeamReq(ManageMentClass.DataManagerClass.userId, m_InvitePlayerData.UserId, m_InvitePlayerData.RoomId);
            }
        });
    }
    public void SetInvitePlayerData(RoomUserInfo data)
    {
        m_InvitePlayerData = data;
    }

    public RoomUserInfo GetInvitePlayerData()
    {
        return m_InvitePlayerData;
    }

    public void SetID(string id)
    {
        m_IdText.text = string.Format("ID:{0}", id);
    }

    public void SetName(string name)
    {
        string shortName = TextTools.setCutAddString(name, 8, "...");
        m_NameText.text = string.Format("{0}", shortName);
    }

    public void SetSex(string sex)
    {
        if (string.IsNullOrEmpty(sex))
        {
            m_ImgSex.gameObject.SetActive(false);
            return;
        }

        m_ImgSex.gameObject.SetActive(true);
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
        //SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Common");
        //Sprite sprite = atlas.GetSprite(spriteName);
        //m_ImgSex.sprite = sprite;

        Sprite sprite = Resources.Load<Sprite>(string.Format("UIRes/UISprite/Common/{0}", spriteName));
        if (sprite != null)
        {
            m_ImgSex.sprite = sprite;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_NameRect);
    }

    public void SetAvatar(string avatar)
    {
        if (string.IsNullOrEmpty(avatar))
        {
            Sprite sprite = InterfaceHelper.GetDefaultAvatarFun();
            m_ImgAvatar.sprite = sprite;
            return;
        }
        MessageManager.GetInstance().DownLoadAvatar(avatar, (sprite) =>
        {
            m_ImgAvatar.sprite = sprite;
        });
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

    /// <summary>
    /// 0 可邀请 1 已邀请 2 组队中
    /// </summary>
    /// <param name="state"></param>
    public void SetInviteState(int state)
    {
        switch (state)
        {
            case (int)PlayerState.Types.Enum.None:
                m_BtnInvite.gameObject.SetActive(false);
                m_InvitedTrans.gameObject.SetActive(false);
                m_InvitingTrans.gameObject.SetActive(false);
                m_ReadyTrans.gameObject.SetActive(false);
                m_TreasureingTrans.gameObject.SetActive(false);
                break;
            case (int)PlayerState.Types.Enum.Idle://空闲
                m_BtnInvite.gameObject.SetActive(true);
                m_InvitedTrans.gameObject.SetActive(false);
                m_InvitingTrans.gameObject.SetActive(false);
                m_ReadyTrans.gameObject.SetActive(false);
                m_TreasureingTrans.gameObject.SetActive(false);
                break;
            case (int)PlayerState.Types.Enum.Invited://已邀请
                m_BtnInvite.gameObject.SetActive(false);
                m_InvitedTrans.gameObject.SetActive(true);
                m_InvitingTrans.gameObject.SetActive(false);
                m_ReadyTrans.gameObject.SetActive(false);
                m_TreasureingTrans.gameObject.SetActive(false);
                break;
            case (int)PlayerState.Types.Enum.Grouping://组队中
                m_BtnInvite.gameObject.SetActive(false);
                m_InvitedTrans.gameObject.SetActive(false);
                m_InvitingTrans.gameObject.SetActive(true);
                m_ReadyTrans.gameObject.SetActive(false);
                m_TreasureingTrans.gameObject.SetActive(false);
                break;
            case (int)PlayerState.Types.Enum.Ready://挖宝准备中
                m_BtnInvite.gameObject.SetActive(false);
                m_InvitedTrans.gameObject.SetActive(false);
                m_InvitingTrans.gameObject.SetActive(false);
                m_ReadyTrans.gameObject.SetActive(true);
                m_TreasureingTrans.gameObject.SetActive(false);
                break;
            case (int)PlayerState.Types.Enum.Treasureing://挖宝中
                m_BtnInvite.gameObject.SetActive(false);
                m_InvitedTrans.gameObject.SetActive(false);
                m_InvitingTrans.gameObject.SetActive(false);
                m_ReadyTrans.gameObject.SetActive(false);
                m_TreasureingTrans.gameObject.SetActive(true);
                break;
        }
    }

    public void SetPageType(int pageType)
    {
        m_PageType = pageType;
    }

    public void SetInviteFriendData(TreasureFriendListRecData data)
    {
        m_InviteFriendData = data;
    }

    public TreasureFriendListRecData GetInviteFriendData()
    {
        return m_InviteFriendData;
    }
}
