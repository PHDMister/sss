//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachWaste : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Text text_Text_tip; 
	private Text text_Text; 
	private Button button_btn_sure; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		text_Text_tip = FindComp<Text>("Image_bg/Text_tip"); 
		text_Text = FindComp<Text>("Image_bg/Text"); 
		button_btn_sure = FindComp<Button>("Image_bg/btn_sure"); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_btn_sure, Onbtn_sureClicked);
	}	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void Onbtn_sureClicked(GameObject go)
	{
		CloseUIForm();
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
        const string tipTemp = "感谢您对环保做出贡献, 奖励您<color=yellow>{0}个贝壳</color>";
        BeachShellResp RewardInfo = Singleton<RainbowBeachController>.Instance.RewardInfo;
        text_Text_tip.text = string.Format(tipTemp, RewardInfo.RewardQuantity);
        Singleton<BagMgr>.Instance.AddShellNum((int)RewardInfo.RewardQuantity);
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
