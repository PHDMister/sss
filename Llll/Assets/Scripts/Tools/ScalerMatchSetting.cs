using UnityEngine;
using UnityEngine.UI;

public class ScalerMatchSetting : MonoBehaviour
{
    public bool Debug = false; //开启会在Update调用，方便调试
    CanvasScaler scaler;
    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        AutoSetMatch();
    }

    private void AutoSetMatch()
    {
        if (scaler)
        {
            // 16 / 9 = 1.777777.....   判断  宽高比大于 1.78则视为分辨率大于 16:9
            float ratio = (float)Screen.width / (float)Screen.height;
            scaler.matchWidthOrHeight = ratio > 1.78 ? 1 : 0;
        }
    }

#if UNITY_EDITOR_WIN
    private void Update()
    {
        if (Debug)
        {
            AutoSetMatch();
        }
    }
#endif
}