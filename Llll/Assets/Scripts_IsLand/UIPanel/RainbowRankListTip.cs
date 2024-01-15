//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class RainbowRankListTip : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Button button_btn_close; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		button_btn_close = FindComp<Button>("RawImage/btn_close"); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
	}	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void Onbtn_closeClicked(GameObject go)
	{
		CloseUIForm();
	}
	// UI EVENT FUNC END

	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
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
