using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using System;
using UnityEngine.U2D;

public class ShopClothItem : BaseUIForm
{
    public enum QualityEnum
    {
        N = 1,
        R = 2,
        SR = 3,
        SSR = 4,
    }
    public Button m_BtnIcon;
    public Button m_BtnBuy;
    public Image m_ImgBg;
    public Image m_IconImg;
    public Text m_HaveNumText;
    public Text m_NameText;
    public Text m_NumText;
    public Text m_PriceText;
    public Text m_TimeText;
    public Image m_SelectImg;
    public Image m_QualityImg;

    public Transform m_NewTrans;
    public Transform m_TimeTrans;
    public Transform m_NoneTrans;
    public Transform m_PriceTrans;

    public RectTransform m_HorizontalLayoutGroup;
    public RectTransform m_VerticalLayoutGroup;
    public RectTransform m_TimeVerticalLayoutGroup;
    public RectTransform m_PriceVerticalLayoutGroup;

    public ShopOutFitItem avatarData;

    private int seconds;
    private float timer;

    private float clickTimer = 0f;
    private float clickInterval = 1f;
    private bool bClicked = false;
    private bool bCanClick = true;

    // Start is called before the first frame update
    void Awake()
    {
        SetItemSelectState(false);
        m_NoneTrans.gameObject.SetActive(false);
        m_PriceTrans.gameObject.SetActive(true);

        m_BtnIcon.onClick.AddListener(() =>
        {
            ShopNewUIForm uiForm = UIManager.GetInstance().GetUIForm(FormConst.SHOPNEWUIFORM) as ShopNewUIForm;
            if (uiForm != null)
            {
                uiForm.SetSelect(avatarData.avatar_id);
                AvatarManager.Instance().ChangeAvatarFun(avatarData.avatar_id);
            }

            if (avatarData.avatar_type1 == (int)AppearanceEditorUIForm.TableType.Suit && avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.HotMan)//HotMan
            {
              /*  GameObject mCurCharacterObj = CharacterManager.Instance().GetPlayerObj();
                CamCtrl camCtrl = mCurCharacterObj.GetComponent<CamCtrl>();
                if (camCtrl == null)
                {
                    camCtrl = mCurCharacterObj.AddComponent<CamCtrl>();
                    camCtrl.modelObj = mCurCharacterObj.transform;
                }*/
            }

            if (avatarData != null)
            {
                if (avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.Coat || avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.Underwear || avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.Suit)
                {
                    PlayClothAct("ShowCloth1");
                }
                if (avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.Shoe)
                {
                    PlayClothAct("ShowCloth2");
                }
            }
        });

        m_BtnBuy.onClick.AddListener(() =>
        {
            bClicked = true;
            clickTimer = 0;
            if (!bCanClick)
            {
                return;
            }
            if (avatarData.items_limited == 1 && avatarData.items_limited_quantity <= 0)//限购买完不弹框
            {
                return;
            }
            ShopNewUIForm uiForm = UIManager.GetInstance().GetUIForm(FormConst.SHOPNEWUIFORM) as ShopNewUIForm;
            if (uiForm != null)
            {
                uiForm.SetSelect(avatarData.avatar_id);
                AvatarManager.Instance().ChangeAvatarFun(avatarData.avatar_id);
            }
            OpenUIForm(FormConst.SHOPNEWBUYTIPSUIFORM);
            object[] parm = new object[] { 4, avatarData };
            SendMessage("OpenShopNewBuyTips", "Success", parm);
        });
    }

    public void SetItemIcon(int id)
    {
        bClicked = false;
        bCanClick = true;

        avatar avatarConfig = ManageMentClass.DataManagerClass.GetAvatarTableFun(id);
        if (avatarConfig != null)
        {
          /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
            Sprite sprite = atlas.GetSprite(avatarConfig.avatar_icon);*/
            m_IconImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(avatarConfig.avatar_icon);
        }
    }

