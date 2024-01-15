using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public class CreateAtlas : MonoBehaviour
{
    /// <summary>
    /// ͼƬ��Ŀ¼ -- ��Ҫ���ͼ�����ļ��и���
    /// ����Ŀ¼�ṹ�������ļ���
    ///                -> ͼƬ�ļ���1
    ///                -> ͼƬ�ļ���2
    ///                ... 
    /// </summary>
    private static string pathRoot = Application.dataPath + "/UISprite";

    /// <summary>
    /// ͼ���洢·��
    /// </summary>Assets/CreateAtlas/Editor/Res/Textures
    private static string atlasStoragePath = "Assets/Resources/UIRes/Atlas/";

    /// <summary>
    /// ÿ����Ҫ��ͼ�����ļ����� -- ��ͼ����
    /// </summary>
    private static string spritefilePathName;

    [MenuItem("Tools/���ͼ��")]
    public static void CreateAllSpriteAtlas()
    {
        Debug.Log("���ͼ����ʼִ��");
        DirectoryInfo info = new DirectoryInfo(pathRoot);
        int index = 0;
        // ������Ŀ¼
        foreach (DirectoryInfo item in info.GetDirectories())
        {
            spritefilePathName = item.Name;

            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(atlasStoragePath + "/" + spritefilePathName + ".spriteatlas", typeof(Object)) as SpriteAtlas;

            // �������򴴽������ͼ��
            if (spriteAtlas == null)
            {
                spriteAtlas = CreateSpriteAtlas(spritefilePathName);
            }

            string spriteFilePath = pathRoot + "/" + spritefilePathName;
            UpdateAtlas(spriteAtlas, spriteFilePath);

            // �������
            EditorUtility.DisplayProgressBar("���ͼ����...", "���ڴ���:" + item, index / info.GetDirectories().Length);
            index++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        Debug.Log("���ͼ��ִ�н���");
    }

    /// <summary>
    /// ����ͼ��
    /// </summary>
    /// <param name="atlasName">ͼ������</param>
    private static SpriteAtlas CreateSpriteAtlas(string atlasName)
    {
        SpriteAtlas atlas = new SpriteAtlas();

        #region ͼ����������

        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 8,
        };
        atlas.SetPackingSettings(packSetting);

        #endregion

        #region ͼ����������

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        atlas.SetTextureSettings(textureSettings);

        #endregion

        #region ��ƽ̨����ͼ����ʽ

        //TextureImporterPlatformSettings platformSetting = atlas.GetPlatformSettings(GetPlatformName(BuildTarget.iOS));
        //platformSetting.overridden = true;
        //platformSetting.maxTextureSize = 2048;
        //platformSetting.textureCompression = TextureImporterCompression.Compressed;
        //platformSetting.format = TextureImporterFormat.PVRTC_RGB4;
        //atlas.SetPlatformSettings(platformSetting);

        // ��Ҫ���ͬ��������дһ��
        //platformSetting = atlas.GetPlatformSettings(GetPlatformName(BuildTarget.Android));
        //platformSetting.overridden = true;
        //platformSetting.maxTextureSize = 2048;
        //platformSetting.textureCompression = TextureImporterCompression.Compressed;
        //platformSetting.format = TextureImporterFormat.ASTC_6x6;
        //atlas.SetPlatformSettings(platformSetting);

        TextureImporterPlatformSettings platformSetting = atlas.GetPlatformSettings(GetPlatformName(BuildTarget.WebGL));
        platformSetting.overridden = true;
        platformSetting.maxTextureSize = 2048;
        platformSetting.textureCompression = TextureImporterCompression.Compressed;
        platformSetting.format = TextureImporterFormat.RGBA32;
        atlas.SetPlatformSettings(platformSetting);

        #endregion

        string atlasPath = atlasStoragePath + "/" + atlasName + ".spriteatlas";
        AssetDatabase.CreateAsset(atlas, atlasPath);
        AssetDatabase.SaveAssets();

        return atlas;
    }

    /// <summary>
    /// ÿ��ͼ��������ͼƬ·��  --  �ǵ���֮ǰ���
    /// </summary>
    private static List<string> textureFullName = new List<string>();

    /// <summary>
    /// ����ͼ������
    /// </summary>
    /// <param name="atlas">ͼ��</param>
    static void UpdateAtlas(SpriteAtlas atlas, string spriteFilePath)
    {
        textureFullName.Clear();
        FileName(spriteFilePath);

        // ��ȡͼ����ͼƬ
        List<Object> packables = new List<Object>(atlas.GetPackables());

        foreach (string item in textureFullName)
        {
            // ����ָ��Ŀ¼
            Object spriteObj = AssetDatabase.LoadAssetAtPath(item, typeof(Object));
            Debug.Log("��png��jpg��׺��ͼƬ: " + item + " , " + !packables.Contains(spriteObj));
            if (!packables.Contains(spriteObj))
            {
                atlas.Add(new Object[] { spriteObj });
            }
        }
    }

    /// <summary>
    /// �ݹ��ļ����µ�ͼ
    /// </summary>
    /// <param name="folderPath"></param>
    static void FileName(string folderPath)
    {
        DirectoryInfo info = new DirectoryInfo(folderPath);
        foreach (DirectoryInfo item in info.GetDirectories())
        {
            FileName(item.FullName);
        }
        foreach (FileInfo item in info.GetFiles())
        {
            // ��png��jpg��׺��ͼƬ
            if (item.FullName.EndsWith(".png", StringComparison.Ordinal)
                || item.FullName.EndsWith(".jpg", StringComparison.Ordinal))
            {
                string dataPath = Application.dataPath.Replace("/", @"\");
                textureFullName.Add("Assets" + item.FullName.Replace(dataPath, ""));
            }
        }
    }

    /// <summary>
    /// ��ͬƽ̨ö�ٶ�Ӧ��ֵ
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    static string GetPlatformName(BuildTarget target)
    {
        string platformName = "";
        switch (target)
        {
            case BuildTarget.Android:
                platformName = "Android";
                break;
            case BuildTarget.iOS:
                platformName = "iPhone";
                break;
            case BuildTarget.PS4:
                platformName = "PS4";
                break;
            case BuildTarget.XboxOne:
                platformName = "XboxOne";
                break;
            case BuildTarget.NoTarget:
                platformName = "DefaultTexturePlatform";
                break;
            case BuildTarget.WebGL:
                platformName = "WebGL";
                break;
            default:
                platformName = "Standalone";
                break;
        }
        return platformName;
    }
}