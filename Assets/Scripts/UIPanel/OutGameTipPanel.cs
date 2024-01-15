using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class OutGameTipPanel : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Fixed;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
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
        //点击了退出
        try
        {

            Debug.Log("强制退出");
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
}
