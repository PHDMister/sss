using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using WebP;

public class FrameHttp : MonoBehaviour
{
    public int nowDownCount = 0;
    public int allDownCount = 0;

    List<string> haveFrameData = new List<string>();
    List<FrameData> DownFrameData = new List<FrameData>();
    private static FrameHttp _instance = null;

    public static FrameHttp Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_FrameHttp").AddComponent<FrameHttp>();
        }
        return _instance;
    }
    public IEnumerator GetPostAction()
    {
        HttpRequest httpRequest = new HttpRequest();
        PersonData personData = new PersonData(ManageMentClass.DataManagerClass.landId);
        string data = JsonConvert.SerializeObject(personData);
        StartCoroutine(httpRequest.PostRequest("/aiera/v1/game/metaverse/decorative/painting/list", ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("加载框的时候的值： " + jo);
            if (jo["code"].ToString() == "0")
            {
                // 如果为0，则有效，返回数据成功
                var FrameData = jo["list"];
                foreach (var item in FrameData)
                {
                    Debug.Log("房间的藏品内容： " + item);
                    FrameData frame = new FrameData();


                    string id = item["id"].ToString();
                    int serial_no = (int)item["serial_no"];
                    string picture = item["picture"].ToString();
                    string product_id = item["product_id"].ToString();
                    string nft_product_size_id = item["nft_product_size_id"].ToString();

                    string OnlyID = product_id + "_" + nft_product_size_id;

                    frame.id = id;
                    frame.serial_no = serial_no;
                    frame.product_id = product_id;
                    frame.nft_product_size_id = nft_product_size_id;
                    frame.picture = picture;
                    frame.OnlyID = OnlyID;


                    if (RoomFurnitureCtrl.Instance().allFurniturePictureItemData.ContainsKey(serial_no))
                    {
                        RoomFurnitureCtrl.Instance().allFurniturePictureItemData[serial_no].product_id = product_id;
                        RoomFurnitureCtrl.Instance().allFurniturePictureItemData[serial_no].id = id;
                        RoomFurnitureCtrl.Instance().allFurniturePictureItemData[serial_no].serial_no = serial_no;
                        RoomFurnitureCtrl.Instance().allFurniturePictureItemData[serial_no].OnlyID = OnlyID;
                    }
                    else
                    {
                        FurniturePictureItemData furniturePictureItem = new FurniturePictureItemData();
                        furniturePictureItem.serial_no = serial_no;
                        furniturePictureItem.id = id;
                        furniturePictureItem.product_id = product_id;
                        furniturePictureItem.OnlyID = OnlyID;
                        RoomFurnitureCtrl.Instance().allFurniturePictureItemData.Add(serial_no, furniturePictureItem);
                    }
                    Debug.Log("A输出一下具体的图片数据： " + JsonConvert.SerializeObject(frame));
                    if (SeachDownPictureIndexFun(haveFrameData, frame.OnlyID) == -1)
                    {
                        DownFrameData.Add(frame);
                    }
                    //位置内有的
                    haveFrameData.Add(OnlyID);
                }


                if (!ManageMentClass.DataManagerClass.is_Owner)
                {
                    nowDownCount = 0;
                    allDownCount = 0;
                    // 要下载所有藏品
                    StartCoroutine(DownLoadImages());
                }
                else
                {
                    StartCoroutine(GetPagePictureFun());
                }
            }
        }
    }
    /// <summary>
    /// 这个协程是用来获取藏品列表的
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetPagePictureFun()
    {
        nowDownCount = 0;
        allDownCount = 0;
        HttpRequest httpRequest = new HttpRequest();
        PageClass pageClass = new PageClass(ManageMentClass.DataManagerClass.pageValue, ManageMentClass.DataManagerClass.pageSize);
        ManageMentClass.DataManagerClass.pageValue += 1;
        string data = JsonConvert.SerializeObject(pageClass);
        Debug.Log("GameToken: " + ManageMentClass.DataManagerClass.tokenValue_Game + "  data:  " + data);
        StartCoroutine(httpRequest.PostRequest("/aiera/v1/game/metaverse/nft/list", ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出一下 ho的值 ：" + jo);
            if (jo["code"].ToString() == "0")
            {
                // 如果为0，则有效，返回数据成功
                var FrameData = jo["list"];
                foreach (var item in FrameData)
                {
                    FrameData frame = new FrameData();
                    frame.product_id = item["product_id"].ToString();
                    frame.nft_product_size_id = item["nft_product_size_id"].ToString();
                    frame.picture = item["picture"].ToString();
                    frame.OnlyID = frame.product_id + "_" + frame.nft_product_size_id;
                    if (SeachDownPictureIndexFun(haveFrameData, frame.OnlyID) == -1)
                    {
                        DownFrameData.Add(frame);
                    }
                }
                // 要下载所有藏品
                StartCoroutine(DownLoadImages());
            }
        }
    }
    public IEnumerator DownLoadImages()
    {
        if (DownFrameData.Count > 0)
        {
            ManageMentClass.DataManagerClass.playerIsHavePicture = true;
        }
        else
        {
            ManageMentClass.DataManagerClass.playerIsHavePicture = false;
        }
        if (ManageMentClass.DataManagerClass.playerIsHavePicture)
        {
            allDownCount = DownFrameData.Count;
            for (int i = 0; i < DownFrameData.Count; i++)
            {
                if (DownFrameData[i].picture != "")
                {
                    if (!RoomFurnitureCtrl.Instance().allFramePictureData.ContainsKey(DownFrameData[i].OnlyID))
                    {
                        string pictureUrl = DownFrameData[i].picture;
                        string urlExt = InterfaceHelper.GetUrlExtension(DownFrameData[i].picture);
                        if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png"))
                        {
                            pictureUrl = string.Format("{0}{1}", DownFrameData[i].picture, "?x-oss-process=style/200W");
                        }
                        UnityWebRequest www = UnityWebRequest.Get(pictureUrl);
                        //UnityWebRequest www = UnityWebRequestTexture.GetTexture(DownFrameData[i].picture);

                        string dataA = JsonConvert.SerializeObject(DownFrameData[i]);
                        Debug.Log("DownFrameData[i].picture的内容 : " + dataA);
                        yield return www.SendWebRequest();
                        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.Log("这里报错了： ");
                            nowDownCount++;
                        }
                        else
                        {
                            Texture2D myTexture = null;
                            Sprite createSprite = null;

                            if (urlExt.Equals(".jpg") || urlExt.Equals(".jpeg") || urlExt.Equals(".png") || urlExt.Equals(".webp"))
                            {
                                Error lError;
                                myTexture = Texture2DExt.CreateTexture2DFromWebP(www.downloadHandler.data, true, true, out lError);
                                if (lError == Error.Success)
                                {
                                    createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0, 0));
                                    // 在这里缓存了所有的图片
                                    if (RoomFurnitureCtrl.Instance().allFramePictureData.ContainsKey(DownFrameData[i].OnlyID))
                                    {
                                        RoomFurnitureCtrl.Instance().allFramePictureData[DownFrameData[i].product_id].product_id = DownFrameData[i].product_id;
                                        RoomFurnitureCtrl.Instance().allFramePictureData[DownFrameData[i].product_id].nft_product_size_id = DownFrameData[i].nft_product_size_id;
                                        RoomFurnitureCtrl.Instance().allFramePictureData[DownFrameData[i].product_id].picture = DownFrameData[i].picture;
                                        RoomFurnitureCtrl.Instance().allFramePictureData[DownFrameData[i].product_id].OnlyID = DownFrameData[i].OnlyID;
                                        RoomFurnitureCtrl.Instance().allFramePictureData[DownFrameData[i].product_id].frameSprite = createSprite;
                                    }
                                    else
                                    {
                                        //图片数据
                                        FramePictureData framePicture = new FramePictureData();
                                        framePicture.product_id = DownFrameData[i].product_id;
                                        framePicture.nft_product_size_id = DownFrameData[i].nft_product_size_id;
                                        framePicture.picture = DownFrameData[i].picture;
                                        framePicture.OnlyID = DownFrameData[i].OnlyID;
                                        framePicture.frameSprite = createSprite;
                                        RoomFurnitureCtrl.Instance().allFramePictureData.Add(framePicture.OnlyID, framePicture);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Webp Load Error : " + lError.ToString());
                                }
                            }
                            else
                            {
                                Debug.LogError("文件格式错误，输出一下错误的图片地址： " + DownFrameData[i].picture);
                            }
                            nowDownCount++;
                        }
                    }
                    else
                    {
                        nowDownCount++;
                    }
                }
                else
                {
                    nowDownCount++;
                    Debug.Log("输出一下 这个图片地址未找到，需要查找");
                }
            }
        }
    }
    public int SeachDownPictureIndexFun(List<string> pictureList, string onlyId)
    {

        for (int i = 0; i < pictureList.Count; i++)
        {
            if (pictureList[i] == onlyId)
            {
                return i;
            }
        }
        return -1;
    }
}
