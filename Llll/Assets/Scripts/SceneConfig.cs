using System.Collections.Generic;

public class SceneConfig
{
    private static Dictionary<LoadSceneType, string> NameMap;

    static SceneConfig()
    {
        NameMap = new Dictionary<LoadSceneType, string>
        {
            { LoadSceneType.RainbowBeach, "彩虹沙滩" },
            { LoadSceneType.ShenMiHaiWan, "神秘海湾" },
            { LoadSceneType.HaiDiXingKong, "海底星空" },
        };
    }

    public static string GetName(int sceneId)
    {
        NameMap.TryGetValue((LoadSceneType)sceneId, out var sceneName);
        return sceneName;
    }

    public static string GetName(uint sceneId)
    {
        NameMap.TryGetValue((LoadSceneType)sceneId, out var sceneName);
        return sceneName;
    }
    
    public static string GetName(LoadSceneType sceneId)
    {
        NameMap.TryGetValue(sceneId, out var sceneName);
        return sceneName;
    }
}
