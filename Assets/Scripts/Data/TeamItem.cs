using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;

public class TeamItem : BaseUIForm
{
    public Image m_ImgAvatar;
    public Text m_NameText;
    public GameObject m_EmptyObj;
    public GameObject m_PlayerObj;
    private RoomUserInfo m_RoomUserInfo;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void SetUserName(string name)
    {
        m_NameText.text = TextTools.setCutAddString(name, 8, "..."); ;
    }

    public void SetUserNameColor(bool bSelf)
    {
        m_NameText.color = bSelf ? new Color(255f / 255f, 242f / 255f, 113f / 255f) : new Color(255f / 255f, 255f / 255f, 255f / 255f);
    }

    public void SetUserAvatar(RoomUserInfo roomUserInfoData)
    {
        m_RoomUserInfo = roomUserInfoData;
        string avatarUrl = roomUserInfoData.PicUrl;
        if (string.IsNullOrEmpty(avatarUrl))
        {
            Sprite sprite = InterfaceHelper.GetDefaultAvatarFun();
            m_ImgAvatar.sprite = sprite;
            return;
        }
        MessageManager.GetInstance().DownLoadAvatar(avatarUrl, (sprite) =>
         {
             m_ImgAvatar.sprite = sprite;
         });
    }



    public void SetPlayerVisable(bool bVisable)
    {
        m_PlayerObj.SetActive(bVisable);
    }

    public void SetEmptyState(bool bEmpty)
    {
        m_EmptyObj.SetActive(bEmpty);
        m_PlayerObj.SetActive(false);
    }

    public void SetUserData(RoomUserInfo data)
    {
        m_RoomUserInfo = data;
    }

    public void SetEnterVisiable(bool bVisiable)
    {
        //m_BtnEnter.gameObject.SetActive(bVisiable);
    }
}
