using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor.SceneManagement;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

public class BuildTool
{
    private static string dataManager;
    private static BuildParam buildParam;

    private static void SetBuildParam()
    {
        var path = Application.dataPath + "/Scripts/Manager/DataManager.cs";
        dataManager = File.ReadAllText(path);

        if (string.IsNullOrEmpty(dataManager))
        {
            throw new Exception("dataManager is null");
        }

        buildParam = ParseBuildParam();

        SetName(buildParam.packageName);
        SetBuildVersion(buildParam.version);
        SetBuildType(buildParam.type);
        SetLogEnable(buildParam.log);

        SetWater("Island01", "Water");
        SetWater("fish", "Water/WaterWave");

        File.WriteAllText(path, dataManager, System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();
    }

    private static BuildParam ParseBuildParam()
    {
        var buildParam = new BuildParam();
        string[] parameters = Environment.GetCommandLineArgs();
        foreach (string str in parameters)
        {
            if (str.StartsWith("name"))
            {
                buildParam.packageName = parseSetting(str);
            }
            else if (str.StartsWith("type"))
            {
                buildParam.type = parseSetting(str);
            }
            else if (str.StartsWith("version"))
            {
                buildParam.version = parseSetting(str);
            }
            else if (str.StartsWith("log"))
            {
                buildParam.log = parseSetting(str);
            }
        }
        return buildParam;
    }

    public static void Build()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        //重新打ab
        //ExportAssetBundle.Run();

        string[] parameters = Environment.GetCommandLineArgs();
        var path = "";
        foreach (string str in parameters)
        {
            if (str.StartsWith("path"))
            {
                path = parseSetting(str);
                break;
            }
        }

        Debug.Log("BuildTool Build: " + path);
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("path is null");
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetBuildScene(),
            locationPathName = path,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    private static string[] GetBuildScene()
    {
        List<string> scenePath = new List<string>();
        EditorBuildSettingsScene[] editorBuildSettingsScene = EditorBuildSettings.scenes;
        if (editorBuildSettingsScene != null)
        {
            foreach (var scene in editorBuildSettingsScene)
            {
                scenePath.Add(scene.path);
            }
        }
        return scenePath.ToArray();
    }

    private static string parseSetting(string str)
    {
        var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
        if (tempParam.Length == 2)
        {
            return tempParam[1].Trim();
        }

        throw new Exception("param length exception: " + str);
    }

    private static void SetName(string name)
    {
        Debug.Log("BuildTool SetName: " + name);
    }

    private static void SetBuildType(string type)
    {
        Debug.Log("BuildTool SetBuildType: " + type);
        if (type == "dev")
        {
            dataManager = ReplaceFieldValue<bool>(dataManager, "isLinkEdition", "true");
            dataManager = ReplaceFieldValue<bool>(dataManager, "isOfficialEdition", "false");
        }
        else if (type == "local")
        {
            dataManager = ReplaceFieldValue<bool>(dataManager, "isLinkEdition", "true");
            dataManager = ReplaceFieldValue<bool>(dataManager, "isOfficialEdition", "false");
        }
        else if (type == "online")
        {
            dataManager = ReplaceFieldValue<bool>(dataManager, "isLinkEdition", "true");
            dataManager = ReplaceFieldValue<bool>(dataManager, "isOfficialEdition", "true");
        }
    }
    //private static void AddDefine(string define)
    //{
    //    PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, out var defines);
    //    var list = defines.ToList();
    //    if (!list.Contains(define))
    //    {
    //        list.Add(define);
    //    }
    //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, list.ToArray<string>());
    //}

    //private static void RemoveDefine(string define)
    //{
    //    PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, out var defines);
    //    var list = defines.ToList();
    //    if (list.Contains(define))
    //    {
    //        list.Remove(define);
    //    }
    //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, list.ToArray<string>());
    //}

    //public static string GetLocalIP()
    //{
    //    IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (IPAddress item in IpEntry.AddressList)
    //    {
    //        if (item.AddressFamily == AddressFamily.InterNetwork)
    //        {
    //            return item.ToString();
    //        }
    //    }
    //    return "";
    //}

    private static void SetBuildVersion(string version)
    {
        Debug.Log("BuildTool setBuildVersion: " + version);
        //设置打包版本
        if (buildParam.type == "online")
        {
            dataManager = ReplaceFieldValue<string>(dataManager, "OfficialVersionNumber", version);
        }
        else { 
            dataManager = ReplaceFieldValue<string>(dataManager, "TestVersionNumber", version);
        }
    }

    private static string ReplaceFieldValue<T>(string cs, string fieldName, string value)
    {
        if (typeof(T) == typeof(string)) {
            value = "\"" + value + "\"";
        };
        var index = cs.IndexOf(fieldName);
        var index1 = cs.IndexOf("=", index) + 1;
        var index2 = cs.IndexOf(";", index);
        var temp = cs.Remove(index1, (index2 - index1));
        return temp.Insert(index1, " " + value);
    }

    private static void SetLogEnable(string enable)
    {
        Debug.Log("BuildTool setLogEnable: " + enable);
        if (enable == "true")
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/LoginLoading.unity");
            if (scene == null)
            {
                throw new Exception("LoaginLoading.unity is null");
            }
            ReporterEditor.CreateReporter();
            var reporter = GameObject.Find("Reporter");
            if (reporter == null) { 
                throw new Exception("reporter is null");
            }
            var script = reporter.GetComponent<Reporter>();
            script.numOfCircleToShow = 5;

            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void SetWater(string sceneName, string waterObjName) {
        Debug.Log($"BuildTool {sceneName} SetWater: {waterObjName}");
        var scene = EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
        if (scene == null)
        {
            throw new Exception($"{sceneName}.unity is null");
        }
        var water = GameObject.Find(waterObjName);
        if (water == null)
        {
            throw new Exception($"{waterObjName} is null");
        }
        var material = water.GetComponent<MeshRenderer>().sharedMaterial;
        material.SetFloat("_ISBULID", 1f);
        material.EnableKeyword("_ISBULID");

        EditorSceneManager.SaveScene(scene);
    }

    //[MenuItem("BuildTool/test")]
    //private static void test()
    //{
    //    SetWater("fish", "Water/WaterWave");
    //}
}
public class BuildParam
{
    public string packageName;
    public string type;
    public string version;
    public string log;
}