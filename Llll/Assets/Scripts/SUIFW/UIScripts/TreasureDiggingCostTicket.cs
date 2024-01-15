using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class TreasureDiggingCostTicket : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnCancel",
          p =>
          {
              CloseUIForm();
          });

        RigisterButtonObjectEvent("BtnConfirm",
          p =>
          {
              MessageManager.GetInstance().RequestCostTicket(() =>
              {
                  CloseUIForm();
                  SendMessage("TreasureCostTicket", "Success", null);
              });
          });
    }

    public override void Display()
    {
        base.Display();
    }
}
