using UIFW;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 每日刷新管理器
/// </summary>
public class DailyMgr : MonoSingleton<DailyMgr>
{
    public const int RefreshHour = 0;//几点刷新

    public void Awake()
    {
        CheckRefresh();
        lastCheckTime = Time.realtimeSinceStartup;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            CheckRefresh();
        }
    }

    float lastCheckTime;//上一次检测的时间
    private void Update()
    {
        //5秒检测一次
        if (Time.realtimeSinceStartup - lastCheckTime > 5)
        {
            lastCheckTime = Time.realtimeSinceStartup;
            CheckRefresh();
        }
    }

    /// <summary>
    /// 检测刷新
    /// </summary>
    void CheckRefresh()
    {
        DateTime now = DateTime.Now;
        int lastDayOfYear = PlayerPrefs.GetInt("lastDayOfYear", -1);
        if (now.DayOfYear != lastDayOfYear)
        {
            //同一年
            if (now.DayOfYear > lastDayOfYear)
            {
                //超过一天未登陆
                if (now.DayOfYear - lastDayOfYear >= 2)
                {
                    PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                    RefreshData();
                }
                //只有一天未登陆
                else
                {
                    if (now.Hour >= RefreshHour)
                    {
                        PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                        RefreshData();
                    }
                }
            }
            //跨年
            else if (now.DayOfYear < lastDayOfYear)
            {
                int lastYear = now.Year - 1;
                int lastYearTotalDays = lastYear % 4 == 0 ? 366 : 365;
                //超过一天未登陆
                if (lastYearTotalDays + now.DayOfYear - lastDayOfYear >= 2)
                {
                    PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                    RefreshData();
                }
                //一天未登陆并且到达刷新时间
                else
                {
                    if (now.Hour >= RefreshHour)
                    {
                        PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                        RefreshData();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 刷新(所有需要每日刷新的数据)
    /// </summary>
    public void RefreshData()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        if(level == 2)//限制宠物小窝请求
        {
            MessageManager.GetInstance().RequestPetList(() =>
            {
                PetSpanManager.Instance().UpdatePet();
            });
        }
    }
}