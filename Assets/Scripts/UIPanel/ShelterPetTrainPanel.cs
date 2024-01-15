using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public class ShelterPetTrainPanel : BaseUIForm
{
    public Text Time_Text;
    public Transform AllTrainBtnRoot;

    PetModelRecData petModelRecData;
    private List<PetTrainProItem> arrPetTrainProItemList = new List<PetTrainProItem>();
    PetTrainProItem trainItem;
    //UI�����ڵ�
    public Image ImgTitleBg;
    public Image ImgArrow;
    public Image ImgTitle;
    public Image ImgFlag;

    public bool isClickBtn = false;

    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        //�ر����
        RigisterButtonObjectEvent("TrainPetClose_Btn", p =>
        {
            CloseUIForm();
        });
        //��ʼѵ��
        RigisterButtonObjectEvent("BeginPetTrain_Btn", p =>
        {

            if (!isClickBtn)
            {
                return;
            }
            isClickBtn = false;
            if (ManageMentClass.DataManagerClass.gas_Amount < trainItem.Gas_Value)
            {
                OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
            }
            else
            {
                MessageManager.GetInstance().RequestPetTrain(petModelRecData.id, trainItem.ID, (JObject jo) =>
                {
                    ManageMentClass.DataManagerClass.gas_Amount = (int)jo["data"]["remain_gas"];
                    SendMessage("UpdataGasValue", "data", null);
                    string jsonData = jo["data"].ToString();
                    TrainInfo trainInfo = JsonUntity.FromJSON<TrainInfo>(jsonData);
                    MessageManager.GetInstance().RequestPetList(() =>
                    {
                        Debug.Log("petModelRecData.id: " + petModelRecData.id);
                        petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id);
                        SendMessage("BeginPetTrain", "Success", petModelRecData);
                        //���Ͷ����¼�
                        SendMessage("BeginPetTrainAnimation", "success", petModelRecData.id);
                        CloseUIForm();
                    });

                }, () =>
                {
                    isClickBtn = true;
                });



            }

        });
        //���ѵ����
        ReceiveMessage("ClickTrain", p =>
        {
            trainItem = p.Values as PetTrainProItem;

            CloseTrainItemFun();
            trainItem.SetOpenSelectStateFun();
            SetBeginBtnTimeTextFun();
            Debug.Log("���������ѡ����");
        });


        ReceiveMessage("OpenTrainPanel", p =>
        {
            petModelRecData = p.Values as PetModelRecData;
        });
        //����UI
        FindUIFun();
    }
    /// <summary>
    ///  
    /// </summary>
    public override void Display()
    {
        base.Display();
        isClickBtn = true;
        PlayUIAni();
        Time_Text.gameObject.SetActive(false);
        CloseTrainItemFun();
        SetAllTrainItemFun();

        BeginSetTrainProjectFun();
    }
    private void FindUIFun()
    {
        for (int i = 0; i < AllTrainBtnRoot.childCount; i++)
        {
            GameObject childObj = AllTrainBtnRoot.GetChild(i).gameObject;
            if (childObj != null)
            {
                PetTrainProItem trainItem = childObj.GetComponent<PetTrainProItem>();
                if (trainItem != null)
                {
                    arrPetTrainProItemList.Add(trainItem);
                }
            }
        }
    }
    //�ر�����ѡ��״̬
    private void CloseTrainItemFun()
    {
        for (int i = 0; i < arrPetTrainProItemList.Count; i++)
        {
            arrPetTrainProItemList[i].SetCloseSelectStateFun();
        }
    }
    //���������������
    private void SetAllTrainItemFun()
    {
        for (int i = 0; i < arrPetTrainProItemList.Count; i++)
        {
            train trainData = ManageMentClass.DataManagerClass.GetTrainTableFun(arrPetTrainProItemList[i].ID);
            arrPetTrainProItemList[i].SetDataFun(trainData);
        }
    }
    /// <summary>
    /// ���ÿ�ʼ��ť����ʾ��ʱ����ʾ
    /// </summary>
    private void SetBeginBtnTimeTextFun()
    {
        if (!Time_Text.gameObject.activeSelf)
        {
            Time_Text.gameObject.SetActive(true);
        }
        Time_Text.text = CalcTools.ChangeMinuteFun(trainItem.trainData.cost_time);
    }
    /// <summary>
    /// ���ó�ʼѡ��
    /// </summary>
    private void BeginSetTrainProjectFun()
    {
        trainItem = arrPetTrainProItemList[0];
        trainItem.SetOpenSelectStateFun();
        SetBeginBtnTimeTextFun();
    }

    /// <summary>
    /// UI����
    /// </summary>
    public void PlayUIAni()
    {
        if (ImgTitleBg != null)
            ImgTitleBg.fillAmount = 0f;

        if (ImgArrow != null)
            ImgArrow.fillAmount = 0f;

        if (ImgFlag != null)
            ImgFlag.fillAmount = 0f;

        if (ImgTitle != null)
        {
            ImgTitle.gameObject.SetActive(false);
            ImgTitle.transform.localScale = new Vector3(2f, 2f, 2f);
        }

        DOTween.To(delegate (float value)
        {
            if (ImgTitleBg != null)
                ImgTitleBg.fillAmount = value;

            if (ImgArrow != null)
                ImgArrow.fillAmount = value;

        }, 0, 1, 1f).OnComplete(() =>
        {
            if (ImgTitleBg != null)
                ImgTitleBg.fillAmount = 1f;

            if (ImgArrow != null)
                ImgArrow.fillAmount = 1f;

            if (ImgTitle != null)
            {
                ImgTitle.gameObject.SetActive(true);
                ImgTitle.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).OnComplete(() =>
                {
                    if (gameObject.activeSelf)
                    {
                        StartCoroutine(SetFlash());
                    }
                });
            }

            DOTween.To(delegate (float value)
            {
                if (ImgFlag != null)
                    ImgFlag.fillAmount = value;
            }, 0, 1, 1f);
        });
    }

    public IEnumerator SetFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            ImgArrow.DOFade(0f, 1.5f).OnComplete(() =>
            {
                ImgArrow.color = new Color(1f, 1f, 1f, 1f);
            });
            yield return new WaitForSeconds(1.5f);
        }
    }
}
