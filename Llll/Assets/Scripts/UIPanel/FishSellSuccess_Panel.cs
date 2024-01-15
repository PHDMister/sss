//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class FishSellSuccess_Panel : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Text text_TxtResult1; 
	private Image image_FishIcon; 
	private Text text_TxtResult2; 
	private Button button_BtnKnow; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		text_TxtResult1 = FindComp<Text>("Content/Result1/TxtResult1"); 
		image_FishIcon = FindComp<Image>("Content/Result1/IconNode/Icon");
		text_TxtResult2 = FindComp<Text>("Content/Result2/TxtResult2"); 
		button_BtnKnow = FindComp<Button>("BtnKnow"); 
		OnAwake(); 
		AddEvent(); 
	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		RigisterCompEvent(button_BtnKnow, OnBtnKnowClicked);
	}	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void OnBtnKnowClicked(GameObject go)
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

        if (AudioMgr.Ins.isPlaying)
        {
            GetComponent<AudioSource>().Play();    
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

	public void ShowTipsContent(int fishId, uint sellCnt, uint income)
	{
		var fishCfg = ManageMentClass.DataManagerClass.GetIsLandFishTable(fishId);
		Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("Fish", fishCfg.fish_icon);
		image_FishIcon.sprite = sprite;
		text_TxtResult1.text = string.Format("成功出售{0}只", sellCnt);
		text_TxtResult2.text = income.ToString();
	}
	
}
