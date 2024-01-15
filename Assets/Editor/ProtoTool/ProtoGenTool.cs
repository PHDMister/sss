using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using System.IO;


public class ProtoGenTool : EditorWindow
{
    [MenuItem("Tools/Proto2CS")]
    public static void GenProto2CS()
    {
        ProtoGenTool openGenTool = EditorWindow.GetWindow<ProtoGenTool>();
        openGenTool.titleContent = new GUIContent("Proto2cs");
        openGenTool.minSize = new Vector2(500, 300);
        openGenTool.Show();
    }



    private string outFilePath;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("ProtoPath:");
        if (string.IsNullOrEmpty(outFilePath))
        {
            outFilePath = EditorPrefs.GetString("ProtoGenTool_outFilePath", "");
        }
        outFilePath = GUILayout.TextField(outFilePath);
        if (GUILayout.Button("选择proto文件", GUILayout.Height(50)))
        {
            if (string.IsNullOrEmpty(outFilePath)) outFilePath = Application.dataPath;
            outFilePath = EditorUtility.OpenFilePanel("选择需要生成的Proto", outFilePath, "*.*");
            EditorPrefs.SetString("ProtoGenTool_outFilePath", outFilePath);
        }
        //检查脚本挂载脚本
        if (GUILayout.Button("Proto2cS", GUILayout.Height(50)))
        {
            if (string.IsNullOrEmpty(outFilePath))
            {
                UnityEngine.Debug.LogError("[Proto2cS] 文件路径为空！！！！！");
                return; 
            }

            string fileName = Path.GetFileName(outFilePath);
            if (!fileName.Contains(".proto"))
            {
                UnityEngine.Debug.LogError($"[Proto2cS] {fileName} 不是proto文件 ！！！！");
                return; 
            }

            OnGenBtnClickEnable(fileName);
        }

        EditorGUILayout.EndVertical();
    }

    private void OnGenBtnClickEnable(string fileName)
    {
        string rootDir = Environment.CurrentDirectory;
        string protoDir = Path.Combine(rootDir, "Proto/");
        string protoc;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            protoc = Path.Combine(protoDir, "protoc.exe");
        }
        else
        {
            protoc = Path.Combine(protoDir, "protoc");
        }

        string hotfixMessageCodePath = Path.Combine(rootDir, "Assets", "Scripts", "ProtoMessage/");

        string argument2 = $"--csharp_out=\"{hotfixMessageCodePath}\" --proto_path=\"{protoDir}\" {fileName}";

        Run(protoc, argument2, waitExit: true);

        UnityEngine.Debug.Log("proto2cs succeed!");

        AssetDatabase.Refresh();
    }

    public static Process Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false)
    {
        try
        {
            bool redirectStandardOutput = true;
            bool redirectStandardError = true;
            bool useShellExecute = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                redirectStandardOutput = false;
                redirectStandardError = false;
                useShellExecute = true;
            }

            if (waitExit)
            {
                redirectStandardOutput = true;
                redirectStandardError = true;
                useShellExecute = false;
            }

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = useShellExecute,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = redirectStandardOutput,
                RedirectStandardError = redirectStandardError,
            };

            Process process = Process.Start(info);

            if (waitExit)
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception($"{process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
                }
            }

            return process;
        }
        catch (Exception e)
        {
            throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
        }
    }
}
