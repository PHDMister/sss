using System.Collections.Generic;
using Base;
using Google.Protobuf;
using Treasure;
using UnityWebSocket;
using UnityEngine;


//长连接配置 为替代const可配置化  不同场景下 可进行重配置  
//继承此类  修改参数或重写函数
public class WebSocketConfig
{
    //websocket 链接地址
    public string Address { get; protected set; } = "wss://test.24nyae.cn/aiera/v1/game/space/ws";

    //websocket 使用字节流模式 互斥
    public bool UseByte { get; protected set; } = true;
    //websocket 使用文本模式    互斥
    public bool UseText { get; protected set; } = false;
    //显示websocket的log
    public bool ShowLog = true;

    //心跳开关
    public bool HeartBeatEnable { get; protected set; } = true;
    //模拟断线重连
    public bool SimulateOfflineReConnect { get; protected set; } = false;
    //心跳
    public uint HeartBeatMessageId { get; protected set; } = 2002;
    //心跳包发送间隔s
    public float HeartBeatInterval { get; protected set; } = 8;
    //等待心跳包返回的超时时间s
    public float WaitHeartBeatCallbackTimeout { get; protected set; } = 5;


    //断链之后多久开始重连s
    public float ReConnectDelay { get; protected set; } = 1;
    //重连的超时总时间s
    public float ReConnectTime { get; protected set; } = 30;
    //重连的等待超时时间s
    public float ReConnectSmallTime { get; protected set; } = 3;


    //请求之后多长时间没有相应出现loading转圈
    public float TimeOutShowLoading { get; protected set; } = 3;
    //请求之后多长时间后服务器依然没有响应的时间
    public float ShowNetWaitPanelTime { get; protected set; } = 2;

    //断线期间，是否缓存未发送成功的请求
    public bool CacheRequestOnDisconnect { get; protected set; } = false;


    public virtual WebSocketProcess GetProcess(WebSocketAgent agent)
    {
        if (UseByte) return new WebSocketByteProcess(agent);
        if (UseText) return new WebSocketTextProcess(agent);
        return null;
    }

    public virtual HeartSendPack GetHeartSendPack(WebSocketAgent agent)
    {
        return new HeartSendPack(agent);
    }

    public virtual ISendPack GetSendPack(uint proxy, WSData data, WsEventCenter.CallBack<int, ByteString> callBack)
    {
        return new SendPack(proxy, data, callBack);
    }

    public virtual ISendPack GetRegiserSendPack(uint proxy, WSData data)
    {
        return new RegiserSendPack(proxy, data);
    }
}

//正式服配置
public class WebSocketReleaseConfig : WebSocketConfig
{
    public WebSocketReleaseConfig()
    {
        Address = "wss://api.aichaoliuapp.cn/aiera/v1/game/space/ws";
        ShowLog = false;
    }
}





//可定义websocket进程执行代码
public class WsProcessBase
{
    protected WebSocketAgent Agent;
    public WsProcessBase(WebSocketAgent agent)
    {
        Agent = agent;
    }
}
//可定义  方便不同场景下的优化需求  需要定制就继承此类
public class WebSocketProcess : WsProcessBase
{
    public IWebNetView NetView;
    protected List<IWebNetView> NetViewList;
    public WebSocketProcess(WebSocketAgent agent) : base(agent)
    {
        NetViewList = new List<IWebNetView>(2);
    }

    public void RegisterNetView(IWebNetView netView)
    {
        if (!NetViewList.Contains(netView))
        {
            NetViewList.Add(netView);
            NetViewList.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }
    public void UnRegisterNetView(IWebNetView netView)
    {
        NetViewList.Remove(netView);
    }
    public T GetNetView<T>() where T : BaseNetView
    {
        foreach (var webNetView in NetViewList)
        {
            if (webNetView is T)
            {
                return webNetView as T;
            }
        }
        return null;
    }
    public virtual void OnFristOpen(OpenEventArgs e)
    {
    }
    public virtual void OnReconnectOpen(OpenEventArgs e)
    {
    }
    public virtual void OnSocket_OnMessageSuccess(WSData wsData)
    {

    }
    public virtual void OnSocket_OnMessageSuccess(string data)
    {

    }
    public virtual void OnSocket_OnMessageError(WSData wsData)
    {
    }
    public virtual void OnSocket_OnMessageError(string data)
    {

    }
    public virtual void Socket_OnClose(CloseEventArgs e)
    {
    }
    public virtual void Socket_OnError(ErrorEventArgs e)
    {
    }
}
//长连接 字节流数据
public class WebSocketByteProcess : WebSocketProcess
{
    public WebSocketByteProcess(WebSocketAgent agent) : base(agent)
    {

    }

