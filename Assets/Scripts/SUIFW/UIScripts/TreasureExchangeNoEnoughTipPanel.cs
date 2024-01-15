using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class TreasureExchangeNoEnoughTipPanel : BaseUIForm
{
    public Text m_DescText;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        RigisterButtonObjectEvent("Cancel_Button",
        p =>
          {
              CloseUIForm();

          });
        RigisterButtonObjectEvent("Affirm_Button",
      p =>
      {
          CloseUIForm();
          ToBuyFun();
      });

    }
    /// <summary>
    ///  去购买
    /// </summary>
    private void ToBuyFun()
    {
        string uRl = "";
        if (ManageMentClass.DataManagerClass.isLinkEdition && ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019769, 1715);
        }
        else
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019769, 1715);
        }
        Debug.Log("uRl: " + uRl);
        if (string.IsNullOrEmpty(uRl))
            return;
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
            Debug.Log("跳转异常： " + e);
        }
    }

    public override void Display()
    {
        base.Display();
        treasure_ticket ticketCfg = ManageMentClass.DataManagerClass.GetTreasureTicketTable(1);
        if (ticketCfg != null)
        {
            m_DescText.text = string.Format("当前【{0}】数量不足\n是否去购买?", ticketCfg.collection_name);
        }
    }

}
