using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Fight;

public class RewardList_Panel : BaseUIForm
{
    // UI VARIABLE STATEMENT START

    private Button button_Close_Btn;
    private Button button_Audio_Btn;
    private Text Shelltext;
    // UI VARIABLE STATEMENT END

    public CircularScrollView.UICircularScrollView rewardListScroll;

    private FightListResp fightListResp;

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {

        button_Close_Btn = FindComp<Button>("ShowReward_Panel/Close_Btn");
        button_Audio_Btn = FindComp<Button>("Audio_Btn");
        Shelltext = FindComp<Text>("Shell_Value/Text");
        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Close_Btn, OnClose_BtnClicked);
        RigisterCompEvent(button_Audio_Btn, OnAudio_BtnClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnClose_BtnClicked(GameObject go)
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
        rewardListScroll.Init(NormalCallBack);
    }

    public override void Display()
    {
        base.Display();
        Shelltext.text = ManageMentClass.DataManagerClass.ShellCount + "";

        FightListReq fightListReq = new FightListReq();
        fightListReq.UserId = ManageMentClass.DataManagerClass.userId;
        fightListReq.Page = 1;
        fightListReq.PageSize = 50;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.FightListReq, fightListReq, (code, bytes) =>
        {
            Debug.Log($"WebSocketConnect  RegisterResp code:{code}");
            if (code != 0) return;
            fightListResp = FightListResp.Parser.ParseFrom(bytes);
            Debug.Log("输出一下对局记录总共有多少个： " + fightListResp.List.Count + "  fightListResp:  " + fightListResp.ToJSON());
            rewardListScroll.ShowList(fightListResp.List.Count);
            // registerResp.List.Count;
        });
        // WinRateReq
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



    private void NormalCallBack(GameObject cell, int index)
    {
        if (cell != null)
        {
            Debug.Log("cell存在： " + index);
        }
        else
        {
            Debug.Log("不存在： ");
        }
        cell.GetComponent<RewardListItem>().SetUIDataFun(fightListResp.List[index - 1]);
    }
}
