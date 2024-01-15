//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class UITreasureHiatusTip : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_quxiao;
    private Button button_button_queding;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_quxiao = FindComp<Button>("dilog-yidong/buttonzhu/button-quxiao");
        button_button_queding = FindComp<Button>("dilog-yidong/buttonzhu/button-queding");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_button_quxiao, Onbutton_quxiaoClicked);
        RigisterCompEvent(button_button_queding, Onbutton_quedingClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private float rTime = 0;
    private void Onbutton_quxiaoClicked(GameObject go)
    {
        if (Time.realtimeSinceStartup - rTime < WebSocketAgent.Config.TimeOutShowLoading) return;
        rTime = Time.realtimeSinceStartup;
        //发送离队请求
        LeaveTeamReq req = new LeaveTeamReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.LeaveTeamReq, req, (code, bytes) =>
         {
             if (code == 0)
             {
                 CloseUIForm();
                 Singleton<TreasuringController>.Instance.ReproduceSelfUserOnLeaveTeam();
             }
         });
    }
    private void Onbutton_quedingClicked(GameObject go)
    {
        //继续挖宝
        CloseUIForm();
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

    }

    public override void Display()
    {
        base.Display();
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

}
