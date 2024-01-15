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
    /// 图片根目录 -- 需要打包图集的文件夹父级
    /// 适用目录结构：根部文件夹
    ///                -> 图片文件夹1
    ///                -> 图片文件夹2
    ///                ... 
    /// </summary>
    private static string pathRoot = Application.dataPath + "/Resources/UIRes/UISprite";
    protected static string pathRoot0 = Application.dataPath + "/Art/UISprite";
    protected static string pathRoot1 = Application.dataPath + "/Art_Island/UISprite";
    /// <summary>
    /// 图集存储路径
    /// </summary>Assets/CreateAtlas/Editor/Res/Textures
    private static string atlasStoragePath = "Assets/Resources/UIRes/Atlas/";

    /// <summary>
    /// 每个需要打图集的文件夹名 -- 即图集名
    /// </summary>
    private static string spritefilePathName;

    [MenuItem("Tools/打包图集")]
    public static void CreateAllSpriteAtlas()
    {
        Debug.Log("打包图集开始执行");
        List<string> Paths = new List<string>()
        {
            pathRoot,
            pathRoot0,
            pathRoot1
        };

        foreach (var path in Paths)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            int index = 0;
            // 遍历根目录
            foreach (DirectoryInfo item in info.GetDirectories())
            {
                spritefilePathName = item.Name;

                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(atlasStoragePath + "/" + spritefilePathName + ".spriteatlas", typeof(Object)) as SpriteAtlas;

                // 不存在则创建后更新图集
                if (spriteAtlas == null)
                {
                    spriteAtlas = CreateSpriteAtlas(spritefilePathName);
                }

                string spriteFilePath = path + "/" + spritefilePathName;
                UpdateAtlas(spriteAtlas, spriteFilePath);

                // 打包进度
                EditorUtility.DisplayProgressBar("打包图集中...", "正在处理:" + item, index / info.GetDirectories().Length);
                index++;
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        Debug.Log("打包图集执行结束");
    }

    /// <summary>
    /// 创建图集
    /// </summary>
    /// <param name="atlasName">图集名字</param>
    private static SpriteAtlas CreateSpriteAtlas(string atlasName)
    {
        SpriteAtlas atlas = new SpriteAtlas();

        #region 图集基础设置

        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 4,
        };
        atlas.SetPackingSettings(packSetting);

        #endregion

        #region 图集纹理设置

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        atlas.SetTextureSettings(textureSettings);

        #endregion

        #region 分平台设置图集格式

        //TextureImporterPlatformSettings platformSetting = atlas.GetPlatformSettings(GetPlatformName(BuildTarget.iOS));
        //platformSetting.overridden = true;
        //platformSetting.maxTextureSize = 2048;
        //platformSetting.textureCompression = TextureImporterCompression.Compressed;
        //platformSetting.format = TextureImporterFormat.PVRTC_RGB4;
        //atlas.SetPlatformSettings(platformSetting);

        // 需要多端同步，就在写一份
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
        platformSetting.format = TextureImporterFormat.DXT5;
        atlas.SetPlatformSettings(platformSetting);

        #endregion

        string atlasPath = atlasStoragePath + "/" + atlasName + ".spriteatlas";
        AssetDatabase.CreateAsset(atlas, atlasPath);
        AssetDatabase.SaveAssets();

        return atlas;
    }

    /// <summary>
    /// 每个图集的所有图片路径  --  记得用之前清空
    /// </summary>
    private static List<string> textureFullName = new List<string>();

    /// <summary>
    /// 更新图集内容
    /// </summary>
    /// <param name="atlas">图集</param>
    static void UpdateAtlas(SpriteAtlas atlas, string spriteFilePath)
    {
        textureFullName.Clear();
        FileName(spriteFilePath);

        // 获取图集下图片
        List<Object> packables = new List<Object>(atlas.GetPackables());

        foreach (string item in textureFullName)
        {
            // 加载指定目录
            Object spriteObj = AssetDatabase.LoadAssetAtPath(item, typeof(Object));
            Debug.Log("存png和jpg后缀的图片: " + item + " , " + !packables.Contains(spriteObj));
            if (!packables.Contains(spriteObj))
            {
                atlas.Add(new Object[] { spriteObj });
            }
        }
    }

    /// <summary>
    /// 递归文件夹下的图
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
            // 存png和jpg后缀的图片
            if (item.FullName.EndsWith(".png", StringComparison.Ordinal)
                || item.FullName.EndsWith(".jpg", StringComparison.Ordinal))
            {
                string dataPath = Application.dataPath.Replace("/", @"\");
                textureFullName.Add("Assets" + item.FullName.Replace(dataPath, ""));
            }
        }
    }

    /// <summary>
    /// 不同平台枚举对应的值
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