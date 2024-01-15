using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class PetFeedDieTipsUIForm : BaseUIForm
{
    private PetListRecData data = null;
    private string petName = "";
    public Text m_TxtDesc = null;

    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnConfirm",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                MessageManager.GetInstance().RequestPetList(() =>
                {
                    SendMessage("AdoptSuccess", "Success", null);
                });
            }
        );

        ReceiveMessage("RecyclePet", p =>
            {
                petName = p.Values.ToString();
                m_TxtDesc.text =string.Format( "您的宠物{0}因未得到有效的照顾，已被强制收容",petName);
            });
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        m_TxtDesc.text = "您领养的宠物未得到有效的照顾，已被强制收容";
    }
}
