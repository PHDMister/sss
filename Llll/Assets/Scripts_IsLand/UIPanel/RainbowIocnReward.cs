//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowIocnReward : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_Name;
    private Text text_Count;
    private Image img_itemIcon;
    private Button button_btn_mai;
    private Button button_btn_take;
    protected BeachShellResp data;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_Name = FindComp<Text>("Image_bg/txt_name");
        text_Count = FindComp<Text>("Image_bg/txt_count");
        img_itemIcon = FindComp<Image>("Image_bg/Image_item");

        button_btn_mai = FindComp<Button>("Image_bg/btn_mai");
        button_btn_take = FindComp<Button>("Image_bg/btn_take");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_mai, Onbutton_btn_maiClicked);
        RigisterCompEvent(button_btn_take, Onbutton_btn_takeClicked);
    }   // UI EVENT REGISTER END


    // UI EVENT FUNC START
    private void Onbutton_btn_maiClicked(GameObject go)
    {
        if (button_btn_mai.interactable == false) return;
        CloseUIForm();
        MessageManager.GetInstance().SendSellReq(data.RewardId, 1, FishSellMsgHandler);
    }

    void FishSellMsgHandler(string result, SellResp rsp)
    {
        if (result == "Success")
        {
            CloseUIForm();
            //出售成功提示
            OpenUIForm(FormConst.FISHSUCCESSPANEL);
            //提示内容
            FishSellSuccess_Panel uiForm = UIManager.GetInstance().GetUIForm(FormConst.FISHSUCCESSPANEL) as FishSellSuccess_Panel;
            if (uiForm != null) uiForm.ShowTipsContent((int)data.RewardId, rsp.SellCount, rsp.Income);
        }
        else if (result == "Fail")
        {
            CloseUIForm();
            ToastManager.Instance.ShowNewToast("出售失败~", 2);
        }
    }

    private void Onbutton_btn_takeClicked(GameObject go)
    {
        CloseUIForm();
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

        data = PopBlackData<BeachShellResp>("RainbowIocnConfirm_Reward");
        int fishCount = Singleton<AquariumDataModel>.Instance.GetFishCount((int)data.RewardId);
        button_btn_mai.interactable = fishCount >= 1;

        //初始化奖励信息
        if (data.RewardType == 1) //实物
        {
            MessageManager.GetInstance().DownLoadPicture(data.RewardIcon, (sprite) => img_itemIcon.sprite = sprite, null);
            text_Name.text = data.RewardName;
        }
        else if (data.RewardType == 2 || data.RewardType == 5)//藏品  5=藏品兑换卷
        {
            img_itemIcon.sprite = null;
            MessageManager.GetInstance().DownLoadPicture(data.RewardIcon, (sprite) => img_itemIcon.sprite = sprite, null);
            text_Name.text = data.RewardName;
        }
        else if (data.RewardType == 6) //鱼
        {
            const string rewardTemp = "【{0}】";
            island_fish fish = ManageMentClass.DataManagerClass.GetIsLandFishTable((int)data.RewardId);
            text_Count.text = fish.fish_price.ToString();
            text_Name.text = string.Format(rewardTemp, fish.fish_name);
            SetIcon(img_itemIcon, "Fish", fish.fish_icon);
            Singleton<AquariumDataModel>.Instance.UpdateFishData(data.RewardId, data.RewardQuantity);
        }
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

}
