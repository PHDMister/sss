//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using Fight;
using Spine.Unity;
using Google.Protobuf;

public class WaitMatch_Panel : BaseUIForm
{
    // UI VARIABLE STATEMENT START
    private Image image_WaitMatch_Panel;
    private Button button_Close_Btn;
    private Text text_waitMatchState_Text;

    private Text text_ShellValue_Text;

    private GameObject spineMatchPrefab;
    private SkeletonGraphic skeletonGraphic;


    private GameObject spineVSPrefab;
    private SkeletonGraphic skeletonGraphicVS;

    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        image_WaitMatch_Panel = transform.GetComponent<Image>();
        button_Close_Btn = FindComp<Button>("Close_Btn");
        text_waitMatchState_Text = FindComp<Text>("waitMatchState_Text");

        text_ShellValue_Text = FindComp<Text>("Shell_Value/Text");

        spineMatchPrefab = transform.Find("Waiting_Panel/SpineMatchWatingPrefab").gameObject;
        spineVSPrefab = transform.Find("WaitSuccess_Panel/SpineVS").gameObject;

        OnAwake();
        AddEvent();
        RegistFun();
    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_Close_Btn, OnClose_BtnClicked);
    }   // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnClose_BtnClicked(GameObject go)
    {
        OpenUIForm(FormConst.MATCHINGPANEL);
        CloseUIForm();
    }
    // UI EVENT FUNC END

    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Normal;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        if (spineMatchPrefab)
        {
            skeletonGraphic = spineMatchPrefab.GetComponent<SkeletonGraphic>();
        }
        if (spineVSPrefab)
        {
            skeletonGraphicVS = spineVSPrefab.GetComponent<SkeletonGraphic>();
            spineVSPrefab.SetActive(false);
        }
    }
    public override void Display()
    {
        base.Display();
        text_ShellValue_Text.text = ManageMentClass.DataManagerClass.ShellCount + "";
        if (skeletonGraphic)
        {
            spineMatchPrefab.SetActive(true);
            string aniName = skeletonGraphic.SkeletonData.Animations.Items[0].Name;
            skeletonGraphic.AnimationState.SetAnimation(0, aniName, true);
        }
        if (spineVSPrefab)
        {
            spineVSPrefab.SetActive(false);
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
    public void RegistFun()
    {
        //匹配成功
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.SceneInitPush, OnMatchSuccess);
    }
    private void OnMatchSuccess(uint clientCode, ByteString data)
    {

        spineMatchPrefab.SetActive(false);
        spineVSPrefab.SetActive(true);
        string aniName = skeletonGraphicVS.SkeletonData.Animations.Items[0].Name;
        skeletonGraphicVS.AnimationState.SetAnimation(0, aniName, false);
        if (clientCode == 0)
        {
            GameController.Instance().OnInitAllGameModelDataFun(data);
        }
        Debug.Log("输出一下相应了准备");
        StartCoroutine(MatchSuccessFun());
    }
    public IEnumerator MatchSuccessFun()
    {
        yield return new WaitForSeconds(1f);
        OpenUIForm(FormConst.FIGHTMAINPANEL);
        CloseUIForm();
    }
}
