using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;

public class CreatePetTipsUIForm : BaseUIForm
{
    PetAdoptV2ReqData _data = null;
    private void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnConfirm", p =>
        {
            CloseUIForm();
            MessageManager.GetInstance().RequestPetAdoptV2(_data, () =>
            {
                MessageManager.GetInstance().RequestPetList(() =>
                {
                    PetSpanManager.Instance().UpdatePet();
                });
            });
        });

        RigisterButtonObjectEvent("BtnCancel", p =>
        {
            CloseUIForm();
        });

        ReceiveMessage("PetAdoptV2", p =>
         {
             _data = p.Values as PetAdoptV2ReqData;
         });
    }
}
