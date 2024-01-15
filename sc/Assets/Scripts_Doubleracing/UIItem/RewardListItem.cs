using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Fight;
using System;

public class RewardListItem : BaseUIForm
{
    // UI VARIABLE STATEMENT START
    private Image image_Success_BGImg;
    private Image image_Fail_BGImg;
    private Image image_Success_Panel;
    private Text text_ShellValue_Text;
    private Image image_Fail_Panel;
    private Text text_IDName_Text;
    private Text text_time_Text;
    // UI VARIABLE STATEMENT END

    // UI VARIABLE ASSIGNMENT START
    private void Awake()
    {
        image_Success_BGImg = FindComp<Image>("Success_BGImg");
        image_Fail_BGImg = FindComp<Image>("Fail_BGImg");
        image_Success_Panel = FindComp<Image>("Success_Panel");
        text_ShellValue_Text = FindComp<Text>("Success_Panel/ShellValue_Text");
        image_Fail_Panel = FindComp<Image>("Fail_Panel");
        text_IDName_Text = FindComp<Text>("ID_Image/IDName_Text");
        text_time_Text = FindComp<Text>("Time_Text");
    }

    public void SetUIDataFun(FightData data)
    {
        int proID = 2;
        bool isWin = false;


        /*  DateTime dateTime = DateTime.FromUnixTimeSeconds(timestamp); // 转换为DateTime类型

          string dateString = dateTime.ToString("yyyy-MM-dd HH:mm:ss");*/

        System.DateTime sss = CalcTools.TimeStampChangeDateTimeFun(data.CreatedAt);
        //string sss = CalcTools.ChangeSecondFun(SecondTimeValue);
        text_time_Text.text = sss + "";
       // Debug.Log("输出一下这里的值的内容这里的时间 " + sss + "    SecondTimeValue: " + sss.ToJSON());
        if (ManageMentClass.DataManagerClass.userId == data.PoliceId)
        {
            proID = 1; 
            //进攻者
            if (data.FightResult == 1)
            {
                //本局赢
                isWin = true;
            }
        }
        else
        {
            if (data.FightResult == 2)
            {
                isWin = true;
            }
        }
        if (data.FightResult == 0)
        {
            //固定输
            isWin = false;
        }

        if (isWin)
        {
            image_Success_BGImg.gameObject.SetActive(true);
            image_Fail_BGImg.gameObject.SetActive(false);

            image_Success_Panel.transform.gameObject.SetActive(true);
            image_Fail_Panel.transform.gameObject.SetActive(false);
            text_ShellValue_Text.text = "+" + data.RewardCount;

        }
        else
        {
            image_Success_BGImg.gameObject.SetActive(false);
            image_Fail_BGImg.gameObject.SetActive(true);

            image_Success_Panel.transform.gameObject.SetActive(false);
            image_Fail_Panel.transform.gameObject.SetActive(true);
        }

        text_IDName_Text.text = proID == 1 ? "追击者" : "出逃者";

    }

}
