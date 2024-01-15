//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;

public class SceneEditorPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_gas;
    private Button button_aixn;
    private Button button_heading_zhuangxiu;
    private ScrollRect scrollrect_scene_neirong;
    private Button button_scene_zhu;
    private Button button_button_shiyong;
    private Button button_button_buy;
    private Text text_txt_shu;
    private Text text_txt_shu9;
    private Image image_Frame_1000008046;
    private Text text_pay;
    private Text text_txt_name;
    private Text text_txt_star;
    private Image image_icon_star_explain;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_gas = transform.Find("Frame_huobiqu/gas").GetComponent<Button>();
        button_aixn = transform.Find("Frame_huobiqu/aixn").GetComponent<Button>();
        button_heading_zhuangxiu = transform.Find("heading-zhuangxiu").GetComponent<Button>();
        button_button_shiyong = transform.Find("changjingqiehuanlan/Frame_1000008046/buttons/button-shiyong").GetComponent<Button>();
        button_button_buy = transform.Find("changjingqiehuanlan/Frame_1000008046/buttons/button-buy").GetComponent<Button>();
        text_txt_shu = transform.Find("Frame_huobiqu/gas/Frame 1000008016/Frame 1000008015/txt-shu").GetComponent<Text>();
        text_txt_shu9 = transform.Find("Frame_huobiqu/aixn/Frame 1000008016/Frame 1000008015/txt-shu").GetComponent<Text>();
        image_Frame_1000008046 = transform.Find("changjingqiehuanlan/Frame_1000008046").GetComponent<Image>();
        text_txt_star = transform.Find("star-explain/txt-star").GetComponent<Text>();
        image_icon_star_explain = transform.Find("star-explain/icon-star-explain").GetComponent<Image>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

        RigisterCompEvent(button_gas, OngasClicked);

        RigisterCompEvent(button_aixn, OnaixnClicked);

        RigisterCompEvent(button_heading_zhuangxiu, Onheading_zhuangxiuClicked);

        RigisterCompEvent(button_scene_zhu, Onscene_zhuClicked);

        RigisterCompEvent(button_button_shiyong, Onbutton_shiyongClicked);

        RigisterCompEvent(button_button_buy, Onbutton_buyClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START




    public CircularScrollView.UICircularScrollView GameScroll_GameRecord;

    public Text GasValue_Text;

    public Text LoveCoin_Text;

    public GameObject BuffTips;

    private void OngasClicked(GameObject go)
    {
        Debug.Log("11111111111111111111111111");
    }

    private void OnaixnClicked(GameObject go)
    {

    }
    //退出
    public void Onheading_zhuangxiuClicked(GameObject go)
    {
        OutDecorateModelFun();

    }

    private void Onscene_neirongValueChanged(Vector2 arg)
    {

    }

    private void Onscene_zhuClicked(GameObject go)
    {

    }

    private void Onbutton_shiyongClicked(GameObject go)
    {
        //保存数据

        //保存房间
        MessageManager.GetInstance().RequestUseRoomBuy(ChangeRoomManager.Instance().GetTempRoomID(), () =>
        {
            ChangeRoomManager.Instance().SaveUseRoomDataFun(ChangeRoomManager.Instance().GetTempRoomID());
            ChangeRoomManager.Instance().SaveRoomFun();
            PetSpanManager.Instance().SetAllObjRestPosFun();
            OnClickSetEditorFun(ChangeRoomManager.Instance().DogDataListIndex - 1);
            ToastManager.Instance.ShowNewToast("保存成功", 5f);
        });
    }
    private void Onbutton_buyClicked(GameObject go)
    {
        OpenUIForm(FormConst.SHOPUNLOCKSCENEPANEL);

        DogUIOutData dogUIOutData = new DogUIOutData(false, ChangeRoomManager.Instance().GetTempRoomID());

        SendMessage("ShopUnlockScenePanel", "data", dogUIOutData);
    }
    // UI EVENT FUNC END

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        UIManager.GetInstance().CloseUIForms(FormConst.HUD);
        ChangeRoomManager.Instance().IntoDecoratingModelFun();
        GameScroll_GameRecord.UpdateList();
        OnClickSetEditorFun(ChangeRoomManager.Instance().DogDataListIndex - 1);
        RefreshGasAndCoinFun();
        button_button_buy.gameObject.SetActive(false);
        button_button_shiyong.gameObject.SetActive(false);
    }
    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        ChangeRoomManager.Instance().OutDecoratingModelFun();
    }

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
        GameScroll_GameRecord.Init(NormalCallBack);
        GameScroll_GameRecord.ShowList(ChangeRoomManager.Instance().AllDogRoomItemData.Count);

        ReceiveMessage("ClickDogDecorateItem", p =>
        {
            int index = (int)p.Values;
            ChangeRoomManager.Instance().DogDataListIndex = index;
            Debug.Log("输出一下点击得Itemn");
            ChangeRoomManager.Instance().ChangeRoomDogBgFun(ChangeRoomManager.Instance().AllDogRoomItemData[index - 1].item_id, true);
            OnClickSetEditorFun(index - 1);
        });
        ReceiveMessage("RefreshDogDecoraterPanelScroll", p =>
        {
            GameScroll_GameRecord.UpdateList();
            RefreshGasAndCoinFun();
            OnClickSetEditorFun(ChangeRoomManager.Instance().DogDataListIndex - 1);
        });
        ReceiveMessage("UpdataGasValue", p =>
        {
            RefreshGasAndCoinFun();
        });

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
        SetRoomData(cell, index);
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="index"></param>
    public void SetRoomData(GameObject cell, int index)
    {
        DogDecorateItem uiItem = cell.GetComponent<DogDecorateItem>();
        //这里面要填写 物品ID   目前测试先传一个index
        uiItem.SetDogDecorateItemDataFun(index);
    }
    /// <summary>
    /// 退出装修模式
    /// </summary>
    private void OutDecorateModelFun()
    {
        if (ChangeRoomManager.Instance().IsSaveRoomFun())
        {
            // 已经保存过了
            CloseUIForm();
            if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
            {
                OpenUIForm(FormConst.RESCUESTATION);
            }
            else
            {
                OpenUIForm(FormConst.PETDENS_UIFORM);
            }
        }
        else
        {
            //未保存

            if (ChangeRoomManager.Instance().IsHaveRoomFun(ChangeRoomManager.Instance().DogDataListIndex - 1))
            {
                //已经购买了
                OpenUIForm(FormConst.SceneDecoratingSavePanel);
            }
            else
            {
                //未购买
                OpenUIForm(FormConst.SHOPUNLOCKSCENEPANEL);
                DogUIOutData dogUIOutData = new DogUIOutData(true, ChangeRoomManager.Instance().GetTempRoomID());
                SendMessage("ShopUnlockScenePanel", "data", dogUIOutData);
            }
        }
    }
    /// <summary>
    /// 根据点击设置
    /// </summary>
    private void OnClickSetEditorFun(int Id)
    {

        DogRoomItemData itemData = ChangeRoomManager.Instance().AllDogRoomItemData[Id];
        petHouse petHouse = ManageMentClass.DataManagerClass.GetPetHouseTableFun(itemData.item_id);
        if (petHouse.item_effect != 0)
        {
            BuffTips.SetActive(true);
        }
        else
        {
            BuffTips.SetActive(false);
        }

        if (ChangeRoomManager.Instance().AllDogRoomItemData[Id].item_default != 0)
        {
            //默认

            button_button_buy.gameObject.SetActive(false);

            if (ChangeRoomManager.Instance().AllDogRoomItemData[Id].is_select == 1)
            {
                button_button_shiyong.gameObject.SetActive(false);
            }
            else
            {
                button_button_shiyong.gameObject.SetActive(true);
            }
        }
        else
        {
            // 非默认
            if (ChangeRoomManager.Instance().AllDogRoomItemData[Id].is_buy == 0)
            {
                button_button_buy.gameObject.SetActive(true);
                button_button_shiyong.gameObject.SetActive(false);
            }
            else
            {
                button_button_buy.gameObject.SetActive(false);
                if (ChangeRoomManager.Instance().AllDogRoomItemData[Id].is_select == 1)
                {
                    //被选中了
                    button_button_shiyong.gameObject.SetActive(false);
                }
                else
                {
                    //未选中
                    button_button_shiyong.gameObject.SetActive(true);
                }
            }
        }
    }


    public void RefreshGasAndCoinFun()
    {
        GasValue_Text.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
        LoveCoin_Text.text = InterfaceHelper.GetCoinDisplay(ManageMentClass.DataManagerClass.loveCoin);
    }

}
