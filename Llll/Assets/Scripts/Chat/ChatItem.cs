using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class ChatItem : BaseUIForm
{
    public GameObject NormalTrans;
    public GameObject NoticeTrans;
    public TMPro.TextMeshProUGUI m_NormalText;
    public TMPro.TextMeshProUGUI m_NoticeText;

    private TMPro.TextMeshProUGUI m_MsgText;
    private ContentSizeFitter m_TextCSF;
    public float TextMaxWidth = 680;
    public float Space = 15f;//Item间距

    private ChatData m_ChatData;

    private Button Norm_Btn;

    private ulong user_ID;
    private bool isNotice = false;

    private void Awake()
    {
        if (NormalTrans != null)
        {
            Norm_Btn = NormalTrans.GetComponent<Button>();
            if (Norm_Btn != null)
            {
                Norm_Btn.onClick.AddListener(BtnClickFun);
                //  Norm_Btn.enabled = false;
            }
        }
    }
    void BtnClickFun()
    {
        Debug.Log("btnclickFunPutUserID ： ----------------------：  " + user_ID);
        if (user_ID != 0 && user_ID != ManageMentClass.DataManagerClass.userId)
        {
            AvatarManager.Instance().AllOtherPlayerAvatarData.TryGetValue(user_ID, out var avatarData);
            if (avatarData == null)
            {
                ToastManager.Instance.ShowNewToast("玩家不在当前房间内",2f);
                return;
            }
            MessageManager.GetInstance().RequestGetPersonData(() =>
            {
                OpenUIForm(FormConst.PERSONALDATAPANEL);
                SendMessage("OpenPersonDataPanelRefreshUI", "Success", user_ID);
            }, user_ID);
        }
    }
    /// <summary>
    /// 设置用户ID
    /// </summary>
    /// <param name="userID"></param>
    public void SetUserIDFun(ulong userID)
    {
        user_ID = userID;
        Debug.Log("at This get my UserID：：：  --------------------------------  " + user_ID);
        if (userID != 0)
        {
            Norm_Btn.enabled = true;
        }
    }

    public void SetChatData(ChatData data)
    {
        m_ChatData = data;
    }

    public void SetState(bool bNotice)
    {
        isNotice = bNotice;
        NormalTrans.SetActive(!bNotice);
        NoticeTrans.SetActive(bNotice);
        if (!bNotice)
        {
            m_MsgText = m_NormalText;
            m_TextCSF = m_NormalText.GetComponent<ContentSizeFitter>();
        }
        else
        {
            m_MsgText = m_NoticeText;
            m_TextCSF = m_NoticeText.GetComponent<ContentSizeFitter>();
        }
    }

    public void SetText(string msg)
    {
        m_MsgText.text = msg;
        ReCSF();
    }

    void ReCSF()
    {
        m_TextCSF.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        m_TextCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        RectTransform rect = m_TextCSF.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        float newWidth = TextMaxWidth;
        float newHeigh = 0f;

        if (m_ChatData != null && m_ChatData.bNotice)
        {
            if (m_MsgText.preferredWidth + 26f > TextMaxWidth)
            {
                float offsetY = Mathf.Abs(rect.anchoredPosition.y);
                newHeigh = m_MsgText.preferredHeight + offsetY;
            }
            else
            {
                newHeigh = 42f;
            }
            rect.sizeDelta = new Vector2(TextMaxWidth, m_MsgText.preferredHeight);
            GetComponent<RectTransform>().sizeDelta = new Vector2(TextMaxWidth, newHeigh);
        }
        else
        {
            if (m_MsgText.preferredWidth > TextMaxWidth)
            {
                float offsetY = Mathf.Abs(rect.anchoredPosition.y);
                newHeigh = m_MsgText.preferredHeight + offsetY;
            }
            else
            {
                newHeigh = 42f;
            }
            GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, newHeigh);
        }
    }

    public Vector2 GetContentSizeFitterPreferredSize(RectTransform rect, ContentSizeFitter contentSizeFitter)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        return new Vector2(HandleSelfFittingAlongAxis(0, rect, contentSizeFitter),
            HandleSelfFittingAlongAxis(1, rect, contentSizeFitter));
    }

    private float HandleSelfFittingAlongAxis(int axis, RectTransform rect, ContentSizeFitter contentSizeFitter)
    {
        ContentSizeFitter.FitMode fitting =
            (axis == 0 ? contentSizeFitter.horizontalFit : contentSizeFitter.verticalFit);
        if (fitting == ContentSizeFitter.FitMode.MinSize)
        {
            return LayoutUtility.GetMinSize(rect, axis);
        }
        else
        {
            return LayoutUtility.GetPreferredSize(rect, axis);
        }
    }

    public bool IsSingleRow()
    {
        if (m_MsgText != null)
        {
            return m_MsgText.preferredWidth <= TextMaxWidth;
        }
        return true;
    }

    public Vector2 GetItemSize()
    {
        return GetComponent<RectTransform>().sizeDelta;
    }
}
