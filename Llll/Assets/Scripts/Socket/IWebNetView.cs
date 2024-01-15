//用于频繁消息同步 直接推送数据 不再走事件广播流程 加快效率

using Base;
using Google.Protobuf;
using UnityWebSocket;

public interface IWebNetView
{
    int Order { get; }
    uint GetCode { get; }
    void DoAddProxy();
    void BindNetAgent(WebSocketAgent agent);
    void EnterRoom();
    void LeaveRoom();
    void UnBindNetAgent();
    bool Contains(uint proxy);
    void Push(uint proxy, ByteString dataBytes);
    void OnMsgError(uint proxy, int errorCode);
    void OnNetRestore();
}
public interface ISendPack
{
    WSData WsData { get; }
    float SendTime { get; }
    uint ClientCode { get; }
    uint SendProxy { get; }
    bool IsTimeout();
    void Execute(int code, ByteString bytes);
}
public interface ISyncMsgCtrl
{
    void UpdateRef(MoveControllerImp move, PlayerItem item);
    int GetCount();
    uint CurIndex();
    uint MaxIndex();
    void Push(IMessage msg);
    bool CheckType(IMessage msg);
    bool CheckMsgIndex(uint lastIndex);
    void InsidePop();
    PlayerControllerImp.SyncProcesState GetStartState();
    void SetStartState();
    void OnUpdate(float time);
    bool OverSetLastIndex();
    void Reset();
    void ClearForRestart();
}
