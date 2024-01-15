using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UIFW;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ModuleMgr: MonoBehaviour
{

    /// <summary>
    /// 外网测试
    /// </summary>
    public const string ModuleRootUrl = "https://24nyae.cn/Space_Test/StreamingAssets/ResModules/";
    /// <summary>
    /// 外网正式
    /// </summary>
    public const string ModuleRootUrl_Offical = "https://www.hotdogapp.cn/Space_Official/StreamingAssets/ResModules/";


    private readonly Dictionary<int, string> ModuleNameList = new Dictionary<int, string> {
        { (int)LoadSceneType.parlorScene, "parlor" },
        { (int)LoadSceneType.dogScene, "dog" },
        { (int)LoadSceneType.ShelterScene, "shelter" },
        { (int)LoadSceneType.TreasureDigging, "treasure" },
        { (int) LoadSceneType.BedRoom, "bedroom" },



        //测试用
        { (int)LoadSceneType.ModuleTest1, "module1" },
        { (int)LoadSceneType.ModuleTest2, "module2" }

    };

    private static ModuleMgr _Instance;
    private const string ModuleConfigFileName = "config.json";
    private const string ModuleRootDirInStreamingAsset = "ResModules"; //考虑到space工程当前的StreamingAssets目录已经存在一些资源，所以单独建立一个目录用来存放所有开发环境下测试用的bundle目录


    private string moduleRoolUrl;
    private string currentRunningBusinessModule;
    private int loadIndex;
    private int loadCount;
    private Dictionary<string, ModuleItem> moduleDict;

    private Text loadingProcessTxt;
    private Text loadingInfoTxt;

    //public CommonLoadingScene CommonLoadingScene;
    private bool readInStreamingAssets = false;

    public static ModuleMgr GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_ModuleMgr").AddComponent<ModuleMgr>();
        }
        return _Instance;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
#if UNITY_EDITOR
        readInStreamingAssets = SelfConfigData.UseStreamingAssetsBundle;
