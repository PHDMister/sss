//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class UITreasureReadyCheck : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_tip;
    private Text text_words;
    private Button button_button_quxiao;
    private Button button_button_queding;
    private Button button_button_guanbi;
    private Toggle toggle_Toggle;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_tip = FindComp<Text>("haoyoutanxhuang/Group 1000008114/Group 1000008112/kongjiantanchuang-bg/Group 1000008291/tip");
        text_words = FindComp<Text>("haoyoutanxhuang/Group 1000008114/Group 1000008112/kongjiantanchuang-bg/words");
        button_button_quxiao = FindComp<Button>("buton/button-quxiao");
        button_button_queding = FindComp<Button>("buton/button-queding");
        button_button_guanbi = FindComp<Button>("haoyoutanxhuang/icon-guanbi02");
        toggle_Toggle = FindComp<Toggle>("Toggle");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_button_quxiao, Onbutton_quxiaoClicked);
        RigisterCompEvent(button_button_queding, Onbutton_quedingClicked);
        RigisterCompEvent(button_button_guanbi, Onbutton_quxiaoClicked);
        toggle_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbutton_quxiaoClicked(GameObject go)
    {
        CloseUIForm();
    }
    private void Onbutton_quedingClicked(GameObject go)
    {
        ReadyTreasureReq req = new ReadyTreasureReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        UITreasureReadyPanel treasureReady =
            UIManager.GetInstance().GetUIForm<UITreasureReadyPanel>(FormConst.TREASUREREADYPANEL);
        if (treasureReady != null)
            WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ReadyTreasureReq, req, (code, data) =>
            {
                CloseUIForm();
                treasureReady.ReadCallback(code, data);
            });
        else
            Debug.LogError("UITreasureReadyPanel  is  null  !!!!");
    }
    private void OnToggleValueChanged(bool arg)
    {
        TreasuringController.ReadyShowNextCheck = !arg;
        PlayerPrefs.SetString(TreasuringController.PlayerPrefs_ShowNextCheck, arg ? TreasureModel.Instance.CurTime.ToString() : "");
        PlayerPrefs.Save();
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
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
