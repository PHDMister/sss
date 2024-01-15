using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;
using UnityWebSocket;

public class WsEventCenter
{
    public delegate void CallBack();
    public delegate void CallBack<T>(T arg);
    public delegate void CallBack<T, X>(T arg1, X arg2);
    public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
    public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
    public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);

    private Dictionary<uint, Delegate> m_EventTable = new Dictionary<uint, Delegate>();

    private void OnListenerAdding(uint eventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }
        Delegate d = m_EventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("����Ϊ�¼�{0}���Ӳ�ͬ���͵�ί�У���ǰ�¼�����Ӧ��ί����{1}��Ҫ���ӵ�ί������Ϊ{2}", eventType, d.GetType(), callBack.GetType()));
        }
    }
    private bool OnListenerRemoving(uint eventType, Delegate callBack)
    {
        return m_EventTable.ContainsKey(eventType);
        //if (m_EventTable.ContainsKey(eventType))
        //{
        //    Delegate d = m_EventTable[eventType];
        //    if (d == null)
        //    {
        //        throw new Exception(string.Format("�Ƴ����������¼�{0}û�ж�Ӧ��ί��", eventType));
        //    }
        //    else if (d.GetType() != callBack.GetType())
        //    {
        //        throw new Exception(string.Format("�Ƴ��������󣺳���Ϊ�¼�{0}�Ƴ���ͬ���͵�ί�У���ǰί������Ϊ{1}��Ҫ�Ƴ���ί������Ϊ{2}", eventType, d.GetType(), callBack.GetType()));
        //    }
        //}
        //else
        //{
        //    throw new Exception(string.Format("�Ƴ���������û���¼���{0}", eventType));
        //}
    }
    private void OnListenerRemoved(uint eventType)
    {
        if (m_EventTable[eventType] == null)
        {
            m_EventTable.Remove(eventType);
        }
    }
    //no parameters
    public void AddListener(uint eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
    }
    //Single parameters
    public void AddListener<T>(uint eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
    }
    //two parameters
    public void AddListener<T, X>(uint eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] + callBack;
    }
    //three parameters
    public void AddListener<T, X, Y>(uint eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
    }
    //four parameters
    public void AddListener<T, X, Y, Z>(uint eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
    }
    //five parameters
    public void AddListener<T, X, Y, Z, W>(uint eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
    }

    //no parameters
    public void RemoveListener(uint eventType, CallBack callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }
    //single parameters
    public void RemoveListener<T>(uint eventType, CallBack<T> callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }
    //two parameters
    public void RemoveListener<T, X>(uint eventType, CallBack<T, X> callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }
    //three parameters
    public void RemoveListener<T, X, Y>(uint eventType, CallBack<T, X, Y> callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }
    //four parameters
    public void RemoveListener<T, X, Y, Z>(uint eventType, CallBack<T, X, Y, Z> callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }
    //five parameters
    public void RemoveListener<T, X, Y, Z, W>(uint eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        if (OnListenerRemoving(eventType, callBack))
        {
            m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
    }


    //no parameters
    public void Broadcast(uint eventType)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack callBack = d as CallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    //single parameters
    public void Broadcast<T>(uint eventType, T arg)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    //two parameters
    public void Broadcast<T, X>(uint eventType, T arg1, X arg2)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    //three parameters
    public void Broadcast<T, X, Y>(uint eventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    //four parameters
    public void Broadcast<T, X, Y, Z>(uint eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }
    //five parameters
    public void Broadcast<T, X, Y, Z, W>(uint eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", eventType));
            }
        }
    }


    //
    public void RemoveAllListener()
    {
        m_EventTable.Clear();
    }
}

public enum TimeState
{
    Open,
    Close
}

//websocket 长链接
public class WebSocketAgent : WsEventCenter
{
    private static WebSocketAgent _ins;
    public static WebSocketAgent Ins
    {
        get
        {
            if (_ins == null) _ins = new WebSocketAgent();
            return _ins;
        }
    }

