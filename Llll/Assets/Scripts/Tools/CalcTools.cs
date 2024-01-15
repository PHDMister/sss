using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 算法相关工具类
/// </summary>
public static class CalcTools
{
    //将分钟转换为 小时：分钟 格式
    public static string ChangeMinuteFun(int minute)
    {
        int hourValue = minute / 60;
        int minuteValue = minute % 60;

        string strHourValue = hourValue + "";
        string strMinuteValue = minuteValue + "";
        if (hourValue < 10)
        {
            strHourValue = "0" + hourValue;
        }
        if (minuteValue < 10)
        {
            strMinuteValue = "0" + minuteValue;
        }
        string strTime = strHourValue + ":" + strMinuteValue + ":00";
        // var aaa = hourValue, minuteValue;
        return strTime;
    }

    //将分钟转换为 小时：分钟 格式
    public static string ChangeSecondFun(int second)
    {
        int hourValue = second / 3600;
        int hourResidueValue = second % 3600;
        int minuteValue = hourResidueValue / 60;
        int secondValue = hourResidueValue % 60;

        string strHourValue = hourValue < 10 ? "0" + hourValue : hourValue.ToString();
        string strMinuteValue = minuteValue < 10 ? "0" + minuteValue : minuteValue.ToString();
        string strSecondValue = secondValue < 10 ? "0" + secondValue : secondValue.ToString();

        if (hourValue < 10)
        {
            strHourValue = "0" + hourValue;
        }
        if (minuteValue < 10)
        {
            strMinuteValue = "0" + minuteValue;
        }
        string strTime = strHourValue + ":" + strMinuteValue + ":" + strSecondValue;
        // var aaa = hourValue, minuteValue;
        return strTime;
    }
    /// <summary>
    /// 时间戳转换成秒
    /// </summary>
    /// <returns></returns>
    public static int TimeStampChangeSecondFun(long timeStamp)
    {
        DateTimeOffset currentTime = DateTimeOffset.Now;
        long timestampValue = currentTime.ToUnixTimeSeconds();
        int seconds = (int)(timeStamp - timestampValue);
        return seconds;
    }
    /// <summary>
    ///时间戳转换为时间格式（年月日时分秒）
    /// </summary>
    /// <param name="timestampValue"></param>
    /// <returns></returns>
    public static DateTime TimeStampChangeDateTimeFun(long timestampValue)
    {
        DateTimeOffset convertedDateTime = DateTimeOffset.FromUnixTimeSeconds(timestampValue);
        DateTime localDateTime = convertedDateTime.LocalDateTime;
        return localDateTime;
    }

    /// <summary>
    /// DateTime转时间戳
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>时间戳（秒）</returns>
    public static long GetUnixTimeStamp(DateTime dt)
    {
        DateTime dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
        long timeStamp = Convert.ToInt32((dt - dtStart).TotalSeconds);
        return timeStamp;
    }

    public static int GetTimeStampDiff(long curTimestamp, long endTimestamp)
    {
        int seconds = (int)(endTimestamp - curTimestamp);
        return seconds;
    }
    /// <summary>
    /// 判断两个是否是同一天
    /// </summary>
    /// <param name="cur"></param>
    /// <param name="old"></param>
    /// <returns></returns>
    public static bool IsNextOrDoubleDay(int cur, int old)
    {
        DateTime dta = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
        dta = dta.AddSeconds(cur);
        DateTime dtb = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
        dtb = dtb.AddSeconds(old);
        return dta.Year > dtb.Year || dta.Month > dtb.Month || dta.Day - dtb.Day >= 1;
    }

    /// <summary>
    /// 获取现在时间戳，秒
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}
