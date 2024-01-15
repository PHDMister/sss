using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;
using System;
using DG.Tweening;

public class RewardItem : BaseUIForm
{
    public Text m_NameText;
    public Text m_RateText;
    public Image m_IconImg;
    public Image m_StarImg;

    void Awake()
    {

    }

    public void SetName(string name, uint num)
    {
        m_NameText.text = string.Format("{0}*{1}", name, num);
    }

    public void SetIcon(string url)
    {
        MessageManager.GetInstance().DownLoadPicture(url, (sprite) =>
         {
             m_IconImg.sprite = sprite;
             //m_IconImg.SetNativeSize();
         },
        () =>
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/TreasureDigging");
            Sprite spriteNull = atlas.GetSprite("changpinjiangli");
            m_IconImg.sprite = spriteNull;
        });
    }

    public void SetIcon(Sprite sprite)
    {
        if (sprite == null)
            return;
        m_IconImg.sprite = sprite;
    }

    public void SetRate(uint rewardId, List<TreasureRewardConfData> list)
    {
        uint totalWeight = 0;
        uint curWeight = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].RewardId == rewardId)
            {
                curWeight = list[i].RewardWeight;
            }
            totalWeight += list[i].RewardWeight;
        }
        float rate = (float)curWeight / totalWeight;
        string rateStr = (rate * 100).ToString("0.0");
        m_RateText.text = string.Format("{0}%¸ÅÂÊ", rateStr);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_RateText.transform.parent.GetComponent<RectTransform>());
    }

    public void PlayStarAni()
    {
        if (m_StarImg != null)
            m_StarImg.fillAmount = 0f;
        DOTween.To(delegate (float value)
        {
            if (m_StarImg != null)
                m_StarImg.fillAmount = value;

        }, 0, 1f, 1f).SetLoops(-1, LoopType.Restart);
    }
}
