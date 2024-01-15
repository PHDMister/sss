using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using System.Linq;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.IO;
using SuperScrollView;
using Treasure;

public class UIChat : BaseUIForm
{
    public LoopListView2 m_ScrollViewEx;
    public TMP_InputField m_InputField;
    public Toggle m_ToggleAll;
    public Toggle m_ToggleRoom;
    public Toggle m_ToggleTeam;
    public Text m_ToggleAllText;
    public Text m_ToggleRoomText;
    public Text m_ToggleTeamText;
    public Transform m_EmojiTrans;
    public CircularScrollView.UICircularScrollView m_EmojiScrollView;
    public Transform m_TopTrans;
    public Transform m_CenterTrans;
    public Transform m_LastChatTrans;
    public Image m_ChatUIBg;
    public ChatItem m_LastChatIItem;
    private Button m_SendBtn;
    private Text m_SendTxt;
    private Image m_GasImg;
    private Button m_BtnExtend;
    private Image m_ImgExtend;

    public ChatType m_ChatType;
    private bool bFold = false;
    private int sendInterval = 3;
    private Material grayMate;
    private Material defaultMate;

    private Dictionary<ChatType, int> dicSendMsgTimer = new Dictionary<ChatType, int>();
    private Dictionary<ChatType, float> dicTimer = new Dictionary<ChatType, float>();
    private int Max_Word_Limit = 30;
    private int Cost_Gas_Value = 100;
    private bool bRecBroadcastMsg = false;
    private Dictionary<string, ImUserInfo> dicUserInfo = new Dictionary<string, ImUserInfo>();

    private Dictionary<ChatType, Toggle> chatTypeToggles;

    private IChatModel chatModel;
    private BaseSyncController controller;

