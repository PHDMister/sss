using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
public class RuleIntro_Panel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Image image_Image;
    private Text text_Text;
    private Button button_Sure_Btn;
    private Button button_Audio_Btn;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        image_Image = FindComp<Image>("Shell_Value/Image");
        text_Text = FindComp<Text>("Shell_Value/Text");
        button_Sure_Btn = FindComp<Button>("Sure_Btn");
        button_Audio_Btn = FindComp<Button>("Audio_Btn");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Sure_Btn, OnSure_BtnClicked);
        RigisterCompEvent(button_Audio_Btn, OnAudio_BtnClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnSure_BtnClicked(GameObject go)
    {
        CloseUIForm();
    }
    private void OnAudio_BtnClicked(GameObject go)
    {

    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

    }

    public override void Display()
    {
        base.Display();
        text_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";
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
