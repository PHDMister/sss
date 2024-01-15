using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class TreasureExchangePanel : BaseUIForm
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
          ExchangeFun();
      });
    }
    private void ExchangeFun()
    {
        //MessageManager.GetInstance().RequestExchangeTickets(() =>
        //{
        //    ExchangeSuccessFun();
        //},
        //() =>
        //{
        //    ExchangeFaileFun();
        //});
    }
    /// <summary>
    /// ¶Ò»»³É¹¦
    /// </summary>
    private void ExchangeSuccessFun()
    {
        CloseUIForm();
        OpenUIForm(FormConst.TREASUREEXCHANGESCCESIPS);
        SendMessage("TreasureExchangeTicket", "Success", null);
    }
    /// <summary>
    ///  ¶Ò»»Ê§°Ü
    ///  ²ØÆ·²»¹»Ê±
    /// </summary>
    private void ExchangeFaileFun()
    {
        CloseUIForm();
        OpenUIForm(FormConst.TREASUREEXCHANGENOENOUGHTIPPANEL);
    }

    public override void Display()
    {
        base.Display();
        treasure_ticket ticketCfg = ManageMentClass.DataManagerClass.GetTreasureTicketTable(1);
        if (ticketCfg != null)
        {
            m_DescText.text = string.Format("ÊÇ·ñÏûºÄ¡¾{0}¡¿*1\n¶Ò»»{1} * 1", ticketCfg.collection_name, ticketCfg.ticket_name);
        }
    }
}