    public string address = "wss://test.24nyae.cn/ws";

#if UNITY_WEB_SOCKET_LOG
    public bool logMessage = true;
#else
    public bool logMessage = true;
#endif

    public int CurConnectCount => ConnectCount;
    public int CurReConnectCount => ReConnectCount;
    public bool SetIsConnected { set => IsConnected = value; }
    //当前是否是链接状态
    public bool IsConnected { get; private set; } = false;
    //当前是否正在重连
    public bool IsReConnecting { get; private set; } = false;




    //链接的次数
    protected int ConnectCount = 0;
    //重新连接的次数
    protected int ReConnectCount = 0;

    //TimerId
    protected Timer ReconnectTimer;
    protected Timer ReconnectSmallTimer;

    //链接Socket
    protected WebSocket socket;
    public static WebSocketConfig Config;
    protected WebSocketProcess Process;
    //当前执行的WebSocket的执行流，可注册多个NetView
    public WebSocketProcess CurProcess => Process;
    //如果当前处于网络连接断开的情况，此时会缓存发送失败的请求，等到断线重连之后再重新请求
    public Queue<WSData> OffLineWsDatas = new Queue<WSData>(4);
    public Dictionary<uint, ISendPack> MsgCallback = new Dictionary<uint, ISendPack>(8);
    public Dictionary<uint, ISendPack> MsgRecords = new Dictionary<uint, ISendPack>(8);
    //心跳包
    public HeartSendPack heartBeat;
    //主要用于长链接中高频率同步的数据广播
    public IWebNetView NetView
    {
        get => Process.NetView;
        set => Process.NetView = value;
    }


    public WebSocketAgent()
    {
    }

