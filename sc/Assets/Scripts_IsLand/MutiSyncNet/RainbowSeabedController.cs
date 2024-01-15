using System.Collections;
using System.Collections.Generic;
using Treasure;
using UnityEngine;
//海底控制器
public class RainbowSeabedController : BaseSyncController, ISingleton
{
    public Room RoomData = new Room();
    public RoomUserInfo UserInfo
    {
        get
        {
            foreach (var userInfo in RoomData.UserList)
            {
                if (userInfo.UserId == SelfUserId) return userInfo;
            }
            return null;
        }
    }

    public void Init()
    {
        
    }

    public override void Enter()
    {
        
    }
    public override void Leave()
    {
        
    }


}
