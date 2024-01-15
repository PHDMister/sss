using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebP;
using UnityEngine.Networking;
using System;

public class DownloadImageManager : MonoBehaviour
{
    private static DownloadImageManager _Instance = null;
    public static DownloadImageManager GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_DownloadImageManager").AddComponent<DownloadImageManager>();
        }
        return _Instance;
    }
    //是否正在下载 / true为正在下载
    public bool IsDownloading = false;

    List<Sprite> listSprite = new List<Sprite>();

    private int nowDownCount = 0;
    private int allDownCount = 0;

    private Action<List<Sprite>> returnSpriteCallBack;
    private Queue<string> queueUrls;


    public void DownloadMoreImageFun(Queue<string> urls, Action<List<Sprite>> callBack)
    {

        nowDownCount = 0;
        allDownCount = urls.Count;
        IsDownloading = true;
        listSprite.Clear();
        queueUrls = urls;
        returnSpriteCallBack = null;
        returnSpriteCallBack = callBack;

        //开始下载
        DownLoadRecursionFun();
    }
    public void DownLoadRecursionFun()
    {
        if (queueUrls.Count > 0)
        {
            string url = queueUrls.Dequeue();
            StartCoroutine(DownloadImageFun(url));
        }
        else
        {
            returnSpriteCallBack?.Invoke(listSprite);
            IsDownloading = false;
        }
    }



    private IEnumerator DownloadImageFun(string url)
    {
        if (url != "")
        {
            Sprite createSprite = null;
            string urlExt = InterfaceHelper.GetUrlExtension(url);
            if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp"))
            {
                if (ManageMentClass.DataManagerClass.PlatformType == 2)
                {
                    url = string.Format("{0}{1}", url, "?x-oss-process=style/200W");
                }
                else
                {
                    url = string.Format("{0}{1}", url, "?x-oss-process=style/600W");
                }

                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("这里报错了： " + www.result);
                    nowDownCount++;
                    createSprite = InterfaceHelper.GetPersonDataDefaultFun();
                    listSprite.Add(createSprite);
                }
                else
                {
                    Error lError;
                    Texture2D myTexture = null;
                    myTexture = Texture2DExt.CreateTexture2DFromWebP(www.downloadHandler.data, true, true, out lError);
                    if (lError == Error.Success)
                    {
                        createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                        listSprite.Add(createSprite);
                    }
                    else
                    {
                        Debug.LogError("Webp Load Error : " + lError.ToString());
                        createSprite = InterfaceHelper.GetPersonDataDefaultFun();
                        listSprite.Add(createSprite);
                    }
                    nowDownCount++;
                }
            }
            else
            {
                Debug.LogError("文件格式错误，输出一下错误得图片地址： " + url);
                nowDownCount++;
                createSprite = InterfaceHelper.GetPersonDataDefaultFun();
                listSprite.Add(createSprite);
            }
        }
        else
        {
            Debug.Log("输出一下 这个图片地址未找到，需要查找");
            nowDownCount++;
            Sprite createSprite = null;
            createSprite = InterfaceHelper.GetPersonDataDefaultFun();
            listSprite.Add(createSprite);
        }
        DownLoadRecursionFun();
    }
}
