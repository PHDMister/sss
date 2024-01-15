using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
public class HomeMain_Panel : BaseUIForm
{

    private Button return_Btn;
    private Button RuleIntro_Btn;
    private Button rewardList_Btn;
    private Button startGame_Btn;


    private void Awake()
    {

        return_Btn = FindComp<Button>("Return_Btn");
        RuleIntro_Btn = FindComp<Button>("RuleIntro_Btn");
        rewardList_Btn = FindComp<Button>("RewardList_Btn");
        startGame_Btn = FindComp<Button>("StartGame_Btn");

        OnAwake();
        AddEvent();
        GameController.Instance().InitFun();
    }
    private void AddEvent()
    {
        RigisterCompEvent(return_Btn, Return_BtnClick);
        RigisterCompEvent(RuleIntro_Btn, RuleIntro_BtnClick);
        RigisterCompEvent(rewardList_Btn, RewardList_BtnClick);
        RigisterCompEvent(startGame_Btn, StartGame_BtnClick);
    }
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
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

    private void Return_BtnClick(GameObject go)
    {
        OnClickReturn();
    }
    private void RuleIntro_BtnClick(GameObject go)
    {
        OpenUIForm(FormConst.RULEINTROPNEL);
    }
    private void RewardList_BtnClick(GameObject go)
    {
        OpenUIForm(FormConst.REWARDLISTPANEL);
    }
    private void StartGame_BtnClick(GameObject go)
    {
        Debug.Log("输出一下具体ide内容的值：");

        OpenUIForm(FormConst.MATCHINGPANEL);
        CloseUIForm();
    }
    void OnClickReturn()
    {
        //点击了退出
        try
        {
          //  SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
}
