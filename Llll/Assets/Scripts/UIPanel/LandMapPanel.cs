//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using UnityEngine.SceneManagement;

public class LandMapPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_Seabed_Btn;
    private Image seabed_NoSelect_Img;
    private Image seabed_Select_Img;
    private Button button_Sandbeach_Btn;
    private Image sandbeach_NoSelect_Img;
    private Image sandbeach_Select_Img;
    private Button button_Gulf_Btn;
    private Image gulf_NoSelect_Img;
    private Image gulf_Select_Img;
    private Button button_Close_Btn;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_Seabed_Btn = FindComp<Button>("Seabed_Btn");
        seabed_NoSelect_Img = FindComp<Image>("Seabed_Btn/NoSelect_Img");
        seabed_Select_Img = FindComp<Image>("Seabed_Btn/Select_Img");
        button_Sandbeach_Btn = FindComp<Button>("Sandbeach_Btn");
        sandbeach_NoSelect_Img = FindComp<Image>("Sandbeach_Btn/NoSelect_Img");
        sandbeach_Select_Img = FindComp<Image>("Sandbeach_Btn/Select_Img");
        button_Gulf_Btn = FindComp<Button>("Gulf_Btn");
        gulf_NoSelect_Img = FindComp<Image>("Gulf_Btn/NoSelect_Img");
        gulf_Select_Img = FindComp<Image>("Gulf_Btn/Select_Img");
        button_Close_Btn = FindComp<Button>("Close_Btn");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END



    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Seabed_Btn, OnSeabed_BtnClicked);
        RigisterCompEvent(button_Sandbeach_Btn, OnSandbeach_BtnClicked);
        RigisterCompEvent(button_Gulf_Btn, OnGulf_BtnClicked);
        RigisterCompEvent(button_Close_Btn, OnClose_BtnClicked);
    }


    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnSeabed_BtnClicked(GameObject go)
    {
        //ToastManager.Instance.ShowNewToast("海底星空正在建设中，敬请期待~", 3);
        if (SceneManager.GetActiveScene().name == "Seabed01")
        {
            ToastManager.Instance.ShowNewToast("已在海底星空，快去看看吧～", 3);
        }
        else
        {
            GoToHaiDiXingKong();
        }
    }
    private void OnSandbeach_BtnClicked(GameObject go)
    {
        if (SceneManager.GetActiveScene().name == "Island01")
        {
            ToastManager.Instance.ShowNewToast("已在彩虹沙滩，快去看看吧～", 3);
        }
        else
        {
            GoToRainbowBeach();
        }
    }
    private void OnGulf_BtnClicked(GameObject go)
    {
        //ToastManager.Instance.ShowNewToast("神秘海湾正在建设中，敬请期待～", 3);
        if (SceneManager.GetActiveScene().name == "fish")
        {
            ToastManager.Instance.ShowNewToast("已在神秘海湾，快去看看吧～", 3);
        }
        else
        {
            GoToShenMiHaiWan();
        }
    }
    private void OnClose_BtnClicked(GameObject go)
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
        RefreshBtnUIFun();
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

    public void RefreshBtnUIFun()
    {
        seabed_NoSelect_Img.gameObject.SetActive(true);
        seabed_Select_Img.gameObject.SetActive(false);

        sandbeach_NoSelect_Img.gameObject.SetActive(true);
        sandbeach_Select_Img.gameObject.SetActive(false);

        gulf_NoSelect_Img.gameObject.SetActive(true);
        gulf_Select_Img.gameObject.SetActive(false);
        switch (SceneManager.GetActiveScene().name)
        {
            case "Island01":
                sandbeach_NoSelect_Img.gameObject.SetActive(false);
                sandbeach_Select_Img.gameObject.SetActive(true);
                break;
            case "fish":
                gulf_NoSelect_Img.gameObject.SetActive(false);
                gulf_Select_Img.gameObject.SetActive(true);
                break;
            case "Seabed01":
                seabed_NoSelect_Img.gameObject.SetActive(false);
                seabed_Select_Img.gameObject.SetActive(true);
                break;
        }

    }



    //彩虹沙滩
    void GoToRainbowBeach()
    {
        LeaveRoom();
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.RainbowBeach);
        CloseUIForm();
    }
    //神秘海湾
    void GoToShenMiHaiWan()
    {
        LeaveRoom();
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.ShenMiHaiWan);
        CloseUIForm();
    }
    //海底星空
    void GoToHaiDiXingKong()
    {
        LeaveRoom();
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.HaiDiXingKong);
        CloseUIForm();
        //ToastManager.Instance.ShowNewToast("敬请期待~", 2);
    }

    private void LeaveRoom()
    {
        if (SceneManager.GetActiveScene().name == "Island01")
            Singleton<RainbowBeachSyncNetView>.Instance.LeaveRoom();
        if (SceneManager.GetActiveScene().name == "fish")
            Singleton<RainbowIocnSyncNetView>.Instance.LeaveRoom();
        if (SceneManager.GetActiveScene().name == "Seabed01")
            Singleton<RainbowSeabedSyncNetView>.Instance.LeaveRoom();
    }
}
