using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;

public class PetTrainTipsUIForm : BaseUIForm
{
    void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        RigisterButtonObjectEvent("PetTrainTipBtnClose",
            p =>
            {
                CloseUIForm();
            }
            );
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
}
