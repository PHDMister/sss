using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PetShopPanel : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_PetShopScroll;

    public Image ShowBigImage;



    //������ʾ���
    public GameObject buyAffitmPanel;
    public Text tipsLoveValue_Text;
    public Text goodsName_Text;



    //����ɹ���ʾ
    public GameObject successTipsPanel;
    public Text succGoodsName_Text;
    public Image succGoodsIcon_Img;



    private int buyID = -1;

    private void Awake()
    {
        //��������
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //��������
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        m_PetShopScroll.Init(NormalCallBack);
        InitUIFun();
        MessageManagerFun();
    }
    /// <summary>
    /// ��
    /// </summary>
    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        if (ManageMentClass.DataManagerClass.ListPetShopItemId.Count > 0)
        {
            if (!m_PetShopScroll.transform.gameObject.activeSelf)
            {
                m_PetShopScroll.transform.gameObject.SetActive(true);
            }
            m_PetShopScroll.ShowList(ManageMentClass.DataManagerClass.ListPetShopItemId.Count);
            int key = ManageMentClass.DataManagerClass.ListPetShopItemId[0];
            SetBigShowImageFun(key);
        }
        else
        {
            m_PetShopScroll.transform.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// �ر�
    /// </summary>
    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }
    void InitUIFun()
    {
        buyAffitmPanel.SetActive(false);
        successTipsPanel.SetActive(false);

        //������ʾ
        RigisterButtonObjectEvent("TipAffirmBtn", p =>
        {
            //ȷ��
            AffirmBuyGoodsFun();
        });

        RigisterButtonObjectEvent("TipCancelBtn", p =>
        {
            //�ر� 
            if (buyAffitmPanel.activeSelf)
            {
                buyAffitmPanel.SetActive(false);
            }
        });

        //����ɹ���ʾ
        RigisterButtonObjectEvent("CloseSuccessTipsPanel_Btn", p =>
        {
            //�ر� 
            if (successTipsPanel.activeSelf)
            {
                successTipsPanel.SetActive(false);
            }
        });
        //�ر��̳�
        RigisterButtonObjectEvent("ClosePetShopPanel_Btn", p =>
        {
            InterfaceHelper.SetJoyStickState(true);
            CloseUIForm();
        });


        //�ҵ�ȯ��
        RigisterButtonObjectEvent("MyInventoryButton", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("��ǰ��App�˲鿴", 5f);
            }
            else
            {
                // string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}&order_id={2}", 0, 0, 0);
                string url = "aiera://www.aiera.com/page/new/score/coupon";
                Debug.Log("�鿴ȯ�� uRl: " + url);
                //������˳�
                try
                {
                    SetTools.SetPortraitModeFun();
                    //��ʾtop��
                    SetTools.CloseGameFun();
                    SetTools.OpenWebUrl(url);
                }
                catch (System.Exception e)
                {
                    Debug.Log("��������ݣ� " + e);
                }
            }

        });


        //�鿴ȯ��
        RigisterButtonObjectEvent("CheckInventoryButton", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("��ǰ��App�˲鿴", 5f);
            }
            else
            {
                // string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}&order_id={2}", 0, 0, 0);
                string url = "aiera://www.aiera.com/page/new/score/coupon";
                Debug.Log("�鿴ȯ�� uRl: " + url);
                //������˳�
                try
                {
                    SetTools.SetPortraitModeFun();
                    //��ʾtop��
                    SetTools.CloseGameFun();
                    SetTools.OpenWebUrl(url);
                }
                catch (System.Exception e)
                {
                    Debug.Log("��������ݣ� " + e);
                }
            }

        });
        //CheckInventoryButton
    }
    /// <summary>
    /// message����
    /// </summary>
    public void MessageManagerFun()
    {
        ReceiveMessage("PetShopItemBuyBtn",
                 p =>
                 {
                     buyID = (int)p.Values;
                     if (this.gameObject.activeSelf)
                     {
                         if (ManageMentClass.DataManagerClass.loveCoin >= ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].price)
                         {
                             if (!buyAffitmPanel.activeSelf)
                             {
                                 buyAffitmPanel.SetActive(true);
                                 SetBuyAffitmPanelFun();
                             }
                         }
                         else
                         {
                             ToastManager.Instance.ShowPetToast("���ıҲ���", 5f);
                             //  ToastManager.
                         }
                     }
                 }
            );


        ReceiveMessage("PetShopItemClick", p =>
        {
            int key = (int)p.Values;
            SetBigShowImageFun(key);

        });
    }
    private void NormalCallBack(GameObject cell, int index)
    {
        if (cell != null)
        {
            Debug.Log("cell���ڣ� " + index);
        }
        else
        {
            Debug.Log("�����ڣ� ");
        }
        cell.GetComponent<PetShopItem>().SetItemFun(index - 1);
    }

    private void SetBigShowImageFun(int key)
    {
        item m_ItemConfig = ManageMentClass.DataManagerClass.GetItemTableFun(ManageMentClass.DataManagerClass.DicPetShopItemData[key].item_id);
        Sprite texture = InterfaceHelper.GetPetShopIconFun(m_ItemConfig.item_icon);
        if (texture != null)
        {
            ShowBigImage.sprite = texture;
        }
        else
        {
            Debug.LogError("��ͼΪ��");
        }
    }

    /// <summary>
    /// ȷ�Ϲ���
    /// </summary>
    private void AffirmBuyGoodsFun()
    {
        PetShopSendData petShopTypeData = new PetShopSendData(buyID);
        Debug.Log("���һ�¹����ID����ţ� " + buyID);
        string Strdata = JsonConvert.SerializeObject(petShopTypeData);
        MessageManager.GetInstance().RequestPetShopBuyReceive(Strdata, (JObject jo) =>
        {
            buyAffitmPanel.SetActive(false);
            // ����ɹ�
            ManageMentClass.DataManagerClass.loveCoin = (int)jo["data"]["remain_lovecoin"];
            SendMessage("UpdateLoveCoin", "Value", null);
            //ˢ���б�����
            BuySuccessedRefreshScrollViewFun();
            //�򿪳ɹ�����Ľ���
            SetSuccessTipsPanelFun();
        });
    }
    /// <summary>
    /// ���ù���ȷ�������Ϣ
    /// </summary>
    private void SetBuyAffitmPanelFun()
    {
        tipsLoveValue_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].price + "";
        goodsName_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].item_name;
    }
    /// <summary>
    /// ������ʾ�ɹ��������Ϣ
    /// </summary>
    private void SetSuccessTipsPanelFun()
    {
        successTipsPanel.SetActive(true);
        succGoodsName_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].item_name + "�һ�ȯ";
        item m_ItemConfig = ManageMentClass.DataManagerClass.GetItemTableFun(ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].item_id);
        if (m_ItemConfig != null)
        {
            Sprite texture = InterfaceHelper.GetPetShopIconFun(m_ItemConfig.item_icon);
            if (texture != null)
            {
                succGoodsIcon_Img.sprite = texture;
            }
            else
            {
                Debug.Log("û���ҵ���Ϊ��");
            }
        }
    }
    /// <summary>
    /// ����ɹ���ˢ������
    /// </summary>
    private void BuySuccessedRefreshScrollViewFun()
    {
        if (ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].sale_quota > 0)
        {
            //�ѹ�����+1
            ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].buy_count++;
        }
        //ʣ����-1��
        ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].inventory--;
        m_PetShopScroll.UpdateList();
        int key = ManageMentClass.DataManagerClass.ListPetShopItemId[0];
        SetBigShowImageFun(key);
    }
}
