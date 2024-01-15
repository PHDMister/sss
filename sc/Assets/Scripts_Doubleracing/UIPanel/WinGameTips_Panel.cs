//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using Fight;

public class WinGameTips_Panel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_Tips_Text;
    private Button button_GoOn_Btn;

    private Text text_ShellValue_Text;

    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_Tips_Text = FindComp<Text>("Image/Tips_Text");
        text_ShellValue_Text = FindComp<Text>("Image/SheelValue_Text");
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
    public void SetRoleData(GameEndPush gameEndPush)
    {



        text_ShellValue_Text.text = "+" + gameEndPush.ReawardCount;
        ManageMentClass.DataManagerClass.ShellCount = (int)gameEndPush.BkCount;
        Debug.Log("输出一下赢了之后现实的内容值： " + gameEndPush.ToJSON());
        if ((int)GameController.Instance().DoubleUserDataDic[ManageMentClass.DataManagerClass.userId].RoleTypeId == 1)
        {
            text_Tips_Text.text = "恭喜你成功抓住出逃者！";
        }
        else
        {
            text_Tips_Text.text = "恭喜你成功躲避追击者！";
        }
    }

}
