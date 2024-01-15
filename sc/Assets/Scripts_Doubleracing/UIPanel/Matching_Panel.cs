using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Fight;

public class Matching_Panel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_ShellValue_Text;
    private Button button_RuleIntro_Btn;
    private Button button_Return_Btn;
    private Button button_RewardList_Btn;
    private Button button_Audio_Btn;
    private Image image_Lightning;
    private Slider slider_Attack_Slider;
    private Slider slider_Runaway_Slider;
    private Slider slider_Lighting_Slider;
    private Text text_AttckValue_Text;
    private Image image_Attack_ChooseTips_Img;
    private Text text_RunAwayValue_Text;
    private Image image_RunAway_ChooseTips_Img;
    private Image image_AttackSaid_Panel;
    private Text text_Text;
    private Image image_RunawaySaid_Panel;
    private Text text_Text16;
    private Image image_CarBigIcon_Img;
    private Button button_Choose_Attack_Btn;
    private Text text_Attack_CarName_Text;
    private Image image_AttackCar_Icon;
    private Image image_AttackCarDI_Img;
    private Image image_AttackBtnChooseTips_Img;



    private Button button_Choose_Runaway_Btn;
    private Text text_Runaway_CarName_Text;

    private Image image_RunawayCar_Icon;
    private Image image_RunawayCarDI_Img;
    private Image image_RunawayBtnChooseTips_Img;


    private Button button_Begin_Btn;


    private Image image_Affiml_Panel;

    private Text text_SheelText;

    private Button button_GiveUp_Btn;

    private Button button_GoOn_Btn;


    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_ShellValue_Text = FindComp<Text>("Shell_Value/ShellValue_Text");
        button_RuleIntro_Btn = FindComp<Button>("RuleIntro_Btn");
        button_Return_Btn = FindComp<Button>("Return_Btn");
        button_RewardList_Btn = FindComp<Button>("RewardList_Btn");
        button_Audio_Btn = FindComp<Button>("Audio_Btn");
        image_Lightning = FindComp<Image>("GameSchedule_Panel/Lightning");
        slider_Attack_Slider = FindComp<Slider>("GameSchedule_Panel/Attack_Slider");
        slider_Runaway_Slider = FindComp<Slider>("GameSchedule_Panel/Runaway_Slider");
        slider_Lighting_Slider = FindComp<Slider>("GameSchedule_Panel/LightingSlider");
        text_AttckValue_Text = FindComp<Text>("GameSchedule_Panel/Attack_Img/AttckValue_Text");
        image_Attack_ChooseTips_Img = FindComp<Image>("GameSchedule_Panel/Attack_Img/Attack_ChooseTips_Img");
        text_RunAwayValue_Text = FindComp<Text>("GameSchedule_Panel/Runaway_Img/RunAwayValue_Text");
        image_RunAway_ChooseTips_Img = FindComp<Image>("GameSchedule_Panel/Runaway_Img/RunAway_ChooseTips_Img");
        image_AttackSaid_Panel = FindComp<Image>("SaidTips_Panel/AttackSaid_Panel");
        text_Text = FindComp<Text>("SaidTips_Panel/AttackSaid_Panel/Image/Text");
        image_RunawaySaid_Panel = FindComp<Image>("SaidTips_Panel/RunawaySaid_Panel");
        text_Text16 = FindComp<Text>("SaidTips_Panel/RunawaySaid_Panel/Image/Text");
        image_CarBigIcon_Img = FindComp<Image>("CarBigIcon_Img");
        button_Choose_Attack_Btn = FindComp<Button>("Choose_Attack_Btn");
        text_Attack_CarName_Text = FindComp<Text>("Choose_Attack_Btn/Attack_CarName_Text");

        image_AttackCar_Icon = FindComp<Image>("Choose_Attack_Btn/Car_Icon");
        image_AttackCarDI_Img = FindComp<Image>("Choose_Attack_Btn/CarDI_Img");
        image_AttackBtnChooseTips_Img = FindComp<Image>("Choose_Attack_Btn/ChooseTips_Img");


        button_Choose_Runaway_Btn = FindComp<Button>("Choose_Runaway_Btn");
        text_Runaway_CarName_Text = FindComp<Text>("Choose_Runaway_Btn/Runaway_CarName_Text");
        image_RunawayCar_Icon = FindComp<Image>("Choose_Runaway_Btn/Car_Icon");
        image_RunawayCarDI_Img = FindComp<Image>("Choose_Runaway_Btn/CarDI_Img");
        image_RunawayBtnChooseTips_Img = FindComp<Image>("Choose_Runaway_Btn/ChooseTips_Img");


        button_Begin_Btn = FindComp<Button>("Begin_Btn");



        image_Affiml_Panel = FindComp<Image>("Affiml_Panel");

        text_SheelText = FindComp<Text>("Affiml_Panel/SheelText");

        button_GiveUp_Btn = FindComp<Button>("Affiml_Panel/GiveUp_Btn");

        button_GoOn_Btn = FindComp<Button>("Affiml_Panel/GoOn_Btn");


        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_RuleIntro_Btn, OnRuleIntro_BtnClicked);
        RigisterCompEvent(button_Return_Btn, OnReturn_BtnClicked);
        RigisterCompEvent(button_RewardList_Btn, OnRewardList_BtnClicked);
        RigisterCompEvent(button_Audio_Btn, OnAudio_BtnClicked);
        slider_Attack_Slider.onValueChanged.AddListener(OnAttack_SliderValueChanged);
        slider_Runaway_Slider.onValueChanged.AddListener(OnRunaway_SliderValueChanged);
        RigisterCompEvent(button_Choose_Attack_Btn, OnChoose_Attack_BtnClicked);
        RigisterCompEvent(button_Choose_Runaway_Btn, OnChoose_Runaway_BtnClicked);
        RigisterCompEvent(button_Begin_Btn, Begin_BtnClick);

        RigisterCompEvent(button_GiveUp_Btn, Begin_GiveUpBtnClick);
        RigisterCompEvent(button_GoOn_Btn, Begin_GoOnBtnClick);
    }


    // UI EVENT REGISTER END

    private uint roletypeId = 0;


    // UI EVENT FUNC START
    private void OnRuleIntro_BtnClicked(GameObject go)
    {
        OpenUIForm(FormConst.RULEINTROPNEL);
    }
    private void OnReturn_BtnClicked(GameObject go)
    {
        OpenUIForm(FormConst.HOMEMAINPANEL);
        CloseUIForm();
    }
    private void OnRewardList_BtnClicked(GameObject go)
    {
        OpenUIForm(FormConst.REWARDLISTPANEL);
    }
    private void OnAudio_BtnClicked(GameObject go)
    {

    }
    private void OnAttack_SliderValueChanged(float arg)
    {

    }
    private void OnRunaway_SliderValueChanged(float arg)
    {

    }
    private void OnChoose_Attack_BtnClicked(GameObject go)
    {
        roletypeId = 1;
        SetAttackBtnEffect(1, true);
        SetRunawayBtnEffect(0.4f, false);

        Debug.Log("输出一下具体的选择的项： 追击者");
    }
    private void OnChoose_Runaway_BtnClicked(GameObject go)
    {
        roletypeId = 2;
        SetAttackBtnEffect(0.4f, false);
        SetRunawayBtnEffect(1, true);

        Debug.Log("输出一下具体的选择的项： 出逃者");
    }
    private void Begin_BtnClick(GameObject go)
    {
        if (roletypeId != 0)
        {
            Debug.Log("开始匹配");
            image_Affiml_Panel.gameObject.SetActive(true);
        }
        else
        {
            ToastManager.Instance.ShowNewToast("请选择角色", 2f);
            Debug.Log("请选择角色");
        }

    }
    // UI EVENT FUNC END



    private void Begin_GiveUpBtnClick(GameObject go)
    {
        image_Affiml_Panel.gameObject.SetActive(false);
    }

    private void Begin_GoOnBtnClick(GameObject go)
    {
        ReadyReq registerReq = new ReadyReq();
        registerReq.UserId = ManageMentClass.DataManagerClass.userId;
        registerReq.RoleType = roletypeId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ReadyReq, registerReq, (code, bytes) =>
        {
            Debug.Log($"WebSocketConnect  RegisterResp code:{code}");
            if (code != 0) return;
            ReadyResp registerResp = ReadyResp.Parser.ParseFrom(bytes);
            Debug.Log("reginsete的结果内容： " + registerResp.ToJSON());
            if (registerResp.StatusCode == 0)
            {
                OpenUIForm(FormConst.WAITMATCHPANEL);
                ManageMentClass.DataManagerClass.ShellCount -= 500;
                text_ShellValue_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";
                CloseUIForm();
            }
            else if (registerResp.StatusCode == 270002)
            {
                ToastManager.Instance.ShowNewToast("您的上局游戏马上结束，请稍后", 3f);
            }
            else if (registerResp.StatusCode == 270001)
            {
                ToastManager.Instance.ShowNewToast("您的贝壳不足", 2f);
            }
        });
    }


    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
    }

    public override void Display()
    {
        base.Display();
        roletypeId = 0;
        text_ShellValue_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";
        SetAttackBtnEffect(1, false);
        SetRunawayBtnEffect(1, false);
        image_CarBigIcon_Img.gameObject.SetActive(false);
        image_Attack_ChooseTips_Img.gameObject.SetActive(false);
        image_RunAway_ChooseTips_Img.gameObject.SetActive(false);

        image_AttackSaid_Panel.gameObject.SetActive(false);
        image_RunawaySaid_Panel.gameObject.SetActive(false);

        image_Affiml_Panel.gameObject.SetActive(false);

        text_SheelText.text = "500";
        WinRateReq registerReq = new WinRateReq();

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.WinRateReq, registerReq, (code, bytes) =>
        {
            Debug.Log($"WebSocketConnect  RegisterResp code:{code}");
            if (code != 0) return;
            WinRateResp registerResp = WinRateResp.Parser.ParseFrom(bytes);
            Debug.Log("WinRateResp的结果内容： " + registerResp.ToJSON());

            Debug.Log("policeDe : " + (float)registerResp.Police / 100f);
            Debug.Log("Criminal : " + (float)registerResp.Criminal / 100f); ;

            slider_Attack_Slider.value = (float)registerResp.Police / 100f;

            slider_Lighting_Slider.value = (float)registerResp.Police / 100f;

            slider_Runaway_Slider.value = (float)registerResp.Criminal / 100f;
            text_AttckValue_Text.text = registerResp.Police + "%";
            text_RunAwayValue_Text.text = registerResp.Criminal + "%";
        });



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




    void SetAttackBtnEffect(float alphaValue, bool isActive)
    {

        Color originalColor = image_AttackCar_Icon.color;
        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
        image_AttackCar_Icon.color = newColor;

        Color originalAColor = image_AttackCarDI_Img.color;
        Color newAColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
        image_AttackCarDI_Img.color = newAColor;
        image_AttackBtnChooseTips_Img.gameObject.SetActive(isActive);

        if (isActive)
        {
            image_CarBigIcon_Img.gameObject.SetActive(true);
            image_CarBigIcon_Img.sprite = Resources.Load("UIRes/Texture/Car/lan_LanCar", typeof(Sprite)) as Sprite;
            image_Attack_ChooseTips_Img.gameObject.SetActive(true);
            image_RunAway_ChooseTips_Img.gameObject.SetActive(false);

            image_AttackSaid_Panel.gameObject.SetActive(true);
            image_RunawaySaid_Panel.gameObject.SetActive(false);
        }

    }

    void SetRunawayBtnEffect(float alphaValue, bool isActive)
    {

        Color originalColor = image_RunawayCar_Icon.color;
        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
        image_RunawayCar_Icon.color = newColor;

        Color originalAColor = image_RunawayCarDI_Img.color;
        Color newAColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
        image_RunawayCarDI_Img.color = newAColor;
        image_RunawayBtnChooseTips_Img.gameObject.SetActive(isActive);
        if (isActive)
        {
            image_CarBigIcon_Img.gameObject.SetActive(true);
            image_CarBigIcon_Img.sprite = Resources.Load("UIRes/Texture/Car/fen_Car", typeof(Sprite)) as Sprite;
            image_Attack_ChooseTips_Img.gameObject.SetActive(false);
            image_RunAway_ChooseTips_Img.gameObject.SetActive(true);

            image_AttackSaid_Panel.gameObject.SetActive(false);
            image_RunawaySaid_Panel.gameObject.SetActive(true);
        }
    }

}
