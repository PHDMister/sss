using Newtonsoft.Json.Linq;
using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

public static class InterfaceHelper
{
    public static RectTransform JoySticktouchArea;

    public static void SetJoyStickState(bool bEnable)
    {
        ManageMentClass.DataManagerClass.IsControllerPlayer = bEnable;
        ManageMentClass.DataManagerClass.CameraController = bEnable;
    }
    /// <summary>
    /// 物体是否发光
    /// </summary>
    /// <param name="isLight"></param>
    public static void SetFurnitureLightFun(GameObject obj, bool isLight)
    {
        Transform[] allChild = obj.transform.GetComponentsInChildren<Transform>();
        Debug.Log("输出一下子物体的个数：： " + allChild.Length);
        foreach (Transform child in allChild)
        {
            if (isLight)
            {
                if (child.gameObject.layer != 9)
                {
                    child.gameObject.layer = 7;
                }
            }
            else
            {
                if (child.gameObject.layer == 7)
                {
                    child.gameObject.layer = 1;
                }
            }
        }
    }


    /// <summary>
    ///  判断是否弹出强制退出游戏面板按钮
    /// </summary>
    public static void CalcGameOutFun(string codeValue)
    {
        if (codeValue == "900009" || codeValue == "110005")
        {
            SetJoyStickState(false);
            UIManager.GetInstance().ShowUIForms(FormConst.OUTGAMETIPPANEL);
        }
    }
    public static Rect GetSpaceRect(Canvas canvas, RectTransform rect, Camera camera)
    {
        Rect spaceRect = rect.rect;
        Vector3 spacePos = GetSpacePos(rect, canvas, camera);
        spaceRect.x = spaceRect.x * rect.lossyScale.x + spacePos.x;
        spaceRect.y = spaceRect.y * rect.lossyScale.y + spacePos.y;
        spaceRect.width = spaceRect.width * rect.lossyScale.x;
        spaceRect.height = spaceRect.height * rect.lossyScale.y;
        return spaceRect;
    }
    private static Vector3 GetSpacePos(RectTransform rect, Canvas canvas, Camera camera)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return rect.position;
        }
        return camera.WorldToScreenPoint(rect.position);
    }


    /// <summary>
    /// 获取ICon
    /// </summary>
    /// <param name="SpriteName"></param>
    /// <returns></returns>
    public static Sprite GetIconFun(string SpriteName)
    {
        // AllIconTextureData
        if (ManageMentClass.DataManagerClass.AllIconTextureData.ContainsKey(SpriteName))
        {
            Debug.Log("在这里获取图");
            return ManageMentClass.DataManagerClass.AllIconTextureData[SpriteName];
        }
        else
        {
            Sprite texture = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(SpriteName);
            Debug.Log("在这里A处获取图: " + texture.name);
            if (texture != null)
            {
                ManageMentClass.DataManagerClass.AllIconTextureData.Add(SpriteName, texture);
            }
            else
            {
                Debug.Log(" 图片是空的，名字为： " + SpriteName);
            }
            return texture;
        }
    }

    public static Sprite GetDefaultAvatarFun()
    {
        Sprite texture = Resources.Load("UIRes/Texture/Head/defaultAvatar", typeof(Sprite)) as Sprite;
        return texture;
    }

    /// <summary>
    /// 个人资料页默认图（藏品、盲盒等）
    /// </summary>
    /// <returns></returns>
    public static Sprite GetPersonDataDefaultFun()
    {
        Sprite texture = Resources.Load("UIRes/Texture/PersonData/DefaultChartImg", typeof(Sprite)) as Sprite;
        return texture;
    }

    /// <summary>
    /// 获取宠物商店Icon
    /// </summary>
    /// <returns></returns>
    public static Sprite GetPetShopIconFun(string SpriteName)
    {
        if (ManageMentClass.DataManagerClass.AllIconTextureData.ContainsKey(SpriteName))
        {
            return ManageMentClass.DataManagerClass.AllIconTextureData[SpriteName];
        }
        else
        {
            Sprite texture = ManageMentClass.ResourceControllerClass.ResLoadPetShopIconFun(SpriteName);
            if (texture != null)
            {
                ManageMentClass.DataManagerClass.AllIconTextureData.Add(SpriteName, texture);
            }
            else
            {
                Debug.Log(" 图片是空的，名字为： " + SpriteName);
            }
            return texture;
        }
    }

    /// <summary>
    /// 前往彩虹集市
    /// </summary>
    public static void GotoRainbowBazaarFun()
    {
        //点击了退出
        try
        {
            SetTools.SetPortraitModeFun();
            SetTools.CloseGameFun();
            string str = ManageMentClass.ModelLinkClass.GetRainBowBazaarData();
            SetTools.GoToRainbowBazaar(str);
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
    public static IEnumerator GotoRainbowAction()
    {

        yield return null;

    }

    /// <summary>
    /// 判断某一点是否在可触控范围（矩形）内，由TouchEvent调用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool inTouchStickArea(Vector2 pos)
    {
        Rect rect = JoySticktouchArea.rect;
        rect.x += JoySticktouchArea.position.x;
        rect.y += JoySticktouchArea.position.y;
        return rect.Contains(pos);
    }

    public static float CalcTextWidth(Text text)
    {
        TextGenerator tg = text.cachedTextGeneratorForLayout;
        TextGenerationSettings setting = text.GetGenerationSettings(Vector2.zero);
        float width = tg.GetPreferredWidth(text.text, setting) / text.pixelsPerUnit;
        return width;
    }

   




    /// <summary>
    /// 查找当前物体的子物体包含的所有材质球
    /// </summary>
    /// <param name="parent">需要查找的物体</param>
    /// <returns>返回获取到的所有材质球</returns>
    public static List<Material> FindChildMaterials(this Transform parent)
    {
        List<Material> t = new List<Material>();

        Find<Material>(parent, ref t);

        return t;
    }
    private static void Find<T>(Transform parent, ref List<Material> list) where T : UnityEngine.Object
    {
        if (parent.GetComponent<MeshRenderer>())
        {
            Material[] m = parent.GetComponent<MeshRenderer>().materials;
            if (m != null && m.Length > 0)
            {
                foreach (var item in m)
                {
                    list.Add(item);
                }
            }
        }
        if (parent.GetComponent<SkinnedMeshRenderer>())
        {
            Material[] m = parent.GetComponent<SkinnedMeshRenderer>().materials;
            if (m != null && m.Length > 0)
            {
                foreach (var item in m)
                {
                    list.Add(item);
                }
            }
        }
        int number = parent.childCount;
        while (number > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).childCount > 0) Find<T>(parent.GetChild(i), ref list);
                number--;
                if (parent.GetChild(i).GetComponent<MeshRenderer>())
                {
                    Material[] m = parent.GetChild(i).GetComponent<MeshRenderer>().materials;
                    if (m != null && m.Length > 0)
                    {
                        foreach (var item in m)
                        {
                            list.Add(item);
                        }
                    }
                }
                if (parent.GetChild(i).GetComponent<SkinnedMeshRenderer>())
                {
                    Material[] m = parent.GetChild(i).GetComponent<SkinnedMeshRenderer>().materials;
                    if (m != null && m.Length > 0)
                    {
                        foreach (var item in m)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
        }
    }

    public static List<Material> FindChildSkinMeshRendererMaterials(this Transform parent)
    {
        List<Material> t = new List<Material>();

        FindSkinMeshRender<Material>(parent, ref t);

        return t;
    }

    private static void FindSkinMeshRender<T>(Transform parent, ref List<Material> list) where T : UnityEngine.Object
    {
        if (parent.GetComponent<SkinnedMeshRenderer>())
        {
            Material[] m = parent.GetComponent<SkinnedMeshRenderer>().materials;
            if (m != null && m.Length > 0)
            {
                foreach (var item in m)
                {
                    list.Add(item);
                }
            }
        }
        int number = parent.childCount;
        while (number > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).childCount > 0) FindSkinMeshRender<T>(parent.GetChild(i), ref list);
                number--;
                if (parent.GetChild(i).GetComponent<SkinnedMeshRenderer>())
                {
                    Material[] m = parent.GetChild(i).GetComponent<SkinnedMeshRenderer>().materials;
                    if (m != null && m.Length > 0)
                    {
                        foreach (var item in m)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
        }
    }

    public static void ChangeLayer(Transform transform, int layer)
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ChangeLayer(transform.GetChild(i), layer);
            }
            transform.gameObject.layer = layer;
        }
        else
        {
            transform.gameObject.layer = layer;
        }
    }


    public static string GetUrlExtension(string url)
    {
        Uri uri = new Uri(url);
        //var filename = HttpUtility
        //.UrlDecode(uri.Segments.Last());
        var filename = HttpUtilityInterface.UrlDecode(uri.Segments.Last());
        string ext = Path.GetExtension(filename);
        return ext;
    }

    public static decimal TruncateDecimal(decimal value, int precision)
    {
        decimal step = (decimal)Math.Pow(10, precision);
        decimal tmp = Math.Truncate(step * value);
        return tmp / step;
    }

    public static string GetCoinDisplay(int coinValue)
    {
        if (coinValue < 1000)
        {
            return coinValue.ToString();
        }

        if (coinValue >= 1000 && coinValue < 1000000)
        {
            decimal newValue = TruncateDecimal((decimal)(coinValue / 1000f), 2);
            return string.Format("{0}K", newValue);
        }

        if (coinValue >= 1000000)
        {
            decimal newValue = TruncateDecimal((decimal)(coinValue / 1000000f), 2);
            return string.Format("{0}M", newValue);
        }
        return string.Empty;
    }
    
    /// <summary>
    /// 点击屏幕坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject GetFirstPickGameObject(Vector2 position)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;
        //射线检测ui
        List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, uiRaycastResultCache);
        if (uiRaycastResultCache.Count > 0)
            return uiRaycastResultCache[0].gameObject;
        return null;
    }

    /// <summary>
    /// 是否点击UI
    /// </summary>
    /// <returns></returns>
    public static bool IsClickUI()
    {
        GameObject obj = null;
#if !UNITY_EDITOR && UNITY_WEBGL
 if (ManageMentClass.DataManagerClass.PlatformType == 1)
        {
          obj = GetFirstPickGameObject(Input.mousePosition);
        if (obj == null)
            return false;
        return obj.layer == LayerMask.NameToLayer("UI");
        }
        else
        {
         bool bClickUI = false;
        if (Input.touchCount > 1)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                Touch touch = Input.GetTouch(i);
                obj = GetFirstPickGameObject(touch.position);
                if (obj != null)
                {
                    bClickUI = (obj.layer == LayerMask.NameToLayer("UI"));
                }
            }
        }
        else
        {
            obj = GetFirstPickGameObject(Input.GetTouch(0).position);
            if (obj == null)
                return false;
            bClickUI = obj.layer == LayerMask.NameToLayer("UI");
        }
        return bClickUI;
        }
       
#else
        obj = GetFirstPickGameObject(Input.mousePosition);
        if (obj == null)
            return false;
        return obj.layer == LayerMask.NameToLayer("UI");
#endif
    }

    public static bool bInRightHalfScreen()
    {
#if !UNITY_EDITOR && UNITY_WEBGL

 if (ManageMentClass.DataManagerClass.PlatformType == 1)
 {
  return Input.mousePosition.x >= Screen.width / 2;
 }
 else
 {
  bool bRight = false;
        if (Input.touchCount > 1)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.position.x >= Screen.width / 2)
                {
                    bRight = true;
                    break;
                }
            }
        }
        else
        {
            bRight = Input.GetTouch(0).position.x >= Screen.width / 2;
        }
        return bRight;
 }


       
#else
        return Input.mousePosition.x >= Screen.width / 2;
#endif
    }
}

