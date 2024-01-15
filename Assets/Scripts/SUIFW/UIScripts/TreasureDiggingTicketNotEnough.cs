using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;

public class TreasureDiggingTicketNotEnough : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnCancel", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnExchange", p =>
       {
           CloseUIForm();
           OpenUIForm(FormConst.TREASUREEXCHANGEPANEL);
       });

    }

    public override void Display()
    {
        base.Display();
    }
    public override void Hiding()
    {
        base.Hiding();
    }
}
