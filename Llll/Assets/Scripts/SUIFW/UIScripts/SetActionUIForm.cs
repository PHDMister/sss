using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


public class SetActionUIForm : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public enum AniType
    {
        Single = 0,//单人
        Multi = 1,//多人
    }
    public enum TableType
    {
        None = 0,
        Furniture = 1,
        Action = 2,
        HotMan = 3,
        RecentUse = 4,
    }
    //public HorizontalLayoutGroup m_HorizontallayoutGroup;
    private Dictionary<int, item> m_ActionItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, ItemData> m_ActionItemData = new Dictionary<int, ItemData>();
    //private Dictionary<int, ItemData> m_UseListItemData = new Dictionary<int, ItemData>();

    private Dictionary<int, MallServerData> m_MallServerData = new Dictionary<int, MallServerData>();

    //private Dictionary<int, UseData> m_UsedActData = new Dictionary<int, UseData>();

    private item m_SelectActItem;
    void Awake()
    {
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        //注册按钮事件
        RigisterButtonObjectEvent("Close", p =>
        {
            CloseUIForm();
        });

        ReceiveMessage("SetActionItem",
            p =>
            {
                m_SelectActItem = p.Values as item;
                if (m_SelectActItem != null)
                {
                    SetActionItemSelectState(m_SelectActItem.item_id);
                    if(bHaveAct(m_SelectActItem.item_id))
                    {
                        RequestUseAct();
                    }
                    else
                    {
                        ToastManager.Instance.ShowNewToast(string.Format("动作{0}尚未解锁，无法使用！",m_SelectActItem.item_name), 5f);
                        CloseUIForm();
                    }
                }
            }
        );

        //ReceiveMessage("SetRecentUseItem",
        //    p =>
        //    {
        //        m_SelectActItem = p.Values as item;
        //        if (m_SelectActItem != null)
        //        {
        //            //SetUseListItemSelectState(m_SelectActItem.item_id);
        //            if (bHaveAct(m_SelectActItem.item_id))
        //            {
        //                RequestUseAct();
        //            }
        //            else
        //            {
        //                ToastManager.Instance.ShowNewToast(string.Format("动作{0}尚未解锁，无法使用！", m_SelectActItem.item_name), 5f);
        //                CloseUIForm();
        //            }
        //        }
        //    }
        //);
    }

    private void Start()
    {

    }

    private void ShowActionServerData()
    {
        int count = m_MallServerData.Count;
        m_VerticalScroll.Init(InitActionItemInfoCallBack);
        m_VerticalScroll.ShowList(count);
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
            itemData.SetItemIcon(item.item_icon);
            //itemData.SetItemName(item.item_name);
            itemData.SetItemData(item);
            itemData.SetMallServerData(mall);
            itemData.SetItemTabType((int)TableType.None);
            itemData.SetItemId(item.item_id);
            //itemData.SetItemSelectState(m_SelectActItem != null && m_SelectActItem.item_id == item.item_id);
            itemData.SetItemLockState(mall.has_num <= 0);
            m_ActionItemData[item.item_id] = itemData;
        }
    }

    private Dictionary<int, animation> GetAnimationConfigData()
    {
        animationContainer m_AniContainer = BinaryDataMgr.Instance.LoadTableById<animationContainer>("animationContainer");
        if (m_AniContainer != null)
        {
            return m_AniContainer.dataDic;
        }
        return new Dictionary<int, animation>();
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

    private void GetActionConfigData()
    {
        Dictionary<int, animation> aniConfigData = GetAnimationConfigData();
        Dictionary<int, item> itemConfigData = GetItemConfigData();
        int index = 0;
        foreach (var item in aniConfigData)
        {
            if (item.Value.animation_type == (int)AniType.Single)
            {
                item itemConfig;
                itemConfigData.TryGetValue(item.Value.animation_id, out itemConfig);
                if(itemConfig != null)
                {
                    m_ActionItemConfigData[item.Value.animation_id] = itemConfig;
                    MallServerData _data = new MallServerData();
                    _data.num = 1;
                    _data.item_id = itemConfig.item_id;
                    _data.item_name = itemConfig.item_name;
                    _data.item_type1 = itemConfig.item_type1;
                    _data.sale_mode = 1;
                    _data.product_number = "0";
                    _data.coin_type = 1;
                    _data.price = 0;
                    _data.has_num = 1;
                    _data.is_used = 0;
                    m_MallServerData.Add(index, _data);
                    index++;
                }
            }
        }
    }

    private void Clear()
    {
        m_MallServerData.Clear();
        m_ActionItemConfigData.Clear();
        m_ActionItemData.Clear();
        //m_UseListItemData.Clear();
    }

    private void InitConfig()
    {
        Clear();
        GetActionConfigData();
        SortActionData();
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

    //public void SetUseListItemSelectState(int id)
    //{
    //    foreach (var item in m_UseListItemData)
    //    {
    //        if (item.Value.GetItemId() == id)
    //        {
    //            item.Value.SetItemSelectState(true);
    //        }
    //        else
    //        {
    //            item.Value.SetItemSelectState(false);
    //        }
    //    }
    //}

    public override void Display()
    {
        base.Display();
        InitConfig();
        m_VerticalScroll.ResetScrollRect();
        RequestShopData();
        //RequestUseActList();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Redisplay()
    {
        base.Redisplay();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        SendMessage("SetActionClose", "Close", null);
        InterfaceHelper.SetJoyStickState(true);
    }

    public void RequestShopData()
    {
        StartCoroutine(RequestData());
    }

    IEnumerator RequestData()
    {
        HttpRequest httpRequest = new HttpRequest();
        ShopData shopData = new ShopData((int)TableType.Action);
        string data = JsonConvert.SerializeObject(shopData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallProps, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                var listData = jo["data"]["list"];
                int index = m_MallServerData.Count;
                foreach (var item in listData)
                {
                    MallServerData _data = new MallServerData();
                    _data.num = (int)item["num"];
                    _data.item_id = (int)item["item_id"];
                    _data.item_name = item["item_name"].ToString();
                    _data.item_type1 = (int)item["item_type1"];
                    _data.sale_mode = (int)item["sale_mode"];
                    _data.product_number = item["product_number"].ToString();
                    _data.coin_type = (int)item["coin_type"];
                    _data.price = (int)item["price"];
                    _data.has_num = (int)item["has_num"];
                    _data.is_used = (int)item["is_used"];
                    if (!IsContains(_data.item_id))
                    {
                        m_MallServerData.Add(index, _data);
                        index += 1;
                    }
                }
                SortActionData();
                ShowActionServerData();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
    }

    public void RequestUseAct()
    {
        StartCoroutine(RequestUseActData());
    }
    IEnumerator RequestUseActData()
    {
        HttpRequest httpRequest = new HttpRequest();
        UseData useData = new UseData(m_SelectActItem.item_id);
        string data = JsonConvert.SerializeObject(useData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.UseAction, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                CloseUIForm();
                PlayerPrefs.SetInt("CurUseActId", m_SelectActItem.item_id);
                SendMessage("SetActionItemSuccess", "UseSuccess", m_SelectActItem);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    //public void RequestUseActList()
    //{
    //    StartCoroutine(RequestUseActListData());
    //}
    //IEnumerator RequestUseActListData()
    //{
    //    HttpRequest httpRequest = new HttpRequest();
    //    UseListData args = new UseListData();
    //    string data = JsonConvert.SerializeObject(args);
    //    StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.UseActionList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
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
    //            m_UsedActData.Clear();
    //            foreach (var item in listData)
    //            {
    //                UseData _data = new UseData((int)item["item_id"]);
    //                if(!IsUseListContains(_data.item_id))
    //                {
    //                    m_UsedActData.Add(index, _data);
    //                    index += 1;
    //                }//}
    //            }
    //            //SetUseActListData();
    //        }
    //        else
    //        {
    //            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
    //        }
    //    }
    //    else
    //    {
    //        ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
    //    }
    

    public void SortActionData()
    {
        SortHelper.Sort(m_MallServerData.Values.ToArray(), (a, b) =>
         {
             int m_ActIdA = a.item_id;
             int m_ActIdB = b.item_id;
             animation m_ActAConf = ManageMentClass.DataManagerClass.GetAnimationTableFun(m_ActIdA);
             animation m_ActBConf = ManageMentClass.DataManagerClass.GetAnimationTableFun(m_ActIdB);
             if (m_ActAConf != null && m_ActBConf != null)
             {
                 if (m_ActAConf.animation_type != m_ActBConf.animation_type)
                 {
                     return m_ActAConf.animation_type > m_ActBConf.animation_type;
                 }
                 else
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
                 }
             }
             return false;
         }
        );
    }

    //public void SetUseActListData()
    //{
    //    GameObject m_TempObj = null;
    //    if (m_HorizontallayoutGroup != null)
    //    {
    //        for (int i = 0; i < m_HorizontallayoutGroup.transform.childCount; i++)
    //        {
    //            GameObject obj = m_HorizontallayoutGroup.transform.GetChild(i).gameObject;
    //            if (obj.activeSelf)
    //            {
    //                Destroy(obj);
    //            }
    //            else
    //            {
    //                m_TempObj = obj;
    //            }
    //        }
    //    }
    //    foreach (var item in m_UsedActData)
    //    {
    //        GameObject m_ActObj = Instantiate(m_TempObj, m_HorizontallayoutGroup.transform);
    //        m_ActObj.SetActive(true);
    //        ItemData m_ItemData = m_ActObj.GetComponent<ItemData>();
    //        if (m_ItemData != null)
    //        {
    //            item m_ItemConf = ManageMentClass.DataManagerClass.GetItemTableFun(item.Value.item_id);
    //            MallServerData mall = null ;
    //            for (int i = 0; i < m_MallServerData.Count; i++)
    //            {
    //                if(m_MallServerData[i].item_id == item.Value.item_id)
    //                {
    //                    mall = m_MallServerData[i];
    //                }
    //            }
    //            m_ItemData.SetItemIcon(m_ItemConf.item_icon);
    //            m_ItemData.SetItemName(m_ItemConf.item_name);
    //            m_ItemData.SetItemData(m_ItemConf);
    //            m_ItemData.SetMallServerData(mall);
    //            m_ItemData.SetItemId(m_ItemConf.item_id);
    //            m_ItemData.SetItemSelectState(false);
    //            m_ItemData.SetItemLockState(mall.has_num <= 0);
    //            m_ItemData.SetItemTabType((int)TableType.RecentUse);
    //            m_UseListItemData[m_ItemConf.item_id] = m_ItemData;
    //        }
    //    }
    //}

    public bool bHaveAct(int itemId)
    {

        foreach(var data in m_MallServerData)
        {
            if(data.Value.item_id == itemId && data.Value.has_num > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsContains(int itemId)
    {
        bool bContains = false;
        foreach(var item in m_MallServerData)
        {
            if(item.Value.item_id == itemId)
            {
                bContains = true;
                break;
            }
        }
        return bContains;
    }
    //public bool IsUseListContains(int itemId)
    //{
    //    bool bContains = false;
    //    foreach (var item in m_UsedActData)
    //    {
    //        if (item.Value.item_id == itemId)
    //        {
    //            bContains = true;
    //            break;
    //        }
    //    }
    //    return bContains;
    //}
}
