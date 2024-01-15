using System;
using UIFW;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.SceneManagement;
using Treasure;
using System.Linq;
using Base;

public class SceneLoadManager : MonoBehaviour
{
    private Transform _TransSceneMgr = null;
    private static SceneLoadManager _instance = null;
    private AsyncOperation asyncOperation = null;
    private int displayProgress = 0;
    private int toProgress = 0;
    private bool loadScene = false;
    private int m_SceneLevel = 0;
    private bool bInit = false;
    private string BeforeLoadSceneName = "";
    private string configSceneName = "";

    public static SceneLoadManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_SceneManager").AddComponent<SceneLoadManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransSceneMgr = this.gameObject.transform;
        DontDestroyOnLoad(_TransSceneMgr);
    }
    public void Load(int level)
    {
        loadScene = false;
        displayProgress = 0;
        toProgress = 0;
        m_SceneLevel = level;
        ManageMentClass.DataManagerClass.SceneID = level;
        //ModuleMgr.GetInstance().InitModule(level);
        LoadSceneWhenModuleReady();
    }

    /// <summary>
    /// load母包内的老场景
    /// </summary>
    public void LoadSceneWhenModuleReady()
    {
        StartCoroutine(LoadScene());
    }


    public void LoadBundleSceneWhenModuleReady(string moduleName, string configSceneName)
    {
        StartCoroutine(LoadBundleScene(moduleName, configSceneName));
    }


    IEnumerator LoadScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync(m_SceneLevel);
        asyncOperation.allowSceneActivation = false;
        loadScene = true;
        UnLoadSceneFun();
        yield return asyncOperation;
    }
    IEnumerator LoadBundleScene(string moduleName, string configSceneName)
    {
        this.configSceneName = configSceneName;
        //asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncOperation = SceneManager.LoadSceneAsync(configSceneName);
        asyncOperation.allowSceneActivation = false;
        loadScene = true;
        UnLoadSceneFun();
        yield return asyncOperation;
    }


    private void FixedUpdate()
    {
        if (loadScene)
        {
            while (asyncOperation.progress < 0.9f)
            {
                toProgress = (int)(asyncOperation.progress * 100);
                while (displayProgress < toProgress)
                {
                    ++displayProgress;
                    SetProgress(displayProgress);
                    return;
                }
            }
            toProgress = 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetProgress(displayProgress);
                return;
            }
            asyncOperation.allowSceneActivation = true;
            if (asyncOperation.isDone)
            {
                //UIManager.GetInstance().CloseUIForms(FormConst.PETDENSLOADING);

                switch (m_SceneLevel)
                {
                    case (int)LoadSceneType.parlorScene:
                        GotoSpace();
                        break;
                    case (int)LoadSceneType.dogScene:
                        GotoPetdens();
                        break;
                    case (int)LoadSceneType.ShelterScene:
                        GotoAidStation();
                        break;
                    case (int)LoadSceneType.TreasureDigging:
                        GotoTreasureDigging();
                        break;
                    case (int)LoadSceneType.BedRoom:
                        GotoBedRoom();
                        break;
                    case (int)LoadSceneType.ModuleTest1:
                        GoModuleTest1();
                        break;
                    case (int)LoadSceneType.ModuleTest2:
                        GoModuleTest2();
                        break;
                    default:
                        break;
                }
                loadScene = false;
            }
        }
    }

    //test
    public void GoModuleTest1()
    {
        Camera camera = Camera.main;
        camera.gameObject.AddComponent<Module1MainScene>();
    }

    //test
    public void GoModuleTest2()
    {
        Camera camera = Camera.main;
        camera.gameObject.AddComponent<Module2MainScene>();
    }
    /// <summary>
    /// 前往宠物小窝
    /// </summary>
    public void GotoPetdens()
    {
        ChangeRoomManager.Instance().InitRoom();

        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENS_UIFORM);
        PetdensUIForm petdensUIForm = UIManager.GetInstance().GetUIForm(FormConst.PETDENS_UIFORM) as PetdensUIForm;
        petdensUIForm.InitFun();

        try
        {
            Vector3 newPos = new Vector3(-0.15f, 0f, -5.82f);
            CharacterManager.Instance().SetCharacterPos(newPos);
            CharacterManager.Instance().SetFollow();
            PlayerCtrlManager.Instance().ResetMainCamera();
            PlayerCtrlManager.Instance().RemoveTouchEvent();
            PlayerCtrlManager.Instance().AddTouchEvent();
            TransferEffectManager.Instance().Init();
            TransferEffectManager.Instance().SetMaterialProperty(1f);
            TransferEffectManager.Instance().bTransferEnd = true;
            TransferEffectManager._timeElapsed = 0f;
            CharacterManager.Instance().SetPetdensCinemachineLook();
        }
        catch (System.Exception e)
        {
            Debug.Log("Êä³öÒ»ÏÂ±¨´íÎ»ÖÃµÄÄÚÈÝ£º " + e.Message);
        }
    }

    /// <summary>
    /// 前往个人空间
    /// </summary>
    public void GotoSpace()
    {
        StartCoroutine(WaitSpaceRequest());
    }

    private IEnumerator WaitSpaceRequest()
    {
        if (!bInit)//初始化进来
        {
            PlayerCtrlManager.Instance().Init();
            TransferEffectManager.Instance().Init();
            MessageManager.GetInstance().RequestOutFitList((outFitData) =>
            {
                //接收服务器个人形象的数据
                AvatarManager.Instance().ReceiveOutFitAvatarDataFun(outFitData);

                //设置起始位置
                Vector3 newPos = new Vector3(-5.422f, 0, 5.61f);
                Quaternion newQuaternion = Quaternion.Euler(0f, 180f, 0f);
                Singleton<ParlorController>.Instance.SavePosRota(newPos, newQuaternion);

                //初始化角色接口
                AvatarManager.Instance().RefreshPlayerFun();
                CharacterManager.Instance().PlayPlayerSpecialEffectFun();
            });
            bInit = true;
        }
        else//跳转过来
        {
            Vector3 newPos;
            Quaternion newQuaternion;
            if (BeforeLoadSceneName.ToLower().Contains("gerenkongjian"))
            {
                //从客厅回来
                newPos = new Vector3(-5.422f, 0, 5.61f);
                newQuaternion = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (BeforeLoadSceneName.ToLower().Contains("bedroom"))
            {
                newPos = new Vector3(-5, 0f, 1);
                newQuaternion = Quaternion.Euler(0f, 90f, 0f);
            }
            else if (BeforeLoadSceneName.ToLower().Contains("dogroom"))
            {
                newPos = new Vector3(4, 0, -5);
                newQuaternion = Quaternion.Euler(0f, -90f, 0f);
            }
            else
            {
                newPos = new Vector3(5.875f, 0f, -5.917f);
                newQuaternion = Quaternion.Euler(0f, 0f, 0f);
            }
            Singleton<ParlorController>.Instance.SavePosRota(newPos, newQuaternion);
            CharacterManager.Instance().SetCharacterPosAndRotation(newPos, newQuaternion);
            CharacterManager.Instance().SetFollow();
            PlayerCtrlManager.Instance().ResetMainCamera();
            PlayerCtrlManager.Instance().RemoveTouchEvent();
            PlayerCtrlManager.Instance().AddTouchEvent();
            TransferEffectManager.Instance().Init();
            TransferEffectManager.Instance().SetMaterialProperty(1f);
            TransferEffectManager.Instance().bTransferEnd = true;
            TransferEffectManager._timeElapsed = 0f;
            TransferEffectManager.Instance().bTransferSpace = true;
        }


        #region 进入客厅
        int IsRequestReady = 0;
        int times = 200;
        EnterTreasureResp enterTreasureResp = null;
        EnterTreasureReq enterTreasureReq = new EnterTreasureReq();
        enterTreasureReq.UserId = ManageMentClass.DataManagerClass.userId;
        enterTreasureReq.FromUserId = ManageMentClass.DataManagerClass.InviteFromUserId;
        enterTreasureReq.LandId = ManageMentClass.DataManagerClass.landId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.EnterTreasureReq, enterTreasureReq, (code, bytes) =>
        {
            enterTreasureResp = EnterTreasureResp.Parser.ParseFrom(bytes);
            ManageMentClass.DataManagerClass.roomId = enterTreasureResp.RoomInfo.RoomId;
            ManageMentClass.DataManagerClass.YXAccid = enterTreasureResp.YunxinAccid;
            ManageMentClass.DataManagerClass.YXToken = enterTreasureResp.YunxinToken;
            ManageMentClass.DataManagerClass.ChatRoomId = enterTreasureResp.RoomInfo.ChatRoomId;
            ManageMentClass.DataManagerClass.ChatRoomAddr = enterTreasureResp.RoomInfo.ChatRoomAddr.ToArray();
            foreach (var player in enterTreasureResp.RoomInfo.UserList)
            {
                Debug.Log("WaitTimeEnterTreasure    EnterTreasureResp  player userId: " + player.YunxinAccid + "    name: " + player.UserName);
            }
            IsRequestReady++;
        });

        //个人信息
        MessageManager.GetInstance().RequestGetPersonData(() => { IsRequestReady++; });

        while (times > 0 && IsRequestReady < 2)
        {
            times--;
            yield return null;
        }
        Singleton<ParlorSyncNetView>.Instance.EnterRoom(enterTreasureResp);
        yield return null;
        if (bInit) CharacterManager.Instance().SetSpaceCinemachineLook();
        yield return null;
        #endregion
        ChatMgr.Instance.ImInit();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RIGHT_BAR_UIFORM);
        yield return null;
        MonoSingleton<NetworkStateMonitor>.instance.StartCheck();
    }


    /// <summary>
    /// 前往救助站
    /// </summary>
    public void GotoAidStation()
    {
        ChangeRoomManager.Instance().InitRoom();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RESCUESTATION);
        RescueStationUIForm rescueStationUIForm = UIManager.GetInstance().GetUIForm(FormConst.RESCUESTATION) as RescueStationUIForm;
        rescueStationUIForm.InitFun();
    }

    /// <summary>
    ///前往寻宝乐园
    /// </summary>
    public void GotoTreasureDigging()
    {
        PlayerCtrlManager.Instance().Init();
        StartCoroutine(WaitTimeEnterTreasure());
    }

    private IEnumerator WaitTimeEnterTreasure()
    {
        //请求房间数据
        int IsRequestReady = 0;
        float timeout = 0;
        EnterTreasureReq enterTreasureReq = new EnterTreasureReq();
        enterTreasureReq.UserId = ManageMentClass.DataManagerClass.userId;
        enterTreasureReq.FromUserId = ManageMentClass.DataManagerClass.InviteFromUserId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.EnterTreasureReq, enterTreasureReq, (code, bytes) =>
        {
            EnterTreasureResp enterTreasureResp = EnterTreasureResp.Parser.ParseFrom(bytes);
            ManageMentClass.DataManagerClass.roomId = enterTreasureResp.RoomInfo.RoomId;

            ManageMentClass.DataManagerClass.YXAccid = enterTreasureResp.YunxinAccid;
            ManageMentClass.DataManagerClass.YXToken = enterTreasureResp.YunxinToken;
            ManageMentClass.DataManagerClass.ChatRoomId = enterTreasureResp.RoomInfo.ChatRoomId;
            ManageMentClass.DataManagerClass.ChatRoomAddr = enterTreasureResp.RoomInfo.ChatRoomAddr.ToArray();

            TreasureModel.Instance.RoomData = enterTreasureResp.RoomInfo;
            TreasureModel.Instance.UserList = enterTreasureResp.RoomInfo.UserList;
            foreach (var player in enterTreasureResp.RoomInfo.UserList)
            {
                Debug.Log("WaitTimeEnterTreasure    EnterTreasureResp  player userId: " + player.YunxinAccid + "    name: " + player.UserName);
            }

            IsRequestReady++;
        });

        MessageManager.GetInstance().RequestOutFitList((outFitData) =>
        {
            //接收服务器个人形象的数据
            AvatarManager.Instance().ReceiveOutFitAvatarDataFun(outFitData);
            IsRequestReady++;
        });

        //请求队伍信息
        MessageManager.GetInstance().RequestTeamInfo((teamInfoResp) =>
        {
            TreasureModel.Instance.TeamUserList = teamInfoResp.List.ToList();
            IsRequestReady++;
        });


        //请求寻宝活动开启时间
        MessageManager.GetInstance().RequestOpenTime(() => { IsRequestReady++; });

        //个人信息
        MessageManager.GetInstance().RequestGetPersonData(() => { IsRequestReady++; });

        while (IsRequestReady < 5)
        {
            timeout += Time.deltaTime;
            if (timeout >= 30)
            {
                ToastManager.Instance.ShowNewToast("连接服务器获取数据失败,请重新进入", 10);
                yield break;
            }
            yield return null;
        }
        //初始化角色接口
        AvatarManager.Instance().RefreshPlayerFun();
        CharacterManager.Instance().PlayPlayerSpecialEffectFun();
        CharacterManager.Instance().SetFollow();
        PlayerCtrlManager.Instance().ResetMainCamera();
        PlayerCtrlManager.Instance().RemoveTouchEvent();
        PlayerCtrlManager.Instance().AddTouchEvent();
        yield return null;
        Singleton<RoomSyncNetView>.Instance.EnterRoom();
        yield return null;
        ChatMgr.Instance.ImInit();
        TreasureStartTimer.Instance().Init();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.TREASUREDIGGINGMAINMENU);
        //检测硬件网络是否可用 暂时注释 等APP端接口更新
        yield return null;
        MonoSingleton<NetworkStateMonitor>.instance.StartCheck();
    }

    /// <summary>
    /// 进入卧室
    /// </summary>
    public void GotoBedRoom()
    {
        Vector3 newPos = new Vector3(-8f, 0, 4f);
        Quaternion newQuaternion = Quaternion.Euler(0f, 90f, 0f);
        CharacterManager.Instance().SetCharacterPosAndRotation(newPos, newQuaternion);

        //初始化角色接口
        AvatarManager.Instance().RefreshPlayerFun();
        CharacterManager.Instance().PlayPlayerSpecialEffectFun();
        CharacterManager.Instance().SetFollow();
        PlayerCtrlManager.Instance().ResetMainCamera();
        PlayerCtrlManager.Instance().RemoveTouchEvent();
        PlayerCtrlManager.Instance().AddTouchEvent();

        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RIGHT_BAR_UIFORM);
    }



    public void InitSceneEditorFun()
    {
        CharacterManager.Instance().SetFollow();
        PlayerCtrlManager.Instance().ResetMainCamera();
        PlayerCtrlManager.Instance().RemoveTouchEvent();
        PlayerCtrlManager.Instance().AddTouchEvent();
        CharacterManager.Instance().SetSpaceCinemachineLook();
        HUDManager.Instance().HidePetEntry();
    }

    public void SetProgress(int value)
    {
        PetdensLoading m_PetdensLoading = UIManager.GetInstance().GetUIForm(FormConst.PETDENSLOADING) as PetdensLoading;
        if (m_PetdensLoading != null)
        {
            m_PetdensLoading.SetProgress(value);
        }
    }

    public void UnLoadSceneFun()
    {
        BeforeLoadSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("BeforeLoadSceneName: " + BeforeLoadSceneName);
        if (BeforeLoadSceneName == "dogroom01")
        {
            ChangeRoomManager.Instance().UnLoadAllRoomFun();
        }
    }

    #region app前后台状态变更接口
#if UNITY_EDITOR
    /// <summary>
    /// 经过测试，webgl版本只能在pc浏览器里触发；手机浏览器以及app里不生效
    /// 所以仅用于模拟app进入后台/恢复到前台时的状态变化,正式环境走的是index.html里的的visibilitychange事件
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            OnForeground();
        }
        else
        {
            OnBackground();
        }
    }
#endif

    public void OnBackground()
    {
        Debug.Log("_SceneManager/ OnBackground >>>>>>>");
    }

    public void OnForeground()
    {
        Debug.Log("_SceneManager/ OnForeground <<<<<<<<");

    }
    #endregion

}
