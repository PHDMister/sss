using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;

public class InvitePushItem : BaseUIForm
{
    public Image m_ImgAvatar;
    public Text m_NameText;
    public GameObject m_SelectObj;
    private InvitePushData m_InvitePlayerData;
    public enum Sex
    {
        Boy = 1,
        Girl = 2,
    }

    void Awake()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (m_InvitePlayerData == null)
                return;
            SendMessage("InvitePushItemClick", "Click", m_InvitePlayerData);
        });
    }

    public void SetInvitePlayerData(InvitePushData data)
    {
        m_InvitePlayerData = data;
    }

    public InvitePushData GetInvitePlayerData()
    {
        return m_InvitePlayerData;
    }

    public void SetName(string name)
    {
        string shortName = TextTools.setCutAddString(name, 8, "...");
        m_NameText.text = string.Format("{0}", shortName);
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

    public void SetSelected(bool bSelected)
    {
        m_SelectObj.SetActive(bSelected);
    }
}
