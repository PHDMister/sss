using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ConvertTexturesToastc : EditorWindow
{

    public enum TextureSize
    {
        x32 = 32,
        x64 = 64,
        x128 = 128,
        x256 = 256,
        x512 = 512,
        x1024 = 1024,
        x2048 = 2048,
        x4096 = 4096
    }


    private List<string> folderPaths = new List<string>();

    private TextureSize curSetSize = TextureSize.x512;
    private TextureImporterFormat curFormart = TextureImporterFormat.ASTC_12x12;
    private BuildTarget curTarget = BuildTarget.WebGL;


    [MenuItem("Tools/打开设置文件夹图片格式窗口 ")]
    private static void Init()
    {
        GetWindow<ConvertTexturesToastc>("Convert Textures to ASTC 12x12");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Folders to Convert", EditorStyles.boldLabel);
        // Add a dropdown to select the texture size
        GUILayout.BeginVertical();
        GUILayout.Label("Texture Size: ", GUILayout.Width(100));
        curSetSize = (TextureSize)EditorGUILayout.EnumPopup(curSetSize);

        GUILayout.Label("Texture Format: ", GUILayout.Width(100));
        curFormart = (TextureImporterFormat)EditorGUILayout.EnumPopup(curFormart);

        GUILayout.Label("Texture Target: ", GUILayout.Width(100));
        curTarget = (BuildTarget)EditorGUILayout.EnumPopup(curTarget);

        GUILayout.EndVertical();



        GUILayout.Space(20);
        // 显示添加文件夹的按钮
        if (GUILayout.Button("Add Folder"))
        {
            var folderPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            if (string.IsNullOrEmpty(folderPath))
                return;
            if (!folderPaths.Contains(folderPath))
                folderPaths.Add(folderPath);
        }

        // 显示已经添加的文件夹
        for (int i = 0; i < folderPaths.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.TextField(folderPaths[i]);
            if (GUILayout.Button("Remove"))
            {
                folderPaths.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        // 显示转换按钮
        if (GUILayout.Button("Convert"))
        {
            ConvertTextures();
        }
    }

    private void ConvertTextures()
    {
        // 转换所有添加的文件夹内的图片资源
        List<string> texturePaths = new List<string>();
        foreach (var folderPath in folderPaths)
        {
            string[] paths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".tga"))
                .Where(s => !s.EndsWith(".meta"))
                .ToArray();
            texturePaths.AddRange(paths);
        }

        int count = 0;
        foreach (var path in texturePaths)
        {
            string assetpath = GetAssetName(path);
            EditorUtility.DisplayCancelableProgressBar("ConvertTextures ", assetpath, count * 1.0f / texturePaths.Count);
            count++;
            var textureImporter = AssetImporter.GetAtPath(assetpath) as TextureImporter;
            if (textureImporter)
            {
                textureImporter.textureCompression = TextureImporterCompression.Compressed;
                textureImporter.compressionQuality = (int)TextureCompressionQuality.Normal;
                textureImporter.SetPlatformTextureSettings(curTarget.ToString(), (int)curSetSize, curFormart);
                textureImporter.SaveAndReimport();
                Debug.Log("Texture " + assetpath + " was converted to ASTC " + curFormart);
            }
            else
            {
                Debug.LogError("不存在文件 " + assetpath);
            }


        }
        EditorUtility.ClearProgressBar();


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    string GetAssetName(string path)
    {
        return path.Remove(0, path.IndexOf("Assets"));
    }
}
