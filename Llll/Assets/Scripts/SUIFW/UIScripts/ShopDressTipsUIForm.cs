using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class ShopDressTipsUIForm : BaseUIForm
{

    public Text m_TextDesc;
    public BuyData data;
    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnConfirm", p =>
             {
                 CloseUIForm();

                 if (ManageMentClass.DataManagerClass.SceneID == (int) LoadSceneType.parlorScene)
                 {
                     //如果当前是在客厅 换装则取消建筑交互
                     Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative();
                 }
                 
                 AvatarManager.Instance().AddOfficalAvatarDataFun(data.item_id);
                 MyOutFitSaveReqData myOutFitSaveReqData = AvatarManager.Instance().myOutFitAvatarIdData;
                 MessageManager.GetInstance().RequestMyOutFitSave(myOutFitSaveReqData, () =>
                  {
                      ChangeSkinReq req = new ChangeSkinReq();
                      req.UserId = ManageMentClass.DataManagerClass.userId;
                      foreach (var item in AvatarManager.Instance().myOutFitAvatarIdData.data)
                      {
                          req.AvatarIds.Add(item.avatar_id);
                      }
                      req.Index = WebSocketAgent.Ins.NetView.GetCode;
                      WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ChangeSkinReq, req, (code, data) =>
                      {
                          Debug.Log("Code的值： " + code);
                      });
                  });
             }
        );

        RigisterButtonObjectEvent("BtnCancel",
            p =>
            {
                CloseUIForm();
            }
        );

        ReceiveMessage("OpenShopDress", p =>
         {
             data = p.Values as BuyData;
             if (data == null)
                 return;
             SetInfo(data);
         });
    }

    public void SetInfo(BuyData data)
    {
        int itemId = data.item_id;
        avatar avatarCfg = ManageMentClass.DataManagerClass.GetAvatarTableFun(itemId);
        if (avatarCfg != null)
        {
            m_TextDesc.text = string.Format("恭喜你成功购买【{0}】X {1}\n是否立即穿戴？", avatarCfg.avatar_name, data.number);
        }
    }
    public override void Display()
    {
        base.Display();
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    private void OnClickConfirm()
    {

    }



    private void OnClickCancel()
    {
        CloseUIForm();
    }

}
