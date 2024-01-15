using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class ShopNewGasTipsUIForm : BaseUIForm
{
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnConfirm",
            p => OnBuyConfirm()
            );

        RigisterButtonObjectEvent("BtnCancel",
            p => OnBuyCancel()
            );
    }


    private void OnBuyConfirm()
    {

        if (!ManageMentClass.DataManagerClass.WebInto)
        {
            //跳转到彩虹集市的方法
            InterfaceHelper.GotoRainbowBazaarFun();
        }
        else
        {
            CloseUIForm();
            ToastManager.Instance.ShowNewToast("请前往App端兑换GAS", 5f);
        }
       
    }

    private void OnBuyCancel()
    {
        CloseUIForm();
    }
}
