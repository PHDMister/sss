//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UIFW;

public class FishClub_Panel : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Button button_Return_Btn; 
	private Button button_Introduce_Btn; 
	private Text text_SheelValue_Text; 
	private Toggle toggle_SmallFishToggle; 
	private Toggle toggle_MiddleFishToggle; 
	private Toggle toggle_BigFishToggle; 
	private Image image_DetailPage_Panel; 
	private Image image_Item_Img; 
	private Image image_CornerMark_Img; 
	private Text text_Count_Text; 
	private Text text_ItemName_Text; 
	private Text text_Price_Text; 
	private Image image_Sell_Btn; 
	private Image image_SmallFish_Panel;
	private Text text_name_Text;
	public CircularScrollView.UICircularScrollView m_SmalFishScroll;
	//private 
	private int selectTabIndex = 0;
	private int selectItemIndex = 0;
	private List<Toggle> tabToggleLst;
	private Color selectedColor = new Color(0, 255, 240);
	private Dictionary<int, List<island_fish>> cfgDic;
	private Material grayMate;
	private Material defaultMate;
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		button_Return_Btn = FindComp<Button>("HeadlinePanel/Return_Btn"); 
		button_Introduce_Btn = FindComp<Button>("HeadlinePanel/Introduce_Btn"); 
		text_SheelValue_Text = FindComp<Text>("ShellValue_Panel/SheelDI_Img/SheelValue_Text"); 
		toggle_SmallFishToggle = FindComp<Toggle>("Tab_Panel/SmallFishToggle"); 
		toggle_MiddleFishToggle = FindComp<Toggle>("Tab_Panel/MiddleFishToggle"); 
		toggle_BigFishToggle = FindComp<Toggle>("Tab_Panel/BigFishToggle"); 
		image_DetailPage_Panel = FindComp<Image>("DetailPage_Panel"); 
		image_Item_Img = FindComp<Image>("DetailPage_Panel/DiKuang/Item_Img"); 
		image_CornerMark_Img = FindComp<Image>("DetailPage_Panel/DiKuang/CornerMark_Img"); 
		text_Count_Text = FindComp<Text>("DetailPage_Panel/DiKuang/CornerMark_Img/Count_Text"); 
		text_ItemName_Text = FindComp<Text>("DetailPage_Panel/NameDi_Img/ItemName_Text"); 
		text_Price_Text = FindComp<Text>("DetailPage_Panel/Price_Panel/Price_Text"); 
		image_Sell_Btn = FindComp<Image>("DetailPage_Panel/Sell_Btn"); 
		image_SmallFish_Panel = FindComp<Image>("SmallFish_Panel"); 
		text_name_Text = FindComp<Text>("SmallFish_Panel/Scroll View/Content/fishItem/name_Text");
		tabToggleLst = new List<Toggle>(3);
		tabToggleLst.Add(toggle_SmallFishToggle);
		tabToggleLst.Add(toggle_MiddleFishToggle);
		tabToggleLst.Add(toggle_BigFishToggle);
		grayMate = new Material(Shader.Find("UI/Gray"));
		defaultMate = new Material(Shader.Find("UI/Default"));
		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_Return_Btn, OnReturn_BtnClicked);
		RigisterCompEvent(button_Introduce_Btn, OnIntroduce_BtnClicked);
		RigisterCompEvent(image_Sell_Btn, OnSell_BtnClicked);
		toggle_SmallFishToggle.onValueChanged.AddListener(OnSmallFishToggleValueChanged);
		toggle_MiddleFishToggle.onValueChanged.AddListener(OnMiddleFishToggleValueChanged);
		toggle_BigFishToggle.onValueChanged.AddListener(OnBigFishToggleValueChanged);
		ReceiveMessage("FishClubItemMsg",FishClubItemMsgHandler);
		ReceiveMessage("AquariumDataMsg",AquariumDataMsgHandler);
		ReceiveMessage("FishSellMsg",FishSellMsgHandler);
	}
	
	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void OnReturn_BtnClicked(GameObject go)
	{
		CloseUIForm();
	}
	private void OnIntroduce_BtnClicked(GameObject go)
	{
		//不需要帮助
	}
	private void OnSell_BtnClicked(GameObject go)
	{
		var curSelectFish = GetCurSelectItemFish();
		var haveCnt = Singleton<AquariumDataModel>.Instance.GetFishCount(curSelectFish.fish_id);
		if( haveCnt > 1 )
		{
			FishSell_Panel.ShowFishSell(curSelectFish.fish_id);
		}
	}
	
	private void OnSmallFishToggleValueChanged(bool arg)
	{
		if (arg)
			SetSelectTab(1);
	}
	private void OnMiddleFishToggleValueChanged(bool arg)
	{
		if (arg)
			SetSelectTab(2);
	}
	private void OnBigFishToggleValueChanged(bool arg)
	{
		if (arg)
			SetSelectTab(3);
	}
	// UI EVENT FUNC END

	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
		m_SmalFishScroll.Init(InitSmallFishItemCallBack);
	}

	public override void Display() 
	{
		base.Display();
		selectTabIndex = 1;
		SetSelectTab(selectTabIndex,true);
		ShowSeashellNumber();
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
	void InitSmallFishItemCallBack(GameObject cell, int index)
	{
		var lst = GetCurTabFishLst();
		var fishCfg = lst[index - 1];
		var haveCnt = Singleton<AquariumDataModel>.Instance.GetFishCount(fishCfg.fish_id);
		FishClubItem item = cell.GetComponent<FishClubItem>();
		item.SetFishInfo(index,fishCfg);
		item.SetSelectState(selectItemIndex == index );
		item.SetIcon(fishCfg.fish_icon, haveCnt < 1 ? grayMate : defaultMate );
	}
	
	void SetSelectTab(int index,bool isRefreshToggle=false)
	{
		selectTabIndex = index;
		if (isRefreshToggle)
		{
			Toggle toggle = tabToggleLst[ index - 1 ];
			toggle.SetIsOnWithoutNotify(true);
		}
		for (int i = 0; i < tabToggleLst.Count; i++)
		{
			Toggle toggle = tabToggleLst[i];
			Text lable = FindComp<Text>(toggle.transform, "Lable");
			lable.color = ( index - 1 ) == i ? selectedColor : Color.white;
		}
		ShowCurTabItemLst();
	}

	void ShowCurTabItemLst()
	{
		SetSelectItemIndex(0);
		var lst = GetCurTabFishLst();
		m_SmalFishScroll.ShowList(lst.Count);
	}

	void FishClubItemMsgHandler(KeyValuesUpdate kv)
	{
		if( kv.Key == "Click" )
		{
			SetSelectItemIndex((int)kv.Values);
			m_SmalFishScroll.UpdateList();
		}
	}
	
	void AquariumDataMsgHandler(KeyValuesUpdate kv)
	{
		if( kv.Key == "Update" )
		{
			m_SmalFishScroll.UpdateList();
			ShowDetailInfo();
		}
	}
	
	void FishSellMsgHandler(KeyValuesUpdate kv)
	{
		if( kv.Key == "Success" )
		{
			ShowSeashellNumber();
		}
	}

	void SetSelectItemIndex(int index)
	{
		selectItemIndex = index;
		ShowDetailInfo();
	}
	
	void ShowDetailInfo()
	{
		island_fish fishCfg = GetCurSelectItemFish();
		image_DetailPage_Panel.gameObject.SetActive( fishCfg !=null );
		if( fishCfg == null )
			return;
		var haveCnt = Singleton<AquariumDataModel>.Instance.GetFishCount(fishCfg.fish_id);
		Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("Fish", fishCfg.fish_icon);
		image_Item_Img.sprite = sprite;
		image_Item_Img.material = haveCnt > 0 ? defaultMate : grayMate;
		text_Count_Text.text = haveCnt.ToString();
		text_Count_Text.transform.parent.gameObject.SetActive( haveCnt > 1 );
		text_ItemName_Text.text = fishCfg.fish_name;
		image_Sell_Btn.material = haveCnt > 1 ? defaultMate : grayMate;
		text_Price_Text.text = string.Format("{0}/条",fishCfg.fish_price);
	}
	
	void ShowSeashellNumber()
	{
		text_SheelValue_Text.text = Singleton<BagMgr>.Instance.ShellNum.ToString();
	}
	
	List<island_fish> GetCurTabFishLst()
	{
		if (cfgDic == null)
			cfgDic = new Dictionary<int,List<island_fish>>();
		List<island_fish> results;
		int fishType = selectTabIndex;
		cfgDic.TryGetValue(fishType, out results);
		if ( results == null )
		{
			results = ManageMentClass.DataManagerClass.GetIslandFishLst(fishType);
			cfgDic.Add(fishType,results);
		}
		return results != null ? results : new List<island_fish>();
	}
	
	island_fish GetCurSelectItemFish()
	{
		var lst = GetCurTabFishLst();
		island_fish result_cfg = null;
		if ( selectItemIndex > 0 && lst.Count >= selectItemIndex )
			result_cfg = lst[selectItemIndex - 1];
		return result_cfg;
	}
	
}
