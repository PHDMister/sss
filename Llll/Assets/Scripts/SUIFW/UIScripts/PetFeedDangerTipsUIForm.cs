using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class PetFeedDangerTipsUIForm : BaseUIForm
{
    public Text m_TextDesc;
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
                Debug.Log("食醋胡");
                if (data != null)
                {
                    Debug.Log("食醋胡1");
                    pet_keeping config = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(data.keep_id);
                    if (config != null)
                    {
                        Debug.Log("食醋胡2");
                        int cost = config.price;
                        if (cost > ManageMentClass.DataManagerClass.gas_Amount)
                        {
                            Debug.Log("食醋胡3");
                            OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
                        }
                        else
                        {
                            Debug.Log("食醋胡4");
                            //治疗
                            PetFeedData petAdoptData = new PetFeedData(data.id, 2);
                            string Strdata = JsonConvert.SerializeObject(petAdoptData);
                            MessageManager.GetInstance().RequestFeed(Strdata, (JObject jo) =>
                            {
                                SendMessage("FeedSuccess", "Success", null);
                            });
                            CloseUIForm();
                        }
                        InterfaceHelper.SetJoyStickState(true);
                    }
                    else
                    {
                        ToastManager.Instance.ShowNewToast("数据错误，请退出重试", 2f);
                    }
                }
                else
                {
                    ToastManager.Instance.ShowNewToast("数据错误，请退出重试", 2f);
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
        ReceiveMessage("DangerOpt",
            p =>
             {
                 data = p.Values as PetListRecData;
                 if (data != null)
                 {
                     pet_keeping config = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(data.keep_id);
                     if (config != null)
                     {
                         m_TextDesc.text = string.Format("{0} 购买药物和羊奶", config.price);
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
