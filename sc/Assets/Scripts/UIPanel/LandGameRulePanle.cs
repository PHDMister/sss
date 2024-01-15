//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using UnityEngine.SceneManagement;

public class LandGameRulePanle : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_Close_Button;
    private Text text_Text;
    private Text Title_Text;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_Close_Button = FindComp<Button>("Close_Button");
        text_Text = FindComp<Text>("Scroll View/Viewport/Content/Text");
        Title_Text = FindComp<Text>("Title_Text");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START

    island_help island_Help;


    private void AddEvent()
    {
        RigisterCompEvent(button_Close_Button, OnClose_ButtonClicked);
    }   // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnClose_ButtonClicked(GameObject go)
    {
        CloseUIForm();
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

    }

    public override void Display()
    {
        base.Display();
        RefreshTextFun();
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

    public void RefreshTextFun()
    {
        switch (ManageMentClass.DataManagerClass.SceneID)
        {
            case (int)LoadSceneType.RainbowBeach:
                island_Help = ManageMentClass.DataManagerClass.GetIsLandHelpTable(1);
                text_Text.text = island_Help.text;
                Title_Text.text = island_Help.help_name;
                break;
            case (int)LoadSceneType.ShenMiHaiWan:
                island_Help = ManageMentClass.DataManagerClass.GetIsLandHelpTable(2);
                text_Text.text = island_Help.text;
                Title_Text.text = island_Help.help_name;
                break;
            case (int)LoadSceneType.HaiDiXingKong:
                island_Help = ManageMentClass.DataManagerClass.GetIsLandHelpTable(3);
                text_Text.text = island_Help.text;
                Title_Text.text = island_Help.help_name;
                break;
        }
    }
    
    public void RefreshTextByCfgId(int cfgId)
    {
        island_Help = ManageMentClass.DataManagerClass.GetIsLandHelpTable(cfgId);
        text_Text.text = island_Help.text;
        Title_Text.text = island_Help.help_name;
    }

}
