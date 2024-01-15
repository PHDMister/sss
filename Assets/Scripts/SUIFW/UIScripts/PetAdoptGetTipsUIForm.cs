using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class PetAdoptGetTipsUIForm : BaseUIForm
{
    public Text m_TextDesc;
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnGet",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                OnClickGet();
            }
        );
        RigisterButtonObjectEvent("BtnCancel",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            }
        );
    }
    public override void Display()
    {
        base.Display();
        m_TextDesc.text = string.Format("未拥有{0}无法领养", ManageMentClass.DataManagerClass.petAdoptCostItem);
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }
    public void OnClickGet()
    {
        string uRl = "";
        if(ManageMentClass.DataManagerClass.isLinkEdition && ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019499, 1484);
        }
        else
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019189, 1081);
        }

        Debug.Log("uRl: " + uRl);
        //点击了退出
        try
        {
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
            SetTools.OpenWebUrl(uRl);

        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
}
