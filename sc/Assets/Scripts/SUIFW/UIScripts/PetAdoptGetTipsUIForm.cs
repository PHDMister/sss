using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class PetAdoptGetTipsUIForm : BaseUIForm
{
    public Text m_TextDesc;
    void Awake()
    {
        //���������
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* ��ť��ע��  */
        RigisterButtonObjectEvent("BtnGet",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
                OnClickGet();
            }
        );
        RigisterButtonObjectEvent("BtnCancel",
            p =>
            {
                InterfaceHelper.SetJoyStickState(true);
                CloseUIForm();
            }
        );
    }
    public override void Display()
    {
        base.Display();
        m_TextDesc.text = string.Format("δӵ��{0}�޷�����", ManageMentClass.DataManagerClass.petAdoptCostItem);
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }
    public void OnClickGet()
    {
        string uRl = "";
        if(ManageMentClass.DataManagerClass.isLinkEdition && ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019499, 1484);
        }
        else
        {
            uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", 1019189, 1081);
        }

        Debug.Log("uRl: " + uRl);
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
            Debug.Log("��������ݣ� " + e);
        }
    }
}
