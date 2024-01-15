/***
 * 
 *    Title: "UIFW" UI框架项目
 *           主题： 框架核心参数  
 *    Description: 
 *           功能：
 *           1： 系统常量
 *           2： 全局性方法。
 *           3： 系统枚举类型
 *           4： 委托定义
 *                          
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    #region 系统枚举类型

    /// <summary>
    /// UI窗体（位置）类型
    /// </summary>
    public enum UIFormType
    {
        //普通窗体
        Normal,   
        //固定窗体                              
        Fixed,
        //弹出窗体
        PopUp,
        //顶层 覆盖
        Top
    }

    /// <summary>
    /// UI窗体的显示类型
    /// </summary>
    public enum UIFormShowMode
    {
        //普通
        Normal,
        //反向切换
        ReverseChange,
        //隐藏其他
        HideOther
    }

    /// <summary>
    /// UI窗体透明度类型
    /// </summary>
    public enum UIFormLucenyType
    {
        //完全透明，不能穿透
        Lucency,
        //半透明，不能穿透
        Translucence,
        //低透明度，不能穿透
        ImPenetrable,
        //可以穿透
        Pentrate    
    }

    #endregion

    public class SysDefine : MonoBehaviour {
        /* 路径常量 */
        public const string SYS_PATH_CANVAS = "UIRes/UIPrefabs/Canvas";
        public const string SYS_PATH_UIFORMS_CONFIG_INFO = "UIRes/UIFormsConfigInfo";
        public const string SYS_PATH_UIFORMS = "UIRes/UIPrefabs";
        public const string SYS_PATH_CONFIG_INFO = "UIRes/SysConfigInfo";
        public const string SYS_PATH_LAUGUAGE_JSON_CONFIG = "UIRes/LauguageJSONConfig";
        public const string SYS_PATH_RT = "RT/RT";
        public const string SYS_PATH_MODEL = "Model/";
        public const string SYS_PATH_PETMODEL = "Dog/";
        public const string SYS_PATH_CHARACTER = "Character/";
        public const string SYS_PATH_GAMECOMPONENT = "GameComponent/";
        public const string SYS_PATH_BOXUI = "UIRes/UIPrefabs/BoxUI";
        public const string SYS_PATH_PETUI = "UIRes/UIPrefabs/PetUI";
        public const string SYS_PATH_FECESUI = "UIRes/UIPrefabs/FecesUI";
        public const string SYS_PATH_TREASUREHUNTTOASTUI = "UIRes/UIPrefabs/TreasureHuntToastUI";
        public const string SYS_PATH_NOTICE = "UIRes/UIPrefabs/NoticePanel";


        /* 标签常量 */
        public const string SYS_TAG_CANVAS = "_TagCanvas";
        public const string SYS_TAG_RT = "_TagRT";
        /* 节点常量 */
        public const string SYS_NORMAL_NODE = "Normal";
        public const string SYS_FIXED_NODE = "Fixed";
        public const string SYS_POPUP_NODE = "PopUp";
        public const string SYS_TOP_NODE = "Top";
        public const string SYS_SCRIPTMANAGER_NODE = "_ScriptMgr";
        public const string SYS_MODEL_NODE = "Model";
        public const string SYS_CAMERA_NODE = "Camera";

        /* 遮罩管理器中，透明度常量 */
        public const float SYS_UIMASK_LUCENCY_COLOR_RGB = 255 / 255F;
        public const float SYS_UIMASK_LUCENCY_COLOR_RGB_A = 0F / 255F;

        public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB = 220 / 255F;
        public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A = 50F / 255F;

        public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB = 50 / 255F;
        public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A = 200F / 255F;

        /* 摄像机层深的常量 */

        /* 全局性的方法 */
        //Todo...

        /* 委托的定义 */
        //Todo....

    }
}