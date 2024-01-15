using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UnityEngine;

/// <summary>
/// 客厅同步
/// </summary>
public class ParlorSyncNetView : BaseNetView, ISingleton
{
    private ParlorController parlorController;

    private ParlorChatBubbleController parlorChatBubbleController;
    private ParlorVirtualOwnerController parlorVirtualOwnerController;
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
        syncProxy.Add((uint)MessageId.Types.Enum.SitdownPush);
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
    
    public void EnterRoom(EnterTreasureResp enterTreasureResp)
    {
        parlorController = Singleton<ParlorController>.Instance;
        parlorController.RoomData = enterTreasureResp.RoomInfo;
        UnBindNetAgent();
        parlorController.Leave();
        BindNetAgent(WebSocketAgent.Ins);
        parlorController.Enter();

        Debug.Log($"111111111 Enter Parlor RoomData = {parlorController.RoomData}   ");
        parlorController.CreateUserInfo(parlorController.RoomData.UserList);
        parlorController.OnReproduceScene();

        parlorChatBubbleController = Singleton<ParlorChatBubbleController>.Instance;
        parlorChatBubbleController.RoomData = enterTreasureResp.RoomInfo;
        parlorChatBubbleController.UserList = enterTreasureResp.RoomInfo.UserList;
        
        parlorVirtualOwnerController = Singleton<ParlorVirtualOwnerController>.Instance;
        parlorVirtualOwnerController.Room = enterTreasureResp.RoomInfo;
        parlorVirtualOwnerController.OwnerInfo = enterTreasureResp.OwnerInfo;
        parlorVirtualOwnerController.EnterRoom();
    }
    public void LeaveRoom()
    {
        UnBindNetAgent();
        parlorController.Leave();
      
        parlorVirtualOwnerController.LeaveRoom();
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
                if (parlorController.TryGetPlayerImp(move.FromUserId, out var p))
                    p.AttachMoveMsg(move);
                break;
            case MessageId.Types.Enum.OtherEnterTreasurePush:
                OtherEnterTreasurePush enterPush = OtherEnterTreasurePush.Parser.ParseFrom(dataBytes);
                if (!parlorController.TryGetPlayerImp(enterPush.NewUserInfo.UserId, out var impOther))
                    parlorController.CreateUserInfo(enterPush.NewUserInfo);
                else
                    parlorController.SetPlayerNetOfflinkEnable(enterPush.NewUserInfo.UserId, enterPush.NewUserInfo.NetworkState);
                
                parlorChatBubbleController.OnOtherEnterTreasurePush(enterPush);
                parlorVirtualOwnerController.OnOtherEnterTreasurePush(enterPush);
                break;
            case MessageId.Types.Enum.OtherLeaveTreasurePush:
                OtherLeaveTreasurePush leavePush = OtherLeaveTreasurePush.Parser.ParseFrom(dataBytes);
                parlorChatBubbleController.OnOtherLeaveTreasurePush(leavePush);
                parlorVirtualOwnerController.OnOtherLeaveTreasurePush(leavePush);
                
                parlorController.RemoveRoomPlayer(leavePush.FromUserId);
                break;
            case MessageId.Types.Enum.OtherDoActionPush:
                OtherDoActionPush otherDoAction = OtherDoActionPush.Parser.ParseFrom(dataBytes);
                if (parlorController.TryGetPlayerImp(otherDoAction.FromUserId, out var imp))
                    imp.AttachActionMsg(otherDoAction);
                break;
            case MessageId.Types.Enum.OtherChangeSkinPush:
                OtherChangeSkinPush changeSkinPush = OtherChangeSkinPush.Parser.ParseFrom(dataBytes);
                if (parlorController.TryGetPlayerImp(changeSkinPush.FromUserId, out var playerSkin))
                    playerSkin.AttachChangeSkinMsg(changeSkinPush);
                break;
            case MessageId.Types.Enum.NetStatePush:
                RoomUserInfo netStatePush = RoomUserInfo.Parser.ParseFrom(dataBytes);
                parlorController.SetPlayerNetOfflinkEnable(netStatePush.UserId, netStatePush.NetworkState);
                break;
            case MessageId.Types.Enum.SitdownPush:
                SitdownPush sitdownPush = SitdownPush.Parser.ParseFrom(dataBytes);
                if (sitdownPush.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (parlorController.TryGetPlayerImp(sitdownPush.FromUserId, out var interactivePlayer))
                    interactivePlayer.AttachInteractiveStateMsg(sitdownPush);
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
}
