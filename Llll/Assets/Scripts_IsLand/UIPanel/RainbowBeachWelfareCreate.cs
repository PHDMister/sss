//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachWelfareCreate : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_btn_quxiao;
    private Button button_btn_queding;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_btn_quxiao = FindComp<Button>("img_back/btn_quxiao");
        button_btn_queding = FindComp<Button>("img_back/btn_queding");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_quxiao, Onbtn_quxiaoClicked);
        RigisterCompEvent(button_btn_queding, Onbtn_quedingClicked);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_quxiaoClicked(GameObject go)
    {
        CloseUIForm();
    }
    private void Onbtn_quedingClicked(GameObject go)
    {
        WelfareOpenReq req = new WelfareOpenReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.WelfareOpenReq, req, (code, dataString) =>
        {
            WelfareOpenResp resp = WelfareOpenResp.Parser.ParseFrom(dataString);
            if (resp.StatusCode == 230025)
            {
                ToastManager.Instance.ShowNewToast("海洋公益名片已开通", 2);
                return;
            }
            CloseUIForm();
            ReqInfo();
        });
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

    public void ReqInfo()
    {
        WelfareInfoReq infoReq = new WelfareInfoReq();
        infoReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint) MessageId.Types.Enum.WelfareInfoReq, infoReq, (code, data) =>
        {
            WelfareInfoResp resp = WelfareInfoResp.Parser.ParseFrom(data);
            if (resp.HasOpen)
            {
                //添加黑盒数据
                AddBlackData("WelfareInfoResp", resp);
                AddBlackData("WelfareInfoResp1", resp);
                //有公益身份证
                OpenUIForm(FormConst.RAINBOWBEACHWELFARE);

                //拉取排行榜数据
                WelfareListReq rankListReq = new WelfareListReq();
                rankListReq.Page = 1;
                rankListReq.PageSize = 100;
                WebSocketAgent.SendMsg((uint) MessageId.Types.Enum.WelfareListReq, rankListReq, (c, d) =>
                {
                    WelfareListResp rankResp = WelfareListResp.Parser.ParseFrom(d);
                    Singleton<RainbowBeachDataModel>.Instance.RankList.Clear();
                    Singleton<RainbowBeachDataModel>.Instance.RankList.AddRange(rankResp.List);
                });

            }
            else
            {
                //没有公益身份证
                OpenUIForm(FormConst.RAINBOWBEACHWELFARECREATE);
            }
        });
    }


}
