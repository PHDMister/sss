using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SetTools
{
    public static string TestResult = "这里是个空值";
    // 返回网页地址信息
    [DllImport("__Internal")]
    public static extern string StringReturnValueFunction();

    // 取消头部导航
    [DllImport("__Internal")]
    public static extern void CloseTopBarFun();

    // 显示头部导航
    [DllImport("__Internal")]
    public static extern void ShowTopBarFun();

    //设置横屏
    [DllImport("__Internal")]
    private static extern void SetLandscapeModeFun();
    //获取屏幕宽度
    [DllImport("__Internal")]
    private static extern int GetWindowWidth();

    //获取屏幕高度
    [DllImport("__Internal")]
    private static extern int GetWinddowHeight();

    //获取修改屏幕大小
    [DllImport("__Internal")]
    public static extern int ResetCanvasSize(int width, int height);

    //关闭游戏
    [DllImport("__Internal")]
    public static extern int CloseGameFun();
    //旋转屏幕
    [DllImport("__Internal")]
    public static extern int SetPortraitModeFun();


    // 
    [DllImport("__Internal")]
    public static extern int ResetABCanvasSize(int width, int height);


    // 
    [DllImport("__Internal")]
    public static extern int getASize();


    // 
    [DllImport("__Internal")]
    public static extern int getBSize();


    //去彩虹集市
    [DllImport("__Internal")]
    public static extern void GoToRainbowBazaar(string index);


    //打开网址
    [DllImport("__Internal")]
    public static extern void OpenWebUrl(string Url);

    //关闭系统软键盘
    [DllImport("__Internal")]
    public static extern void HideKeyboardFun();

    ////调浏览器播放器
    [DllImport("__Internal")]
    public static extern void showVideoPlayer();


    ////跳转app客服页
    [DllImport("__Internal")]
    public static extern void OpenChatWindow();

    //获取移动端还是PC端
    [DllImport("__Internal")]
    public static extern int GetOperationPlatformFun();


    //加密
    [DllImport("__Internal")]
    public static extern string encrypt(string str, string key);

    //云信SDK==============================================
    [DllImport("__Internal")]
    public static extern string ImUtilIns();
    //SDK初始化
    [DllImport("__Internal")]
    public static extern string ImInit(string appKey, string token, string account);
    //更新用户信息
    [DllImport("__Internal")]
    public static extern string ImUpdateUserInfo(string nickname, string avatar);
    //进入群
    [DllImport("__Internal")]
    public static extern string ImInChatRoom(string roomId, string roomPath);
    //退出群的时候需要调用
    [DllImport("__Internal")]
    public static extern string ImDisconnect();
    //建立会话
    [DllImport("__Internal")]
    public static extern string ImConnect();
    //发送消息
    [DllImport("__Internal")]
    public static extern string ImSendTextMessage(string msg, string toUId, string custom);
    //退出网页的时候断开连接
    [DllImport("__Internal")]
    public static extern string ImOutLogin();

    //获取用户资料
    [DllImport("__Internal")]
    public static extern string ImGetUserInfo(string accId);
    //云信SDK==============================================

    public static Vector2 GetWindowSize()
    {
        int w = GetWindowWidth();
        int h = GetWinddowHeight();

        return new Vector2(w, h);
    }
    //设置最大屏幕适配分辨率
    public static void SetCanvasMaxSize()
    {
        Vector2 vector2 = GetWindowSize();
        int width, height;
        if (vector2.x / vector2.y > 1.77)  //1.77 = 16/9;//16:9画布比例
        {
            width = (int)(vector2.y * 1.77);
            height = (int)vector2.y;
            ResetCanvasSize(width, height);
        }
        else
        {
            width = (int)(vector2.x);
            height = (int)(vector2.x / 1.77);
            ResetCanvasSize(width, height);
        }
        Screen.SetResolution(width, height, false);
    }
    //设置自定义分辨率
    public static void SetCustonCanvasSize(int width, int height)
    {
        // ResetCanvasSize(width, height);
        Screen.SetResolution(width, height, false);
    }
    /// <summary>
    /// 设置横屏
    /// </summary>
    private static void SetLandscapeOrientation()
    {
        // 设置游戏方向为横屏
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 如果当前平台是移动平台，则调整游戏分辨率以适应横屏
        if (Application.isMobilePlatform)
        {
            int height = Screen.height;
            int width = Screen.width;
            ResetCanvasSize(width, height);
            Screen.SetResolution(width, height, true);
        }
    }


    //获取当前环境的平台类型
    //返回值： Ipad:1 ,Iphone:2 ,Android:3 ,WindowsCe:4 ,WindowsMobile:5 ,PC(非移动端统称)6 ,Mobile(移动端统称)7
    [DllImport("__Internal")]
    private static extern int GetEnvironmentPlatformFun();
    public static string GetEnvironmentPlatform()
    {
#if UNITY_EDITOR
        return "PC";
#else
        int p = GetEnvironmentPlatformFun();
        switch (p)
        {
            case 1: return "Ipad";
            case 2:return "Iphone";
            case 3:return "Android";
            case 4:return "WindowsCe";
            case 5:return "WindowsMobile";
            case 6:return "PC";
            case 7:return "Mobile";
        }
        return "Unknow";
#endif
    }

    //网络是否可用
    [DllImport("__Internal")]
    private static extern int GetNetworkAvailableFun();
    public static bool GetNetworkAvailable()
    {
#if UNITY_EDITOR
        return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork
               || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
#else
        return GetNetworkAvailableFun() == 1;
#endif
    }

    //app是否在前台
    [DllImport("__Internal")]
    private static extern int GetAppIsForegroundFun();
    public static bool GetAppIsForeground()
    {
#if UNITY_EDITOR
        return Application.isFocused || Application.isPlaying;
#else
        return GetAppIsForegroundFun() == 1;
#endif
    }

    [DllImport("__Internal")]
    private static extern void InitWindowCustomPropDefine();
    public static void SetWindowCustomPropDefine()
    {
#if !UNITY_EDITOR
        InitWindowCustomPropDefine();
#endif
    }

    //开启或隐藏系统状态拦 目前适用于Android
    [DllImport("__Internal")]
    private static extern void ChangeSystemStatusBarFun(bool isShow);
    public static void ChangeSystemStatusBar(bool isShow)
    {
#if !UNITY_EDITOR
          ChangeSystemStatusBarFun(isShow);
#endif
    }


}
