using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;

public class TreasureDiggingRoomList : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_RoomVerticalScroll;
    private RoomListResp m_RoomData;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

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
                    RoomUserInfo selfInfo = Singleton<TreasuringController>.Instance.GetSelfUserInfo();
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
        InvokeRepeating("RequestRoonList", 0, 5f);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
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

    public void UpdateRoomListData(RoomListResp data)
    {
        m_RoomData = data;
    }

    public void RequestRoonList()
    {
        if (TreasureModel.Instance.bReqRoomList)
        {
            TreasureModel.Instance.bReqRoomList = false;
            MessageManager.GetInstance().SendRoomList();
        }
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
}
