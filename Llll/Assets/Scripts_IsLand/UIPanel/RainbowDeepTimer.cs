//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class RainbowDeepTimer : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_Timer;
    private SimpleCountDown countDown;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_Timer = FindComp<Text>("bg/Timer");
        countDown = text_Timer.GetComponent<SimpleCountDown>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

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

    public void StartTimer(int delayTime)
    {
        int endTime = delayTime + ManageMentClass.DataManagerClass.CurTime;
        countDown.SetColor(new Color(110 / 255.0f, 253 / 255.0f, 1), new Color(1, 0, 0), 10);
        countDown.SetEndTime(endTime, -1, "", i =>
        {
            CloseUIForm();
            OpenUIForm(FormConst.RAINBOWCASTAIRTIP);
        });
    }
    public void StartTimerByEndtime(int endtime)
    {
        int endTime = endtime;
        countDown.SetColor(new Color(110 / 255.0f, 253 / 255.0f, 1), new Color(1, 0, 0), 10);
        countDown.SetEndTime(endTime, -1, "", i =>
        {
            CloseUIForm();
            OpenUIForm(FormConst.RAINBOWCASTAIRTIP);
        });
    }

}