#endif

        moduleDict = new Dictionary<string, ModuleItem>();
        if (ManageMentClass.DataManagerClass.isLinkEdition)
        {
            if (ManageMentClass.DataManagerClass.isOfficialEdition)
            {
                moduleRoolUrl = ModuleRootUrl_Offical;
            }
            else
            {
                moduleRoolUrl = ModuleRootUrl;
            }
        }
        else
        {
            moduleRoolUrl = SelfConfigData.ModuleRootUrl;
        }
    }

    private void OnDestroy()
    {
        foreach (var moduleItem in moduleDict)
        {
            foreach (BundleReader item in moduleItem.Value.assetBundles)
            {
                if (item.bundle != null) item.bundle.Unload(true);
                if (item.stream != null)
                {
                    item.stream.Close();
                    item.stream.Dispose();
                }
            }
        }
        Debug.Log($"ModuleMgr/ OnDestroy 销毁所有bundle句柄");
    }

    #region 模块切换时Shader相关临时处理

    //public static void SetDefaultShader(string shader = "Universal Render Pipeline/Lit")
    public void SetDefaultShader()
    {
        //editor下会出现材质shader丢失问题
#if UNITY_EDITOR
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        MeshRenderer render = null;
        //Shader urpLitShader = Shader.Find(shader);
        foreach (GameObject item in objs)
        {
            render = item.GetComponent<MeshRenderer>();
            if (render != null && render.material != null)
            {
                render.material.shader = Shader.Find(render.material.shader.name);
            }
        }
#endif
    }

    #endregion

    #region 模块化管理对业务层开放接口
    public void RegisterLoadUI(Text infoText, Text processText)
    {
        loadingInfoTxt = infoText;
        loadingInfoTxt.gameObject.SetActive(true);

        if (processText != null)
        {
            loadingProcessTxt = processText;
            loadingProcessTxt.gameObject.SetActive(true);
        }
    }
    public void InitModule(int moduleType)
    {
        if (!string.IsNullOrEmpty(currentRunningBusinessModule))
        {
            DestroyModule(currentRunningBusinessModule);
        }

        string moduleName = ModuleNameList[moduleType];
        InitModule(moduleName);
    }

    /// <summary>
    /// 回收一个模块所有bundle内存资源，下次需要时重新从磁盘读取
    /// </summary>
    public void DestroyModule(string module)
    {
        if (!string.IsNullOrEmpty(currentRunningBusinessModule) && currentRunningBusinessModule == module)
        {
            ModuleItem oldModel = moduleDict[currentRunningBusinessModule];
            //if (oldModel.bundle != null) oldModel.bundle.Unload(true);
            //oldModel.bundle = null;
            foreach (BundleReader item in oldModel.assetBundles)
            {
                item.bundle.Unload(true);
                item.stream.Close();
                item.stream.Dispose();
            }
            oldModel.assetBundles.Clear();
            //大模块间切换时，bundle内存彻底回收，模块内部场景切换时，bundle内存保持缓存
            moduleDict.Remove(currentRunningBusinessModule);
            //随着模块的切换，bundle内部的asset对象缓存也一并清理
            ResourcesMgr.GetInstance().ClearHTFromBundle();
            currentRunningBusinessModule = null;
            Resources.UnloadUnusedAssets();
        }
    }

    #endregion


    #region 模块化资源管理相关内部接口与流程
    //测试用
    public void SwitchModuleAsync(string nextModule, bool destroyCurrentModule = true, LoadingSceneType loadingType = LoadingSceneType.CommonLoadingScene0)
    {
        string oldModule = currentRunningBusinessModule;
        if (currentRunningBusinessModule == nextModule)
        {
            Debug.LogError($" ModuleMgr/SwitchModuleAsync() {nextModule}正在运行中，不能再次切换");
            return;
        }

        currentRunningBusinessModule = nextModule;
        //test
        CommonLoadingScene.Create(nextModule, loadingType);

        DestroyModule(oldModule);
    }
    public void InitModule(string moduleName)
    {
        Debug.Log($"ModuleMgr/ >>> InitModule:  {moduleName} <<<");

        if (readInStreamingAssets)
        {
            //editor开发环境下直接读取本地streamingAssets内配置和bundle，不做版本对比也不做资源检查
            PublishData config = ReadTargetModuleConfigInStreamingAsset(moduleName);
            if (CheckTargetModuleCachedInMemory(moduleName))
            {
                ModuleAllReady(moduleName, config);
            }
            else
            {
                CreateAndSaveToModuleDict(moduleName, config);
            }
        }
        else
        {
            StartCoroutine(LoadRemoteModuleConfig(moduleName));
        }
    }
    public void SwitchModule(string nextModule, bool destroyCurrentModule = true)
    {
        if (currentRunningBusinessModule != null && currentRunningBusinessModule == nextModule)
        {
            Debug.LogError($"ModuleMgr/ {nextModule}正在运行中，不能再次切换");
            return;
        }

        //彻底销毁内存里的bundle占用,下次使用需要重新从本地磁盘读取，但dll的内存占用保留
        if (destroyCurrentModule)
        {
            DestroyModule(currentRunningBusinessModule);
        }
        currentRunningBusinessModule = null;

        InitModule(nextModule);
    }
    /// <summary>
    /// 所有dll的Type对象和ab的AssetBundle对象都放入一个MolduleItem对象内，把对象存储到moduleDict
    /// </summary>
    private void CreateAndSaveToModuleDict(string moduleName, PublishData config)
    {
        ModuleItem moduleItem = new ModuleItem();
        moduleItem.name = moduleName;
        moduleItem.assetBundles = new List<BundleReader>();
        moduleDict.Add(moduleItem.name, moduleItem);

        foreach (string bundleName in config.abOrder)
        {
            string bundleFilePath = null;
            bundleFilePath = Path.Combine(GetModuleBundleDirPath(moduleName, readInStreamingAssets), bundleName);
            BundleReader reader = ReadLocalBundleFile(bundleFilePath, config);
            moduleItem.assetBundles.Add(reader);
        }
        ModuleAllReady(moduleName, config);
    }

    private PublishData ReadTargetModuleConfigInStreamingAsset(string moduleName)
    {
        string moduleConfigPath = GetModuleConfigFilePath(moduleName, true);
        PublishData localJsonConfig = ReadLocalModuleJsonFile(moduleConfigPath);
        return localJsonConfig;
    }
    private  PublishData ReadLocalModuleJsonFile(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);
        string jsonContent = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        PublishData localJsonConfig = PublishData.Parser(jsonContent);
        return localJsonConfig;
    }

    private  void ModuleAllReady(string moduleName, PublishData config)
    {
        Debug.Log($"ModuleMgr/ {moduleName} 模块的资源包读取完毕 {config.startPrefab}");
        currentRunningBusinessModule = moduleName;
        loadingInfoTxt.text = moduleName + "模块资源准备完毕";

        if (!string.IsNullOrEmpty(config.startPrefab))
        {
            Debug.Log($"ModuleMgr/ 启动bundle内的配置场景：{config.startPrefab}");
            //RunModuleScene(moduleName, config.startPrefab);
            SceneLoadManager.Instance().LoadBundleSceneWhenModuleReady(moduleName, config.startPrefab);
        }
        else
        {
            Debug.Log("ModuleMgr/ 当前模块没有配置bundle场景，走老逻辑启动母包老场景");
            SceneLoadManager.Instance().LoadSceneWhenModuleReady();
        }
    }

    /// <summary>
    /// 如果moduleName作为key存在于moduleDict，那么必然此模块所有bundle资源都以读取好了
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    private bool CheckTargetModuleCachedInMemory(string moduleName)
    {
        if (moduleDict != null && moduleDict.ContainsKey(moduleName))
        {
            return true;
        }

        return false;
    }

    private IEnumerator LoadRemoteModuleConfig(string moduleName)
    {
        loadingInfoTxt.text = "正在加载" + moduleName + "配置";
        string url = moduleRoolUrl + moduleName + "/" + ModuleConfigFileName;
        url = GetURLAvoidCache(url);
        Debug.Log($"ModuleMgr/ 准备下载 {url}");
        UnityWebRequest req = UnityWebRequest.Get(url);
        string moduleDirPath = GetModuleDirPath(moduleName);
        req.SendWebRequest();
        while (!req.isDone)
        {
            if(loadingProcessTxt!=null) loadingProcessTxt.text = Math.Floor(req.downloadProgress * 100.0f).ToString() + "%";
            yield return 0;
        }

        if (req.result == UnityWebRequest.Result.ConnectionError ||
             req.result == UnityWebRequest.Result.ProtocolError ||
             req.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError($"ModuleMgr/ {moduleName}/{ModuleConfigFileName} load error!!!");
            Debug.LogError(req.error);
        }
        else
        {
            if (req.isDone)
            {
                if (loadingProcessTxt != null) loadingProcessTxt.text = "100%";
                loadingInfoTxt.text = moduleName + "配置加载完毕";
                Debug.Log($"ModuleMgr/ loadCompleted 本地路径：\n{moduleDirPath}/{ModuleConfigFileName}");

                string results = req.downloadHandler.text;
                PublishData remoteJsonConfig = PublishData.Parser(results);

                bool existLocal = CheckModuleJsonConfigExist(moduleName);
                if (existLocal)
                {
                    PublishData localJsonConfig = ReadLocalJsonConfig(moduleName);

                    if (CheckModuleVersionNoChange(remoteJsonConfig, localJsonConfig) && CheckModuleFilesAllExist(moduleName, localJsonConfig))
                    {
                        if (CheckTargetModuleCachedInMemory(moduleName))
                        {
                            ModuleAllReady(moduleName, localJsonConfig);
                        }
                        else
                        {
                            CreateAndSaveToModuleDict(moduleName, localJsonConfig);
                        }
                    }
                    else
                    {
                        ClearAndLoadModule(moduleDirPath, moduleName, req.downloadHandler.data, remoteJsonConfig);
                    }
                }
                else
                {
                    ClearAndLoadModule(moduleDirPath, moduleName, req.downloadHandler.data, remoteJsonConfig);
                }
            }
        }
    }

    /// <summary>
    /// 清理干净当前module现有本地目录文件然后重新下载
    /// </summary>
    private void ClearAndLoadModule(string moduleDirPath, string moduleName, byte[] jsonData, PublishData remoteJsonConfig)
    {
        DeleteDirectory(moduleDirPath);
        CheckDirAndCreateWhenNeeded(moduleDirPath);
        CheckDirAndCreateWhenNeeded(GetModuleBundleDirPath(moduleName));
        WriteRemoteJsonDataInLocal(moduleDirPath, jsonData);
        LoadModuleTotalBundle(moduleName, remoteJsonConfig);
    }

    private PublishData ReadLocalJsonConfig(string moduleName)
    {
        StreamReader reader = new StreamReader(GetModuleConfigFilePath(moduleName));
        string jsonContent = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        PublishData localJsonConfig = PublishData.Parser(jsonContent);
        return localJsonConfig;
    }

    private void AllBundleLoadComplete(string moduleName, PublishData config)
    {
        CreateAndSaveToModuleDict(moduleName, config);
    }

    private void LoadModuleTotalBundle(string moduleName, PublishData config)
    {
        if(config.abOrder.Count > 0)
        {
            loadCount = config.abOrder.Count;
            loadIndex = 0;
            string bundleDirPath = GetModuleBundleDirPath(moduleName);
            string bundleName = config.abOrder[loadIndex];
            StartCoroutine(LoadModuleBundle(moduleName, bundleDirPath, bundleName, config));
        }
        else
        {
            AllBundleLoadComplete(moduleName, config);
        }
    }

    private void WriteRemoteJsonDataInLocal(string moduleDirPath, byte[] jsonData)
    {
        Debug.Log($"ModuleMgr/ 远程配置文件写入本地: \n{moduleDirPath}");
        string configFilePath = Path.Combine(moduleDirPath, ModuleConfigFileName);
        FileInfo fileInfo = new FileInfo(configFilePath);
        FileStream fs = fileInfo.Create();
        fs.Write(jsonData, 0, jsonData.Length);
        fs.Flush();
        fs.Close();
        fs.Dispose();
    }

    private void DeleteDirectory(string dirPath)
    {
        if (Directory.Exists(dirPath))
        {
            Debug.Log($"ModuleMgr/ 清空老目录 {dirPath}");
            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }
    }

    private string GetModuleConfigFilePath(string moduleName, bool isStreamingAssets = false)
    {
        string moduleDirPath = GetModuleDirPath(moduleName, isStreamingAssets);
        string localJsonFilePath = Path.Combine(moduleDirPath, ModuleConfigFileName);
        return localJsonFilePath;
    }

    private string GetModuleDirPath(string moduleName, bool isStreamingAssets = false)
    {
        string pathHead = isStreamingAssets ? Path.Combine(Application.streamingAssetsPath, ModuleRootDirInStreamingAsset) : Application.persistentDataPath;
        string moduleDirPath = Path.Combine(pathHead, moduleName);
        return moduleDirPath;
    }

    private string GetModuleBundleDirPath(string moduleName, bool isStreamingAssets = false)
    {
        string moduleDirPath = GetModuleDirPath(moduleName, isStreamingAssets);
        /*
         * streamingAssets内的模块资源是在使用发布工具发布时拷贝的发布目录，而当前发布目录里bundle文件都从ab目录里挪出来了，所以streamingAssset的开发环境
         * 和线上生产环境下载写入到本地的bundle目录是不一致的，生产环境目录的bundle都还是在模块内部的ab目录内
         */
        if (isStreamingAssets)
        {
            return moduleDirPath;
        }
        return Path.Combine(moduleDirPath, "ab");

    }
    private bool CheckModuleJsonConfigExist(string moduleName)
    {
        if (File.Exists(GetModuleConfigFilePath(moduleName)))
        {
            return true;
        }
        return false;
    }

    private bool CheckModuleVersionNoChange(PublishData remoteConfig, PublishData localConfig)
    {
        if (remoteConfig.version != localConfig.version)
        {
            Debug.Log($"ModuleMgr/ 【检查版本】：需要更新{remoteConfig.manifestAb} old version: {localConfig.version} new version: {remoteConfig.version}");
            return false;
        }
        Debug.Log($"ModuleMgr/ 【检测版本】：无需更新{remoteConfig.manifestAb} current version: {localConfig.version}");

        return true;
    }

    private bool CheckModuleFilesAllExist(string moduleName, PublishData config)
    {
        //根据abOrder和dllOrder判断当前模块本地所有文件是否都健全
        string bundleDirPath = GetModuleBundleDirPath(moduleName);
        string bundlePath = null;
        foreach (string item in config.abOrder)
        {
            bundlePath = Path.Combine(bundleDirPath, item);
            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"ModuleMgr/ {moduleName}模块的本地bundle资源有缺失，因为{bundlePath}不存在");
                return false;
            }
        }

        return true;
    }


    private void CheckDirAndCreateWhenNeeded(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"ModuleMgr/ 创建本地目录 {folderPath}");
        }
    }

    /// <summary>
    /// 配置以及ab包的所有远程http请求都以版本号作为后缀，避免http下载时拿取浏览器内部缓存数据
    /// 没有版本号时以“13位时间戳_随机数” 作为后缀
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private string GetURLAvoidCache(string baseUrl, PublishData config = null)
    {
        string suffix = "?v=";
        if (config == null)
        {
            long now = ((System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            suffix += now.ToString() + "_" + UnityEngine.Random.Range(0, 10000);
        }
        else
        {
            suffix += config.version;
        }
        string newUrl = baseUrl + suffix;
        return newUrl;
    }


    //加载模块的bundle后，实例化bundle
    private IEnumerator LoadModuleBundle(string moduleName, string bundleDirPath, string bundleName, PublishData config)
    {
        loadingInfoTxt.text = "正在加载" + bundleName + "资源包";
        string url = moduleRoolUrl + moduleName + "/" + bundleName;//发布工具发布出来的bundle包从ab里挪出来了，why?
        url = GetURLAvoidCache(url, config);
        Debug.Log($"ModuleMgr/ 准备下载bundle: {url} ");
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SendWebRequest();

        while (!req.isDone)
        {
            if (loadingProcessTxt != null) loadingProcessTxt.text = Math.Floor(req.downloadProgress * 100.0f).ToString() + "%";
            yield return 0;
        }

        if (req.result == UnityWebRequest.Result.ConnectionError ||
             req.result == UnityWebRequest.Result.ProtocolError ||
             req.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError($"ModuleMgr/ bundle file: {bundleName} load error!!!");
            Debug.LogError(req.error);
        }
        else
        {
            if (req.isDone)
            {
                if (loadingProcessTxt != null) loadingProcessTxt.text = "100%";
                loadingInfoTxt.text = bundleName + "资源包加载完毕";
                Debug.Log($"{moduleName}: {bundleName} load completed, \npath: {bundleDirPath}/{bundleName}");

                byte[] results = req.downloadHandler.data;
                if (Caching.ClearAllCachedVersions(bundleName))
                {
                    Debug.Log($"ModuleMgr/ bundle文件: {bundleName}的http缓存清除成功");
                }
                else
                {
                    Debug.LogError($"ModuleMgr/ bundle文件: {bundleName}的http缓存清除失败");
                }

                //不使用unity自带UnityWebRequestAssetBundle缓存机制，此处根据下载的bytes单独写入本地模块目录
                string bundleFilePath = Path.Combine(bundleDirPath, bundleName);
                FileInfo fileInfo = new FileInfo(bundleFilePath);
                FileStream fs = fileInfo.Create();
                fs.Write(results, 0, results.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();

                loadIndex++;
                if (loadIndex == loadCount)
                {
                    AllBundleLoadComplete(moduleName, config);
                }
                else
                {
                    string nextBundleName = config.abOrder[loadIndex];
                    StartCoroutine(LoadModuleBundle(moduleName, bundleDirPath, nextBundleName, config));

                }
            }
        }
    }

    private BundleReader ReadLocalBundleFile(string bundleFilePath, PublishData config)
    {
        Debug.Log($"ModuleMgr/ 准备读取本地bundle:\n {bundleFilePath}");
        FileStream fs = new FileStream(bundleFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false);
        AssetBundle ab = AssetBundle.LoadFromStream(fs);
        BundleReader reader = new BundleReader();
        reader.bundle = ab;
        reader.stream = fs;

        if (ab == null)
        {
            Debug.LogError($"ModuleMgr/ 读取失败");
        }
        Debug.Log($"ModuleMgr/ 读取成功");
        return reader;
    }

    //private void RunModuleScene(string moduleName, string sceneName)
    //{
    //    AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    //    asy.completed += operation =>
    //    {
    //        Debug.Log($"scene: {sceneName} load completed");
    //        //test for space module scene
    //        Camera camera = Camera.main;
    //        switch (moduleName)
    //        {
    //            case "module1":
    //                camera.gameObject.AddComponent<Module1MainScene>();
    //                break;
    //            case "module2":
    //                camera.gameObject.AddComponent<Module2MainScene>();
    //                break;
    //            default:
    //                break;
    //        }

    //        Resources.UnloadUnusedAssets();
    //    };
    //}


    #endregion




}

#region 模块化资源管理相关类
public class ModuleItem
{
    public string name;
    /// <summary>
    /// 主要包
    /// </summary>
    public AssetBundle bundle;
    /// <summary>
    /// 关联的所有子包
    /// </summary>
    public List<BundleReader> assetBundles;
};

public class BundleReader
{
    public AssetBundle bundle;
    public FileStream stream;
}

public class PublishData
{
    public string version;
    public string cdn;
    public string manifestAb;
    public string startPrefab;
    //public AssemblyData startFunc = new AssemblyData();
    public List<string> preloadPrefab = new List<string>();
    public List<string> abOrder = new List<string>();
    public List<string> dllOrder = new List<string>();
    //public List<AssetsOrder> assetsOrder = new List<AssetsOrder>();
    public static PublishData Parser(string json)
    {
        PublishData publishData = JsonUtility.FromJson<PublishData>(json);
        return publishData;
    }
}
#endregion
