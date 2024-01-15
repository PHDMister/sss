using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;
using WebP;

public class Result<T>
{
    //是否完成
    public bool comPlete;
    //是否成功
    public bool succ;
    //数据内容
    public T value;
    //图片
    public List<Sprite> downList = new List<Sprite>();
}
public class HttpRequest
{
    string testHttpUrl = "https://test.24nyae.cn";
    string officalUrl = "https://game-api.juhaoqiang.com";
    Result<string> result = new Result<string>();
    public bool isComPlete => result.comPlete;
    public bool isSucc => result.succ;
    public string value => isSucc && isComPlete ? result.value : "";
    public List<Sprite> downPictureList => isSucc && isComPlete ? result.downList : null;
    public int nowDownValue = 0;
    public int allDownValue = 0;

    // 获取数据文件，写入本地
    public IEnumerator GetRequest(string url)
    {
        result.comPlete = false;
        if (string.IsNullOrEmpty(url))
        {
            // 如果为空的话，交互完成，但是属于失败状态
            result.comPlete = true;
            result.succ = false;
            yield break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            result.comPlete = true;
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                // 访问错误
                result.succ = false;
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("下载进度： " + webRequest.downloadProgress);
                var File = webRequest.downloadHandler.data;
                // 创建写入对象
                FileStream nFile = new FileStream(Application.dataPath + "/" + "git.jpg", FileMode.Create);
                Debug.Log("这里的路径值： " + Application.dataPath);
                nFile.Write(File, 0, File.Length);
                nFile.Close();
                //访问成功
                result.succ = true;
                result.value = webRequest.downloadHandler.text;
            }
        }
    }
    /// <summary>
    /// post请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="token"></param>
    /// <param name="JsonData"></param>
    /// <returns></returns>
    public IEnumerator PostRequest(string url, string token, string JsonData)
    {
        result.comPlete = false;
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(token))
        {
            // 如果为空的话，交互完成，但是属于失败状态

            Debug.Log("这里报错");

            result.comPlete = true;
            result.succ = false;
            yield break;
        }
        string finalUrl = "";
        if (ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            finalUrl = officalUrl + url;
        }
        else
        {
            finalUrl = testHttpUrl + url;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Post(finalUrl, JsonData))
        {
            webRequest.timeout = 30;
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            if (token != "")
            {
                webRequest.SetRequestHeader("token", token);
            }
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            yield return webRequest.SendWebRequest();
            result.comPlete = true;
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                // 错误
                result.succ = false;
                Debug.Log("错误内容： " + webRequest.error);
            }
            else
            {
                Debug.Log("输出一下内容： " + webRequest.result);
                //成功
                result.succ = true;
                result.value = webRequest.downloadHandler.text;
            }
        }
    }




    public IEnumerator PostRequest(string url, string JsonData)
    {
        result.comPlete = false;
        if (string.IsNullOrEmpty(url))
        {
            // 如果为空的话，交互完成，但是属于失败状态

            Debug.Log("这里报错");

            result.comPlete = true;
            result.succ = false;
            yield break;
        }
        string finalUrl = "";
        if (ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            finalUrl = officalUrl + url;
        }
        else
        {
            finalUrl = testHttpUrl + url;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Post(finalUrl, JsonData))
        {
            webRequest.timeout = 30;
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            yield return webRequest.SendWebRequest();
            result.comPlete = true;
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                // 错误
                result.succ = false;
                Debug.Log("错误内容： " + webRequest.error);
            }
            else
            {
                Debug.Log("输出一下内容： " + webRequest.result);
                //成功
                result.succ = true;
                result.value = webRequest.downloadHandler.text;
            }
        }
    }

    /// <summary>
    /// 下载多张照片
    /// </summary>
    /// <param name="urls"></param>
    /// <returns></returns>
    public IEnumerator DownloadImages(string[] urls)
    {
        result.comPlete = false;
        nowDownValue = 0;
        allDownValue = urls.Length;
        for (int i = 0; i < urls.Length; i++)
        {
            Debug.Log("输出URL的值： " + urls[i]);
            string urlExt = InterfaceHelper.GetUrlExtension(urls[i]);
            if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp"))
            {
                if (ManageMentClass.DataManagerClass.PlatformType == 2)
                {
                    urls[i] = string.Format("{0}{1}", urls[i], "?x-oss-process=style/100W");
                }
                else
                {
                    urls[i] = string.Format("{0}{1}", urls[i], "?x-oss-process=style/600W");
                }
                Debug.Log("内容值");
                UnityWebRequest www = UnityWebRequest.Get(urls[i]);
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    result.comPlete = true;
                    result.succ = false;
                    result.downList.Add(null);
                    Debug.Log(www.error);
                    nowDownValue++;
                }
                else
                {
                    if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp"))
                    {
                        Error lError;
                        Texture2D myTexture = Texture2DExt.CreateTexture2DFromWebP(www.downloadHandler.data, true, true, out lError);
                        if (lError == Error.Success)
                        {
                            Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                            result.comPlete = true;
                            result.succ = true;
                            nowDownValue++;
                            result.downList.Add(createSprite);
                        }
                        else
                        {
                            Debug.LogError("Webp Load Error : " + lError.ToString());
                        }
                    }
                    else
                    {
                        Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                        result.comPlete = true;
                        result.succ = true;
                        nowDownValue++;
                        result.downList.Add(createSprite);
                    }
                    //  imageA.GetComponent<Image>().sprite = createSprite;
                }
            }
            else
            {
                Debug.Log("进来了这个来了，看一下具体内容");
                //是用默认头像
                Sprite texture = InterfaceHelper.GetDefaultAvatarFun();
                result.downList.Add(texture);
                result.succ = true;
                result.comPlete = true;
                nowDownValue++;
            }
        }
    }

    /// <summary>
    /// 下载单张图片
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerator DownloadImage(string url)
    {
        result.comPlete = false;
        Debug.Log("输出URL的值： " + url);
        string urlExt = InterfaceHelper.GetUrlExtension(url);
        if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp") || urlExt.Equals(""))
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
                result.comPlete = true;
                result.succ = false;
                result.downList.Add(null);
                Debug.Log(www.error);
            }
            else
            {
                if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp") || urlExt.Equals(""))
                {
                    Error lError;
                    Texture2D myTexture = Texture2DExt.CreateTexture2DFromWebP(www.downloadHandler.data, true, true, out lError);
                    if (lError == Error.Success)
                    {
                        Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                        result.comPlete = true;
                        result.succ = true;
                        result.downList.Add(createSprite);
                    }
                    else
                    {
                        result.comPlete = true;
                        result.succ = false;
                        result.downList.Add(null);
                        Debug.LogError("Webp Load Error : " + lError.ToString());
                    }
                }
                else
                {
                    Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                    result.comPlete = true;
                    result.succ = true;
                    nowDownValue++;
                    result.downList.Add(createSprite);
                }
            }
        }
        else
        {
            result.comPlete = true;
            result.succ = false;
            result.downList.Add(null);
        }

    }

}
