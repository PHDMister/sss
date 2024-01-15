//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class UITreasurePartnerLeaveTip : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Button button_buttonzhu; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		button_buttonzhu = FindComp<Button>("dilog-duiyoulixian/buttonzhu"); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_buttonzhu, OnbuttonzhuClicked);
	}	
    // UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void OnbuttonzhuClicked(GameObject go)
	{
		CloseUIForm();
	}
	// UI EVENT FUNC END

	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.Normal;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

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

}
