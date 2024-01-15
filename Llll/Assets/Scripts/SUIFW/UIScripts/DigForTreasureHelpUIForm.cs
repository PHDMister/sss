using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class DigForTreasureHelpUIForm : BaseUIForm
{
    //public Text m_TextDesc;
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
        //help helpConfig = ManageMentClass.DataManagerClass.GetHelpTable(2);
        //if (helpConfig != null)
        //{
        //    string strDesc = helpConfig.text;
        //    m_TextDesc.text = strDesc;
        //}
        InterfaceHelper.SetJoyStickState(false);
        ScrollRect m_ScrollRect = UnityHelper.GetTheChildNodeComponetScripts<ScrollRect>(gameObject, "Scroll View");
        if (m_ScrollRect != null)
        {
            if (m_ScrollRect != null)
                m_ScrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }
}
