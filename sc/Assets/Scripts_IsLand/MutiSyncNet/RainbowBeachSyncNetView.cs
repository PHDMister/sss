using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;

public class RainbowBeachSyncNetView : BaseNetView, ISingleton
{


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
        syncProxy.Add((uint)MessageId.Types.Enum.RoomRewardPush);
        syncProxy.Add((uint)MessageId.Types.Enum.RewardPush);
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

    public override void EnterRoom()
    {
        RainbowBeachController rbController = Singleton<RainbowBeachController>.Instance;
        UnBindNetAgent();
        rbController.Leave();
        BindNetAgent(WebSocketAgent.Ins);
        rbController.Enter();
    }
    public override void LeaveRoom()
    {
        UnBindNetAgent();
        RainbowBeachController rbController = Singleton<RainbowBeachController>.Instance;
        rbController.Leave();
    }


    public override void Push(uint proxy, ByteString dataBytes)
    {
        MessageId.Types.Enum msgId = (MessageId.Types.Enum)proxy;
        RainbowBeachController rbController = Singleton<RainbowBeachController>.Instance;
        switch (msgId)
        {
            case MessageId.Types.Enum.Move:
                Move move = Move.Parser.ParseFrom(dataBytes);
                //Debug.Log($"1111111111 push  MOVE   userId: {move.FromUserId}  index:{move.Index}  ");
                if (move.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (rbController.TryGetPlayerImp(move.FromUserId, out var p))
                    p.AttachMoveMsg(move);
                break;
            case MessageId.Types.Enum.OtherEnterTreasurePush:
                OtherEnterTreasurePush enterPush = OtherEnterTreasurePush.Parser.ParseFrom(dataBytes);
                if (!rbController.TryGetPlayerImp(enterPush.NewUserInfo.UserId, out var impOther))
                    rbController.CreateUserInfo(enterPush.NewUserInfo);
                else
                    rbController.SetPlayerNetOfflinkEnable(enterPush.NewUserInfo.UserId, enterPush.NewUserInfo.NetworkState);
                break;
            case MessageId.Types.Enum.OtherLeaveTreasurePush:
                OtherLeaveTreasurePush leavePush = OtherLeaveTreasurePush.Parser.ParseFrom(dataBytes);
                rbController.RemoveRoomPlayer(leavePush.FromUserId);
                break;
            case MessageId.Types.Enum.OtherDoActionPush:
                OtherDoActionPush otherDoAction = OtherDoActionPush.Parser.ParseFrom(dataBytes);
                if (rbController.TryGetPlayerImp(otherDoAction.FromUserId, out var imp))
                    imp.AttachActionMsg(otherDoAction);
                break;
            case MessageId.Types.Enum.OtherChangeSkinPush:
                OtherChangeSkinPush changeSkinPush = OtherChangeSkinPush.Parser.ParseFrom(dataBytes);
                if (rbController.TryGetPlayerImp(changeSkinPush.FromUserId, out var playerSkin))
                    playerSkin.AttachChangeSkinMsg(changeSkinPush);
                break;
            case MessageId.Types.Enum.NetStatePush:
                RoomUserInfo netStatePush = RoomUserInfo.Parser.ParseFrom(dataBytes);
                rbController.SetPlayerNetOfflinkEnable(netStatePush.UserId, netStatePush.NetworkState);
                break;
            case MessageId.Types.Enum.RoomRewardPush:
                RoomRewardPush rrp = RoomRewardPush.Parser.ParseFrom(dataBytes);
                if (rrp.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (rbController.TryGetPlayerImp(rrp.FromUserId, out var imp1))
                {
                    rrp.Index = imp1.CurMaxIndex + 1;
                    imp1.AttachRainbowPlayerStateMsg(rrp);
                }
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
