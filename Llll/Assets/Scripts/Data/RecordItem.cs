using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using Treasure;
using System;

public class RecordItem : BaseUIForm
{
    public Text m_DescText;
    public Text m_TimeText;

    void Awake()
    {

    }

    public void SetDesc(string name, string rewardName, uint rewardNum)
    {
        string shortName = TextTools.setCutAddString(name, 8, "...");
        //string itemName = "";
        //treasure_reward itemCfg = ManageMentClass.DataManagerClass.GetTreasureRewardTable((int)rewardId);
        //if (itemCfg != null)
        //{
        //    itemName = itemCfg.reward_name;
        //}
        m_DescText.text = string.Format("Íæ¼Ò[<color=#00BBB7>{0}</color>]ÍÚµ½ÁË[<color=#C8A000>{1}*{2}</color>]", shortName, rewardName, rewardNum);
    }

    public void SetTime(long time)
    {
        DateTime dateTime = CalcTools.TimeStampChangeDateTimeFun(time);
        string dateStr = dateTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        m_TimeText.text = dateStr;
    }
}
