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


    //��ʾ�ָ����飬��࣬���� ���

    public GameObject OtherSetDataPanel;
    public Text Title_Text;
    public Text OtherPanelGasValue_Text;
    public Text OtherPanelTips_Text;

    //��ʾ�ָ����� ���
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

    //ѵ�����

    //�ɳ�ֵ
    public Slider GrowthValue_Slider;
    public Text GrowthValue_Text;
    //���ֵ
    public Text BodyData_Text;
    //�ǻ�
    public Text WisdomData_Text;
    //����
    public Text LuckyData_Text;
    //��ѵ������/����
    public Text TrainingCount_Text;

    //ѵ�����
    public GameObject Train_Panel;
    //����ѵ�����
    public GameObject Training_Panel;

    //ѵ����ʾ����
    public Text PetTrainTypeTip_Text;
    //ѵ������ʱ
    public Text PetTrainTime_Text;

    public Button Train_Btn;


    object[] arrAniMessageObj = new object[2];

    // ����

    //��������
    PetModelRecData petModelRecData;
    //ι���۸�����
    pet_keeping petKeepingData;
    //���͵�����
    DogPetFeedData petAdoptData;


    //ι������
    int FeedCount = 1;
    //����ι���Ĵ���
    int CanFeedCount = 0;
    //����ι����Ҫ���ӵĽ���
    float FeedAddSliderValue = 0f;
    //��������ʼ����
    float FeedSliderValue = 0f;
    //��������ι���ĵ���
    int CanAddFeedValue = 0;

    int ConditionTypeId = 0;


    private int SecondTimeValue;

    // ���� condition_type ID , ��Ӧ�Ľ������ֵ 
    Dictionary<int, PetSliderValueData> DogConditionValuesData = new Dictionary<int, PetSliderValueData>();

    Color Newcolor;

    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //��������
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency; //��͸�������ܴ�͸
        OtherSetDataPanel.SetActive(false);
        FeedPetDataPanel.SetActive(false);
        //�����¼�����
        ReceiveMessageFun();
    }
    private void Start()
    {
        RigisterButtonObjectEvent("Hunger_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("��������ѵ��"), 5f);
                return;
            }

            if (DogConditionValuesData[0].petcondition.good_max > DogConditionValuesData[0].cur_val)
            {
                //ιʳ
                SetTipPanelData(0);
            }
            else
            {
                ToastManager.Instance.ShowNewToast("����������������ιʳ", 8f);
            }
        });
        RigisterButtonObjectEvent("Mood_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("��������ѵ��"), 5f);
                return;
            }
            if (DogConditionValuesData[2].petcondition.good_max > DogConditionValuesData[2].cur_val)
            {
                //���
                SetTipPanelData(2);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("������������������", 8f);
            }

        });
        RigisterButtonObjectEvent("Clean_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("��������ѵ��"), 5f);
                return;
            }
            if (DogConditionValuesData[1].petcondition.good_max > DogConditionValuesData[1].cur_val)
            {
                //���
                SetTipPanelData(1);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("�����������������", 8f);
            }

        });
        RigisterButtonObjectEvent("Health_Button", p =>
        {
            if (petModelRecData.train_info != null)
            {
                ToastManager.Instance.ShowPetToast(string.Format("��������ѵ��"), 5f);
                return;
            }
            if (DogConditionValuesData[3].petcondition.good_max > DogConditionValuesData[3].cur_val)
            {
                //����
                SetTipPanelData(3);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("��������������������", 8f);
            }

        });
        // otherPanel ���İ�ť
        RigisterButtonObjectEvent("OtherSetConfirmPaymentButton", p =>
        {
            Debug.Log("���һ������ֵ��ֵ���� " + petKeepingData.price);
            if (petKeepingData.price <= ManageMentClass.DataManagerClass.gas_Amount)
            {
                Debug.Log("��������");
                SendServerFun();
            }
            else
            {
                Debug.Log("��ʾ����");
                //gas����
                OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
            }

        });
        RigisterButtonObjectEvent("OtherSetPanelCloseBtn", p =>
        {
            //�ر����
            if (OtherSetDataPanel.activeSelf)
            {
                OtherSetDataPanel.SetActive(false);
            }
        });
        //FeedPetpanel���İ�ť
        RigisterButtonObjectEvent("FeedPetArrifmButton", p =>
        {
            Debug.Log("���һ������ֵ��ֵ���� " + (petKeepingData.price * FeedCount) + "   FeedCount: " + FeedCount);
            //֧�� 
            if (petKeepingData.price * FeedCount <= ManageMentClass.DataManagerClass.gas_Amount)
            {
                Debug.Log("����ֵ�����⣺����С����gas ");
                SendServerFun();
            }
            else
            {
                Debug.Log("����ֵ�����⣺���Ҵ�����gas ");
                //gas����
                OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
            }
        });


        RigisterButtonObjectEvent("FeedPetCloseBtn", p =>
        {
            //�ر� 
            if (FeedPetDataPanel.activeSelf)
            {
                FeedPetDataPanel.SetActive(false);
            }
        });
        //ѵ����ť
        RigisterButtonObjectEvent("Training_Button", p =>
        {
            JudgeTrainPetFun();
        });
        //����ѵ����ť
        RigisterButtonObjectEvent("TrainingFinish_Button", p =>
        {
            //����ѵ��
            OpenUIForm(FormConst.PETTRAINFINISHPANEL);
            SendMessage("PetTrainFinishPanelTime", "", petModelRecData);
        });

        //�鿴���ܰ�ť
        RigisterButtonObjectEvent("PetTrainCheckTips_Btn", p =>
        {
            //����ѵ��
            OpenUIForm(FormConst.PETTRAINtIPSUIFORM);
        });

        RigisterButtonObjectEvent("ExchangeBtn", p =>
        {
            if (!bCanExchange())
            {
                ToastManager.Instance.ShowPetToast("����ĸ���״̬Ҫ�ﵽ60���ϣ��ſɶһ�ʵ��Ȯ����ƾ֤Ŷ~", 3f);
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
    /// �����¼����ݹ���
    /// </summary>
    public void ReceiveMessageFun()
    {
        //�����ʱ���չ�������( ���������� ����ID);
        ReceiveMessage("RefreshDogPanel",
                  p =>
                  {
                      if (!gameObject.activeSelf)
                      {
                          return;
                      }

                      int dogID = (int)p.Values;
                      Debug.Log("���һ��DOgID��ֵ�� " + dogID);
                      SetDogPanelData(dogID);
                      SetDogTrainShowStateFun();

                      if (petModelRecData.train_info != null)
                      {
                          SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                          Debug.Log("����Э��");
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

                      //ˢ�¸������ֵ
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

                  //�ر����
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
        //��ʼѵ��  ��Ϣ����
        ReceiveMessage("BeginPetTrain",
            p =>
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }
                petModelRecData = p.Values as PetModelRecData;
                //��ʼѵ������ʼ����ʱ
                SetDogTrainShowStateFun();
                PetSpanManager.Instance().SetCameraTrainFun();
                if (petModelRecData.train_info != null)
                {
                    Debug.Log("����Э��2");

                    SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
                    StartCoroutine(CountDownTimerFun());
                }
            }
        );

        //ѵ��ʱ���鲻�ѵ�ȷ�ϰ�ť
        ReceiveMessage("PetTrainMoodDownTipPanelAffirmBtn",
            p =>
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }
                //��ѵ�����
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
                //����ѵ����
                Train_Panel.SetActive(true);
                Training_Panel.SetActive(false);
                StopCoroutine(CountDownTimerFun());
                Debug.Log("�ر�Э����");
            }
        );
        ReceiveMessage("RefreshPetTrainDataUIMessage",
           p =>
           {

               if (!gameObject.activeSelf)
               {
                   return;
               }
               //����ѵ����
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
               Debug.Log("ˢ������������");
           }
       );





    }
    //��ʾ
    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
    }
    //����
    public override void Hiding()
    {
        base.Hiding();
        PetSpanManager.Instance().CancelLookAtPet();
        InterfaceHelper.SetJoyStickState(true);
        StopCoroutine(CountDownTimerFun());
    }
    /// <summary>
    /// ��������Ƿ���ѵ��״̬
    /// </summary>
    public void SetDogTrainShowStateFun()
    {
        if (petModelRecData.train_info != null)
        {
            //����ѵ��
            Train_Panel.SetActive(false);
            Training_Panel.SetActive(true);
            SecondTimeValue = CalcTools.TimeStampChangeSecondFun(petModelRecData.train_info.train_end_time);
            PetTrainTime_Text.text = CalcTools.ChangeSecondFun(SecondTimeValue);
            PetTrainTypeTip_Text.text = ManageMentClass.DataManagerClass.GetTrainTableFun(petModelRecData.train_info.train_id).name + "��...";



        }
        else
        {
            //����ѵ��
            Train_Panel.SetActive(true);
            Training_Panel.SetActive(false);
        }
    }

    /// <summary>
    /// �ж��Ƿ���Կ�ʼѵ��
    /// </summary>
    public void JudgeTrainPetFun()
    {
        if (Train_Btn.enabled == false)
        {
            return;
        }
        if (petModelRecData.pet_remain_train_num > 0)
        {
            //��ѵ����������0

            for (int i = 0; i < 4; i++)
            {
                Debug.Log("DogConditionValuesData" + DogConditionValuesData[i].cur_val + "  i  : " + i);
            }

            if (DogConditionValuesData[0].cur_val > 60 && DogConditionValuesData[1].cur_val > 60 && DogConditionValuesData[3].cur_val > 60)
            {
                if (DogConditionValuesData[2].cur_val < 20)
                {
                    //�����������ȷ�����
                    OpenUIForm(FormConst.PETTRAINMOODDOWNTIPPANEL);
                }
                else
                {
                    //��ѵ�����
                    //��ѵ�����
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
                ToastManager.Instance.ShowPetToast("�豥ʳ�ȡ����ȡ������Ⱦ��ﵽ60���ϲſɽ���ѵ��", 8f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("��ĳ���ǳ����ˣ�������ѵ����", 8f);
        }
    }

    /// <summary>
    /// �ж��Ƿ���Զһ�
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
            petAdoptData = new DogPetFeedData(petModelRecData.id, ConditionTypeId, FeedCount);//����ID��ι������
        }
        else
        {
            petAdoptData = new DogPetFeedData(petModelRecData.id, ConditionTypeId, 1);//����ID��ι������
        }
        string Strdata = JsonConvert.SerializeObject(petAdoptData);
        Debug.Log("strData: " + Strdata);
        MessageManager.GetInstance().RequestFeed(Strdata, (JObject jo) =>
        {
            if (jo == null)
            {
                Debug.Log("joΪ��û�д�����");
            }
            else
            {
                Debug.Log("jo������");
            }
            // PetSpanManager.Instance().PlayAddLoveCoinAni(imgClean, p.remain_lovecoin);
            MessageManager.GetInstance().RequestGasValue();
            //ˢ������
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
    /// ι���ɹ��� ������ˢ��
    /// </summary>
    public void FeedsucceedAllDataFun(JObject jo)
    {
        Debug.Log("���һ�� ho��ֵ ��" + jo);
        //ι���ɹ�
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
            //ʧ��
        }
    }
    public void ColseThisPanelFun()
    {
        CloseUIForm();
        Debug.Log("�ر��������");
    }
    //���������������
    void SetOtherDataPanelFun(int typeID)
    {
        if (!OtherSetDataPanel.activeSelf)
        {
            OtherSetDataPanel.SetActive(true);
        }
        if (petKeepingData != null)
        {
            Debug.Log("petKeepingData��������ݲ�Ϊ��");
        }


        Debug.Log(" petKeepingData.price�� " + petKeepingData.price);
        switch (typeID)
        {
            case 1:
                Title_Text.text = "���";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "�ָ��������ֵ";
                break;
            case 2:
                Title_Text.text = "���";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "�ָ���������ֵ";
                break;
            case 3:
                Title_Text.text = "����";
                OtherPanelGasValue_Text.text = petKeepingData.price + "";
                OtherPanelTips_Text.text = "�ָ����н���ֵ";
                break;
            default:
                break;
        }
    }
    //����ι������ֵ���������
    void SetFeedPetDataPanelFun()
    {
        if (!FeedPetDataPanel.activeSelf)
        {
            FeedPetDataPanel.SetActive(true);
        }
        //��ʼ������
        FeedCount = 1;
        FeedAddSliderValue = 0f;
        CanFeedCount = 0;
        //��ʼ�����
        m_CostText.text = string.Format("ÿ����{0}GAS����10�㱥ʳ��", petKeepingData.price);
        //FeedCount_Text.text = FeedCount + "";
        m_InputFieldNum.text = string.Format("{0}", FeedCount);
        //  SelectCount_Slider.value = 0;
        FeedGasValue_Text.text = petKeepingData.price + "";
        //AdjustGasUI();
        CanAddFeedValue = DogConditionValuesData[0].petcondition.good_max - DogConditionValuesData[0].cur_val;


        if (CanAddFeedValue > 0)
        {
            CanFeedCount = Mathf.CeilToInt((float)CanAddFeedValue / 10);
            //���������ι������
            FeedAddSliderValue = 1 / (float)(CanFeedCount - 1);
            ExceedPanel.SetActive(false);
            ExceedValue_Text.text = "0";
        }
    }
    /// <summary>
    /// ѡ���������ֵ
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
        Debug.Log("���һ������value��ֵ�� " + value);
    }
    //������
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
    //�Ӵ���
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
    ///������
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
            //���״̬ 
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
    /// ��������ֵ
    /// </summary>
    void SetDogPanelData(int dogID)
    {
        Debug.Log("����ID�� " + dogID);
        petModelRecData = PetSpanManager.Instance().GetPetModelData(dogID);
        if (petModelRecData != null)
        {
            //ˢ�²���������ֵ
            SetRefreshDogMaxValuesFun();
            DogName_Text.text = petModelRecData.pet_type == 2 || petModelRecData.pet_type == 4 ? string.Format("<color=#4DABCF>{0}</color>", TextTools.setCutAddString(petModelRecData.pet_name, 7, "...")) : string.Format("<color=#ED716B>{0}</color>", TextTools.setCutAddString(petModelRecData.pet_name, 7, "..."));
            SerialNumber_Text.text = "��ţ�" + petModelRecData.pet_number;
            if (petModelRecData.pet_type == 2 || petModelRecData.pet_type == 4)
            {
                //��
                DogGenderRoot.transform.GetChild(0).gameObject.SetActive(true);
                DogGenderRoot.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                //ĸ
                DogGenderRoot.transform.GetChild(0).gameObject.SetActive(false);
                DogGenderRoot.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (petModelRecData.pet_type == 2 || petModelRecData.pet_type == 3)
            {
                DogState_Text.text = "��Ȯ";
            }
            else
            {
                DogState_Text.text = "��Ȯ";
            }
            //ˢ�¸������ֵ
            RefreshDogDataFun();
        }
    }
    /// <summary>
    /// ˢ��dog�ĸ������
    /// </summary>
    void RefreshDogDataFun()
    {
        FeedCount = 1;
        petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id);
        SetRefreshDogMaxValuesFun();
        //ˢ�½�����
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
        //ѵ�����
        if (petModelRecData.lo_exp >= petModelRecData.exp)
        {
            //�Ѵﵽ���ʵ������
            GrowthValue_Slider.value = 1;
            GrowthValue_Text.text = "MAX";
            //��ť�û�
            /*Train_Btn.enabled = false;
            TrainTip_Text.text = "����ѵ��";
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
                    //���
                    BodyData_Text.text = "���:" + petModelRecData.pet_attribute[i].cur_val;
                    break;
                case 2:
                    //�ǻ�
                    WisdomData_Text.text = "�ǻ�:" + petModelRecData.pet_attribute[i].cur_val; ;
                    break;
                case 3:
                    //����
                    LuckyData_Text.text = "����:" + petModelRecData.pet_attribute[i].cur_val; ;
                    break;
            }
        }
        //�򿪽�����
        if (petModelRecData.train_result != null)
        {
            //��ѵ�����
            OpenUIForm(FormConst.PETTRAINRESULTTIPPANEL);
            SendMessage("OpemPetTrainResultTipPanel", "Success", petModelRecData);
        }
    }


    /// <summary>
    /// ���ü���ֵ����
    /// </summary>
    void SetHungerSliderFun(int conditionTypeID)
    {
        Hunger_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        HungerSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(HungerSlider_Img, Hunger_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// ��������ֵ����
    /// </summary>
    void SetMoodSliderFun(int conditionTypeID)
    {
        Mood_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        MoodSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(MoodSlider_Img, Mood_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// �������ֵ����
    /// </summary>
    void SetCleanSliderFun(int conditionTypeID)
    {
        Clean_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        CleanSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(CleanSlider_Img, Clean_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// ���ý���ֵ����
    /// </summary>
    void SetHealthSliderFun(int conditionTypeID)
    {
        Health_Slider.value = (float)DogConditionValuesData[conditionTypeID].cur_val / (float)DogConditionValuesData[conditionTypeID].petcondition.good_max;
        HealthSliderValue_Text.text = string.Format("{0}/{1}", DogConditionValuesData[conditionTypeID].cur_val, DogConditionValuesData[conditionTypeID].petcondition.good_max);
        SetImageColorFun(HealthSlider_Img, Health_Slider.value, conditionTypeID);
    }
    /// <summary>
    /// ����ͼƬ����ɫ
    /// </summary>
    /// <param name="img">ͼƬ</param>
    /// <param name="value">�ٷֱ�ֵ</param>
    public void SetImageColorFun(Image img, float value, int conditionTypeID)
    {
        //#FF3B30 ��ɫ
        //#FFC800 ��ɫ+
        //#5DBC82 ��ɫ

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
    /// �������ȡ�����ڽ�����������
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
        Debug.Log("��������ݵ�ֵ�� petModelRecData.pet_type�� " + petModelRecData.pet_type + "  consumeTypeID:  " + consumeTypeID);
        pet_keeping petKeepingData = ManageMentClass.DataManagerClass.GetPetKeepingTableFun(petModelRecData.pet_type, consumeTypeID);
        if (petKeepingData != null)
        {
            return petKeepingData;
        }
        return null;
    }

    /// <summary>
    /// ��ʱ��
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
                //ѵ������
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
                //�н����
                //��ѵ�����
                OpenUIForm(FormConst.PETTRAINRESULTTIPPANEL);
                SendMessage("OpemPetTrainResultTipPanel", "Success", petModelRecData);
                Train_Panel.SetActive(true);
                Training_Panel.SetActive(false);
            }
            else
            {
                // ��ѵ�����
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
