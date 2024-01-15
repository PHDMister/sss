using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


public class AppearanceEditorUIForm : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public CircularScrollView.UICircularScrollView m_VerticalScrollSuit;

    public RectTransform m_ScrollViewBg;
    public Toggle m_Toggle_Head;
    public Toggle m_Toggle_Appearance;
    public Toggle m_Toggle_Suit;
    public Toggle m_Toggle_Color;

    public Text m_ToggleHeadTxt;
    public Text m_ToggleAppearanceTxt;
    public Text m_ToggleSuitTxt;
    public Text m_ToggleColorTxt;
    public Button m_Button_Save;
    public Transform m_Trans_HeadSecond;
    public Transform m_Trans_AppearanceSecond;
    public Transform m_Trans_Empty;
    public Transform m_Trans_NotNetwork;
    //头部
    //头发
    public Toggle m_Toggle_Hair;
    //脸型
    public Toggle m_Toggle_Face;
    //眼镜
    public Toggle m_Toggle_Glasses;
    //项链
    public Toggle m_Toggle_Necklace;
    //耳饰
    public Toggle m_Toggle_Earring;
    //眼睛
    public Toggle m_Toggle_Eye;
    //眉毛
    public Toggle m_Toggle_Eyebrow;
    //鼻子
    public Toggle m_Toggle_Nose;
    //嘴巴
    public Toggle m_Toggle_Mouth;
    //耳朵
    public Toggle m_Toggle_Ear;

    //外观
    public Toggle m_Toggle_Coat;
    public Toggle m_Toggle_Underwear;
    public Toggle m_Toggle_Shoe;
    public Toggle m_Toggle_Bag;
    public Toggle m_Toggle_Watch;
    public Toggle m_Toggle_Rings;

    //服装数据
    List<OutFitRecData> outFitData = new List<OutFitRecData>();
    List<ThreeLevelData> threeLevelData = new List<ThreeLevelData>();
    Dictionary<int, ClothingItem> dicClothItems = new Dictionary<int, ClothingItem>();
    //当前穿戴数据
    public ThreeLevelData mCurTabTypeClothingData = null;
    Dictionary<int, int> dicOriginClothingData = new Dictionary<int, int>();
    Dictionary<int, int> dicOptClothingData = new Dictionary<int, int>();


    public bool IsGoShop = false;

    public enum TableType
    {
        Suit = 1,
        Head = 2,
        Appearance = 3,
        Color = 4,
        Heent = 5,
    }

    public enum HeadTableType
    {
        Hair = 3,//头发
        Glasses = 4,//眼镜
        Earring = 5,//耳饰
        Necklace = 6,//项链 
        Eyebrow = 11,//眉毛
        Face = 13,//脸型
        Eye = 14,//眼睛
        Mouth = 15,//嘴巴
        Nose = 16,//鼻子
        Ear = 17,//耳朵
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
    public enum ColorTableType
    {
        Color = 12,// 肤色
    }

    private TableType m_TabType;
    private HeadTableType m_HeadTabType;
    private AppearanceTableType m_AppearanceTableType;
    private ColorTableType m_ColorTableType;



    private bool IsCanClick = false;
    private float nowTime = 0f;
    private float maxTime = 1f;


    //[DllImport("wininet")]
    //private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

    void Awake()
    {
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        m_Trans_Empty.gameObject.SetActive(false);
        m_Trans_NotNetwork.gameObject.SetActive(false);

        ToggleAddListener();

        //注册按钮事件
        RigisterButtonObjectEvent("BtnClose", p =>
         {
             if (!IsChanged())
             {
                 CloseUIForm();
             }
             else
             {
                 OpenUIForm(FormConst.EDITORAPPEARANCETIPSUIFORM);
             }
         });

        RigisterButtonObjectEvent("BtnSave", p =>
        {
            if (!IsChanged())
            {
                return;
            }
            if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.parlorScene)
            {
                //如果当前是在客厅 换装则取消建筑交互
                Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative();
            }


            AvatarManager.Instance().SaveOutfitAvatarIDFun();
            //MyOutFitSaveReqData myOutFitSaveReqData = new MyOutFitSaveReqData();
            //myOutFitSaveReqData.data = new List<MyOutFitRecData>();
            //foreach (var _data in dicOptClothingData)
            //{
            //    MyOutFitRecData myOutFitRecData = new MyOutFitRecData();
            //    myOutFitRecData.avatar_id = _data.Value;
            //    myOutFitSaveReqData.data.Add(myOutFitRecData);
            //}
            MyOutFitSaveReqData myOutFitSaveReqData = AvatarManager.Instance().myOutFitAvatarIdData;
            MessageManager.GetInstance().RequestMyOutFitSave(myOutFitSaveReqData, () =>
             {
                 ChangeSkinReq req = new ChangeSkinReq();
                 req.UserId = ManageMentClass.DataManagerClass.userId;
                 foreach (var item in AvatarManager.Instance().myOutFitAvatarIdData.data)
                 {
                     req.AvatarIds.Add(item.avatar_id);
                 }
                 req.Index = WebSocketAgent.Ins.NetView.GetCode;

                 WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ChangeSkinReq, req, (code, data) =>
                 {
                     Debug.Log("Code的值： " + code);
                 });

                 ToastManager.Instance.ShowNewToast("保存成功", 2f);
                 MessageManager.GetInstance().RequestOutFitList((p) =>
                 {
                     SendMessage("ReceiveOutFitData", "Success", p);
                 });
             });
        });

        RigisterButtonObjectEvent("BtnShop", p =>
        {
            IsGoShop = true;
            CloseUIForm();
            OpenUIForm(FormConst.SHOPNEWUIFORM);
        });

        RigisterButtonObjectEvent("BtnRefresh", p =>
        {
            MessageManager.GetInstance().RequestOutFitList((p) =>
            {
                SendMessage("ReceiveOutFitData", "Success", p);
            });
        });

        RigisterButtonObjectEvent("EditoPersonDataButton", p =>
        {
            if (IsCanClick)
            {
                Debug.Log("点击这里来了");
                CloseUIForm();
                OpenUIForm(FormConst.PERSONALDATAPANEL);
                SendMessage("OpenPersonDataPanelRefreshUI", "Success", (ulong)0);
            }
        });

        ReceiveMessage("ReceiveOutFitData", p =>
        {
            outFitData = p.Values as List<OutFitRecData>;
            if (outFitData == null)
                return;
            SetClothingData();
            SetSaveBtnState();
            ShowOutFitList();
        });
    }

    private void ToggleAddListener()
    {
        m_Toggle_Head.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_HeadTabType = HeadTableType.Hair;
                SetHeadToggleGroup();

                m_TabType = TableType.Head;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);
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
                ShowOutFitList();
            }
        });

        m_Toggle_Suit.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Suit;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);
                ShowOutFitList();
            }
        });
        m_Toggle_Color.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Color;
                SetToggleTextColor(m_TabType);
                SetSecondTab(m_TabType);
                ShowOutFitList();
            }
        });

        //头部
        m_Toggle_Hair.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Head;
                m_HeadTabType = HeadTableType.Hair;
                ShowOutFitList();
            }
        });


        m_Toggle_Face.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Face;
                ShowOutFitList();
            }
        });

        m_Toggle_Glasses.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Head;
                m_HeadTabType = HeadTableType.Glasses;
                ShowOutFitList();
            }
        });

        m_Toggle_Earring.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Earring;
                ShowOutFitList();
            }
        });

        m_Toggle_Necklace.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Head;
                m_HeadTabType = HeadTableType.Necklace;
                ShowOutFitList();
            }
        });

        m_Toggle_Eye.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Eye;
                ShowOutFitList();
            }
        });

        m_Toggle_Eyebrow.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Head;
                m_HeadTabType = HeadTableType.Eyebrow;
                ShowOutFitList();
            }
        });

        m_Toggle_Nose.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Nose;
                ShowOutFitList();
            }
        });

        m_Toggle_Mouth.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Mouth;
                ShowOutFitList();
            }
        });

        m_Toggle_Ear.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_TabType = TableType.Heent;
                m_HeadTabType = HeadTableType.Ear;
                ShowOutFitList();
            }
        });

        //外观
        m_Toggle_Coat.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Coat;
                ShowOutFitList();
            }
        });

        m_Toggle_Underwear.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Underwear;
                ShowOutFitList();
            }
        });

        m_Toggle_Shoe.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Shoe;
                ShowOutFitList();
            }
        });

        m_Toggle_Bag.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Bag;
                ShowOutFitList();
            }
        });

        m_Toggle_Watch.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Watch;
                ShowOutFitList();
            }
        });

        m_Toggle_Rings.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_AppearanceTableType = AppearanceTableType.Rings;
                ShowOutFitList();
            }
        });
    }
    private void SetClothingData()
    {
        dicOriginClothingData.Clear();
        dicOptClothingData.Clear();
        foreach (var data in outFitData)
        {
            foreach (var secondData in data.second_data)
            {
                foreach (var threeData in secondData.list)
                {
                    if (threeData.status == 1)
                    {
                        dicOriginClothingData[threeData.avatar_type2] = threeData.avatar_id;
                        dicOptClothingData[threeData.avatar_type2] = threeData.avatar_id;
                    }
                }
            }
        }
    }

    public void SetOptClothingData(int avatar_type2, int avatar_id)
    {
        dicOptClothingData[avatar_type2] = avatar_id;
    }

    private void ShowOutFitList()
    {

        threeLevelData = GetSecondTabTypeOutFitData(m_TabType);
        Debug.Log("在showoutfitlist 方法中 获取tabtype 的值：   " + JsonConvert.SerializeObject(threeLevelData) + "   M_TabType: " + m_TabType);
        SortOutFitData();
        GetCurDressData();
        Debug.Log(threeLevelData.Count);

        if (m_TabType == TableType.Suit || m_TabType == TableType.Color)
        {
            m_VerticalScroll.gameObject.SetActive(false);
            m_VerticalScrollSuit.gameObject.SetActive(true);
            m_VerticalScrollSuit.Init(InitItemInfoCallBack);
            m_VerticalScrollSuit.ShowList(threeLevelData.Count);
            m_VerticalScrollSuit.ResetScrollRect();
        }
        else
        {
            m_VerticalScroll.gameObject.SetActive(true);
            m_VerticalScrollSuit.gameObject.SetActive(false);

            m_VerticalScroll.Init(InitItemInfoCallBack);
            m_VerticalScroll.ShowList(threeLevelData.Count);
            m_VerticalScroll.ResetScrollRect();
        }

        m_Trans_Empty.gameObject.SetActive(threeLevelData.Count <= 0);
    }

    private void SortOutFitData()
    {
        SortHelper.Sort(threeLevelData.ToArray(), (a, b) =>
        {
            avatar avatarConfigA = ManageMentClass.DataManagerClass.GetAvatarTableFun(a.avatar_id);
            avatar avatarConfigB = ManageMentClass.DataManagerClass.GetAvatarTableFun(b.avatar_id);

            if (avatarConfigA != null && avatarConfigB != null)
            {
                int qualityA = avatarConfigA.avatar_rare;
                int qualityB = avatarConfigB.avatar_rare;

                return qualityA < qualityB;//品质降序
            }
            return false;
        });
    }
    /// <summary>
    /// 获取的是二级数据
    /// </summary>
    /// <param name="tabType"></param>
    /// <returns></returns>
    public List<SecondLevelData> GetOutFitDataByTableType(TableType tabType)
    {
        if (outFitData == null)
            return null;
        List<SecondLevelData> filterData = new List<SecondLevelData>();

        Debug.Log("输出一下获取道德OutFitData 的内容： " + JsonConvert.SerializeObject(outFitData));

        foreach (var item in outFitData)
        {
            if (item.avatar_type1 == (int)tabType)
            {
                foreach (var data in item.second_data)
                {
                    filterData.Add(data);
                }
            }
        }
        Debug.Log("输出一下获取道德OutFitData 的内容： ----------   ： " + (int)tabType + "筛选后的值： " + JsonConvert.SerializeObject(filterData));
        return filterData;
    }

    /// <summary>
    /// 当前二级页签数据
    /// </summary>
    /// <param name="tabType"></param>
    /// <returns></returns>
    public List<ThreeLevelData> GetSecondTabTypeOutFitData(TableType tabType)
    {
        List<SecondLevelData> filterData = GetOutFitDataByTableType(tabType);
        List<ThreeLevelData> data = new List<ThreeLevelData>();

        Debug.Log("第二层数据内容：   " + JsonConvert.SerializeObject(filterData) + "   M_TabType: " + m_TabType + "  m_HeadTabType:  " + m_HeadTabType);

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
            case TableType.Color:
                data = GetOutFitDataByColorType(filterData);
                break;
            case TableType.Heent:
                data = GetOutFitDataByHeadType(filterData, m_HeadTabType);
                break;
        }
        return data;
    }

    private List<ThreeLevelData> GetOutFitDataByHeadType(List<SecondLevelData> filterData, HeadTableType headTableType)
    {
        List<ThreeLevelData> headTabData = new List<ThreeLevelData>();
        foreach (var item in filterData)
        {
            foreach (var data in item.list)
            {
                if (data.avatar_type2 == (int)headTableType)
                {
                    headTabData.Add(data);
                }
            }
        }
        return headTabData;
    }
    private List<ThreeLevelData> GetOutFitDataByAppearanceType(List<SecondLevelData> filterData, AppearanceTableType appearanceTableType)
    {
        List<ThreeLevelData> appearanceTabData = new List<ThreeLevelData>();
        foreach (var item in filterData)
        {
            foreach (var data in item.list)
            {
                if (data.avatar_type2 == (int)appearanceTableType)
                {
                    appearanceTabData.Add(data);
                }
            }
        }
        return appearanceTabData;
    }

    private List<ThreeLevelData> GetOutFitDataBySuitType(List<SecondLevelData> filterData)
    {
        List<ThreeLevelData> suitTabData = new List<ThreeLevelData>();
        foreach (var item in filterData)
        {
            foreach (var data in item.list)
            {
                if (data.avatar_type2 == (int)AppearanceTableType.HotMan || data.avatar_type2 == (int)AppearanceTableType.Suit)
                {
                    suitTabData.Add(data);
                }

            }
        }
        return suitTabData;
    }
    private List<ThreeLevelData> GetOutFitDataByColorType(List<SecondLevelData> filterData)
    {
        List<ThreeLevelData> suitTabData = new List<ThreeLevelData>();
        foreach (var item in filterData)
        {
            foreach (var data in item.list)
            {
                if (data.avatar_type2 == (int)ColorTableType.Color)
                {
                    suitTabData.Add(data);
                }
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

        ClothingItem clothingItem = cell.transform.GetComponent<ClothingItem>();
        if (clothingItem != null)
        {
            ThreeLevelData data = threeLevelData[index - 1];
            if (data == null)
                return;

            Debug.Log("  ThreeLevelData的数据:  " + JsonConvert.SerializeObject(data));
            clothingItem.SetItemIcon(data.avatar_id);
            clothingItem.SetItemName(data.avatar_name);
            clothingItem.SetItemHaveNum(data.has_num);
            clothingItem.SetItemSelectState(mCurTabTypeClothingData != null && mCurTabTypeClothingData.avatar_id == data.avatar_id);
            clothingItem.SetItemUsingState(data.status == 1);
            clothingItem.SetItemQuality(data.avatar_rare);
            clothingItem.SetAvatarData(data);
            if (!dicClothItems.ContainsKey(data.avatar_id))
            {
                dicClothItems[data.avatar_id] = clothingItem;
            }
        }
    }

    public override void Display()
    {
        base.Display();
        IsCanClick = false;
        nowTime = 0f;
        m_TabType = TableType.Appearance;
        m_AppearanceTableType = AppearanceTableType.Coat;
        m_HeadTabType = HeadTableType.Hair;

        InterfaceHelper.SetJoyStickState(false);
        RTManager.GetInstance().LoadCharacter();

        SetToggleGroup();
        SetAppearanceToggleGroup();
        SetToggleTextColor(m_TabType);
        SetSecondTab(m_TabType);
        CheckNetwork();
    }

    public void SetSaveBtnState()
    {
        if (m_Button_Save != null)
        {
            m_Button_Save.interactable = IsChanged();
        }
    }

    public void SetToggleGroup()
    {
        m_Toggle_Appearance.isOn = true;
        m_Toggle_Head.isOn = false;
        m_Toggle_Suit.isOn = false;
        m_Toggle_Color.isOn = false;
    }

    public void SetHeadToggleGroup()
    {
        m_Toggle_Hair.isOn = true;
        m_Toggle_Glasses.isOn = false;
        m_Toggle_Earring.isOn = false;
        m_Toggle_Necklace.isOn = false;
        //脸型
        m_Toggle_Face.isOn = false;
        //眼睛
        m_Toggle_Eye.isOn = false;
        //眉毛
        m_Toggle_Eyebrow.isOn = false;
        //鼻子
        m_Toggle_Nose.isOn = false;
        //嘴巴
        m_Toggle_Mouth.isOn = false;
        //耳朵
        m_Toggle_Ear.isOn = false;
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

    private void SetToggleTextColor(TableType tabType)
    {
        switch (tabType)
        {
            case TableType.Head:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleColorTxt != null)
                    m_ToggleColorTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case TableType.Appearance:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleColorTxt != null)
                    m_ToggleColorTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case TableType.Suit:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
                if (m_ToggleColorTxt != null)
                    m_ToggleColorTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                break;
            case TableType.Color:
                if (m_ToggleHeadTxt != null)
                    m_ToggleHeadTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleAppearanceTxt != null)
                    m_ToggleAppearanceTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleSuitTxt != null)
                    m_ToggleSuitTxt.color = new Color(228f / 255f, 255f / 255f, 254f / 255f);
                if (m_ToggleColorTxt != null)
                    m_ToggleColorTxt.color = new Color(51f / 255f, 51f / 255f, 51f / 255f);
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
            case TableType.Color:
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
        m_TabType = TableType.Appearance;
        m_AppearanceTableType = AppearanceTableType.Coat;
        m_HeadTabType = HeadTableType.Hair;
        SetToggleGroup();
        AvatarManager.Instance().RefreshPlayerFun();
        RTManager.GetInstance().ResetCharacter(IsGoShop);
        RTManager.GetInstance().DestroyCharacter();

        if (IsGoShop)
        {
            IsGoShop = false;
            base.Hiding();
        }
        else
        {
            StartCoroutine(DeleTimeOpen());
        }

    }
    IEnumerator DeleTimeOpen()
    {
        yield return null;
        InterfaceHelper.SetJoyStickState(true);
        base.Hiding();

    }

    public void GetCurDressData()
    {
        foreach (var data in threeLevelData)
        {
            if (data.avatar_type1 == (int)m_TabType)
            {
                if (m_TabType == TableType.Head)
                {
                    if ((int)m_HeadTabType == data.avatar_type2 && data.status == 1)
                    {
                        mCurTabTypeClothingData = data;
                        break;
                    }
                }
                if (m_TabType == TableType.Appearance)
                {
                    if ((int)m_AppearanceTableType == data.avatar_type2 && data.status == 1)
                    {
                        mCurTabTypeClothingData = data;
                        break;
                    }
                }
                if (m_TabType == TableType.Suit)
                {
                    if (data.status == 1)
                    {
                        mCurTabTypeClothingData = data;
                        break;
                    }
                }
                if (m_TabType == TableType.Color)
                {
                    if (data.status == 1)
                    {
                        mCurTabTypeClothingData = data;
                        break;
                    }
                }
                if (m_TabType == TableType.Heent)
                {
                    if (data.status == 1&& (int)m_HeadTabType == data.avatar_type2)
                    {
                        mCurTabTypeClothingData = data;
                        break;
                    }
                }
            }
        }
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

    public bool IsChanged()
    {
        foreach (var data in dicOriginClothingData)
        {
            if (dicOptClothingData.ContainsKey(data.Key))
            {
                if (dicOptClothingData[data.Key] != data.Value)
                {
                    return true;
                }
            }
        }

        if (dicOptClothingData.Count > dicOriginClothingData.Count)
        {
            foreach (var data in dicOptClothingData)
            {
                if (!dicOriginClothingData.ContainsKey(data.Key))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_Trans_NotNetwork.gameObject.SetActive(true);
        }
        else
        {
            m_Trans_NotNetwork.gameObject.SetActive(false);
        }
        //m_Trans_NotNetwork.gameObject.SetActive(!IsConnectedInternet());
    }

    private void Update()
    {
        if (!IsCanClick)
        {
            if (nowTime < maxTime)
            {
                nowTime += Time.deltaTime;
            }
            else
            {
                IsCanClick = true;
            }
        }
    }

    //public bool IsConnectedInternet()
    //{
    //    int i = 0;
    //    if (InternetGetConnectedState(out i, 0))
    //        return true;
    //    else
    //        return false;
    //}
}
