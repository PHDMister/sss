using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class PetAdoptTipsUIForm : BaseUIForm
{
    //  public Text m_TextDesc;
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnAdopt",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                if (ManageMentClass.DataManagerClass.petEnableAdoptStatus == 0)
                {
                    OpenUIForm(FormConst.PETADOPTGETTIPS_UIFORM);
                }
                else
                {
                    MessageManager.GetInstance().RequestAdopt(() =>
                    {
                        MessageManager.GetInstance().RequestPetList(() =>
                        {
                            PetSpanManager.Instance().UpdatePet();
                            SendMessage("AdoptSuccess", "Success", null);
                        });
                        MessageManager.GetInstance().RequestEnableAdopt(1, () =>
                         {
                            //SendMessage("UpdateCostItemValue", "CostItemValue", ManageMentClass.DataManagerClass.petAdoptCostItemNum);
                        });
                    });
                }
            }
        );

        RigisterButtonObjectEvent("BtnClose",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            });
    }

    public override void Display()
    {
        base.Display();
        /* string petName = ManageMentClass.DataManagerClass.petAdoptCostItem;
         m_TextDesc.text = string.Format("拥有一个{0}可以领养一只小狗，领养之后需每日照顾小狗。当小狗成长到一定阶段后您就有机会把它带回家啦。\n每个小狗都是一个不容丢弃的小生命。它们也有一定的寿命。若超过24小时未照顾，我们会将您所领养的小狗送往救助站。只有幼犬期以上的小狗才会保留，孵化中的将被强制救助。\n驯养小狗的过程中可以获得爱心，收集足够的爱心可以再商店中兑换物品或进行捐赠。", petName);
       */
        transform.SetAsLastSibling();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public void GetBoxData()
    {

    }
}
