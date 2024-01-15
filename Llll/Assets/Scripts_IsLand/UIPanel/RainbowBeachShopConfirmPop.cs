//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachShopConfirmPop : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Text text_txtContent; 
	private Image btnCancel; 
	private Button btnSure; 
	private Button button_btnClose; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		text_txtContent = FindComp<Text>("center/txtContent"); 
		btnCancel = FindComp<Image>("center/btnCancel"); 
		btnSure = FindComp<Button>("center/btnSure"); 
		button_btnClose = FindComp<Button>("center/btnClose"); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(btnSure, OnbtnSureClicked);
		RigisterCompEvent(btnCancel, OnbtnCancelClicked);
		RigisterCompEvent(button_btnClose, OnbtnCloseClicked);
	}	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void OnbtnCancelClicked(GameObject go)
	{
		CloseUIForm();
	}
	private void OnbtnSureClicked(GameObject go)
	{
		MessageManager.GetInstance().RequestShopBuy(_itemId, () =>
		{
			ToastManager.Instance.ShowToast("购买成功");
			CloseUIForm();
		});
	}
	private void OnbtnCloseClicked(GameObject go)
	{
		CloseUIForm();
	}
	// UI EVENT FUNC END

	private int _itemId;
	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
		
		ReceiveMessage("OnRainbowBeachShopConfirmPopOpen", (p) =>
		{
			var param = p.Values as RainbowBeachShopConfirmPopParam;
			SetData(param);
		});
	}

	public override void Display() 
	{
		base.Display();
	}

	private void SetData(RainbowBeachShopConfirmPopParam param)
	{
		_itemId = param.itemId;
		var bagItem = Singleton<BagMgr>.Instance.GetItem(_itemId);
		var costName = "贝壳";
		var costNum = bagItem.Price;
		var goodsName = bagItem.Name;
		var buyNum = 1;
		text_txtContent.text = $"是否消耗【{costName}】x{costNum}购买【<color=#F9EF65>{goodsName}</color>】x{buyNum}";
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

public class RainbowBeachShopConfirmPopParam
{
	public int itemId;
}