using System.Linq;
using Google.Protobuf.Collections;
using Treasure;
using UnityEngine;

public class ParlorChatBubbleController : ISingleton
 {
     private Room roomData = new Room();
     private RepeatedField<RoomUserInfo> userList;

     public void Init()
     {

     }
    
     public Room RoomData
     {
         get
         {
             return roomData;
         }
         set
         {
             roomData = value;
         }
     }

     public RepeatedField<RoomUserInfo> UserList
     {
         get
         {
             return userList;
         }
         set
         {
             userList = value;
         }
     }

     public void ShowChatBubble(ChatData data, int chatType)
     {
         ulong userId = 0;
         {
             foreach (var player in RoomData.UserList)
             {
                 if (player.YunxinAccid.Equals(data.fromAccId))
                 {
                     userId = player.UserId;
                     break;
                 }
             }
             Debug.Log("ShowChatBubble containsKey");
             Debug.Log(Singleton<ParlorController>.Instance.Players.ContainsKey(userId));
             if (Singleton<ParlorController>.Instance.Players.TryGetValue(userId, out var imp))
             {
                 imp.LookFollowHud.SetChatBubble(data);
             }
         }
     }

     public void HideChatBubble(ChatData data)
     {
         ulong userId = 0;
         foreach (var player in RoomData.UserList)
         {
             if (player.YunxinAccid.Equals(data.fromAccId))
             {
                 userId = player.UserId;
                 break;
             }
         }

         if (Singleton<ParlorController>.Instance.Players.TryGetValue(userId, out var imp))
         {
             imp.LookFollowHud.HideChatBubble();
         }
     }

     /// <summary>
     /// 他人加入房间推送
     /// </summary>
     /// <param name="clientCode"></param>
     /// <param name="data"></param>
     public void OnOtherEnterTreasurePush(OtherEnterTreasurePush enterTreasurePush)
     {
         if (!UserList.ToList().Exists(x => x.UserId == enterTreasurePush.NewUserInfo.UserId))
         {
             UserList.Add(enterTreasurePush.NewUserInfo);
         }

     }
     
     /// <summary>
     /// 他人离开房间推送
     /// </summary>
     /// <param name="clientCode"></param>
     /// <param name="data"></param>
     public void OnOtherLeaveTreasurePush(OtherLeaveTreasurePush otherLeaveTreasurePush)
     {
         for (int i = UserList.Count - 1; i >= 0; i--)
         {
             if (UserList[i].UserId == otherLeaveTreasurePush.FromUserId)
             {
                 UserList.Remove(UserList[i]);
             }
         }
     }

 }