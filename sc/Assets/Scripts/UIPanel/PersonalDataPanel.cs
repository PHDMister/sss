//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Networking;
using WebP;
using UnityEngine.SceneManagement;

public class PersonalDataPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private RawImage rawimage_RawImage;
    public Image Bg_Img;
    private Button Return_Btn;
    private Button EditImage_Btn;
    private Button button_Button_Obj;
    private Text text_Button_Obj;
    private Button button_Button_Land;
    private Text text_Button_Land;
    private Button button_Button_Box;
    private Text text_Button_Box;
    private Button button_Button_Num;
    private Text text_Button_Num;
    private Image image_Tip_Image;
    private Button button_EditData_Btn;
    private Text text_AgeText;
    private Image image_Name_Image;
    private Text text_Name_Text;
    private Image image_icon_boy;
    private Text text_IDValue_Text;
    private Image image_Frame_1000008253;
    private Text text_Text;
    private Text biaotiName_Text;

    private RawImage Head_RawImage;

    private RectTransform rect_Frame_1000008253;

    private Button button_Frame_Button_Obj;
    private Text text_FrameButtonValues_Obj;

    private Image tipsPanel;
    private Image privateTipsPanel;


    private Image ownerImage;
    private Image anOwnerImage;


    //藏品列表
    private CircularScrollView.UICircularScrollView m_CollectionScroll;
    private GameObject CircularContent;
    //土地列表
    private CircularScrollView.UICircularScrollView m_LandScroll;
    private GameObject LandContent;

    //盲盒列表
    private CircularScrollView.UICircularScrollView m_BoxScroll;
    private GameObject BoxContent;

    //数字艺术列表
    private CircularScrollView.UICircularScrollView m_NumScroll;
    private GameObject NumContent;



    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        rawimage_RawImage = FindComp<RawImage>("RawImage");
        Bg_Img = FindComp<Image>("DataPanel/BG_Img");
        Return_Btn = FindComp<Button>("Return_Btn");
        EditImage_Btn = FindComp<Button>("EditImage_Btn");
        button_EditData_Btn = FindComp<Button>("DataPanel/EditData_Btn");
        text_AgeText = FindComp<Text>("DataPanel/TopPanel/AgeImage/AgeText");
        image_Name_Image = FindComp<Image>("DataPanel/TopPanel/Name_Image");
        text_Name_Text = FindComp<Text>("DataPanel/TopPanel/Name_Image/Name_Text");
        image_icon_boy = FindComp<Image>("DataPanel/TopPanel/icon-boy");
        text_IDValue_Text = FindComp<Text>("DataPanel/TopPanel/IDValue_Text");
        image_Frame_1000008253 = FindComp<Image>("DataPanel/TopPanel/Frame 1000008253");
        rect_Frame_1000008253 = FindComp<RectTransform>("DataPanel/TopPanel/Frame 1000008253");
        button_Frame_Button_Obj = FindComp<Button>("DataPanel/TopPanel/Frame 1000008253/Frame_Button");
        text_FrameButtonValues_Obj = FindComp<Text>("DataPanel/TopPanel/Frame 1000008253/Frame_Button/FrameButtonValue_Text");
        biaotiName_Text = FindComp<Text>("DataPanel/DownPanel/biaoti/biaotiName_Text");
        text_Text = FindComp<Text>("DataPanel/TopPanel/Frame 1000008253/Text");
        button_Button_Obj = FindComp<Button>("DataPanel/DownPanel/BtnPanel/Button_Obj");
        text_Button_Obj = FindComp<Text>("DataPanel/DownPanel/BtnPanel/Button_Obj/Value_Text");
        button_Button_Land = FindComp<Button>("DataPanel/DownPanel/BtnPanel/Button_Land");
        text_Button_Land = FindComp<Text>("DataPanel/DownPanel/BtnPanel/Button_Land/Value_Text");
        button_Button_Box = FindComp<Button>("DataPanel/DownPanel/BtnPanel/Button_Box");
        text_Button_Box = FindComp<Text>("DataPanel/DownPanel/BtnPanel/Button_Box/Value_Text");
        button_Button_Num = FindComp<Button>("DataPanel/DownPanel/BtnPanel/Button_Num");
        text_Button_Num = FindComp<Text>("DataPanel/DownPanel/BtnPanel/Button_Num/Value_Text");
        image_Tip_Image = FindComp<Image>("DataPanel/DownPanel/BtnPanel/Tip_Image");

        Head_RawImage = FindComp<RawImage>("DataPanel/TopPanel/Head_RawImage");
        tipsPanel = FindComp<Image>("DataPanel/DownPanel/TipsPanel");
        privateTipsPanel = FindComp<Image>("DataPanel/DownPanel/PrivateTipsPanel");
        m_CollectionScroll = FindComp<CircularScrollView.UICircularScrollView>("DataPanel/DownPanel/CollectionScroViewPanel");
        CircularContent = transform.Find("DataPanel/DownPanel/CollectionScroViewPanel/Content").gameObject;

        m_LandScroll = FindComp<CircularScrollView.UICircularScrollView>("DataPanel/DownPanel/LandScroViewPanel");
        LandContent = transform.Find("DataPanel/DownPanel/LandScroViewPanel/Content").gameObject;

        m_BoxScroll = FindComp<CircularScrollView.UICircularScrollView>("DataPanel/DownPanel/BoxScroViewPanel");
        BoxContent = transform.Find("DataPanel/DownPanel/BoxScroViewPanel/Content").gameObject;

        m_NumScroll = FindComp<CircularScrollView.UICircularScrollView>("DataPanel/DownPanel/NumScroViewPanel");
        NumContent = transform.Find("DataPanel/DownPanel/NumScroViewPanel/Content").gameObject;

        ownerImage = FindComp<Image>("Return_Btn/Owner_Image");
        anOwnerImage = FindComp<Image>("Return_Btn/AnOwner_Image");

        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START


    private void AddEvent()
    {

        RigisterCompEvent(Return_Btn, ReturnBtnClicked);
        RigisterCompEvent(EditImage_Btn, EditorBtnClicked);




        RigisterCompEvent(button_Button_Obj, OnButton_ObjClicked);
        RigisterCompEvent(button_Button_Land, OnButton_LandClicked);
        RigisterCompEvent(button_Button_Box, OnButton_BoxClicked);
        RigisterCompEvent(button_Button_Num, OnButton_NumClicked);
        RigisterCompEvent(button_EditData_Btn, OnEditData_BtnClicked);
        RigisterCompEvent(button_Frame_Button_Obj, OnFrame_BtnClicked);


    }




    // UI EVENT REGISTER END

    // UI EVENT FUNC START

    private Vector3 initPos = new Vector3();
    private int collectCount = 0;
    private int landCount = 0;
    private int boxCount = 0;
    private int NumCount = 0;


    // （藏品、土地、盲盒、数字艺术）列表数据
    private PersonPictureListData personPictureListData = new PersonPictureListData();

    Queue<string> Urls = new Queue<string>();

    private bool isOpen = false;


    private bool IsCanClick = false;
    private float nowTime = 0f;
    private float maxTime = 1f;
    private ulong userId = 1;

    public bool isClickOwner = false;


    private int IsVisible = 0;


    //退出
    private void ReturnBtnClicked(GameObject go)
    {
        CloseUIForm();
        // InterfaceHelper.SetJoyStickState(true);
    }

    //编辑形象
    private void EditorBtnClicked(GameObject go)
    {
        if (IsCanClick)
        {
            //打开个人形象面板
            MessageManager.GetInstance().RequestOutFitList((p) =>
            {
                CloseUIForm();
                OpenUIForm(FormConst.APPEARANCEEDITORUIFORM);
                SendMessage("ReceiveOutFitData", "Success", p);
            });
        }
    }



    private void OnAgeImageClicked(GameObject go)
    {

    }
    //藏品按钮
    private void OnButton_ObjClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);


        m_LandScroll.gameObject.SetActive(false);
        m_CollectionScroll.gameObject.SetActive(false);
        m_BoxScroll.gameObject.SetActive(false);
        m_NumScroll.gameObject.SetActive(false);

        if (!isClickOwner)
        {
            if (IsVisible == 0)
            {
                privateTipsPanel.gameObject.SetActive(true);
                tipsPanel.gameObject.SetActive(false);
                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
            }
            else if (IsVisible == 1)
            {
                if (collectCount > 0)
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(false);


                    m_BoxScroll.gameObject.SetActive(false);
                    m_NumScroll.gameObject.SetActive(false);
                    m_LandScroll.gameObject.SetActive(false);
                    if (!m_CollectionScroll.gameObject.activeSelf)
                    {
                        m_CollectionScroll.gameObject.SetActive(true);
                        m_CollectionScroll.ResetScrollRect();
                        GetCollectionPicDataFun();
                    }
                }
                else
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(true);
                }

            }
        }
        else
        {
            if (collectCount > 0)
            {
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
                m_LandScroll.gameObject.SetActive(false);
                if (!m_CollectionScroll.gameObject.activeSelf)
                {
                    m_CollectionScroll.gameObject.SetActive(true);
                    m_CollectionScroll.ResetScrollRect();
                    GetCollectionPicDataFun();
                }
            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(true);
            }
        }













    }
    //土地按钮
    private void OnButton_LandClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);
        m_LandScroll.gameObject.SetActive(false);
        m_CollectionScroll.gameObject.SetActive(false);
        m_BoxScroll.gameObject.SetActive(false);
        m_NumScroll.gameObject.SetActive(false);

        if (!isClickOwner)
        {
            if (IsVisible == 0)
            {
                privateTipsPanel.gameObject.SetActive(true);
                tipsPanel.gameObject.SetActive(false);
                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
            }
            else if (IsVisible == 1)
            {
                if (landCount > 0)
                {

                    m_CollectionScroll.gameObject.SetActive(false);
                    m_BoxScroll.gameObject.SetActive(false);
                    m_NumScroll.gameObject.SetActive(false);

                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(false);
                    if (!m_LandScroll.gameObject.activeSelf)
                    {
                        m_LandScroll.gameObject.SetActive(true);
                        m_LandScroll.ResetScrollRect();
                        GetLandPicDataFun();
                    }
                }
                else
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(true);
                }

            }

        }
        else
        {

            if (landCount > 0)
            {
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);


                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(false);
                if (!m_LandScroll.gameObject.activeSelf)
                {
                    m_LandScroll.gameObject.SetActive(true);
                    m_LandScroll.ResetScrollRect();
                    GetLandPicDataFun();
                }
            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(true);
            }
        }




    }
    //盲盒按钮
    private void OnButton_BoxClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);







        m_LandScroll.gameObject.SetActive(false);
        m_CollectionScroll.gameObject.SetActive(false);
        m_BoxScroll.gameObject.SetActive(false);
        m_NumScroll.gameObject.SetActive(false);





        if (!isClickOwner)
        {
            if (IsVisible == 0)
            {
                privateTipsPanel.gameObject.SetActive(true);
                tipsPanel.gameObject.SetActive(false);
                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
            }
            else if (IsVisible == 1)
            {
                if (boxCount > 0)
                {
                    m_LandScroll.gameObject.SetActive(false);
                    m_CollectionScroll.gameObject.SetActive(false);
                    m_NumScroll.gameObject.SetActive(false);


                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(false);
                    if (!m_BoxScroll.gameObject.activeSelf)
                    {
                        m_BoxScroll.gameObject.SetActive(true);
                        m_BoxScroll.ResetScrollRect();
                        GetBoxPicDataFun();
                    }
                }
                else
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(true);
                }

            }
        }
        else
        {
            if (boxCount > 0)
            {

                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(false);
                if (!m_BoxScroll.gameObject.activeSelf)
                {
                    m_BoxScroll.gameObject.SetActive(true);
                    m_BoxScroll.ResetScrollRect();
                    GetBoxPicDataFun();
                }
            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(true);
            }
        }


    }
    //数字艺术按钮
    private void OnButton_NumClicked(GameObject go)
    {
        SetBtnTipImgePosFun(go);
        m_LandScroll.gameObject.SetActive(false);
        m_CollectionScroll.gameObject.SetActive(false);
        m_BoxScroll.gameObject.SetActive(false);
        m_NumScroll.gameObject.SetActive(false);

        if (!isClickOwner)
        {
            if (IsVisible == 0)
            {
                privateTipsPanel.gameObject.SetActive(true);
                tipsPanel.gameObject.SetActive(false);
                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);
                m_NumScroll.gameObject.SetActive(false);
            }
            else if (IsVisible == 1)
            {
                if (NumCount > 0)
                {

                    m_LandScroll.gameObject.SetActive(false);
                    m_CollectionScroll.gameObject.SetActive(false);
                    m_BoxScroll.gameObject.SetActive(false);

                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(false);
                    if (!m_NumScroll.gameObject.activeSelf)
                    {
                        m_NumScroll.gameObject.SetActive(true);
                        m_NumScroll.ResetScrollRect();
                        GetNumPicDataFun();
                    }
                }
                else
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(true);
                }

            }
        }
        else
        {

            if (NumCount > 0)
            {

                m_LandScroll.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(false);
                m_BoxScroll.gameObject.SetActive(false);

                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(false);
                if (!m_NumScroll.gameObject.activeSelf)
                {
                    m_NumScroll.gameObject.SetActive(true);
                    m_NumScroll.ResetScrollRect();
                    GetNumPicDataFun();
                }
            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                tipsPanel.gameObject.SetActive(true);
            }
        }


    }

    //编辑个人资料
    private void OnEditData_BtnClicked(GameObject go)
    {

        OpenUIForm(FormConst.EDITPERSONALDATAPANEL);
    }

    //个人签名展开收起
    private void OnFrame_BtnClicked(GameObject go)
    {
        IsOpenFrameFun(isOpen);

    }


    private void IsOpenFrameFun(bool _isOpen)
    {
        if (_isOpen)
        {
            isOpen = false;
            //收起
            rect_Frame_1000008253.sizeDelta = new Vector2(image_Frame_1000008253.transform.GetComponent<RectTransform>().rect.width, 120);
            text_FrameButtonValues_Obj.text = "展开";
        }
        else
        {
            isOpen = true;
            //展开
            rect_Frame_1000008253.sizeDelta = new Vector2(image_Frame_1000008253.transform.GetComponent<RectTransform>().rect.width, 200);
            text_FrameButtonValues_Obj.text = "收起";
        }
    }
    // UI EVENT FUNC END
    private void OnAwake()
    {
        Debug.Log("输出一下具体的userID的内容值： " + ManageMentClass.DataManagerClass.userId);
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        SetIsOwnerUIFun(isClickOwner);

        ReceiveMessage("OpenPersonDataPanelRefreshUI", p =>
        {
            if ((ulong)p.Values == 0)
            {
                isClickOwner = true;
                biaotiName_Text.text = "我的资产";

                ownerImage.gameObject.SetActive(true);
                anOwnerImage.gameObject.SetActive(false);

            }
            else
            {
                isClickOwner = false;
                biaotiName_Text.text = "TA的资产";
                ownerImage.gameObject.SetActive(false);
                anOwnerImage.gameObject.SetActive(true);
            }
            SetIsOwnerUIFun(isClickOwner);
            Debug.Log("输出一下具体的values得值： " + p.Values + "  userId: " + userId);
            if ((ulong)p.Values == 0)
            {
                if (userId != 0)
                {
                    // 刷新数据
                    RefreshUIFun((ulong)p.Values);
                }
            }
            else if (userId != (ulong)p.Values)
            {
                //刷新数据
                RefreshUIFun((ulong)p.Values);
            }
            userId = (ulong)p.Values;

            RTManager.GetInstance().LoadCharacter(userId);
            SetPersonDataToUIFun();
            RefreshPersonImgFun();
            Debug.Log("食醋胡一下visuiblede的值： " + IsVisible);


            /*if (!isClickOwner && IsVisible == 1)
            {
                privateTipsPanel.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(true);
                GetCollectionPicDataFun();

            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(true);
                GetCollectionPicDataFun();
            }*/

            /*

                        if (!isClickOwner)
                        {
                            if (IsVisible == 1)
                            {
                                privateTipsPanel.gameObject.SetActive(false);
                                if (collectCount > 0)
                                {
                                    if (!m_CollectionScroll.gameObject.activeSelf)
                                    {
                                        m_CollectionScroll.gameObject.SetActive(true);
                                        m_CollectionScroll.ResetScrollRect();
                                        GetCollectionPicDataFun();
                                    }
                                    tipsPanel.gameObject.SetActive(false);
                                }
                                else
                                {
                                    tipsPanel.gameObject.SetActive(true);
                                }
                            }
                            else
                            {
                                if (collectCount > 0)
                                {
                                    tipsPanel.gameObject.SetActive(false);
                                }
                                else
                                {
                                    tipsPanel.gameObject.SetActive(true);
                                }
                            }

                        }
                        else
                        {
                            privateTipsPanel.gameObject.SetActive(false);
                            m_CollectionScroll.gameObject.SetActive(true);
                            GetCollectionPicDataFun();
                        }*/


            /*if (!isClickOwner && IsVisible == 1)
            {
            }
            else
            {
                if (collectCount > 0)
                {
                    tipsPanel.gameObject.SetActive(false);
                }
                else
                {
                    tipsPanel.gameObject.SetActive(true);
                }
            }


            if (!isClickOwner && IsVisible == 0)
            {
                m_CollectionScroll.gameObject.SetActive(false);
                privateTipsPanel.gameObject.SetActive(true);
            }
            else
            {
                privateTipsPanel.gameObject.SetActive(false);
                m_CollectionScroll.gameObject.SetActive(true);
                GetCollectionPicDataFun();
            }*/
            SetBtnTipImgePosFun(button_Button_Obj.gameObject);
        });
        IsOpenFrameFun(true);
        ReceiveMessage("EditorPersonalDataPanelSaveData", p =>
        {
            SetPersonDataToUIFun();
        });

        m_CollectionScroll.Listener += CollectionValueChanged;
        m_LandScroll.Listener += LandValueChanged;
        m_BoxScroll.Listener += BoxValueChanged;
        m_NumScroll.Listener += NumValueChanged;

        m_CollectionScroll.Init(CollectionCallBackFun);
        m_LandScroll.Init(LandCallBackFun);
        m_BoxScroll.Init(BoxCallBackFun);
        m_NumScroll.Init(NumCallBackFun);

        tipsPanel.gameObject.SetActive(false);

        m_LandScroll.gameObject.SetActive(false);
        m_BoxScroll.gameObject.SetActive(false);
        m_NumScroll.gameObject.SetActive(false);
    }

    void RefreshUIFun(ulong _userID)
    {
        userId = _userID;
        PersonNumData personNumData = new PersonNumData();
        personNumData.market_types = new string[3] { "art_create", "land", "copyright" };//"star_select",
        personNumData.user_id = _userID;

        CleanPictureDataFun();
        string data = JsonConvert.SerializeObject(personNumData);
        MessageManager.GetInstance().RequestPersonAssetCountData((jo) =>
        {
            Debug.Log("获取的其他人的藏品数量等 " + jo.ToString());
            //设置数量
            var FrameData = jo["list"];
            foreach (var item in FrameData)
            {
                switch (item["market_type"].ToString())
                {
                    case "art_create":
                        collectCount = (int)item["total"];
                        break;
                    case "land":
                        landCount = (int)item["total"];
                        break;
                    case "box":
                        boxCount = (int)item["total"];
                        break;
                    case "copyright":
                        NumCount = (int)item["total"];
                        break;
                }
            }
            Debug.Log("输出一下具体ide内容值： " + jo["visible"]);
            int visValue = (int)jo["visible"];
            IsVisible = visValue;

            Debug.Log("暑促一下 visible的值   " + IsVisible);
            RefreshAllCoutUIFun();
            m_LandScroll.gameObject.SetActive(false);
            m_CollectionScroll.gameObject.SetActive(false);
            m_BoxScroll.gameObject.SetActive(false);
            m_NumScroll.gameObject.SetActive(false);

            if (!isClickOwner)
            {
                if (IsVisible == 0)
                {
                    privateTipsPanel.gameObject.SetActive(true);
                    tipsPanel.gameObject.SetActive(false);
                    m_LandScroll.gameObject.SetActive(false);
                    m_CollectionScroll.gameObject.SetActive(false);
                    m_BoxScroll.gameObject.SetActive(false);
                    m_NumScroll.gameObject.SetActive(false);
                }
                else if (IsVisible == 1)
                {
                    if (collectCount > 0)
                    {
                        privateTipsPanel.gameObject.SetActive(false);
                        tipsPanel.gameObject.SetActive(false);


                        m_BoxScroll.gameObject.SetActive(false);
                        m_NumScroll.gameObject.SetActive(false);
                        m_LandScroll.gameObject.SetActive(false);
                        if (!m_CollectionScroll.gameObject.activeSelf)
                        {
                            m_CollectionScroll.gameObject.SetActive(true);
                            m_CollectionScroll.ResetScrollRect();
                            GetCollectionPicDataFun();
                        }
                    }
                    else
                    {
                        privateTipsPanel.gameObject.SetActive(false);
                        tipsPanel.gameObject.SetActive(true);
                    }

                }
            }
            else
            {
                if (collectCount > 0)
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(false);
                    m_BoxScroll.gameObject.SetActive(false);
                    m_NumScroll.gameObject.SetActive(false);
                    m_LandScroll.gameObject.SetActive(false);
                    if (!m_CollectionScroll.gameObject.activeSelf)
                    {
                        m_CollectionScroll.gameObject.SetActive(true);
                        m_CollectionScroll.ResetScrollRect();
                        GetCollectionPicDataFun();
                    }
                }
                else
                {
                    privateTipsPanel.gameObject.SetActive(false);
                    tipsPanel.gameObject.SetActive(true);
                }
            }


        }, data);

    }


    public override void Display()
    {
        base.Display();
        IsCanClick = false;
        nowTime = 0f;
        Debug.unityLogger.logEnabled = true;

        InterfaceHelper.SetJoyStickState(false);

    }
    public override void Hiding()
    {
        base.Hiding();
        RTManager.GetInstance().DestroyCharacter();
        InterfaceHelper.SetJoyStickState(true);
    }

    public override void Redisplay()
    {
        base.Redisplay();

    }

    public override void Freeze()
    {
        base.Freeze();
    }

    void SetBtnTipImgePosFun(GameObject Obj)
    {
        image_Tip_Image.transform.localPosition = new Vector3(Obj.transform.localPosition.x, image_Tip_Image.transform.localPosition.y, image_Tip_Image.transform.localPosition.z);
    }
    /// <summary>
    /// 根据数据刷新UI
    /// </summary>
    void SetPersonDataToUIFun()
    {
        text_IDValue_Text.text = "ID:" + ManageMentClass.DataManagerClass.personUserData.code;
        text_Name_Text.text = TextTools.setCutAddString(ManageMentClass.DataManagerClass.personUserData.login_name, 8, "...");
        if (ManageMentClass.DataManagerClass.personUserData.age == "")
        {
            text_AgeText.text = "00后 狮子座";
        }
        else
        {
            text_AgeText.text = ManageMentClass.DataManagerClass.personUserData.age + " " + ManageMentClass.DataManagerClass.personUserData.constell;
        }
        if (ManageMentClass.DataManagerClass.personUserData.explain == "")
        {
            text_Text.text = "个人签名:未来世界已到来";
        }
        else
        {
            text_Text.text = "个人签名:" + ManageMentClass.DataManagerClass.personUserData.explain;
        }
        string spriteName = "";
        switch (ManageMentClass.DataManagerClass.personUserData.gender)
        {
            case "男":
                image_icon_boy.gameObject.SetActive(true);
                spriteName = string.Format("{0}", "icon-boy");
                image_icon_boy.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun(spriteName);
                break;
            case "女":
                image_icon_boy.gameObject.SetActive(true);
                spriteName = string.Format("{0}", "icon-gril");
                image_icon_boy.sprite = ManageMentClass.ResourceControllerClass.ResLoadCommonByPathNameFun(spriteName);
                break;
            default:
                image_icon_boy.gameObject.SetActive(false);
                break;
        }


        if (isClickOwner)
        {
            if (CharacterManager.Instance().GetPlayerObj() != null)
            {
                foreach (Transform item in CharacterManager.Instance().GetPlayerObj().GetComponentInChildren<Transform>())
                {
                    if (item.name == "Hud")
                    {
                        foreach (Transform itemA in item.GetComponentInChildren<Transform>())
                        {
                            if (itemA.name == "NamePanel")
                            {
                                LookFollowHud lookFollowHud = itemA.GetComponent<LookFollowHud>();

                                if (lookFollowHud != null)
                                {
                                    lookFollowHud.SetPlayerName(ManageMentClass.DataManagerClass.personUserData.login_name, true);
                                }
                                return;
                            }
                        }
                        return;
                    }
                }
            }
        }

    }
    void RefreshAllCoutUIFun()
    {
        text_Button_Obj.text = collectCount.ToString();
        text_Button_Land.text = landCount.ToString();
        text_Button_Box.text = boxCount.ToString();
        text_Button_Num.text = NumCount.ToString();
    }
    //藏品回调
    void CollectionCallBackFun(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        PersonPicItem personPicItem = cell.transform.GetComponent<PersonPicItem>();
        if (personPicItem != null)
        {
            personPicItem.SetItemIcon(personPictureListData.collectionPictureList[index - 1]);
        }
    }
    //土地回调
    void LandCallBackFun(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        PersonPicItem personPicItem = cell.transform.GetComponent<PersonPicItem>();
        if (personPicItem != null)
        {
            personPicItem.SetItemIcon(personPictureListData.landPictureList[index - 1]);
        }
    }
    //盲盒回调
    void BoxCallBackFun(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        PersonPicItem personPicItem = cell.transform.GetComponent<PersonPicItem>();
        if (personPicItem != null)
        {
            personPicItem.SetItemIcon(personPictureListData.boxPictureList[index - 1]);
        }
    }
    //数字艺术回调
    void NumCallBackFun(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        PersonPicItem personPicItem = cell.transform.GetComponent<PersonPicItem>();
        if (personPicItem != null)
        {
            personPicItem.SetItemIcon(personPictureListData.numPictureList[index - 1]);
        }
    }
    //藏品滑动回调
    void CollectionValueChanged(Vector2 v2)
    {
        if (v2.y <= -0.2f)
        {

            GetCollectionPicDataFun();
        }
    }

    //土地滑动回调
    void LandValueChanged(Vector2 v2)
    {
        if (v2.y <= -0.2f)
        {
            GetLandPicDataFun();
        }

    }
    //盲盒滑动回调
    void BoxValueChanged(Vector2 v2)
    {
        if (v2.y <= -0.2f)
        {
            GetBoxPicDataFun();
        }
    }
    //数字艺术滑动回调
    void NumValueChanged(Vector2 v2)
    {
        if (v2.y <= -0.2f)
        {
            GetNumPicDataFun();
        }
    }
    //获取藏品图数据
    void GetCollectionPicDataFun()
    {
        if (!personPictureListData.isHaveCollection && !DownloadImageManager.GetInstance().IsDownloading && personPictureListData.collectionNowCount < personPictureListData.collectionMaxCount)
        {
            personPictureListData.isHaveCollection = true;
            PersonImageData personImageData = new PersonImageData();
            personImageData.market_type = "art_create";
            personPictureListData.collectionPage++;
            personImageData.page = personPictureListData.collectionPage;
            personImageData.page_size = 3;
            personImageData.user_id = userId;



            string imgData = JsonConvert.SerializeObject(personImageData);

            Debug.Log("输出一下啊具体的书记：： " + imgData);
            Urls.Clear();
            MessageManager.GetInstance().RequestPersonImageData((jo) =>
            {
                Debug.Log("输出一个打他： " + jo.ToString());
                //设置数量
                foreach (var item in jo["data"]["list"])
                {
                    PersonPicture personPicture = new PersonPicture();
                    personPicture.product_title = item["product_title"].ToString();
                    personPicture.product_picture = item["product_picture"].ToString();
                    personPicture.num = (int)item["num"];
                    personPicture.sprite = InterfaceHelper.GetPersonDataDefaultFun();
                    if (personPicture.sprite != null)
                    {
                        Debug.Log("这个图片不为空");
                    }

                    personPictureListData.collectionPictureList.Add(personPicture);
                    Urls.Enqueue(item["product_picture"].ToString());
                    Debug.Log("输出一下这里获取道德数据内容： " + item.ToString());
                }
                Debug.Log("m_Collection的长度：  " + Urls.Count);
                if (Urls.Count > 0)
                {
                    personPictureListData.isHaveCollection = false;
                    m_CollectionScroll.ShowList(personPictureListData.collectionPictureList.Count);

                    DownloadImageManager.GetInstance().DownloadMoreImageFun(Urls, (jo) =>
                    {
                        if (jo.Count > 0)
                        {
                            //图片下载完了 并把图片放入下载的图
                            for (int i = 0; i < jo.Count; i++)
                            {
                                personPictureListData.collectionPictureList[personPictureListData.collectionPictureList.Count - (jo.Count - i)].sprite = jo[i];
                                personPictureListData.collectionNowCount++;
                            }
                            m_CollectionScroll.UpdateList();
                        }
                    });
                }
                //下载
                Debug.Log("获取得值最后得内容： " + jo.ToString());
            }, imgData);
        }
    }
    //获取土地图数据
    void GetLandPicDataFun()
    {
        if (!personPictureListData.isHaveLand && !DownloadImageManager.GetInstance().IsDownloading && personPictureListData.landNowCount < personPictureListData.landMaxCount)
        {
            personPictureListData.isHaveLand = true;
            PersonImageData personImageData = new PersonImageData();
            personImageData.market_type = "land";
            personPictureListData.landPage++;
            personImageData.page = personPictureListData.landPage;
            personImageData.page_size = 3;
            personImageData.user_id = userId;
            string imgData = JsonConvert.SerializeObject(personImageData);
            Urls.Clear();
            Debug.Log("食醋胡下imgdata: " + imgData.ToJSON());
            MessageManager.GetInstance().RequestPersonImageData((jo) =>
            {
                Debug.Log("土地信息中的结果： " + jo.ToString());
                //设置数量
                foreach (var item in jo["data"]["list"])
                {
                    PersonPicture personPicture = new PersonPicture();
                    personPicture.product_title = item["product_title"].ToString();
                    personPicture.product_picture = item["product_picture"].ToString();
                    personPicture.num = (int)item["num"];
                    personPicture.sprite = InterfaceHelper.GetPersonDataDefaultFun();
                    personPictureListData.landPictureList.Add(personPicture);
                    Urls.Enqueue(item["product_picture"].ToString());
                    Debug.Log("输出一下这里获取道德数据内容： " + item.ToString());
                }
                if (Urls.Count > 0)
                {
                    personPictureListData.isHaveLand = false;
                    m_LandScroll.ShowList(personPictureListData.landPictureList.Count);
                    DownloadImageManager.GetInstance().DownloadMoreImageFun(Urls, (jo) =>
                    {
                        if (jo.Count > 0)
                        {
                            //图片下载完了 并把图片放入下载的图
                            for (int i = 0; i < jo.Count; i++)
                            {
                                personPictureListData.landPictureList[personPictureListData.landPictureList.Count - (jo.Count - i)].sprite = jo[i];
                                personPictureListData.landNowCount++;
                            }
                            m_LandScroll.UpdateList();
                        }
                    });
                }
                //下载
                Debug.Log("获取得值最后得内容： " + jo.ToString());
            }, imgData);
        }
    }
    //获取盲盒图数据
    void GetBoxPicDataFun()
    {
        if (!personPictureListData.isHaveBox && !DownloadImageManager.GetInstance().IsDownloading && personPictureListData.boxNowCount < personPictureListData.boxMaxCount)
        {
            personPictureListData.isHaveBox = true;
            PersonImageData personImageData = new PersonImageData();
            personImageData.market_type = "box";
            personPictureListData.boxPage++;
            personImageData.page = personPictureListData.boxPage;
            personImageData.page_size = 3;
            personImageData.user_id = userId;
            string imgData = JsonConvert.SerializeObject(personImageData);
            Urls.Clear();
            MessageManager.GetInstance().RequestPersonImageData((jo) =>
            {
                //设置数量
                foreach (var item in jo["data"]["list"])
                {
                    PersonPicture personPicture = new PersonPicture();
                    personPicture.product_title = item["product_title"].ToString();
                    personPicture.product_picture = item["product_picture"].ToString();
                    personPicture.num = (int)item["num"];
                    personPicture.sprite = InterfaceHelper.GetPersonDataDefaultFun();
                    personPictureListData.boxPictureList.Add(personPicture);
                    Urls.Enqueue(item["product_picture"].ToString());
                    Debug.Log("输出一下这里获取道德数据内容： " + item.ToString());
                }
                if (Urls.Count > 0)
                {
                    personPictureListData.isHaveBox = false;
                    m_BoxScroll.ShowList(personPictureListData.boxPictureList.Count);
                    DownloadImageManager.GetInstance().DownloadMoreImageFun(Urls, (jo) =>
                    {
                        if (jo.Count > 0)
                        {
                            //图片下载完了 并把图片放入下载的图
                            for (int i = 0; i < jo.Count; i++)
                            {
                                personPictureListData.boxPictureList[personPictureListData.boxPictureList.Count - (jo.Count - i)].sprite = jo[i];
                                personPictureListData.boxNowCount++;
                            }
                            m_BoxScroll.UpdateList();
                        }
                    });
                }
                //下载
                Debug.Log("获取得值最后得内容： " + jo.ToString());
            }, imgData);
        }
    }
    //获取数字艺术图数据
    void GetNumPicDataFun()
    {
        if (!personPictureListData.isHaveNum && !DownloadImageManager.GetInstance().IsDownloading && personPictureListData.numNowCount < personPictureListData.numMaxCount)
        {
            personPictureListData.isHaveNum = true;
            PersonImageData personImageData = new PersonImageData();
            personImageData.market_type = "copyright";
            personPictureListData.numPage++;
            personImageData.page = personPictureListData.numPage;
            personImageData.page_size = 3;
            personImageData.user_id = userId;
            string imgData = JsonConvert.SerializeObject(personImageData);
            Urls.Clear();
            MessageManager.GetInstance().RequestPersonImageData((jo) =>
            {
                //设置数量
                foreach (var item in jo["data"]["list"])
                {
                    PersonPicture personPicture = new PersonPicture();
                    personPicture.product_title = item["product_title"].ToString();
                    personPicture.product_picture = item["product_picture"].ToString();
                    personPicture.num = (int)item["num"];
                    personPicture.sprite = InterfaceHelper.GetPersonDataDefaultFun();
                    personPictureListData.numPictureList.Add(personPicture);
                    Urls.Enqueue(item["product_picture"].ToString());
                    Debug.Log("输出一下这里获取道德数据内容： " + item.ToString());
                }
                if (Urls.Count > 0)
                {
                    personPictureListData.isHaveNum = false;
                    Debug.Log("输出一下这个长度： " + personPictureListData.numPictureList.Count);
                    m_NumScroll.ShowList(personPictureListData.numPictureList.Count);
                    DownloadImageManager.GetInstance().DownloadMoreImageFun(Urls, (jo) =>
                    {
                        if (jo.Count > 0)
                        {
                            //图片下载完了 并把图片放入下载的图
                            for (int i = 0; i < jo.Count; i++)
                            {
                                Debug.Log("输出一下这里的index的只： " + (personPictureListData.numPictureList.Count - (jo.Count - i)));
                                personPictureListData.numPictureList[personPictureListData.numPictureList.Count - (jo.Count - i)].sprite = jo[i];
                                personPictureListData.numNowCount++;
                            }
                            m_NumScroll.UpdateList();
                        }
                    });
                }
                //下载
                Debug.Log("获取得值最后得内容： " + jo.ToString());
            }, imgData);
        }
    }
    private void Update()
    {
        if (!IsCanClick)
        {
            if (nowTime < maxTime)
            {
                nowTime += Time.deltaTime;
            }
            else
            {
                IsCanClick = true;
            }
        }
    }

    private void CleanPictureDataFun()
    {
        collectCount = 0;
        landCount = 0;
        boxCount = 0;
        NumCount = 0;

        personPictureListData.isHaveCollection = false;
        personPictureListData.collectionPage = 0;
        personPictureListData.collectionMaxCount = 40;
        personPictureListData.collectionNowCount = 0;
        personPictureListData.collectionPictureList.Clear();

        personPictureListData.isHaveLand = false;
        personPictureListData.landPage = 0;
        personPictureListData.landMaxCount = 30;
        personPictureListData.landNowCount = 0;
        personPictureListData.landPictureList.Clear();


        personPictureListData.isHaveBox = false;
        personPictureListData.boxPage = 0;
        personPictureListData.boxMaxCount = 20;
        personPictureListData.boxNowCount = 0;
        personPictureListData.boxPictureList.Clear();


        personPictureListData.isHaveNum = false;
        personPictureListData.numPage = 0;
        personPictureListData.numMaxCount = 20;
        personPictureListData.numNowCount = 0;
        personPictureListData.numPictureList.Clear();


        m_CollectionScroll.ShowList(0);
        m_LandScroll.ShowList(0);
        m_BoxScroll.ShowList(0);
        m_NumScroll.ShowList(0);

    }

    private void SetIsOwnerUIFun(bool isOwner)
    {
        Debug.Log("输出一下isowner的数据值： " + isOwner);
        if (isOwner)
        {
            button_EditData_Btn.gameObject.SetActive(true);
            EditImage_Btn.gameObject.SetActive(true);
        }
        else
        {
            button_EditData_Btn.gameObject.SetActive(false);
            EditImage_Btn.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 刷新头像
    /// </summary>
    void RefreshPersonImgFun()
    {

        string picturePath = "";
        if (userId == 0 || userId == 1)
        {
            //自己
            userId = ManageMentClass.DataManagerClass.userId;
            Head_RawImage.texture = null;
        }
        else
        {
            Head_RawImage.texture = null;
        }
        if (SceneManager.GetActiveScene().name == "gerenkongjian01")
        {
            foreach (var item in Singleton<ParlorController>.Instance.RoomData.UserList)
            {
                if (userId == item.UserId)
                {
                    picturePath = item.PicUrl;
                }
            }
        }

        else if (SceneManager.GetActiveScene().name == "TreasureHunt")
        {
            foreach (var item in Singleton<TreasuringController>.Instance.GetTreasurRoomInfo())
            {
                if (userId == item.UserId)
                {
                    picturePath = item.PicUrl;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Island01")
        {
            if (Singleton<RainbowBeachController>.Instance.Players.ContainsKey(userId))
            {
                picturePath = Singleton<RainbowBeachController>.Instance.Players[userId].UserInfo.PicUrl;
            }
        }
        else if (SceneManager.GetActiveScene().name == "fish")
        {
            if (Singleton<RainbowIocnController>.Instance.Players.ContainsKey(userId))
            {
                picturePath = Singleton<RainbowIocnController>.Instance.Players[userId].UserInfo.PicUrl;
            }
        }
        if (picturePath != "")
        {
            Debug.Log("输出一下具体的路径地址：" + picturePath);
            StartCoroutine(DownloadImageFun(picturePath));
        }
        else
        {
            Head_RawImage.texture = InterfaceHelper.GetPersonDataDefaultFun().texture;
        }

    }


    private IEnumerator DownloadImageFun(string url)
    {
        if (url != "")
        {
            Sprite createSprite = null;
            string urlExt = InterfaceHelper.GetUrlExtension(url);
            if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp"))
            {
                if (ManageMentClass.DataManagerClass.PlatformType == 2)
                {
                    url = string.Format("{0}{1}", url, "?x-oss-process=style/100W");
                }
                else
                {
                    url = string.Format("{0}{1}", url, "?x-oss-process=style/200W");
                }

                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("这里报错了： " + www.result);

                    createSprite = InterfaceHelper.GetPersonDataDefaultFun();

                }
                else
                {
                    Error lError;
                    Texture2D myTexture = null;
                    myTexture = Texture2DExt.CreateTexture2DFromWebP(www.downloadHandler.data, true, true, out lError);
                    if (lError == Error.Success)
                    {
                        createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                    }
                    else
                    {
                        Debug.LogError("Webp Load Error : " + lError.ToString());
                        createSprite = InterfaceHelper.GetPersonDataDefaultFun();
                    }

                }
                Debug.Log("下载成功了");
                Head_RawImage.texture = createSprite.texture;

            }
            else
            {
                Debug.LogError("文件格式错误，输出一下错误得图片地址： " + url);
                createSprite = InterfaceHelper.GetPersonDataDefaultFun();
                Head_RawImage.texture = createSprite.texture;
            }
        }
        else
        {
            Debug.Log("输出一下 这个图片地址未找到，需要查找");
            Sprite createSprite = null;
            createSprite = InterfaceHelper.GetPersonDataDefaultFun();
            Head_RawImage.texture = createSprite.texture;
        }

    }


}
