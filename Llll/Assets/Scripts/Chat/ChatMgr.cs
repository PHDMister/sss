using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Treasure;
using UIFW;
using UnityEngine;
//using System.Web.Script.Serialization;

public class ChatMgr : ISingleton
{
    public static ChatMgr Instance => Singleton<ChatMgr>.Instance;

    public Dictionary<string, ChatData> m_AllMsgDic = new Dictionary<string, ChatData>();
    public Dictionary<uint, Dictionary<string, ChatData>>  m_RoomMsgDic = new Dictionary<uint, Dictionary<string, ChatData>>();
    public Dictionary<string, ChatData> m_TeamMsgDic = new Dictionary<string, ChatData>();
    public bool bChangeDisConnect = false;//是否切房间断开连接
    public bool bChangeConnect = false;//是否切房间建立连接

    public void AddChatMsg(ChatType chatType, string key, ChatData data)
    {
        switch (chatType)
        {
            case ChatType.All:
                if (!m_AllMsgDic.ContainsKey(key))
                {
                    m_AllMsgDic.Add(key, data);
                }
                break;
            case ChatType.Room:
                var roomID = data.customData.roomID;
                m_RoomMsgDic.TryGetValue(roomID, out var dic);
                if (dic == null)
                {
                    dic = new Dictionary<string, ChatData>();
                    m_RoomMsgDic[roomID] = dic;
                }
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, data);
                }
                break;
            case ChatType.Team:
                if (!m_TeamMsgDic.ContainsKey(key))
                {
                    m_TeamMsgDic.Add(key, data);
                }
                break;
        }
    }

    public void Init()
    {
        //Clear();
    }

    public void Clear()
    {
        m_AllMsgDic.Clear();
        m_RoomMsgDic.Clear();
        m_TeamMsgDic.Clear();
    }

    public int CalcuStrLength(string str)
    {
        string pattern = "<sprite=\\d+>";
        int num = Regex.Matches(str, pattern).Count;
        string newStr = Regex.Replace(str, pattern, "", RegexOptions.IgnoreCase);
        //Debug.Log("CalcuStrLength match num =" + num + "  newStr = " + newStr);
        return newStr.Length + num;
    }

    public void ImInit()
    {
        Clear();

        try
        {
            Debug.Log("ImInit appKey =  " + ManageMentClass.DataManagerClass.YXAppKey + " token = " + ManageMentClass.DataManagerClass.YXToken + " account = " + ManageMentClass.DataManagerClass.YXAccid);
            SetTools.ImUtilIns();
            SetTools.ImInit(ManageMentClass.DataManagerClass.YXAppKey, ManageMentClass.DataManagerClass.YXToken, ManageMentClass.DataManagerClass.YXAccid);
        }
        catch (Exception ex)
        {
            Debug.Log("ImInit Exception:  " + ex);
        }
    }

    public void ImUpdateUserInfo()
    {
        try
        {
            string fromNick = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            string fromAvatar = ManageMentClass.DataManagerClass.selfPersonData.user_pic_url;
            SetTools.ImUpdateUserInfo(TextTools.setCutAddString(fromNick, 8, "..."), fromAvatar);
        }
        catch (Exception ex)
        {
            Debug.Log("ImUpdateUserInfo Exception:  " + ex);
        }
    }

    //进入群
    public void InChatRoom()
    {
        try
        {
            ulong chatRoomId = ManageMentClass.DataManagerClass.ChatRoomId;
            string[] chatRoomAddr = ManageMentClass.DataManagerClass.ChatRoomAddr.ToArray();
            string jsonStr = chatRoomAddr.ToJSON();
            Debug.Log($"InChatRoom  room id: " + chatRoomId + $"room addres :{jsonStr}");
            SetTools.ImInChatRoom(chatRoomId.ToString(), jsonStr);
        }
        catch (Exception ex)
        {
            Debug.Log("InChatRoom Exception:  " + ex);
        }
    }

    //开始群连接会话
    public void Connect()
    {
        try
        {
            SetTools.ImConnect();
        }
        catch (Exception ex)
        {
            Debug.Log("ImConnect Exception:  " + ex);
        }
    }

    public void DisconnectChat()
    {
        try
        {
            SetTools.ImDisconnect();
        }
        catch (Exception ex)
        {
            Debug.Log("ImDisconnect Exception:  " + ex);
        }
    }

    public void SendTextMessage(string msg, string toUid = "", string customData = "")
    {
        try
        {
            SetTools.ImSendTextMessage(msg, toUid, customData);
        }
        catch (Exception ex)
        {
            Debug.Log("ImSendTextMessage Exception:  " + ex);
        }
    }

    public void OutLogin()
    {
        try
        {
            SetTools.ImOutLogin();
        }
        catch (Exception ex)
        {
            Debug.Log("ImOutLogin Exception:  " + ex);
        }
    }

    public delegate void TreasureBrocastToAllRespCallBack();
    public void SendWorldMsg(string msg, string data,TreasureBrocastToAllRespCallBack callback = null)
    {
        TreasureBrocastToAllReq req = new TreasureBrocastToAllReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        var jObject = new JObject();
        jObject["msg"] = msg;
        jObject["custom"] = data;
        req.Msg = jObject.ToString();
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.TreasureBrocastToAllReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            TreasureBrocastToAllResp resp = TreasureBrocastToAllResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 230017)//服务器过滤垃圾字错误码
            {
                ToastManager.Instance.ShowNewToast("发送内容不合法!", 3f);
                return;
            }

            if (resp.StatusCode == 0)
            {
                callback?.Invoke();
            }
        });
    }

    /// <summary>
    /// 切换房间
    /// </summary>
    public void DisconnectChatRoom()
    {
        bChangeDisConnect = true;
        Debug.Log("ChangeChatRoom   DisconnectChat==============>");
        DisconnectChat();
    }

    /// <summary>
    /// 切换房间调用
    /// </summary>
    public void UpdateChatData()
    {
        //清除房间队伍聊天数据
        m_RoomMsgDic.Clear();
        m_TeamMsgDic.Clear();
        UIChat chatForm = UIManager.GetInstance().GetUIForm(FormConst.UICHAT) as UIChat;
        chatForm.UpdateChatChannel();
    }

    public void GetUserInfo(string accId)
    {
        try
        {
            SetTools.ImGetUserInfo(accId);
        }
        catch (Exception ex)
        {
            Debug.Log("GetUserInfo Exception:  " + ex);
        }
    }
}

