using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCanvasScale : MonoBehaviour
{
    Vector2 screenScale = new Vector2();
    Vector2 CanvasScaleV2 = new Vector2();
    CanvasScaler canvasScaler = null;
    float scaleFactor = 0; //比例系数
    //  public Text aaa;
    // Start is called before the first frame update
    void OnEnable()
    {
        var canvasScalerCom = transform.GetComponent<CanvasScaler>();
        canvasScalerCom.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScalerCom.referenceResolution = new Vector2(1920, 1080);
        canvasScalerCom.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScalerCom.matchWidthOrHeight = 0.5f;
        try
        {
            screenScale = SetTools.GetWindowSize(); //比例，x 高， y宽
            if (screenScale.x < screenScale.y)
            {
                scaleFactor = (float)screenScale.x / (float)screenScale.y;
            }
            else
            {
                scaleFactor = (float)screenScale.y / (float)screenScale.x;
            }
            canvasScaler = GetComponent<CanvasScaler>();
            CanvasScaleV2 = canvasScaler.referenceResolution;
            CanvasScaleV2.y = CanvasScaleV2.x * scaleFactor;
            int weight = (int)Mathf.Round(CanvasScaleV2.x);
            int height = (int)Mathf.Round(CanvasScaleV2.y);
            SetTools.ResetABCanvasSize(weight, height);
            int weightB = Screen.width;
            int heightB = Screen.height;
            //  aaa.text = " scaleFactor " + scaleFactor + "  screenScale: " + screenScale + "   CanvasScaleV2: " + CanvasScaleV2 + "  canvasScaler.referenceResolution: " + canvasScaler.referenceResolution + "  weight: " + weight + "  height: " + height + "   screenResolution:  " + "  weightB:" + weightB + "  heightB:" + heightB;
        }
        catch (System.Exception e)
        {
            //  aaa.text = "报错：" + e.ToString();
            Debug.Log("输出一下E的只： " + e.ToString());
        }
    }
}
