//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using CircularScrollView;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachShareShells : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Toggle toggle_tab1;
    private Toggle toggle_tab2;
    private Toggle toggle_tab3;
    private Text text_txt_process;
    private Slider slider_slider;
    private Text text_txt_slider_word;
    private Text text_share_word;
    private Text text_share_word_1;
    private Button button_btn_send;
    private Toggle toggle_tab_room_all;
    private Toggle toggle_tab_room_cur;
    private ScrollRect scrollrect_ScrollView1;
    private ScrollRect scrollrect_ScrollView2;
    private Text text_Text;
    private Button button_btn_close;
    private Transform RedTrans;
    private Transform view_1;
    private Transform view_2;
    private Transform view_3;

    private UICircularScrollView ScrollView_AllRoom;
    private UICircularScrollView ScrollView_CurRoom;

    const string temp = "<color=#C8A000>彩虹沙滩-{0}</color>【<color=#00BBB7>{1}</color>】发贝壳啦！";
    private int mainTab = 0;  //1=发贝壳  2=抢贝壳  3=规则
    private int view2Tab = 0; //1=全部房间  2=当前房间
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        toggle_tab1 = FindComp<Toggle>("ri_bg/tab_ver/tab1");
        toggle_tab2 = FindComp<Toggle>("ri_bg/tab_ver/tab2");
        toggle_tab3 = FindComp<Toggle>("ri_bg/tab_ver/tab3");
        text_txt_process = FindComp<Text>("ri_bg/context/ct_view1/txt_process");
        slider_slider = FindComp<Slider>("ri_bg/context/ct_view1/slider");
        text_txt_slider_word = FindComp<Text>("ri_bg/context/ct_view1/slider/txt_slider_word");
        text_share_word = FindComp<Text>("ri_bg/context/ct_view1/txt_share_word");
        text_share_word_1 = FindComp<Text>("ri_bg/context/ct_view1/txt_share_word_1");
        button_btn_send = FindComp<Button>("ri_bg/context/ct_view1/btn_send");
        toggle_tab_room_all = FindComp<Toggle>("ri_bg/context/ct_view2/tab_room_all");
        toggle_tab_room_cur = FindComp<Toggle>("ri_bg/context/ct_view2/tab_room_cur");
        scrollrect_ScrollView1 = FindComp<ScrollRect>("ri_bg/context/ct_view2/ScrollView1");
        scrollrect_ScrollView2 = FindComp<ScrollRect>("ri_bg/context/ct_view2/ScrollView2");
        text_Text = FindComp<Text>("ri_bg/context/ct_view3/Text");
        button_btn_close = FindComp<Button>("ri_bg/btn_close");
        view_1 = transform.Find("ri_bg/context/ct_view1");
        view_2 = transform.Find("ri_bg/context/ct_view2");
        view_3 = transform.Find("ri_bg/context/ct_view3");
        RedTrans = toggle_tab2.transform.Find("red");

        ScrollView_AllRoom = scrollrect_ScrollView1.GetComponent<UICircularScrollView>();
        ScrollView_CurRoom = scrollrect_ScrollView2.GetComponent<UICircularScrollView>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        toggle_tab1.onValueChanged.AddListener(Ontab1ValueChanged);
        toggle_tab2.onValueChanged.AddListener(Ontab2ValueChanged);
        toggle_tab3.onValueChanged.AddListener(Ontab3ValueChanged);
        RigisterCompEvent(button_btn_send, Onbtn_sendClicked);
        toggle_tab_room_all.onValueChanged.AddListener(Ontab_room_allValueChanged);
        toggle_tab_room_cur.onValueChanged.AddListener(Ontab_room_curValueChanged);
        RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Ontab1ValueChanged(bool arg)
    {
        if (arg) mainTab = 1;
        view_1.gameObject.SetActive(arg);
    }
    private void Ontab2ValueChanged(bool arg)
    {
        if (arg) mainTab = 2;
        view_2.gameObject.SetActive(arg);
        toggle_tab_room_cur.isOn = arg;
        Ontab_room_allValueChanged(!arg);
        Ontab_room_curValueChanged(arg);
    }
    private void Ontab3ValueChanged(bool arg)
    {
        if (arg) mainTab = 3;
        view_3.gameObject.SetActive(arg);
    }
    private void Onbtn_sendClicked(GameObject go)
    {
        uint freeCount = Singleton<RainbowBeachDataModel>.Instance.FreeShovleCount;
        if (freeCount < 10) return;
        SendHongbaoReq req = new SendHongbaoReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.Ins.Send((uint)MessageId.Types.Enum.SendHongbaoReq, req, (code, dataString) =>
        {
            if (code == 230021)
            {
                ToastManager.Instance.ShowNewToast("游戏次数小于10不能发红包", 2);
                return;
            }

            ToastManager.Instance.ShowNewToast("贝壳红包发送成功，快去试试手气吧~", 2);
            Singleton<RainbowBeachDataModel>.Instance.SetFreeShovleCount(0);
            UpdateView1();
            UpdateView2();
        });
    }
    private void Ontab_room_allValueChanged(bool arg)
    {
        ScrollView_AllRoom.gameObject.SetActive(arg);
        if (arg)
        {
            view2Tab = 1;
            int count = Singleton<RainbowBeachDataModel>.Instance.TempAllRoomDatas.Count;
            ScrollView_AllRoom.ShowList(count);
            ScrollView_AllRoom.ResetScrollRect();
        }
        else
        {
            ScrollView_AllRoom.ShowList(0);
        }
    }
    private void Ontab_room_curValueChanged(bool arg)
    {
        ScrollView_CurRoom.gameObject.SetActive(arg);
        if (arg)
        {
            view2Tab = 2;
            int count = Singleton<RainbowBeachDataModel>.Instance.TempCurRoomDatas.Count;
            ScrollView_CurRoom.ShowList(count);
            ScrollView_CurRoom.ResetScrollRect();
        }
        else
        {
            ScrollView_CurRoom.ShowList(0);
        }
    }
    private void Onbtn_closeClicked(GameObject go)
    {
        CloseUIForm();
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;

        ScrollView_AllRoom.Init(OnAllRoomHandle);
        ScrollView_CurRoom.Init(OnCurRoomHandle);

        ReceiveMessage("UpdateSendAllRoom", kv => UpdateView2());
    }

    public override void Display()
    {
        base.Display();
        view_1.gameObject.SetActive(true);
        view_2.gameObject.SetActive(false);
        view_3.gameObject.SetActive(false);

        ScrollView_AllRoom.gameObject.SetActive(false);
        ScrollView_CurRoom.gameObject.SetActive(false);

        toggle_tab1.isOn = true;

        UpdateView1();
        UpdateView2();

        //显示红点
        //bool hongdian = Singleton<RainbowBeachDataModel>.Instance.OtherShareHongbao;
        RedTrans.gameObject.SetActive(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        mainTab = 0;
        view2Tab = 0;
    }

    public override void Redisplay()
    {
        base.Redisplay();
        if (mainTab == 1) ScrollView_AllRoom.UpdateList();
        if (mainTab == 2) ScrollView_CurRoom.UpdateList();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    //View1
    protected void UpdateView1()
    {
        uint freeCount = Singleton<RainbowBeachDataModel>.Instance.FreeShovleCount;
        slider_slider.value = freeCount / 10.0f;
        text_txt_slider_word.text = freeCount + "/10";
        button_btn_send.interactable = freeCount >= 10;
        text_share_word.gameObject.SetActive(freeCount < 10);
        text_share_word_1.gameObject.SetActive(freeCount >= 10);
    }
    //View2
    protected void UpdateView2()
    {
        //全部房间
        SendAllRoomReq();
        //当前房间
        SendCurRoomReq();
    }
    protected void SendAllRoomReq()
    {
        HongbaoListReq req = new HongbaoListReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        req.Page = 1;
        req.PageSize = 100;
        req.RoomId = 0;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.HongbaoListReq, req, (code, data) =>
        {
            HongbaoListResp resp = HongbaoListResp.Parser.ParseFrom(data);
            Singleton<RainbowBeachDataModel>.Instance.HongbaoListAllRoomDatas(resp.List);
            if (mainTab == 2 && view2Tab == 1) ScrollView_AllRoom.ShowList(resp.List.Count);
        });
    }
    protected void SendCurRoomReq()
    {
        HongbaoListReq req1 = new HongbaoListReq();
        req1.UserId = ManageMentClass.DataManagerClass.userId;
        req1.Page = 1;
        req1.PageSize = 100;
        req1.RoomId = ManageMentClass.DataManagerClass.roomId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.HongbaoListReq, req1, (code, data) =>
        {
            HongbaoListResp resp = HongbaoListResp.Parser.ParseFrom(data);
            Singleton<RainbowBeachDataModel>.Instance.HongbaoListCurRoomDatas(resp.List);
            if (mainTab == 2 && view2Tab == 2) ScrollView_CurRoom.ShowList(resp.List.Count);
        });
    }
    protected void OnAllRoomHandle(GameObject go, int index)
    {
        int dIndex = index - 1;
        HongbaoData data = Singleton<RainbowBeachDataModel>.Instance.GetAllRoomData(dIndex);
        Text context = FindComp<Text>(go.transform, "txtContent");
        Text timeStr = FindComp<Text>(go.transform, "txtTime");
        GameObject expireGo = go.transform.Find("txtStateExpire").gameObject;
        GameObject imgGo = go.transform.Find("txtStateIng").gameObject;

        string name = TextTools.setCutAddString(data.OwnerName, 8, "...");
        context.text = string.Format(temp, data.RoomId, name);

        long endTimestamp = data.CreatedAt + 3600;
        //如果当前时间大于发红包时间  则显示日期    显示：已过期
        if (endTimestamp <= ManageMentClass.DataManagerClass.CurTime)
        {
            DateTime dateTime = CalcTools.TimeStampChangeDateTimeFun(data.CreatedAt);
            string dateStr = dateTime.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            timeStr.text = dateStr;
            expireGo.SetActive(true);
            imgGo.SetActive(false);
        }
        //如果当前时间小于发红包1小时 则显示倒计时   显示：进行中
        else
        {
            SimpleCountDown scd = timeStr.GetComponent<SimpleCountDown>();
           // scd.SetEndTime(endTimestamp, dIndex, "倒计时：", (idx) => ScrollView_AllRoom.UpdateCellByIndex(idx));
            expireGo.SetActive(false);
            imgGo.SetActive(true);
        }
    }
    protected void OnCurRoomHandle(GameObject go, int index)
    {
        int dIndex = index - 1;
        HongbaoData data = Singleton<RainbowBeachDataModel>.Instance.GetCurRoomData(dIndex);
        if (data == null) return;

        Text context = FindComp<Text>(go.transform, "txtContent");
        Text timeStr = FindComp<Text>(go.transform, "txtTime");
        Button btnExpire = FindComp<Button>(go.transform, "btnStateExpire");
        Button btnImg = FindComp<Button>(go.transform, "btnStateIng");
        Image grabImg = FindComp<Image>(go.transform, "stateEnd");

        context.text = string.Format(temp, data.RoomId, TextTools.setCutAddString(data.OwnerName, 8, "..."));

        if (!data.HasGrab)
        {
            grabImg.gameObject.SetActive(false);
            long endTimestamp = data.CreatedAt + 3600;
            //如果当前时间大于发红包时间  则显示 已过期
            if (endTimestamp <= ManageMentClass.DataManagerClass.CurTime)
            {
                DateTime dateTime = CalcTools.TimeStampChangeDateTimeFun(data.CreatedAt);
                string dateStr = dateTime.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                timeStr.text = dateStr;
                btnExpire.interactable = false;
                btnExpire.gameObject.SetActive(true);
                btnImg.gameObject.SetActive(false);
            }
            //如果当前时间小于发红包1小时 则显示倒计时   显示：抢贝壳
            else
            {
                SimpleCountDown scd = timeStr.GetComponent<SimpleCountDown>();
              //  scd.SetEndTime(endTimestamp, dIndex, "倒计时：", (idx) => ScrollView_CurRoom.UpdateCellByIndex(idx));
                btnExpire.gameObject.SetActive(false);
                btnImg.gameObject.SetActive(true);
                btnImg.onClick.RemoveAllListeners();
                btnImg.onClick.AddListener(() => OnBtnImgClickHandle(dIndex));
            }
        }
        else
        {
            btnImg.gameObject.SetActive(false);
            btnExpire.gameObject.SetActive(false);
            grabImg.gameObject.SetActive(true);

            DateTime dateTime = CalcTools.TimeStampChangeDateTimeFun(data.CreatedAt);
            string dateStr = dateTime.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            timeStr.text = dateStr;
        }
    }
    protected void OnBtnImgClickHandle(int dataIndex)
    {
        //发送协议 抢贝壳
        HongbaoData data = Singleton<RainbowBeachDataModel>.Instance.GetCurRoomData(dataIndex);
        if (data == null) return;
        GrabHongbaoReq req = new GrabHongbaoReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        req.Id = data.Id;
        WebSocketAgent.Ins.Send((uint)MessageId.Types.Enum.GrabHongbaoReq, req);
    }

}
