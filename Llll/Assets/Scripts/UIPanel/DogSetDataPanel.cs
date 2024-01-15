using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.U2D;

public class DogSetDataPanel : BaseUIForm
{

    public Button ColseThisPanelBtn;
    public Button m_ExchangeBtn;
    public Text DogName_Text;
    public GameObject DogGenderRoot;
    public Text DogState_Text;


    public Slider Hunger_Slider;
    public Slider Mood_Slider;
    public Slider Clean_Slider;
    public Slider Health_Slider;

    public Image HungerSlider_Img;
    public Image MoodSlider_Img;
    public Image CleanSlider_Img;
    public Image HealthSlider_Img;

    public Text HungerSliderValue_Text;
    public Text MoodSliderValue_Text;
    public Text CleanSliderValue_Text;
    public Text HealthSliderValue_Text;

    public Text SerialNumber_Text;


    //提示恢复心情，清洁，健康 面板

    public GameObject OtherSetDataPanel;
    public Text Title_Text;
    public Text OtherPanelGasValue_Text;
    public Text OtherPanelTips_Text;

    //提示恢复饥饿 面板
    public GameObject FeedPetDataPanel;
    public Text FeedCount_Text;
    public InputField m_InputFieldNum;
    public Text FeedGasValue_Text;
    public Text m_CostText;
    public Button SubtractCount_Btn;
    public Button AddCount_Btn;
    public Button m_BtnMax;
    // public Slider SelectCount_Slider;
    public Text ExceedValue_Text;
    //public Image GasValueBG_Image;
    public GameObject ExceedPanel;

    public Transform[] ArrPetBtnTran;

    //训练相关

    //成长值
    public Slider GrowthValue_Slider;
    public Text GrowthValue_Text;
    //体格值
    public Text BodyData_Text;
    //智慧
    public Text WisdomData_Text;
    //幸运
    public Text LuckyData_Text;
    //可训练次数/总数
    public Text TrainingCount_Text;

    //训练面板
    public GameObject Train_Panel;
    //正在训练面板
    public GameObject Training_Panel;

    //训练提示内容
    public Text PetTrainTypeTip_Text;
    //训练倒计时
    public Text PetTrainTime_Text;

    public Button Train_Btn;


    object[] arrAniMessageObj = new object[2];

    // 数据

    //狗的数据
    PetModelRecData petModelRecData;
    //喂养价格数据
    pet_keeping petKeepingData;
    //发送的数据
    DogPetFeedData petAdoptData;


    //喂养次数
    int FeedCount = 1;
    //可以喂养的次数
    int CanFeedCount = 0;
    //单次喂养需要增加的进度
    float FeedAddSliderValue = 0f;
    //进度条初始进度
    float FeedSliderValue = 0f;
    //可以增加喂养的点数
    int CanAddFeedValue = 0;

    int ConditionTypeId = 0;


    private int SecondTimeValue;

    // 狗的 condition_type ID , 对应的进度最大值 
    Dictionary<int, PetSliderValueData> DogConditionValuesData = new Dictionary<int, PetSliderValueData>();

