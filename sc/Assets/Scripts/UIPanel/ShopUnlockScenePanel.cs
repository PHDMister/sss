//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class ShopUnlockScenePanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_quxiao;
    private Button button_queding;
    private Button button_quxiao1;
    private Button button_guanbi;
    private Text text_shoming1;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_quxiao = transform.Find("jiesuo-1/duihuanxuzhi/buttons/quxiao").GetComponent<Button>();
        button_queding = transform.Find("jiesuo-1/duihuanxuzhi/buttons/queding").GetComponent<Button>();
        button_quxiao1 = transform.Find("jiesuo-1/duihuanxuzhi/buttons/quxiao1").GetComponent<Button>();
        button_guanbi = transform.Find("jiesuo-1/duihuanxuzhi/buttons/guanbi").GetComponent<Button>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

        RigisterCompEvent(button_quxiao, OnquxiaoClicked);

        RigisterCompEvent(button_queding, OnquedingClicked);

        RigisterCompEvent(button_quxiao1, Onquxiao1Clicked);

        RigisterCompEvent(button_guanbi, OnguanbiClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START

    public Text GasValue_Text;

    public Text RoomName_Text;



    public int indexID;

    public bool IsOut;


    private void OnquxiaoClicked(GameObject go)
    {
        //退出

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
    private void OnquedingClicked(GameObject go)
    {
        //购买并使用

        //购买房间
        petHouse petHouse = ManageMentClass.DataManagerClass.GetPetHouseTableFun(indexID);
        if (petHouse.item_price <= ManageMentClass.DataManagerClass.gas_Amount)
        {
            MessageManager.GetInstance().RequestBuyRoomBuy(indexID, () =>
            {
                ToastManager.Instance.ShowNewToast("购买成功", 5f);
                ChangeRoomManager.Instance().SaveBuyRoomDataFun(ChangeRoomManager.Instance().GetTempRoomID());
                //保存
                MessageManager.GetInstance().RequestUseRoomBuy(indexID, () =>
                {
                    ChangeRoomManager.Instance().SaveUseRoomDataFun(ChangeRoomManager.Instance().GetTempRoomID());
                    ChangeRoomManager.Instance().SaveRoomFun();
                    PetSpanManager.Instance().SetAllObjRestPosFun();
                    ToastManager.Instance.ShowNewToast("保存成功", 5f);
                    SendMessage("RefreshDogDecoraterPanelScroll", "data", 1);
                    MessageManager.GetInstance().RequestGasValue();
                    CloseUIForm();
                });
            });
        }
        else
        {
            CloseUIForm();
            OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
        }
    }
    private void OnguanbiClicked(GameObject go)
    {
        //关闭
        CloseUIForm();
    }
    private void Onquxiao1Clicked(GameObject go)
    {
        //仅购买
        petHouse petHouse = ManageMentClass.DataManagerClass.GetPetHouseTableFun(indexID);
        if (petHouse.item_price <= ManageMentClass.DataManagerClass.gas_Amount)
        {
            MessageManager.GetInstance().RequestBuyRoomBuy(ChangeRoomManager.Instance().GetTempRoomID(), () =>
            {
                ChangeRoomManager.Instance().SaveBuyRoomDataFun(ChangeRoomManager.Instance().GetTempRoomID());
                SendMessage("RefreshDogDecoraterPanelScroll", "data", 0);
                ToastManager.Instance.ShowNewToast("购买成功", 5f);
                CloseUIForm();
            });
        }
        else
        {
            CloseUIForm();
            OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
        }
    }
    // UI EVENT FUNC END

    private string format = "是否消耗     {0} 解锁 <color=#FFF271> {1} </color> 并使用";
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
        ReceiveMessage("ShopUnlockScenePanel", p =>
        {
            DogUIOutData dogUIOutData = (DogUIOutData)p.Values;
            indexID = dogUIOutData.item_id;
            IsOut = dogUIOutData.isOut;
            SetPanelShowFun(indexID);

        });
    }
    private void SetShowButtonFun()
    {
        if (IsOut)
        {
            button_quxiao.gameObject.SetActive(true);
            button_quxiao1.gameObject.SetActive(false);
        }
        else
        {
            button_quxiao.gameObject.SetActive(false);
            button_quxiao1.gameObject.SetActive(true);
        }
    }
    private void SetPanelShowFun(int roomID)
    {
        //是否消耗     9999<color=#FDF3CD>解锁</color> < color =#FFF271> 原木小屋 </color> 
        petHouse petHouseData = ManageMentClass.DataManagerClass.GetPetHouseTableFun(roomID);
        RoomName_Text.text = string.Format("<color=#FDF3CD>解锁</color> <color=#FFF271> {0} </color> ", petHouseData.item_name);
        GasValue_Text.text = petHouseData.item_price + "";
        SetShowButtonFun();
    }
}
