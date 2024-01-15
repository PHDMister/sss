//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class DogSuccessExchangePanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_txtshuoming;
    private Button button_button_lianxikefu;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_txtshuoming = transform.Find("Successful exchange/pet-adoption/txtshuoming").GetComponent<Text>();
        button_button_lianxikefu = transform.Find("Successful exchange/button-lianxikefu").GetComponent<Button>();

        OnAwake();
        AddEvent();
    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_button_lianxikefu, Onbutton_lianxikefuClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START

    private void Onbutton_lianxikefuClicked(GameObject go)
    {
        if (ManageMentClass.DataManagerClass.WebInto)
        {
            ToastManager.Instance.ShowNewToast("请前往App端联系客服领养", 5f);
            CloseUIForm();
        }
        else
        {
            CloseUIForm();
            try
            {
                SetTools.SetPortraitModeFun();
                SetTools.CloseGameFun();
                SetTools.OpenChatWindow();
            }
            catch (System.Exception e)
            {
                Debug.Log("跳转客服异常： " + e);
            }
        }
        
    }
    // UI EVENT FUNC END

    private Button button_close;
    private void OnAwake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("mengcheng", p =>
        {
            CloseUIForm();
        });

        ReceiveMessage("OpenPetExchangeSuccess", p =>
        {
            PetModelRecData data = p.Values as PetModelRecData;
            if (data == null)
                return;
            string petNumber = data.pet_number;
            string petName = data.pet_name;
            SetInfo(data);
        });
    }

    public void SetInfo(PetModelRecData data)
    {
        text_txtshuoming.text = string.Format("您已将编号：{0}的\n【{1}】\n成功兑换为实体犬领养凭证\n", data.pet_number, data.pet_name);
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
