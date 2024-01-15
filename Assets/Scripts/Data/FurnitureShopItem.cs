using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class FurnitureShopItem : BaseUIForm
{
    public Button m_BtnIcon;
    public Button m_BtnBuy;
    public Image m_BgImg;
    public Image m_IconImg;
    public Text m_NameText;
    public Text m_PriceText;
    public Image m_SelectImg;
    public Button m_IconBtn;
    public Button m_BuyBtn;

    private item m_ItemData;
    private MallServerData m_MallServerData;

    private int m_TabType;
    private int m_ItemId = 0;

    public enum TableType
    {
        None = 0,
        Furniture = 1,
        Action = 2,
        HotMan = 3,
        RecentUse = 4,
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_BtnIcon.onClick.AddListener(() =>
        {
            ShopNewUIForm uiForm = UIManager.GetInstance().GetUIForm(FormConst.SHOPNEWUIFORM) as ShopNewUIForm;
            if (uiForm != null)
            {
                if (uiForm.m_FurnitureActionType == ShopNewUIForm.FurnitureActionType.Furniture)
                {
                    uiForm.SetFurntureItemSelectState(m_ItemData.item_id);
                    SetModelRT();
                }
                else
                {
                    uiForm.SetActionItemSelectState(m_ItemData.item_id);
                }
            }
        });

        m_BtnBuy.onClick.AddListener(() =>
        {
            ShopNewUIForm uiForm = UIManager.GetInstance().GetUIForm(FormConst.SHOPNEWUIFORM) as ShopNewUIForm;
            if (uiForm != null)
            {
                if (uiForm.m_FurnitureActionType == ShopNewUIForm.FurnitureActionType.Furniture)
                {
                    uiForm.SetFurntureItemSelectState(m_ItemData.item_id);
                    SetModelRT();
                }
                else
                {
                    uiForm.SetActionItemSelectState(m_ItemData.item_id);
                }
            }

            OpenUIForm(FormConst.SHOPNEWBUYTIPSUIFORM);
            object[] parm = new object[] { m_TabType, m_MallServerData };
            SendMessage("OpenShopNewBuyTips", "Success", parm);
        });
    }

    public void SetItemData(item data)
    {
        m_ItemData = data;
    }

    public void SetMallServerData(MallServerData data)
    {
        m_MallServerData = data;
    }

    public void SetItemId(int id)
    {
        m_ItemId = id;
    }

    public int GetItemId()
    {
        return m_ItemId;
    }

    public void SetItemIcon(string spriteName)
    {
        /*SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
        Sprite sprite = atlas.GetSprite(spriteName);*/
        m_IconImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(spriteName);
    }

    public void SetItemName(string name)
    {
        m_NameText.text = name;
    }

    public void SetItemPrice(int price)
    {
        m_PriceText.text = price.ToString();
    }

    public void SetItemTabType(int tableType)
    {
        m_TabType = tableType;
    }

    public void SetItemSelectState(bool bSelect)
    {
        if (m_SelectImg == null)
            return;
        m_SelectImg.gameObject.SetActive(bSelect);
        if (bSelect)
        {
            SetModelRT();
        }
    }

    public void SetModelRT()
    {
        if (m_ItemData == null)
            return;
        RTManager.GetInstance().LoadRTModel(m_ItemData.item_id);
    }
}
