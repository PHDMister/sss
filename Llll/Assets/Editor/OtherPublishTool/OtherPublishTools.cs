using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PlasticGui;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental;
using UnityEditor.VersionControl;


namespace OtherPublishTools
{
    /// <summary>
    /// 集成发布  打包   改命名空间
    /// </summary>
    public class OtherPublishTools
    {
        private static PublishConfigTool windowTool;

        //发布window 创建配置
        [MenuItem("PublishTools/元宇宙发布工具")]
        public static void OpenWindow()
        {
            if (windowTool == null)
            {
                windowTool = EditorWindow.GetWindow<PublishConfigTool>();
                windowTool.titleContent = new GUIContent("元宇宙游戏发布工具");
                windowTool.minSize = new Vector2(1000, 800);
                windowTool.Show();
                windowTool.InitTool();
            }
        }
    }


    [Serializable]
    public class PrefabPack
    {
        public string ab;
        public string prefab;
    }

    [Serializable]
    public class AssemblyPack
    {
        public string assembly;
        public string func;
    }

    [Serializable]
    public class PublishConfig
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version;
        /// <summary>
        /// 启动时 需要的资源
        /// </summary>
        public PrefabPack start;
        /// <summary>
        /// 启动时 需要的程序集   启动函数
        /// </summary>
        public AssemblyPack startfunc;
        /// <summary>
        /// 预加载的prefab清单
        /// </summary>
        public List<PrefabPack> preloadPrefab;
        /// <summary>
        /// 所有ab的清单
        /// </summary>
        public List<string> abOrder;
    }


    /// <summary>
    /// 具体窗口
    /// </summary>
    public class PublishConfigTool : EditorWindow
    {
        private const string ModuleRootDirInStreamingAsset = "ResModules"; //by jd
        private const string RootPathPk = "PublishConfigTool_RootPathPk";
        private const string NameSpace_NamePk = "NameSpace_NamePk";
        private const string NameSpace_PathPk = "NameSpace_PathPk";
        private const string GenAb_PathPk = "GenAb_PathPk";
        private const string Publish_DataPk = "Publish_DataPk";
        private const string ToolDll_DataPk = "ToolDll_DataPk";
        private const string Publish_ProjectPk = "Publish_ProjectPk";
        private const string abAsset_PathPk = "abAsset_PathPk";

        private int tar_index;
        private string[] tabBarNames = new string[] { "打包资源和程序集", "发布", "设置" };
        public static string RootPath = "";
        public static string RootCachePath => PublishConfigTool.RootPath + "/Cache";
        public static string ToolDataPath;
        public static string ToolCachePath => PublishConfigTool.ToolDataPath + "/Cache";
        private bool isfirstDrawToolConfg = true;
        public static List<string> wList = new List<string>()
        {
            "Assembly-CSharp.dll",
            "Assembly-CSharp-firstpass.dll",
            "UnityEngine.TestRunner.dll",
            "Unity.RenderPipeline.Universal.ShaderLibrary.dll",
            "Unity.RenderPipelines.Core.Runtime.dll",
            "Unity.RenderPipelines.Core.ShaderLibrary.dll",
            "Unity.RenderPipelines.ShaderGraph.ShaderGraphLibrary.dll",
            "Unity.RenderPipelines.Universal.Runtime.dll",
            "Unity.RenderPipelines.Universal.Shaders.dll"
        };
        private static AssetBundleManifest MainManifest;


        public void InitTool()
        {
            RootPath = EditorPrefs.GetString(RootPathPk, "");
            if (!string.IsNullOrEmpty(RootPath))
            {
                int index = RootPath.IndexOf("Assets");
                ToolDataPath = RootPath.Substring(index);
            }

            nameSpaceText = EditorPrefs.GetString(NameSpace_NamePk, "");
            scriptPath = EditorPrefs.GetString(NameSpace_PathPk, "");

            dllPath = EditorPrefs.GetString(ToolDll_DataPk, "");
            publishPath = EditorPrefs.GetString(Publish_ProjectPk, "");

            abPath = EditorPrefs.GetString(GenAb_PathPk, "");
            abAssetPath = EditorPrefs.GetString(abAsset_PathPk, "");

            //////
            //var settings = HybridCLR.Editor.HybridCLRSettings.Instance;
            //string appDataPath = Application.dataPath.Replace("Assets", "");
            //dllPath = $"{appDataPath}{settings.hotUpdateDllCompileOutputRootDir}/{EditorUserBuildSettings.activeBuildTarget}";
            //EditorPrefs.SetString(ToolDll_DataPk, dllPath);
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                if (string.IsNullOrEmpty(RootPath) || string.IsNullOrEmpty(publishPath))
                {
                    DrawToolConfg(isfirstDrawToolConfg);
                    isfirstDrawToolConfg = false;
                }
                else
                {
                    DrawTabs();
                }
            }
            EditorGUILayout.EndVertical();
        }

        #region Tabs
        private int lasTab = -1;
        private void DrawTabs()
        {
            tar_index = GUILayout.Toolbar(tar_index, tabBarNames);
            GUILayout.Space(20);

            bool isEnter = lasTab != tar_index;
            lasTab = tar_index;

            switch (tar_index)
            {
                case 0: DrawTabContext_BuildAB(isEnter); break;
                case 1: DrawTabContext_Publish(isEnter); break;
                case 2: DrawToolConfg(isEnter); break;
            }

        }
        #endregion


        #region Settings

        private string publishPath;
        private void DrawToolConfg(bool onEnter)
        {
            GUILayout.BeginHorizontal();
            {
                RootPath = EditorGUILayout.TextField("设置当前工具的根目录：", RootPath);
                if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
                {
                    string outFilePath = "";
                    if (!string.IsNullOrEmpty(RootPath)) outFilePath = RootPath;
                    else outFilePath = Application.dataPath;
                    RootPath = EditorUtility.OpenFolderPanel("选择工具根目录：", outFilePath, "");
                    EditorPrefs.SetString(RootPathPk, RootPath);
                    //int index = RootPath.IndexOf("Assets");
                    //ToolDataPath = RootPath.Substring(index);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            {
                publishPath = EditorGUILayout.TextField("设置发布目录：", publishPath);
                if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
                {
                    string outFilePath = "";
                    if (!string.IsNullOrEmpty(RootPath)) outFilePath = publishPath;
                    else outFilePath = Application.dataPath;
                    publishPath = EditorUtility.OpenFolderPanel("选择工具根目录：", outFilePath, "");
                    EditorPrefs.SetString(Publish_ProjectPk, publishPath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("清除本地发布配置", GUILayout.Height(30)))
            {
                EditorPrefs.DeleteKey(Publish_DataPk);
                EditorPrefs.DeleteKey(ToolDll_DataPk);
                EditorPrefs.DeleteKey(Publish_ProjectPk);
                EditorPrefs.DeleteKey(Publish_DataPk);
            }

        }
        #endregion


        #region NameSpace
        private string nameSpaceText;
        private string scriptPath;
        private string dllPath;
        private void DrawTabContext_NameSpace()
        {
            GUILayout.BeginHorizontal();
            nameSpaceText = EditorGUILayout.TextField("命名空间：", nameSpaceText);
            if (GUILayout.Button("保存", GUILayout.MaxWidth(100))) EditorPrefs.SetString(NameSpace_NamePk, nameSpaceText);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            scriptPath = EditorGUILayout.TextField("修改命名空间脚本目录：", scriptPath);
            if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
            {
                string outFilePath = "";
                if (!string.IsNullOrEmpty(scriptPath)) outFilePath = scriptPath;
                else outFilePath = Application.dataPath;
                scriptPath = EditorUtility.OpenFolderPanel("选择目录：", outFilePath, "");
                EditorPrefs.SetString(NameSpace_PathPk, scriptPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("执行添加命名空间", GUILayout.Height(30)))
            {

            }
        }
        #endregion


        #region CompileDll


        private void DrawTabContext_CompileDll()
        {
            //////HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
        }
        #endregion


        #region BuildAB

        private string ABPath
        {
            get => publishPath + "/";
        }
        private string abPath;
        private string abAssetPath;
        private string cdllPath
        {
            get => publishPath + "/dll";
        }
        private void DrawTabContext_BuildAB(bool onEnter)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("输入生成AB的资源路径：", abAssetPath);
            if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
            {
                string outFilePath = "";
                if (!string.IsNullOrEmpty(abAssetPath)) outFilePath = abAssetPath;
                else outFilePath = Application.dataPath;
                abAssetPath = EditorUtility.OpenFolderPanel("选择目录：", outFilePath, "");
                EditorPrefs.SetString(abAsset_PathPk, abAssetPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("输出生成bundle路径：", abPath);
            if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
            {
                string outFilePath = "";
                if (!string.IsNullOrEmpty(abPath)) outFilePath = abPath;
                else outFilePath = Application.dataPath;
                abPath = EditorUtility.OpenFolderPanel("选择目录：", outFilePath, "");
                EditorPrefs.SetString(GenAb_PathPk, abPath);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            if (GUILayout.Button("生成AB包", GUILayout.Height(30)))
            {
                if (!string.IsNullOrEmpty(ABPath))
                {
                    CreateAtlas.CreateAllSpriteAtlas();
                    Thread.Sleep(50);
                    AssetDatabase.Refresh();
                    Thread.Sleep(50);
                    if (!Directory.Exists(abPath)) Directory.CreateDirectory(abPath);
                    else DelectDir(abPath);
                    //DirectoryInfo directoryInfo = new DirectoryInfo(abAssetPath);
                    //FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    //if (files.Length < 1)
                    //{
                    //    Debug.LogError("Error: selection directory  file  null");
                    //    return;
                    //}
                    //AssetBundleBuild[] builds = new AssetBundleBuild[2];
                    ////Assets
                    //builds[0] = GetAssetBundleBuild(files);
                    ////Sence
                    //builds[1] = GetSceneBundleBuild(files);

                    MainManifest = BuildPipeline.BuildAssetBundles(abPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
                    Thread.Sleep(50);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("~~~~~~~~~~~~~生成AB  Fail  AB路径是空的 !!!! ~~~~~~~~~~");
                }
            }
            GUILayout.Space(50);

            EditorGUILayout.LabelField("HybridCLR:");
            EditorGUILayout.LabelField("HybridCLR安装状态：", HasInstalledHybridCLR().ToString());
            GUILayout.BeginHorizontal();
            //dllPath = EditorGUILayout.TextField("生成Dll路径：", dllPath);
            EditorGUILayout.LabelField("HybridCLR 生成Dll路径：", dllPath);
            //if (GUILayout.Button("选择目录", GUILayout.MaxWidth(100)))
            //{
            //    string outFilePath = "";
            //    if (!string.IsNullOrEmpty(dllPath)) outFilePath = dllPath;
            //    else outFilePath = Application.dataPath;
            //    dllPath = EditorUtility.OpenFolderPanel("选择目录：", outFilePath, "");
            //    //EditorPrefs.SetString(ToolDll_DataPk, dllPath);
            //}
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("HybridCLR Dll 发布 路径：", cdllPath);
            GUILayout.Space(20);
            if (GUILayout.Button("HybridCLR DLL 生成All", GUILayout.Height(30)))
            {
                if (!HasInstalledHybridCLR())
                {
                    Debug.LogError("~~~~~~~~~~~~~HybridCLR 未安装~~~~~~~~~~~~");
                    return;
                }
                if (!string.IsNullOrEmpty(dllPath))
                {
                    DrawTabContext_CompileDll();
                }
                else
                {
                    Debug.LogError("~~~~~~~~~~~~~生成DLL  Fail  DLL路径是空的 !!!! ~~~~~~~~~~");
                }
            }
        }
        private float HasInstalledLastTime = 0;
        private bool HasInstalledLastResult = false;
        public bool HasInstalledHybridCLR()
        {
            if (Time.realtimeSinceStartup - HasInstalledLastTime > 5)
            {
                HasInstalledLastTime = Time.realtimeSinceStartup;
                //////HasInstalledLastResult = Directory.Exists($"{HybridCLR.Editor.SettingsUtil.LocalIl2CppDir}/libil2cpp/hybridclr");
            }
            return HasInstalledLastResult;
        }
        private AssetBundleBuild GetAssetBundleBuild(FileInfo[] files)
        {
            AssetBundleBuild bundleBuild = new AssetBundleBuild();
            bundleBuild.assetBundleName = Path.GetFileName(abAssetPath);
            bundleBuild.assetBundleVariant = "assetbundle";
            List<string> resPath = new List<string>();
            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath.FullName) == ".meta") continue;
                if (Path.GetExtension(filePath.FullName) == ".unity") continue;
                string assetPath = filePath.FullName.Replace(Application.dataPath.Replace("/", @"\"), "Assets");
                resPath.Add(assetPath);
            }
            bundleBuild.assetNames = resPath.ToArray();
            return bundleBuild;
        }
        private AssetBundleBuild GetSceneBundleBuild(FileInfo[] files)
        {
            AssetBundleBuild bundleBuild = new AssetBundleBuild();
            bundleBuild.assetBundleName = Path.GetFileName(abAssetPath) + "_scene";
            bundleBuild.assetBundleVariant = "assetbundle";
            List<string> sceneResPath = new List<string>();
            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath.FullName) == ".meta") continue;
                if (Path.GetExtension(filePath.FullName) != ".unity") continue;
                string assetPath = filePath.FullName.Replace(Application.dataPath.Replace("/", @"\"), "Assets");
                sceneResPath.Add(assetPath);
            }
            bundleBuild.assetNames = sceneResPath.ToArray();
            return bundleBuild;
        }

        #endregion


        #region Publish
        private PushlishData _pushlishData;
        private PushlishData pushlishData
        {
            get
            {
                if (_pushlishData == null)
                {
                    string data = EditorPrefs.GetString(Publish_DataPk, "");
                    if (!string.IsNullOrEmpty(data))
                        _pushlishData = JsonUtility.FromJson<PushlishData>(data);
                    else
                        _pushlishData = new PushlishData();
                }
                return _pushlishData;
            }
        }

        private UIDrawElement _drawElement;
        public UIDrawElement drawElement
        {
            get
            {
                if (_drawElement == null)
                {
                    _drawElement = new UIDrawElement();
                    _drawElement.preloadPrefab.AddRange(pushlishData.preloadPrefab);
                    pushlishData.abOrder.Clear();
                    _drawElement.dllOrder.Clear();
                }
                return _drawElement;
            }
        }

        private SerializedObject _publishSerObject;
        private SerializedObject PublishSerObject
        {
            get
            {
                if (_publishSerObject == null)
                {
                    _publishSerObject = new SerializedObject(drawElement);
                    _preloadPrefabSerPro = PublishSerObject.FindProperty("preloadPrefab");
                    _abOrderSerPro = PublishSerObject.FindProperty("abOrder");
                    _dllOrderSerPro = PublishSerObject.FindProperty("dllOrder");
                }
                return _publishSerObject;
            }
        }

        private SerializedProperty _preloadPrefabSerPro;
        private SerializedProperty preloadPrefabSerPro
        {
            get
            {
                if (_preloadPrefabSerPro == null)
                    _preloadPrefabSerPro = PublishSerObject.FindProperty("preloadPrefab");
                return _preloadPrefabSerPro;
            }
        }

        private SerializedProperty _abOrderSerPro;
        private SerializedProperty abOrderSerPro
        {
            get
            {
                if (_abOrderSerPro == null)
                    _abOrderSerPro = PublishSerObject.FindProperty("abOrder");
                return _abOrderSerPro;
            }
        }

        private SerializedProperty _dllOrderSerPro;
        private SerializedProperty dllOrderSerPro
        {
            get
            {
                if (_dllOrderSerPro != null)
                    _dllOrderSerPro = PublishSerObject.FindProperty("dllOrder");
                return _dllOrderSerPro;
            }
        }

        private Vector2 scVector2;
        private void DrawTabContext_Publish(bool onEnter)
        {
            if (onEnter)
            {
                //把所有AssetBundle 加入列表
                if (!string.IsNullOrEmpty(abPath) && (drawElement.abOrder.Count == 0 || string.IsNullOrEmpty(pushlishData.manifestAb)))
                {
                    string[] filePaths = Directory.GetFiles(abPath);
                    List<string> fList = new List<string>(filePaths);
                    var allFilePaths = fList.FindAll(s => s.EndsWith(".assetbundle") || !Path.HasExtension(s));
                    var manifest = fList.Find(s => !Path.HasExtension(s) && !s.Contains("build_info"));
                    pushlishData.manifestAb = Path.GetFileName(manifest);
                    drawElement.abOrder.Clear();
                    allFilePaths.ForEach(s => drawElement.abOrder.Add(Path.GetFileName(s)));
                    List<string> denpends = DependenAssetBundleList();
                    pushlishData.abOrder.AddRange(denpends);
                }
                //把所有程序集改名后加入列表
                if (!string.IsNullOrEmpty(dllPath) && drawElement.dllOrder.Count == 0)
                {
                    string[] filePaths = Directory.GetFiles(dllPath);
                    List<string> fList = new List<string>(filePaths);
                    var allFilePaths = fList.FindAll(s => s.EndsWith(".dll"));
                    drawElement.dllOrder.Clear();
                    allFilePaths.ForEach(s =>
                    {
                        string name = Path.GetFileName(s);
                        if (!wList.Contains(name))
                            drawElement.dllOrder.Add(name + ".bytes");
                    });
                }

                PublishSerObject.Update();
            }

            scVector2 = EditorGUILayout.BeginScrollView(scVector2);
            pushlishData.version = EditorGUILayout.TextField("Version：", pushlishData.version);
            GUILayout.Space(10);

            pushlishData.cdn = EditorGUILayout.TextField("CDN：", pushlishData.cdn);
            GUILayout.Space(10);

            pushlishData.manifestAb = EditorGUILayout.TextField("ManifestAb：", pushlishData.manifestAb);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("启动资源:");
            pushlishData.startPrefab = EditorGUILayout.TextField("prefab名字：", pushlishData.startPrefab);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("启动程序:");
            pushlishData.startFunc.assembly = EditorGUILayout.TextField("启动程序集：", pushlishData.startFunc.assembly);
            pushlishData.startFunc.func = EditorGUILayout.TextField("启动函数：", pushlishData.startFunc.func);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("预加载列表:");
            EditorGUILayout.PropertyField(preloadPrefabSerPro);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Assetbuild清单：");
            EditorGUILayout.PropertyField(abOrderSerPro);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("程序集清单：");
            EditorGUILayout.PropertyField(dllOrderSerPro);
            GUILayout.Space(10);

            if (GUILayout.Button("发布 [测试服]", GUILayout.Height(30)))
            {
                PublishFunc(0);
            }
            GUILayout.Space(10);

            if (GUILayout.Button("发布 [正式服]", GUILayout.Height(30)))
            {
                //PublishFunc(1);
                Debug.LogError("~~~~~~~~~~~~~暂时未实现~~~~~~~~~~~~");
            }
            GUILayout.Space(10);
            if (GUILayout.Button("【打包WebGL】", GUILayout.Height(30)))
            {
                this.Close();
                Thread.Sleep(30);
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = new[]
                {
                    "Assets/Scenes/LoginLoading.unity",
                    "Assets/Scenes/Empty1.unity",
                    "Assets/Scenes/Empty2.unity",
                    "Assets/Scenes/Empty3.unity",
                    "Assets/Scenes/Empty4.unity",
                    "Assets/Scenes/Empty5.unity",
                    "Assets/Scenes/Island01.unity"
                };
                buildPlayerOptions.locationPathName = "SpaceWebGL";
                buildPlayerOptions.target = BuildTarget.WebGL;
                buildPlayerOptions.options = BuildOptions.None;
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                Thread.Sleep(50);
                BuildSummary summary = report.summary;
                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                }
                else if (summary.result == BuildResult.Failed)
                {
                    Debug.Log("Build failed");
                }
            }
            EditorGUILayout.EndScrollView();
        }
        #endregion

        //channel 0:测试服   1:正式服
        private void PublishFunc(int channel)
        {
            pushlishData.preloadPrefab.Clear();
            pushlishData.preloadPrefab.AddRange(drawElement.preloadPrefab);

            pushlishData.abOrder.Clear();
            pushlishData.abOrder.AddRange(drawElement.abOrder);

            pushlishData.dllOrder.Clear();
            pushlishData.dllOrder.AddRange(drawElement.dllOrder);

            CollectAssetOrder(pushlishData.assetsOrder, true, drawElement.abOrder);

            string jsonstr = JsonUtility.ToJson(pushlishData, false);
            EditorPrefs.SetString(Publish_DataPk, jsonstr);

            /*
             * by jd, 发布的同时，拷贝到项目本地的StreamingAssets目录下，
             * 1, 外网测试/正式版本发布的时候也会把StreamingAssets/ResModules一起发布出去，外网app用户就是从这个目录内去下载bundle配置和资源
             * 2，此目录同时给本地editor debug开发环境测试用。
             * */
            if (channel == 0)
            {
                string[] temp = abPath.Split('/');
                string dest = Path.Combine(Application.streamingAssetsPath, ModuleRootDirInStreamingAsset, temp[temp.Length - 1]);
                Debug.Log($"将发布的目录文件拷贝至：{dest}");
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                    Thread.Sleep(50);
                    AssetDatabase.Refresh();
                    Thread.Sleep(50);
                }
                //FileUtil.DeleteFileOrDirectory(dest);
                //FileUtil.CopyFileOrDirectory(abPath, dest);
                //清除文件中不在配置表内的资源
                DelectFileNotInAbOrder(dest, drawElement.abOrder);
                File.WriteAllText(dest + "/config.json", jsonstr);

                //如果拷贝的dll目录就有删掉
                string oldDLLPath = Path.Combine(dest, "dll");
                if (Directory.Exists(oldDLLPath))
                {
                    FileUtil.DeleteFileOrDirectory(oldDLLPath);
                }
            }
            Debug.Log("~~~~~~~~~~~~~发布成功~~~~~~~~~~");
            Thread.Sleep(50);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 复制文件到指定目录
        /// </summary>
        /// <param name="fromPath">起始路径</param>
        /// <param name="toPath">目标路径</param>
        /// <param name="ext">文件后缀</param>
        /// <param name="hasNoExt">是否包含没有后缀的文件</param>
        /// <param name="addExt">增加额外后缀</param>
        private void CopyFile2Dir(string fromPath, string toPath, string ext, bool hasNoExt = false, string addExt = "", List<string> target = null)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                Debug.LogError($"~~~~~~~~~~fromPath  is null  toPath:{toPath}  ext:{ext} ~~~~~~~~~~");
                return;
            }
            if (fromPath == toPath) return;
            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }
            string newPath;
            FileInfo fileInfo;
            if (!toPath.EndsWith("/") && !toPath.EndsWith("\\")) toPath += "/";
            string[] strs = Directory.GetFiles(fromPath);
            foreach (string path in strs)
            {
                if (path.EndsWith(ext) || (hasNoExt && !Path.HasExtension(path)))
                {
                    if (target != null)
                    {
                        if (target.Contains(Path.GetFileName(path)))
                        {
                            fileInfo = new FileInfo(path);
                            newPath = toPath + fileInfo.Name;
                            if (!string.IsNullOrEmpty(addExt) && !newPath.EndsWith(addExt))
                                newPath = newPath + addExt;
                            File.Copy(path, newPath, true);
                        }
                    }
                    else
                    {
                        fileInfo = new FileInfo(path);
                        newPath = toPath + fileInfo.Name;
                        if (!string.IsNullOrEmpty(addExt) && !newPath.EndsWith(addExt))
                            newPath = newPath + addExt;
                        File.Copy(path, newPath, true);
                    }
                }
            }
        }
        private void CopyFile2Dir(string fromPath, string toPath, string ext, string addExt, List<string> target)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                Debug.LogError($"~~~~~~~~~~fromPath  is null  toPath:{toPath}  ext:{ext} ~~~~~~~~~~");
                return;
            }
            if (fromPath == toPath) return;
            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }
            string newPath;
            FileInfo fileInfo;
            if (!toPath.EndsWith("/") && !toPath.EndsWith("\\")) toPath += "/";
            string[] strs = Directory.GetFiles(fromPath);
            foreach (string path in strs)
            {
                if (path.EndsWith(ext) && target.Contains(Path.GetFileName(path) + ".bytes"))
                {
                    fileInfo = new FileInfo(path);
                    newPath = toPath + fileInfo.Name;
                    if (!string.IsNullOrEmpty(addExt))
                        newPath = newPath + addExt;
                    File.Copy(path, newPath, true);
                }
            }
        }
        private void DelFilesByExt(string fromPath, string ext)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                Debug.LogError($"~~~~~~~~~~fromPath  is null  fromPath:{fromPath}  ext:{ext} ~~~~~~~~~~");
                return;
            }
            string[] strs = Directory.GetFiles(fromPath);
            foreach (var filePath in strs)
            {
                if (Path.GetExtension(filePath) == ext)
                {
                    File.Delete(filePath);
                }
            }
        }
        public void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);
                    }
                    else
                    {
                        File.Delete(i.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("e=" + e.Message + "   " + e.StackTrace);
            }
        }
        private void CollectAssetOrder(List<AssetsOrder> assetsOrder, bool needFullPath, List<string> abOrder)
        {
            pushlishData.assetsOrder.Clear();
            string[] fileNames = Directory.GetFiles(abPath, "*.manifest", SearchOption.TopDirectoryOnly);
            foreach (var fileName in fileNames)
            {
                if (!abOrder.Contains(Path.GetFileNameWithoutExtension(fileName)))
                    continue;
                string fName = Path.GetFileName(fileName);
                string ext = Path.GetExtension(fName);
                if (string.IsNullOrEmpty(ext) || ext != ".manifest") continue;
                string nfName = fileName.Replace("\\", "/");
                Encoding encoding = AddNamespaceTool.GetTextFileEncodingType(nfName);
                string[] nStrings = File.ReadAllLines(nfName, encoding);
                List<string> strList = new List<string>(nStrings);
                AssetsOrder itemOrder = new AssetsOrder();
                string ctx = Path.GetFileNameWithoutExtension(nfName);
                itemOrder.key = ctx;
                int ctxIndex = strList.FindIndex(str => str == "Assets:");
                if (ctxIndex == -1) continue;
                ctxIndex += 1;
                int ctxEndidenx = strList.FindIndex(str => str == "Dependencies:");
                if (ctxEndidenx == -1) ctxEndidenx = strList.Count - 1;

                for (int i = ctxIndex; i <= ctxEndidenx; i++)
                {
                    string txtStrStart = strList[i];
                    if (txtStrStart.StartsWith("-"))
                    {
                        txtStrStart = txtStrStart.Replace("- ", "");
                        if (!needFullPath)
                        {
                            if (txtStrStart.StartsWith("Assets/Resources/"))
                            {
                                string path = txtStrStart.Replace("Assets/Resources/", "");
                                itemOrder.assetsItems.Add(path);
                            }
                            else if (txtStrStart.StartsWith("Assets/"))
                            {
                                string path = txtStrStart.Replace("Assets/", "");
                                itemOrder.assetsItems.Add(path);
                            }
                        }
                        else
                        {
                            itemOrder.assetsItems.Add(txtStrStart);
                        }
                    }
                    else if (char.IsLetterOrDigit(txtStrStart[0]))
                    {
                        break;
                    }
                }
                assetsOrder.Add(itemOrder);
            }
        }
        private List<string> DependenAssetBundleList()
        {
            List<string> deList = new List<string>();
            if (MainManifest == null) return deList;
            string manifest = Path.GetFileName(abPath);
            //string tarManifest = abPath + "/" + manifest+ ".manifest";
            string[] assetStrings = MainManifest.GetAllAssetBundles();
            foreach (var assetString in assetStrings)
            {
                if (!pushlishData.abOrder.Contains(assetString))
                {
                    if (TryGetTopFullName(assetString, out var fullName))
                    {
                        deList.Add(fullName);
                    }
                }
            }
            return deList;
        }
        private bool TryGetTopFullName(string assetString, out string fullName)
        {
            fullName = null;
            string strAssetPath = Application.streamingAssetsPath + "/ResModules/";
            string[] StreamingRootPath = Directory.GetDirectories(strAssetPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var path in StreamingRootPath)
            {
                string newName = path + "/" + assetString;
                if (File.Exists(newName))
                {
                    fullName = Path.GetFileName(path) + "/" + assetString;
                    return true;
                }
            }
            return false;
        }
        //清除文件中不在配置表内的资源
        private void DelectFileNotInAbOrder(string path, List<string> assetOrder)
        {
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (!assetOrder.Contains(Path.GetFileName(file)))
                {
                    File.Delete(file);
                }
            }
        }
    }
    //数据层
    [Serializable]
    public class AssemblyData
    {
        public string assembly;
        public string func;
    }
    [Serializable]
    public class PushlishData
    {
        public string version;
        public string cdn;
        public string manifestAb;
        public string startPrefab;
        public AssemblyData startFunc = new AssemblyData();
        public List<string> preloadPrefab = new List<string>();
        public List<string> abOrder = new List<string>();
        public List<string> dllOrder = new List<string>();
        public List<AssetsOrder> assetsOrder = new List<AssetsOrder>();
    }
    [Serializable]
    public class AssetsOrder
    {
        public string key;
        public List<string> assetsItems = new List<string>();
    }
    //表现层
    [Serializable]
    public class UIDrawElement : ScriptableObject
    {
        public List<string> preloadPrefab = new List<string>();
        public List<string> abOrder = new List<string>();
        public List<string> dllOrder = new List<string>();
    }

}
