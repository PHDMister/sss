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
    ///  ȥ����
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
        //������˳�
        try
        {
            SetTools.SetPortraitModeFun();
            //��ʾtop��
            SetTools.CloseGameFun();
            SetTools.OpenWebUrl(uRl);

        }
        catch (System.Exception e)
        {
            Debug.Log("��ת�쳣�� " + e);
        }
    }

    public override void Display()
    {
        base.Display();
        treasure_ticket ticketCfg = ManageMentClass.DataManagerClass.GetTreasureTicketTable(1);
        if (ticketCfg != null)
        {
            m_DescText.text = string.Format("��ǰ��{0}����������\n�Ƿ�ȥ����?", ticketCfg.collection_name);
        }
    }

}
