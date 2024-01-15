using UIFW;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetdensLoading : BaseUIForm
{
    // 普通样式
    public GameObject TypeA_Panel;
    // 寻宝样式
    public GameObject TypeB_Panel;




    //普通样式的组件
    public Slider m_Slider;
    public Text m_ProgressText;
    public Text m_TipsText;
    public Text m_SubTipsText;
    public RawImage m_ImgBg;


    public RawImage sliderFill_Img;
    public RawImage sliderBackground_Img;


    //寻宝样式的组件
    public Slider m_SliderB;
    public Text m_TipsTextB;
    public Text m_SubTipsTextB;
    public RawImage m_ImgBgB;
    public Text sub_bottm_text;

    private int levelID;
    public void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Top;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable; //半透明，不能穿透
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.HideOther;
        ReceiveMessage("TransferLoading", p =>
         {
             levelID = (int)p.Values;
             SetTips(levelID);
             SetTexture(levelID);
             SceneLoadManager.Instance().Load(levelID);
         });
    }
    public override void Display()
    {
        base.Display();

        if (levelID == (int)LoadSceneType.TreasureDigging)
        {
            m_SliderB.value = 0f;
        }
        else
        {
            m_Slider.value = 0f;
            m_ProgressText.text = string.Format("{0}%", 0);
        }

    }

    public void SetProgress(int value)
    {
        if (levelID == (int)LoadSceneType.TreasureDigging)
        {
            m_SliderB.value = value;
        }
        else
        {
            m_Slider.value = value;
            m_ProgressText.text = string.Format("{0}%", value);
        }
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public void SetTips(int level)
    {
        if (level != (int)LoadSceneType.TreasureDigging)
        {
            if (m_TipsText == null)
                return;
        }
        else
        {
            if (m_TipsTextB == null)
                return;
        }

        if (level == (int)LoadSceneType.parlorScene)
        {
            m_TipsText.text = "正在前往个人空间...";
        }
        else if (level == (int)LoadSceneType.dogScene)
        {
            m_TipsText.text = "正在前往宠物小窝...";
        }
        else if (level == (int)LoadSceneType.ShelterScene)
        {
            m_TipsText.text = "正在前往宠物救助站...";
        }
        else if (level == (int)LoadSceneType.TreasureDigging)
        {

            m_TipsTextB.text = "正在前往寻宝乐园...";
        }
        else if (level == (int)LoadSceneType.BedRoom)
        {
            m_TipsText.text = "正在前往卧室...";
        }
        else if (level == (int)LoadSceneType.ModuleTest1)
        {
            m_TipsText.text = "正在前往测试模块1主场景...";
        }
        else if (level == (int)LoadSceneType.ModuleTest2)
        {
            m_TipsText.text = "正在前往测试模块2主场景...";
        }
        else if (level == (int)LoadSceneType.RainbowBeach)
        {
            m_TipsText.text = "";
        }
        else if (level == (int)LoadSceneType.ShenMiHaiWan)
        {
            m_TipsText.text = "";
        }
        else if (level == (int)LoadSceneType.ShenMiHaiWan)
        {
            m_TipsText.text = "";
        }
    }

    public void SetTips(LoadType loadType)
    {
        if (m_TipsTextB == null)
            return;
        switch (loadType)
        {
            case LoadType.Create:
                m_TipsTextB.text = "正在创建寻宝空间...";
                break;
            case LoadType.Join:
                m_TipsTextB.text = "正在加入寻宝空间...";
                break;
            case LoadType.TreasureEnd:
                m_TipsTextB.text = "本次挖宝活动已结束，正在返回等待区...";
                break;
            case LoadType.RainbowCreate:
            case LoadType.RainbowJoin:
                m_TipsTextB.text = "";
                break;
            default:
                m_TipsTextB.text = "";
                break;
        }
    }

    public void SetTexture(int level)
    {
        string path = "";
        string SliderFillPath = "";
        string SliderBackGround = "";
        sub_bottm_text.gameObject.SetActive(true);
        m_SubTipsText.gameObject.SetActive(false);
        switch (level)
        {
            case (int)LoadSceneType.parlorScene:

                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/Loading/back ground");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/img_v2_78f631da-ccf8-4315-b089-aef84c3a8c9g");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 31");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
            case (int)LoadSceneType.dogScene:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/Petdens/back ground");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 32");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 33");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
            case (int)LoadSceneType.ShelterScene:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/AidStations/back ground");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 32");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 33");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
            case (int)LoadSceneType.TreasureDigging:
                TypeA_Panel.gameObject.SetActive(false);
                TypeB_Panel.gameObject.SetActive(true);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsTextB, null);

                // path = string.Format("{0}", "UIRes/Texture/Loading/TreasureDigginBG");
                break;
            case (int)LoadSceneType.ModuleTest1:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                break;
            case (int)LoadSceneType.ModuleTest2:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                break;
            case (int)LoadSceneType.RainbowBeach:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                sub_bottm_text.gameObject.SetActive(true);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/AidStations/loading");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 32");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 33");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
            case (int)LoadSceneType.ShenMiHaiWan:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                sub_bottm_text.gameObject.SetActive(true);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/AidStations/loading");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 32");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 33");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
            case (int)LoadSceneType.HaiDiXingKong:
                TypeA_Panel.gameObject.SetActive(true);
                TypeB_Panel.gameObject.SetActive(false);
                sub_bottm_text.gameObject.SetActive(true);
                ModuleMgr.GetInstance().RegisterLoadUI(m_SubTipsText, null);
                m_SubTipsText.gameObject.SetActive(false);
                return;
                path = string.Format("{0}", "UIRes/Texture/AidStations/loading");
                SliderFillPath = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 32");
                SliderBackGround = string.Format("{0}", "UIRes/Texture/Loading/Rectangle 33");
                if (sliderFill_Img != null)
                    sliderFill_Img.texture = Resources.Load<Texture2D>(SliderFillPath);
                if (sliderBackground_Img != null)
                    sliderBackground_Img.texture = Resources.Load<Texture2D>(SliderBackGround);
                if (m_ImgBg != null)
                    m_ImgBg.texture = Resources.Load<Texture2D>(path);
                break;
        }

    }

    public void SetOtherTipsFun(string strTip)
    {
        if (m_TipsText == null)
            return;
        m_TipsText.text = strTip;
    }
    IEnumerator CheckloadNextScenes(Action callBack)
    {
        bool isLoadNext = true;
        while (isLoadNext)
        {
            yield return new WaitForSeconds(0.1f);
            if (!ManageMentClass.DataManagerClass.playerIsHavePicture)
            {
                isLoadNext = false;
                callBack?.Invoke();
                StopCoroutine(CheckloadNextScenes(callBack));
            }
            if (FrameHttp.Instance().allDownCount != 0 && FrameHttp.Instance().nowDownCount == FrameHttp.Instance().allDownCount)
            {
                isLoadNext = false;
                //跳转场景
                callBack?.Invoke();
                StopCoroutine(CheckloadNextScenes(callBack));
            }
            else if (FrameHttp.Instance().allDownCount != 0)
            {

                if (FrameHttp.Instance().nowDownCount != 0)
                {
                    int progress = (int)Mathf.Round(((float)FrameHttp.Instance().nowDownCount / (float)FrameHttp.Instance().allDownCount) * 100);
                    SetProgress(progress);
                }
                else
                {
                    SetProgress(0);
                }
            }
        }
    }
}
