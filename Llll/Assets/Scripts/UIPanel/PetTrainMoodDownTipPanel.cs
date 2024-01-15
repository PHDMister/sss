using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
public class PetTrainMoodDownTipPanel : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //取消按钮
        RigisterButtonObjectEvent("PetTrainMoodDownCanel_Btn", p =>
        {
            CloseUIForm();
        });

        //确认按钮
        RigisterButtonObjectEvent("PetTrainMoodDownAffirm_Btn", p =>
        {
            CloseUIForm();
            SendMessage("PetTrainMoodDownTipPanelAffirmBtn", "Success", null);
        });
    }
}
