using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using DG.Tweening;

public class PetTrainResultTipPanel : BaseUIForm
{

    public Text PetTrainResultTip_Text;

    public Text GrowthValueName_Text;
    public Text GrowthValue_Text;

    public Text PhysiquevalueName_Text;
    public Text Physiquevalve_Text;

    //UI动画节点
    public Image ImgTitleBg;
    public Image ImgArrow;
    public Image ImgTitle;
    public Image ImgFlag;

    //宠物数据
    PetModelRecData petModelRecData;

    private bool isCanCilckBtn = false;

    private void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        ReceiveMessage("OpemPetTrainResultTipPanel", p =>
        {
            petModelRecData = p.Values as PetModelRecData;
            if (petModelRecData != null)
            {
                isCanCilckBtn = true;
                SetPetResultPanelFun();
            }
            else
            {
                isCanCilckBtn = false;
                ToastManager.Instance.ShowPetToast("数据错误，请重试!", 5f);
            }
        });
        RigisterButtonObjectEvent("PetTrainResultAffirm_Btn", p =>
        {
            if (isCanCilckBtn)
            {
                isCanCilckBtn = false;
                //关闭面板，且向服务发送数据
                MessageManager.GetInstance().RequestPetClearTrainData(petModelRecData.train_result.train_record_id, () =>
                {
                    MessageManager.GetInstance().RequestPetList(() =>
                    {
                        petModelRecData = PetSpanManager.Instance().GetPetModelData(petModelRecData.id, petModelRecData.pet_box_id);

                        if (petModelRecData.lo_exp >= petModelRecData.exp)
                        {
                            PetSpanManager.Instance().bBigPet = true;
                            petModelRecData.lo_exp = petModelRecData.exp;
                            PetSpanManager.Instance().UpdatePet();
                            PetSpanManager.Instance().bBigPet = false;
                            //SendMessage("GrowUpAdultDog", "Success", petModelRecData.id);
                        }
                        PetSpanManager.Instance().SetFinishTrainPetRigidbody(petModelRecData.id);
                        PetSpanManager.Instance().SetTrainData(petModelRecData.id, false);
                        PetSpanManager.Instance().PlayPetAni(petModelRecData.id, (int)PetStateAnimationType.Idle);
                        PetSpanManager.Instance().LookAtPet(petModelRecData.id);
                        //SendMessage("FinishPetTrainAnimation", "Success", petModelRecData.id);

                        SendMessage("RefreshPetTrainDogData", "Success", null);
                        CloseUIForm();
                    });
                });
            }
            else
            {
                CloseUIForm();
            }
        });
    }
    /// <summary>
    /// 设置宠物结果面板
    /// </summary>
    void SetPetResultPanelFun()
    {
        PetTrainResultTip_Text.text = petModelRecData.train_result.title;
        GrowthValueName_Text.text = petModelRecData.train_result.reward[0].name;
        GrowthValue_Text.text = "+" + petModelRecData.train_result.reward[0].val;

        PhysiquevalueName_Text.text = petModelRecData.train_result.reward[1].name;
        Physiquevalve_Text.text = "+" + petModelRecData.train_result.reward[1].val;

    }
    public override void Display()
    {
        base.Display();
        PlayUIAni();
    }

    public override void Hiding()
    {
        base.Hiding();
        StopCoroutine(SetFlash());
    }
    /// <summary>
    /// UI动画
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
    IEnumerator delaySenMessageFun()
    {
        yield return new WaitForSeconds(0.5f); ;
        SendMessage("FinishPetTrainAnimation", "Success", petModelRecData.id);
        StopCoroutine(delaySenMessageFun());
    }

}
