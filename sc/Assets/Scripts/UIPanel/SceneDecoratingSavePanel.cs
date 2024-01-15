//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class SceneDecoratingSavePanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_quxiao;
    private Button button_button_queding;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_quxiao = transform.Find("weishiyong/buttons/button-quxiao").GetComponent<Button>();
        button_button_queding = transform.Find("weishiyong/buttons/button-queding").GetComponent<Button>();

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
        CloseUIForm();
        UIManager.GetInstance().CloseUIForms(FormConst.SceneEditorPanel);
        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
        {
            OpenUIForm(FormConst.RESCUESTATION);
        }
        else
        {
            OpenUIForm(FormConst.PETDENS_UIFORM);
        }
    }

    private void Onbutton_quedingClicked(GameObject go)
    {
        //保存房间
        MessageManager.GetInstance().RequestUseRoomBuy(ChangeRoomManager.Instance().GetTempRoomID(), () =>
        {
            ChangeRoomManager.Instance().SaveUseRoomDataFun(ChangeRoomManager.Instance().GetTempRoomID());
            ChangeRoomManager.Instance().SaveRoomFun();
            SendMessage("RefreshDogDecoraterPanelScroll", "data", 1);
            PetSpanManager.Instance().SetAllObjRestPosFun();
            ToastManager.Instance.ShowNewToast("保存成功", 5f);
            CloseUIForm();
            UIManager.GetInstance().CloseUIForms(FormConst.SceneEditorPanel);
            if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
            {
                OpenUIForm(FormConst.RESCUESTATION);
            }
            else
            {
                OpenUIForm(FormConst.PETDENS_UIFORM);
            }
        });
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
    }

}
