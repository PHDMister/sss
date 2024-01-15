//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class DogExchangCardShowTips : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_queding;
    private Button button_btn_close;
    private Text text_txt;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_queding = FindComp<Button>("chankanpingzheng/neirong/xixineirong/button-queding");
        button_btn_close = FindComp<Button>("chankanpingzheng/btn_close");
        text_txt = FindComp<Text>("chankanpingzheng/neirong/xixineirong/xinxi/txt");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

        RigisterCompEvent(button_button_queding, Onbutton_quedingClicked);

        RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START

    private void Onbutton_quedingClicked(GameObject go)
    {
      
        if (!ManageMentClass.DataManagerClass.WebInto)
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
        else
        {
            ToastManager.Instance.ShowNewToast("请前往App端联系客服领养", 5f);
            CloseUIForm();
        }

    }

    private void Onbtn_closeClicked(GameObject go)
    {
        CloseUIForm();
    }
    // UI EVENT FUNC END


    private void OnAwake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        ReceiveMessage("OpenDogExchangeCardShow", p =>
         {
             ExchangeProofRecData data = p.Values as ExchangeProofRecData;
             SetNum(data.total);
         });
    }

    private void SetNum(int num)
    {
        text_txt.text = string.Format("您拥有宠物领养凭证*{0}", num);
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
