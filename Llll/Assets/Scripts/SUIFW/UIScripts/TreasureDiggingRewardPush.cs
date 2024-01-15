using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;
using Treasure;
using UnityEngine.U2D;
using DG.Tweening;

public class TreasureDiggingRewardPush : BaseUIForm
{
    public Text m_NameText;
    public Image m_IconImg;
    public Image m_LightImg;
    public Image m_StarImg;

    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });

        ReceiveMessage("RewardPush", p =>
         {
             RewardPush data = p.Values as RewardPush;
             if (data == null)
                 return;
             SetRewardInfo(data);
         });
    }

    public void SetRewardInfo(RewardPush data)
    {
        treasure_reward itemCfg = ManageMentClass.DataManagerClass.GetTreasureRewardTable((int)data.RewardId);
        if (itemCfg != null)
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/TreasureDigging");
            if (string.IsNullOrEmpty(itemCfg.reward_icon))
            {
                Sprite spriteNull = atlas.GetSprite("changpinjiangli");
                m_IconImg.sprite = spriteNull;
                return;
            }
            Sprite sprite = atlas.GetSprite(itemCfg.reward_icon);
            m_IconImg.sprite = sprite;
            m_NameText.text = string.Format("{0}*1", itemCfg.reward_name);
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        PlayUIAni();
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }

    public void PlayUIAni()
    {
        if (m_StarImg != null)
            m_StarImg.fillAmount = 0f;
        DOTween.To(delegate (float value)
        {
            if (m_StarImg != null)
                m_StarImg.fillAmount = value;

        }, 0, 1f, 1.2f).SetLoops(-1, LoopType.Restart);

        if (m_LightImg != null)
            m_LightImg.fillAmount = 0f;
        DOTween.To(delegate (float value)
        {
            if (m_LightImg != null)
                m_LightImg.fillAmount = value;

        }, 0, 1f, 1.5f).SetLoops(-1, LoopType.Restart);
    }
}
