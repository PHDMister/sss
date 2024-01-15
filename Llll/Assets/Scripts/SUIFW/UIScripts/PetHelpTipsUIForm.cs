using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class PetHelpTipsUIForm : BaseUIForm
{
  //  public Text m_TextDesc;
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnClose",
            p =>
            {
                CloseUIForm();
            });
    }

    public override void Display()
    {
        base.Display();
       /* help helpConfig = ManageMentClass.DataManagerClass.GetHelpTable(1);
        if(helpConfig != null)
        {
            string strDesc = helpConfig.text;
            m_TextDesc.text = strDesc;
        }*/
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }
}
