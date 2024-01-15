//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using Google.Protobuf;
using Treasure;
using UnityEngine.UI;
using UIFW;
using Text = UnityEngine.UI.Text;

public class RainbowShallConfirm : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private GameObject go_Image_Item1;
    private GameObject go_Image_Item2;
    private GameObject go_Image_Item3;
    private Toggle toggle_Item1;
    private Toggle toggle_Item2;
    private Toggle toggle_Item3;
    private Button btn_close;
    private Button btn_quxiao;
    private Button btn_start;
    protected int UseItemType = 0;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        go_Image_Item1 = transform.Find("img_bg/Image_Item1").gameObject;
        go_Image_Item2 = transform.Find("img_bg/Image_Item2").gameObject;
        go_Image_Item3 = transform.Find("img_bg/Image_Item3").gameObject;

        toggle_Item1 = go_Image_Item1.GetComponent<Toggle>();
        toggle_Item2 = go_Image_Item2.GetComponent<Toggle>();
        toggle_Item3 = go_Image_Item3.GetComponent<Toggle>();

        btn_close = FindComp<Button>("btn_close");
        btn_quxiao = FindComp<Button>("img_bg/btn_quxiao");
        btn_start = FindComp<Button>("img_bg/btn_start");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        toggle_Item1.onValueChanged.AddListener(OnToggleChanged1);
        toggle_Item2.onValueChanged.AddListener(OnToggleChanged2);
        toggle_Item3.onValueChanged.AddListener(OnToggleChanged3);

        btn_close.onClick.AddListener(OnBtnCloseClicked);
        btn_quxiao.onClick.AddListener(OnBtnQuxiaoClicked);
        btn_start.onClick.AddListener(OnBtnStartClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
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
        InitItem(go_Image_Item1, 10001);
        InitItem(go_Image_Item2, 10002);
        InitItem(go_Image_Item3, 10003);
    }

    public override void Hiding()
    {
        base.Hiding();

        toggle_Item1.isOn = false;
        toggle_Item2.isOn = false;
        toggle_Item3.isOn = false;
    }

    public override void Redisplay()
    {
        base.Redisplay();

        InitItem(go_Image_Item1, 10001);
        InitItem(go_Image_Item2, 10002);
        InitItem(go_Image_Item3, 10003);
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    protected void OnToggleChanged1(bool value)
    {
        if (value)
        {
            UseItemType = 10001;
            BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
            if (bgItem.Count <= 0) UseItemType = 0;
        }
        else
        {
            UseItemType = 0;
        }
    }
    protected void OnToggleChanged2(bool value)
    {
        if (value)
        {
            UseItemType = 10002;
            BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
            if (bgItem.Count <= 0) UseItemType = 0;
        }
        else
        {
            UseItemType = 0;
        }
    }
    protected void OnToggleChanged3(bool value)
    {
        if (value)
        {
            UseItemType = 10003;
            BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(UseItemType);
            if (bgItem.Count <= 0) UseItemType = 0;
        }
        else
        {
            UseItemType = 0;
        }
    }
    protected void OnBtnCloseClicked()
    {
        CloseUIForm();
        Singleton<RainbowBeachController>.Instance.SetMoveNormal(true);
    }
    protected void OnBtnQuxiaoClicked()
    {
        CloseUIForm();
        Singleton<RainbowBeachController>.Instance.SetMoveNormal(true);
    }
    protected void OnBtnStartClicked()
    {
        if (UseItemType == 0)
        {
            ToastManager.Instance.ShowNewToast("请选择一种铲子", 2);
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
        BeachShellReq shellReq = new BeachShellReq();
        shellReq.UserId = ManageMentClass.DataManagerClass.userId;
        shellReq.ToolId = (uint)UseItemType;
        Singleton<RainbowBeachController>.Instance.LastUserItemId = UseItemType;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.BeachShellReq, shellReq, OnBeachShellResp);
    }

    protected void OnBeachShellResp(int code, ByteString data)
    {
        CloseUIForm();
        BeachShellResp resp = BeachShellResp.Parser.ParseFrom(data);
        if (resp.StatusCode > 0)
        {
            ToastManager.Instance.ShowNewToast("挖贝壳出现错误", 2);
            Singleton<RainbowBeachController>.Instance.SetMoveNormal(true);
            return;
        }
        Singleton<RainbowBeachController>.Instance.RewardInfo = resp;
        Singleton<RainbowBeachController>.Instance.ExcavateShell(resp);
        //更新背包道具数量
        Singleton<BagMgr>.Instance.UpdateItem(resp.ToolId, resp.ToolNum);
    }


    protected void InitItem(GameObject go, int index)
    {
        Transform parent = go.transform;
        Text TextName = FindComp<Text>(parent, "TextName");
        Text TextCount = FindComp<Text>(parent, "TextCount");
        Image icon = FindComp<Image>(parent, "Imageicon");
        Button btn = FindComp<Button>(parent, "Image_no");
        //赋值
        BagItem bgItem = Singleton<BagMgr>.Instance.GetItem(index);
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
        });
    }
}
