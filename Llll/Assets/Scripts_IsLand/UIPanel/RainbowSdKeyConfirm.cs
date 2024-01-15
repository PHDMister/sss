//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using Google.Protobuf;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowSdKeyConfirm : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Toggle toggle_Image_Item1;
    private Text text_TextName;
    private Text text_TextCount;
    private Toggle toggle_Image_Item2;
    private Text text_TextName6;
    private Text text_TextCount7;
    private Button button_btn_quxiao;
    private Button button_btn_start;
    private Button button_btn_close;
    protected int UseItemType = 0;
    protected int Item1 = 50001;
    protected int Item2 = 50002;

    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        toggle_Image_Item1 = FindComp<Toggle>("img_bg/Image_Item1");
        toggle_Image_Item2 = FindComp<Toggle>("img_bg/Image_Item2");
        button_btn_quxiao = FindComp<Button>("img_bg/btn_quxiao");
        button_btn_start = FindComp<Button>("img_bg/btn_start");
        button_btn_close = FindComp<Button>("btn_close");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {

        toggle_Image_Item1.onValueChanged.AddListener(OnImage_Item1ValueChanged);
        toggle_Image_Item2.onValueChanged.AddListener(OnImage_Item2ValueChanged);
        RigisterCompEvent(button_btn_quxiao, Onbtn_quxiaoClicked);
        RigisterCompEvent(button_btn_start, Onbtn_startClicked);
        RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
    }

    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnImage_Item1ValueChanged(bool value)
    {
        if (value)
        {
            UseItemType = Item1;
            BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
            if (bgItem.Count <= 0) UseItemType = 0;
        }
        else
        {
            UseItemType = 0;
        }
    }
    private void OnImage_Item2ValueChanged(bool value)
    {
        if (value)
        {
            UseItemType = Item2;
            BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
            if (bgItem.Count <= 0) UseItemType = 0;
        }
        else
        {
            UseItemType = 0;
        }
    }
    private void Onbtn_quxiaoClicked(GameObject go)
    {
        CloseUIForm();
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(true);
    }
    private void Onbtn_startClicked(GameObject go)
    {
        if (UseItemType == 0)
        {
            ToastManager.Instance.ShowNewToast("请选择一种钥匙", 2);
            return;
        }
        BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
        Debug.Log($"111111111111111 RainbowShallConfirm  OnBtnStartClicked  select  UseItemType={UseItemType}  {bgItem.Name}  {bgItem.Count}");
        //检测数量
        if (bgItem.Count <= 0)
        {
            ToastManager.Instance.ShowNewToast($"{bgItem.Name} 数量不足", 2);
            return;
        }
        uint curBoxId = Convert.ToUInt32(PopBlackData("RewardBox_CurBoxId"));
        BeachShellReq shellReq = new BeachShellReq();
        shellReq.UserId = ManageMentClass.DataManagerClass.userId;
        shellReq.ToolId = (uint)UseItemType;
        shellReq.Box = curBoxId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.BeachShellReq, shellReq, OnBeachShellResp);
    }
    private void Onbtn_closeClicked(GameObject go)
    {
        CloseUIForm();
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(true);
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

    }

    public override void Display()
    {
        base.Display();
        UseItemType = 0;
        InitItem(toggle_Image_Item1.gameObject, Item1);
        InitItem(toggle_Image_Item2.gameObject, Item2);
    }

    public override void Hiding()
    {
        base.Hiding();
        toggle_Image_Item1.isOn = false;
        toggle_Image_Item2.isOn = false;
    }

    public override void Redisplay()
    {
        base.Redisplay();
        InitItem(toggle_Image_Item1.gameObject, Item1);
        InitItem(toggle_Image_Item2.gameObject, Item2);
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    protected void InitItem(GameObject go, int itemId)
    {
        Transform parent = go.transform;
        Text TextName = FindComp<Text>(parent, "TextName");
        Text TextCount = FindComp<Text>(parent, "TextCount");
        Image icon = FindComp<Image>(parent, "Imageicon");
        Button btn = FindComp<Button>(parent, "Image_no");
        //赋值
        BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(itemId);
        const string temp = "拥有：{0}";

        SetIcon(icon, "ShellIcon", bgItem.Icon);
        TextName.text = bgItem.Name;
        TextCount.text = string.Format(temp, bgItem.Count);

        //goto shop
        btn.gameObject.SetActive(bgItem.Count <= 0);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            Debug.Log("点击了 跳转到商店");
            OpenUIForm(FormConst.RAINBOWBEACHSHOPPANEL);
            MessageCenter.SendMessage("OnRainbowBeachShopPanelShow", new KeyValuesUpdate("", new RainbowBeachShopPanelParam
            {
                tab = ShopTab.yaoshi
            }));
        });
    }

    protected void OnBeachShellResp(int code, ByteString data)
    {
        CloseUIForm();
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(true);

        BeachShellResp resp = BeachShellResp.Parser.ParseFrom(data);
        if (resp.StatusCode == 230027 || resp.StatusCode > 0)
        {
            ToastManager.Instance.ShowNewToast("宝箱已被他人开启", 3);
            return;
        }
       
        //更新背包道具数量a
        Singleton<BagMgr>.Instance.UpdateItem(resp.ToolId, resp.ToolNum);
        
        int curBoxId = Singleton<RainbowSeabedController>.Instance.CurBoxId;
        SeabedRewardBox rewardBox = SeabedRewardBox.GetBox(curBoxId);
        rewardBox.OpenBox(() =>
        {
            //弹出奖励
            AddBlackData("RainbowRewardInfo", resp);
            //4=海洋垃圾
            if (resp.RewardType == 4) OpenUIForm(FormConst.RAINBOWBEACHWASTE);
            //123 正常奖励
            else OpenUIForm(FormConst.RAINBOWBEACHREWARD);

            //刷新宝箱
            Singleton<RainbowSeabedController>.Instance.UpdateSeabedBox(resp.Boxs); 

        });
    }
}