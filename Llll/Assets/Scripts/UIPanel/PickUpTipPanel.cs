using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
//一键收起提示面板
public class PickUpTipPanel : BaseUIForm
{
    private bool AffirmPickUp = true;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence; //半透明，不能穿透
        RigisterButtonObjectEvent("Cancel_Btn", p =>
        {
            Debug.Log("1");
            //关闭本面板
            CloseUIForm();
        });
        RigisterButtonObjectEvent("Affirm_Btn", p =>
        {
            Debug.Log("2");
            //检测并取消角色与建筑的交互
            Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative(1);

            SendMessage("PickTipAffirm", "data", "affirm");
            //关闭面板
            CloseUIForm();
        });
    }
}