    public override void OnFristOpen(OpenEventArgs e)
    {

    }
    public override void OnReconnectOpen(OpenEventArgs e)
    {
        NetView?.OnNetRestore();
    }
    public override void OnSocket_OnMessageSuccess(WSData wsData)
    {
        //高频Move优化
       /* if (wsData.MessageType == (uint)MessageId.Types.Enum.Move && NetView != null)
        {
            NetView.Push(wsData.MessageType, wsData.Data);
            return;
        }*/

        //高频同步直接使用NetView进行同步
        if (NetView != null && NetView.Contains(wsData.MessageType))
        {
            NetView.Push(wsData.MessageType, wsData.Data);
        }

        //模块协议
        if (NetViewList.Count > 0)
        {
            bool isReturn = false;
            foreach (var webNetView in NetViewList)
            {
                if (webNetView.Contains(wsData.MessageType))
                {
                    webNetView.Push(wsData.MessageType, wsData.Data);
                    isReturn = true;
                }
            }
            if (isReturn) return;
        }

        //普通协议使用事件广播同步
        if (wsData.ClientCode > 0 && Agent.MsgCallback.TryGetValue(wsData.ClientCode, out var sp))
        {
            Debug.Log("输出clinetcode: "+ wsData.ClientCode+ "  wsData.MessageType: "+ wsData.MessageType);
            Agent.MsgCallback.Remove(wsData.ClientCode);
            sp.Execute(0, wsData.Data);
        }
        else
        {
            //定向推送协议
            //收到消息 清除等待信息
            if (wsData.ClientCode > 0 && Agent.MsgRecords.TryGetValue(wsData.ClientCode, out var isp))
            {
                Agent.MsgRecords.Remove(wsData.ClientCode);
                isp.Execute(0, wsData.Data);
            }
            Debug.Log("输出clinetcode: " + wsData.ClientCode + "  wsData.MessageType: " + wsData.MessageType+" ");
            Agent.Broadcast(wsData.MessageType, wsData.ClientCode, wsData.Data);
        }
    }
    public override void OnSocket_OnMessageError(WSData wsData)
    {
        if (NetView != null && NetView.Contains(wsData.MessageType))
        {
            NetView.OnMsgError(wsData.MessageType, (int)wsData.ErrorCode);
        }
        if (wsData.ClientCode > 0 && Agent.MsgCallback.ContainsKey(wsData.ClientCode))
        {
            ISendPack sendPack = Agent.MsgCallback[wsData.ClientCode];
            Agent.MsgCallback.Remove(wsData.ClientCode);
            sendPack.Execute(1, ByteString.Empty);
        }
        else
        {
            //收到消息 清除等待信息
            if (wsData.ClientCode > 0 && Agent.MsgRecords.ContainsKey(wsData.ClientCode))
            {
                ISendPack sendPack = Agent.MsgRecords[wsData.ClientCode];
                Agent.MsgRecords.Remove(wsData.ClientCode);
                sendPack.Execute(0, wsData.Data);
            }
            Agent.Broadcast(wsData.MessageType, wsData.ClientCode, ByteString.Empty);
        }
    }
    public override void Socket_OnClose(CloseEventArgs e)
    {
        //断线是否启用缓存未发送成功的消息
        if (WebSocketAgent.Config != null && WebSocketAgent.Config.CacheRequestOnDisconnect)
        {
            foreach (ISendPack sendPack in Agent.MsgCallback.Values)
            {
                Agent.OffLineWsDatas.Enqueue(sendPack.WsData);
            }
            foreach (ISendPack sendPack in Agent.MsgRecords.Values)
            {
                Agent.OffLineWsDatas.Enqueue(sendPack.WsData);
            }
        }
        NetView?.OnMsgError(0, 1);
    }
    public override void Socket_OnError(ErrorEventArgs e)
    {

    }
}
//长连接 json等文本消息处理
public class WebSocketTextProcess : WebSocketProcess
{
    public WebSocketTextProcess(WebSocketAgent agent) : base(agent)
    {

    }
    public override void OnFristOpen(OpenEventArgs e)
    {

    }

    public override void OnReconnectOpen(OpenEventArgs e)
    {

    }

    public override void OnSocket_OnMessageSuccess(string data)
    {

    }
    public override void Socket_OnClose(CloseEventArgs e)
    {

    }
    public override void Socket_OnError(ErrorEventArgs e)
    {

    }
}