public enum ChatType
{
    All = 1,
    Room = 2,
    Team = 3,
}

public class ChatData
{
    public string fromAccId;
    public string fromNick;
    public string msg;
    public bool bNotice;
    public string flow;//消息流向 'in'表示此消息是收到的消息 'out'表示此消息是发出的消息
    public CustomData customData;
}

public class SendMsgRespData
{
    public string chatroomId;//聊天室的 id
    public string idClient;//- idClient SDK端生成的消息id
    public string from;//- from 消息发送方, 帐号
    public string fromNick;//- fromNick 消息发送方的昵称
    public string fromAvatar;//- fromAvatar 消息发送方的头像
    //public string fromExt;//- fromExt 消息发送方的扩展字段
    public string fromCustom;
    public int userUpdateTime;
    public string status;//- status 'success' | 'fail' | 'sending'
    public string fromClientType;//- fromClientType 发送方的设备类型 Android = 1,iOS = 2,PC = 4,WindowsPhone = 8,Web = 16,Server = 32,Mac = 64
    public int time;//- time 消息时间戳
    public string type;//消息类型 text = 0,image = 1,audio = 2,video = 3,geo = 4,notification = 5,file = 6,tip = 10,robot = 11,g2 = 12,custom = 100
    public string text;
    public bool resend;
    public string flow;//- flow 消息的流向 'in'表示此消息是收到的消息 'out'表示此消息是发出的消息
}

public class KickedRespData
{
    public string reason; //- reason未知 | 互斥类型的客户端互踢-不允许同一个帐号在多个地方同时登录 | 服务器端发起踢客户端指令-被服务器踢了 | 被自己账号所在的其他端踢掉 | 被悄悄踢掉, 表示这个链接已经废掉了 'unknow' | 'samePlatformKick' | 'serverKick' | 'otherPlatformKick' | 'silentlyKick'
    public string message;//原因的详细描述
    public string clientType;//踢了本链接的那个客户端的类型 Android = 1,iOS = 2,PC = 4,WindowsPhone = 8,Web = 16,Server = 32,Mac = 64
}

public class ImUserInfo
{
    public string account;
    public string nick;
    public string avatar;
    public string gender;
    public ulong createTime;
    public ulong updateTime;
}

