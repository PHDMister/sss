//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachGift : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_txt_context;
    private Button button_btn_ok;
    private const string context_temp = @"恭喜您获得来自<color=#2AA2A3>【{0}】</color>
    赠送的<color=#FF9C28>{1}个贝壳</color>";
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_txt_context = FindComp<Text>("img_bg/txt_context");
        button_btn_ok = FindComp<Button>("img_bg/btn_ok");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_ok, Onbtn_okClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_okClicked(GameObject go)
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

        string tName = Singleton<RainbowBeachDataModel>.Instance.RobShellName;
        uint count = Singleton<RainbowBeachDataModel>.Instance.RobShellCount;
        string testName1 = TextTools.setCutAddString(tName, 8, "...");
        text_txt_context.text = string.Format(context_temp, testName1, count);

        Singleton<BagMgr>.Instance.AddShellNum((int)count);
    }

    public override void Hiding()
    {
        base.Hiding();
        text_txt_context.text = "";
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
