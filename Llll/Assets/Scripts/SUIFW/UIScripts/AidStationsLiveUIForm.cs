using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class AidStationsLiveUIForm : BaseUIForm
{
    public RawImage m_RawImg;
    public Toggle m_ToggleGarden;
    public Toggle m_ToggleYard;
    public enum TableType
    {
        Garden = 1,
        Yard = 2,
    }
    public TableType m_TableType = TableType.Garden;

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
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            });

        m_ToggleGarden.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                m_TableType = TableType.Garden;
            else
                m_TableType = TableType.Yard;
        });

        m_ToggleYard.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                m_TableType = TableType.Yard;
            else
                m_TableType = TableType.Garden;
        });
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        Transform m_ToggleGroup = UnityHelper.FindTheChildNode(this.gameObject, "ToggleGroup");
        if (m_ToggleGroup != null)
            m_ToggleGroup.gameObject.SetActive(false);
#if !UNITY_EDITOR
            try
            {
                SetTools.showVideoPlayer();
                Debug.Log("WebGL Platform Play");
            }
            catch (System.Exception e)
            {
                  Debug.Log("Exception： " + e);
            }
#endif
    }

    public override void Hiding()
    {
        base.Hiding();
        m_ToggleGarden.isOn = true;
        m_ToggleYard.isOn = false;
    }
}