    Color Newcolor;

    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency; //半透明，不能穿透
        OtherSetDataPanel.SetActive(false);
        FeedPetDataPanel.SetActive(false);
        //接收事件数据
        ReceiveMessageFun();
    }
    private void Start()
    {
        RigisterButtonObjectEvent("Hunger_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("宠物正在训练"), 5f);
                return;
            }

            if (DogConditionValuesData[0].petcondition.good_max > DogConditionValuesData[0].cur_val)
            {
                //喂食
                SetTipPanelData(0);
            }
            else
            {
                ToastManager.Instance.ShowNewToast("饥饿度已满，无需喂食", 8f);
            }
        });
        RigisterButtonObjectEvent("Mood_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("宠物正在训练"), 5f);
                return;
            }
            if (DogConditionValuesData[2].petcondition.good_max > DogConditionValuesData[2].cur_val)
            {
                //玩具
                SetTipPanelData(2);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("心情度已满，无需玩具", 8f);
            }

        });
        RigisterButtonObjectEvent("Clean_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("宠物正在训练"), 5f);
                return;
            }
            if (DogConditionValuesData[1].petcondition.good_max > DogConditionValuesData[1].cur_val)
            {
                //清洁
                SetTipPanelData(1);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("清洁度已满，无需清洁", 8f);
            }

        });
        RigisterButtonObjectEvent("Health_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("宠物正在训练"), 5f);
                return;
            }
            if (DogConditionValuesData[3].petcondition.good_max > DogConditionValuesData[3].cur_val)
            {
                //治疗
                SetTipPanelData(3);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("健康度已满，无需治疗", 8f);
            }

        });
        // otherPanel 面板的按钮
        RigisterButtonObjectEvent("OtherSetConfirmPaymentButton", p =>
        {
            Debug.Log("输出一下内容值价值：： " + petKeepingData.price);
            if (petKeepingData.price <= ManageMentClass.DataManagerClass.gas_Amount)
            {
                Debug.Log("发送数据");
                SendServerFun();
            }
            else
            {
                Debug.Log("显示不足");
                //gas不足
                OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
            }

        });
        RigisterButtonObjectEvent("OtherSetPanelCloseBtn", p =>
        {
            //关闭面板
            if (OtherSetDataPanel.activeSelf)
            {
                OtherSetDataPanel.SetActive(false);
            }
        });
        //FeedPetpanel面板的按钮
        RigisterButtonObjectEvent("FeedPetArrifmButton", p =>
        {
            Debug.Log("输出一下内容值价值：： " + (petKeepingData.price * FeedCount) + "   FeedCount: " + FeedCount);
            //支付 
            if (petKeepingData.price * FeedCount <= ManageMentClass.DataManagerClass.gas_Amount)
            {
                Debug.Log("内容值无问题：并且小于总gas ");
                SendServerFun();
            }
            else
            {
                Debug.Log("内容值有问题：并且大于总gas ");
                //gas不足
                OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
            }
        });


        RigisterButtonObjectEvent("FeedPetCloseBtn", p =>
        {
            //关闭 
            if (FeedPetDataPanel.activeSelf)
            {
                FeedPetDataPanel.SetActive(false);
            }
        });
        //训练按钮
        RigisterButtonObjectEvent("Training_Button", p =>
        {
            JudgeTrainPetFun();
        });
        //结束训练按钮
        RigisterButtonObjectEvent("TrainingFinish_Button", p =>
        {
            //结束训练
            OpenUIForm(FormConst.PETTRAINFINISHPANEL);
            SendMessage("PetTrainFinishPanelTime", "", petModelRecData);
        });

        //查看介绍按钮
        RigisterButtonObjectEvent("PetTrainCheckTips_Btn", p =>
        {
            //结束训练
            OpenUIForm(FormConst.PETTRAINtIPSUIFORM);
        });

        RigisterButtonObjectEvent("ExchangeBtn", p =>
        {
            if (!bCanExchange())
            {
                ToastManager.Instance.ShowPetToast("宠物的各项状态要达到60以上，才可兑换实体犬领养凭证哦~", 3f);
                return;
            }
            OpenUIForm(FormConst.DogExchangeTips);
        });

        SubtractCount_Btn.onClick.AddListener(SubtractCountBtnFun);
        AddCount_Btn.onClick.AddListener(AddCountBtnFun);
        m_BtnMax.onClick.AddListener(OnClickMax);

        SetRefreshDogMaxValuesFun();

        ColseThisPanelBtn.onClick.AddListener(ColseThisPanelFun);
        // SelectCount_Slider.onValueChanged.AddListener(SelectCountSliderCalveChangedFun);
    }


    /// <summary>
    /// 接收事件数据管理
    /// </summary>
    public void ReceiveMessageFun()
    {
        //打开面板时接收狗的数据( 参数传过来 狗的ID);
        ReceiveMessage("RefreshDogPanel",
                  p =>
                  {
                      if (!gameObject.activeSelf)
                      {
                          return;
                      }

                      int dogID = (int)p.Values;
                      Debug.Log("输出一下DOgID的值： " + dogID);
                      SetDogPanelData(dogID);
                      SetDogTrainShowStateFun();

                      if (petModelRecData.train_info != null)
                      {
                          SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                          Debug.Log("开启协程");
                          StartCoroutine(CountDownTimerFun());
                      }

                  }
             );
        ReceiveMessage("LoopUpdatePetData",
                  p =>
                  {
                      if (!gameObject.activeSelf)
                      {
                          return;
                      }

                      //刷新各项进度值
                      RefreshDogDataFun();
                      SetDogTrainShowStateFun();
                      if (petModelRecData.train_info != null)
                      {
                          SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                      }

                  }
             );
        ReceiveMessage("CloseShopGasTipsUiFormPanel",
              p =>
              {
                  if (!gameObject.activeSelf)
                  {
                      return;
                  }

                  //关闭面板
                  if (FeedPetDataPanel.activeSelf)
                  {
                      FeedPetDataPanel.SetActive(false);
                  }
                  if (OtherSetDataPanel.activeSelf)
                  {
                      OtherSetDataPanel.SetActive(false);
                  }

              }
         );
        //开始训练  消息接收
        ReceiveMessage("BeginPetTrain",
            p =>
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }
                petModelRecData = p.Values as PetModelRecData;
                //开始训练，开始倒计时
                SetDogTrainShowStateFun();
                PetSpanManager.Instance().SetCameraTrainFun();
                if (petModelRecData.train_info != null)
                {
                    Debug.Log("开启协程2");

                    SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                    StartCoroutine(CountDownTimerFun());
                }
            }
        );

        //训练时心情不佳的确认按钮
        ReceiveMessage("PetTrainMoodDownTipPanelAffirmBtn",
            p =>
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }
                //打开训练面板
                if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
                {
                    //
                    OpenUIForm(FormConst.SHELTERPETTRAINPANEL);
                }
                else
                {
                    OpenUIForm(FormConst.PETTRAINPANEL);
                }

                SendMessage("OpenTrainPanel", "Success", petModelRecData);
            }
        );
        ReceiveMessage("ClosePetTrainMessage",
            p =>
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }
                //结束训练了
                Train_Panel.SetActive(true);
                Training_Panel.SetActive(false);
                StopCoroutine(CountDownTimerFun());
                Debug.Log("关闭协程了");
            }
        );
        ReceiveMessage("RefreshPetTrainDataUIMessage",
           p =>
           {

               if (!gameObject.activeSelf)
               {
                   return;
               }
               //结束训练了
               petModelRecData = p.Values as PetModelRecData;
               RefreshPetTrainDataUIFun();
           }
       );

        ReceiveMessage("RefreshPetTrainDogData",
           p =>
           {
               if (!gameObject.activeSelf)
               {
                   return;
               }
               petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id, petModelRecData.pet_box_id);
               Debug.Log("刷新了数据内容");
           }
       );





    }
    //显示
    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
    }
    //隐藏
    public override void Hiding()
    {
        base.Hiding();
        PetSpanManager.Instance().CancelLookAtPet();
        InterfaceHelper.SetJoyStickState(true);
        StopCoroutine(CountDownTimerFun());
    }
    /// <summary>
    /// 设置面板是否在训练状态
    /// </summary>
    public void SetDogTrainShowStateFun()
    {
        if (petModelRecData.train_info != null)
        {
            //正在训练
            Train_Panel.SetActive(false);
            Training_Panel.SetActive(true);
            SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
            PetTrainTime_Text.text = CalcTools.ChangeSecondFun(SecondTimeValue);
            PetTrainTypeTip_Text.text = ManageMentClass.DataManagerClass.GetTrainTableFun(petModelRecData.train_info.train_id).name + "中...";



        }
        else
        {
            //不在训练
            Train_Panel.SetActive(true);
            Training_Panel.SetActive(false);
        }
    }

    /// <summary>
    /// 判断是否可以开始训练
    /// </summary>
    public void JudgeTrainPetFun()
    {
        if (Train_Btn.enabled == false)
        {
            return;
        }
        if (petModelRecData.pet_remain_train_num > 0)
        {
            //可训练次数大于0

            for (int i = 0; i < 4; i++)
            {
                Debug.Log("DogConditionValuesData" + DogConditionValuesData[i].cur_val + "  i  : " + i);
            }

            if (DogConditionValuesData[0].cur_val > 60 && DogConditionValuesData[1].cur_val > 60 && DogConditionValuesData[3].cur_val > 60)
            {
                if (DogConditionValuesData[2].cur_val < 20)
                {
                    //弹出心情低落确认面板
                    OpenUIForm(FormConst.PETTRAINMOODDOWNTIPPANEL);
                }
                else
                {
                    //打开训练面板
                    //打开训练面板
                    if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
                    {
                        //
                        OpenUIForm(FormConst.SHELTERPETTRAINPANEL);
                    }
                    else
                    {
                        OpenUIForm(FormConst.PETTRAINPANEL);
                    }
                    SendMessage("OpenTrainPanel", "Success", petModelRecData);
                }
            }
            else
            {
                ToastManager.Instance.ShowPetToast("需饱食度、清洁度、健康度均达到60以上才可进行训练", 8f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("你的宠物非常累了，不能再训练了", 8f);
        }
    }

    /// <summary>
    /// 判断是否可以兑换
    /// </summary>
    public bool bCanExchange()
    {
        bool bExchange = true;
        if (DogConditionValuesData == null)
            return false;
        foreach (var data in DogConditionValuesData)
        {
            if (data.Value.cur_val <= 60)
            {
                bExchange = false;
                break;
            }
        }
        return bExchange;
    }

    public void SetTipPanelData(int typeID)
    {
        petKeepingData = GetConsumeDataFun(typeID + 3);
        ConditionTypeId = typeID;
        if (typeID == 0)
        {
            SetFeedPetDataPanelFun();
        }
        else
        {
            SetOtherDataPanelFun(typeID);
        }
    }

    //public void AdjustGasUI()
    //{
    //    FeedGasValue_Text.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
    //    float width = InterfaceHelper.CalcTextWidth(FeedGasValue_Text);
    //    float height = GasValueBG_Image.transform.GetComponent<RectTransform>().sizeDelta.y;
    //    GasValueBG_Image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(230 + width, height);
    //}
    void SendServerFun()
    {
        if (ConditionTypeId == 0)
        {
            petAdoptData = new DogPetFeedData(petModelRecData.id, ConditionTypeId, FeedCount);//宠物ID，喂养类型
        }
        else
        {
            petAdoptData = new DogPetFeedData(petModelRecData.id, ConditionTypeId, 1);//宠物ID，喂养类型
        }
        string Strdata = JsonConvert.SerializeObject(petAdoptData);
        Debug.Log("strData: " + Strdata);
        MessageManager.GetInstance().RequestFeed(Strdata, (JObject jo) =>
        {
            if (jo == null)
            {
                Debug.Log("jo为空没有传进来");
            }
            else
            {
                Debug.Log("jo传进来");
            }
            // PetSpanManager.Instance().PlayAddLoveCoinAni(imgClean, p.remain_lovecoin);
            MessageManager.GetInstance().RequestGasValue();
            //刷新数据
            FeedsucceedAllDataFun(jo);
            SendPetAnimationMessageFun();
        });
    }
    public void FlyLoveCoinFun(int index, int loveCoinCount)
    {
        if (index < ArrPetBtnTran.Length && ArrPetBtnTran.Length > 0 && loveCoinCount > 0)
        {
            PetSpanManager.Instance().PlayAddLoveCoinAni(ArrPetBtnTran[index], loveCoinCount);
        }
    }

    public void SendPetAnimationMessageFun()
    {
        switch (ConditionTypeId)
        {
            case 0:
                arrAniMessageObj[0] = PetStateAnimationType.Feed;
                arrAniMessageObj[1] = petModelRecData.id;
                SendMessage("PetStateAnimationTypeMes", "Success", arrAniMessageObj);
                break;
            case 1:
                arrAniMessageObj[0] = PetStateAnimationType.Clean;
                arrAniMessageObj[1] = petModelRecData.id;
                SendMessage("PetStateAnimationTypeMes", "Success", arrAniMessageObj);
                break;
            case 2:
                arrAniMessageObj[0] = PetStateAnimationType.Toy;
                arrAniMessageObj[1] = petModelRecData.id;
                SendMessage("PetStateAnimationTypeMes", "Success", arrAniMessageObj);
                break;
            case 3:
                arrAniMessageObj[0] = PetStateAnimationType.Cure;
                arrAniMessageObj[1] = petModelRecData.id;
                SendMessage("PetStateAnimationTypeMes", "Success", arrAniMessageObj);
                break;
        }
    }
    /// <summary>
    /// 喂养成功后 的数据刷新
    /// </summary>
    public void FeedsucceedAllDataFun(JObject jo)
    {
        Debug.Log("输出一下 ho的值 ：" + jo);
        //喂养成功
        if (jo["code"].ToString() == "0")
        {
            var conditionData = jo["list"];
            var key = PetSpanManager.Instance().GetPetModelPos(petModelRecData.id);
            foreach (var item in conditionData)
            {
                for (int i = 0; i < ManageMentClass.DataManagerClass.petModelRecData[key].pet_condition.Count; i++)
                {
                    if ((int)item["condition_type"] == ManageMentClass.DataManagerClass.petModelRecData[key].pet_condition[i].condition_type)
                    {
                        ManageMentClass.DataManagerClass.petModelRecData[key].pet_condition[i].cur_val = (int)item["cur_val"];
                    }
                }
            }
            if ((int)jo["reward_lovecoin"] > 0)
            {
                //ManageMentClass.DataManagerClass.loveCoin = (int)jo["remain_lovecoin"];
                //  SendMessage("UpdateLoveCoin", "Value", null);
                FlyLoveCoinFun(ConditionTypeId, (int)jo["remain_lovecoin"]);
            }
            SendMessage("FeedPetSuccess", "Success", petModelRecData.id);
            if (FeedPetDataPanel.activeSelf)
            {
                FeedPetDataPanel.SetActive(false);
            }
            if (OtherSetDataPanel.activeSelf)
            {
                OtherSetDataPanel.SetActive(false);
            }
            RefreshDogDataFun();
        }
        else
        {
            //失败
        }
    }
    public void ColseThisPanelFun()
    {
        CloseUIForm();
        Debug.Log("关闭面板内容");
    }
    //设置其他面板数据
    void SetOtherDataPanelFun(int typeID)
    {
        if (!OtherSetDataPanel.activeSelf)
        {
            OtherSetDataPanel.SetActive(true);
        }
        if (petKeepingData != null)
        {
            Debug.Log("petKeepingData这里的内容不为空");
        }


        Debug.Log(" petKeepingData.price： " + petKeepingData.price);
        switch (typeID)
        {
            case 1:
                Title_Text.text = "清洁";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "恢复所有清洁值";
                break;
            case 2:
                Title_Text.text = "玩具";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "恢复所有心情值";
                break;
            case 3:
                Title_Text.text = "治疗";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "恢复所有健康值";
                break;
            default:
                break;
        }
    }
    //设置喂养健康值的面板数据
    void SetFeedPetDataPanelFun()
    {
        if (!FeedPetDataPanel.activeSelf)
        {
            FeedPetDataPanel.SetActive(true);
        }
        //初始化数据
        FeedCount = 1;
        FeedAddSliderValue = 0f;
        CanFeedCount = 0;
        //初始化面板
        m_CostText.text = string.Format("每消耗{0}GAS增加10点饱食度", petKeepingData.price);
        //FeedCount_Text.text = FeedCount + "";
        m_InputFieldNum.text = string.Format("{0}", FeedCount);
        //  SelectCount_Slider.value = 0;
        FeedGasValue_Text.text = petKeepingData.price + "";
        //AdjustGasUI();
        CanAddFeedValue = DogConditionValuesData[0].petcondition.good_max - DogConditionValuesData[0].cur_val;


        if (CanAddFeedValue > 0)
        {
            CanFeedCount = Mathf.CeilToInt((float)CanAddFeedValue / 10);
            //计算出单次喂养内容
            FeedAddSliderValue = 1 / (float)(CanFeedCount - 1);
            ExceedPanel.SetActive(false);
            ExceedValue_Text.text = "0";
        }
    }
    /// <summary>
    /// 选择进度条的值
    /// </summary>
    public void SelectCountSliderCalveChangedFun(float value)
    {
        if (CanAddFeedValue > 0 && FeedAddSliderValue > 0)
        {
            int feedCount = Mathf.FloorToInt(value / FeedAddSliderValue);
            FeedCount = feedCount + 1;
            FeedSliderValue = value;
            if (value >= 1)
            {
                FeedCount = CanFeedCount;
            }
            FeedGasValue_Text.text = petKeepingData.price * FeedCount + "";
            //AdjustGasUI();
            FeedCount_Text.text = FeedCount + "";

        }
        Debug.Log("输出一下内容value的值： " + value);
    }
    //减次数
    public void SubtractCountBtnFun()
    {
        if (CanAddFeedValue > 0)
        {
            if (FeedCount > 1)
            {
                FeedCount--;
                //  SelectCount_Slider.value = (FeedCount - 1) * FeedAddSliderValue;
                m_InputFieldNum.text = string.Format("{0}", FeedCount);
                FeedGasValue_Text.text = string.Format("{0}", petKeepingData.price * FeedCount);
                SetOutValueFun();
            }
        }
        //SelectCount_Slider.value += 0.1f;

    }
    //加次数
    public void AddCountBtnFun()
    {
        if (CanAddFeedValue > 0)
        {
            if (FeedCount < CanFeedCount)
            {
                FeedCount++;
                //SelectCount_Slider.value = (FeedCount - 1) * FeedAddSliderValue;
                m_InputFieldNum.text = string.Format("{0}", FeedCount);
                FeedGasValue_Text.text = string.Format("{0}", petKeepingData.price * FeedCount);
                SetOutValueFun();
            }
        }

    }

    /// <summary>
    ///最大次数
    /// </summary>
    public void OnClickMax()
    {
        FeedCount = CanFeedCount;
        m_InputFieldNum.text = string.Format("{0}", FeedCount);
        FeedGasValue_Text.text = string.Format("{0}", petKeepingData.price * FeedCount);
        SetOutValueFun();
    }


    private void SetOutValueFun()
    {
        if (CanAddFeedValue < FeedCount * 10)
        {
            //溢出状态 
            ExceedValue_Text.text = (FeedCount * 10 - CanAddFeedValue) + "";
            ExceedPanel.SetActive(true);
        }
        else
        {
            ExceedValue_Text.text = 0 + "";
            ExceedPanel.SetActive(false);
        }
    }


    /// <summary>
    /// 设置面板的值
    /// </summary>
    void SetDogPanelData(int dogID)
    {
        Debug.Log("狗的ID： " + dogID);
        petModelRecData = PetSpanManager.Instance().GetPetModelData(dogID);
        if (petModelRecData != null)
        {
            //刷新查找最大进度值
            SetRefreshDogMaxValuesFun();
            DogName_Text.text = petModelRecData.pet_type == 2 || petModelRecData.pet_type == 4 ? string.Format("<color=#4DABCF>{0}</color>", TextTools.setCutAddString(petModelRecData.pet_name, 7, "...")) : string.Format("<color=#ED716B>{0}</color>", TextTools.setCutAddString(petModelRecData.pet_name, 7, "..."));
            SerialNumber_Text.text = "编号：" + petModelRecData.pet_number;
            if (petModelRecData.pet_type == 2 || petModelRecData.pet_type == 4)
            {
                //公
                DogGenderRoot.transform.GetChild(0).gameObject.SetActive(true);
                DogGenderRoot.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                //母
                DogGenderRoot.transform.GetChild(0).gameObject.SetActive(false);
                DogGenderRoot.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (petModelRecData.pet_type == 2 || petModelRecData.pet_type == 3)
            {
                DogState_Text.text = "幼犬";
            }
            else
            {
                DogState_Text.text = "成犬";
            }
            //刷新各项进度值
            RefreshDogDataFun();
        }
    }
    /// <summary>
    /// 刷新dog的各项进度
    /// </summary>
    void RefreshDogDataFun()
    {
        FeedCount = 1;
        petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id);
        SetRefreshDogMaxValuesFun();
        //刷新进度条
        if (petModelRecData != null)
        {
            for (int i = 0; i < petModelRecData.pet_condition.Count; i++)
            {
                Debug.Log("petModelRecData.pet_condition[i].condition_type::   " + petModelRecData.pet_condition[i].condition_type + "   petModelRecData.pet_condition[i].cur: " + petModelRecData.pet_condition[i].cur_val);
                switch (petModelRecData.pet_condition[i].condition_type)
                {
                    case 0:
                        SetHungerSliderFun(0);
                        break;
                    case 1:
                        SetCleanSliderFun(1);
                        break;
                    case 2:
                        SetMoodSliderFun(2);
                        break;
                    case 3:
                        SetHealthSliderFun(3);
                        break;
                    default:
                        break;
                }
            }
        }
        RefreshPetTrainDataUIFun();
    }
    void RefreshPetTrainDataUIFun()
    {
        //训练相关
        if (petModelRecData.lo_exp >= petModelRecData.exp)
        {
            //已达到最大实体上限
            GrowthValue_Slider.value = 1;
            GrowthValue_Text.text = "MAX";
            //按钮置灰
            /*Train_Btn.enabled = false;
            TrainTip_Text.text = "无需训练";
            TrainingCount_Text.gameObject.SetActive(false);*/

            Train_Btn.gameObject.SetActive(false);
            m_ExchangeBtn.gameObject.SetActive(true);
        }
        else
        {
            Train_Btn.gameObject.SetActive(true);
            m_ExchangeBtn.gameObject.SetActive(false);

            Train_Btn.enabled = true;
            TrainingCount_Text.gameObject.SetActive(true);
            float sliderValue = (float)petModelRecData.lo_exp / petModelRecData.exp;
            GrowthValue_Slider.value = sliderValue;
            GrowthValue_Text.text = petModelRecData.lo_exp + "/" + petModelRecData.exp;
            TrainingCount_Text.text = petModelRecData.pet_remain_train_num + "/" + petModelRecData.pet_train_total;
        }
        for (int i = 0; i < petModelRecData.pet_attribute.Count; i++)
        {
            switch (petModelRecData.pet_attribute[i].attribute_type)
            {
                case 1:
                    //体格
                    BodyData_Text.text = "体格:" + petModelRecData.pet_attribute[i].cur_val;
                    break;
                case 2:
                    //智慧
                    WisdomData_Text.text = "智慧:" + petModelRecData.pet_attribute[i].cur_val; ;
                    break;
                case 3:
                    //幸运
                    LuckyData_Text.text = "幸运:" + petModelRecData.pet_attribute[i].cur_val; ;
                    break;
            }
        }
        //打开结果面板
        if (petModelRecData.train_result != null)
        {
            //有训练结果
            OpenUIForm(FormConst.PETTRAINRESULTTIPPANEL);
            SendMessage("OpemPetTrainResultTipPanel", "Success", petModelRecData);
        }
    }


    /// <summary>
    /// 设置饥饿值进度
    /// </summary>
    void SetHungerSliderFun(int conditionTypeID)
    {
        Hunger_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        HungerSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(HungerSlider_Img, Hunger_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// 设置心情值进度
    /// </summary>
    void SetMoodSliderFun(int conditionTypeID)
    {
        Mood_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        MoodSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(MoodSlider_Img, Mood_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// 设置清洁值进度
    /// </summary>
    void SetCleanSliderFun(int conditionTypeID)
    {
        Clean_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        CleanSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(CleanSlider_Img, Clean_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// 设置健康值进度
    /// </summary>
    void SetHealthSliderFun(int conditionTypeID)
    {
        Health_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        HealthSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(HealthSlider_Img, Health_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// 设置图片的颜色
    /// </summary>
    /// <param name="img">图片</param>
    /// <param name="value">百分比值</param>
    public void SetImageColorFun(Image img, float value, int conditionTypeID)
    {
        //#FF3B30 红色
        //#FFC800 黄色+
        //#5DBC82 绿色

        string colorDecimal = "";
        float goodMax = (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        float goodMin = (float)DogConditionValuesData[conditionTypeID].petcondition.good_min;
        float poorMin = (float)DogConditionValuesData[conditionTypeID].petcondition.poor_min;

        if (value >= goodMin / goodMax)
        {
            colorDecimal = "#5DBC82";
        }
        else if (value >= poorMin / goodMax)
        {
            colorDecimal = "#FFC800";
        }
        else
        {
            colorDecimal = "#FF3B30";
        }
        if (ColorUtility.TryParseHtmlString(colorDecimal, out Newcolor))
        {
            img.color = Newcolor;
        }
    }

    /// <summary>
    /// 在这里获取狗关于进度条的数据
    /// </summary>
    public void SetRefreshDogMaxValuesFun()
    {
        DogConditionValuesData.Clear();
        if (petModelRecData != null)
        {
            foreach (var item in petModelRecData.pet_condition)
            {
                if (item != null)
                {
                    petcondition DogCondition = ManageMentClass.DataManagerClass.GetPetConditionTable(petModelRecData.pet_type, item.condition_type);
                    PetSliderValueData petSliderValueData = new PetSliderValueData();
                    petSliderValueData.cur_val = item.cur_val;
                    petSliderValueData.petcondition = DogCondition;
                    if (!DogConditionValuesData.ContainsKey(item.condition_type))
                    {
                        DogConditionValuesData.Add(item.condition_type, petSliderValueData);
                    }
                    else
                    {
                        DogConditionValuesData[item.condition_type] = petSliderValueData;
                    }
                }
            }
        }

    }

    public pet_keeping GetConsumeDataFun(int consumeTypeID)
    {
        Debug.Log("这里的内容的值： petModelRecData.pet_type： " + petModelRecData.pet_type + "  consumeTypeID:  " + consumeTypeID);
        pet_keeping petKeepingData = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(petModelRecData.pet_type, consumeTypeID);
        if (petKeepingData != null)
        {
            return petKeepingData;
        }
        return null;
    }

    /// <summary>
    /// 计时器
    /// </summary>
    /// <returns></returns>
    public IEnumerator CountDownTimerFun()
    {
        while (SecondTimeValue > 0)
        {
            if (petModelRecData == null)
            {
                StopAllCoroutines();
            }
            if (petModelRecData.train_info == null)
            {
                StopAllCoroutines();
                //StopCoroutine(CountDownTimerFun());
            }
            else
            {
                SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
            }
            yield return new WaitForSeconds(1);
            PetTrainTime_Text.text = CalcTools.ChangeSecondFun(SecondTimeValue);
            if (SecondTimeValue <= 0)
            {
                //训练结束
                TimeEndFun();
            }
        }

    }

    public void TimeEndFun()
    {
        StopCoroutine(CountDownTimerFun());
        MessageManager.GetInstance().RequestPetList(() =>
        {
            Debug.Log("petModelRecData.id: " + petModelRecData.id);
            petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id);
            if (petModelRecData.train_result != null)
            {
                //有结果了
                //有训练结果
                OpenUIForm(FormConst.PETTRAINRESULTTIPPANEL);
                SendMessage("OpemPetTrainResultTipPanel", "Success", petModelRecData);
                Train_Panel.SetActive(true);
                Training_Panel.SetActive(false);
            }
            else
            {
                // 无训练结果
                SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                if (SecondTimeValue > 0)
                {
                    StartCoroutine(CountDownTimerFun());
                }
            }
        });
    }

    public PetModelRecData GetPetData()
    {
        return petModelRecData;
    }
}
