//<Tools\GenUICode>工具生成, UI变化重新生成
using DG.Tweening;
using UnityEngine;
using Treasure;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;

public class TreasureRewardPanel : BaseUIForm
{

    // UI VARIABLE STATEMENT START
    private Image image_shangpintu;
    private Text text_txtname;
    private Button button_btnBack;
    private Image image_stars;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        image_shangpintu = FindComp<Image>("jianglitanchuang/mianban/cangpinjiangli-bg/shangpintubg-1/Render/Mask/shangpintu");
        text_txtname = FindComp<Text>("jianglitanchuang/mianban/Frame1000008158/txtname");
        button_btnBack = FindComp<Button>("btnBack");
        image_stars = FindComp<Image>("jianglitanchuang/mianban/str/Render");
        if(image_shangpintu != null)
        {
            image_shangpintu.gameObject.SetActive(false);
        }
        OnAwake();
        AddEvent();

    }
    // UI VARIABLE ASSIGNMENT END

    // UI EVENT REGISTER START
    private void AddEvent()
    {
        RigisterCompEvent(button_btnBack, OnBackHandle);
    }
    // UI EVENT REGISTER END

    // UI EVENT FUNC START
    private void OnBackHandle(GameObject go)
    {
        CloseUIForm();
    }
    // UI EVENT FUNC END
    private Tweener tweener;
    private void OnAwake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

    }

    public override void Display()
    {
        base.Display();
        //ShowDefineIcon();
        ShowRewardItem();

        tweener = DOTween.To(fact => { image_stars.fillAmount = fact; }, 0, 1f, 1f);
        tweener.SetLoops(-1, LoopType.Restart);
        
        MessageCenter.SendMessage("CloseChatUI", KeyValuesUpdate.Empty);
    }

    public override void Hiding()
    {
        base.Hiding();
        Singleton<TreasuringController>.Instance.RewardPush = null;
        tweener.Kill();
        tweener = null;

        MessageCenter.SendMessage("OpenChatUI", KeyValuesUpdate.Empty);
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    private void ShowDefineIcon()
    {
        SetIcon(image_shangpintu, "TreasureDigging", "changpinjiangli");
    }
    private void ShowRewardItem()
    {
        RewardPush reward = Singleton<TreasuringController>.Instance.RewardPush;
        TreasureRewardConfData data = TreasureModel.Instance.RewardPreviewData.Find(x => x.RewardId == reward.RewardId);
        if (data != null)
        {
            text_txtname.text = string.Format("{0}*{1}", data.RewardName, data.RewardQuantity);
            MessageManager.GetInstance().DownLoadPicture(data.RewardIcon, (sprite) =>
            {
                if (image_shangpintu != null)
                {
                    image_shangpintu.sprite = sprite;
                    image_shangpintu.gameObject.SetActive(true);
                }
            },
            () =>
            {
                SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/TreasureDigging");
                Sprite spriteNull = atlas.GetSprite("changpinjiangli");
                if(image_shangpintu != null)
                {
                    image_shangpintu.sprite = spriteNull;
                    image_shangpintu.gameObject.SetActive(true);
                }
            });
        }
    }

}