    //链接函数  大重连 调用Connect
    protected void Connect()
    {
        ReConnectCount = 0;
        ClearCache();
        socket = new WebSocket(address);
        socket.OnOpen += Socket_OnOpen;
        socket.OnMessage += Socket_OnMessage;
        socket.OnClose += Socket_OnClose;
        socket.OnError += Socket_OnError;
        Log($"Connecting... url:{address} ");
        socket.ConnectAsync();
    }
    protected void Connect(string address)
    {
        this.address = address;
        Connect();
    }
    public void Connect(WebSocketConfig config)
    {
        Config = config;
        logMessage = config.ShowLog;
        this.address = config.Address;
        this.Process = config.GetProcess(this);
        heartBeat = Config.GetHeartSendPack(this);
        Connect();
    }
    //断链函数
    public void DisConnect()
    {
        if (Config.SimulateOfflineReConnect)
        {
            if (socket != null)
            {
                socket.OnOpen -= Socket_OnOpen;
                socket.OnMessage -= Socket_OnMessage;
                socket.OnClose -= Socket_OnClose;
                socket.OnError -= Socket_OnError;
                socket.CloseAsync();
            }
            socket = null;
        }
        else
        {
            if (socket != null)
            {
                socket.OnOpen -= Socket_OnOpen;
                socket.OnMessage -= Socket_OnMessage;
                socket.OnClose -= Socket_OnClose;
                socket.OnError -= Socket_OnError;
                if (socket.ReadyState == WebSocketState.Open) socket.CloseAsync();

#if !NET_LEGACY && (UNITY_EDITOR || !UNITY_WEBGL) && !UNITY_WEB_SOCKET_ENABLE_ASYNC
                WebSocketManager.Instance.Remove(socket);
#elif !UNITY_EDITOR && UNITY_WEBGL
                WebSocketManager.Remove(socket.instanceId);
#endif
            }
            socket = null;
        }
    }
    public void ResetWebsocket()
    {
        ConnectCount = 0;
        ReConnectCount = 0;
        heartBeat.Close();
        ReConnectTimer(TimeState.Close);
        ReConnectSmallTimer(TimeState.Close);
        ClearCache();
        DisConnect();
    }
    public void ReConnect()
    {
        if (IsReConnecting) return;
        IsReConnecting = true;
        Log("wait for start reconnect.......................");
        DisConnect();
        Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.ReConnectDelay, () =>
         {
             ReConnectCount++;
             socket = new WebSocket(address);
             socket.OnOpen += Socket_OnOpen;
             socket.OnMessage += Socket_OnMessage;
             socket.OnClose += Socket_OnClose;
             socket.OnError += Socket_OnError;
             Log($"start reconnecting... url:{address} times:{ReConnectCount} ");
             socket.ConnectAsync();

             ReConnectSmallTimer(TimeState.Open);
             ReConnectTimer(TimeState.Open);
         });
    }
    private void ReConnectTimer(TimeState option)
    {
        switch (option)
        {
            //启动重连计时器  重连过程总计30秒  超过这个时间则进入大重连 
            case TimeState.Open:
                if (ReconnectTimer != null) return;
                Log("start up  reconnect  timer");
                ReconnectTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.ReConnectTime, () =>
                {
                    //整个重连过程超时了 进入大重连  弹出界面给玩家选择
                    Log($"reconnect  timeout {WebSocketAgent.Config.ReConnectTime}  send event wait for player click");
                    IsReConnecting = false;
                    ClearCache();
                    DisConnect();
                    AllTimerCancel();
                    SendEvent(WebSocketConst.WsNet_OnReConnectFail, null);
                });
                break;
            //关闭重连计时器
            case TimeState.Close:
                Log("stop  reconnect  timeout   timer ");
                ReconnectTimer?.Cancel();
                ReconnectTimer = null;
                break;
        }
    }
    private void ReConnectSmallTimer(TimeState option)
    {
        switch (option)
        {
            //单次重连 (5)秒超时
            case TimeState.Open:
                if (ReconnectSmallTimer != null) return;
                ReconnectSmallTimer = Timer.RegisterRealTimeNoLoop(WebSocketAgent.Config.ReConnectSmallTime, () =>
                {
                    Log($"simple reconnect  timeout {WebSocketAgent.Config.ReConnectSmallTime} wait next ");
                    ReconnectSmallTimer = null;
                    IsReConnecting = false;
                    ReConnect();
                });
                break;
            //关闭单次重连
            case TimeState.Close:
                ReconnectSmallTimer?.Cancel();
                ReconnectSmallTimer = null;
                break;
        }
    }
    private void AllTimerCancel()
    {
        ReconnectTimer?.Cancel();
        ReconnectTimer = null;
        ReconnectSmallTimer?.Cancel();
        ReconnectSmallTimer = null;
    }
    private void ClearCache()
    {
        OffLineWsDatas.Clear();
        MsgCallback.Clear();
        MsgRecords.Clear();
    }

    //当前链接的回调事件
    private void Socket_OnOpen(object sender, OpenEventArgs e)
    {
        Log("connect success !!!  ");
        ConnectCount++;
        IsConnected = true;
        //首次链接
        if (ConnectCount <= 1)
        {
            Process?.OnFristOpen(e);
            SendEvent(WebSocketConst.WsNet_OnFirstOpen, ConnectCount);
        }
        //重连成功
        else
        {
            IsReConnecting = false;
            ReConnectSmallTimer(TimeState.Close);
            ReConnectTimer(TimeState.Close);
            heartBeat.ImmOpen(); //启动心跳包
            CheckOfflineReqList(); //检查离线时发送的消息
            Process?.OnReconnectOpen(e); //通知进程重连完成
            SendEvent(WebSocketConst.WsNet_OnReConnect, ReConnectCount);
        }
    }
    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (e.IsBinary && Config.UseByte)
        {
            //解析协议   e.Data  e.RawData
            WSData wsData = WSData.Parser.ParseFrom(e.RawData);
            Log($"Socket_OnMessage  MessageId : {wsData.MessageType}  ClientCode: {wsData.ClientCode}  ErrorCode : {wsData.ErrorCode}");
            //先看是不是心跳
            if (wsData.MessageType == Config.HeartBeatMessageId)
            {
                //心跳出现错误 进入重登流程
                //Log("Socket_OnMessage  heart beat");
                if (wsData.ErrorCode > 0)
                {
                    heartBeat.Error();
                    return;
                }
                //收到心跳  继续下一次心跳
                heartBeat.Execute(0, ByteString.Empty);
                return;
            }

            //消息成功
            if (wsData.ErrorCode == 0)
            {
                //可定制执行过程
                Process?.OnSocket_OnMessageSuccess(wsData);
            }
            else
            {
                LogError($"Error  MessageId : {wsData.MessageType}  ClientCode: {wsData.ClientCode} ErrorCode : {wsData.ErrorCode} !!!");
                //可定制执行过程
                Process?.OnSocket_OnMessageError(wsData);
            }
        }
        else if (e.IsText && Config.UseText)
        {
            Log("revices string msg but not handle !  data=" + e.Data);
            Process?.OnSocket_OnMessageSuccess(e.Data);
        }
    }
    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        Log(e != null
            ? $"connect close  disconnect !  code:{e.Code}  reason:{e.Reason}"
            : "connect close  disconnect !  ");

        this.IsConnected = false;
        heartBeat.Close();
        Process?.Socket_OnClose(e);
        SendEvent(WebSocketConst.WsNet_OnDisConnect, ConnectCount);
        ReConnect();
    }
    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        LogError(e != null
            ? $"requst  error!  msg:{e.Message}"
            : "requst  error!   disconnect  wait  for  offlink");
        Process?.Socket_OnError(e);
    }
    private void CheckOfflineReqList()
    {
        if (!Config.CacheRequestOnDisconnect) return;
        if (OffLineWsDatas.Count <= 0) return;
        for (int i = 0; i < OffLineWsDatas.Count; i++)
        {
            WSData wsData = OffLineWsDatas.Dequeue();
            socket.SendAsync(wsData.ToByteArray());
        }
    }
    public void SendHeartBeat()
    {
        heartBeat.ImmOpen();
    }

    //高频同步发送消息
    public void Send(uint proxy, byte[] data, bool overriod = false)
    {
        if (IsConnected)
        {
            WSData wsData = new WSData();
            wsData.MessageType = proxy;
            wsData.Data = ByteString.CopyFrom(data);
            wsData.ClientCode = 0;
            socket?.SendAsync(wsData.ToByteArray());
        }
        else
        {
            NetView?.OnMsgError(proxy, 1);
        }
    }
    public void Send<T>(uint proxy, T dataObj, bool overriod = false) where T : IMessage
    {
        if (IsConnected)
        {
            WSData wsData = new WSData();
            wsData.MessageType = proxy;
            wsData.Data = dataObj.ToByteString();
            wsData.ClientCode = 0;
            socket?.SendAsync(wsData.ToByteArray());
        }
        else
        {
            NetView?.OnMsgError(proxy, 1);
        }
    }
    public void SendBytes(uint proxy, byte[] data)
    {
        if (IsConnected)
        {
            socket?.SendAsync(data);
        }
        else
        {
            NetView?.OnMsgError(proxy, 1);
        }
    }

    //普通请求使用
    public uint Send<T>(uint proxy, T dataObj) where T : IMessage
    {
        //暂时屏蔽 离线消息缓存  后续改开关
        if (!IsConnected)
        {
            Log("  offline    proxy:" + proxy + "  send  fail");
            return 1;
        }
        WSData wsData = new WSData();
        wsData.MessageType = proxy;
        wsData.Data = dataObj.ToByteString();
        wsData.ClientCode = WebSocketConst.Client_Code;
        MsgRecords[wsData.ClientCode] = Config.GetRegiserSendPack(proxy, wsData);
        if (!IsConnected)
        {
            if (Config.CacheRequestOnDisconnect) OffLineWsDatas.Enqueue(wsData);
            Broadcast(proxy, wsData.ClientCode, ByteString.Empty);
            return wsData.ClientCode;
        }
        //正常发送
        socket?.SendAsync(wsData.ToByteArray());
        return wsData.ClientCode;
    }
    public static uint SendMsg<T>(uint proxy, T dataObj) where T : IMessage
    {
        return WebSocketAgent.Ins.Send(proxy, dataObj);
    }
    public void Send<T>(uint proxy, T dataObj, CallBack<int, ByteString> callBack) where T : IMessage
    {
        WSData wsData = new WSData();
        wsData.MessageType = proxy;
        wsData.Data = dataObj.ToByteString();
        wsData.ClientCode = WebSocketConst.Client_Code;
        MsgCallback[wsData.ClientCode] = Config.GetSendPack(proxy, wsData, callBack);
        if (!IsConnected)
        {
            if (Config.CacheRequestOnDisconnect) OffLineWsDatas.Enqueue(wsData);
            return;
        }
        //正常发送
        socket?.SendAsync(wsData.ToByteArray());
    }
    public static void SendMsg<T>(uint proxy, T dataObj, CallBack<int, ByteString> callBack) where T : IMessage
    {
        WebSocketAgent.Ins.Send(proxy, dataObj, callBack);
    }

    //注册协议的返回监听   CallBack<int, byte[]>  int：ClientCode    byte[]消息体使用对应proto解析
    public void AddProxy(uint id, CallBack<uint, ByteString> callBack)
    {
        AddListener(id, callBack);
    }
    public static void AddProxyMsg(uint id, CallBack<uint, ByteString> callBack)
    {
        WebSocketAgent.Ins.AddProxy(id, callBack);
    }
    public void DelProxy(uint id, CallBack<uint, ByteString> callBack)
    {
        RemoveListener(id, callBack);
    }
    public static void DelProxyMsg(uint id, CallBack<uint, ByteString> callBack)
    {
        WebSocketAgent.Ins.DelProxy(id, callBack);
    }

    //注册小模块
    public static void RegisterSubNetView(IWebNetView webNetView)
    {
        if (Ins.CurProcess == null)
        {
            LogError("Ins.CurProcess is  null");
            return;
        }
        webNetView.BindNetAgent(Ins);
        Ins.CurProcess.RegisterNetView(webNetView);
    }
    public static void UnRegisterSubNetView(IWebNetView webNetView)
    {
        if (Ins.CurProcess == null)
        {
            LogError("Ins.CurProcess is  null");
            return;
        }
        webNetView.UnBindNetAgent();
        Ins.CurProcess.UnRegisterNetView(webNetView);
    }
    public static T GetSubNetView<T>() where T : BaseNetView
    {
        if (Ins.CurProcess == null)
        {
            LogError("Ins.CurProcess is  null");
            return default(T);
        }
        return Ins.CurProcess.GetNetView<T>();
    }


    //Log
    public static void Log(string msg)
    {
        if (!Ins.logMessage) return;
        Debug.Log($"<color=yellow>[WebSocket]  {msg} </color>");

#if UNITY_WEB_SOCKET_LOG
        SendEvent("WebSocketAgent_log", $"<color=yellow>[WebSocket]  {msg} </color>", 0);
#endif
    }
    public static void LogError(string msg)
    {
        if (!Ins.logMessage) return;
        Debug.Log($"<color=red>[WebSocket] {msg} </color>");

#if UNITY_WEB_SOCKET_LOG
        SendEvent("WebSocketAgent_logerror", $"<color=yellow>[WebSocket]  {msg} </color>", 0);
#endif
    }


    //事件广播到UI层  MessageCenter.SendMessage
    public void SendEvent(string msgType, object msgContent)
    {
        Log($"SendEvent : {msgType}  params:{msgContent}");
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }
    public void SendEvent(string msgType, object msgContent, int overriod = 0)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate("", msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }
}
