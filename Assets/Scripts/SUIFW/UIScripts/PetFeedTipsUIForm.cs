using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class PetFeedTipsUIForm : BaseUIForm
{
    public Text m_TextCost;
    private PetListRecData data = null;
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnConfirm",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                if (data != null)
                {
                    pet_keeping config = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(data.keep_id);
                    if (config != null)
                    {
                        int cost = config.price;
                        if (cost > ManageMentClass.DataManagerClass.gas_Amount)
                        {
                            ToastManager.Instance.ShowPetToast("您的GAS不足，无法喂养羊奶", 3f);
                            OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
                        }
                        else
                        {
                            //喂养
                            PetFeedData petAdoptData = new PetFeedData(data.id,1);
                            string Strdata = JsonConvert.SerializeObject(petAdoptData);
                            MessageManager.GetInstance().RequestFeed(Strdata, (JObject jo) =>
                            {
                                SendMessage("FeedSuccess", "Success", null);
                            });
                        }
                    }
                }
            }
        );
        RigisterButtonObjectEvent("BtnCancel",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            }
        );
        ReceiveMessage("HungryOpt",
            p =>
             {
                 data = p.Values as PetListRecData;
                 if (data != null)
                 {
                     pet_keeping config = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(data.keep_id);
                     if (config != null)
                     {
                         m_TextCost.text = string.Format("{0}", config.price);
                     }
                 }
             }
         );
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
    }
}
