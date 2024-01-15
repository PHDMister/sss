//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class UITreasureReadyPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_button_ready;
    private Text text_name;
    private Text text_proces;
    private Text text_name4;
    private Text text_proces5;
    private Text text_time;
    private Image image_Render;
    private Image image_Render2;
    private GameObject go_trans_ready;
    private GameObject go_trans_Team1;
    private GameObject go_trans_Team2;
    private GameObject go_trans_time;
    private GameObject go_biaotiqu;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_button_ready = FindComp<Button>("trans_ready/button-ready");
        text_name = FindComp<Text>("wabaosatemianban/renyuanxinxi/trans_Team1/team-mianban/Frame 1000008135/name");
        text_proces = FindComp<Text>("wabaosatemianban/renyuanxinxi/trans_Team1/Frame 1000007996/proces");
        text_name4 = FindComp<Text>("wabaosatemianban/renyuanxinxi/trans_Team2/team-mianban/buff/name");
        text_proces5 = FindComp<Text>("wabaosatemianban/renyuanxinxi/trans_Team2/buff/proces");
        text_time = FindComp<Text>("wabaosatemianban/trans_time/Text");
        go_trans_ready = transform.Find("trans_ready").gameObject;
        go_trans_Team1 = transform.Find("wabaosatemianban/renyuanxinxi/trans_Team1").gameObject;
        go_trans_Team2 = transform.Find("wabaosatemianban/renyuanxinxi/trans_Team2").gameObject;
        go_trans_time = transform.Find("wabaosatemianban/trans_time").gameObject;
        go_biaotiqu = transform.Find("wabaosatemianban/biaotiqu").gameObject;
        image_Render = FindComp<Image>("wabaosatemianban/renyuanxinxi/trans_Team1/team-mianban/Frame 1000008135/touxiang/touxiang/Render");
        image_Render2 = FindComp<Image>("wabaosatemianban/renyuanxinxi/trans_Team2/team-mianban/buff/touxiang/touxiang/Render");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_button_ready, Onbutton_readyClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private float LastTime = 0;
    private void Onbutton_readyClicked(GameObject go)
    {
        if (Time.realtimeSinceStartup - LastTime < 2) return;
        LastTime = Time.realtimeSinceStartup;

        //发送
        RoomUserInfo selfUserInfo = Singleton<TreasuringController>.Instance.GetSelfUserInfo();
        GameObject selfPlayerGo = CharacterManager.Instance().GetPlayerObj();
        if (!TreasureCircle.IsInside(selfUserInfo.CircleIndex, selfPlayerGo.transform.position))
        {
            ToastManager.Instance.ShowNewToast("不在指定挖宝范围内", 3);
            return;
        }

        //检测当前消费券
        if (ManageMentClass.DataManagerClass.ticket <= 0)
        {
            OpenUIForm(FormConst.TREASUREDIGGINGTICKETEXCHANGE);
            return;
        }
        //有挖宝卷  需要弹出确认框
        if (TreasuringController.ReadyShowNextCheck)
        {
            OpenUIForm(FormConst.UITREASUREREADYCHECK);
            return;
        }

        ReadyTreasureReq req = new ReadyTreasureReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.ReadyTreasureReq, req, ReadCallback);
    }
    // UI EVENT FUNC END
    public const string Event_OnTreasureStart = "Event_OnTreasureStart";
    public const string Event_OnUpdateReadyList = "Event_OnUpdateReadyList";
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

    }

    public override void Display()
    {
        base.Display();
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.ReadyTreasurePush, OnReadyTreasurePush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.CancelReadyTreasurePush, OnCancelReadyTreasurePush);

        ReceiveMessage(Event_OnTreasureStart, OnTreasureStartHandler);
        ReceiveMessage(Event_OnUpdateReadyList, OnUpdateReadyListHandle);

        UpdateTeamListUI();
    }



    public override void Hiding()
    {
        base.Hiding();
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.ReadyTreasurePush, OnReadyTreasurePush);
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.CancelReadyTreasurePush, OnCancelReadyTreasurePush);

        RemoveMsgListener(Event_OnTreasureStart, OnTreasureStartHandler);
        RemoveMsgListener(Event_OnUpdateReadyList, OnUpdateReadyListHandle);
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    private void OnReadyTreasurePush(uint clientCode, ByteString data)
    {
        ReadyTreasurePush readyTreasure = ReadyTreasurePush.Parser.ParseFrom(data);
        Debug.Log($"111111    OnReadyTreasurePush  readyTreasure:{readyTreasure} ");
        RoomUserInfo userInfo = Singleton<TreasuringController>.Instance.SetPlayerState(readyTreasure.FromUserId, readyTreasure.State);
        Singleton<TreasuringController>.Instance.ReproduceUserState(userInfo);
        UpdateTextState(readyTreasure.FromUserId, readyTreasure.State);
    }
    private void OnCancelReadyTreasurePush(uint clientCode, ByteString data)
    {
        ReadyTreasurePush readyTreasure = ReadyTreasurePush.Parser.ParseFrom(data);
        Singleton<TreasuringController>.Instance.SetPlayerState(readyTreasure.FromUserId, readyTreasure.State);
        Singleton<TreasuringController>.Instance.SetTeamPlayerAnim(readyTreasure.FromUserId, "Idle");
        if (readyTreasure.FromUserId == ManageMentClass.DataManagerClass.userId)
            Singleton<TreasuringController>.Instance.ClearSelfMoveCheck();
        UpdateTextState(readyTreasure.FromUserId, readyTreasure.State);
    }
    private void OnTreasureStartHandler(KeyValuesUpdate kv)
    {
        UpdateTeamListUI();
    }
    private void OnUpdateReadyListHandle(KeyValuesUpdate kv)
    {
        UpdateTeamListUI(true);
    }

    private void UpdateTeamListUI(bool isTeamUpdate = false)
    {
        List<RoomUserInfo> teamUserInfo = TreasureModel.Instance.TeamUserList;
        ShowTeamUI1(teamUserInfo.Count > 0, teamUserInfo);
        ShowTeamUI2(teamUserInfo.Count > 1, teamUserInfo);
        //标题显示
        bool isAllReady = teamUserInfo.Exists(info => info.State != (uint)PlayerState.Types.Enum.Treasureing);
        go_biaotiqu.gameObject.SetActive(isAllReady);
        //全都是挖宝状态
        bool isAllTreasureing = teamUserInfo.TrueForAll(info => info.State == (uint)PlayerState.Types.Enum.Treasureing);
        go_trans_time.gameObject.SetActive(isAllTreasureing);
        //显示时间
        RoomUserInfo userInfo = teamUserInfo.Find(info => info.UserId == ManageMentClass.DataManagerClass.userId);
        if (userInfo == null) Debug.LogError("11111111111111   UpdateTeamListUI   is   null  ");
        TreasureCountDown countDown = text_time.GetComponent<TreasureCountDown>();
        if (userInfo != null && userInfo.PartnerLeaveTime > 0)
        {
            countDown.SetEndTime(2);
            if (teamUserInfo.Count == 1) countDown.ResetEndTimeOnLeavePartner(1);
        }
        else
        {
            if (isAllTreasureing && !isTeamUpdate) countDown.SetEndTime(teamUserInfo.Count);
            if (teamUserInfo.Count == 1 && userInfo != null && userInfo.PartnerLeaveTime > 0) countDown.ResetEndTimeOnLeavePartner(1);
        }
        //准备按钮的状态
        go_trans_ready.SetActive(userInfo != null && userInfo.State != (uint)PlayerState.Types.Enum.Ready
                                && userInfo.State != (uint)PlayerState.Types.Enum.Treasureing);
    }

    protected const string State1 = "<color=#FF4893>未准备</color>";
    protected const string State2 = "<color=#B8F57C>已准备</color>";
    protected const string State3 = "<color=#B8F57C>挖宝中...</color>";

    private const string Proces0 = "<color=#9D9D9D>+{0}%</color>";
    private const string Proces1 = "<color=#FFF271>+{0}%</color>";
    private string IsLoadHeadIcon = "";
    private string IsLoadHeadIcon2 = "";
    private void ShowTeamUI1(bool show, List<RoomUserInfo> userInfos)
    {
        if (!show)
        {
            go_trans_Team1.SetActive(false);
            return;
        }
        RoomUserInfo userInfo = userInfos[0];
        go_trans_Team1.SetActive(true);
        //Debug.Log($"1111111 ShowTeamUI1  userInfo:{userInfo} ");
        text_name.text = GetState(userInfo.State);

        if (userInfos.Count <= 1) text_proces.text = string.Format(Proces0, 0);
        else text_proces.text = string.Format(Proces1, 20);

        //加载头像
        if (IsLoadHeadIcon == userInfo.PicUrl) return;
        MessageManager.GetInstance().DownLoadAvatar(userInfo.PicUrl, (sprite) =>
        {
            IsLoadHeadIcon = userInfo.PicUrl;
            image_Render.sprite = sprite;
        });
    }
    private void ShowTeamUI2(bool show, List<RoomUserInfo> userInfos)
    {
        if (!show)
        {
            go_trans_Team2.SetActive(false);
            return;
        }
        RoomUserInfo userInfo = userInfos[1];
        go_trans_Team2.SetActive(true);
        //Debug.Log($"1111111 ShowTeamUI2  userInfo:{userInfo} ");
        text_name4.text = GetState(userInfo.State);

        if (userInfos.Count <= 1) text_proces5.text = string.Format(Proces0, 0);
        else text_proces5.text = string.Format(Proces1, 20);

        //加载头像
        if (IsLoadHeadIcon2 == userInfo.PicUrl) return;
        MessageManager.GetInstance().DownLoadAvatar(userInfo.PicUrl, (sprite) =>
        {
            IsLoadHeadIcon2 = userInfo.PicUrl;
            image_Render2.sprite = sprite;
        });
    }
    private void UpdateTextState(ulong userId, uint state)
    {
        if (userId == 0) userId = ManageMentClass.DataManagerClass.userId;
        List<RoomUserInfo> teamUserInfo = TreasureModel.Instance.TeamUserList;
        int index = teamUserInfo.FindIndex(info => info.UserId == userId);
        if (index == 0) text_name.text = GetState(state);
        if (index == 1) text_name4.text = GetState(state);
    }
    private string GetState(uint state)
    {
        switch (state)
        {
            case 0:
            case 1:
            case 2:
            case 3: return State1;
            case 4: return State2;
            case 5: return State3;
        }
        return State1;
    }
    public void ReadCallback(int code, ByteString data)
    {
        if (code == 0)
        {
            go_trans_ready.SetActive(false);
            //Debug.Log("11111111111  ReadyTreasureReq    code="+code);
            //检测是否变更自己的状态
            RoomUserInfo newSelfUserInfo = Singleton<TreasuringController>.Instance.GetSelfUserInfo();
            if (newSelfUserInfo.State != (uint)PlayerState.Types.Enum.Treasureing
                && newSelfUserInfo.State != (uint)PlayerState.Types.Enum.Ready)
            {
                //设置自己状态
                Singleton<TreasuringController>.Instance.SetPlayerState(0, (uint)PlayerState.Types.Enum.Ready);
                Singleton<TreasuringController>.Instance.ReproduceUserState(newSelfUserInfo);
                //刷新状态
                UpdateTeamListUI();
            }
        }
        else
        {
            Debug.LogError("ReadyTreasureReq  error  code  "+code);
        }
    }
}
