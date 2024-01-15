using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fight;
using System;

public class LoginLoadingPanel : BaseUIForm
{
    //ÍË³ö°´Å¥
    Text versionsNumText;
    public Text Tip_Text;
    public Text infoText;
    public Text processText;


    private void Awake()
    {
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif

        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
        if (ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            Debug.unityLogger.logEnabled = false;
        }

    }
    public override void Display()
    {
        base.Display();

        Tip_Text.text = "正在登录...";
        Debug.Log("1111111111111  TestVersionNumber=" + ManageMentClass.DataManagerClass.TestVersionNumber);

        InterfaceHelper.SetJoyStickState(true);
        GetPlatformTypeFun();
        GetLocalUrlData();
        SetBeginUIFun();
        //ÉèÖÃ²âÊÔ·þ°æ±¾ºÅ
        SetVersionNumFun();
        //Ìí¼Ó½øÈëÍÚ±¦µÄÊÂ¼þ¼àÌý
        ReceiveMessage(WebSocketConst.WsNet_OnFirstOpen, OnWsNetFirstOpen);
    }

    //³¤Á´½Ó¿ªÊ¼Á´½Ó·þÎñÆ÷
    void WebSocketConnect()
    {
        bool isLinkEdititon = ManageMentClass.DataManagerClass.isLinkEdition;
        bool isOfficial = ManageMentClass.DataManagerClass.isOfficialEdition;
        if (isLinkEdititon && isOfficial)
        {
            //正式服
            WebSocketAgent.Ins.Connect(new WebSocketReleaseConfig());
        }
        else
        {
            Debug.Log("在这里查看一下链接得事件内容：  发送链接");
            //测试服
            WebSocketAgent.Ins.Connect(new WebSocketConfig());
        }
    }

