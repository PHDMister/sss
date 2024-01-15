//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class EditPersonalDataPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    public Image thisBg;
    private Image image_NamePanel;
    private Text nameValue_Text;
    private Text text_NameValue_FontCount_Text;
    private Image image_GenderPanel;
    private Button button_Girl_Button;
    private Button button_Boy_Button;
    private Button button_Secrecy_Button;
    private Image image_Tip_Image;
    private Text text_Text;
    private Button button_Age_Button;
    private Text text_AgeBtnTip_Text;
    private Image image_AgeBtnTip_Image;
    private InputField inputfield_Signature_InputField;
    private Text inputfield_Signature_Text;
    private Text text_SignatureFontCount_Text;
    private Button button_Cancel_Button;
    private Button button_Affirm_Button;

    private GameObject choosePanel;
    private Button chooseAgeCancel_Button;
    private Button chooseAgeAffirm_Button;


    private PersonUserData personUserData = new PersonUserData();


    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        thisBg = transform.GetComponent<Image>();
        image_NamePanel = FindComp<Image>("NamePanel");
        nameValue_Text = FindComp<Text>("NamePanel/NameValue_Text");
        text_NameValue_FontCount_Text = FindComp<Text>("NamePanel/NameValue_FontCount_Text");
        image_GenderPanel = FindComp<Image>("GenderPanel");
        button_Girl_Button = FindComp<Button>("GenderPanel/Girl_Button");
        button_Boy_Button = FindComp<Button>("GenderPanel/Boy_Button");
        button_Secrecy_Button = FindComp<Button>("GenderPanel/Secrecy_Button");
        image_Tip_Image = FindComp<Image>("GenderPanel/Tip_Image");
        text_Text = FindComp<Text>("AgePanel/Text");
        button_Age_Button = FindComp<Button>("AgePanel/Age_Button");
        text_AgeBtnTip_Text = FindComp<Text>("AgePanel/Age_Button/AgeBtnTip_Text");
        image_AgeBtnTip_Image = FindComp<Image>("AgePanel/Age_Button/AgeBtnTip_Image");
        inputfield_Signature_InputField = FindComp<InputField>("SignaturePanel/Signature_InputField");

        inputfield_Signature_Text = FindComp<Text>("SignaturePanel/Signature_InputField/Placeholder");
        text_SignatureFontCount_Text = FindComp<Text>("SignaturePanel/SignatureFontCount_Text");
        button_Cancel_Button = FindComp<Button>("Cancel_Button");
        button_Affirm_Button = FindComp<Button>("Affirm_Button");

        choosePanel = transform.Find("ChooseAge_Panel").gameObject;
        chooseAgeCancel_Button = FindComp<Button>("ChooseAge_Panel/ChooseAgeCancel_Button");
        chooseAgeAffirm_Button = FindComp<Button>("ChooseAge_Panel/ChooseAgeAffirm_Button");


        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

 
        RigisterCompEvent(button_Girl_Button, OnGirl_ButtonClicked);
        RigisterCompEvent(button_Boy_Button, OnBoy_ButtonClicked);
        RigisterCompEvent(button_Secrecy_Button, OnSecrecy_ButtonClicked);
        RigisterCompEvent(button_Age_Button, OnAge_ButtonClicked);
        inputfield_Signature_InputField.onValueChanged.AddListener(OnSignature_InputFieldValueChanged);

        RigisterCompEvent(button_Cancel_Button, OnCancel_ButtonClicked);
        RigisterCompEvent(button_Affirm_Button, OnAffirm_ButtonClicked);

        RigisterCompEvent(chooseAgeCancel_Button, ChooseAgeCancel_ButtonClicked);
        RigisterCompEvent(chooseAgeAffirm_Button, ChooseAgeAffirm_ButtonClicked);
    }

    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnNameValue_InputFieldValueChanged(string arg)
    {
        Debug.Log("输出一下字符的长度： " + arg.Length);
        text_NameValue_FontCount_Text.text = string.Format("{0}/{1}", arg.Length, "20");
        if (arg == "")
        {
            arg = ManageMentClass.DataManagerClass.selfPersonData.login_name;
        }
        personUserData.login_name = arg;
    }


    private void OnGirl_ButtonClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);
        personUserData.gender = "女";
        Debug.Log("输出性别： " + personUserData.gender);
    }
    private void OnBoy_ButtonClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);
        personUserData.gender = "男";
        Debug.Log("输出性别： " + personUserData.gender);
    }
    private void OnSecrecy_ButtonClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);
        personUserData.gender = "保密";
        Debug.Log("输出性别： " + personUserData.gender);
    }
    private void OnAge_ButtonClicked(GameObject go)
    {
        choosePanel.gameObject.SetActive(true);
        image_AgeBtnTip_Image.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    private void OnSignature_InputFieldValueChanged(string arg)
    {
        text_SignatureFontCount_Text.text = string.Format("{0}/{1}", arg.Length, "160");
        if (arg == "")
        {
            arg = ManageMentClass.DataManagerClass.selfPersonData.explain;
        }
        personUserData.explain = arg;
    }
    private void OnCancel_ButtonClicked(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(go);
        CloseUIForm();
    }
    private void OnAffirm_ButtonClicked(GameObject go)
    {
        PersonUserSaveTopData userData = new PersonUserSaveTopData();
        userData.data.age = personUserData.age;
        userData.data.constell = personUserData.constell;
        userData.data.explain = personUserData.explain;
        userData.data.login_name = personUserData.login_name;
        userData.data.gender = personUserData.gender;
        string data = JsonConvert.SerializeObject(userData);
        EventSystem.current.SetSelectedGameObject(go);
        Debug.Log("输出一下最后的值： " + data);
        MessageManager.GetInstance().RequestSavePersonData(() =>
        {
            //成功
            SendMessage("RefreshPlayerName", "Success", userData.data.login_name);
            SaveEditorPersonDataFun();
            SendMessage("EditorPersonalDataPanelSaveData", "Success", null);
            CloseUIForm();

        }, () =>
        {
            //失败
            // CloseUIForm();
        }, data);


    }
    // 选择年龄的取消按钮
    private void ChooseAgeCancel_ButtonClicked(GameObject go)
    {
        choosePanel.gameObject.SetActive(false);
        image_AgeBtnTip_Image.transform.localEulerAngles = new Vector3(180, 0, 0);
    }
    // 选择年龄的确认按钮
    private void ChooseAgeAffirm_ButtonClicked(GameObject go)
    {
        choosePanel.gameObject.SetActive(false);
        image_AgeBtnTip_Image.transform.localEulerAngles = new Vector3(180, 0, 0);

        text_AgeBtnTip_Text.text = personUserData.age + " " + personUserData.constell;
    }
    // UI EVENT FUNC END
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        ReceiveMessage("DateControlAgeValue", p =>
        {
            personUserData.age = (string)p.Values;
        });
        ReceiveMessage("DateControlConstellationValue", p =>
        {
            personUserData.constell = (string)p.Values;
        });
    }
    public override void Display()
    {
        base.Display();
        choosePanel.gameObject.SetActive(false);
        InitializeDataSetUIFun();
        SetUIFun();
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
    void SetBtnTipImgePosFun(GameObject Obj)
    {
        image_Tip_Image.transform.localPosition = new Vector3(Obj.transform.localPosition.x, image_Tip_Image.transform.localPosition.y, image_Tip_Image.transform.localPosition.z);

    }

    /// <summary>
    /// 每次打开刷新数据
    /// </summary>
    void InitializeDataSetUIFun()
    {
        Debug.Log("输出一下正式数据的值： " + ManageMentClass.DataManagerClass.selfPersonData.login_name);
        personUserData.age = ManageMentClass.DataManagerClass.selfPersonData.age;
        personUserData.constell = ManageMentClass.DataManagerClass.selfPersonData.constell;
        personUserData.gender = ManageMentClass.DataManagerClass.selfPersonData.gender;
        personUserData.explain = ManageMentClass.DataManagerClass.selfPersonData.explain;
        personUserData.login_name = ManageMentClass.DataManagerClass.selfPersonData.login_name;
        personUserData.code = ManageMentClass.DataManagerClass.selfPersonData.code;
    }
    /// <summary>
    /// 保存数据
    /// </summary>
    void SaveEditorPersonDataFun()
    {

        Debug.Log("保存数据的值： " + personUserData.login_name);

        ManageMentClass.DataManagerClass.selfPersonData.age = personUserData.age;
        ManageMentClass.DataManagerClass.selfPersonData.constell = personUserData.constell;
        ManageMentClass.DataManagerClass.selfPersonData.gender = personUserData.gender;
        ManageMentClass.DataManagerClass.selfPersonData.explain = personUserData.explain;
        ManageMentClass.DataManagerClass.selfPersonData.login_name = personUserData.login_name;
        ManageMentClass.DataManagerClass.selfPersonData.code = personUserData.code;
    }

    void SetUIFun()
    {
        
        inputfield_Signature_InputField.text = null;
        nameValue_Text.text = TextTools.setCutAddString(ManageMentClass.DataManagerClass.selfPersonData.login_name, 16, "");

        if (ManageMentClass.DataManagerClass.selfPersonData.age == "")
        {
            text_AgeBtnTip_Text.text = "00后 狮子座";
        }
        else
        {
            text_AgeBtnTip_Text.text = ManageMentClass.DataManagerClass.selfPersonData.age + " " + ManageMentClass.DataManagerClass.selfPersonData.constell;
        }
        if (ManageMentClass.DataManagerClass.selfPersonData.explain == "")
        {
            inputfield_Signature_Text.text = "未来世界已到来";
        }
        else
        {
            inputfield_Signature_Text.text = ManageMentClass.DataManagerClass.selfPersonData.explain;
        }

        switch (ManageMentClass.DataManagerClass.selfPersonData.gender)
        {
            case "男":
                SetBtnTipImgePosFun(button_Boy_Button.gameObject);
                break;
            case "女":
                SetBtnTipImgePosFun(button_Girl_Button.gameObject);
                break;
            case "保密":
                SetBtnTipImgePosFun(button_Secrecy_Button.gameObject);
                break;
            default:
                //为空时 为保密状态
                SetBtnTipImgePosFun(button_Secrecy_Button.gameObject);
                break;
        }
    }

}
