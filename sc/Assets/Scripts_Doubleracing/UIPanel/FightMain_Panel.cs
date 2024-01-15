//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using Spine.Unity;
using Fight;
using Google.Protobuf;

public class FightMain_Panel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Button button_Attack_Btn;
    private Button button_Defense_Btn;
    private Button button_Return_Btn;
    private Text text_Time_Text;
    private GameObject TipsPanel;

    private Text text_tipsText;

    private GameObject spineTipsPrefab;
    private SkeletonGraphic skeletonGraphicTips;

    private Text text_SheelValue_Text;


    private Button Left_Btn;
    private Button Right_Btn;


    private Slider AttackSlider;
    private Slider DenfensSlider;

    private Skill AttackSkiillData;
    private Skill DenfensSkiillData;



    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        button_Attack_Btn = FindComp<Button>("Attack_Btn");
        button_Defense_Btn = FindComp<Button>("Defense_Btn");
        button_Return_Btn = FindComp<Button>("Return_Btn");
        text_Time_Text = FindComp<Text>("GroupAAAA/GroupBBB/Time_Text");


        Left_Btn = FindComp<Button>("Left_Button");
        Right_Btn = FindComp<Button>("Right_Button");

        AttackSlider = FindComp<Slider>("Attack_Btn/Slider");
        DenfensSlider = FindComp<Slider>("Defense_Btn/Slider");
        text_tipsText = FindComp<Text>("TipsPanel/tipsText");

        text_SheelValue_Text = FindComp<Text>("Shell_Value/ShellValue_Text");

        TipsPanel = transform.Find("TipsPanel").gameObject;
        spineTipsPrefab = transform.Find("TipsPanel/SpineTips").gameObject;

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Attack_Btn, OnAttack_BtnClicked);
        RigisterCompEvent(button_Defense_Btn, OnDefense_BtnClicked);
        RigisterCompEvent(button_Return_Btn, OnReturn_BtnClicked);

        RigisterCompEvent(Left_Btn, OnLeft_BtnClicked);
        RigisterCompEvent(Right_Btn, OnRight_BtnClicked);
    }

    // UI EVENT REGISTER END

    private float SecondTimeValue;

    // UI EVENT FUNC START
    private void OnAttack_BtnClicked(GameObject go)
    {
        PropReq propReq = new PropReq();
        propReq.UserId = ManageMentClass.DataManagerClass.userId;
        propReq.PropId = 1;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.PropReq, propReq);
    }
    private void OnDefense_BtnClicked(GameObject go)
    {
        PropReq propReq = new PropReq();
        propReq.UserId = ManageMentClass.DataManagerClass.userId;
        propReq.PropId = 2;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.PropReq, propReq);
    }
    private void OnReturn_BtnClicked(GameObject go)
    {
        Debug.Log("退出游戏");
    }
    private void OnLeft_BtnClicked(GameObject go)
    {
        Debug.Log("左边");
        GameController.Instance().ControCarLeftFun(ManageMentClass.DataManagerClass.userId);
    }
    private void OnRight_BtnClicked(GameObject go)
    {
        Debug.Log("右边");
        GameController.Instance().ControCarRightFun(ManageMentClass.DataManagerClass.userId);
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        if (spineTipsPrefab)
        {
            skeletonGraphicTips = spineTipsPrefab.GetComponent<SkeletonGraphic>();
        }
        //开始移动
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.GameStartPush, OnChangesSinResp);
        ReceiveMessage("OnUseSkillTipFun", p =>
        {
            PropResp sitdownPush = p.Values as PropResp;

            if (sitdownPush.StatusCode == 0)
            {
                ManageMentClass.DataManagerClass.ShellCount = (int)sitdownPush.BkCount;
                if (sitdownPush.PropId == 1)
                {
                    
                        OnControllerAttackSkillBtnFun(10.5f);
                    
                   
                    GameController.Instance().PlayerAttackOtherFun();
                    Debug.Log("输出一下， 发起攻击了");
                }
                else if (sitdownPush.PropId == 2)
                {
                    
                        OnControllerDenfensSkillBtnFun(10.5f);
                    
                  
                }
               // ManageMentClass.DataManagerClass.ShellCount = 0;
            }
            else if (sitdownPush.StatusCode == 27001)
            {
                ToastManager.Instance.ShowNewToast("贝壳不足", 2f);
            }
            else if (sitdownPush.StatusCode == 27003)
            {
                ToastManager.Instance.ShowNewToast("技能冷却中", 2f);
            }
            Debug.Log("技能释放了：：  : " + sitdownPush.ToJSON());
            text_SheelValue_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";
             
        });

        DenfensSkiillData = ManageMentClass.DataManagerClass.GetSkillTable(2002);
        AttackSkiillData = ManageMentClass.DataManagerClass.GetSkillTable(2002);
        ReceiveMessage("GameBegin", p =>
        {
            SecondTimeValue = 60;
            // StartCoroutine(CountDownTimerFun());
        });
    }
    public override void Display()
    {
        base.Display();
        TipsPanel.SetActive(true);
        AttackSlider.value = 0;
        DenfensSlider.value = 0;
        text_Time_Text.text = "倒计时：01:00";
        text_SheelValue_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";


        // DoubleUserDataDic.Add(item.UserId, doubleacingUserData);
        if (GameController.Instance().DoubleUserDataDic.ContainsKey(ManageMentClass.DataManagerClass.userId))
        {
            if (GameController.Instance().DoubleUserDataDic[ManageMentClass.DataManagerClass.userId].RoleTypeId == 1)
            {
                text_tipsText.text = "有人闯入草原，快去抓住他!";
            }
            else
            {
                text_tipsText.text = "赶快穿越草原，不要被追击者抓住!";
            }
        }
        else
        {
            text_tipsText.text = "加油！战胜对方！";
        }
        if (skeletonGraphicTips)
        {
            string aniName = skeletonGraphicTips.SkeletonData.Animations.Items[0].Name;
            skeletonGraphicTips.AnimationState.SetAnimation(0, aniName, true);
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
    private void OnChangesSinResp(uint clientCode, ByteString data)
    {
        GameStartPush startPush = GameStartPush.Parser.ParseFrom(data);
        Debug.Log($" 内容值   士大夫胜多负少   {startPush}");
        TipsPanel.SetActive(false);
        GameController.Instance().StartGameFun();
    }
    //攻击按钮cd
    public void OnControllerAttackSkillBtnFun(float timeA)
    {
        AttackSlider.value = 1;
        button_Attack_Btn.enabled = false;
        StartCoroutine(OnCtrAttackSkilBtnFun(timeA));
    }
    public IEnumerator OnCtrAttackSkilBtnFun(float timeA)
    {
        while (timeA > 0)
        {
            yield return null;
            timeA -= Time.deltaTime;
            AttackSlider.value = timeA / AttackSkiillData.skill_cdTime;
            if (timeA <= 0)
            {
                // 结束冷却
                AttackSlider.value = 0;
                button_Attack_Btn.enabled = true;
            }
        }
    }


    /// <summary>
    /// 防御按钮Cd
    /// </summary>
    /// <param name="timeA"></param>
    public void OnControllerDenfensSkillBtnFun(float timeA)
    {
        DenfensSlider.value = 1;
        button_Defense_Btn.enabled = false;
        StartCoroutine(OnCtrDenfensSkilBtnFun(timeA));
    }
    public IEnumerator OnCtrDenfensSkilBtnFun(float timeA)
    {
        while (timeA > 0)
        {
            yield return null;
            timeA -= Time.deltaTime;
            DenfensSlider.value = timeA / DenfensSkiillData.skill_cdTime;
            if (timeA <= 0)
            {
                // 结束冷却
                DenfensSlider.value = 0;
                button_Defense_Btn.enabled = true;
            }
        }
    }
    public void Update()
    {
        if (GameController.Instance().GameRuning)
        {
            SecondTimeValue -= Time.deltaTime;
            text_Time_Text.text = "倒计时：00:" + SecondTimeValue.ToString("F0");
        }
    }

}