    void OnWsNetFirstOpen(KeyValuesUpdate kv)
    {
        RemoveMsgListener(WebSocketConst.WsNet_OnFirstOpen, OnWsNetFirstOpen);
        //注册请求
        RegisterReq registerReq = new RegisterReq();
        registerReq.Token = ManageMentClass.DataManagerClass.tokenValue_Game;
        registerReq.SceneId = (uint)ManageMentClass.DataManagerClass.SceneID;
        Debug.Log("输出一下具体得值： " + ManageMentClass.DataManagerClass.SceneID);
        Debug.Log("在这里查看一下链接得事件内容：  发送注册");
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.Register, registerReq, (code, bytes) =>
        {
            Debug.Log($"WebSocketConnect  RegisterResp code:{code}");
            if (code != 0) return;
            RegisterResp registerResp = RegisterResp.Parser.ParseFrom(bytes);
            Debug.Log("输出一下具体的返回值“ " + registerResp.ToJSON());
            if (registerResp.StatusCode == 0)
            {
                ManageMentClass.DataManagerClass.ShellCount = (int)registerResp.BkCount;
                ManageMentClass.DataManagerClass.userId = registerResp.UserId;
                WebSocketAgent.Ins.SendHeartBeat();
                TreasureModel.Instance.AddLoginEvent();
            }
        });
    }

    /// <summary>
    /// 获取本地URL代码
    /// </summary>
    void GetLocalUrlData()
    {

        string UrlMsg = "";

        bool isTrue = false;
        if (ManageMentClass.DataManagerClass.isOfficialEdition || ManageMentClass.DataManagerClass.isLinkEdition)
        {
            try
            {
                UrlMsg = SetTools.StringReturnValueFunction();
                isTrue = true;
            }
            catch (System.Exception e)
            {
                UrlMsg = "[catch]" + e.Message;
            }
            //  isTrue = true;
            if (isTrue)
            {
                //  UrlMsg = "https://24nyae.cn/PC_Test/index.html?screen-orientation=0&hide_navigate=1&hide_status=1#SceneType@2&version@3.1.2&t@MTIzNDc4OTAwOTg3NDMyMVpkcmtFalVQaGZsYTA5UWRhTWRUc1VvRUZ0bDQxazRQNHVRNGU3WExka3RtK0ZuMG5QcWh0c2JhU1VjVzFjZEQ=&landID@Qez#p";
                // Debug.Log("这里的内容asdfsdf");
                //
                string[] a = UrlMsg.Split('#');
                Debug.Log("a的长度：  " + a.Length + "  UrlMsg " + UrlMsg);


                if (a.Length >= 3)
                {

                    ManageMentClass.DataManagerClass.WebInto = true;

                    //从网页端点击进入
                    string[] b1 = a[1].Split('&');
                    for (int i = 0; i < b1.Length; i++)
                    {
                        string[] c = b1[i].Split('@');
                        Debug.Log("C 0:    " + c[0]);
                        if (c[0] == "t")
                        {
                            //   Debug.Log("c[1]: " + c[1]);
                            //解码
                            string strA = PasswordTools.Base64_Decode(c[1]);
                            //  Debug.Log(" 解码： " + strA);
                            //截取
                            string strB = strA.Substring(16);
                            //  Debug.Log("截取：" + strB);
                            //解密

                            string strC = PasswordTools.DecryptString(strB, "12345678876543218765432112345678", "1234789009874321");
                            /* string str = testAppToken.Substring(2);
                             Debug.Log("输出内容token的值： " + str);*/
                            // Debug.Log("解密：" + strC);

                            ManageMentClass.DataManagerClass.tokenValue_App = strC;
                        }
                        else if (c[0] == "landID")
                        {
                            ManageMentClass.DataManagerClass.landId = c[1];
                        }
                        else if (c[0] == "version")
                        {
                            ManageMentClass.DataManagerClass.VersionsNumber = c[1];
                        }
                        else if (c[0] == "SceneType")
                        {
                            ManageMentClass.DataManagerClass.SceneID = int.Parse(c[1]);
                        }
                    }
                }
                else
                {
                    string[] b2 = a[1].Split('&');
                    for (int i = 0; i < b2.Length; i++)
                    {
                        string[] c = b2[i].Split('@');
                        if (c[0] == "t")
                        {
                            ManageMentClass.DataManagerClass.tokenValue_App = c[1];
                            Debug.Log("token:" + c[1]);
                        }
                        else if (c[0] == "landID")
                        {
                            Debug.Log("landID:" + c[1]);
                            ManageMentClass.DataManagerClass.landId = c[1];
                        }
                        else if (c[0] == "version")
                        {
                            Debug.Log("version:" + c[1]);
                            ManageMentClass.DataManagerClass.VersionsNumber = c[1];
                        }
                        else if (c[0] == "SceneType")
                        {
                            Debug.Log("SceneType:" + c[1]);
                            ManageMentClass.DataManagerClass.SceneID = int.Parse(c[1]);
                        }
                        else if (c[0] == "invite")
                        {
                            ManageMentClass.DataManagerClass.bInviteFromApp = int.Parse(c[1]) == 1;
                            Debug.Log("bInviteFromApp:" + ManageMentClass.DataManagerClass.bInviteFromApp);
                        }
                        else if (c[0] == "user_id")
                        {
                            ManageMentClass.DataManagerClass.InviteFromUserId = ulong.Parse(c[1]);
                            Debug.Log("InviteFromUserId:" + ManageMentClass.DataManagerClass.InviteFromUserId);
                        }
                    }
                    if (ManageMentClass.DataManagerClass.bInviteFromApp)
                    {
                        for (int i = 0; i < b2.Length; i++)
                        {
                            string[] c = b2[i].Split('=');
                            if (c[0] == "token")
                            {
                                ManageMentClass.DataManagerClass.tokenValue_App = c[1];
                            }
                        }
                    }
                }

                LoginFun();
            }
            else
            {
                Debug.Log("没有获取到UrlMsg");
            }
        }
        else
        {
            ManageMentClass.DataManagerClass.SceneID = SelfConfigData.testSceneTypeID;

            //ÉèÖÃappToken
            if (SelfConfigData.testAppToken == "")
            {
                if (string.IsNullOrEmpty(SelfConfigData.selfNumber))
                {
                    Debug.LogError("需要预设测试手机号！");
                    return;
                }
                GetTokenData getTokenData = new GetTokenData(SelfConfigData.selfNumber, SelfConfigData.selfNumber.Substring(7, 4), "0", "D38AC10C-A844-41D7-929B-0DA230263B0F", "", "mobile");
                MessageManager.GetInstance().GetTokenFun(getTokenData, () =>
                {
                    Debug.Log($"输出tokenApp的内容值：  { ManageMentClass.DataManagerClass.tokenValue_App} </color>");
                    LoginFun();
                });
            }
            else
            {
                ManageMentClass.DataManagerClass.tokenValue_App = SelfConfigData.testAppToken;
                LoginFun();
            }
        }
    }

    /// <summary>
    /// µÇÂ¼
    /// </summary>
    void LoginFun()
    {
        MessageManager.GetInstance().LoginHDFun(() =>
        {
            if (ManageMentClass.DataManagerClass.isLinkEdition || ManageMentClass.DataManagerClass.isOfficialEdition)
            {
                //µÇÂ¼³É¹¦µÄ·½·¨

                //ÅÐ¶ÏÈ¥ÄÄ¸ö³¡¾°
                SelectToWhereSceneFun();

                //³¤Á´½ÓµÇÂ¼
                WebSocketConnect();
            }
            else
            {
                if (SelfConfigData.testLandId == "")
                {
                    GetLandIDActionFun(() =>
                    {
                        Debug.Log($"<color=red>输出一下获取道德LandID：  {ManageMentClass.DataManagerClass.landId} </color>");
                        //µÇÂ¼³É¹¦µÄ·½·¨
                        //ÅÐ¶ÏÈ¥ÄÄ¸ö³¡¾°
                        SelectToWhereSceneFun();

                        //³¤Á´½ÓµÇÂ¼
                        WebSocketConnect();

                    });
                }
                else
                {
                    ManageMentClass.DataManagerClass.landId = SelfConfigData.testLandId;
                    SelectToWhereSceneFun();
                    //³¤Á´½ÓµÇÂ¼
                    WebSocketConnect();
                }
            }

        }, () =>
        {
            //µÇÂ¼Ê§°ÜµÄ·½·¨
            //ÌáÊ¾
            Tip_Text.text = "登录失败";

        });
    }

    /// <summary>
    /// 设置初始UI
    /// </summary>
    void SetBeginUIFun()
    {
        versionsNumText = GameObject.Find("VersionsNum_Text").GetComponent<Text>();
    }
    /// <summary>
    /// 设置版本号显示
    /// </summary>
    void SetVersionNumFun()
    {
        if (versionsNumText != null)
        {
            versionsNumText.gameObject.SetActive(true);
            if (ManageMentClass.DataManagerClass.isOfficialEdition)
            {
                versionsNumText.text = "V：" + ManageMentClass.DataManagerClass.OfficialVersionNumber;
            }
            else
            {
                versionsNumText.text = "V：" + ManageMentClass.DataManagerClass.TestVersionNumber;
            }
        }
    }
    /// <summary>
    /// 获取网页 运行的平台
    /// </summary>
    void GetPlatformTypeFun()
    {
        try
        {
            // 1 = PC端  ， 2 = 移动端
            ManageMentClass.DataManagerClass.PlatformType = SetTools.GetOperationPlatformFun();
        }
        catch (System.Exception e)
        {
            ManageMentClass.DataManagerClass.PlatformType = 1;
            Debug.Log("e : " + e.Message);
        }
    }




    /// <summary>
    /// 跳转到具体哪个场景里面去
    /// </summary>
    void SelectToWhereSceneFun()
    {
        //CheckModuleBundleRes(ManageMentClass.DataManagerClass.SceneID);
        //return;

        GoToDoubleRacing();

        return;
        Debug.Log("SelectToWhereSceneFun    sceneId:" + ManageMentClass.DataManagerClass.SceneID);
        switch ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID)
        {
            case LoadSceneType.parlorScene:
                //客厅
                //ToParlorSceneFun();
                break;
            case LoadSceneType.dogScene:
                //狗窝

                //ToDogSceneFun();
                break;
            case LoadSceneType.ShelterScene:
                //救助站
                //ToShelterSceneFun();
                break;
            case LoadSceneType.TreasureDigging:
                //GoToTreasureDigging();
                break;
            case LoadSceneType.RainbowBeach:
                GoToRainbowBeach();
                break;
            case LoadSceneType.ShenMiHaiWan:
                GoToShenMiHaiWan();
                break;
            case LoadSceneType.HaiDiXingKong:
                GoToHaiDiXingKong();
                break;

            case LoadSceneType.ModuleTest1://test
                ToModuleTest1SceneFun();
                break;
            case LoadSceneType.ModuleTest2://test
                ToModuleTest2SceneFun();
                break;
            default:
                //其他

                break;
        }
    }

    void ToModuleTest1SceneFun()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.ModuleTest1);
        CloseUIForm();
    }

    void ToModuleTest2SceneFun()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.ModuleTest2);
        CloseUIForm();
    }

    void GetLandIDActionFun(Action CallBack)
    {
        if (SelfConfigData.testLandId == "")
        {
            MessageManager.GetInstance().GetLandIDListFun(SelfConfigData.AreaIDA, (jo) =>
             {
                 if (jo["data"]["my_land_list"].ToString() == "")
                 {
                     MessageManager.GetInstance().GetLandIDListFun(SelfConfigData.AreaIDB, (jo) =>
                     {
                         if (jo["data"]["my_land_list"].ToString() == "")
                         {
                             //Ê¹ÓÃ±¸ÓÃµÄÍÁµØID
                             ManageMentClass.DataManagerClass.landId = SelfConfigData.testSpareLandID;

                             CallBack?.Invoke();
                             Debug.Log("这里的值的LandIDº" + ManageMentClass.DataManagerClass.landId);
                         }
                         else
                         {
                             foreach (var item in jo["data"]["my_land_list"])
                             {
                                 if (item["id"].ToString() != "")
                                 {
                                     SelfConfigData.testLandId = item["id"].ToString();
                                     break;
                                 }
                             }
                             if (SelfConfigData.testLandId == "")
                             {
                                 ManageMentClass.DataManagerClass.landId = SelfConfigData.testSpareLandID;

                                 Debug.Log("这里的值的LandID" + ManageMentClass.DataManagerClass.landId);
                             }
                             else
                             {
                                 ManageMentClass.DataManagerClass.landId = SelfConfigData.testLandId;
                             }
                             CallBack?.Invoke();
                         }
                     });
                 }
                 else
                 {
                     foreach (var item in jo["data"]["my_land_list"])
                     {
                         if (item["id"].ToString() != "")
                         {
                             SelfConfigData.testLandId = item["id"].ToString();
                             break;
                         }
                     }
                     if (SelfConfigData.testLandId == "")
                     {
                         //Ê¹ÓÃ±¸ÓÃµÄÍÁµØID
                         ManageMentClass.DataManagerClass.landId = SelfConfigData.testSpareLandID;

                         Debug.Log("这里的值的LandID" + ManageMentClass.DataManagerClass.landId);
                     }
                     else
                     {
                         ManageMentClass.DataManagerClass.landId = SelfConfigData.testLandId;
                     }
                     CallBack?.Invoke();

                 }
             });
        }
        else
        {
            ManageMentClass.DataManagerClass.landId = SelfConfigData.testLandId;
            CallBack?.Invoke();
        }
    }




    void GoToRainbowBeach()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.RainbowBeach);
        CloseUIForm();
    }
    void GoToShenMiHaiWan()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.ShenMiHaiWan);
        CloseUIForm();
    }
    void GoToHaiDiXingKong()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.HaiDiXingKong);
        CloseUIForm();
    }

    /// <summary>
    /// 前往赛车场景
    /// </summary>
    void GoToDoubleRacing()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SendMessage("TransferLoading", "SceneTransfer", 1);
        CloseUIForm();
    }

}
