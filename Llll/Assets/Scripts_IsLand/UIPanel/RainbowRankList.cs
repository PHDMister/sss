//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using CircularScrollView;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowRankList : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_btn_icon;
    private Button button_btn_back;
    private ScrollRect scrollrect_ScrollView;
    private Image img_rank_bg;
    private Image img_head;
    private Text text_txt_rank;
    private Text text_name;
    private Text text_gongyizhi;
    private Button button_btn_close;
    private UICircularScrollView scrollView;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_btn_icon = FindComp<Button>("raw_bg/btn_icon");
        button_btn_back = FindComp<Button>("raw_bg/btn_back");
        scrollrect_ScrollView = FindComp<ScrollRect>("raw_bg/ScrollView");
        img_rank_bg = FindComp<Image>("raw_bg/bottomBar/img_rank_bg");
        text_txt_rank = FindComp<Text>("raw_bg/bottomBar/txt_rank");
        text_name = FindComp<Text>("raw_bg/bottomBar/name");
        text_gongyizhi = FindComp<Text>("raw_bg/bottomBar/gongyizhi");
        button_btn_close = FindComp<Button>("btn_close");
        img_head = FindComp<Image>("raw_bg/bottomBar/head/headImg");
        scrollView = scrollrect_ScrollView.GetComponent<UICircularScrollView>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_icon, Onbtn_iconClicked);
        RigisterCompEvent(button_btn_back, Onbtn_backClicked);
        RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
    }


    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_iconClicked(GameObject go)
    {
        OpenUIForm(FormConst.RAINBOWRANKLISTTIP);
    }
    private void Onbtn_backClicked(GameObject go)
    {
        CloseUIForm();
    }
    private void OnScrollViewValueChanged(Vector2 arg)
    {

    }
    private void Onbtn_closeClicked(GameObject go)
    {

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

        button_btn_close.gameObject.SetActive(false);

        scrollView.Init(OnInitCallBack);
        int sCount = Singleton<RainbowBeachDataModel>.Instance.RankList.Count;
        scrollView.ResetScrollRect();
        scrollView.ShowList(sCount);


        UpdateBottomBarInfo();
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

    private void OnInitCallBack(GameObject arg1, int arg2)
    {
        int dataIndex = arg2 - 1;
        WelfareData data = Singleton<RainbowBeachDataModel>.Instance.RankList[dataIndex];
        RankItem item = arg1.GetComponent<RankItem>();
        item.SetData(data);
    }

    protected void UpdateBottomBarInfo()
    {
        WelfareData data = Singleton<RainbowBeachDataModel>.Instance.GetSelfData();
        WelfareInfoResp resp = PeekBlackData<WelfareInfoResp>("WelfareInfoResp1");
        if (data == null || data.Index > 100)
        {
            SetRankIcon(1000);
            text_txt_rank.text = "未上榜";
            string name = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            text_name.text = TextTools.setCutAddString(name, 8, "...");
            text_gongyizhi.text = "当前：" + resp.Count;
        }
        else
        {
            SetRankIcon(data.Index);
            text_txt_rank.text = data.Index <= 3 ? "" : data.Index.ToString();
            string name1 = ManageMentClass.DataManagerClass.selfPersonData.login_name;
            text_name.text = TextTools.setCutAddString(name1, 8, "...");
            text_gongyizhi.text = "当前：" + data.WelfareCount;
        }
        LoadHead();
    }
    private void SetRankIcon(uint no)
    {
        if (no == 1) SetIcon(img_rank_bg, "RankList", "rank_bg_1");
        else if (no == 2) SetIcon(img_rank_bg, "RankList", "rank_bg_2");
        else if (no == 3) SetIcon(img_rank_bg, "RankList", "rank_bg_3");
        img_rank_bg.gameObject.SetActive(no <= 3);
    }

    protected void LoadHead()
    {
        if (ManageMentClass.DataManagerClass.Head_Texture != null)
        {
            img_head.sprite = ManageMentClass.DataManagerClass.Head_Texture;
        }
        else
        {
            string url = ManageMentClass.DataManagerClass.selfPersonData.user_pic_url;
            MessageManager.GetInstance().DownLoadAvatar(url, (sprite) =>
            {
                img_head.sprite = sprite;
                ManageMentClass.DataManagerClass.Head_Texture = sprite;
            });
        }
    }
}