    public void SetItemName(string name)
    {
        if (m_NameText != null)
            m_NameText.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_HorizontalLayoutGroup);
    }

    public void SetItemNum(int num)
    {
        m_VerticalLayoutGroup.gameObject.SetActive(num > 0);
        if (m_NumText != null)
            m_NumText.text = string.Format("{0}", num);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_VerticalLayoutGroup);
    }
    public void SetItemSelectState(bool bSelect)
    {
        if (m_SelectImg == null)
            return;
        m_SelectImg.gameObject.SetActive(bSelect);
    }

    public void SetItemQuality(int quality)
    {
        string spriteName = "";
        switch (quality)
        {
            case (int)QualityEnum.N:
                spriteName = string.Format("{0}", "N");
                break;
            case (int)QualityEnum.R:
                spriteName = string.Format("{0}", "R");
                break;
            case (int)QualityEnum.SR:
                spriteName = string.Format("{0}", "SR");
                break;
            case (int)QualityEnum.SSR:
                spriteName = string.Format("{0}", "SSR");
                break;
        }
        Debug.Log("输出一下imgeae名字： " + spriteName);
      /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Shop");
        Sprite sprite = atlas.GetSprite(spriteName);*/
        m_QualityImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadShopByPathNameFun(spriteName); 
        m_QualityImg.SetNativeSize();
    }

    public void SetItemPrice(int id)
    {
        mall mallConfig = ManageMentClass.DataManagerClass.GetMallTableFun(id);
        if (mallConfig != null)
        {
            m_PriceText.text = string.Format("{0}", mallConfig.price);

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_PriceVerticalLayoutGroup);
    }

    public void SetItemNew(bool bNew)
    {
        m_NewTrans.gameObject.SetActive(bNew);
    }

    public void SetLimitNum(bool bLimitNum, int num)
    {
        m_HaveNumText.gameObject.SetActive(bLimitNum && num > 0);
        m_HaveNumText.text = string.Format("剩余:{0}", num);

        m_NoneTrans.gameObject.SetActive(bLimitNum && num <= 0);
        m_PriceTrans.gameObject.SetActive(!bLimitNum || (bLimitNum && num > 0));
        if (bLimitNum && num <= 0)
        {
            m_IconImg.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            m_IconImg.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void SetLimitTime(int timer)
    {
        seconds = CalcTools.TimeStampChangeSecondFun(timer);
        if (seconds > 0)
        {
            m_TimeTrans.gameObject.SetActive(true);
            SetTimeStr(seconds);
        }
        else
        {
            m_TimeTrans.gameObject.SetActive(false);
        }
    }

    public void SetTimeStr(int second)
    {
        TimeSpan time = TimeSpan.FromSeconds(second);
        string str = time.ToString(@"hh\:mm\:ss");
        m_TimeText.text = string.Format("{0}后下架", str);
    }


    public void SetNumPos()
    {
        if (avatarData.items_limited_time_off_timestamp > 0)
        {
            m_VerticalLayoutGroup.anchoredPosition = new Vector3(108f, -5.4f, 0f);
        }
        else
        {
            m_VerticalLayoutGroup.anchoredPosition = new Vector3(108f, -36f, 0f);
        }
    }
    public void SetAvatarData(ShopOutFitItem data)
    {
        avatarData = data;
    }

    private void Update()
    {
        if (seconds > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                seconds -= 1;
                timer = 0;
                SetTimeStr(seconds);
            }
        }

        if (bClicked)
        {
            if (clickTimer < clickInterval)
            {
                clickTimer += Time.deltaTime;
                bCanClick = false;
            }

            if (clickTimer >= clickInterval)
            {
                clickTimer = 0;
                bCanClick = true;
                bClicked = false;
            }
        }
    }

    public void PlayClothAct(string triggerName)
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator(triggerName);
        }
    }
}
