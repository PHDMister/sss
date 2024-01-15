using System;
using Base;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;

namespace UnityWebSocket
{
    //心跳发送的记录
    public class HeartSendPack : ISendPack
    {
        private float _SendTime;
        private uint _ClientCode;
        private uint _SendProxy;
        public float SendTime => _SendTime;
        public uint ClientCode => _ClientCode;
        public uint SendProxy => _SendProxy;
        public WSData WsData => heartData;
        private WSData heartData;

        private Timer waitHeartCallbackTimer;
        private Timer nextSendTimer;
        private WebSocketAgent agent;


        public HeartSendPack(WebSocketAgent agent)
        {
            this.agent = agent;
            _SendProxy = WebSocketAgent.Config.HeartBeatMessageId;
            _ClientCode = 0;
            heartData = new WSData();
            heartData.MessageType = _SendProxy;
        }

        public virtual void ImmOpen()
        {
            if (!WebSocketAgent.Config.HeartBeatEnable) return;
            _SendTime = Time.realtimeSinceStartup;
            WSData hd = new WSData();
            hd.Id = ManageMentClass.DataManagerClass.userId;
            hd.MessageType = _SendProxy;
            agent.SendBytes(_SendProxy, hd.ToByteArray());
            waitHeartCallbackTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.WaitHeartBeatCallbackTimeout, OnHeartTimeout);
        }
        public virtual void Open()
        {
            if (!WebSocketAgent.Config.HeartBeatEnable) return;
            waitHeartCallbackTimer?.Cancel();
            waitHeartCallbackTimer = null;
            nextSendTimer?.Cancel();
            nextSendTimer = null;
            nextSendTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.HeartBeatInterval, OnNextSendHeart);
        }
        public virtual void Close()
        {
            waitHeartCallbackTimer?.Cancel();
            waitHeartCallbackTimer = null;
            nextSendTimer?.Cancel();
            nextSendTimer = null;
        }
        private void OnNextSendHeart()
        {
            if (!WebSocketAgent.Config.HeartBeatEnable) return;
            _SendTime = Time.realtimeSinceStartup;
            WSData hd = new WSData();
            hd.Id = ManageMentClass.DataManagerClass.userId;
            hd.MessageType = _SendProxy;
            agent.SendBytes(_SendProxy, hd.ToByteArray());
            waitHeartCallbackTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.WaitHeartBeatCallbackTimeout, OnHeartTimeout);
        }
        private void OnHeartTimeout()
        {
            WebSocketAgent.Log($"StartHeartBeat wait heart respones  timeout  Time:{WebSocketAgent.Config.WaitHeartBeatCallbackTimeout} ");
            waitHeartCallbackTimer?.Cancel();
            waitHeartCallbackTimer = null;
            agent.SetIsConnected = false;
            agent.SendEvent(WebSocketConst.WsNet_OnDisConnect, agent.CurConnectCount);
            agent.ReConnect();
        }
        public virtual bool IsTimeout()
        {
            return Time.realtimeSinceStartup - _SendTime >= WebSocketAgent.Config.WaitHeartBeatCallbackTimeout;
        }
        public virtual void Execute(int code, ByteString bytes)
        {
            if (!WebSocketAgent.Config.HeartBeatEnable) return;
            waitHeartCallbackTimer?.Cancel();
            waitHeartCallbackTimer = null;
            Open();
        }
        public virtual void Error()
        {
            Close();
            agent.ResetWebsocket();
            if (UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING))
                UIManager.GetInstance().CloseUIForms(FormConst.WAITNETLOADING);
            if (!UIManager.GetInstance().IsOpend(FormConst.NETRECONNECTPANEL))
                UIManager.GetInstance().ShowUIForms(FormConst.NETRECONNECTPANEL);
        }
    }


    //回调型发送的发送记录
    public class SendPack : ISendPack
    {
        private float _SendTime;
        private uint _ClientCode;
        private uint _SendProxy;
        private WSData _Data;
        public float SendTime => _SendTime;
        public uint ClientCode => _ClientCode;
        public uint SendProxy => _SendProxy;
        public WSData WsData => _Data;

        private WsEventCenter.CallBack<int, ByteString> CallBack;
        private Timer showLoadingTimer;
        private bool IsTimeOut = false;


        public SendPack(uint proxy, WSData data, WsEventCenter.CallBack<int, ByteString> callBack)
        {
            _SendProxy = proxy;
            _ClientCode = data.ClientCode;
            _SendTime = Time.realtimeSinceStartup;
            _Data = data;
            CallBack = callBack;
            showLoadingTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.TimeOutShowLoading, ShowLoadingPanel, OnRequestLongTime);
        }
        public virtual bool IsTimeout()
        {
            return Time.realtimeSinceStartup - _SendTime >= WebSocketAgent.Config.TimeOutShowLoading;
        }

        private void ShowLoadingPanel()
        {
            if (UIManager.GetInstance().IsOpend(FormConst.PETDENSLOADING)) return;
            if (IsTimeOut && UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
            {
                //计数--
                ui.DecWaitRecordNum();
            }
            ToastManager.Instance.ShowNewToast("服务器开小差,请稍后再试");
        }
        private void OnRequestLongTime(float time)
        {
            //WebSocketAgent.Log("OnRequestLongTime  waitTime=" + waitTime);
            if (UIManager.GetInstance().IsOpend(FormConst.PETDENSLOADING)) return;
            if (Time.realtimeSinceStartup - _SendTime >= WebSocketAgent.Config.ShowNetWaitPanelTime && !IsTimeOut)
            {
                WebSocketAgent.Log($"SendPack req={(MessageId.Types.Enum)SendProxy}  reqId:{SendProxy}   requset Timeout  great  {WebSocketAgent.Config.ShowNetWaitPanelTime}s   open laoding panel !!!");
                if (UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
                {
                    //计数++
                    ui.AddWaitRecordNum();
                }
                else
                {
                    UIManager.GetInstance().ShowUIForms(FormConst.WAITNETLOADING);
                }
                IsTimeOut = true;
            }
        }

        public virtual void Execute(int code, ByteString bytes)
        {
            if (showLoadingTimer != null) showLoadingTimer.Cancel();
            showLoadingTimer = null;
            if (IsTimeOut && UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
            {
                //计数--
                ui.DecWaitRecordNum();
            }
            if (CallBack != null) CallBack(code, bytes);
        }

    }


    //注册型发送的发送记录
    public class RegiserSendPack : ISendPack
    {
        private float _SendTime;
        private uint _ClientCode;
        private uint _SendProxy;
        private WSData _Data;
        public float SendTime => _SendTime;
        public uint ClientCode => _ClientCode;
        public uint SendProxy => _SendProxy;
        public WSData WsData => _Data;

        private bool IsTimeOut = false;
        private Timer showLoadingTimer;

        public RegiserSendPack(uint proxy, WSData data)
        {
            _SendProxy = proxy;
            _Data = data;
            _ClientCode = data.ClientCode;
            _SendTime = Time.realtimeSinceStartup;
            showLoadingTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.TimeOutShowLoading, ShowLoadingPanel, OnRequestLongTime);
        }

        public virtual bool IsTimeout()
        {
            return Time.realtimeSinceStartup - _SendTime >= WebSocketAgent.Config.TimeOutShowLoading;
        }
        private void ShowLoadingPanel()
        {
            if (UIManager.GetInstance().IsOpend(FormConst.PETDENSLOADING)) return;
            if (IsTimeOut && UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
            {
                //计数--
                ui.DecWaitRecordNum();
            }
            ToastManager.Instance.ShowNewToast("服务器开小差,请稍后再试");
        }
        private void OnRequestLongTime(float time)
        {
            //WebSocketAgent.Log("OnRequestLongTime  waitTime=" + waitTime);
            if (UIManager.GetInstance().IsOpend(FormConst.PETDENSLOADING)) return;
            if (Time.realtimeSinceStartup - _SendTime >= WebSocketAgent.Config.ShowNetWaitPanelTime && !IsTimeOut)
            {
                WebSocketAgent.Log($"SendPack req={(MessageId.Types.Enum)SendProxy}  reqId:{SendProxy}   requset Timeout  great  {WebSocketAgent.Config.ShowNetWaitPanelTime}s   open laoding panel !!!");
                if (UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
                {
                    //计数++
                    ui.AddWaitRecordNum();
                }
                else
                {
                    UIManager.GetInstance().ShowUIForms(FormConst.WAITNETLOADING);
                }
                IsTimeOut = true;
            }
        }

        public virtual void Execute(int code, ByteString bytes)
        {
            if (showLoadingTimer != null) showLoadingTimer.Cancel();
            showLoadingTimer = null;
            if (IsTimeOut && UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING, out UIWaitNetLoading ui))
            {
                //计数--
                ui.DecWaitRecordNum();
            }
        }
    }

}