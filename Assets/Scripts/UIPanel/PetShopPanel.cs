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



    //购买提示面板
    public GameObject buyAffitmPanel;
    public Text tipsLoveValue_Text;
    public Text goodsName_Text;



    //购买成功提示
    public GameObject successTipsPanel;
    public Text succGoodsName_Text;
    public Image succGoodsIcon_Img;



    private int buyID = -1;

    private void Awake()
    {
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        m_PetShopScroll.Init(NormalCallBack);
        InitUIFun();
        MessageManagerFun();
    }
    /// <summary>
    /// 打开
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
    /// 关闭
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

        //购买提示
        RigisterButtonObjectEvent("TipAffirmBtn", p =>
        {
            //确认
            AffirmBuyGoodsFun();
        });

        RigisterButtonObjectEvent("TipCancelBtn", p =>
        {
            //关闭 
            if (buyAffitmPanel.activeSelf)
            {
                buyAffitmPanel.SetActive(false);
            }
        });

        //购买成功提示
        RigisterButtonObjectEvent("CloseSuccessTipsPanel_Btn", p =>
        {
            //关闭 
            if (successTipsPanel.activeSelf)
            {
                successTipsPanel.SetActive(false);
            }
        });
        //关闭商场
        RigisterButtonObjectEvent("ClosePetShopPanel_Btn", p =>
        {
            InterfaceHelper.SetJoyStickState(true);
            CloseUIForm();
        });


        //我的券库
        RigisterButtonObjectEvent("MyInventoryButton", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("请前往App端查看", 5f);
            }
            else
            {
                // string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}&order_id={2}", 0, 0, 0);
                string url = "aiera://www.aiera.com/page/new/score/coupon";
                Debug.Log("查看券库 uRl: " + url);
                //点击了退出
                try
                {
                    SetTools.SetPortraitModeFun();
                    //显示top栏
                    SetTools.CloseGameFun();
                    SetTools.OpenWebUrl(url);
                }
                catch (System.Exception e)
                {
                    Debug.Log("这里的内容： " + e);
                }
            }

        });


        //查看券库
        RigisterButtonObjectEvent("CheckInventoryButton", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("请前往App端查看", 5f);
            }
            else
            {
                // string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}&order_id={2}", 0, 0, 0);
                string url = "aiera://www.aiera.com/page/new/score/coupon";
                Debug.Log("查看券库 uRl: " + url);
                //点击了退出
                try
                {
                    SetTools.SetPortraitModeFun();
                    //显示top栏
                    SetTools.CloseGameFun();
                    SetTools.OpenWebUrl(url);
                }
                catch (System.Exception e)
                {
                    Debug.Log("这里的内容： " + e);
                }
            }

        });
        //CheckInventoryButton
    }
    /// <summary>
    /// message方法
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
                             ToastManager.Instance.ShowPetToast("爱心币不足", 5f);
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
            Debug.Log("cell存在： " + index);
        }
        else
        {
            Debug.Log("不存在： ");
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
            Debug.LogError("此图为空");
        }
    }

    /// <summary>
    /// 确认购买
    /// </summary>
    private void AffirmBuyGoodsFun()
    {
        PetShopSendData petShopTypeData = new PetShopSendData(buyID);
        Debug.Log("输出一下购买的ID的序号： " + buyID);
        string Strdata = JsonConvert.SerializeObject(petShopTypeData);
        MessageManager.GetInstance().RequestPetShopBuyReceive(Strdata, (JObject jo) =>
        {
            buyAffitmPanel.SetActive(false);
            // 购买成功
            ManageMentClass.DataManagerClass.loveCoin = (int)jo["data"]["remain_lovecoin"];
            SendMessage("UpdateLoveCoin", "Value", null);
            //刷新列表数据
            BuySuccessedRefreshScrollViewFun();
            //打开成功购买的界面
            SetSuccessTipsPanelFun();
        });
    }
    /// <summary>
    /// 设置购买确认面板信息
    /// </summary>
    private void SetBuyAffitmPanelFun()
    {
        tipsLoveValue_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].price + "";
        goodsName_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].item_name;
    }
    /// <summary>
    /// 设置提示成功的面板信息
    /// </summary>
    private void SetSuccessTipsPanelFun()
    {
        successTipsPanel.SetActive(true);
        succGoodsName_Text.text = ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].item_name + "兑换券";
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
                Debug.Log("没有找到，为空");
            }
        }
    }
    /// <summary>
    /// 购买成功后刷新数据
    /// </summary>
    private void BuySuccessedRefreshScrollViewFun()
    {
        if (ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].sale_quota > 0)
        {
            //已购数量+1
            ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].buy_count++;
        }
        //剩余库存-1；
        ManageMentClass.DataManagerClass.DicPetShopItemData[buyID].inventory--;
        m_PetShopScroll.UpdateList();
        int key = ManageMentClass.DataManagerClass.ListPetShopItemId[0];
        SetBigShowImageFun(key);
    }
}
