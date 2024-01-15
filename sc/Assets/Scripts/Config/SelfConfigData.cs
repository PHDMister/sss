using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelfConfigData
{
    //测试服使用数据  isLinkEdition&&isOfficialEdition ==false 时生效 （两个变量间具体作用，前往DataManager 类中查看）

    public static string selfNumber = "16688888887"; // 为11位手机号。 此手机号要加入白名单。  （加入白名单后，验证码固定为手机号的后四位， 登录验证码时通过截取此处的后四位拿取的 ）

    public static string testAppToken = ""; //AppToken （为空时，从接口获取。 不为空时，用此处的值）；

    public static string testLandId = ""; // 土地ID  （为空时，从接口获取。 不为空时，用此处的值） 

    public static int testSceneTypeID = 7; // 6=彩虹沙滩   7=神秘海湾   8=海底星空

    public static uint projectID = 1; // 0空间项目, 1沙滩项目
    //--------------------------------------------------------------配置额外数据（基本不更改）--------------------------------------------------------------------

    public static string AreaIDA = "8O";// 土地区域ID A区 （无需关注）

    public static string AreaIDB = "7O";// 土地区域ID B区 （无需关注）

    public static string testSpareLandID = "Qez"; //备用的土地ID （当testLandId 为空时，且这个账号没有土地时，会使用这个备用土地ID）



    public static int CarA = 20;
    public static int CarB = 10;
    public static int pos = 20;

    #region 模块化资源管理相关配置

#if UNITY_EDITOR
    public static bool UseStreamingAssetsBundle = true;
#endif
    public static string FirstRunModule = "";

    //本地web server测试地址
    public static string ModuleRootUrl = "http://10.0.7.31/space_cdn/StreamingAssets/ResModules/"; //jd
    //public static string ModuleRootUrl = ""; //tiger
    //public static string ModuleRootUrl = ""; //panada
    //public static string ModuleRootUrl = ""; //henry



    #endregion

}