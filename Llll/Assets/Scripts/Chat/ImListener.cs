using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class ImListener : MonoBehaviour
{
    public void OnMessageChange(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnMessageChange", kvs);
        Debug.Log("ImListener OnMessageChange msg: " + msg);
    }

    public void OnKicked(string reason)
    {
        //KeyValuesUpdate kvs = new KeyValuesUpdate("", reason);
        //MessageCenter.SendMessage("OnKicked", kvs);
        Debug.Log("ImListener OnKicked reason: " + reason);
    }

    public void OnWillReconnect(string msg)
    {
        //KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        //MessageCenter.SendMessage("OnWillReconnect", kvs);
        Debug.Log("ImListener OnWillReconnect msg: " + msg);
    }

    public void OnSendTextMessage(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnSendTextMessage", kvs);
        Debug.Log("ImListener OnSendTextMessage msg: " + msg);
    }

    public void OnConnect(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnConnect", kvs);
        Debug.Log("ImListener OnConnect msg: " + msg);
    }

    public void OnBroadcastMsg(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnBroadcastMsg", kvs);
        Debug.Log("ImListener OnBroadcastMsg msg: " + msg);
    }

    public void OnUpdateUserInfo(string msg)
    {
        Debug.Log("ImListener OnUpdateUserInfo msg: " + msg);
    }

    public void OnGetUserInfo(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnGetUserInfo", kvs);
        Debug.Log("ImListener OnGetUserInfo msg: " + msg);
    }

    public void OnDisconnect(string msg)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msg);
        MessageCenter.SendMessage("OnDisconnect", kvs);
        Debug.Log("ImListener OnDisconnect msg: " + msg);
    }

    public void OnLoginCall(string msg)
    {
        Debug.Log("ImListener OnLoginCall msg: " + msg);
        ChatMgr.Instance.ImUpdateUserInfo();
        //?????
        ChatMgr.Instance.InChatRoom();
        ChatMgr.Instance.Connect();
    }
}
