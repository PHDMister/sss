using UIFW;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ÿ��ˢ�¹�����
/// </summary>
public class DailyMgr : MonoSingleton<DailyMgr>
{
    public const int RefreshHour = 0;//����ˢ��

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

    float lastCheckTime;//��һ�μ���ʱ��
    private void Update()
    {
        //5����һ��
        if (Time.realtimeSinceStartup - lastCheckTime > 5)
        {
            lastCheckTime = Time.realtimeSinceStartup;
            CheckRefresh();
        }
    }

    /// <summary>
    /// ���ˢ��
    /// </summary>
    void CheckRefresh()
    {
        DateTime now = DateTime.Now;
        int lastDayOfYear = PlayerPrefs.GetInt("lastDayOfYear", -1);
        if (now.DayOfYear != lastDayOfYear)
        {
            //ͬһ��
            if (now.DayOfYear > lastDayOfYear)
            {
                //����һ��δ��½
                if (now.DayOfYear - lastDayOfYear >= 2)
                {
                    PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                    RefreshData();
                }
                //ֻ��һ��δ��½
                else
                {
                    if (now.Hour >= RefreshHour)
                    {
                        PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                        RefreshData();
                    }
                }
            }
            //����
            else if (now.DayOfYear < lastDayOfYear)
            {
                int lastYear = now.Year - 1;
                int lastYearTotalDays = lastYear % 4 == 0 ? 366 : 365;
                //����һ��δ��½
                if (lastYearTotalDays + now.DayOfYear - lastDayOfYear >= 2)
                {
                    PlayerPrefs.SetInt("lastDayOfYear", now.DayOfYear);
                    RefreshData();
                }
                //һ��δ��½���ҵ���ˢ��ʱ��
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
    /// ˢ��(������Ҫÿ��ˢ�µ�����)
    /// </summary>
    public void RefreshData()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        if(level == 2)//���Ƴ���С������
        {
            MessageManager.GetInstance().RequestPetList(() =>
            {
                PetSpanManager.Instance().UpdatePet();
            });
        }
    }
}