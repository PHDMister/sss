using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;

public class AccountLoginTips : BaseUIForm
{
    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnConfirm", p =>
        {
            OnClickQuit();
        });
    }

    private void OnClickQuit()
    {
        CloseUIForm();
        try
        {
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
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
