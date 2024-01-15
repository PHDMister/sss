using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class BubbleTipItem : MonoBehaviour
{
    private FurnitureRootData furnitureRootData;
    private Camera mainCamera;
    private RectTransform Tip;
    private Button Tip_Btn;
    private Image Tip_Img;
    private bool isController = false;
    public int idA;
    public event Action<FurnitureRootData> OnClickAction;

    public GameObject Image_Panel;

    public GameObject Hua_Image;
    public GameObject Furniture_Image;

   

    private void Awake()
    {
 

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Tip = transform.GetComponent<RectTransform>();
        Tip_Btn = transform.GetComponent<Button>();
        Tip_Img = transform.GetComponent<Image>();

        Tip_Btn.onClick.AddListener(OnClickFun);
    }
    private void OnEnable()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    public void initData(FurnitureRootData furnData, FurnitureType furnType)
    {
        isController = true;
        furnitureRootData = furnData;
        idA = furnData.PlaceID;
        Debug.Log("输出一下具体的PlaceID的值： " + furnData.PlaceID);
        if (furnType == FurnitureType.hua)
        {
            Hua_Image.SetActive(true);
            Furniture_Image.SetActive(false);
        }
        else
        {
            Hua_Image.SetActive(false);
            Furniture_Image.SetActive(true);
        }
        //开始刷新
        StartCoroutine(coroutineFunc());
    }
    IEnumerator coroutineFunc()
    {
        bool isShow = false;
        while (isController)
        {
            yield return new WaitForEndOfFrame();
            //  transform.position=
            isShow = false;
            if (ManageMentClass.DataManagerClass.CamerIdaData == null)
            {
                Image_Panel.SetActive(false);
                Tip_Btn.enabled = false;
                Tip_Img.enabled = false;
                break;
            }

            for (int i = 0; i < ManageMentClass.DataManagerClass.CamerIdaData.Length; i++)
            {
              
                if (furnitureRootData.PlaceID == ManageMentClass.DataManagerClass.CamerIdaData[i] + RoomFurnitureCtrl.Instance().GetStartIndex())
                {
                    isShow = true;
                    break;
                }
            }

            if (isShow)
            {
                Image_Panel.SetActive(true);
                Tip_Btn.enabled = true;
                Tip_Img.enabled = true;
            }
            else
            {
                Image_Panel.SetActive(false);
                Tip_Btn.enabled = false;
                Tip_Img.enabled = false;
            }


            Vector3 screenPos = mainCamera.WorldToScreenPoint(furnitureRootData.furnitureRootPos);
            Vector2 uiPos = new Vector2(screenPos.x - Screen.width / 2f, screenPos.y - Screen.height / 2f);
            Tip.anchoredPosition = uiPos;
            /*if (IsVisableInCamera)
            {
                Image_Panel.SetActive(true);
                Tip_Btn.enabled = true;
            }
            else
            {
                Image_Panel.SetActive(false);
                Tip_Btn.enabled = false;
            }*/
        }
    }
    private void OnDisable()
    {
        isController = false;
        StopCoroutine(coroutineFunc());
    }
    public bool IsVisableInCamera
    {
        get
        {
            Camera mCamera = Camera.main;
            Vector3 pos = furnitureRootData.furnitureRootPos;
            //转化为视角坐标
            Vector3 viewPos = mCamera.WorldToViewportPoint(pos);
            // z<0代表在相机背后
            if (viewPos.z < 0) return false;
            //太远了！看不到了！
            if (viewPos.z > mCamera.farClipPlane)
                return false;
            if (Vector3.Distance(pos, mainCamera.transform.position) > 10)
            {
                return false;
            }
            // x,y取值在 0~1之外时代表在视角范围外；
            if (viewPos.x < 0 || viewPos.y < 0 || viewPos.x > 1 || viewPos.y > 1) return false;
            return true;
        }
    }
    public void OnClickFun()
    {
        OnClickAction?.Invoke(furnitureRootData);
    }
}
