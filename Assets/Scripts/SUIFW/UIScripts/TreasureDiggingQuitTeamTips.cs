using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;

public class TreasureDiggingQuitTeamTips : BaseUIForm
{
    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnConfirm", p =>
        {
            OnClickLeave();
        });

        RigisterButtonObjectEvent("BtnCancel", p =>
        {
            CloseUIForm();
        });
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }

    /// <summary>
    /// 离开队伍
    /// </summary>
    public void OnClickLeave()
    {
        LeaveTeamReq leaveTeamReq = new LeaveTeamReq();
        leaveTeamReq.UserId = ManageMentClass.DataManagerClass.userId;
        //TreasureModel.Instance.LeaveUserId = leaveTeamReq.UserId;

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.LeaveTeamReq, leaveTeamReq, (code, bytes) =>
        {
            if (code != 0) return;
            LeaveRoomResp leaveRoomResp = LeaveRoomResp.Parser.ParseFrom(bytes);
            if (leaveRoomResp.StatusCode == 0)
            {
                CloseUIForm();
                Debug.Log($"[WebSocket] LeaveRoomResp Success");
            }
        });
    }
}
