using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;

public class ClothingItem : BaseUIForm
{
    public enum QualityEnum
    {
        N = 1,
        R = 2,
        SR = 3,
        SSR = 4,
    }
    public Button m_BtnIcon;
    public RawImage m_IconImg;
    public Text m_HaveNumText;
    public Text m_NameText;
    public Image m_SelectImg;
    public Image m_UsingImg;
    public Image m_QualityImg;

    public RectTransform m_HorizontalLayoutGroup;
    public RectTransform m_VerticalLayoutGroup;

    public ThreeLevelData avatarData;
    // Start is called before the first frame update
    void Awake()
    {
        SetItemSelectState(false);
        m_BtnIcon.onClick.AddListener(() =>
        {
            AppearanceEditorUIForm uiForm = UIManager.GetInstance().GetUIForm(FormConst.APPEARANCEEDITORUIFORM) as AppearanceEditorUIForm;
            if (uiForm != null)
            {
                uiForm.SetSelect(avatarData.avatar_id);
                uiForm.SetOptClothingData(avatarData.avatar_type2, avatarData.avatar_id);
                uiForm.SetSaveBtnState();
                AvatarManager.Instance().ChangeAvatarFun(avatarData.avatar_id);

                /* GameObject mCurCharacterObj = CharacterManager.Instance().GetPlayerObj();
                 CamCtrl camCtrl = mCurCharacterObj.GetComponent<CamCtrl>();
                 if (camCtrl == null)
                 {
                     camCtrl = mCurCharacterObj.AddComponent<CamCtrl>();
                     camCtrl.modelObj = mCurCharacterObj.transform;
                 }
                 else
                 {
                     camCtrl.modelObj = mCurCharacterObj.transform;
                 }*/


                if (avatarData.avatar_type1 == (int)AppearanceEditorUIForm.TableType.Suit && avatarData.avatar_type2 == (int)AppearanceEditorUIForm.AppearanceTableType.HotMan)//HotMan
                {

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
            }
        });
    }

    public void SetItemIcon(int id)
    {
        avatar avatarConfig = ManageMentClass.DataManagerClass.GetAvatarTableFun(id);
        if (avatarConfig != null)
        {
            /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
              Debug.Log("这里的物品的信息名称： " + avatarConfig.avatar_icon);
              Sprite sprite = atlas.GetSprite(avatarConfig.avatar_icon);
  */
            m_IconImg.texture = ManageMentClass.ResourceControllerClass.ResLoadIconTextureByPathNameFun(avatarConfig.avatar_icon);
        }
        else
        {
            skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(id);
            if (skinColor != null)
            {
                m_IconImg.texture = ManageMentClass.ResourceControllerClass.ResLoadIconTextureByPathNameFun(skinColor.avatar_icon);
            }
        }
    }

    public void SetItemHaveNum(int num)
    {
        if (m_HaveNumText != null)
            m_HaveNumText.text = string.Format("{0}", num);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_VerticalLayoutGroup);
    }
    public void SetItemName(string name)
    {
        if (m_NameText != null)
            m_NameText.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_HorizontalLayoutGroup);
    }

    public void SetItemSelectState(bool bSelect)
    {
        if (m_SelectImg == null)
            return;
        m_SelectImg.gameObject.SetActive(bSelect);
    }

    public void SetItemUsingState(bool bUsing)
    {
        if (m_UsingImg == null)
            return;
        m_UsingImg.gameObject.SetActive(bUsing);
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
        /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Shop");
         Sprite sprite = atlas.GetSprite(spriteName);*/
        m_QualityImg.sprite = ManageMentClass.ResourceControllerClass.ResLoadShopByPathNameFun(spriteName);
        m_QualityImg.SetNativeSize();
    }

    public void SetAvatarData(ThreeLevelData data)
    {
        avatarData = data;
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
