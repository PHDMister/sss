using System.Linq;
using Google.Protobuf;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class RainbowBeachRoomList : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_RoomVerticalScroll;
    private RoomListResp m_RoomData;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence;

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnCreate", p =>
        {
            Room m_SelfRoomData = GetSelfRoomData(ManageMentClass.DataManagerClass.roomId);
            if (m_SelfRoomData != null)
            {
                if (m_SelfRoomData.UserList.Count <= 1)
                {
                    RoomUserInfo selfInfo = Singleton<RainbowBeachController>.Instance.UserInfo;
                    if (selfInfo != null)
                    {
                        ToastManager.Instance.ShowNewToast("当前房间只有你自己，无法创建房间哦~", 5f);
                        return;
                    }
                }
            }
            MessageManager.GetInstance().SendCreateRoom();
        });


        ReceiveMessage("RoomListResp", p =>
         {
             m_RoomData = p.Values as RoomListResp;
             if (m_RoomData == null)
                 return;
             SetRoomListInfo(m_RoomData);
             ResetScrollRect();
         });
    }
    
    public void SetRoomListInfo(RoomListResp roomData)
    {
        if (roomData == null)
            return;
        int count = roomData.RoomList.Count;
        m_RoomVerticalScroll.Init(InitRoomInfoCallBack);
        m_RoomVerticalScroll.ShowList(count);
    }

    public void InitRoomInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        RoomItem roomItem = cell.transform.GetComponent<RoomItem>();
        if (roomItem != null)
        {
            Room room = m_RoomData.RoomList[index - 1];
            if (room == null)
                return;
            roomItem.SetRoomName(room.RoomId.ToString());
            roomItem.SetRoomNum(room.UserList.Count);
            roomItem.SetFullState(room.UserList.Count >= 6);
            roomItem.SetSelfRoomState(room.RoomId == ManageMentClass.DataManagerClass.roomId);
            roomItem.SetRoomData(room);
            roomItem.SetEnterVisiable(room.RoomId != ManageMentClass.DataManagerClass.roomId && room.UserList.Count < 6);
        }
    }
    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        //InvokeRepeating("RequestRoonList", 0, 5f);
        m_RoomVerticalScroll.Init(InitRoomInfoCallBack);
        m_RoomVerticalScroll.ShowList(0);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.RoomListResp, OnRoomListResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.JoinRoomResp, OnJoinRoomResp);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.CreateRoomResp, OnCreateRoomResp);


        MessageManager.GetInstance().SendRoomList();
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.RoomListResp, OnRoomListResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.JoinRoomResp, OnJoinRoomResp);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.CreateRoomResp, OnCreateRoomResp);

        m_RoomVerticalScroll.ShowList(0);
    }

    public Room GetSelfRoomData(uint roomId)
    {
        if (m_RoomData == null)
            return null;
        for (int i = 0; i < m_RoomData.RoomList.Count; i++)
        {
            if (m_RoomData.RoomList[i].RoomId == roomId)
            {
                return m_RoomData.RoomList[i];
            }
        }
        return null;
    }

    public void UpdateRoomData(RoomListResp data)
    {
        if (data == null)
            return;
        m_RoomData = data;
    }

    public void ResetScrollRect()
    {
        if (m_RoomVerticalScroll != null)
            m_RoomVerticalScroll.ResetScrollRect();
    }


    /// <summary>
    /// 房间列表返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnRoomListResp(uint clientCode, ByteString data)
    {
        RoomListResp roomListResp = RoomListResp.Parser.ParseFrom(data);
        if (!UIManager.GetInstance().IsOpend(FormConst.RAINBOWBEACHROOMLIST))
        {
            OpenUIForm(FormConst.RAINBOWBEACHROOMLIST);
            MessageManager.GetInstance().SendMessage("RoomListResp", "Success", roomListResp);
        }
        else if (UIManager.GetInstance().IsOpend(FormConst.RAINBOWBEACHROOMLIST))
        {
            TreasureModel.Instance.bReqRoomList = true;
            RainbowBeachRoomList uiForm = UIManager.GetInstance().GetUIForm<RainbowBeachRoomList>(FormConst.RAINBOWBEACHROOMLIST);
            if (uiForm != null)
            {
                uiForm.UpdateRoomData(roomListResp);
                uiForm.SetRoomListInfo(roomListResp);
            }
        }
    }
    /// <summary>
    /// 创建房间返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnCreateRoomResp(uint clientCode, ByteString data)
    {
        CreateRoomResp createRoomResp = CreateRoomResp.Parser.ParseFrom(data);
        if (createRoomResp.StatusCode == 230015)
        {
            ToastManager.Instance.ShowNewToast("当前正在挖宝，请稍后再试~", 3);
            return;
        }

        if (createRoomResp.Room == null || createRoomResp.Room.UserList == null)
            return;
        
        MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

        ManageMentClass.DataManagerClass.roomId = createRoomResp.Room.RoomId;
        TreasureModel.Instance.RoomData.ChatRoomId = createRoomResp.Room.ChatRoomId;
        TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
        TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(createRoomResp.Room.ChatRoomAddr);

        //缓存数据
        ManageMentClass.CurSyncPlayerController.SetRoomData(createRoomResp.Room);
        //队伍刷新
        UIManager.GetInstance().CloseUIForms(FormConst.RAINBOWBEACHROOMLIST);
        TreasureLoadManager.Instance().Load(LoadType.RainbowCreate);
        //红点Tip
        Singleton<RainbowBeachDataModel>.Instance.OtherShareHongbao = false;
        MessageCenter.SendMessage("UpdateShellRedPointData", KeyValuesUpdate.Empty);
        //调用当前房间控制器
        WebSocketAgent.Ins.NetView.EnterRoom();
        MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);
    }
    /// <summary>
    /// 加入房间返回
    /// </summary>
    /// <param name="clientCode"></param>
    /// <param name="data"></param>
    private void OnJoinRoomResp(uint clientCode, ByteString data)
    {
        JoinRoomResp joinRoomResp = JoinRoomResp.Parser.ParseFrom(data);
        if (joinRoomResp.StatusCode == 230003 || joinRoomResp.StatusCode > 0)
        {
            ToastManager.Instance.ShowNewToast("房间已满", 3);
            MessageManager.GetInstance().SendRoomList();
            return;
        }
        MessageCenter.SendMessage("HideCurChatBubble", KeyValuesUpdate.Empty);

        ManageMentClass.DataManagerClass.roomId = joinRoomResp.RoomId;
        TreasureModel.Instance.RoomData.ChatRoomId = joinRoomResp.ChatRoomId;
        TreasureModel.Instance.RoomData.ChatRoomAddr.Clear();
        TreasureModel.Instance.RoomData.ChatRoomAddr.AddRange(joinRoomResp.ChatRoomAddr);

        //角色刷新
        ManageMentClass.CurSyncPlayerController.SetRoomUser(joinRoomResp.UserList);
        //关闭UI
        UIManager.GetInstance().CloseUIForms(FormConst.RAINBOWBEACHROOMLIST);
        TreasureLoadManager.Instance().Load(LoadType.RainbowJoin);
        //红点Tip
        Singleton<RainbowBeachDataModel>.Instance.OtherShareHongbao = false;
        MessageCenter.SendMessage("UpdateShellRedPointData", KeyValuesUpdate.Empty);
        //
        WebSocketAgent.Ins.NetView.EnterRoom();
        MessageCenter.SendMessage("RefreshUIChat", KeyValuesUpdate.Empty);
    }

}
