using Google.Protobuf;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;

//挖宝房间同步
public class RoomSyncNetView : BaseNetView, ISingleton
{
    private TreasuringController treasureController;

    public void Init()
    {

    }

    public override void DoAddProxy()
    {
        base.DoAddProxy();
        syncProxy.Add((uint)MessageId.Types.Enum.Move);
        syncProxy.Add((uint)MessageId.Types.Enum.OtherEnterTreasurePush);
        syncProxy.Add((uint)MessageId.Types.Enum.OtherLeaveTreasurePush);
        syncProxy.Add((uint)MessageId.Types.Enum.OtherDoActionPush);
        syncProxy.Add((uint)MessageId.Types.Enum.OtherChangeSkinPush);
        syncProxy.Add((uint)MessageId.Types.Enum.NetStatePush);
    }
    public override void BindNetAgent(WebSocketAgent agent)
    {
        base.BindNetAgent(agent);
        RoomSyncConst.SetSyncFrame();
        pauseSyncMove = false;
    }
    public override void UnBindNetAgent()
    {
        base.UnBindNetAgent();
        RoomSyncConst.ResumeSyncFrame();
        lastMovingFrame = 0;
        pauseSyncMove = true;
    }

    public void EnterRoom()
    {
        treasureController = Singleton<TreasuringController>.Instance;
        UnBindNetAgent();
        treasureController.Leave();
        BindNetAgent(WebSocketAgent.Ins);
        treasureController.Enter();

        Debug.Log($"111111111 EnterRoom RoomData = {TreasureModel.Instance.RoomData}   ");
        treasureController.CreateUserInfo(TreasureModel.Instance.RoomData.UserList);
        treasureController.OnReproduceScene();
    }

    public void LeaveRoom()
    {
        UnBindNetAgent();
        treasureController.Leave();
    }


    public override void Push(uint proxy, ByteString dataBytes)
    {
        MessageId.Types.Enum msgId = (MessageId.Types.Enum)proxy;
        switch (msgId)
        {
            case MessageId.Types.Enum.Move:
                Move move = Move.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111 push  MOVE   userId: {move.FromUserId}  index:{move.Index}  ");
                if (move.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (treasureController.TryGetPlayerImp(move.FromUserId, out var p))
                    p.AttachMoveMsg(move);
                break;
            case MessageId.Types.Enum.OtherEnterTreasurePush:
                OtherEnterTreasurePush enterPush = OtherEnterTreasurePush.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111  {MessageId.Types.Enum.OtherEnterTreasurePush}   enterPush:{enterPush} ");
                treasureController.CreateUserInfo(enterPush.NewUserInfo);
                break;
            case MessageId.Types.Enum.OtherLeaveTreasurePush:
                OtherLeaveTreasurePush leavePush = OtherLeaveTreasurePush.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111  {MessageId.Types.Enum.OtherLeaveTreasurePush}   leavePush:{leavePush} ");
                //if (leavePush.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                treasureController.RemoveRoomPlayer(leavePush.FromUserId);
                break;
            case MessageId.Types.Enum.OtherDoActionPush:
                OtherDoActionPush otherDoAction = OtherDoActionPush.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111  {MessageId.Types.Enum.OtherDoActionPush}   otherDoAction:{otherDoAction} ");
                //if (otherDoAction.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (treasureController.TryGetPlayerImp(otherDoAction.FromUserId, out var imp))
                    imp.AttachActionMsg(otherDoAction);
                break;
            case MessageId.Types.Enum.OtherChangeSkinPush:
                OtherChangeSkinPush changeSkinPush = OtherChangeSkinPush.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111  {MessageId.Types.Enum.OtherChangeSkinPush}   changeSkinPush:{changeSkinPush} ");
                if (treasureController.TryGetPlayerImp(changeSkinPush.FromUserId, out var playerSkin))
                    playerSkin.AttachChangeSkinMsg(changeSkinPush);
                break;
            case MessageId.Types.Enum.NetStatePush:
                RoomUserInfo netStatePush = RoomUserInfo.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111  {MessageId.Types.Enum.NetStatePush}   netStatePush:{netStatePush} ");
                treasureController.SetPlayerNetOfflinkEnable(netStatePush.UserId, netStatePush.NetworkState);
                break;
        }
    }
    public override void OnMsgError(uint proxy, int errorCode)
    {
        base.OnMsgError(proxy, errorCode);
        if (!agent.IsConnected)
        {
            pauseSyncMove = true;
            offlineStartTime = Time.realtimeSinceStartup;
        }
    }
    public override void OnNetRestore()
    {
        base.OnNetRestore();
        pauseSyncMove = false;
    }


    public override void SyncSelfMove(float ty, Vector3 pos, float nowDis, int state)
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
    public override void SendMove(Vector2 joydeltaPos, Vector3 pos, float nowDis, int state)
    {
        Move move = new Move();
        move.Index = GetCode;
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
    public override void SendMove(float ry, Vector3 pos, float nowDis, int state)
    {
        // x*1000  y*1000
        Move move = new Move();
        move.Index = GetCode;
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