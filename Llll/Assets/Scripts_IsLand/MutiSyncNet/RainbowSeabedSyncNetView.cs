using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UnityEngine;
//海底同步
public class RainbowSeabedSyncNetView : BaseNetView, ISingleton
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
        syncProxy.Add((uint)MessageId.Types.Enum.BuffToolStartPush);
        syncProxy.Add((uint)MessageId.Types.Enum.BuffToolEndPush);
    }


    public override void EnterRoom()
    {
        RainbowSeabedController rbController = Singleton<RainbowSeabedController>.Instance;
        UnBindNetAgent();
        rbController.Leave();
        BindNetAgent(WebSocketAgent.Ins);

        //Debug.Log($"111111111 RainbowSeabedSyncNetView RoomData = {rbController.RoomData}   ");
        rbController.Enter();
    }



    public override void LeaveRoom()
    {
        UnBindNetAgent();
        RainbowSeabedController rbController = Singleton<RainbowSeabedController>.Instance;
        rbController.Leave();
    }
    public override void Push(uint proxy, ByteString dataBytes)
    {
        MessageId.Types.Enum msgId = (MessageId.Types.Enum)proxy;
        RainbowSeabedController rbController = Singleton<RainbowSeabedController>.Instance;
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
            case MessageId.Types.Enum.BuffToolStartPush:
                BuffToolStartPush startPush = BuffToolStartPush.Parser.ParseFrom(dataBytes);
                if (startPush.FromUserId == ManageMentClass.DataManagerClass.userId) return;
                if (rbController.TryGetPlayerImp(startPush.FromUserId, out var imp))
                {
                    rbController.DrewDivingEquipment(startPush.FromUserId,
                        startPush.ToolId);
                    imp.UseHelmet(startPush.ToolId);
                }
                break;
            case MessageId.Types.Enum.BuffToolEndPush:
                BuffToolEndPush endPush = BuffToolEndPush.Parser.ParseFrom(dataBytes);
                if (rbController.TryGetPlayerImp(endPush.FromUserId, out var imp2))
                {
                    imp2.UnUseHelmet();
                    rbController.TakeOffDivingEquipment(endPush.FromUserId);
                    rbController.ResetBirthPos(endPush.FromUserId);
                }
                break;
        }
    }
}
