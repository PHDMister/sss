using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


public class ShopNewUIForm : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_FurnitureActionVerticalScroll;
    public CircularScrollView.UICircularScrollView m_SuitVerticalScroll;
    public CircularScrollView.UICircularScrollView m_AppearanceVerticalScroll;

    public RectTransform m_ScrollViewBg;
    public Transform m_PlayerRawImg;
    public Transform m_ModelRawImg;

    public Toggle m_Toggle_Furniture;
    public Toggle m_Toggle_Image;
    public Toggle m_Toggle_Action;
    public Text m_ToggleFurnitureTxt;
    public Text m_ToggleImageTxt;
    public Text m_ToggleActionTxt;

    public Toggle m_Toggle_Head;
    public Toggle m_Toggle_Appearance;
    public Toggle m_Toggle_Suit;
    public Text m_ToggleHeadTxt;
    public Text m_ToggleAppearanceTxt;
    public Text m_ToggleSuitTxt;
    public Text m_NoneText;
    public Transform m_Trans_HeadSecond;
    public Transform m_Trans_AppearanceSecond;
    public Transform m_Trans_ImageToggleGroup;
    //头部
    public Toggle m_Toggle_Hair;
    public Toggle m_Toggle_Glasses;
    public Toggle m_Toggle_Necklace;
    public Toggle m_Toggle_Earring;
    public Toggle m_Toggle_Eye;
    public Toggle m_Toggle_Eyebrow;
    public Toggle m_Toggle_Nose;
    public Toggle m_Toggle_Mouth;
    public Toggle m_Toggle_Ear;

    //外观
    public Toggle m_Toggle_Coat;
    public Toggle m_Toggle_Underwear;
    public Toggle m_Toggle_Shoe;
    public Toggle m_Toggle_Bag;
    public Toggle m_Toggle_Watch;
    public Toggle m_Toggle_Rings;

    //服装数据
    List<ShopOutFitRecData> shopOutFitRecDataList = null;
    List<ShopOutFitItem> shopOutFitItemList = new List<ShopOutFitItem>();
    Dictionary<int, ShopClothItem> dicClothItems = new Dictionary<int, ShopClothItem>();

    //家具动作数据
    private Dictionary<int, MallServerData> m_MallServerData = new Dictionary<int, MallServerData>();
    private Dictionary<int, item> m_FurntureItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, item> m_ActionItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, FurnitureShopItem> m_FurntureItemData = new Dictionary<int, FurnitureShopItem>();
    private Dictionary<int, FurnitureShopItem> m_ActionItemData = new Dictionary<int, FurnitureShopItem>();
    private int m_SelectItemId = 0;


    //下拉列表
    public Dropdown m_DropdownHair;
    public Dropdown m_DropdownAppearance;
    public Dropdown m_DropdownSuit;
    public Dropdown m_DropdownFurniture;
    public Dropdown m_DropdownAction;
    List<string> clothingOptionStr = new List<string>();
    List<string> furnAndActOptionStr = new List<string>();

    public enum SortyType
    {
        Default = 0,
        PriceAsc = 1,
        PriceDesc = 2,
        QualityAsc = 3,
        QualityDesc = 4,
    }

    public enum FurnitureActionType
    {
        Furniture = 1,
        Action = 2,
    }
    public enum PageType
    {
        Furniture = 1,
        Appearance = 2,
        Action = 3,
    }

    public enum TableType
    {
        Suit = 1,
        Head = 2,
        Appearance = 3,
    }

    public enum HeadTableType
    {
        Hair = 3,//头发
        Glasses = 4,//眼镜
        Earring = 5,//耳饰
        Necklace = 6,//项链 
        Eye = 20,//眼睛
        Eyebrow = 11,//眉毛
        Nose = 12,//鼻子
        Mouth = 13,//嘴巴
        Ear = 14,//耳朵
    }

    public enum AppearanceTableType
    {
        HotMan = 1,//HotMan
        Suit = 2,//套装
        Coat = 7,//外套
        Underwear = 8,//下衣
        Shoe = 9,//鞋子
        Watch = 10,//手表
        Rings = 12,//戒子
        Bag = 20,//背包
    }

    private PageType m_PageType;
    private TableType m_TabType;
    private HeadTableType m_HeadTabType;
    private AppearanceTableType m_AppearanceTableType;
    public FurnitureActionType m_FurnitureActionType = FurnitureActionType.Furniture;

    void Awake()
    {
        MessageManager.GetInstance().RequestShopOutFitList((p) =>
        {
            SendMessage("ReceiveShopOutFitData", "Success", p);
        });

        if (m_NoneText != null)
            m_NoneText.gameObject.SetActive(false);
        InitConfig();
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        ToggleAddListener();
        DropdownAddListener();

        //注册按钮事件
        RigisterButtonObjectEvent("BtnClose", p =>
         {
             CloseUIForm();
         });

        ReceiveMessage("ReceiveShopOutFitData", p =>
        {
            shopOutFitRecDataList = p.Values as List<ShopOutFitRecData>;
            if (shopOutFitRecDataList == null)
                return;
            ShowOutFitList();
        });

        ReceiveMessage("OpenShopDress", p =>
        {
            //服装购买成功
            MessageManager.GetInstance().RequestShopOutFitList((p) =>
            {
                SendMessage("ReceiveShopOutFitData", "Success", p);
            });
        });

        ReceiveMessage("ShopBuySuccess",
          p =>
          {
              if (gameObject.activeSelf == false)
              {
                  return;
              }
              BuyData args = p.Values as BuyData;
              int m_BuyItemId = args.item_id;
              int m_BuyNum = args.number;
              MessageManager.GetInstance().RequestShopData((int)m_FurnitureActionType, () =>
              {
                  m_MallServerData = MessageManager.GetInstance().GetMallData();
                  if (m_FurnitureActionType == FurnitureActionType.Furniture)
                  {
                      ShowFurniture();
                  }
                  else
                  {
                      ShowAction();
                  }
              });
          }
      );

    }

    private void ToggleAddListener()
    {
        m_Toggle_Furniture.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Furniture;
                m_FurnitureActionType = FurnitureActionType.Furniture;
                SetPageToggleTextColor(m_PageType);

                m_Trans_ImageToggleGroup.gameObject.SetActive(false);
                m_Trans_AppearanceSecond.gameObject.SetActive(false);
                m_Trans_HeadSecond.gameObject.SetActive(false);

                m_FurnitureActionVerticalScroll.gameObject.SetActive(true);
                m_AppearanceVerticalScroll.gameObject.SetActive(false);
                m_SuitVerticalScroll.gameObject.SetActive(false);
                m_NoneText.gameObject.SetActive(false);

                m_PlayerRawImg.gameObject.SetActive(false);
                m_ModelRawImg.gameObject.SetActive(true);
                RTManager.GetInstance().SetCharacterVisable(false);

                SetDropdownVisible();
                ShowFurniture();
            }
        });

        m_Toggle_Image.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Appearance;
                SetPageToggleTextColor(m_PageType);

                m_TabType = TableType.Head;
                SetToggleGroup();
                SetHeadToggleGroup();
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);

                m_Trans_ImageToggleGroup.gameObject.SetActive(true);
                m_Trans_AppearanceSecond.gameObject.SetActive(false);
                m_Trans_HeadSecond.gameObject.SetActive(true);

                m_FurnitureActionVerticalScroll.gameObject.SetActive(false);
                m_AppearanceVerticalScroll.gameObject.SetActive(true);
                m_SuitVerticalScroll.gameObject.SetActive(false);
                m_NoneText.gameObject.SetActive(false);

                m_PlayerRawImg.gameObject.SetActive(true);
                m_ModelRawImg.gameObject.SetActive(false);

                RTManager.GetInstance().LoadCharacter();
                RTManager.GetInstance().SetCharacterVisable(true);

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });


        m_Toggle_Action.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Action;
                m_FurnitureActionType = FurnitureActionType.Action;
                SetPageToggleTextColor(m_PageType);

                m_Trans_ImageToggleGroup.gameObject.SetActive(false);
                m_Trans_AppearanceSecond.gameObject.SetActive(false);
                m_Trans_HeadSecond.gameObject.SetActive(false);

                m_FurnitureActionVerticalScroll.gameObject.SetActive(true);
                m_AppearanceVerticalScroll.gameObject.SetActive(false);
                m_SuitVerticalScroll.gameObject.SetActive(false);

                m_PlayerRawImg.gameObject.SetActive(true);
                m_ModelRawImg.gameObject.SetActive(false);

                AvatarManager.Instance().RefreshPlayerFun();
                RTManager.GetInstance().LoadCharacter();
                RTManager.GetInstance().SetCharacterVisable(true);

                SetDropdownVisible();
                ShowAction();
            }
        });


        m_Toggle_Head.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Hair;
                SetHeadToggleGroup();

                m_TabType = TableType.Head;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);

                m_AppearanceVerticalScroll.gameObject.SetActive(true);
                m_SuitVerticalScroll.gameObject.SetActive(false);

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Appearance.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Coat;
                SetAppearanceToggleGroup();

                m_TabType = TableType.Appearance;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);

                m_AppearanceVerticalScroll.gameObject.SetActive(true);
                m_SuitVerticalScroll.gameObject.SetActive(false);

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Suit.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Suit;
                m_TabType = TableType.Suit;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);

                m_AppearanceVerticalScroll.gameObject.SetActive(false);
                m_SuitVerticalScroll.gameObject.SetActive(true);

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        //头部
        m_Toggle_Hair.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Hair;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Glasses.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Glasses;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Earring.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Earring;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Necklace.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Necklace;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Eye.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Eye;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Eyebrow.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Eyebrow;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Nose.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Nose;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Mouth.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Mouth;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Ear.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Ear;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        //外观
        m_Toggle_Coat.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Coat;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Underwear.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Underwear;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Shoe.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Shoe;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Bag.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Bag;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Watch.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Watch;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });

        m_Toggle_Rings.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Rings;

                if (shopOutFitRecDataList == null)
                    return;
                SetDropdownVisible();
                ShowOutFitList();
            }
        });
    }

    private void DropdownAddListener()
    {
        m_DropdownFurniture.onValueChanged.AddListener(delegate { FurnitureDropdownValueChanged(m_DropdownFurniture); });
        m_DropdownAction.onValueChanged.AddListener(delegate { ActionDropdownValueChanged(m_DropdownAction); });

        m_DropdownHair.onValueChanged.AddListener(delegate { HairDropdownValueChanged(m_DropdownHair); });
        m_DropdownAppearance.onValueChanged.AddListener(delegate { AppearanceDropdownValueChanged(m_DropdownAppearance); });
        m_DropdownSuit.onValueChanged.AddListener(delegate { SuitDropdownValueChanged(m_DropdownSuit); });

        clothingOptionStr.Clear();
        for (int i = 0; i < m_DropdownHair.options.Count; i++)
        {
            clothingOptionStr.Add(m_DropdownHair.options[i].text);
        }

        furnAndActOptionStr.Clear();
        for (int i = 0; i < m_DropdownFurniture.options.Count; i++)
        {
            furnAndActOptionStr.Add(m_DropdownFurniture.options[i].text);
        }
    }

    private void FurnitureDropdownValueChanged(Dropdown change)
    {
        ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] = change.value;
        SetDropdownItem(change, change.value, furnAndActOptionStr);
        SortFurnAndActByType(change.value);
        SetFunrnitureInfo();
    }

    private void ActionDropdownValueChanged(Dropdown change)
    {
        ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] = change.value;
        SetDropdownItem(change, change.value, furnAndActOptionStr);
        SortFurnAndActByType(change.value);
        SetActionInfo();
    }

    private void HairDropdownValueChanged(Dropdown change)
    {
        ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_HeadTabType] = change.value;
        SetDropdownItem(change, change.value, clothingOptionStr);
        SortClothingByType(change.value);
    }

    private void AppearanceDropdownValueChanged(Dropdown change)
    {
        ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_AppearanceTableType] = change.value;
        SetDropdownItem(change, change.value, clothingOptionStr);
        SortClothingByType(change.value);
    }

    private void SuitDropdownValueChanged(Dropdown change)
    {
        ManageMentClass.DataManagerClass.dicClothingSortType[(int)AppearanceTableType.Suit] = change.value;
        SetDropdownItem(change, change.value, clothingOptionStr);
        SortClothingByType(change.value);
    }

    private void SetDropdownItem(Dropdown dropdown, int curVal, List<string> optionsStr)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (curVal == i)
            {
                dropdown.options[i].text = string.Format("<color=#FFFFFF>{0}</color>", optionsStr[i]);
            }
            else
            {
                dropdown.options[i].text = string.Format("<color=#2FD8E3>{0}</color>", optionsStr[i]);
            }
        }
        dropdown.captionText.text = optionsStr[curVal];
    }

    /// <summary>
    /// 家具
    /// </summary>
    private void ShowFurniture()
    {
        MessageManager.GetInstance().RequestShopData((int)m_FurnitureActionType, () =>
        {
            m_MallServerData = MessageManager.GetInstance().GetMallData();
            int sortIndex = ManageMentClass.DataManagerClass.dicFurnAndActSortType.ContainsKey((int)m_PageType) ? ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] : 0;
            SortFurnAndActByType(sortIndex);
            SetFunrnitureInfo();
        });
    }

    public void SortFurntureData()
    {
        SortHelper.Sort(m_MallServerData.Values.ToArray(), (a, b) =>
        {
            return a.item_id < b.item_id;//id降序
        });
    }

    public void SetFunrnitureInfo()
    {
        int count = m_MallServerData.Count;
        m_FurnitureActionVerticalScroll.Init(InitFurnitureItemInfoCallBack);
        m_FurnitureActionVerticalScroll.ShowList(count);
        m_FurnitureActionVerticalScroll.ResetScrollRect();
        m_DropdownFurniture.gameObject.SetActive(count > 0);
    }

    private void InitFurnitureItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        FurnitureShopItem itemData = cell.transform.GetComponent<FurnitureShopItem>();
        if (itemData != null)
        {
            int key = m_MallServerData.ElementAt(index - 1).Key;
            MallServerData mall;
            m_MallServerData.TryGetValue(key, out mall);
            item item;
            m_FurntureItemConfigData.TryGetValue(mall.item_id, out item);
            if (item == null)
                return;
            if (index == 1)
            {
                m_SelectItemId = mall.item_id;
            }

            itemData.SetItemIcon(item.item_icon);
            itemData.SetItemName(item.item_name);
            //itemData.SetItemNum(mall.num);
            itemData.SetItemPrice(mall.price);
            itemData.SetItemData(item);
            itemData.SetMallServerData(mall);
            itemData.SetItemTabType(mall.item_type1);
            itemData.SetItemId(item.item_id);
            itemData.SetItemSelectState(m_SelectItemId == item.item_id);
            m_FurntureItemData[item.item_id] = itemData;
        }
    }

    private void GetFurnitureConfigData()
    {
        Dictionary<int, mall> mallConfigData = GetShopConfigData();
        Dictionary<int, item> itemConfigData = GetItemConfigData();
        foreach (var item in mallConfigData)
        {
            if (item.Value.item_type1 == (int)FurnitureActionType.Furniture)
            {
                item itemConfig;
                itemConfigData.TryGetValue(item.Value.item_id, out itemConfig);
                m_FurntureItemConfigData[item.Value.item_id] = itemConfig;
            }
        }
    }

    private void GetActionConfigData()
    {
        Dictionary<int, mall> mallConfigData = GetShopConfigData();
        Dictionary<int, item> itemConfigData = GetItemConfigData();
        foreach (var item in mallConfigData)
        {
            if (item.Value.item_type1 == (int)FurnitureActionType.Action)
            {
                item itemConfig;
                itemConfigData.TryGetValue(item.Value.item_id, out itemConfig);
                m_ActionItemConfigData[item.Value.item_id] = itemConfig;
            }
        }
    }

    private void Clear()
    {
        m_MallServerData.Clear();
        m_FurntureItemConfigData.Clear();
        m_ActionItemConfigData.Clear();
        m_FurntureItemData.Clear();
        m_ActionItemData.Clear();
    }

    private void InitConfig()
    {
        Clear();
        GetFurnitureConfigData();
        GetActionConfigData();
    }

    private Dictionary<int, mall> GetShopConfigData()
    {
        mallContainer m_mallContainer = BinaryDataMgr.Instance.LoadTableById<mallContainer>("mallContainer");
        if (m_mallContainer != null)
        {
            return m_mallContainer.dataDic;
        }
        return new Dictionary<int, mall>();
    }

    private Dictionary<int, item> GetItemConfigData()
    {
        itemContainer m_itemContainer = BinaryDataMgr.Instance.LoadTableById<itemContainer>("itemContainer");
        if (m_itemContainer != null)
        {
            return m_itemContainer.dataDic;
        }
        return new Dictionary<int, item>();
    }

    public void SetFurntureItemSelectState(int id)
    {
        foreach (var item in m_FurntureItemData)
        {
            if (item.Value.GetItemId() == id)
            {
                item.Value.SetItemSelectState(true);
            }
            else
            {
                item.Value.SetItemSelectState(false);
            }
        }
    }
    public void SetFurntureItemDefaultState()
    {
        foreach (var item in m_FurntureItemData)
        {
            if (item.Value != null)
            {
                item.Value.SetItemSelectState(false);
            }
        }
    }

    public void SetActionItemSelectState(int id)
    {
        foreach (var item in m_ActionItemData)
        {
            if (item.Value.GetItemId() == id)
            {
                item.Value.SetItemSelectState(true);
            }
            else
            {
                item.Value.SetItemSelectState(false);
            }
        }
    }
    public void SetActionItemDefaultState()
    {
        foreach (var item in m_ActionItemData)
        {
            if (item.Value != null)
            {

                item.Value.SetItemSelectState(false);
            }
        }
    }

    private void ShowAction()
    {
        MessageManager.GetInstance().RequestShopData((int)m_FurnitureActionType, () =>
        {
            m_MallServerData = MessageManager.GetInstance().GetMallData();
            int sortIndex = ManageMentClass.DataManagerClass.dicFurnAndActSortType.ContainsKey((int)m_PageType) ? ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] : 0;
            SortFurnAndActByType(sortIndex);
            SetActionInfo();
        });
    }


    private void SetActionInfo()
    {
        int count = m_MallServerData.Count;
        m_FurnitureActionVerticalScroll.Init(InitActionItemInfoCallBack);
        m_FurnitureActionVerticalScroll.ShowList(count);
        m_FurnitureActionVerticalScroll.ResetScrollRect();
        m_NoneText.gameObject.SetActive(count <= 0);
        m_DropdownAction.gameObject.SetActive(count > 0);
    }

    public void SortActionData()
    {
        SortHelper.Sort(m_MallServerData.Values.ToArray(), (a, b) =>
        {
            if (a.has_num < 0 && b.has_num < 0)
            {
                return a.item_id < b.item_id;
            }

            if (a.has_num > 0 && b.has_num > 0)
            {
                return a.item_id < b.item_id;
            }

            return a.has_num < 0 && b.has_num > 0;
        });
    }

    private void InitActionItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        FurnitureShopItem itemData = cell.transform.GetComponent<FurnitureShopItem>();
        if (itemData != null)
        {
            MallServerData mall;
            m_MallServerData.TryGetValue(index - 1, out mall);
            item item;
            m_ActionItemConfigData.TryGetValue(mall.item_id, out item);
            if (item == null)
                return;
            itemData.SetItemIcon(item.item_icon);
            itemData.SetItemName(item.item_name);
            //itemData.SetItemNum(mall.num);
            itemData.SetItemPrice(mall.price);
            itemData.SetItemData(item);
            itemData.SetMallServerData(mall);
            itemData.SetItemTabType(mall.item_type1);
            itemData.SetItemId(item.item_id);
            itemData.SetItemSelectState(m_SelectItemId == item.item_id);
            m_ActionItemData[item.item_id] = itemData;
        }
    }

    private void ShowOutFitList()
    {
        shopOutFitItemList = GetSecondTabTypeOutFitData(m_TabType);
        int sortIndex = 0;
        if (m_PageType == PageType.Furniture || m_PageType == PageType.Action)
        {
            sortIndex = ManageMentClass.DataManagerClass.dicFurnAndActSortType.ContainsKey((int)m_PageType) ? ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] : 0;
        }
        if (m_TabType == TableType.Head)
        {
            sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_HeadTabType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_HeadTabType] : 0;
        }
        if (m_TabType == TableType.Appearance)
        {
            sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_AppearanceTableType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_AppearanceTableType] : 0;
            m_DropdownAppearance.value = sortIndex;
            m_DropdownAppearance.captionText.text = clothingOptionStr[sortIndex];
        }
        if (m_TabType == TableType.Suit)
        {
            sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_AppearanceTableType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_AppearanceTableType] : 0;
        }

        SortClothingByType(sortIndex);
        UpdateOutFitList();
    }

    private void UpdateOutFitList()
    {
        if (m_TabType != TableType.Suit)
        {
            m_AppearanceVerticalScroll.gameObject.SetActive(true);
            m_SuitVerticalScroll.gameObject.SetActive(false);

            m_AppearanceVerticalScroll.Init(InitItemInfoCallBack);
            m_AppearanceVerticalScroll.ShowList(shopOutFitItemList.Count);
            m_AppearanceVerticalScroll.ResetScrollRect();
        }
        else
        {
            m_SuitVerticalScroll.gameObject.SetActive(false);
            m_SuitVerticalScroll.gameObject.SetActive(true);
            m_SuitVerticalScroll.Init(InitItemInfoCallBack);
            m_SuitVerticalScroll.ShowList(shopOutFitItemList.Count);
            m_SuitVerticalScroll.ResetScrollRect();
        }
    }
    /// <summary>
    /// 服装默认排序
    /// </summary>
    private void SortOutFitDataDefault()
    {
        if (shopOutFitItemList == null)
            return;
        shopOutFitItemList.Sort((a, b) =>
        {
            bool bNewA = a.is_new == 1;
            bool bLimitNumA = a.items_limited == 1;
            bool bLimitTimeA = a.items_limited_time_off_timestamp > 0;

            bool bNewB = b.is_new == 1;
            bool bLimitNumB = b.items_limited == 1;
            bool bLimitTimeB = b.items_limited_time_off_timestamp > 0;

            bool bInAllA = bNewA && bLimitNumA && bLimitNumA;
            bool bInAllB = bNewB && bLimitNumB && bLimitTimeB;

            bool bLimitTimeAndNumA = bLimitNumA && bLimitTimeA;
            bool bLimitTimeAndNumB = bLimitNumB && bLimitTimeB;

            if (bInAllA.CompareTo(bInAllB) != 0)
            {
                return -(bInAllA.CompareTo(bInAllB));
            }
            else if (bNewA.CompareTo(bNewB) != 0)
            {
                return -(bNewA.CompareTo(bNewB));
            }
            else if (bLimitTimeAndNumA.CompareTo(bLimitTimeAndNumB) != 0)
            {
                return -(bLimitTimeAndNumA.CompareTo(bLimitTimeAndNumB));
            }
            else if (bLimitNumA.CompareTo(bLimitNumB) != 0 || bLimitTimeA.CompareTo(bLimitTimeB) != 0)
            {
                if (bLimitNumA.CompareTo(bLimitNumB) != 0)
                {
                    return -(bLimitNumA.CompareTo(bLimitNumB));
                }
                if (bLimitTimeA.CompareTo(bLimitTimeB) != 0)
                {
                    return -(bLimitTimeA.CompareTo(bLimitTimeB));
                }
            }
            else if (a.avatar_rare.CompareTo(b.avatar_rare) != 0)
            {
                return -(a.avatar_rare.CompareTo(b.avatar_rare));//降序
            }
            return 1;
        });
    }
    /// <summary>
    /// 获取的是二级数据
    /// </summary>
    /// <param name="tabType"></param>
    /// <returns></returns>
    public List<ShopOutFitItem> GetOutFitDataByTableType(TableType tabType)
    {
        if (shopOutFitRecDataList == null)
            return null;
        List<ShopOutFitItem> filterData = new List<ShopOutFitItem>();
        foreach (var item in shopOutFitRecDataList)
        {
            if (item.avatar_type1 == (int)tabType)
            {
                foreach (var data in item.list)
                {
                    filterData.Add(data);
                }
            }
        }
        return filterData;
    }

    /// <summary>
    /// 当前二级页签数据
    /// </summary>
    /// <param name="tabType"></param>
    /// <returns></returns>
    public List<ShopOutFitItem> GetSecondTabTypeOutFitData(TableType tabType)
    {
        List<ShopOutFitItem> filterData = GetOutFitDataByTableType(tabType);
        List<ShopOutFitItem> data = new List<ShopOutFitItem>();
        switch (tabType)
        {
            case TableType.Head:
                data = GetOutFitDataByHeadType(filterData, m_HeadTabType);
                break;
            case TableType.Appearance:
                data = GetOutFitDataByAppearanceType(filterData, m_AppearanceTableType);
                break;
            case TableType.Suit:
                data = GetOutFitDataBySuitType(filterData);
                break;
        }
        return data;
    }

    private List<ShopOutFitItem> GetOutFitDataByHeadType(List<ShopOutFitItem> filterData, HeadTableType headTableType)
    {
        List<ShopOutFitItem> headTabData = new List<ShopOutFitItem>();
        foreach (var item in filterData)
        {
            if (item.avatar_type2 == (int)headTableType)
            {
                headTabData.Add(item);
            }
        }
        return headTabData;
    }
    private List<ShopOutFitItem> GetOutFitDataByAppearanceType(List<ShopOutFitItem> filterData, AppearanceTableType appearanceTableType)
    {
        List<ShopOutFitItem> appearanceTabData = new List<ShopOutFitItem>();
        foreach (var item in filterData)
        {
            if (item.avatar_type2 == (int)appearanceTableType)
            {
                appearanceTabData.Add(item);
            }
        }
        return appearanceTabData;
    }

    private List<ShopOutFitItem> GetOutFitDataBySuitType(List<ShopOutFitItem> filterData)
    {
        List<ShopOutFitItem> suitTabData = new List<ShopOutFitItem>();
        foreach (var item in filterData)
        {
            if (item.avatar_type1 == (int)TableType.Suit && (item.avatar_type2 == (int)AppearanceTableType.HotMan || item.avatar_type2 == (int)AppearanceTableType.Suit))
            {
                suitTabData.Add(item);
            }
        }
        return suitTabData;
    }

    private void InitItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        ShopClothItem clothingItem = cell.transform.GetComponent<ShopClothItem>();
        if (clothingItem != null)
        {
            ShopOutFitItem data = shopOutFitItemList[index - 1];
            if (data == null)
                return;
            clothingItem.SetItemIcon(data.avatar_id);
            clothingItem.SetItemName(data.avatar_name);
            clothingItem.SetItemNum(data.has_num);
            clothingItem.SetItemQuality(data.avatar_rare);
            clothingItem.SetItemSelectState(false);
            clothingItem.SetItemPrice(data.avatar_id);
            clothingItem.SetItemNew(data.is_new == 1);
            clothingItem.SetLimitNum(data.items_limited == 1, data.items_limited_quantity);
            clothingItem.SetLimitTime(data.items_limited_time_off_timestamp);
            clothingItem.SetAvatarData(data);
            clothingItem.SetNumPos();

            if (!dicClothItems.ContainsKey(data.avatar_id))
            {
                dicClothItems[data.avatar_id] = clothingItem;
            }
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        SetDefaultPage();
        RTManager.GetInstance().LoadCharacter();
        RTManager.GetInstance().SetCharacterVisable(true);


    }

    public void SetToggleGroup()
    {
        m_Toggle_Head.isOn = true;
        m_Toggle_Appearance.isOn = false;
        m_Toggle_Suit.isOn = false;
    }

    private void SetDefaultPage()
    {
        m_PageType = PageType.Appearance;
        m_TabType = TableType.Head;
        m_AppearanceTableType = AppearanceTableType.Coat;
        m_HeadTabType = HeadTableType.Hair;

        m_PlayerRawImg.gameObject.SetActive(true);
        m_ModelRawImg.gameObject.SetActive(false);

        SetPageToggleTextColor(m_PageType);

        m_Trans_ImageToggleGroup.gameObject.SetActive(true);
        m_Trans_AppearanceSecond.gameObject.SetActive(false);
        m_Trans_HeadSecond.gameObject.SetActive(true);

        m_FurnitureActionVerticalScroll.gameObject.SetActive(false);
        m_AppearanceVerticalScroll.gameObject.SetActive(true);
        m_SuitVerticalScroll.gameObject.SetActive(false);

        SetPageToggleGroup();
        SetToggleGroup();
        SetHeadToggleGroup();

        SetToggleTextColor(m_TabType);
        SetSecondTab(m_TabType);

        SetDropdownVisible();
        SortClothingByType(m_DropdownHair.value);
    }

    public void SetPageToggleGroup()
    {
        m_Toggle_Furniture.isOn = false;
        m_Toggle_Image.isOn = true;
        m_Toggle_Action.isOn = false;
    }

    public void SetHeadToggleGroup()
    {
        m_Toggle_Hair.isOn = true;//m_HeadTabType == HeadTableType.Hair;
        m_Toggle_Glasses.isOn = false;//m_HeadTabType == HeadTableType.Eye;
        m_Toggle_Earring.isOn = false;//m_HeadTabType == HeadTableType.Eyebrow;
        m_Toggle_Necklace.isOn = false;//m_HeadTabType == HeadTableType.Nose;
        //m_Toggle_Mouth.isOn = false;//m_HeadTabType == HeadTableType.Mouth;
        //m_Toggle_Ear.isOn = false;//m_HeadTabType == HeadTableType.Ear;
    }

    public void SetAppearanceToggleGroup()
    {
        m_Toggle_Coat.isOn = true;//m_AppearanceTableType == AppearanceTableType.Coat;
        m_Toggle_Underwear.isOn = false;//m_AppearanceTableType == AppearanceTableType.Underwear;
        m_Toggle_Shoe.isOn = false;//m_AppearanceTableType == AppearanceTableType.Shoe;
        //m_Toggle_Bag.isOn = false;//m_AppearanceTableType == AppearanceTableType.Bag;
        m_Toggle_Watch.isOn = false;//m_AppearanceTableType == AppearanceTableType.Watch;
        //m_Toggle_Rings.isOn = false;//m_AppearanceTableType == AppearanceTableType.Rings;
    }

    private void SetPageToggleTextColor(PageType pageType)
    {
        switch (pageType)
        {
            case PageType.Furniture:
                if (m_ToggleFurnitureTxt != null)
                    m_ToggleFurnitureTxt.color = new Color(51f / 255f, 51f / 255f, 50f / 255f);
                if (m_ToggleImageTxt != null)
                    m_ToggleImageTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleActionTxt != null)
                    m_ToggleActionTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case PageType.Appearance:
                if (m_ToggleFurnitureTxt != null)
                    m_ToggleFurnitureTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleImageTxt != null)
                    m_ToggleImageTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
                if (m_ToggleActionTxt != null)
                    m_ToggleActionTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case PageType.Action:
                if (m_ToggleFurnitureTxt != null)
                    m_ToggleFurnitureTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleImageTxt != null)
                    m_ToggleImageTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleActionTxt != null)
                    m_ToggleActionTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
                break;
        }
    }

    private void SetToggleTextColor(TableType tabType)
    {
        switch (tabType)
        {
            case TableType.Head:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case TableType.Appearance:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case TableType.Suit:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(0f / 255f, 0f / 255f, 0f / 255f);
                break;
        }
    }

    private void SetSecondTab(TableType tabType)
    {
        switch (tabType)
        {
            case TableType.Head:
                if (m_Trans_HeadSecond != null)
                    m_Trans_HeadSecond.gameObject.SetActive(true);
                if (m_Trans_AppearanceSecond != null)
                    m_Trans_AppearanceSecond.gameObject.SetActive(false);
                break;
            case TableType.Appearance:
                if (m_Trans_HeadSecond != null)
                    m_Trans_HeadSecond.gameObject.SetActive(false);
                if (m_Trans_AppearanceSecond != null)
                    m_Trans_AppearanceSecond.gameObject.SetActive(true);
                break;
            case TableType.Suit:
                if (m_Trans_HeadSecond != null)
                    m_Trans_HeadSecond.gameObject.SetActive(false);
                if (m_Trans_AppearanceSecond != null)
                    m_Trans_AppearanceSecond.gameObject.SetActive(false);
                break;
        }
    }
    public override void Redisplay()
    {
        base.Redisplay();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        SetDefaultPage();
        AvatarManager.Instance().RefreshPlayerFun();
        RTManager.GetInstance().ResetCharacter();
        RTManager.GetInstance().DestroyCharacter();
        StartCoroutine(DeleTimeOpen());
    }

    IEnumerator DeleTimeOpen()
    {
        yield return null;
        InterfaceHelper.SetJoyStickState(true);
        base.Hiding();
    }

    public void SetSelect(int avatarId)
    {
        foreach (var item in dicClothItems)
        {
            if (item.Value.avatarData.avatar_id == avatarId)
            {
                item.Value.SetItemSelectState(true);
            }
            else
            {
                item.Value.SetItemSelectState(false);
            }
        }
    }

    public PageType GetPageType()
    {
        return m_PageType;
    }
    /// <summary>
    /// 按稀有度降序
    /// </summary>
    private void SortByQualityDescendOutFitData()
    {
        if (shopOutFitItemList == null)
            return;
        shopOutFitItemList.Sort((a, b) =>
        {
            if (a.avatar_rare.CompareTo(b.avatar_rare) != 0)
            {
                return -(a.avatar_rare.CompareTo(b.avatar_rare));
            }
            return 1;
        });
    }
    /// <summary>
    /// 按稀有度升序
    /// </summary>
    private void SortByQualityAscendOutFitData()
    {
        if (shopOutFitItemList == null)
            return;
        shopOutFitItemList.Sort((a, b) =>
        {
            if (a.avatar_rare.CompareTo(b.avatar_rare) != 0)
            {
                return a.avatar_rare.CompareTo(b.avatar_rare);
            }
            return 1;
        });
    }

    /// <summary>
    /// 按价格降序
    /// </summary>
    private void SortByPriceDescendOutFitData()
    {
        if (shopOutFitItemList == null)
            return;
        shopOutFitItemList.Sort((a, b) =>
        {
            mall mallConfigA = ManageMentClass.DataManagerClass.GetMallTableFun(a.avatar_id);
            mall mallConfigB = ManageMentClass.DataManagerClass.GetMallTableFun(b.avatar_id);

            if (mallConfigA.price.CompareTo(mallConfigB.price) != 0)
            {
                return -(mallConfigA.price.CompareTo(mallConfigB.price));
            }
            return 1;
        });
    }


    /// <summary>
    /// 按价格升序
    /// </summary>
    private void SortByPriceAscendOutFitData()
    {
        if (shopOutFitItemList == null)
            return;
        shopOutFitItemList.Sort((a, b) =>
        {
            mall mallConfigA = ManageMentClass.DataManagerClass.GetMallTableFun(a.avatar_id);
            mall mallConfigB = ManageMentClass.DataManagerClass.GetMallTableFun(b.avatar_id);

            if (mallConfigA.price.CompareTo(mallConfigB.price) != 0)
            {
                return (mallConfigA.price.CompareTo(mallConfigB.price));
            }
            return 1;
        });
    }

    /// <summary>
    /// 家具动作按价格降序
    /// </summary>
    private void SortByPriceDescendFurAct()
    {
        if (m_MallServerData == null)
            return;
        List<KeyValuePair<int, MallServerData>> data = m_MallServerData.ToList();
        data.Sort((a, b) =>
        {
            MallServerData dataA = a.Value;
            MallServerData dataB = b.Value;
            if (dataA.price.CompareTo(dataB.price) != 0)
            {
                return -(dataA.price.CompareTo(dataB.price));
            }
            return 1;
        });
        m_MallServerData = data.ToDictionary(item => item.Key, item => item.Value);
    }


    /// <summary>
    /// 家具动作按价格升序
    /// </summary>
    private void SortByPriceAscendFurAct()
    {
        if (m_MallServerData == null)
            return;
        List<KeyValuePair<int, MallServerData>> data = m_MallServerData.ToList();
        data.Sort((a, b) =>
        {
            MallServerData dataA = a.Value;
            MallServerData dataB = b.Value;
            if (dataA.price.CompareTo(dataB.price) != 0)
            {
                return (dataA.price.CompareTo(dataB.price));
            }
            return 1;
        });
        m_MallServerData = data.ToDictionary(item => item.Key, item => item.Value);
    }

    public void SetDropdownVisible()
    {
        if (m_PageType == PageType.Furniture)
        {
            m_DropdownFurniture.gameObject.SetActive(true);
            m_DropdownHair.gameObject.SetActive(false);
            m_DropdownAppearance.gameObject.SetActive(false);
            m_DropdownSuit.gameObject.SetActive(false);
            m_DropdownAction.gameObject.SetActive(false);

            int sortIndex = ManageMentClass.DataManagerClass.dicFurnAndActSortType.ContainsKey((int)m_PageType) ? ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] : 0;
            m_DropdownFurniture.value = sortIndex;
            m_DropdownFurniture.captionText.text = furnAndActOptionStr[sortIndex];
            SetDropdownItem(m_DropdownFurniture, m_DropdownFurniture.value, furnAndActOptionStr);
        }
        if (m_PageType == PageType.Appearance && m_TabType == TableType.Head)
        {
            m_DropdownFurniture.gameObject.SetActive(false);
            m_DropdownHair.gameObject.SetActive(true);
            m_DropdownAppearance.gameObject.SetActive(false);
            m_DropdownSuit.gameObject.SetActive(false);
            m_DropdownAction.gameObject.SetActive(false);

            int sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_HeadTabType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_HeadTabType] : 0;
            m_DropdownHair.value = sortIndex;
            m_DropdownHair.captionText.text = clothingOptionStr[sortIndex];
            SetDropdownItem(m_DropdownHair, m_DropdownHair.value, clothingOptionStr);
        }
        if (m_PageType == PageType.Appearance && m_TabType == TableType.Appearance)
        {
            m_DropdownFurniture.gameObject.SetActive(false);
            m_DropdownHair.gameObject.SetActive(false);
            m_DropdownAppearance.gameObject.SetActive(true);
            m_DropdownSuit.gameObject.SetActive(false);
            m_DropdownAction.gameObject.SetActive(false);

            int sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_AppearanceTableType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_AppearanceTableType] : 0;
            m_DropdownAppearance.value = sortIndex;
            m_DropdownAppearance.captionText.text = clothingOptionStr[sortIndex];
            SetDropdownItem(m_DropdownAppearance, m_DropdownAppearance.value, clothingOptionStr);
        }
        if (m_PageType == PageType.Appearance && m_TabType == TableType.Suit)
        {
            m_DropdownFurniture.gameObject.SetActive(false);
            m_DropdownHair.gameObject.SetActive(false);
            m_DropdownAppearance.gameObject.SetActive(false);
            m_DropdownSuit.gameObject.SetActive(true);
            m_DropdownAction.gameObject.SetActive(false);

            int sortIndex = ManageMentClass.DataManagerClass.dicClothingSortType.ContainsKey((int)m_AppearanceTableType) ? ManageMentClass.DataManagerClass.dicClothingSortType[(int)m_AppearanceTableType] : 0;
            m_DropdownSuit.value = sortIndex;
            m_DropdownSuit.captionText.text = clothingOptionStr[sortIndex];
            SetDropdownItem(m_DropdownSuit, m_DropdownSuit.value, clothingOptionStr);
        }

        if (m_PageType == PageType.Action)
        {
            m_DropdownFurniture.gameObject.SetActive(false);
            m_DropdownHair.gameObject.SetActive(false);
            m_DropdownAppearance.gameObject.SetActive(false);
            m_DropdownSuit.gameObject.SetActive(false);
            m_DropdownAction.gameObject.SetActive(true);

            int sortIndex = ManageMentClass.DataManagerClass.dicFurnAndActSortType.ContainsKey((int)m_PageType) ? ManageMentClass.DataManagerClass.dicFurnAndActSortType[(int)m_PageType] : 0;
            m_DropdownAction.value = sortIndex;
            m_DropdownAction.captionText.text = furnAndActOptionStr[sortIndex];
            SetDropdownItem(m_DropdownAction, m_DropdownAction.value, furnAndActOptionStr);
        }
    }
    private void SortFurnAndActByType(int type)
    {
        switch (type)
        {
            case (int)SortyType.Default:
                if (m_PageType == PageType.Furniture)
                {
                    SortFurntureData();
                }
                else if (m_PageType == PageType.Action)
                {
                    SortActionData();
                }
                break;
            case (int)SortyType.PriceAsc:
                SortByPriceAscendFurAct();
                break;
            case (int)SortyType.PriceDesc:
                SortByPriceDescendFurAct();
                break;
        }
    }
    private void SortClothingByType(int type)
    {
        switch (type)
        {
            case (int)SortyType.Default:
                SortOutFitDataDefault();
                break;
            case (int)SortyType.PriceAsc:
                SortByPriceAscendOutFitData();
                break;
            case (int)SortyType.PriceDesc:
                SortByPriceDescendOutFitData();
                break;
            case (int)SortyType.QualityAsc:
                SortByQualityAscendOutFitData();
                break;
            case (int)SortyType.QualityDesc:
                SortByQualityDescendOutFitData();
                break;
        }
        UpdateOutFitList();
    }
}
