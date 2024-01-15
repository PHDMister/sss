using Google.Protobuf;
using System.Collections.Generic;
using Treasure;
using UnityEngine;

/// <summary>
/// 模块化网络同步基类
/// </summary>
public class BaseNetView : IWebNetView
{
    protected WebSocketAgent agent;
    protected List<ulong> syncProxy = new List<ulong>();
    protected AutoincrementSeeds Code;
    public uint GetCode => Code.uIntCode;
    protected int _order;
    public int Order => _order;
    protected float lastMovingFrame = 0;
    protected float lastRotaFrame = 0;
    protected float offlineStartTime = 0;
    protected bool pauseSyncMove = false;

    public BaseNetView()
    {
        ProxyHandle();
        Code = new AutoincrementSeeds();
    }
    //防止虚函数在构造函数中被调用
    private void ProxyHandle()
    {
        DoAddProxy();
    }

    public virtual void DoAddProxy()
    {

    }

    public virtual void BindNetAgent(WebSocketAgent agent)
    {
        this.agent = agent;
        this.agent.NetView = this;
        Code.ResetAutoAddtive();
    }

    public virtual void UnBindNetAgent()
    {
        if (this.agent != null) agent.NetView = null;
    }

    public virtual bool Contains(uint proxy)
    {
        return syncProxy.Contains(proxy);
    }

    public virtual void Push(uint proxy, ByteString dataBytes)
    {

    }

    public virtual void OnMsgError(uint proxy, int errorCode)
    {

    }

    public virtual void OnNetRestore()
    {

    }

    public virtual void SyncSelfMove(float ty, UnityEngine.Vector3 pos, float nowDis, int state)
    {
        if (pauseSyncMove) return;
        if (state < 10)
        {
            if (state == 1)
            {
                lastMovingFrame = Time.realtimeSinceStartup;
            }
            else if (state == 2 && Time.realtimeSinceStartup - lastMovingFrame >= RoomSyncConst.RoomSyncMoveInterval)
            {
                lastMovingFrame = Time.realtimeSinceStartup;
                SendMove(ty, pos, nowDis, state);
            }
            else if (state == 3)
            {
                lastMovingFrame = Time.realtimeSinceStartup;
                SendMove(ty, pos, nowDis, state);
            }
        }
        else
        {
            if (state == 11 && Time.realtimeSinceStartup - lastRotaFrame >= RoomSyncConst.RoomSyncMoveInterval)
            {
                lastRotaFrame = Time.realtimeSinceStartup;
                SendMove(ty, pos, nowDis, state);
            }
            else if (state == 12)
            {
                lastRotaFrame = Time.realtimeSinceStartup;
                SendMove(ty, pos, nowDis, state);
            }
        }
    }
    public virtual void SendMove(Vector2 joydeltaPos, Vector3 pos, float nowDis, int state)
    {
        Move move = new Move();
        move.Index = Code.uIntCode;
        move.FromUserId = ManageMentClass.DataManagerClass.userId;
        move.State = state;
        //x*1000  y*1000
        joydeltaPos *= 1000;
        int h_maxDis = (int)(nowDis * 1000);
        move.Dir = new Pos() { X = (int)joydeltaPos.x, Y = (int)joydeltaPos.y, Z = h_maxDis };
        //x*1000  y*1000  z*1000
        pos *= 1000;
        move.Pos = new Pos() { X = (int)pos.x, Y = (int)pos.y, Z = (int)pos.z };
        agent.Send((uint)MessageId.Types.Enum.Move, move, true);
    }
    public virtual void SendMove(float ry, Vector3 pos, float nowDis, int state)
    {
        // x*1000  y*1000
        Move move = new Move();
        move.Index = Code.uIntCode;
        move.FromUserId = ManageMentClass.DataManagerClass.userId;
        move.State = state;
        int h_maxDis = (int)(nowDis * 1000);
        int euY = (int)(ry * 1000);
        move.Dir = new Pos() { Y = euY, Z = h_maxDis };
        pos *= 1000;
        move.Pos = new Pos() { X = (int)pos.x, Y = (int)pos.y, Z = (int)pos.z };
        agent.Send((uint)MessageId.Types.Enum.Move, move, true);
        //Debug.Log("1111111111  send  move.Index=" + move.Index);
    }
}
