using UIFW;
using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;

public class ClothingItem:BaseUIForm
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

    private ThreeLevelData avatarData;

    void Awake()
    {
        SetItemSelectState(false);
        m_BtnIcon.onClick.AddListener(() =>
        {
            SendMessage("ReceiveClothingItemClick", "", index);
        });
        
        ReceiveMessage("ReceiveClothingItemClick", (p) =>
        {
            var itemIndex = (int)p.Values;
            SetItemSelectState(index == itemIndex);
        });
    }

    public void SetData(ThreeLevelData data)
    {
        SetItemIcon(data.avatar_id);
        SetItemName(data.avatar_name);
        SetItemHaveNum(data.has_num);
        SetItemSelectState(index == ManageMentClass.DataManagerClass.ClothingSelectedIndex);
        SetItemUsingState(data.status == 1);
        SetItemQuality(data.avatar_rare);
        SetAvatarData(data);
    }

    private int index;
    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void SetItemIcon(int id)
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

    private void SetItemHaveNum(int num)
    {
        if (m_HaveNumText != null)
            m_HaveNumText.text = string.Format("{0}", num);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_VerticalLayoutGroup);
    }
    private void SetItemName(string name)
    {
        if (m_NameText != null)
            m_NameText.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_HorizontalLayoutGroup);
    }

    private void SetItemSelectState(bool bSelect)
    {
        if (m_SelectImg == null)
            return;
        m_SelectImg.gameObject.SetActive(bSelect);
    }

    private void SetItemUsingState(bool bUsing)
    {
        if (m_UsingImg == null)
            return;
        m_UsingImg.gameObject.SetActive(bUsing);
    }

    private void SetItemQuality(int quality)
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

    private void SetAvatarData(ThreeLevelData data)
    {
        avatarData = data;
    }
}
