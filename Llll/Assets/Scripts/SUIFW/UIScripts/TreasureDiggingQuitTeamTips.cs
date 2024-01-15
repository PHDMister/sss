using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;

public class TreasureDiggingQuitTeamTips : BaseUIForm
{
    public void Awake()
    {
        //���������
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //ע��������ǵ��¼�
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
    /// �뿪����
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
