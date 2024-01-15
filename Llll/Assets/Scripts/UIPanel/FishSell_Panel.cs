//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;
using UnityEngineInternal;

public class FishSell_Panel : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Button button_BtnAdd; 
	private Button button_BtnDel; 
	private Button button_BtnMax;
	private Button button_BtnCancel;
	private Button button_BtnSell;
	private Text text_TxtNumber;
	// UI VARIABLE STATEMENT END

	private int sellFishId = 0;
	private int curSellNumber = 0;
	private int minCnt = 0;
	private int maxCnt = 0;

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		button_BtnAdd = FindComp<Button>("ImgBg/SellSelect/BtnAdd"); 
		button_BtnDel = FindComp<Button>("ImgBg/SellSelect/BtnDel"); 
		button_BtnMax = FindComp<Button>("ImgBg/SellSelect/BtnMax"); 
		button_BtnCancel = FindComp<Button>("ImgBg/BtnCancel"); 
		button_BtnSell = FindComp<Button>("ImgBg/BtnSell");
		text_TxtNumber = FindComp<Text>("ImgBg/SellSelect/Number/TxtNumber");
		OnAwake(); 
		AddEvent(); 
	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_BtnAdd, OnBtnAddClicked);
		RigisterCompEvent(button_BtnDel, OnBtnDelClicked);
		RigisterCompEvent(button_BtnMax, OnBtnMaxClicked);
		RigisterCompEvent(button_BtnCancel, OnBtnCancelClicked);
		RigisterCompEvent(button_BtnSell, OnBtnSellClicked);
		ReceiveMessage("FishSellMsg",FishSellMsgHandler);
	}
	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void OnBtnAddClicked(GameObject go)
	{
		SetNumber(curSellNumber + 1);
	}
	private void OnBtnDelClicked(GameObject go)
	{
		SetNumber(curSellNumber - 1);
	}
	private void OnBtnMaxClicked(GameObject go)
	{
		if ( curSellNumber >= maxCnt )
		{
			ToastManager.Instance.ShowNewToast("当前已为最大出售数量~", 2);
		}
		else
		{
			SetNumber(maxCnt);
		}
	}
	private void OnBtnCancelClicked(GameObject go)
	{
		CloseUIForm();
	}
	private void OnBtnSellClicked(GameObject go)
	{
		if ( curSellNumber > 0 && sellFishId > 0 )
		{
			MessageManager.GetInstance().SendSellReq((uint)sellFishId,(uint)curSellNumber);
		}
		else
		{
			ToastManager.Instance.ShowNewToast("出售操作异常~", 2);
		}
	}
	void FishSellMsgHandler(KeyValuesUpdate kv)
	{
		if( kv.Key == "Success" )
		{
			CloseUIForm();
			//出售成功提示
			OpenUIForm(FormConst.FISHSUCCESSPANEL);
			//提示内容
			FishSellSuccess_Panel uiForm = UIManager.GetInstance().GetUIForm(FormConst.FISHSUCCESSPANEL) as FishSellSuccess_Panel;
			if (uiForm != null){
				var rsp = kv.Values as SellResp;
				uiForm.ShowTipsContent(sellFishId, rsp.SellCount,rsp.Income);
			}
		}else if ( kv.Key == "Fail" )
		{
			CloseUIForm();
			ToastManager.Instance.ShowNewToast("出售失败~", 2);
		}
	}
	
	// UI EVENT FUNC END

	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence;
	}

	public override void Display() 
	{
		base.Display();
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

	void SetNumber(int number)
	{
		number = Mathf.Clamp(number,minCnt,maxCnt);
		curSellNumber = number;
		text_TxtNumber.text = number.ToString();
	}
	
	void SetSellFishId(int fishId)
	{
		sellFishId = fishId;
		var canSellCnt = Singleton<AquariumDataModel>.Instance.GetFishCount(fishId) - 1;
		maxCnt = canSellCnt > 0 ? canSellCnt : 0;
		minCnt = maxCnt > 0 ? 1 : 0 ;
		SetNumber(maxCnt);
	}

	public static void ShowFishSell(int fishId)
	{
		//出售界面
		UIManager.GetInstance().ShowUIForms(FormConst.FISHSELLPANEL);
		//设置出售鱼的id
		FishSell_Panel uiForm = UIManager.GetInstance().GetUIForm(FormConst.FISHSELLPANEL) as FishSell_Panel;
		if (uiForm != null){
			uiForm.SetSellFishId(fishId);
		}
	}

}
