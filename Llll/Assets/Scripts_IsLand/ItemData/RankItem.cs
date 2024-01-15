using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.U2D;
using Treasure;

public class RankItem : BaseUIForm
{
    public Image img_rank_bg;
    public Text txt_rank;
    public Image headImg;
    public Text name;
    public Text gongyi;

    public void SetData(WelfareData data)
    {
        txt_rank.text = data.Index <= 3 ? "" : data.Index.ToString();
        SetRankIcon(data.Index);
        LoadHeadIcon(data.Head);
        name.text = TextTools.setCutAddString(data.Name, 2, "*");
        gongyi.text = data.WelfareCount.ToString();
    }

    protected void LoadHeadIcon(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            headImg.sprite = InterfaceHelper.GetDefaultAvatarFun();
            return;
        }
        if (Singleton<RainbowBeachDataModel>.Instance.TryGetHeadCache(url, out var value))
        {
            headImg.sprite = value;
        }
        else
        {
            MessageManager.GetInstance().DownLoadAvatar(url, (avaUrl, sprite) =>
            {
                headImg.sprite = sprite;
                Singleton<RainbowBeachDataModel>.Instance.AddHeadCache(avaUrl, sprite);
            });
        }
    }

    private void SetRankIcon(uint no)
    {
        if (no == 1) SetIcon(img_rank_bg, "RankList", "rank_bg_1");
        else if (no == 2) SetIcon(img_rank_bg, "RankList", "rank_bg_2");
        else if (no == 3) SetIcon(img_rank_bg, "RankList", "rank_bg_3");
        img_rank_bg.gameObject.SetActive(no <= 3);
    }

}
