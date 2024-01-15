using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using DG.Tweening;

public class TreasureExchangeSuccessTips : BaseUIForm
{
    public Transform imgTicket;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        RigisterButtonObjectEvent("Click_Button",
      p =>
      {
          CloseUIForm();
      });
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        PlayerAnimFun();
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }


    private void PlayerAnimFun()
    {
        if (imgTicket == null)
        {
            return;
        }
        DOTween.To(delegate (float value)
        {
            imgTicket.localScale = new Vector3(value, value, value);
        }, 0f, 1.2f, 0.7f).OnComplete(() =>
        {

            DOTween.To(delegate (float value)
            {
                imgTicket.localScale = new Vector3(value, value, value);
            }, 1.2f, 1f, 0.1f);
        });
    }

}
