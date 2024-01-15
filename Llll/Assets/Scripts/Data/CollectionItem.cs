using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;
using System;

public class CollectionItem : BaseUIForm
{
    public Text m_NameText;
    public Text m_NumText;
    public Image m_ImgIcon;
    public Button m_BtnNone;
    private CollectioListRecData m_CollectionData;

    void Awake()
    {
        if (m_BtnNone != null)
        {
            m_BtnNone.onClick.AddListener(OnClickNone);
        }
    }

    public void OnClickNone()
    {
        if (m_CollectionData == null)
            return;
        string url = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}", m_CollectionData.icollection_id, m_CollectionData.collectiont_size_id);
        Debug.Log("OnClickNone  url: " + url);
        try
        {
            SetTools.SetPortraitModeFun();
            SetTools.CloseGameFun();
            SetTools.OpenWebUrl(url);
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception£º " + e);
        }
    }

    public void SetName(string name, int num)
    {
        string shortName = TextTools.setCutAddString(name, 8, "...");
        if (m_NameText != null)
        {
            m_NameText.text = string.Format("{0}*{1}", shortName, num);
        }
    }

    public void SetName(string name, int num, int buyNum)
    {
        string shortName = TextTools.setCutAddString(name, 8, "...");
        if (m_NameText != null)
        {
            m_NameText.text = string.Format("{0}*{1}", shortName, num * buyNum);
        }
    }

    public void SetHaveNum(int num)
    {
        if (m_NumText != null)
        {
            m_NumText.text = string.Format("ÓµÓÐ£º{0}", num);
        }
    }

    public void SetIcon(string avatar)
    {
        if (string.IsNullOrEmpty(avatar))
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/TreasureDigging");
            Sprite spriteNone = atlas.GetSprite("icon");
            if (m_ImgIcon)
            {
                m_ImgIcon.sprite = spriteNone;
            }
            return;
        }

        if (!TreasureModel.Instance.m_CollectionSpriteDic.ContainsKey(m_CollectionData.icollection_id))
        {
            MessageManager.GetInstance().DownLoadAvatar(avatar, (sprite) =>
            {
                m_ImgIcon.sprite = sprite;
                KeyValuesUpdate kvs = new KeyValuesUpdate("", new object[] { m_CollectionData.icollection_id, sprite });
                MessageCenter.SendMessage("SetIconEnd", kvs);
            });
        }
    }

    public void SetIcon(Sprite sprite)
    {
        if (sprite == null)
            return;
        if (m_ImgIcon)
        {
            m_ImgIcon.sprite = sprite;
        }
    }

    public void SetNoneState(bool bNone)
    {
        if (m_BtnNone != null)
        {
            m_BtnNone.gameObject.SetActive(bNone);
        }
    }

    public void SetCollectionData(CollectioListRecData data)
    {
        m_CollectionData = data;
    }

    public void SetNumState(bool bShow)
    {
        if (m_NumText != null)
        {
            m_NumText.gameObject.SetActive(bShow);
        }
    }

    public void SetNamePosition(Vector2 pos)
    {
        if (m_NameText != null)
        {
            m_NameText.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}
