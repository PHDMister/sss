using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class OutGameTipPanel : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Fixed;  //��������
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //��͸�������ܴ�͸
    }
    private void Start()
    {
        RigisterButtonObjectEvent("OutGame_Btn", p =>
        {
            OutGameFun();
        });
    }
    private void OutGameFun()
    {
        //������˳�
        try
        {

            Debug.Log("ǿ���˳�");
            SetTools.SetPortraitModeFun();
            //��ʾtop��
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("��������ݣ� " + e);
        }
    }
}
