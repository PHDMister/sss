//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachWelfare : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Text text_txt_word;
    private Text text_txt_word2;
    private Text text_txt_word3;
    private Button button_btn_close;
    private GameObject go_img_head;
    private Image img_head;
    private bool IsLoadHeadIcon;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        text_txt_word = FindComp<Text>("raw_bg/img_hor1/txt_word");
        text_txt_word2 = FindComp<Text>("raw_bg/img_hor2/txt_word");
        text_txt_word3 = FindComp<Text>("raw_bg/img_hor3/txt_word");
        button_btn_close = FindComp<Button>("btn_close");
        go_img_head = transform.Find("raw_bg/ri_mask/img_head").gameObject;
        img_head = go_img_head.GetComponent<Image>();

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btn_close, Onbtn_closeClicked);
    }   // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void Onbtn_closeClicked(GameObject go)
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

        WelfareInfoResp resp =  PopBlackData<WelfareInfoResp>("WelfareInfoResp");
        string name = ManageMentClass.DataManagerClass.selfPersonData.login_name;
        text_txt_word.text = TextTools.setCutAddString(name, 8, "...");
        text_txt_word2.text = resp.Days + "天";
        text_txt_word3.text = resp.Count.ToString();

        //img_head
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
