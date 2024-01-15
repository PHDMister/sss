using System;
using Treasure;
using UIFW;
using UnityEngine.UI;

public class UIBagRewardItem:BaseUIForm
{
    private Text txtTitle;
    private Text txtContent;
    private Text txtTime;
    private void Awake()
    {
        txtTitle = transform.Find("txtTitle").GetComponent<Text>();
        txtContent = transform.Find("txtContent").GetComponent<Text>();
        txtTime = transform.Find("txtTime").GetComponent<Text>();
    }

    public void SetData(TreasureRecord data)
    {
        txtTitle.text = SceneConfig.GetName(data.SceneId);
        txtContent.text = $"[<color=#00BBB7>{data.RewardName}</color>] +{data.RewardQuantity}";
        
        DateTime dateTime = CalcTools.TimeStampChangeDateTimeFun(data.CreatedAt);
        string dateStr = dateTime.ToString("yyyy-MM.dd.HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        txtTime.text =  dateStr;
    }
}