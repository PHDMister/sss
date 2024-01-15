//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class DogExchangCardTips : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_quxiao;
    private Button button_button_queding;
    private Text text_txt_shuoming;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_quxiao = transform.Find("duihuanquerentishi/Frame 1000008002/buton/button-quxiao").GetComponent<Button>();
        button_button_queding = transform.Find("duihuanquerentishi/Frame 1000008002/buton/button-queding").GetComponent<Button>();
        text_txt_shuoming = transform.Find("duihuanquerentishi/Frame 1000008002/txt-shuoming").GetComponent<Text>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

        RigisterCompEvent(button_button_quxiao, Onbutton_quxiaoClicked);

        RigisterCompEvent(button_button_queding, Onbutton_quedingClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START

    private void Onbutton_quxiaoClicked(GameObject go)
    {
        CloseUIForm();
    }

    private void Onbutton_quedingClicked(GameObject go)
    {
        OnExchange();
    }
    // UI EVENT FUNC END


    private string format = @"<color=#FEF4CD>是否将编号：{0}的
【{1}】
兑换为实体犬领养凭证？</color>

<color=#FF4893>兑换为凭证后，虚拟犬将消失。</color>";
    private void OnAwake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        ReceiveMessage("OpenPetExchangeConfirm", (p) =>
        {
            PetModelRecData data = p.Values as PetModelRecData;
            string petNumber = data.pet_number;
            string petName = data.pet_name;
            text_txt_shuoming.text = string.Format(format, petNumber, petName);
        });
    }


    private void OnExchange()
    {
        DogSetDataPanel dogSetDataPanel = UIManager.GetInstance().GetUIForm(FormConst.DOGSETDATAPANEL) as DogSetDataPanel;
        PetModelRecData data = dogSetDataPanel.GetPetData();
        if (dogSetDataPanel != null)
        {
            if (!dogSetDataPanel.bCanExchange())
            {
                ToastManager.Instance.ShowPetToast("宠物状态均未达到60以上，无法兑换实体犬领养凭证。", 3f);
                return;
            }
            if (data != null)
            {
                if (data.status == (int)PetStatus.Died)
                {
                    ToastManager.Instance.ShowPetToast("您的宠物因健康度过低已被收容，无法兑换。", 3f);
                    return;
                }
            }
        }
        //兑换请求TODO
        MessageManager.GetInstance().RequestExchangeProof(data.id, () =>
         {
             CloseUIForm();
             UIManager.GetInstance().CloseUIForms(FormConst.DOGSETDATAPANEL);
             OpenUIForm(FormConst.DogSuccessExchange);
             SendMessage("OpenPetExchangeSuccess", "Success", data);
             PetSpanManager.Instance().RemovePetObj(data.id);
             MessageManager.GetInstance().RequestPetList(() =>
             {
                 PetSpanManager.Instance().UpdatePet();
             });

             MessageManager.GetInstance().RequestExchangeProofNum((p) =>
            {
                ExchangeProofRecData data = p;
                SendMessage("RecExchangeProofNum", "Success", data);
            });
         });
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
