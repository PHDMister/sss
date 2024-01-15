using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class EditorAppearanceTipsUIForm : BaseUIForm
{
    void Awake()
    {
        //���������
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* ��ť��ע��  */
        RigisterButtonObjectEvent("BtnCancel",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            }
        );
        RigisterButtonObjectEvent("BtnExit",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                UIManager.GetInstance().CloseUIForms(FormConst.APPEARANCEEDITORUIFORM);
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
    public void OnClickGet()
    {
        string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019499, 1484);
        Debug.Log("uRl: " + uRl);
        //������˳�
        try
        {
            SetTools.SetPortraitModeFun();
            //��ʾtop��
            SetTools.CloseGameFun();
            SetTools.OpenWebUrl(uRl);

        }
        catch (System.Exception e)
        {
            Debug.Log("��������ݣ� " + e);
        }
    }
}