    private Dictionary<string, ChatData> msgDic;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Fixed;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        m_SendBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(this.gameObject, "BtnSend");
        m_SendTxt = UnityHelper.GetTheChildNodeComponetScripts<Text>(m_SendBtn.gameObject, "Text");
        m_GasImg = UnityHelper.GetTheChildNodeComponetScripts<Image>(m_SendTxt.gameObject, "Gas");
        m_BtnExtend = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "BtnExtend");
        m_ImgExtend = m_BtnExtend.GetComponent<Image>();

        grayMate = new Material(Shader.Find("UI/Gray"));
        defaultMate = new Material(Shader.Find("UI/Default"));

        m_ToggleAll.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                m_ChatType = ChatType.All;
                SetToggleTextColor(m_ChatType);
                SetChatFoldState(bFold, m_ChatType);
                SetChatPageInfo(m_ChatType);

                if (!dicSendMsgTimer.ContainsKey(m_ChatType))
                {
                    dicSendMsgTimer.Add(m_ChatType, 0);
                }
                SetSendBtnState();
            }
        });

        m_ToggleRoom.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                m_ChatType = ChatType.Room;
                SetToggleTextColor(m_ChatType);
                SetChatFoldState(bFold, m_ChatType);
                SetChatPageInfo(m_ChatType);

                if (!dicSendMsgTimer.ContainsKey(m_ChatType))
                {
                    dicSendMsgTimer.Add(m_ChatType, 0);
                }
                SetSendBtnState();
            }
        });

        m_ToggleTeam.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                m_ChatType = ChatType.Team;
                SetToggleTextColor(m_ChatType);
                SetChatFoldState(bFold, m_ChatType);
                SetChatPageInfo(m_ChatType);

                if (!dicSendMsgTimer.ContainsKey(m_ChatType))
                {
                    dicSendMsgTimer.Add(m_ChatType, 0);
                }
                SetSendBtnState();
            }
        });

        m_SendBtn.onClick.AddListener(() =>
        {
              EventSystem.current.SetSelectedGameObject(m_SendBtn.gameObject);
              if (m_ChatType != ChatType.All)
              {
                  ReqSDKSendMsg();
              }
              else
              {
                  if (PlayerPrefs.GetInt("ChatCostTips") <= 0)
                  {
                      OpenUIForm(FormConst.UICHATCOSTGASTIPS);
                  }
                  else
                  {
                      ReqSDKSendMsg();
                  }
              }
          });

        RigisterButtonObjectEvent("BtnEmoji", p =>
         {
             OnClickOpenEmoji();
         });

        m_BtnExtend.onClick.AddListener(OnClickExtend);

        RigisterButtonObjectEvent("BtnCloseEmoji", p =>
         {
             OnClickCloseEmoji();
         });

        m_EmojiTrans.gameObject.SetActive(false);

        ReceiveMessage("EmojiClicked", p =>
         {
             emoji data = p.Values as emoji;
             string emojiStr = string.Format("<sprite={0}>", data.Index);
             m_InputField.text = string.Format("{0}{1}", m_InputField.text, emojiStr);
             HideEmoji();
         });

        ReceiveMessage("SendAllMsg", p =>
         {
             ReqSDKSendMsg();
         });

        ReceiveMessage("NoticeMsgPush", p =>
        {
            if (m_ChatType == ChatType.All)
            {
                SetChatFoldState(bFold, m_ChatType);
                SetChatPageInfo(m_ChatType);
            }
        });

        ReceiveMessage("TeamUpdate", p =>
        {
            ChatMgr.Instance.m_TeamMsgDic.Clear();
            if (m_ChatType == ChatType.Team)
            {
                SetChatFoldState(bFold, m_ChatType);
                SetChatPageInfo(m_ChatType);
            }
        });


        ReceiveMessage("UpdatePlayerName", p =>
         {
             Debug.Log("UIChat    newName = " + p.Values.ToString());
             SetChatFoldState(bFold, m_ChatType);
             SetChatPageInfo(m_ChatType);
         });

        m_InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        m_InputField.onEndEdit.AddListener(OnInputFieldEndEdit);

        AddListenerIMHandler();

        chatTypeToggles = new Dictionary<ChatType, Toggle>
        {
            { ChatType.All , m_ToggleAll },
            { ChatType.Room, m_ToggleRoom },
            { ChatType.Team, m_ToggleTeam },
        };

        ReceiveMessage("OnOpenUIChat", p =>
        {
            var param = p.Values as UIChatParam;
            if (param == null)
            {
                Debug.LogError("打开聊天需要传递参数");
                return;
            }
            var chatTypes = param.ChatTypes;
            var defaultChatType = chatTypes[0];
            chatModel = param.ChatModel;
            controller = param.Controller;
            foreach (var pair in chatTypeToggles)
            {
                var chatType = pair.Key;
                var toggle = pair.Value;
                toggle.gameObject.SetActive(chatTypes.Contains(chatType));
                toggle.isOn = chatType == defaultChatType;
            }
            setDefaultInfo(defaultChatType);
        });

        ReceiveMessage("HideCurChatBubble", p =>
        {
            hideCurChatBubble();
        });
        
        ReceiveMessage("RefreshUIChat", p =>
        {
            setDefaultInfo(m_ChatType);
        });
        
        ReceiveMessage("CloseChatUI", p =>
        {
            UIManager.GetInstance().CloseUIForms(FormConst.UICHAT);
        });
        
        m_ScrollViewEx.InitListView(0, OnGetItemByIndex);
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 loopView, int index)
    {
        if (index < 0 || index >= msgDic.Count)
        {
            return null;
        }
        
        var item = loopView.NewListViewItem("ChatItem");

        item.gameObject.SetActive(true);
        ulong userID = 0;
        ChatItem m_ChatItem = item.GetComponent<ChatItem>();
        ChatData data = msgDic.ElementAt(index).Value;
        string fromAccId = data.fromAccId;
        string fromNick = data.fromNick;
        string msg = data.msg;
        bool bNotice = data.bNotice;
        if (data.customData != null)
        {
            userID = data.customData.userID;
        }

        if (controller.Players.TryGetValue(ManageMentClass.DataManagerClass.userId, out var imp))
        {
            if (fromAccId.Equals(imp.UserInfo.YunxinAccid)) //自己发送的消息
            {
                UpdateAllChatSelfName(imp);
                fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
                msg = string.Format("<color=#F7EB67>{0}</color><color=#FFFFFF>：{1}</color>",
                    TextTools.setCutAddString(fromNick, 8, "..."), msg);
            }
            else
            {
                msg = string.Format("<color=#01EBFF>{0}</color><color=#FFFFFF>：{1}</color>",
                    TextTools.setCutAddString(fromNick, 8, "..."), msg);
            }
        }

        m_ChatItem.SetChatData(data);
        m_ChatItem.SetState(bNotice);
        m_ChatItem.SetText(msg);
        m_ChatItem.SetUserIDFun(userID);
        
        
        // Vector2 itemSize = m_ChatItem.GetItemSize();
        // return new Vector2(itemSize.x, itemSize.y + m_ChatItem.Space);
        return item;
    }

    private void ReqSDKSendMsg()
    {
        if (!dicSendMsgTimer.ContainsKey(m_ChatType))
        {
            dicSendMsgTimer.Add(m_ChatType, sendInterval);
        }
        else
        {
            if (dicSendMsgTimer[m_ChatType] == 0)
            {
                dicSendMsgTimer[m_ChatType] = sendInterval;
            }
        }
        SetSendBtnState();
        SendMsg(m_ChatType);
    }

    private void OnInputFieldValueChanged(string str)
    {
        Debug.Log("OnInputFieldValueChanged     str = " + str);
        int len = ChatMgr.Instance.CalcuStrLength(str);
        if (len >= Max_Word_Limit)
        {
            m_InputField.text = str.Substring(0, Max_Word_Limit);
            ToastManager.Instance.ShowNewToast("已超出30个字符限制", 3f);
        }
    }

    private void OnInputFieldEndEdit(string str)
    {

    }

    public override void Display()
    {
        base.Display();
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    private void setDefaultInfo(ChatType chatType)
    {
        bFold = true;
        m_ChatType = chatType;
        SetToggleTextColor(m_ChatType);
        SetChatPageInfo(m_ChatType);
        SetChatFoldState(bFold, m_ChatType);

        dicSendMsgTimer[m_ChatType] = 0;
        SetSendBtnState();
    }

    private void ShowMsgInfo()
    {
        m_ScrollViewEx.SetListItemCount(0);
        
        if (!m_CenterTrans.gameObject.activeSelf)
            return;

        Dictionary<string, ChatData> tempMsgDic = null;
        switch (m_ChatType)
        {
            case ChatType.All:
                tempMsgDic = ChatMgr.Instance.m_AllMsgDic;
                break;
            case ChatType.Room:
                var roomId = ManageMentClass.DataManagerClass.roomId;
                ChatMgr.Instance.m_RoomMsgDic.TryGetValue(roomId, out tempMsgDic);
                break;
            case ChatType.Team:
                tempMsgDic = ChatMgr.Instance.m_TeamMsgDic;
                break;
        }

        if (tempMsgDic == null)
        {
            return;
        }

        msgDic = tempMsgDic;
        m_ScrollViewEx.SetListItemCount(msgDic.Count);
        m_ScrollViewEx.MovePanelToItemIndex(msgDic.Count - 1, 0);
    }
    

    private void SendMsg(ChatType chatType)
    {
        string msg = m_InputField.text;
        SetDefalutInputFieldValue(chatType);//重置输入框默认状态

        if (string.IsNullOrEmpty(msg))
        {
            ToastManager.Instance.ShowNewToast("消息内容不能为空！", 3f);
            return;
        }

        CustomData customData = new CustomData();
        customData.userID = ManageMentClass.DataManagerClass.userId;
        customData.projectID = SelfConfigData.projectID;
        customData.roomID = ManageMentClass.DataManagerClass.roomId;
        string data = JsonConvert.SerializeObject(customData);
        Debug.Log("sendMessage Data Value=  " + data);
        msg = string.Format("{0}|{1}", msg, (int)chatType);
        if (chatType == ChatType.Team)//队伍走单聊
        {
            var model = chatModel as TreasureModel;
            if (model.TeamUserList.Count > 1)
            {
                RoomUserInfo partner = model.TeamUserList.Find(user => user.UserId != ManageMentClass.DataManagerClass.userId);
                if (partner != null)
                {
                    ChatMgr.Instance.SendTextMessage(msg, partner.YunxinAccid, data);
                }
            }
            else
            {
                RoomUserInfo selfInfo = model.TeamUserList.Find(user => user.UserId == ManageMentClass.DataManagerClass.userId);
                if (selfInfo != null)
                {
                    ChatMgr.Instance.SendTextMessage(msg, selfInfo.YunxinAccid, data);
                }
            }
        }
        else if (chatType == ChatType.Room)
        {
            ChatMgr.Instance.SendTextMessage(msg, "", data);
        }
        else
        {
        
            if (controller.Players.TryGetValue(ManageMentClass.DataManagerClass.userId, out var imp))
            {
                string sendPlayerName = ManageMentClass.DataManagerClass.selfPersonData.login_name;
                msg = string.Format("{0}:{1}", msg, TextTools.setCutAddString(sendPlayerName, 8, "..."));//塞入发送者名字
            }
        
            ChatMgr.Instance.SendWorldMsg(msg, data, () =>
             {
                 //更新gas数量
                 MessageManager.GetInstance().RequestGasValue();
             });
        }
    }

    private void OnClickOpenEmoji()
    {
        ShowEmoji();
    }

    private void OnClickCloseEmoji()
    {
        HideEmoji();
    }

    private void OnClickExtend()
    {
        bFold = !bFold;
        SetChatFoldState(bFold, m_ChatType);
        SetChatPageInfo(m_ChatType);
    }


    private void SetChatPageInfo(ChatType chatType)
    {
        ShowMsgInfo();
        SetDefalutInputFieldValue(chatType);
    }

    private void SetDefalutInputFieldValue(ChatType chatType)
    {
        TextMeshProUGUI defaultTMP = m_InputField.placeholder.GetComponent<TextMeshProUGUI>();
        switch (chatType)
        {
            case ChatType.All:
                m_InputField.text = string.Empty;
                if (defaultTMP != null)
                {
                    defaultTMP.text = "<color=#FFED4E>全部：</color>点击输入聊天信息";
                }
                break;
            case ChatType.Room:
                m_InputField.text = string.Empty;
                if (defaultTMP != null)
                {
                    defaultTMP.text = "<color=#FFED4E>房间：</color>点击输入聊天信息";
                }
                break;
            case ChatType.Team:
                m_InputField.text = string.Empty;
                if (defaultTMP != null)
                {
                    defaultTMP.text = "<color=#FFED4E>队伍：</color>点击输入聊天信息";
                }
                break;
        }
    }

    private void SetToggleTextColor(ChatType chatType)
    {
        switch (chatType)
        {
            case ChatType.All:
                if (m_ToggleAllText != null)
                    m_ToggleAllText.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                if (m_ToggleRoomText != null)
                    m_ToggleRoomText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                if (m_ToggleTeamText != null)
                    m_ToggleTeamText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                break;
            case ChatType.Room:
                if (m_ToggleAllText != null)
                    m_ToggleAllText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                if (m_ToggleRoomText != null)
                    m_ToggleRoomText.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                if (m_ToggleTeamText != null)
                    m_ToggleTeamText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                break;
            case ChatType.Team:
                if (m_ToggleAllText != null)
                    m_ToggleAllText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                if (m_ToggleRoomText != null)
                    m_ToggleRoomText.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
                if (m_ToggleTeamText != null)
                    m_ToggleTeamText.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                break;
        }
    }

    public void ShowEmoji()
    {
        if (!m_EmojiTrans.gameObject.activeSelf)
        {
            m_EmojiTrans.gameObject.SetActive(true);
        }

        Dictionary<int, emoji> m_EmojiData = ManageMentClass.DataManagerClass.GetEmojiData();
        int count = m_EmojiData.Count;
        m_EmojiScrollView.Init(InitEmojiInfoCallBack);
        m_EmojiScrollView.ShowList(count);
        m_EmojiScrollView.ResetScrollRect();
    }

    public void HideEmoji()
    {
        if (m_EmojiTrans.gameObject.activeSelf)
        {
            m_EmojiTrans.gameObject.SetActive(false);
        }
    }

    private void InitEmojiInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        cell.SetActive(true);
        EmojiItem emojiItem = cell.transform.GetComponent<EmojiItem>();
        emoji data = ManageMentClass.DataManagerClass.GetEmojiTable(index);
        if (data != null)
        {
            emojiItem.SetEmoji(data);
        }
    }

    public void SetChatFoldState(bool bFold, ChatType chatType)
    {
        m_ImgExtend.GetComponent<RectTransform>().rotation = bFold ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
        m_CenterTrans.gameObject.SetActive(!bFold);
        m_LastChatTrans.gameObject.SetActive(bFold);
        SetLastChatInfo(chatType);
        SetChatWindowSize(bFold);
    }

    public void SetChatWindowSize(bool bFold)
    {
        RectTransform rectTransform = m_ChatUIBg.transform.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, -240f);

        RectTransform m_TopRectTrans = m_TopTrans.GetComponent<RectTransform>();
        m_TopRectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        m_TopRectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        m_TopRectTrans.pivot = new Vector2(0.5f, 0.5f);

        RectTransform m_LastRectTrans = m_LastChatTrans.GetComponent<RectTransform>();
        m_LastRectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        m_LastRectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        m_LastRectTrans.pivot = new Vector2(0.5f, 0.5f);

        RectTransform m_LastItemRect = m_LastChatIItem.GetComponent<RectTransform>();
        m_LastItemRect.anchorMin = new Vector2(0f, 1f);
        m_LastItemRect.anchorMax = new Vector2(0f, 1f);
        m_LastItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 680);
        m_LastItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 42);
        m_LastItemRect.anchoredPosition = new Vector2(0, 0);

        if (bFold)
        {
            if (m_LastChatIItem.IsSingleRow())
            {
                rectTransform.sizeDelta = new Vector2(730, 270);

                m_TopRectTrans.anchoredPosition = new Vector2(0, -15f);
                m_TopRectTrans.sizeDelta = new Vector2(680f, 38f);

                m_LastRectTrans.anchoredPosition = new Vector2(0, -79f);
                m_LastRectTrans.sizeDelta = new Vector2(680f, 42f);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(730f, 293f);

                m_TopRectTrans.anchoredPosition = new Vector2(0, 9f);
                m_TopRectTrans.sizeDelta = new Vector2(680f, 38f);

                m_LastRectTrans.anchoredPosition = new Vector2(0, -44f);
                m_LastRectTrans.sizeDelta = new Vector2(680f, 42f);
            }
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(730f, 480f);

            m_TopRectTrans.anchoredPosition = new Vector2(0, 199f);
            m_TopRectTrans.sizeDelta = new Vector2(680f, 38f);
        }
    }

    public void SetLastChatInfo(ChatType chatType)
    {
        bool bNotice = false;
        string msg = string.Empty;
        string fromAccId = string.Empty;
        string fromNick = string.Empty;
        ulong userID = 0;

        ChatData m_ChatData = null;
        switch (chatType)
        {
            case ChatType.All:
                if (ChatMgr.Instance.m_AllMsgDic.Count > 0)
                { 
                    m_ChatData = ChatMgr.Instance.m_AllMsgDic.Values.Last();
                }
                break;
            case ChatType.Team:
                if (ChatMgr.Instance.m_TeamMsgDic.Count > 0)
                { 
                    m_ChatData = ChatMgr.Instance.m_TeamMsgDic.Values.Last();
                }
                break;
            case ChatType.Room:
                if (ChatMgr.Instance.m_RoomMsgDic.Count > 0)
                { 
                    var roomId = ManageMentClass.DataManagerClass.roomId;
                    ChatMgr.Instance.m_RoomMsgDic.TryGetValue(roomId, out var dic);
                    if (dic != null)
                    {
                        m_ChatData = dic.Values.Last();
                    }
                }
                break;
        }

        if (m_ChatData == null)
        {
            m_LastChatTrans.gameObject.SetActive(false);
            return;
        }
        
        msg = m_ChatData.msg;
        bNotice = m_ChatData.bNotice;
        fromAccId = m_ChatData.fromAccId;
        fromNick = m_ChatData.fromNick;
        
        if (m_ChatData.customData != null)
        {
            userID = m_ChatData.customData.userID;
        }
        
        if (controller.Players.TryGetValue(ManageMentClass.DataManagerClass.userId, out var imp))
        {
            if (fromAccId.Equals(imp.UserInfo.YunxinAccid))//自己发送的消息
            {
                UpdateAllChatSelfName(imp);
                fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
                msg = string.Format("<color=#F7EB67>{0}</color><color=#FFFFFF>：{1}</color>", TextTools.setCutAddString(fromNick, 8, "..."), msg);
            }
            else
            {
                msg = string.Format("<color=#01EBFF>{0}</color><color=#FFFFFF>：{1}</color>", TextTools.setCutAddString(fromNick, 8, "..."), msg);
            }
        
        }
        if (m_LastChatIItem != null)
        {
            m_LastChatIItem.SetState(bNotice);
            m_LastChatIItem.SetText(msg);
            m_LastChatIItem.SetUserIDFun(userID);
        }
    }

    private void Update()
    {
        ChatType[] keyArr = dicSendMsgTimer.Keys.ToArray<ChatType>();
        for (int i = 0; i < keyArr.Length; i++)
        {
            if (dicSendMsgTimer[keyArr[i]] > 0)
            {
                if (!dicTimer.ContainsKey(keyArr[i]))
                {
                    dicTimer.Add(keyArr[i], 0);
                }
                dicTimer[keyArr[i]] += Time.deltaTime;
                if (dicTimer[keyArr[i]] >= 1f)
                {
                    dicSendMsgTimer[keyArr[i]] -= 1;
                    dicTimer[keyArr[i]] = 0;
                    if (keyArr[i] == m_ChatType)
                    {
                        SetSendBtnState();
                    }
                }
            }
        }
    }

    public void SetSendBtnState()
    {
        if (dicSendMsgTimer[m_ChatType] > 0)
        {
            m_SendTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 3);
            m_GasImg.gameObject.SetActive(false);

            m_SendBtn.GetComponent<Image>().material = grayMate;
            m_SendBtn.interactable = false;
            m_SendTxt.text = string.Format("发送({0}s)", dicSendMsgTimer[m_ChatType]);
        }
        else
        {
            if (m_ChatType == ChatType.All)
            {
                m_SendTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(11, 3);
                m_GasImg.gameObject.SetActive(true);
                m_SendTxt.text = string.Format("-{0}{1}", Cost_Gas_Value, "发送");

                if (ManageMentClass.DataManagerClass.gas_Amount < Cost_Gas_Value)
                {
                    m_SendBtn.GetComponent<Image>().material = grayMate;
                    m_SendBtn.interactable = false;
                }
                else
                {
                    m_SendBtn.GetComponent<Image>().material = defaultMate;
                    m_SendBtn.interactable = true;
                }
            }
            else
            {
                m_SendTxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 3);
                m_GasImg.gameObject.SetActive(false);
                m_SendTxt.text = string.Format("{0}", "发送");

                m_SendBtn.GetComponent<Image>().material = defaultMate;
                m_SendBtn.interactable = true;
            }
        }
    }
    /// <summary>
    /// IM消息事件监听
    /// </summary>
    public void AddListenerIMHandler()
    {
        ReceiveMessage("OnMessageChange", p =>
         {
             //string jsonStr = "[{'chatroomId':'5964515111', 'idClient':'431e1e50-e205-459c-999a-730c68e69b9e', 'from':'myjmwa9a', 'fromNick':'Doge0011', 'fromAvatar':'https://aiera.oss-cn-shanghai.aliyuncs.com/users/255322749/1687952090990.jpg', 'fromCustom':'', 'userUpdateTime':'1698666820471', 'fromClientType':'Web', 'time':'1698666825877', 'type':'text', 'text':'发广告|2"','resend':'false','status':'success', 'flow':'out' }]";
             string jsonStr = p.Values.ToString();
             //  jsonStr.Replace(@"\"");
             int chatType = 0;
             Debug.Log("UIChat   OnMessageChange   jsonStr = " + jsonStr);
             JArray jsonArry = (JArray)JsonConvert.DeserializeObject(jsonStr);

             if (jsonArry != null)
             {
                 //Debug.Log("UIChat   OnMessageChange   jsonArry Count = " + jsonArry.Count);
                 for (int i = 0; i < jsonArry.Count; i++)
                 {
                     JObject jo = JObject.Parse(jsonArry[i].ToString());
                     //Debug.Log("type: " + jo["type"].ToString() + " text: " + jo["text"].ToString());
                     if (jo["type"].ToString().Equals("text"))
                     {
                         string msg = jo["text"].ToString();
                         //Debug.Log("UIChat   OnMessageChange   text = " + msg);

                         int index = msg.LastIndexOf('|');
                         //Debug.Log("UIChat   OnMessageChange   index = " + index);

                         if (index > 0)
                         {
                             string chatTypeStr = msg.Substring(index + 1);
                             //Debug.Log("UIChat   OnMessageChange   chatTypeStr = " + chatTypeStr + " chatType = " + chatType);

                             int.TryParse(chatTypeStr, out chatType);
                             msg = msg.Substring(0, index);
                         }
                         //Debug.Log("UIChat   OnMessageChange   msg = " + msg + " chatType = " + chatType);

                         string key = jo["idClient"].ToString();
                         ChatData chatData = new ChatData();
                         chatData.fromAccId = jo["from"].ToString();
                         chatData.fromNick = jo["fromNick"] != null ? jo["fromNick"].ToString() : string.Empty;
                         chatData.msg = msg;//jo["text"].ToString();
                         chatData.bNotice = false;
                         chatData.flow = jo["flow"].ToString();
                         if (jo["custom"] != null)
                         {
                             var json = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(jo["custom"].ToString())));
                             chatData.customData = json.ToString().FromJSON<CustomData>();
                         }
                         ChatMgr.Instance.AddChatMsg((ChatType)chatType, key, chatData);

                         //聊天气泡
                         chatModel.ShowChatBubble(chatData, chatType);
                     }
                 }
                 if (m_ChatType == (ChatType)chatType)
                 {
                     SetChatFoldState(bFold, (ChatType)chatType);
                     SetChatPageInfo((ChatType)chatType);
                 }
             }
         });

        ReceiveMessage("OnKicked", p =>
         {
             Debug.Log("UIChat   OnKicked   jsonStr = " + p.Values.ToString());
         });

        ReceiveMessage("OnWillReconnect", p =>
        {
            Debug.Log("UIChat   OnWillReconnect   jsonStr = " + p.Values.ToString());
        });

        ReceiveMessage("OnSendTextMessage", p =>
        {
            Debug.Log("UIChat   OnSendTextMessage   jsonStr = " + p.Values.ToString());
            JObject jo = JObject.Parse(p.Values.ToString());
            //Debug.Log("idClient: " + jo["idClient"].ToString() + "  from: " + jo["from"].ToString() + " fromNick: " + jo["fromNick"].ToString() + " text: " + jo["text"].ToString() + " flow: " + jo["flow"].ToString());

            string msg = jo["text"].ToString();
            //Debug.Log("UIChat   OnSendTextMessage   msg = " + msg);

            int index = msg.LastIndexOf('|');
            //Debug.Log("UIChat   OnSendTextMessage   index = " + index);

            int chatType = 0;
            if (index > 0)
            {
                string chatTypeStr = msg.Substring(index + 1);
                //Debug.Log("UIChat   OnSendTextMessage   chatTypeStr = " + chatTypeStr + " chatType = " + chatType);

                int.TryParse(chatTypeStr, out chatType);
                msg = msg.Substring(0, index);
            }
            //Debug.Log("UIChat   OnSendTextMessage   msg = " + msg + " chatType = " + chatType);

            string key = jo["idClient"].ToString();
            ChatData chatData = new ChatData();
            chatData.fromAccId = jo["from"].ToString();
            chatData.fromNick = jo["fromNick"].ToString();
            chatData.msg = msg;//jo["text"].ToString();
            chatData.bNotice = false;
            chatData.flow = jo["flow"].ToString();
            if (jo["custom"] != null)
            {
                var json = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(jo["custom"].ToString())));
                chatData.customData = json.ToString().FromJSON<CustomData>();
            }

            ChatMgr.Instance.AddChatMsg((ChatType)chatType, key, chatData);
            if (m_ChatType == (ChatType)chatType)
            {
                SetChatFoldState(bFold, (ChatType)chatType);
                SetChatPageInfo((ChatType)chatType);
            }
            //聊天气泡
            chatModel.ShowChatBubble(chatData, chatType);
        });

        ReceiveMessage("OnBroadcastMsg", p =>
        {
            bRecBroadcastMsg = true;
            string jsonStr = p.Values.ToString();
            Debug.Log("UIChat   OnBroadcastMsg   jsonStr = " + jsonStr);
            JArray jsonArry = (JArray)JsonConvert.DeserializeObject(jsonStr);

            if (jsonArry != null)
            {
                //Debug.Log("UIChat   OnBroadcastMsg   jsonArry Count = " + jsonArry.Count);
                for (int i = 0; i < jsonArry.Count; i++)
                {
                    JObject jo = JObject.Parse(jsonArry[i].ToString());
                    //Debug.Log("body: " + jo["body"].ToString() + "broadcastId: " + jo["broadcastId"].ToString() + "fromAccid: " + jo["fromAccid"].ToString() + "fromUid: " + jo["fromUid"].ToString() + "timestamp: " + jo["timestamp"].ToString());

                    var body = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(jo["body"].ToString())));
                    var bodyJObject = JObject.Parse(body.ToJSON());
                    var msg = bodyJObject["msg"].ToString();
                    //Debug.Log("UIChat   OnBroadcastMsg   msg = " + msg);

                    int nameSplitIdx = msg.LastIndexOf(':');
                    string name = string.Empty;
                    if (nameSplitIdx > 0)
                    {
                        name = msg.Substring(nameSplitIdx + 1);
                        msg = msg.Substring(0, nameSplitIdx);
                    }
                    //Debug.Log("UIChat   OnBroadcastMsg   msg = " + msg + " name = " + name);

                    int index = msg.LastIndexOf('|');
                    //Debug.Log("UIChat   OnBroadcastMsg   index = " + index);

                    int chatType = 0;
                    if (index > 0)
                    {
                        string chatTypeStr = msg.Substring(index + 1);
                        //Debug.Log("UIChat   OnBroadcastMsg   chatTypeStr = " + chatTypeStr + " chatType = " + chatType);

                        int.TryParse(chatTypeStr, out chatType);
                        msg = msg.Substring(0, index);
                    }
                    //Debug.Log("UIChat   OnBroadcastMsg   msg = " + msg + " chatType = " + chatType);

                    string key = jo["broadcastId"].ToString();
                    ChatData chatData = new ChatData();
                    chatData.fromAccId = jo["fromAccid"].ToString();
                    chatData.fromNick = name;//jo["fromUid"].ToString();
                    chatData.msg = msg;//jo["body"].ToString();
                    chatData.bNotice = false;
                    chatData.flow = "in";
                    
                    var json = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(bodyJObject["custom"].ToString())));
                    chatData.customData = json.ToString().FromJSON<CustomData>();
                    if (chatData.customData.projectID != SelfConfigData.projectID)
                    {
                        continue;
                    }

                    ChatMgr.Instance.AddChatMsg(ChatType.All, key, chatData);

                    ////聊天气泡
                    chatModel.ShowChatBubble(chatData, chatType);
                    if (m_ChatType == (ChatType)chatType)
                    {
                        SetChatFoldState(bFold, (ChatType)chatType);
                        SetChatPageInfo((ChatType)chatType);
                    }

                    //if (dicUserInfo.ContainsKey(chatData.fromAccId))
                    //{
                    //    chatData.fromNick = dicUserInfo[chatData.fromAccId].nick;
                    //    Debug.Log("UIChat   OnBroadcastMsg  Not Request UserInfo chatData.fromNick: " + chatData.fromNick);

                    //    ////聊天气泡
                    //    chatModel.ShowChatBubble(chatData);
                    //    SetChatPageInfo((ChatType)chatType);
                    //    SetChatFoldState(bFold, (ChatType)chatType);
                    //}
                    //else
                    //{
                    //    Debug.Log("UIChat   OnBroadcastMsg  Request UserInfo chatData.fromAccId: " + chatData.fromAccId);
                    //    ChatMgr.Instance.GetUserInfo(chatData.fromAccId);
                    //}
                }
            }
        });

        ReceiveMessage("OnGetUserInfo", p =>
         {
             string jsonStr = p.Values.ToString();
             ImUserInfo userInfo = JsonUntity.FromJSON<ImUserInfo>(jsonStr);

             if (userInfo == null)
             {
                 Debug.LogError("获取Im用户信息错误！");
                 return;
             }

             Debug.Log("OnGetUserInfo account: " + userInfo.account + "   nick: " + userInfo.nick + " avatar: " + userInfo.avatar);
             if (!dicUserInfo.ContainsKey(userInfo.account))
             {
                 dicUserInfo.Add(userInfo.account, userInfo);
             }

             ChatData m_ChatData = null;
             foreach (var chatInfo in ChatMgr.Instance.m_AllMsgDic)
             {
                 if (chatInfo.Value.fromAccId.Equals(userInfo.account))
                 {
                     chatInfo.Value.fromNick = userInfo.nick;
                     m_ChatData = chatInfo.Value;
                     break;
                 }
             }

             if (bRecBroadcastMsg)
             {
                 //聊天气泡
                 chatModel.ShowChatBubble(m_ChatData, (int)m_ChatType);
                 if (m_ChatType == ChatType.All)
                 {
                     SetChatFoldState(bFold, m_ChatType);
                     SetChatPageInfo(m_ChatType);
                 }
                 bRecBroadcastMsg = false;
             }
         });


        ReceiveMessage("OnConnect", p =>
        {
            Debug.Log("UIChat   OnConnect   jsonStr = " + p.Values.ToString());
            if (ChatMgr.Instance.bChangeConnect)
            {
                ChatMgr.Instance.UpdateChatData();
                ChatMgr.Instance.bChangeConnect = false;
            }
        });


        ReceiveMessage("OnDisconnect", p =>
        {
            Debug.Log("OnDisconnect   p==============>");
            if (ChatMgr.Instance.bChangeDisConnect)
            {
                ChatMgr.Instance.bChangeConnect = true;
                ChatMgr.Instance.InChatRoom();
                ChatMgr.Instance.Connect();
                ChatMgr.Instance.bChangeDisConnect = false;
            }
        });
    }

    public void UpdateChatChannel()
    {
        SetChatFoldState(bFold, m_ChatType);
        SetChatPageInfo(m_ChatType);
    }

    public void UpdateAllChatSelfName(PlayerControllerImp imp)
    {
        if (imp == null)
            return;

        foreach (var data in ChatMgr.Instance.m_AllMsgDic)
        {
            if (data.Value.fromAccId == imp.UserInfo.YunxinAccid)
            {
                data.Value.fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            }
        }
    }

    public void UpdateRoomChatSelfName(PlayerControllerImp imp)
    {
        if (imp == null)
            return;
        var roomId = ManageMentClass.DataManagerClass.roomId;
        ChatMgr.Instance.m_RoomMsgDic.TryGetValue(roomId, out var dic);
        if (dic == null)
        {
           return;
        }
        foreach (var data in dic)
        {
            if (data.Value.fromAccId == imp.UserInfo.YunxinAccid)
            {
                data.Value.fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            }
        }
    }

    public void UpdateTeamChatSelfName(PlayerControllerImp imp)
    {
        if (imp == null)
            return;
        foreach (var data in ChatMgr.Instance.m_TeamMsgDic)
        {
            if (data.Value.fromAccId == imp.UserInfo.YunxinAccid)
            {
                data.Value.fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            }
        }
    }

    /// <summary>
    /// 切房间时隐藏当前正在显示的聊天气泡
    /// </summary>
    private void hideCurChatBubble()
    {
        switch (m_ChatType)
        {
            case ChatType.All:
                foreach (var data in ChatMgr.Instance.m_AllMsgDic)
                {
                    chatModel.HideChatBubble(data.Value);
                }

                break;
            case ChatType.Room:
                var roomId = ManageMentClass.DataManagerClass.roomId;
                ChatMgr.Instance.m_RoomMsgDic.TryGetValue(roomId, out var dic);
                if (dic != null)
                {
                    foreach (var data in dic)
                    {
                        chatModel.HideChatBubble(data.Value);
                    }
                }
                break;
            case ChatType.Team:
                foreach (var data in ChatMgr.Instance.m_TeamMsgDic)
                {
                    chatModel.HideChatBubble(data.Value);
                }

                break;
        }
    }
}

public class UIChatParam
{
    public List<ChatType> ChatTypes;
    public IChatModel ChatModel;
    public BaseSyncController Controller;
}
