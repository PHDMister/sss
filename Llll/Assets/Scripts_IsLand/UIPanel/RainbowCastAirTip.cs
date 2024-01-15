//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class RainbowCastAirTip : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_btn_ok;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_btn_ok = FindComp<Button>("Frame 1000008537/Group 1000008327/Frame 1000008567/Group 1000008326/btn_ok");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_ok, Onbtn_okClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_okClicked(GameObject go)
    {
        CloseUIForm();
        //将角色移动到出生点
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(true);
        Singleton<RainbowSeabedController>.Instance.TakeOffDivingEquipment(ManageMentClass.DataManagerClass.userId);
        Singleton<RainbowSeabedController>.Instance.ResetBirthPos(ManageMentClass.DataManagerClass.userId);
        MessageCenter.SendMessage(SeabedSafeRange.Event_Start, "start", null);
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

    }

    public override void Display()
    {
        base.Display();
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(false);
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
