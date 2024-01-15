using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


public class ShopUIForm : BaseUIForm
{
    public Toggle m_Toggle_furniture;
    public Toggle m_Toggle_action;
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public Text m_Toggle1Txt;
    public Text m_Toggle2Txt;
    public Text m_TextNone;
    public Image m_ImgMask;
    public enum TableType
    {
        Furniture = 1,
        Action = 2,
    }
    public TableType m_TableType = TableType.Furniture;
    private Dictionary<int, item> m_FurntureItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, item> m_ActionItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, ItemData> m_FurntureItemData = new Dictionary<int, ItemData>();
    private Dictionary<int, ItemData> m_ActionItemData = new Dictionary<int, ItemData>();
    private Dictionary<int, MallServerData> m_MallServerData = new Dictionary<int, MallServerData>();
    private int m_SelectItemId = 0;
    void Awake()
    {
        InitConfig();
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        //注册按钮事件：退出
        RigisterButtonObjectEvent("Btn_Close",
            P =>
            {
                CloseUIForm();
                InterfaceHelper.SetJoyStickState(true);
            }
        );

        ReceiveMessage("ShopFurntureItem",
            p =>
            {
                int itemId = (int)p.Values;
                m_SelectItemId = itemId;
                SetFurntureItemSelectState(itemId);
            }
        );

        ReceiveMessage("ShopActionItem",
            p =>
            {
                int selectId = (int)p.Values;
                m_SelectItemId = selectId;
                SetActionItemSelectState(selectId);
            }
        );

        ReceiveMessage("ShopFurntureBuy",
            p =>
            {
                object[] args = p.Values as object[];
                item m_ItemData = (item)args[1];
                if (m_ItemData != null)
                {
                    SetFurntureItemSelectState(m_ItemData.item_id);
                }
            }
        );

        ReceiveMessage("ShopActionBuy",
            p =>
            {
                string[] args = p.Values as string[];
                int m_ActionId = 0;
                int.TryParse(args[0], out m_ActionId);
                SetActionItemSelectState(m_ActionId);
            }
        );

        ReceiveMessage("ShopBuySuccess",
            p =>
            {
                Debug.Log("输出一下成功C");
                UIManager.GetInstance().CloseUIForms(FormConst.SHOPBUYTIPS_UIFORM);
                if (gameObject.activeSelf == false)
                {
                    return;
                }
                int[] args = p.Values as int[];
                int m_BuyItemId = args[0];
                int m_BuyNum = args[1];
                MessageManager.GetInstance().RequestShopData((int)m_TableType, () =>
                 {
                     m_MallServerData = MessageManager.GetInstance().GetMallData();
                     if (m_TableType == TableType.Furniture)
                     {
                         SortFurntureData();
                         ShowFurntureServerData(false);
                     }
                     else
                     {
                         SortActionData();
                         ShowActionServerData();
                     }
                 });
            }
        );

        m_Toggle_furniture.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                m_TableType = TableType.Furniture;
            else
                m_TableType = TableType.Action;
            SetToggleTextColor(true);
            MessageManager.GetInstance().RequestShopData((int)m_TableType, () =>
            {
                m_MallServerData = MessageManager.GetInstance().GetMallData();
                if (m_TableType == TableType.Furniture)
                {
                    SortFurntureData();
                    ShowFurntureServerData(true);
                }
                else
                {
                    SortActionData();
                    ShowActionServerData();
                }
            });
        });
        m_Toggle_action.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                m_TableType = TableType.Action;
            else
                m_TableType = TableType.Furniture;

            SetToggleTextColor(false);
            MessageManager.GetInstance().RequestShopData((int)m_TableType, () =>
            {
                m_MallServerData = MessageManager.GetInstance().GetMallData();
                if (m_TableType == TableType.Furniture)
                {
                    SortFurntureData();
                    ShowFurntureServerData(true);
                }
                else
                {
                    SortActionData();
                    ShowActionServerData();
                }
            });
        });
    }

    private void SetToggleTextColor(bool isOn)
    {
        if (isOn)
        {
            if (m_Toggle1Txt != null)
                m_Toggle1Txt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
            if (m_Toggle2Txt != null)
                m_Toggle2Txt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
        }
        else
        {
            if (m_Toggle1Txt != null)
                m_Toggle1Txt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
            if (m_Toggle2Txt != null)
                m_Toggle2Txt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
        }
    }
    private void ShowFurntureServerData(bool bResetScrollRect)
    {
        int count = m_MallServerData.Count;
        m_VerticalScroll.Init(InitFurnitureItemInfoCallBack);
        m_VerticalScroll.ShowList(count);
        if (bResetScrollRect)
            m_VerticalScroll.ResetScrollRect();
        if (m_TextNone != null)
            m_TextNone.gameObject.SetActive(count <= 0);
        if (m_ImgMask != null)
            m_ImgMask.gameObject.SetActive(count > 0);
    }

    private void ShowActionServerData()
    {
        int count = m_MallServerData.Count;
        m_VerticalScroll.Init(InitActionItemInfoCallBack);
        m_VerticalScroll.ShowList(count);
        m_VerticalScroll.ResetScrollRect();
        if (m_TextNone != null)
            m_TextNone.gameObject.SetActive(count <= 0);
        if (m_ImgMask != null)
            m_ImgMask.gameObject.SetActive(count > 0);
    }


    private void InitFurnitureItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        ItemData itemData = cell.transform.GetComponent<ItemData>();
        if (itemData != null)
        {
            MallServerData mall;
            m_MallServerData.TryGetValue(index - 1, out mall);
            item item;
            m_FurntureItemConfigData.TryGetValue(mall.item_id, out item);
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
            itemData.SetItemUnLockState(false);
            itemData.SetItemLockState(false);
            m_FurntureItemData[item.item_id] = itemData;
        }
    }

    private void InitActionItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        ItemData itemData = cell.transform.GetComponent<ItemData>();
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
            itemData.SetItemUnLockState(mall.has_num > 0);
            itemData.SetItemLockState(false);
            m_ActionItemData[item.item_id] = itemData;
        }
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
    private void GetFurnitureConfigData()
    {
        Dictionary<int, mall> mallConfigData = GetShopConfigData();
        Dictionary<int, item> itemConfigData = GetItemConfigData();
        foreach (var item in mallConfigData)
        {
            if (item.Value.item_type1 == (int)TableType.Furniture)
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
            if (item.Value.item_type1 == (int)TableType.Action)
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
        //DisableFunc();
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
    public override void Display()
    {
        base.Display();
        m_Toggle_furniture.isOn = true;
        m_Toggle_action.isOn = false;
        //SetToggleTextColor(true);

        m_TableType = TableType.Furniture;
        m_MallServerData = MessageManager.GetInstance().GetMallData();
        if (m_TableType == TableType.Furniture)
        {
            SortFurntureData();
            ShowFurntureServerData(true);
        }
        else
        {
            SortActionData();
            ShowActionServerData();
        }
        SetFurntureItemDefaultState();
    }
    public override void Redisplay()
    {
        base.Redisplay();
    }
    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        m_Toggle_furniture.isOn = true;
        m_Toggle_action.isOn = false;
    }
    //public void RequestShopData(bool bRestScrollRect)
    //{
    //    StartCoroutine(RequestData(bRestScrollRect));
    //}
    //IEnumerator RequestData(bool bRestScrollRect)
    //{
    //    HttpRequest httpRequest = new HttpRequest();
    //    ShopData shopData = new ShopData((int)m_TableType);
    //    string data = JsonConvert.SerializeObject(shopData);
    //    StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallProps, ManageMentClass.DataManagerClass.tokenValue_Game, data));
    //    while (!httpRequest.isComPlete)
    //    {
    //        yield return null;
    //    }
    //    if (httpRequest.isSucc)
    //    {
    //        JObject jo = JObject.Parse(httpRequest.value);
    //        if ((int)jo["code"] == 0)
    //        {
    //            var listData = jo["data"]["list"];
    //            int index = 0;
    //            m_MallServerData.Clear();
    //            foreach (var item in listData)
    //            {
    //                MallServerData _data = new MallServerData();
    //                _data.num = (int)item["num"];
    //                _data.item_id = (int)item["item_id"];
    //                _data.item_name = item["item_name"].ToString();
    //                _data.item_type1 = (int)item["item_type1"];
    //                _data.sale_mode = (int)item["sale_mode"];
    //                _data.product_number = item["product_number"].ToString();
    //                _data.coin_type = (int)item["coin_type"];
    //                _data.price = (int)item["price"];
    //                _data.has_num = (int)item["has_num"];
    //                _data.is_used = (int)item["is_used"];
    //                if (_data.item_type1 == (int)TableType.Action)
    //                {
    //                    animation m_AniConfig = ManageMentClass.DataManagerClass.GetAnimationTableFun(_data.item_id);
    //                    if (m_AniConfig.animation_initial > 0)//初始动作不加入商店列表
    //                    {
    //                        m_MallServerData.Add(index, _data);
    //                    }
    //                }
    //                else
    //                {
    //                    m_MallServerData.Add(index, _data);
    //                }
    //                index += 1;
    //            }
    //            if (m_TableType == TableType.Furniture)
    //            {
    //                SortFurntureData();
    //                ShowFurntureServerData(bRestScrollRect);
    //            }
    //            else
    //            {
    //                SortActionData();
    //                ShowActionServerData();
    //            }
    //        }
    //        else
    //        {
    //            ToastManager.Instance.ShowNewToast(httpRequest.value, 2f);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("请求商店道具列表失败");
    //    }
    //}
    /// <summary>
    /// 家具页签排序 (id降序)
    /// </summary>
    public void SortFurntureData()
    {
        SortHelper.Sort(m_MallServerData.Values.ToArray(), (a, b) =>
        {
            return a.item_id < b.item_id;
        });
    }

    /// <summary>
    /// 动作页签数据排序
    /// </summary>
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

    /// <summary>
    /// 第一期禁用动作页签
    /// </summary>
    public void DisableFunc()
    {
        if (m_Toggle_action != null)
            m_Toggle_action.gameObject.SetActive(false);
    }
}
