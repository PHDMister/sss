using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class UIChatCostGasTips : BaseUIForm
{
    public Toggle m_Toggle;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        m_Toggle.isOn = PlayerPrefs.GetInt("ChatCostTips") > 0;
        m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);

        RigisterButtonObjectEvent("BtnSend", p =>
         {
             CloseUIForm();
             KeyValuesUpdate kvs = new KeyValuesUpdate("", null);
             MessageCenter.SendMessage("SendAllMsg", kvs);
         });

        RigisterButtonObjectEvent("BtnCancel", p =>
         {
             CloseUIForm();
         });
    }

    private void OnToggleValueChanged(bool isOn)
    {
        PlayerPrefs.SetInt("ChatCostTips", isOn ? 1 : 0);
        PlayerPrefs.Save();
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
