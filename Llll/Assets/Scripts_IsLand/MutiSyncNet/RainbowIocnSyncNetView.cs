using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UnityEngine;

public class RainbowIocnSyncNetView : RainbowBeachSyncNetView
{

    public override void EnterRoom()
    {
        RoomSyncConst.SetSyncFrame();

        RainbowIocnController rbController = Singleton<RainbowIocnController>.Instance;
        UnBindNetAgent();
        rbController.Leave();
        BindNetAgent(WebSocketAgent.Ins);

        //Debug.Log($"111111111 RainbowIocnSyncNetView RoomData = {rbController.RoomData}   ");
        rbController.Enter();
    }
    public override void LeaveRoom()
    {
        UnBindNetAgent();
        RainbowIocnController rbController = Singleton<RainbowIocnController>.Instance;
        rbController.Leave();
    }
    public override void Push(uint proxy, ByteString dataBytes)
    {
        MessageId.Types.Enum msgId = (MessageId.Types.Enum)proxy;
        RainbowIocnController rbController = Singleton<RainbowIocnController>.Instance;
        switch (msgId)
        {
            case MessageId.Types.Enum.Move:
                Move move = Move.Parser.ParseFrom(dataBytes);
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



}
