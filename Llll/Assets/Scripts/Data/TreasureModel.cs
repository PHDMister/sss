using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;

public class TreasureModel : ISingleton
{
    public static TreasureModel Instance => Singleton<TreasureModel>.Instance;
    private Room roomData = new Room();
    private RepeatedField<RoomUserInfo> userList;
    private RoomUserInfo inviteFromUserInfo;//好友邀请邀请者来源
    private float reciveCurTime;
    private int curTime;
    private int startTime;
    private int endTime;
    //private ulong leaveUserId = 0;//离开队伍玩家Id
    public List<InvitePushData> InvitePushList = new List<InvitePushData>();//邀请列表
    public List<RoomUserInfo> TeamUserList = new List<RoomUserInfo>();//队伍列表
    private List<TreasureFriendListRecData> friendList = new List<TreasureFriendListRecData>();//好友数据
    private bool _bCostTicket = false;//当天是否消耗过寻宝券
    public bool bRecTicketNum = false;
    public List<TreasureFriendListRecData> OptInviteFriendList = new List<TreasureFriendListRecData>();//操作过的好友列表
    public List<RoomUserInfo> OptInviteNearList = new List<RoomUserInfo>();//操作过的附近列表
    public List<TreasureRewardConfData> RewardPreviewData = new List<TreasureRewardConfData>();//奖励预览数据
    public bool bInNpcNear = false;
    public uint TreasureEfficiencyTime;
    public uint TreasureEfficiencyBuff;
    public Dictionary<int, Sprite> m_CollectionSpriteDic = new Dictionary<int, Sprite>();
    public bool bRecNoticePush = false;
    public bool bReqRoomList = true;

    // public string NewPlayerName;

    public void Init()
    {

    }

