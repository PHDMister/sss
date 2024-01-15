//<Tools\GenUICode>工具生成, UI变化重新生成

using System;using UnityEngine;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachShopPanel : BaseUIForm
{
    // UI VARIABLE STATEMENT START
    private Toggle toggle_tab1;
    private Toggle toggle_tab2;
    private Toggle toggle_tab3;
    //private Text text_txtDes;
    private ScrollRect scrollrect_Scroll_View;
    private Button button_BtnClose;

    private Text tab1_button_txt;
    private Text tab2_button_txt;
    private Text tab3_button_txt;

    private Transform pageLayOut;
    private Transform pageItem;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        toggle_tab1 = FindComp<Toggle>("center/Group 1000008350/Render/tab1");
        toggle_tab2 = FindComp<Toggle>("center/Group 1000008350/Render/tab2");
        toggle_tab3 = FindComp<Toggle>("center/Group 1000008350/Render/tab3");
        //text_txtDes = FindComp<Text>("center/Group 1000008350/txtDes");
        scrollrect_Scroll_View = FindComp<ScrollRect>("center/Group 1000008350/Scroll View");
        button_BtnClose = FindComp<Button>("center/BtnClose");

        tab1_button_txt = FindComp<Text>("center/Group 1000008350/Render/tab1/button-txt");
        tab2_button_txt = FindComp<Text>("center/Group 1000008350/Render/tab2/button-txt");
        tab3_button_txt = FindComp<Text>("center/Group 1000008350/Render/tab3/button-txt");
        
        pageLayOut = FindComp<Transform>("center/Group 1000008350/pageLayOut");
        pageItem = FindComp<Transform>("center/Group 1000008350/pageLayOut/pagItem");
        pageItem.gameObject.SetActive(false);
        
        OnAwake();
        AddEvent();
    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        toggle_tab1.onValueChanged.AddListener(Ontab1ValueChanged);
        toggle_tab2.onValueChanged.AddListener(Ontab2ValueChanged);
        toggle_tab3.onValueChanged.AddListener(Ontab3ValueChanged);
        scrollrect_Scroll_View.onValueChanged.AddListener(OnScroll_ViewValueChanged);
        RigisterCompEvent(button_BtnClose, OnBtnCloseClicked);
        //RigisterCompEvent(toggle_tab3, OnToggleTab3Clicked);
        
        ReceiveMessage("OnRainbowBeachShopPanelShow", (p) =>
        {
           var param = p.Values as RainbowBeachShopPanelParam;
           var index = _pageList.FindIndex(x=>x.Equals((int)param.tab));
           if (index < 0)
           {
               return;
           }

           var toggle = pageLayOut.transform.GetChild(index + 1).GetComponent<Toggle>();
           toggle.isOn = true;
        });
    }


    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Ontab1ValueChanged(bool arg)
    {
        if (arg)
        {
            //第一页数据
            _tempList = Singleton<ShopMgr>.Instance.GetShop(1);
            resetPageTab();
            tab1_button_txt.text = "<color=#000000>彩虹沙滩</color>";
        }
        else
        {
            tab1_button_txt.text = "<color=#ffffff>彩虹沙滩</color>";
        }
    }

    private void resetPageTab()
    {
        _pageList.Clear();
        _dic.Clear();
        foreach (var id in _tempList)
        {
            var item = Singleton<BagMgr>.Instance.GetItem(id);
            if (!_pageList.Contains(item.Type))
            {
                _pageList.Add(item.Type);
            }
            
            _dic.TryGetValue(item.Type, out var list);
            if (list == null)
            {
                list = new List<int>();
                _dic.Add(item.Type, list);
            }
            list.Add(id);
        }

        while (pageLayOut.childCount > 1)
        {
            var child = pageLayOut.GetChild(1);
            child.SetParent(null);
            GameObject.Destroy(child.gameObject);
        }
        
        for (int i = 0; i < _pageList.Count; i++)
        {
            var type = _pageList[i];
            var item = GameObject.Instantiate(pageItem, pageLayOut, true);
            item.gameObject.SetActive(true);
            item.Find("txtDes").gameObject.GetComponent<Text>().text = _typeDic[type];
            var toggle = item.GetComponent<Toggle>();
            toggle.isOn = false;
            
            if (i == 0)
            {
                toggle.isOn = true;
                onToggleClick(true, type);
            }
            
            toggle.group = pageLayOut.GetComponent<ToggleGroup>();
            
            var t = type;
            toggle.onValueChanged.AddListener((select) =>
            {
                onToggleClick(select, t);
            });


        }
    }

    private void onToggleClick(bool select, int type)
    {
        if (!select)
        {
            return;
        }

        _list = _dic[type];
        
        int count1 = _list.Count / RowCount;
        if (_list.Count % RowCount > 0)
        {
            count1++;
        }
        _view.SetListItemCount(count1,false);
        _view.RefreshAllShownItem();
    }

    private void Ontab2ValueChanged(bool arg)
    {
        if (arg)
        {
            //第二页数据
            _tempList = Singleton<ShopMgr>.Instance.GetShop(2);
            resetPageTab();
            tab2_button_txt.text = "<color=#000000>神秘海湾</color>";
        }
        else
        {
            tab2_button_txt.text = "<color=#ffffff>神秘海湾</color>";
        }
    }

    private void Ontab3ValueChanged(bool arg)
    {
        if (arg)
        {
            //第二页数据
            _tempList = Singleton<ShopMgr>.Instance.GetShop(3);
            resetPageTab();

            tab3_button_txt.text = "<color=#000000>海底星空</color>";
        }
        else
        {
            tab3_button_txt.text = "<color=#ffffff>海底星空</color>";
        }
    }

    private void OnScroll_ViewValueChanged(Vector2 arg)
    {
    }

    private void OnBtnCloseClicked(GameObject go)
    {
        CloseUIForm();
    }

    //private void OnToggleTab3Clicked(GameObject go)
    //{
    //    ToastManager.Instance.ShowNewToast("敬请期待~", 2);
    //} 
    // UI EVENT FUNC END
    //商店数据
    //private List<int> _list;
    //商店item页签
    private Dictionary<int, string> _typeDic;
    private List<int> _pageList;
    
    private List<int> _tempList;
    private List<int> _list;
    private Dictionary<int, List<int>> _dic;

    private const int RowCount = 3;
    private LoopListView2 _view;
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

        _typeDic = new Dictionary<int, string>()
        {
            { 1, "铲子" },
            { 3, "鱼竿" },
            { 4, "潜水装备" },
            { 5, "钥匙" },
        };

        _view = scrollrect_Scroll_View.GetComponent<LoopListView2>();
        _view.InitListView(0, OnGetItemByIndex);

        _dic = new Dictionary<int, List<int>>();
        _tempList = new List<int>();
        _pageList = new List<int>();
        _list = new List<int>();

        //toggle_tab3.interactable = false;
    }

    private const int rowCount = 3;

    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0)
        {
            return null;
        }
        LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
        //update all items in the row
        for (int i = 0; i < rowCount; ++i)
        {
            int itemIndex = index * rowCount + i;
            var uiItem = item.transform.GetChild(i).GetComponent<UIShopItem>();
            if (itemIndex >= _list.Count)
            {
                uiItem.gameObject.SetActive(false);
                continue;
            }
            var data = _list[itemIndex];
            //update the subitem content.
            if (data != null)
            {
                uiItem.gameObject.SetActive(true);
                uiItem.SetData(data);
            }
            else
            {
                uiItem.gameObject.SetActive(false);
            }
        }
        return item;
    }

    public override void Display()
    {
        base.Display();
        var sceneID = ManageMentClass.DataManagerClass.SceneID;
        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            if (toggle_tab1.isOn)
            {
                toggle_tab1.SetIsOnWithoutNotify(true);
                Ontab1ValueChanged(true);
            }
            else
            {
                toggle_tab1.isOn = true;
            }
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan) {
            if (toggle_tab2.isOn)
            {
                toggle_tab2.SetIsOnWithoutNotify(true);
                Ontab2ValueChanged(true);
            }
            else
            {
                toggle_tab2.isOn = true;
            }
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            if (toggle_tab3.isOn)
            {
                toggle_tab3.SetIsOnWithoutNotify(true);
                Ontab3ValueChanged(true);
            }
            else
            {
                toggle_tab3.isOn = true;
            }
        }
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Freeze()
    {
        base.Freeze();
    }
}

public class RainbowBeachShopPanelParam
{
    public ShopTab tab;
}

public enum ShopTab{
    chanzi = 1,
    yugan = 3,
    qianshuizhuangbei = 4,
    yaoshi = 5,
}