//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachReward : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_Text_NameCount;
    private Button button_btn_confirm;
    private Image Image_ibg_1;
    private Image img_itemIcon;

    private GameObject Image_item_mask;
    private Image Image_itemIcon;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_Text_NameCount = FindComp<Text>("Image_bg/Text_NameCount");
        button_btn_confirm = FindComp<Button>("Image_bg/btn_confirm");
        img_itemIcon = FindComp<Image>("Image_bg/Image_item");
        Image_ibg_1 = FindComp<Image>("Image_bg/Image_ibg_1");

        Image_item_mask = transform.Find("Image_bg/Image_item_mask").gameObject;
        Image_itemIcon = FindComp<Image>("Image_bg/Image_item_mask/Image_item_1");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_confirm, Onbtn_confirmClicked);
    }   // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_confirmClicked(GameObject go)
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

        //BeachShellResp data = Singleton<RainbowBeachController>.Instance.RewardInfo;
        BeachShellResp data = PopBlackData<BeachShellResp>("RainbowRewardInfo");
        //初始化奖励信息
        if (data.RewardType == 1) //实物
        {
            Image_ibg_1.gameObject.SetActive(false);
            img_itemIcon.gameObject.SetActive(false);
            Image_item_mask.gameObject.SetActive(true);
            Image_itemIcon.gameObject.SetActive(true);
            MessageManager.GetInstance().DownLoadPicture(data.RewardIcon, (sprite) => Image_itemIcon.sprite = sprite, null);
            text_Text_NameCount.text = data.RewardName;
        }
        else if (data.RewardType == 2 || data.RewardType == 5)//藏品  5=藏品兑换卷
        {
            Image_ibg_1.gameObject.SetActive(false);
            img_itemIcon.gameObject.SetActive(false);
            Image_item_mask.gameObject.SetActive(true);
            Image_itemIcon.gameObject.SetActive(true);
            Image_itemIcon.sprite = null;
            MessageManager.GetInstance().DownLoadPicture(data.RewardIcon, (sprite) => Image_itemIcon.sprite = sprite, null);
            text_Text_NameCount.text = data.RewardName;
        }
        else if (data.RewardType == 3)//贝壳
        {
            Image_ibg_1.gameObject.SetActive(true);
            img_itemIcon.gameObject.SetActive(true);
            Image_item_mask.gameObject.SetActive(false);
            Image_itemIcon.gameObject.SetActive(false);
            const string rewardTemp = "{0}x{1}";
            text_Text_NameCount.text = string.Format(rewardTemp, data.RewardName, data.RewardQuantity);
            Singleton<BagMgr>.Instance.AddShellNum((int)data.RewardQuantity);
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