    public void AddLoginEvent()
    {
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.KickOfflinePush, OnKickOfflinePush);
    }

    private void OnKickOfflinePush(uint code, ByteString dataBytes)
    {
        KickOfflinePush kickOfflinePush = KickOfflinePush.Parser.ParseFrom(dataBytes);
        ulong userId = kickOfflinePush.UserId;
        UIManager.GetInstance().ShowUIForms(FormConst.ACCOUNTLOGINTIPS);
    }

    public void TreasureEndHandle(TreasureActivityEndPush data)
    {
        StartTime = (int)data.Start;
        EndTime = (int)data.End;
        TreasureStartTimer.Instance().Init();
        if (UIManager.GetInstance().IsOpend(FormConst.TREASUREDIGGINGMAINMENU))
        {
            TreasureDiggingMainMenu uiForm = UIManager.GetInstance().GetUIForm(FormConst.TREASUREDIGGINGMAINMENU) as TreasureDiggingMainMenu;
            if (uiForm != null)
            {
                uiForm.SetRoomInfo();
            }
        }
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

    public RoomUserInfo InviteFromUserInfo
    {
        get
        {
            return inviteFromUserInfo;
        }
        set
        {
            inviteFromUserInfo = value;
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

    public int CurTime
    {
        get
        {
            return curTime + Mathf.FloorToInt(Time.realtimeSinceStartup - reciveCurTime);
        }
        set
        {
            curTime = value;
            reciveCurTime = Time.realtimeSinceStartup;
        }
    }

    public int StartTime
    {
        get
        {
            return startTime;
        }
        set
        {
            startTime = value;
        }
    }

    public int EndTime
    {
        get
        {
            return endTime;
        }
        set
        {
            endTime = value;
        }
    }

    //public ulong LeaveUserId
    //{
    //    get
    //    {
    //        return leaveUserId;
    //    }
    //    set
    //    {
    //        leaveUserId = value;
    //    }
    //}

    public List<TreasureFriendListRecData> FriendList
    {
        get
        {
            return friendList;
        }
        set
        {
            friendList = value;
        }
    }

    public bool bCostTicket
    {
        get
        {
            return _bCostTicket;
        }
        set
        {
            _bCostTicket = value;
        }
    }

    public void RemoveUserListExceptSelf(uint roomId)
    {
        for (int i = UserList.Count - 1; i >= 0; i--)
        {
            if (UserList[i].UserId != ManageMentClass.DataManagerClass.userId)
            {
                UserList.Remove(UserList[i]);
            }
            else
            {
                UserList[i].RoomId = roomId;
            }
        }
    }

    public void RemoveTeamUserListExceptSelf(uint roomId)
    {
        for (int i = TeamUserList.Count - 1; i >= 0; i--)
        {
            if (TeamUserList[i].UserId != ManageMentClass.DataManagerClass.userId)
            {
                TeamUserList.Remove(TeamUserList[i]);
            }
            else
            {
                TeamUserList[i].RoomId = roomId;
            }
        }
    }

    public bool IsTreasureOpen()
    {
        return (StartTime <= 0 && EndTime <= 0) || (StartTime <= CurTime && CurTime < EndTime);
    }

    public void UpdateRoomPlayer(RepeatedField<RoomUserInfo> userList, bool bJump)
    {
        UserList.Clear();
        UserList.AddRange(userList);
        RoomData.UserList.Clear();
        RoomData.UserList.AddRange(userList);

        foreach (var player in UserList)
        {
            if (player.UserId == ManageMentClass.DataManagerClass.userId)
            {
                player.UserName = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            }
        }

        foreach (var player in RoomData.UserList)
        {
            if (player.UserId == ManageMentClass.DataManagerClass.userId)
            {
                player.UserName = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            }
        }

        if (bJump)
        {
             Singleton<RoomSyncNetView>.Instance.EnterRoom();
        }
    }

    //房间内玩家是否是自己
    public bool bSelf()
    {
        if (RoomData == null)
            return false;
        foreach (var info in RoomData.UserList)
        {
            if (info.UserId == ManageMentClass.DataManagerClass.userId)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveInvitePushList(ulong userId)
    {
        for (int i = InvitePushList.Count - 1; i >= 0; i--)
        {
            if (InvitePushList[i].PushData.FromUserInfo.UserId == userId)
            {
                InvitePushList.RemoveAt(i);
            }
        }
    }

    public RoomUserInfo GetSelfInfo()
    {
        foreach (var info in RoomData.UserList)
        {
            if (info.UserId == ManageMentClass.DataManagerClass.userId)
            {
                return info;
            }
        }
        return null;
    }

    public void ShowChatBubble(ChatData data, int chatType)
    {
        ulong userId = 0;
        if (chatType == (int)ChatType.Team)
        {
            foreach (var player in TeamUserList)
            {
                if (player.YunxinAccid.Equals(data.fromAccId))
                {
                    userId = player.UserId;
                    break;
                }
            }

            if (Singleton<TreasuringController>.Instance.Players.TryGetValue(userId, out var imp))
            {
                imp.LookFollowHud.SetChatBubble(data);
            }
        }
        else
        {
            foreach (var player in RoomData.UserList)
            {
                if (player.YunxinAccid.Equals(data.fromAccId))
                {
                    userId = player.UserId;
                    break;
                }
            }

            if (Singleton<TreasuringController>.Instance.Players.TryGetValue(userId, out var imp))
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

        if (Singleton<TreasuringController>.Instance.Players.TryGetValue(userId, out var imp))
        {
            imp.LookFollowHud.HideChatBubble();
        }
    }

}

public class InvitePushData
{
    public int PageType;
    public OtherInviteJoinTeamPush PushData;
    public long ReceiveTimestamp;
    public InvitePushData(int pageType, OtherInviteJoinTeamPush pushData, long receiveTimestamp)
    {
        PageType = pageType;
        PushData = pushData;
        ReceiveTimestamp = receiveTimestamp;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            InvitePushData data = (InvitePushData)obj;
            return (PushData.FromUserInfo.UserId == data.PushData.FromUserInfo.UserId);
        }
    }

    public override int GetHashCode()
    {
        return PushData.GetHashCode();
    }
}