//<Tools\GenUICode>工具生成, UI变化重新生成

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Treasure;
using UnityEngine.UI;
using UIFW;
using UnityTimer;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class RainbowBeachMainUI : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_Btn_Leave;
    private Button button_Btn_wen;
    private Image img_title;
    private Text text_title;
    private Text text_num;
    private Button button_btn_change;
    private Button button_btn_gongyi;
    private Text text_Text;
    private Button button_btn_shuizuguan;
    private Text text_num8;
    private Text text_gas;
    private Slider slider_Slider;
    private Text text_num10;
    private Button button_btn_pb;
    private Button button_btn_map;
    private Button button_btn_shop;
    private Button button_btn_selfdata;
    private Button button_btn_pack;
    private Button button_btn_chat;
    private Button button_btn_biaoqing;
    private Button button_biaoqingbar;
    private Image image_biaoqingIcon;
    private GameObject freeBeikeGo;
    private GameObject freeBeikeTip;
    private Button button_freeBekei;
    private Transform freeBeikeRedTip;
    private Button button_btn_music;
    private GameObject btn_music_open;
    private GameObject btn_music_close;
    protected bool visiableOtherPlayer = true;
    protected float gongyiLastTime = 0;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_Btn_Leave = FindComp<Button>("Layout_Left/Btn_Leave");
        button_Btn_wen = FindComp<Button>("Layout_Left/Btn_wen");
        img_title = FindComp<Image>("Layout_Left/Btn_Leave/Image");
        text_title = FindComp<Text>("Layout_Left/ChangSpace/title");
        text_num = FindComp<Text>("Layout_Left/ChangSpace/num");
        button_btn_change = FindComp<Button>("Layout_Left/ChangSpace/btn_change");
        button_btn_gongyi = FindComp<Button>("Layout_Left/btn_gongyi");
        text_Text = FindComp<Text>("Layout_Left/btn_gongyi/Text");
        button_btn_shuizuguan = FindComp<Button>("Layout_Left/btn_shuizuguan");
        text_num8 = FindComp<Text>("Layout_Left/beike/num");
        text_gas = FindComp<Text>("Layout_Left/gas/num");
        slider_Slider = FindComp<Slider>("Layout_Right/freeBeike/Slider");
        text_num10 = FindComp<Text>("Layout_Right/freeBeike/Slider/num");
        button_btn_pb = FindComp<Button>("Layout_Right/btn_pb");
        button_btn_map = FindComp<Button>("Layout_Right/btn_map");
        button_btn_shop = FindComp<Button>("Layout_Right/btn_shop");
        button_btn_selfdata = FindComp<Button>("Layout_Right/btn_selfdata");
        button_btn_pack = FindComp<Button>("Layout_Right/btn_pack");
        button_btn_chat = FindComp<Button>("Layout_Right/btn_chat");
        button_btn_biaoqing = FindComp<Button>("Layout_Right/btn_biaoqing");
        button_biaoqingbar = FindComp<Button>("Layout_Right/biaoqingbar");
        image_biaoqingIcon = FindComp<Image>("Layout_Right/biaoqing");
        freeBeikeGo = transform.Find("Layout_Right/freeBeike").gameObject;
        freeBeikeTip = transform.Find("Layout_Right/freeBeike/tip").gameObject;
        button_freeBekei = FindComp<Button>("Layout_Right/freeBeike");
        freeBeikeRedTip = transform.Find("Layout_Right/freeBeike/redtip");
        button_btn_music = FindComp<Button>("Layout_Right/btn_music");
        btn_music_open = button_btn_music.transform.Find("front1").gameObject;
        btn_music_close = button_btn_music.transform.Find("front2").gameObject;

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Btn_Leave, OnBtn_LeaveClicked);
        RigisterCompEvent(button_Btn_wen, OnBtn_wenClicked);
        RigisterCompEvent(button_btn_change, Onbtn_changeClicked);
        RigisterCompEvent(button_btn_shuizuguan, Onbtn_shuizuguanClicked);
        RigisterCompEvent(button_btn_gongyi, On_btn_gongyi_Clicked);
        slider_Slider.onValueChanged.AddListener(OnSliderValueChanged);
        RigisterCompEvent(button_btn_pb, Onbtn_pbClicked);
        RigisterCompEvent(button_btn_map, Onbtn_mapClicked);
        RigisterCompEvent(button_btn_shop, Onbtn_shopClicked);
        RigisterCompEvent(button_btn_selfdata, Onbtn_selfdataClicked);
        RigisterCompEvent(button_btn_pack, Onbtn_packClicked);
        RigisterCompEvent(button_btn_chat, Onbtn_chatClicked);
        RigisterCompEvent(button_btn_biaoqing, Onbtn_biaoqingClicked);
        RigisterCompEvent(button_biaoqingbar, Onbutton_biaoqingbar);
        RigisterCompEvent(button_freeBekei, Onbutton_freeBekeiClicked);
        RigisterCompEvent(button_btn_music, Onbutton_btn_musicClicked);

        ReceiveMessage("OpenChatUI", p =>
        {
            openChatUI();
        });
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnBtn_LeaveClicked(GameObject go)
    {
        OnClickReturnBtnFun();
    }
    private void OnBtn_wenClicked(GameObject go)
    {
        if (CheckStateBySceneID()) return;
        OpenUIForm(FormConst.LANDGAMERULEPANLE);
    }
    private void Onbtn_changeClicked(GameObject go)
    {
        if (CheckStateBySceneID()) return;
        OpenUIForm(FormConst.RAINBOWBEACHROOMLIST);
    }

    private void Onbtn_shuizuguanClicked(GameObject go)
    {
        if (CheckStateBySceneID()) return;
        //ToastManager.Instance.ShowNewToast("海洋博物馆建设中，敬请期待~", 3);
        OpenUIForm(FormConst.FISHCLUBPANEL);
    }
    private void On_btn_gongyi_Clicked(GameObject go)
    {
        if (CheckStateBySceneID()) return;

        if (Time.realtimeSinceStartup - gongyiLastTime < 2) return;
        gongyiLastTime = Time.realtimeSinceStartup;

        WelfareInfoReq infoReq = new WelfareInfoReq();
        infoReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.WelfareInfoReq, infoReq, (code, data) =>
       {
           WelfareInfoResp resp = WelfareInfoResp.Parser.ParseFrom(data);
           if (resp.HasOpen)
           {
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
    private void OnSliderValueChanged(float arg)
    {
        //text_num10.text = arg + "/" + 10;
    }
    private void Onbtn_pbClicked(GameObject go)
    {
        ShowOrHideOtherRoomPlayBySceneID();
    }
    private void Onbtn_mapClicked(GameObject go)
    {
        if (CheckStateBySceneID())
        {
            return;
        }
        OpenUIForm(FormConst.LANDMAPPANEL);
    }
    private void Onbtn_shopClicked(GameObject go)
    {
        //if (CheckStateBySceneID())
        //{
        //    return;
        //}
        OpenUIForm(FormConst.RAINBOWBEACHSHOPPANEL);
    }
    private void Onbtn_selfdataClicked(GameObject go)
    {
        if (CheckStateBySceneID())
        {
            return;
        }
        if (sceneID == (int) LoadSceneType.HaiDiXingKong)
        {
            ToastManager.Instance.ShowNewToast("正在潜水，请稍后在试~", 2);
            return;
        }
        MessageManager.GetInstance().RequestGetPersonData(() =>
        {
            OpenUIForm(FormConst.PERSONALDATAPANEL);
            SendMessage("OpenPersonDataPanelRefreshUI", "Success", (ulong)0);
        });
    }
    private void Onbtn_packClicked(GameObject go)
    {
        //if (CheckStateBySceneID())
        //{
        //    return;
        //}
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHBAGPANEL);
    }
    private void Onbtn_chatClicked(GameObject go)
    {
        //if (CheckStateBySceneID())
        //{
        //    return;
        //}
        var isOpen = UIManager.GetInstance().IsOpend(FormConst.UICHAT);
        if (isOpen)
        {
            UIManager.GetInstance().CloseUIForms(FormConst.UICHAT);
        }
        else
        {
            openChatUI();
        }
    }
    private void Onbtn_biaoqingClicked(GameObject go)
    {
        if (CheckStateBySceneID())
        {
            return;
        }
        GameObject actButton = button_btn_biaoqing.gameObject;
        if (actButton != null) actButton.gameObject.SetActive(false);
        OpenUIForm(FormConst.SETACTION_UIFORM);
    }
    private float lastSendActionTime = 0;
    private void Onbutton_biaoqingbar(GameObject go)
    {
        if (CheckStateBySceneID())
        {
            return;
        }
        if (m_SelectActItemData == null) return;
        int itemId = m_SelectActItemData.item_id;
        animation m_Animation = ManageMentClass.DataManagerClass.GetAnimationTableFun(itemId);
        if (m_Animation != null)
        {
            string actClipName = m_Animation.animation_model;
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem == null) return;
            if (playerItem.IsHaveState(actClipName))
            {
                //播放表情动作
                OnPlaySelectAct(actClipName);
                //在挖宝房间内发送消息同步
                if (CheckSceneToSendAction() && Time.realtimeSinceStartup - lastSendActionTime > 3)
                {
                    lastSendActionTime = Time.realtimeSinceStartup;
                    DoActionReq doAction = new DoActionReq();
                    doAction.Index = WebSocketAgent.Ins.NetView.GetCode;
                    doAction.UserId = ManageMentClass.DataManagerClass.userId;
                    doAction.Action = (uint)itemId;
                    WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.DoActionReq, doAction);
                }
            }
            else
            {
                ToastManager.Instance.ShowNewToast(string.Format("暂未拥有{0}动作，敬请期待！", m_Animation.animation_name), 5f);
            }
        }
    }
    private void Onbutton_freeBekeiClicked(GameObject go)
    {
        //if (CheckStateBySceneID()) return;
        if (freeBeikeRedTip.gameObject.activeSelf)
        {
            freeBeikeRedTip.gameObject.SetActive(false);
        }
        OpenUIForm(FormConst.RAINBOWBEACHSHARESHELLS);
    }
    private void Onbutton_btn_musicClicked(GameObject go)
    {
        bool val = AudioMgr.Ins.isPlaying;
        AudioMgr.Ins.SetMute(val);
        UpdateAudioState();
    }

    // UI EVENT FUNC END
    private item m_SelectActItemData;
    private Timer UpdateShellCountTimer;

    private int requestNum = 0;
    private bool bFirstGuide = false;

    private int sceneID;
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
        ReceiveMessage("SetActionItemSuccess", p =>
        {
            m_SelectActItemData = p.Values as item;
            if (m_SelectActItemData != null)
                SetItemIcon(m_SelectActItemData.item_icon);
        });
        ReceiveMessage("SetActionClose", p =>
        {
            GameObject actButton = button_btn_biaoqing.gameObject;
            if (actButton != null) actButton.gameObject.SetActive(true);
        });
        ReceiveMessage("UpdateFreeShellsOnMainUI", p =>
        {
            uint count = Singleton<RainbowBeachDataModel>.Instance.FreeShovleCount;
            slider_Slider.value = count / 10.0f;
            text_num10.text = count + "/" + 10;
            freeBeikeTip.gameObject.SetActive(count >= 10);
        });
        ReceiveMessage("UpdateShellRedPointData", p =>
        {
            if (!freeBeikeTip.gameObject.activeSelf)
            {
                bool redPoint = Singleton<RainbowBeachDataModel>.Instance.OtherShareHongbao;
                freeBeikeRedTip.gameObject.SetActive(redPoint);
            }
        });
    }

    public override void Display()
    {
        base.Display();
        sceneID = ManageMentClass.DataManagerClass.SceneID;
        //更新标题
        SetIcon(img_title, "ShallMainUI", GetSceneImgBySceneID());
        //更新房间描述
        text_title.text = GetSceneNameBySceneID();
        UpdateAudioState();
        SetCurActIcon();
        UpdateChangedData();
        UpdateFreeShells();
        if (UpdateShellCountTimer == null)
            UpdateShellCountTimer = Timer.Register(1, UpdateChangedData, null, true, true);
        ActiveDoAction();

        requestNum = 0;
        StartCoroutine(WaitOpenChat());
    }

    private IEnumerator WaitOpenChat()
    {
        SetGasAndTicket();
        CheckShowGuide();
        while (requestNum < 2)
        {
            yield return null;
        }

        if (!bFirstGuide)
        {
        //     openChatUI();
        }
    }

    private void SetGasAndTicket()
    {
        MessageManager.GetInstance().RequestGasValue(() =>
        {
            //m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
            requestNum++;
        });
    }

    /// <summary>
    /// 新手引导Tips
    /// </summary>
    private void CheckShowGuide()
    {
        CheckBeginLaunchReq req = new CheckBeginLaunchReq
        {
            UserId = ManageMentClass.DataManagerClass.userId
        };

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.CheckBeginLaunchReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            CheckBeginLaunchResp checkBeginLaunchResp = CheckBeginLaunchResp.Parser.ParseFrom(bytes);
            if (checkBeginLaunchResp.StatusCode == 0)
            {
                if ((int)checkBeginLaunchResp.IsComplete == 0)
                {
                    bFirstGuide = true;
                    OpenGuideBySceneID();
                }
            }
            requestNum++;
        });
    }

    private void openChatUI()
    {
        if (UIManager.GetInstance().IsOpend(FormConst.UICHAT))
        {
            return;
        }
        OpenUIForm(FormConst.UICHAT);
        var param = GetUIChatParamBySceneID();
        MessageCenter.SendMessage("OnOpenUIChat", "param", param);
    }

    public override void Hiding()
    {
        base.Hiding();
        bFirstGuide = false;
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    protected void OnDestroy()
    {
        UpdateShellCountTimer?.Cancel();
        UpdateShellCountTimer = null;
    }
    public void SetCurActIcon()
    {
        int m_SelectActId = PlayerPrefs.GetInt("CurUseActId");
        if (m_SelectActId <= 0)
        {
            m_SelectActId = 2003;//默认问候
            PlayerPrefs.SetInt("CurUseActId", m_SelectActId);
        }
        m_SelectActItemData = ManageMentClass.DataManagerClass.GetItemTableFun(m_SelectActId);
        if (m_SelectActItemData != null)
        {
            string spriteName = m_SelectActItemData.item_icon;
            SetItemIcon(spriteName);
        }
    }
    public void OnPlaySelectAct(string triggerName)
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator(triggerName);
        }
        //GameStartController.PlayerItem.SetAnimator(triggerName);
    }
    public void SetItemIcon(string spriteName)
    {
        /*  var atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
          Sprite sprite = atlas.GetSprite(spriteName);*/
        Transform biaoqingtrans = button_btn_biaoqing.transform.parent.Find("biaoqing");
        if (!biaoqingtrans) return;
        //biaoqingtrans.GetComponent<Image>().sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(spriteName);
        Image biaoqingImage = biaoqingtrans.GetComponent<Image>();
        SetIcon(biaoqingImage, "SetAction", spriteName);
    }
    private bool CheckSceneToSendAction()
    {
        return ManageMentClass.DataManagerClass.SceneID == 4
               || ManageMentClass.DataManagerClass.SceneID == 1
               || ManageMentClass.DataManagerClass.SceneID == 6
               || ManageMentClass.DataManagerClass.SceneID == 7
               || ManageMentClass.DataManagerClass.SceneID == 8;
    }
    private void UpdateChangedData()
    {
        text_num.text = ManageMentClass.DataManagerClass.roomId.ToString();
        text_num8.text = Singleton<BagMgr>.Instance.ShellNum.ToString();
        text_gas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
    }
    private void OnClickReturnBtnFun()
    {
        //点击了退出
        try
        {
            //关闭音效控制器
             AudioMgr.Ins?.SetMute(true);
            //设置竖屏
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
    //免费贝壳奖励的挖贝壳测试
    private void UpdateFreeShells()
    {
        //免费贝壳tip
        uint freeCount = Singleton<RainbowBeachDataModel>.Instance.FreeShovleCount;
        slider_Slider.value = freeCount / 10.0f;
        text_num10.text = freeCount + "/10";
        freeBeikeTip.gameObject.SetActive(freeCount >= 10);

        bool redPoint = Singleton<RainbowBeachDataModel>.Instance.OtherShareHongbao;
        freeBeikeRedTip.gameObject.SetActive(redPoint);
    }
    private void UpdateAudioState()
    {
        bool val = AudioMgr.Ins.isPlaying;
        btn_music_open.gameObject.SetActive(val);
        btn_music_close.gameObject.SetActive(!val);
    }

    /*------------------------------------------------根据场景变化的函数------------------------------------------------*/

    private string GetSceneImgBySceneID()
    {
        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            return "caihongshatan";
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan)
        {
            return "shenmihaiwan";
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            return "biaotixinxilan";
        }
        return null;
    }

    private string GetSceneNameBySceneID()
    {
        return SceneConfig.GetName(sceneID)+" ·";
    }

    private void OpenGuideBySceneID()
    {
        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            OpenUIForm(FormConst.RAINBOWBEACHGUIDEPANEL);
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan)
        {
            OpenUIForm(FormConst.RAINBOWIOCNGUIDEPANEL);
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            OpenUIForm(FormConst.RAINBOWSEABEDGUIDEPANEL);
        }
    }

    private bool CheckStateBySceneID()
    {
        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            if (Singleton<RainbowBeachController>.Instance.IsWorking)
            {
                ToastManager.Instance.ShowNewToast("正在挖贝壳，请稍后在试~", 2);
                return true;
            }
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan)
        {
            if (Singleton<RainbowIocnController>.Instance.IsWorking)
            {
                ToastManager.Instance.ShowNewToast("正在钓鱼，请稍后在试~", 2);
                return true;
            }
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            if (Singleton<RainbowSeabedController>.Instance.IsWorking)
            {
                ToastManager.Instance.ShowNewToast("正在潜水，请稍后在试~", 2);
                return true;
            }
        }
        return false;
    }

    private UIChatParam GetUIChatParamBySceneID()
    {

        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            var param = new UIChatParam()
            {
                ChatTypes = new List<ChatType> { ChatType.All, ChatType.Room },
                Controller = Singleton<RainbowBeachController>.Instance,
            };
            return param;
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan)
        {
            var param = new UIChatParam()
            {
                ChatTypes = new List<ChatType> { ChatType.All, ChatType.Room },
                Controller = Singleton<RainbowIocnController>.Instance,
            };
            return param;
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            var param = new UIChatParam()
            {
                ChatTypes = new List<ChatType> { ChatType.All, ChatType.Room },
                Controller = Singleton<RainbowSeabedController>.Instance,
            };
            return param;
        }
        return null;
    }


    //当前场景是否可以做表情
    private void ActiveDoAction()
    {
        bool active = sceneID != (int)LoadSceneType.ShenMiHaiWan;
        button_btn_biaoqing.gameObject.SetActive(active);
        button_biaoqingbar.gameObject.SetActive(active);
        image_biaoqingIcon.gameObject.SetActive(active);
    }

    private void ShowOrHideOtherRoomPlayBySceneID()
    {
        if (sceneID == (int)LoadSceneType.RainbowBeach)
        {
            Singleton<RainbowBeachController>.Instance.ShowOrHideOtherRoomPlayer(!visiableOtherPlayer);
        }
        else if (sceneID == (int)LoadSceneType.ShenMiHaiWan)
        {
            Singleton<RainbowIocnController>.Instance.ShowOrHideOtherRoomPlayer(!visiableOtherPlayer);
        }
        else if (sceneID == (int)LoadSceneType.HaiDiXingKong)
        {
            Singleton<RainbowSeabedController>.Instance.ShowOrHideOtherRoomPlayer(!visiableOtherPlayer);
        }
        visiableOtherPlayer = !visiableOtherPlayer;
    }
}
