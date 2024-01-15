using System;
using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityTimer;

public class TreasureEndTimer : MonoBehaviour
{
    private static TreasureEndTimer _instance = null;
    public static TreasureEndTimer Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("TreasureEndTimer").AddComponent<TreasureEndTimer>();
        }
        return _instance;
    }

    private List<Material> materials;
    private int[] numArry = new int[6];
    private float m_Timer = 0;

    private GameObject m_WallObj;
    private GameObject m_WallBgObj;
    private string m_NoneTimerTex = "T_wenzi";//无倒计时贴图
    private string m_TimerTex = "T_wenzi_daojishi";//有倒计时贴图
    public void Init()
    {
        m_WallObj = GameObject.FindGameObjectWithTag("_TagEndTimer");
        m_WallBgObj = GameObject.Find("model/M_Twenziqiang01/M_Twenziqiang01_06");

        if (m_WallObj == null || m_WallBgObj == null)
            return;

        if (materials == null)
        {
            materials = new List<Material>();
            Transform hourNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Hour_tens");
            List<Material> hourNum2Materials = InterfaceHelper.FindChildMaterials(hourNum2Trans);
            materials.AddRange(hourNum2Materials);
            Transform hourNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Hour_ones");
            List<Material> hourNum1Materials = InterfaceHelper.FindChildMaterials(hourNum1Trans);
            materials.AddRange(hourNum1Materials);
            Transform MinNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Min_tens");
            List<Material> MinNum2Materials = InterfaceHelper.FindChildMaterials(MinNum2Trans);
            materials.AddRange(MinNum2Materials);
            Transform MinNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Min_ones");
            List<Material> MinNum1Materials = InterfaceHelper.FindChildMaterials(MinNum1Trans);
            materials.AddRange(MinNum1Materials);
            Transform secNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Sec_tens");
            List<Material> secNum2Materials = InterfaceHelper.FindChildMaterials(secNum2Trans);
            materials.AddRange(secNum2Materials);
            Transform secNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Sec_ones");
            List<Material> secNum1Materials = InterfaceHelper.FindChildMaterials(secNum1Trans);
            materials.AddRange(secNum1Materials);
        }

        if (TreasureModel.Instance.StartTime <= 0 && TreasureModel.Instance.EndTime <= 0)
        {
            Texture mTexture = Resources.Load(m_NoneTimerTex, typeof(Texture)) as Texture;
            m_WallBgObj.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", mTexture);

            m_WallObj.SetActive(false);
        }
        else
        {
            Texture mTexture = Resources.Load(m_TimerTex, typeof(Texture)) as Texture;
            m_WallBgObj.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", mTexture);

            long seconds = (long)TreasureModel.Instance.EndTime - CalcTools.GetTimeStamp();
            m_Timer = (float)seconds;
            if (seconds > 0)
            {
                m_WallObj.SetActive(true);
                Timer.RegisterRealTimeNoLoop(seconds, OnEndTreasureComplete, OnEndTreasureUpdate);
            }
            else
            {
                m_WallObj.SetActive(false);
            }
        }
    }

    void OnEndTreasureComplete()
    {
        if (m_WallObj != null)
            m_WallObj.SetActive(false);
        MessageManager.GetInstance().SendMessage("TreasureEnd", "End", null);
    }

    void OnEndTreasureUpdate(float time)
    {
        float second = m_Timer - time;
        ShowTimer(second);
    }

    void ShowTimer(float seconds)
    {
        int hour = (int)seconds / 3600;
        int residue = (int)seconds % 3600;
        int minute = residue / 60;
        int second = residue % 60;

        int hour2 = hour < 10 ? 0 : hour / 10;
        int hour1 = hour < 10 ? hour : hour % 10;
        int minute2 = minute < 10 ? 0 : minute / 10;
        int minute1 = minute < 10 ? minute : minute % 10;
        int second2 = second < 10 ? 0 : second / 10;
        int second1 = second < 10 ? second : second % 10;

        numArry[0] = hour2;
        numArry[1] = hour1;
        numArry[2] = minute2;
        numArry[3] = minute1;
        numArry[4] = second2;
        numArry[5] = second1;

        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            if (material != null)
            {
                material.SetFloat("_Number", numArry[i]);
            }
        }
    }
}
