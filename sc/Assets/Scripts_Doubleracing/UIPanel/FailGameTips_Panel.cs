//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class FailGameTips_Panel : BaseUIForm
{
    // UI VARIABLE STATEMENT START
    private Text text_Tips_Text;
    private Button button_GoOn_Btn;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_Tips_Text = FindComp<Text>("Image/Tips_Text");
        button_GoOn_Btn = FindComp<Button>("GoOn_Btn");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_GoOn_Btn, OnGoOn_BtnClicked);
    }   // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnGoOn_BtnClicked(GameObject go)
    {
        UIManager.GetInstance().CloseAllShowPanel();
        OpenUIForm(FormConst.HOMEMAINPANEL);
    }
    // UI EVENT FUNC END
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
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

    public void SetRoleId(int id)
    {
        if (id == 1)
        {
            text_Tips_Text.text = "很遗憾没有抓住出逃者~";
        }
        else
        {
            text_Tips_Text.text = "很遗憾已被追击者抓住~";
        }
    }

}
