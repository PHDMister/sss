using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ShopNewBuyTipsUIForm : BaseUIForm
{
    public Text m_TextCost;
    public Text m_TextName;
    public Text m_TextHaveNum;
    public Image m_ImgIcon;
    public InputField m_InputField;

    private ShopOutFitItem avatarData;
    private int buyNum = 1;
    private int m_CurCoin = 0;
    private int m_Price = 0;

    private MallServerData mallData;

    private int m_TableType = 0;

    private float clickTimer = 0f;
    private float clickInterval = 1f;
    private bool bClicked = false;
    private bool bCanClick = true;


    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        m_InputField.interactable = false;

        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnClose", p =>
             {
                 CloseUIForm();
             }
         );

        RigisterButtonObjectEvent("BtnAdd", p =>
             {
                 if (m_TableType == 4)
                 {
                     buyNum++;
                     int maxBuyNum = avatarData.avatar_quantity;
                     int canBuyNum = avatarData.items_limited == 1 ? Math.Min(avatarData.items_limited_quantity, maxBuyNum) : maxBuyNum;
                     if (buyNum > canBuyNum)
                         buyNum = canBuyNum;
                     m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                     m_InputField.text = buyNum.ToString();
                 }
                 else
                 {
                     buyNum++;
                     int canBuyNum = Math.Max(1, m_CurCoin / mallData.price);
                     int maxNum = Math.Min(buyNum, canBuyNum);
                     if (buyNum > maxNum)
                         buyNum = maxNum;
                     m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                     m_InputField.text = buyNum.ToString();
                 }
             }
        );

        RigisterButtonObjectEvent("BtnSub", p =>
             {
                 if (m_TableType == 4)
                 {
                     buyNum--;
                     if (buyNum < 1)
                         buyNum = 1;

                     m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                     m_InputField.text = buyNum.ToString();
                 }
                 else
                 {
                     buyNum--;
                     if (buyNum < 1)
                         buyNum = 1;
                     m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                     m_InputField.text = buyNum.ToString();
                 }
             }
        );

        RigisterButtonObjectEvent("BtnMax", p =>
             {
                 if (m_CurCoin < m_Price)
                 {
                     buyNum = 1;
                     m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                     m_InputField.text = buyNum.ToString();
                 }
                 else
                 {
                     if (m_TableType == 4)
                     {
                         int maxBuyNum = avatarData.avatar_quantity;
                         int canBuyNum = avatarData.items_limited == 1 ? Math.Min(avatarData.items_limited_quantity, maxBuyNum) : maxBuyNum;
                         buyNum = canBuyNum;
                         m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                         m_InputField.text = buyNum.ToString();
                     }
                     else
                     {

                         int maxBuyNum = m_CurCoin / m_Price;
                         buyNum = maxBuyNum;
                         m_TextCost.text = string.Format("{0}", m_Price * buyNum);
                         m_InputField.text = buyNum.ToString();
                     }
                 }
             }
        );

        RigisterButtonObjectEvent("BtnBuy", p =>
             {
                 bClicked = true;
                 clickTimer = 0;
                 if (!bCanClick)
                 {
                     return;
                 }

                 if (m_Price * buyNum > m_CurCoin)
                 {
                     OpenUIForm(FormConst.SHOPNEWGASTIPSUIFORM);
                     return;
                 }
                 if (m_TableType == 4)
                 {
                     BuyData data = new BuyData(avatarData.avatar_id, buyNum);
                     MessageManager.GetInstance().RequestOutFitBuy(data, () =>
                     {
                         CloseUIForm();
                         OpenUIForm(FormConst.SHOPDRESSTIPSUIFORM);
                         SendMessage("OpenShopDress", "Success", data);
                     });
                 }
                 else
                 {
                     BuyData data = new BuyData(mallData.item_id, buyNum);
                     MessageManager.GetInstance().RequestOutFitBuy(data, () =>
                     {
                         CloseUIForm();
                         int[] args = new int[] { data.item_id, data.number };
                         RoomFurnitureCtrl.Instance().BuyFurnAddCountFun(data.item_id, data.number);
                         SendMessage("ShopBuySuccess", "BuySucess", data);
                         ToastManager.Instance.ShowNewToast("购买成功", 2f);
                     });
                 }
             }
        );

        ReceiveMessage("OpenShopNewBuyTips", p =>
        {
            object[] args = p.Values as object[];
            m_TableType = (int)args[0];
            if (m_TableType == 4)//服装页签
            {
                avatarData = args[1] as ShopOutFitItem;
                SetOutFitInfo();
            }
            else//家具动作
            {
                mallData = args[1] as MallServerData;
                SetFurnitureInfo();
            }
        });
    }

    public void SetOutFitInfo()
    {
        buyNum = 1;
        if (avatarData == null)
            return;

        m_TextName.text = avatarData.avatar_name;
        m_TextHaveNum.text = string.Format("已拥有：{0}", avatarData.has_num);

        avatar avatarConfig = ManageMentClass.DataManagerClass.GetAvatarTableFun(avatarData.avatar_id);
        if (avatarConfig != null)
        {
           /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
            Sprite sprite = atlas.GetSprite(avatarConfig.avatar_icon);*/
            m_ImgIcon.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(avatarConfig.avatar_icon);
        }


        mall mallConfig = ManageMentClass.DataManagerClass.GetMallTableFun(avatarData.avatar_id);
        if (mallConfig != null)
        {
            m_TextCost.text = string.Format("{0}", mallConfig.price * buyNum);
            m_Price = mallConfig.price;
        }

        m_InputField.text = buyNum.ToString();
    }

    public void SetFurnitureInfo()
    {
        buyNum = 1;
        if (mallData == null)
            return;

        m_TextName.text = mallData.item_name;
        m_TextHaveNum.text = string.Format("已拥有：{0}", mallData.has_num);

        item itemCfg = ManageMentClass.DataManagerClass.GetItemTableFun(mallData.item_id);
        if (itemCfg != null)
        {
          /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
            Sprite sprite = atlas.GetSprite(itemCfg.item_icon);*/
            m_ImgIcon.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(itemCfg.item_icon) ;
        }


        m_TextCost.text = string.Format("{0}", mallData.price * buyNum);
        m_Price = mallData.price;

        m_InputField.text = buyNum.ToString();
    }

    public override void Display()
    {
        base.Display();
        bClicked = false;
        bCanClick = true;
        m_CurCoin = ManageMentClass.DataManagerClass.gas_Amount;
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    private void Update()
    {
        if (bClicked)
        {
            if (clickTimer < clickInterval)
            {
                clickTimer += Time.deltaTime;
                bCanClick = false;
            }

            if (clickTimer >= clickInterval)
            {
                clickTimer = 0;
                bCanClick = true;
                bClicked = false;
            }
        }
    }

}
