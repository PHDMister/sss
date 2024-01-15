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
                    case (int)LoadSceneType.RainbowBeach:
                        GoToRainbowBeach();
                        break;
                    case (int)LoadSceneType.ShenMiHaiWan:
                        GoToShenMiHaiWan();
                        break;
                    case (int)LoadSceneType.HaiDiXingKong:
                        GoToHaiDiXingKong();
                        break;
                }
                loadScene = false;
            }
        }
    }

    //初始化角色接口
    protected void InitPlayerFunc()
    {
        AvatarManager.Instance().RefreshPlayerFun();
        CharacterManager.Instance().PlayPlayerSpecialEffectFun();
        CharacterManager.Instance().SetFollow();
        PlayerCtrlManager.Instance().ResetMainCamera();
        PlayerCtrlManager.Instance().RemoveTouchEvent();
        PlayerCtrlManager.Instance().AddTouchEvent();
    }
    //彩虹沙滩
    protected void GoToRainbowBeach()
    {
        ManageMentClass.CurSyncPlayerController = Singleton<RainbowBeachController>.Instance;
        PlayerCtrlManager.Instance().Init();
        StartCoroutine(InitRainbowBeach());
    }
    //神秘海湾
    protected void GoToShenMiHaiWan()
    {
        ManageMentClass.CurSyncPlayerController = Singleton<RainbowIocnController>.Instance;
        PlayerCtrlManager.Instance().Init();
        StartCoroutine(InitShenMiHaiWan());
    }
    //海底星空
    protected void GoToHaiDiXingKong()
    {
        ManageMentClass.CurSyncPlayerController = Singleton<RainbowSeabedController>.Instance;
        PlayerCtrlManager.Instance().Init();
        StartCoroutine(InitSeabed());
    }



    protected IEnumerator InitRainbowBeach()
    {
        int IsRequestReady = 0;
        float timeout = 0;
        //请求房间数据
        MessageManager.GetInstance().SendRoomEnter((uint)LoadSceneType.RainbowBeach, data => IsRequestReady++);
        //请求服务器个人形象的数据
        MessageManager.GetInstance().RequestOutFitList((outFitData) =>
        {
            AvatarManager.Instance().ReceiveOutFitAvatarDataFun(outFitData);
            IsRequestReady++;
        });
        //个人信息
        MessageManager.GetInstance().RequestGetPersonData(() => { IsRequestReady++; });
        //gas
        MessageManager.GetInstance().RequestGasValue(() => IsRequestReady++);
        //贝壳
        MessageManager.GetInstance().RequestShellCount(shelldata =>
        {
            Singleton<BagMgr>.Instance.SetShellNum((uint)shelldata.amount);
            IsRequestReady++;
        });
        //背包
        MessageManager.GetInstance().SendGetBagItem(data => IsRequestReady++);
        //商店
        MessageManager.GetInstance().RequestShopData(list =>
        {
            Singleton<ShopMgr>.Instance.UpdateShops(list);
            IsRequestReady++;
        });
        //免费贝壳次数
        MessageManager.GetInstance().SendFreeShellCount(data => IsRequestReady++);
        //海洋博物馆
        MessageManager.GetInstance().SendAquariumReq(() => IsRequestReady++);
        while (IsRequestReady < 9)
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
        InitPlayerFunc();
        yield return null;
        Singleton<RainbowBeachSyncNetView>.Instance.EnterRoom();
        ChatMgr.Instance.ImInit();
        yield return new WaitForSeconds(0.5f);
        if (AudioMgr.Ins == null) ResourcesMgr.GetInstance().LoadAsset("Prefabs/AudioPrefab/AudioManager", false);
        if (AudioMgr.Ins != null) AudioMgr.Ins.UpdateAudioSource();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHMAINUI);
        yield return null;
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        //检测硬件网络是否可用
        MonoSingleton<NetworkStateMonitor>.instance.StartCheck();
    }


    /// <summary>
    /// 初始化神秘海底
    /// </summary>
    /// <returns></returns>
    protected IEnumerator InitShenMiHaiWan()
    {
        int IsRequestReady = 0;
        float timeout = 0;
        //请求房间数据
        MessageManager.GetInstance().SendRoomEnter((uint)LoadSceneType.ShenMiHaiWan, data => IsRequestReady++);
        //请求服务器个人形象的数据
        MessageManager.GetInstance().RequestOutFitList((outFitData) =>
        {
            AvatarManager.Instance().ReceiveOutFitAvatarDataFun(outFitData);
            IsRequestReady++;
        });
        //个人信息
        MessageManager.GetInstance().RequestGetPersonData(() => { IsRequestReady++; });
        //gas
        MessageManager.GetInstance().RequestGasValue(() => IsRequestReady++);
        //贝壳
        MessageManager.GetInstance().RequestShellCount(shelldata =>
        {
            Singleton<BagMgr>.Instance.SetShellNum((uint)shelldata.amount);
            IsRequestReady++;
        });
        //背包
        MessageManager.GetInstance().SendGetBagItem(data => IsRequestReady++);
        //商店
        MessageManager.GetInstance().RequestShopData(list =>
        {
            Singleton<ShopMgr>.Instance.UpdateShops(list);
            IsRequestReady++;
        });
        //免费贝壳次数
        MessageManager.GetInstance().SendFreeShellCount(data => IsRequestReady++);
        //海洋博物馆
        MessageManager.GetInstance().SendAquariumReq(() => IsRequestReady++);

        while (IsRequestReady < 9)
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
        InitPlayerFunc();
        yield return null;
        Singleton<RainbowIocnSyncNetView>.Instance.EnterRoom();
        ChatMgr.Instance.ImInit();
        yield return new WaitForSeconds(0.5f);
        if (AudioMgr.Ins == null) ResourcesMgr.GetInstance().LoadAsset("Prefabs/AudioPrefab/AudioManager", false);
        if (AudioMgr.Ins != null) AudioMgr.Ins.UpdateAudioSource();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHMAINUI);
        yield return null;
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        //检测硬件网络是否可用
        MonoSingleton<NetworkStateMonitor>.instance.StartCheck();
    }

    //海底星空
    protected IEnumerator InitSeabed()
    {
        int IsRequestReady = 0;
        float timeout = 0;
        //请求房间数据
        MessageManager.GetInstance().SendRoomEnter((uint)LoadSceneType.HaiDiXingKong, data => IsRequestReady++);
        //请求服务器个人形象的数据
        MessageManager.GetInstance().RequestOutFitList((outFitData) =>
        {
            AvatarManager.Instance().ReceiveOutFitAvatarDataFun(outFitData);
            IsRequestReady++;
        });
        //个人信息
        MessageManager.GetInstance().RequestGetPersonData(() => { IsRequestReady++; });
        //gas
        MessageManager.GetInstance().RequestGasValue(() => IsRequestReady++);
        //贝壳
        MessageManager.GetInstance().RequestShellCount(shelldata =>
        {
            Singleton<BagMgr>.Instance.SetShellNum((uint)shelldata.amount);
            IsRequestReady++;
        });
        //背包
        MessageManager.GetInstance().SendGetBagItem(data => IsRequestReady++);
        //商店
        MessageManager.GetInstance().RequestShopData(list =>
        {
            Singleton<ShopMgr>.Instance.UpdateShops(list);
            IsRequestReady++;
        });
        //免费贝壳次数
        MessageManager.GetInstance().SendFreeShellCount(data => IsRequestReady++);
        //海洋博物馆
        MessageManager.GetInstance().SendAquariumReq(() => IsRequestReady++);

        while (IsRequestReady < 9)
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
        InitPlayerFunc();
        yield return null;
        Singleton<RainbowSeabedSyncNetView>.Instance.EnterRoom();
        ChatMgr.Instance.ImInit();
        yield return new WaitForSeconds(0.5f);
        if (AudioMgr.Ins == null) ResourcesMgr.GetInstance().LoadAsset("Prefabs/AudioPrefab/AudioManager", false);
        if (AudioMgr.Ins != null) AudioMgr.Ins.UpdateAudioSource();
        UIManager.GetInstance().CloseAllShowPanel();
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHMAINUI);
        yield return null;
        GC.Collect();
        yield return Resources.UnloadUnusedAssets();
        //检测硬件网络是否可用
        MonoSingleton<NetworkStateMonitor>.instance.StartCheck();
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
