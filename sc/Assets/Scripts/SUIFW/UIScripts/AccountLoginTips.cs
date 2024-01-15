using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;

public class AccountLoginTips : BaseUIForm
{
    public void Awake()
    {
        //���������
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        //ע��������ǵ��¼�
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
            //��ʾtop��
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("��������ݣ� " + e);
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
