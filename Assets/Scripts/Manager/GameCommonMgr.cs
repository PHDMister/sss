using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;


public class GameCommonMgr : ISingleton
{
    public void Init()
    {


        OnAddEvent();
    }

    protected void OnAddEvent()
    {
        MessageCenter.AddMsgListener(WebSocketConst.WsNet_OnReConnectFail, OnReConnectFailHandle);
    }

    private void OnReConnectFailHandle(KeyValuesUpdate kv)
    {
        if (UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING))
        {
            UIManager.GetInstance().CloseUIForms(FormConst.WAITNETLOADING);
        }
        if (!UIManager.GetInstance().IsOpend(FormConst.NETRECONNECTPANEL))
        {
            UIManager.GetInstance().ShowUIForms(FormConst.NETRECONNECTPANEL);
        }
    }

}
