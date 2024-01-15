using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 根据机型是配置
/// </summary>
public class DeviceOptimize
{
    protected static bool IsSetDevice = false;

    public static void SetDeviceOptimize()
    {
        if (IsSetDevice) return;
//#if UNITY_EDITOR
//        DevicePerformanceLevel level = GetDevicePerformanceLevel();
//#else
//        DevicePerformanceLevel level = DevicePerformanceLevel.Mid;
//#endif
//        ModifyDeviceQuality(level);

        IsSetDevice = true;

        if (!Application.isEditor)
        {
            CustomModifyDeviceQuality();
        }
    }


    public static DevicePerformanceLevel GetDevicePerformanceLevel()
    {
        string platform = SetTools.GetEnvironmentPlatform();
        //显存
        int graphicsMemorySize = SystemInfo.graphicsMemorySize;
        //内存
        int systemMemorySize = SystemInfo.systemMemorySize;
        Debug.Log("111111 DeviceOptimize   platform=" + platform + "     graphicsMemorySize=" + graphicsMemorySize + "  systemMemorySize=" + systemMemorySize);
        if (SystemInfo.graphicsDeviceVendorID == 32902)
        {
            //集显
            return DevicePerformanceLevel.Low;
        }

        if (Application.isEditor || platform == "PC")
        {
            return DevicePerformanceLevel.High;
        }

        if (platform == "Android")
        {
            if (SystemInfo.processorCount <= 4)
                return DevicePerformanceLevel.Low;
            if (graphicsMemorySize >= 6000 && systemMemorySize >= 8000)
                return DevicePerformanceLevel.High;
            if (graphicsMemorySize >= 2000 && systemMemorySize >= 4000)
                return DevicePerformanceLevel.Mid;
            else
                return DevicePerformanceLevel.Low;
        }

        if (platform == "Iphone" || platform == "Ipad")
        {
            if (SystemInfo.processorCount < 2)
                return DevicePerformanceLevel.Low;
            if (graphicsMemorySize >= 4000 && systemMemorySize >= 8000)
                return DevicePerformanceLevel.High;
            if (graphicsMemorySize >= 2000 && systemMemorySize >= 4000)
                return DevicePerformanceLevel.Mid;
            else
                return DevicePerformanceLevel.Low;
        }

        return DevicePerformanceLevel.Mid;
    }

    public static void ModifyDeviceQuality(DevicePerformanceLevel level)
    {
        switch (level)
        {
            case DevicePerformanceLevel.Low:
                QualitySettings.masterTextureLimit = 2;
                QualitySettings.realtimeReflectionProbes = false;
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.vSyncCount = 0;
                QualitySettings.antiAliasing = 0;
                Application.targetFrameRate = 30;
                break;
            case DevicePerformanceLevel.Mid:
                QualitySettings.masterTextureLimit = 1;
                QualitySettings.realtimeReflectionProbes = false;
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.vSyncCount = 0;
                QualitySettings.antiAliasing = 0;

                string platform = SetTools.GetEnvironmentPlatform();
                if (platform == "Iphone" || platform == "Ipad")
                    Application.targetFrameRate = 45;
                else if (platform == "Android")
                    Application.targetFrameRate = -1;
                break;
            case DevicePerformanceLevel.High:
                Application.targetFrameRate = 50;
                QualitySettings.masterTextureLimit = 0;
                QualitySettings.realtimeReflectionProbes = false;
                break;
        }
    }

    public static void CustomModifyDeviceQuality()
    {
        QualitySettings.masterTextureLimit = 1;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;

        string platform = SetTools.GetEnvironmentPlatform();
        if (platform == "Iphone" || platform == "Ipad")
            Application.targetFrameRate = 40;
        else if (platform == "Android")
            Application.targetFrameRate = -1;
    }

    public static void ResetSreenSize(float ratio)
    {
        int sWidth = SetTools.getASize();
        int sHeight = SetTools.getBSize();

        int nWidth = Mathf.CeilToInt(sWidth * ratio);
        int nHeight = Mathf.CeilToInt(sHeight * ratio);

        SetTools.ResetABCanvasSize(nWidth, nHeight);
        Screen.SetResolution(nWidth, nHeight, false);
    }


}


public enum DevicePerformanceLevel
{
    Low,
    Mid,
    High
}