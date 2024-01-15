using System.Collections;
using System.Collections.Generic;
using System.Security;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class LookFollowHud : MonoBehaviour
{
    public const string TipTmp = "与[{0}]组队挖宝效率会更高哦~";
    public const string Event_ShowTip = "LookFollowHud_Event_ShowTip";

    public float lookSpeep = 60;
    public Color SelfColor;
    public Color OtherColor;
    private Camera Main;
    private Transform Icon;
    private Text text;
    private string textCache = "";
    private bool iconEnable = false;
    private bool isSelf = false;
    private Transform Tip;
    private Text TipTxt;
    private Timer Timer = null;

    private Transform m_ChatBubble;
    private RectTransform m_ChatBgRect;
    private TMPro.TextMeshProUGUI m_ChatTMP;
    private RectTransform m_ChatTMPRect;
    private float Max_Line_Width = 360f;
    private float hideTime = 5f;
    private float m_HideTimer;

    // Start is called before the first frame update
    protected void Start()
    {
        Main = Camera.main;
        Icon = transform.Find("Text/Image");
        text = transform.Find("Text").GetComponent<Text>();
        text.text = TextTools.setCutAddString(textCache, 8, "...");
        text.color = isSelf ? SelfColor : OtherColor;
        Icon.gameObject.SetActive(iconEnable);
        Tip = transform.Find("Tip");
        TipTxt = transform.Find("Tip/Text").GetComponent<Text>();
        Tip.gameObject.SetActive(false);

        //聊天气泡
        GameObject rootObj = this.gameObject;
        m_ChatBubble = rootObj.transform.Find("ChatBubble");
        m_ChatBubble.gameObject.SetActive(false);
        m_ChatBgRect = UnityHelper.FindTheChildNode(rootObj, "BubbleBg").GetComponent<RectTransform>();
        m_ChatTMP = UnityHelper.FindTheChildNode(rootObj, "BubbleText").GetComponent<TMPro.TextMeshProUGUI>();
        m_ChatTMPRect = m_ChatTMP.GetComponent<RectTransform>();

        m_ChatTMPRect.anchorMin = new Vector2(0f, 0.5f);
        m_ChatTMPRect.anchorMax = new Vector2(0f, 0.5f);
        m_ChatTMPRect.pivot = new Vector2(0, 0.5f);
        m_ChatTMPRect.anchoredPosition = new Vector2(40f, 0);

        m_ChatBgRect.anchorMin = new Vector2(0.5f, 0.5f);
        m_ChatBgRect.anchorMax = new Vector2(0.5f, 0.5f);
        m_ChatBgRect.pivot = new Vector2(0, 0.5f);
        m_ChatBgRect.anchoredPosition = new Vector2(-100f, -100f);
    }

    // Update is called once per frame
    protected void Update()
    {
        m_HideTimer += Time.deltaTime;
        if(m_HideTimer >= hideTime)
        {
            HideChatBubble();
            m_HideTimer = 0;
        }

        if (Main == null)
        {
            Main = Camera.main;
            if (Main == null) return;
        }

        Vector3 offset = Main.transform.position - transform.position;
        offset.y = 0;
        Quaternion origiRota = Quaternion.LookRotation(offset);
        transform.rotation = Quaternion.Slerp(transform.rotation, origiRota, lookSpeep * Time.deltaTime);

        if (isSelf && TreasuringController.EnterCircleShowTip)
        {
            if (TreasureModel.Instance.TeamUserList.Count < 2) return;
            GameObject mCurCharacterObj = CharacterManager.Instance().GetPlayerObj();
            Vector3 localPos = mCurCharacterObj.transform.localPosition;
            if (localPos.x < 5 && localPos.y < 0.1f)
            {
                TreasuringController.EnterCircleShowTip = false;
                TreasuringController.SaveShowTipTime();

                Tip.gameObject.SetActive(true);
                string name = GetPartnerName();
                TipTxt.text = string.Format(TipTmp, name);
                Timer = Timer.RegisterRealTimeNoLoop(5, () =>
                {
                    Timer = null;
                    if (Tip) Tip.gameObject.SetActive(false);
                });
            }
        }
    }

    protected void OnDestroy()
    {
        Timer?.Cancel();
        Timer = null;
        Main = null;
    }

    public void SetPlayerName(string name, bool isSelf)
    {
        textCache = string.IsNullOrEmpty(name) ? isSelf ? "我" : "玩家" : TextTools.setCutAddString(name, 8, "...");
        this.isSelf = isSelf;

        if (text) text.text = TextTools.setCutAddString(name, 8, "...");
        if (text) text.color = isSelf ? SelfColor : OtherColor;
    }

    public void SetNetOfflinkEnable(bool enable)
    {
        iconEnable = enable;
        if (Icon) Icon.gameObject.SetActive(enable);
    }

    public void SetActive(bool enable)
    {
        this.gameObject.SetActive(enable);
    }

    public void ChangeParent(Transform parent)
    {
        this.transform.parent = parent;
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }

    public void ChangeParent(GameObject go, string node)
    {
        Transform parent = go.transform.Find(node);
        this.transform.parent = parent;
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }

    private string GetPartnerName()
    {
        if (TreasureModel.Instance.TeamUserList.Count < 2) return "";
        RoomUserInfo partner = TreasureModel.Instance.TeamUserList.Find(user => user.UserId != ManageMentClass.DataManagerClass.userId);
        if (partner != null) return TextTools.setCutAddString(partner.UserName, 8, "...");
        return "";
    }

    /// <summary>
    /// 玩家聊天气泡
    /// </summary>
    /// <param name="data"></param>
    public void SetChatBubble(ChatData data)
    {
        m_HideTimer = 0;
        m_ChatBubble.gameObject.SetActive(true);
        if (!data.bNotice)
        {
            m_ChatTMP.text = data.msg;
            ContentSizeFitter m_ContentSizeFitter = m_ChatTMP.GetComponent<ContentSizeFitter>();
            if (m_ChatTMP.preferredWidth <= Max_Line_Width)
            {
                m_ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                m_ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_ChatBgRect);
                m_ChatBgRect.sizeDelta = new Vector2(m_ChatTMP.preferredWidth + 80f, 121f);
            }
            else
            {
                m_ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                m_ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_ChatBgRect);
                m_ChatTMP.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, m_ChatTMP.preferredHeight);
                m_ChatBgRect.sizeDelta = new Vector2(421f, 121f);
            }
        }
    }

    public void HideChatBubble()
    {
        if (m_ChatBubble.gameObject.activeSelf)
        {
            m_ChatBubble.gameObject.SetActive(false);
        }
    }
}
