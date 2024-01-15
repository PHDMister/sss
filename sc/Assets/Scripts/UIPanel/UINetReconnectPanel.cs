//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class UINetReconnectPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_quxiao;
    private Button button_button_queding;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_quxiao = FindComp<Button>("dilog/buttonzhu/button-quxiao");
        button_button_queding = FindComp<Button>("dilog/buttonzhu/button-queding");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_button_quxiao, Onbutton_quxiaoClicked);
        RigisterCompEvent(button_button_queding, Onbutton_quedingClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbutton_quxiaoClicked(GameObject go)
    {
        OnClickReturnBtnFun();
    }
    private void Onbutton_quedingClicked(GameObject go)
    {
        CloseUIForm();
        WebSocketAgent.Ins.ResetWebsocket();
        LoadSceneType sceneType = (LoadSceneType)ManageMentClass.DataManagerClass.SceneID;
        switch (sceneType)
        {
            case LoadSceneType.parlorScene:
                Singleton<ParlorSyncNetView>.Instance.LeaveRoom();
                break;
            case LoadSceneType.dogScene:
            case LoadSceneType.ShelterScene:
                break;
            case LoadSceneType.TreasureDigging:
                Singleton<RoomSyncNetView>.Instance.LeaveRoom();
                break;
            case LoadSceneType.RainbowBeach:
                Singleton<RainbowBeachSyncNetView>.Instance.LeaveRoom();
                break;
            case LoadSceneType.ShenMiHaiWan:
                Singleton<RainbowIocnSyncNetView>.Instance.LeaveRoom();
                break;
        }
        UIManager.GetInstance().ShowUIForms(FormConst.LOGINLOADINGPANEL);

        //OpenUIForm(FormConst.WAITNETLOADING);
        //ReceiveMessage(WebSocketConst.WsNet_OnReConnect, OnReConnetHandle);
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

    private void OnReConnetHandle(KeyValuesUpdate kv)
    {
        RemoveMsgListener(WebSocketConst.WsNet_OnReConnect, OnReConnetHandle);
        UIManager.GetInstance().CloseUIForms(FormConst.WAITNETLOADING);
    }
    private void OnClickReturnBtnFun()
    {
        //点击了退出
        try
        {
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }

}
