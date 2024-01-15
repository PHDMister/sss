using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;

public class ItemData : BaseUIForm
{
    public Image m_IconImg;
    public Text m_NumText;
    public Text m_HaveNumText;
    public Text m_NameText;
    public Text m_PriceText;
    public Image m_SelectImg;
    public Button m_IconBtn;
    public Button m_BuyBtn;
    public GameObject m_LockObj;
    public GameObject m_UnlockObj;
    public Image m_UsingImg;

    private item m_ItemData;
    private mall m_MallData;
    private MallServerData m_MallServerData;
    private CharacterData m_CharacterServerData;

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
        if (m_UnlockObj != null)
            m_UnlockObj.gameObject.SetActive(false);
        if (m_LockObj != null)
            m_LockObj.gameObject.SetActive(false);
        if (m_IconBtn != null)
            m_IconBtn.onClick.AddListener(OnClickItem);
        if (m_BuyBtn != null)
            m_BuyBtn.onClick.AddListener(OnClickBuy);
        if (m_UnlockObj != null)
            m_UnlockObj.GetComponent<Button>().onClick.AddListener(OnClickUnLock);
        SetItemSelectState(false);
    }

    public void OnClickItem()
    {
        if (m_TabType == (int)TableType.Furniture)
        {
            OpenUIForm(FormConst.SHOPITEMPRVIEW_UIFORM);
            SendMessage("ShopFurntureItem", "ItemClick", m_ItemId);
        }
        else if (m_TabType == (int)TableType.Action)
        {
            OpenUIForm(FormConst.SHOPITEMPRVIEW_UIFORM);
            SendMessage("ShopActionItem", "ItemClick", m_ItemId);
        }
        else if (m_TabType == (int)TableType.HotMan)
        {
            object[] args = new object[] { m_CharacterServerData, m_ItemData };
            SendMessage("SetCharacterItem", "ItemClick", args);
        }
        else if (m_TabType == (int)TableType.RecentUse)
        {
            SendMessage("SetRecentUseItem", "ItemClick", m_ItemData);
        }
        else
        {
            SendMessage("SetActionItem", "ItemClick", m_ItemData);
        }
    }

    public void OnClickBuy()
    {
        OpenUIForm(FormConst.SHOPITEMPRVIEW_UIFORM);
        SendMessage("ShopFurntureItem", "ItemClick", m_ItemId);

        if (m_TabType == (int)TableType.Furniture)
        {
            //UIManager.GetInstance().ShowUIForms(FormConst.SHOPFURNTUREBUY_UIFORM);
            object[] args = new object[] { m_MallServerData, m_ItemData, 1 };
            SendMessage("ShopFurntureBuy", "FurntureBuy", args);
        }
        else if (m_TabType == (int)TableType.Action)
        {
            //UIManager.GetInstance().ShowUIForms(FormConst.SHOPACTIONBUY_UIFORM);
            string[] args = new string[] { m_ItemData.item_id.ToString(), m_ItemData.item_type1.ToString(), m_ItemData.item_name, (m_MallServerData.price).ToString() };
            SendMessage("ShopActionBuy", "ActionBuy", args);
        }
    }

    public void OnClickUnLock()
    {
        ToastManager.Instance.ShowNewToast(string.Format("动作<color=#00ff00>{0}</color>已解锁！", m_ItemData.item_name), 5f);
    }

    public void SetItemData(item data)
    {
        m_ItemData = data;
    }
    public void SetMallData(mall data)
    {
        m_MallData = data;
    }

    public void SetMallServerData(MallServerData data)
    {
        m_MallServerData = data;
    }
    public void SetCharacterServerData(CharacterData data)
    {
        m_CharacterServerData = data;
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
       /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
        Sprite sprite = atlas.GetSprite(spriteName);*/
        m_IconImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(spriteName);
    }
    public void SetItemNum(int num)
    {
        m_NumText.text = string.Format("x{0}", num);
    }
    public void SetItemHaveNum(int num)
    {
        if (m_HaveNumText != null)
            m_HaveNumText.text = string.Format("当前拥有：{0}", num);
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
    }

    public void SetItemUsingState(bool bUsing)
    {
        if (m_UsingImg == null)
            return;
        m_UsingImg.gameObject.SetActive(bUsing);
    }

    public void SetItemLockState(bool bUnlock)
    {
        if (m_LockObj != null)
            m_LockObj.gameObject.SetActive(bUnlock);
        if (m_IconImg != null)
        {
            if(bUnlock)
            {
                Material grayMate = new Material(Shader.Find("UI/Gray"));
                m_IconImg.GetComponent<Image>().material = grayMate;
            }
            else
            {
                Material defaultMate = new Material(Shader.Find("UI/Default"));
                m_IconImg.GetComponent<Image>().material = defaultMate;
            }
        }
    }

    public void SetItemUnLockState(bool bUnlock)
    {
        if (m_UnlockObj != null)
            m_UnlockObj.gameObject.SetActive(bUnlock);
    }
}
