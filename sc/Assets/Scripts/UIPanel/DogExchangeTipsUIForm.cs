//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class DogExchangeTipsUIForm : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Button button_button_quxiao; 
	private Button button_button_quduihuan; 
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		button_button_quxiao = transform.Find("duihuanxuzhi/button/button-quxiao").GetComponent<Button>(); 
		button_button_quduihuan = transform.Find("duihuanxuzhi/button/button-quduihuan").GetComponent<Button>(); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{

		RigisterCompEvent(button_button_quxiao, Onbutton_quxiaoClicked);

		RigisterCompEvent(button_button_quduihuan, Onbutton_quduihuanClicked);
	}
	// UI EVENT REGISTER END

	// UI EVENT FUNC START

	private void Onbutton_quxiaoClicked(GameObject go)
	{
		CloseUIForm();
	}

	private void Onbutton_quduihuanClicked(GameObject go)
	{
		PetModelRecData data = null;
		DogSetDataPanel dogSetDataPanel = UIManager.GetInstance().GetUIForm(FormConst.DOGSETDATAPANEL) as DogSetDataPanel;
		if (dogSetDataPanel != null)
		{
			data = dogSetDataPanel.GetPetData();
		}
		CloseUIForm();
		OpenUIForm(FormConst.DogExchangCardTips);
		SendMessage("OpenPetExchangeConfirm", "Success", data);
	}
	// UI EVENT FUNC END

	private void OnAwake() 
	{
		//窗体的性质
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
	}

    public override void Display()
    {
        base.Display();
		InterfaceHelper.SetJoyStickState(false);
	}

    public override void Hiding()
    {
        base.Hiding();
		InterfaceHelper.SetJoyStickState(true);
    }
}
