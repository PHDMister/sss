using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneFurnitureIndex
{
    //客厅
    parlor = 1,
    //卧室
    bedRoom = 28,
}

public class RoomFurnitureCtrl : MonoBehaviour
{
    private static RoomFurnitureCtrl _instance = null;

    //所有家具根节点数据
    public Dictionary<int, FurnitureRootData> allFurnitureRootData = new Dictionary<int, FurnitureRootData>();

    //房间各个位置对应放置的家具 （ 位置编号，数据）
    public Dictionary<int, FurniturePlaceData> furniturePlaceDicData = new Dictionary<int, FurniturePlaceData>();

    //更改前初始的房间各个位置对应放置的家具 （ 位置编号，数据） 
    public Dictionary<int, int> furnitureTemporaryPlaceDicData = new Dictionary<int, int>();

    //更改后初始的房间各个位置对应放置的家具 （ 位置编号，数据） 
    public Dictionary<int, int> furnitureTemporaryAfterPlaceDicData = new Dictionary<int, int>();

    //当前获取得家具类型的数据
    public Dictionary<FurnitureType, List<FurntureHasDataClass>> furntureHasData = new Dictionary<FurnitureType, List<FurntureHasDataClass>>();

    /// <summary>
    /// 藏品画的数据
    /// </summary>
    //画框（位置，画框）
    public Dictionary<int, FurniturePictureItemData> allFurniturePictureItemData = new Dictionary<int, FurniturePictureItemData>();

    //所有下载下来的画的数据
    public Dictionary<string, FramePictureData> allFramePictureData = new Dictionary<string, FramePictureData>();

    //临时更改藏品的数据
    public Dictionary<int, string> allTemporaryPlacePictureData = new Dictionary<int, string>();

    //放光的临时物体
    public List<PlaceGameObj> arrTemporaryLight = new List<PlaceGameObj>();

    //临时的类型数据
    public List<FurnitureType> arrPostFurnitureTypeData = new List<FurnitureType>();
    // 镜头数据
    public Dictionary<int, CameraValueData> CameraValueDataDic = new Dictionary<int, CameraValueData>();

    /// <summary>
    /// 卧室额外同步的角色
    /// </summary>
    public GameObject extraObj = null;


    //人物初始位置和角度
    public Vector3 startPos;
    public Vector3 startEulerAngles;

    /// <summary>
    /// 所有家具的根节点
    /// </summary>
    private GameObject furnitureNode;
    /// <summary>
    /// 所有家具的位置根节点
    /// </summary>
    private GameObject furniturePosNode;

    public static RoomFurnitureCtrl Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject().AddComponent<RoomFurnitureCtrl>();
        }
        return _instance;
    }
    private Transform _transform = null;
    private void Awake()
    {
        _transform = this.transform;
        DontDestroyOnLoad(_transform);
    }
    /// <summary>
    /// 初始化赋值（进场景调用赋值）
    /// </summary>
    /// <param name="rootNode"></param>
    /// <param name="posNode"></param>
    public void InitNodeFun(GameObject rootNode, GameObject posNode)
    {
        furnitureNode = rootNode;
        furniturePosNode = posNode;
    }



    /// <summary>
    /// 获取本场景所有的家具位置节点（初始化进场景就调用）
    /// </summary>
    public void GetAllFurnitureNodeFun()
    {
        allFurnitureRootData.Clear();
        if (furnitureNode != null)
        {
            for (int i = 0; i < furnitureNode.transform.childCount; i++)
            {
                FurnitureRootData rootData = new FurnitureRootData();
                rootData.furnitureRoot = furnitureNode.transform.GetChild(i).gameObject;
                rootData.PlaceID = i + GetStartIndex();
                if (furniturePosNode && furniturePosNode.transform.GetChild(i))
                {
                    rootData.furnitureRootPos = furniturePosNode.transform.GetChild(i).position;
                }
                if (rootData == null)
                {
                    Debug.Log("暴恐");
                }
                FurnitureType furnitureType = SetFurnitureRootType(rootData.furnitureRoot.name);
                rootData.furnitureType = furnitureType;
                allFurnitureRootData.Add(i + GetStartIndex(), rootData);
                Debug.Log("输出一下这里对应的具体的值： " + (i + GetStartIndex()));
                SetFurniturePictureFun(rootData.furnitureType, rootData.furnitureRoot, i + GetStartIndex());
            }
        }
    }



    /// <summary>
    /// 初始家具生成
    /// </summary>
    public void SetStartFurnitureFun()
    {
        for (int i = 0; i < furnitureNode.transform.childCount; i++)
        {
            int placeID = i + GetStartIndex();
            if (furniturePlaceDicData.ContainsKey(placeID))
            {
                if (furniturePlaceDicData[placeID].furnitureId != 0)
                {
                    //初始化家具

                    StartSetFunitureFun(furniturePlaceDicData[placeID].furnitureId, placeID);
                }
            }
            else
            {
                FurniturePlaceData placeData = new FurniturePlaceData();
                placeData.furnitureId = 0;
                furniturePlaceDicData.Add(placeID, placeData);
            }
        }
    }
    /// <summary>
    /// 初始化藏品
    /// </summary>
    public void SetStartPictureFun()
    {
        for (int i = 0; i < furnitureNode.transform.childCount; i++)
        {
            int placeID = i + GetStartIndex();
            if (allFurniturePictureItemData.ContainsKey(placeID))
            {
                if (allFurniturePictureItemData[placeID].product_id != null)
                {
                    string OnlyID = allFurniturePictureItemData[placeID].OnlyID;
                    if (allFramePictureData.ContainsKey(OnlyID))
                    {
                        StartSetPictureFun(allFramePictureData[OnlyID], placeID);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 获取各场景起始位置
    /// </summary>
    /// <returns></returns>
    public int GetStartIndex()
    {
        int index = 1;
        switch (SceneManager.GetActiveScene().name)
        {
            case "gerenkongjian01":
                index = (int)SceneFurnitureIndex.parlor;
                break;
            case "bedroom01":
                index = (int)SceneFurnitureIndex.bedRoom;
                break;
            default:
                index = 1;
                break;
        }
        return index;
    }




    /// <summary>
    /// 一键装修所有家具
    /// </summary>
    public void OneKeyFinishAllFrameFun()
    {
        var keys = allFurnitureRootData.Keys;
        Debug.Log("key的长度： " + keys.Count);

        foreach (var key in keys)
        {
            Debug.Log("Key的名字： " + key.ToString() + "  allFurnitureRootData[key].furnitureType :  " + allFurnitureRootData[key].furnitureType);
            if (allFurnitureRootData[key].furnitureType != FurnitureType.hua && allFurnitureRootData[key].furnitureType != FurnitureType.None)
            {
                // 拉取所有的家具
                int furnID = SeachAllMaxFurnitureIDFun(allFurnitureRootData[key].furnitureType);
                Debug.Log("输出一下这个seachallmaFUniture ID 的值： " + furnID);
                if (furnID != -1)
                {
                    //换家具
                    if (furniturePlaceDicData.ContainsKey(key))
                    {
                        if (furniturePlaceDicData[key].furnitureId <= 0)
                        {
                            Debug.Log("输出一下在判断的值 A： " + furniturePlaceDicData[key].furnitureId);
                            ChangeFurnitureFun(furnID, key, true);
                        }
                    }
                    else
                    {
                        Debug.Log("输出一下在判断的值 B： " + furnID);
                        ChangeFurnitureFun(furnID, key, true);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 一键收起所有的家具
    /// </summary>
    public void OneKeyPickUpAllFrameFun()
    {
        var keys = allFurnitureRootData.Keys;
        foreach (var key in keys)
        {
            if (allFurnitureRootData[key].furnitureType != FurnitureType.hua && allFurnitureRootData[key].furnitureType != FurnitureType.None)
            {
                PickUpFrameFun(key, allFurnitureRootData[key].furnitureType, true);
            }
        }
    }


    /// <summary>
    /// 初始化设置家具
    /// </summary>
    public void StartSetFunitureFun(int furnitureId, int placeId)
    {
        FurnitureItemData itemData = GetFuritureFun(furnitureId);
        itemData.furnitureObj.SetActive(true);
        itemData.furnitureObj.transform.SetParent(allFurnitureRootData[placeId].furnitureRoot.transform);
        itemData.furnitureObj.transform.localPosition = Vector3.zero;
        if (furniturePlaceDicData.ContainsKey(placeId))
        {
            furniturePlaceDicData[placeId].furnitureId = itemData.furnitureId;
            furniturePlaceDicData[placeId].furnitureObj = itemData.furnitureObj;
        }
        else
        {
            FurniturePlaceData placeData = new FurniturePlaceData();
            furniturePlaceDicData.Add(placeId, placeData);
        }
        //初始化 初始数据
        if (furnitureTemporaryPlaceDicData.ContainsKey(placeId))
        {
            furnitureTemporaryPlaceDicData[placeId] = itemData.furnitureId;
        }
        else
        {
            furnitureTemporaryPlaceDicData.Add(placeId, itemData.furnitureId);
        }
    }

    /// <summary>
    /// 换家具
    /// </summary>
    /// <param name="furnitureId"></param>
    /// <param name="placeId"></param>
    public void ChangeFurnitureFun(int furnitureId, int placeId, bool IsOneKey)
    {


        var kkkk = furniturePlaceDicData.Keys;
        Debug.Log("食醋胡嫌疑啊具体爹开始： " + "    placeID的值： " + placeId + "    key的长度： " + kkkk.Count);
        foreach (var key in furniturePlaceDicData.Keys)
        {
            Debug.Log("食醋胡一下具体可接受对方： " + key);
        }

        if (furniturePlaceDicData[placeId].furnitureId == furnitureId && furniturePlaceDicData[placeId].furnitureObj != null)
        {
            Debug.Log("换家具ID相同");
            return;
        }
        //回收家具
        PackUpFurnitureFun(placeId);
        FurnitureItemData itemData = GetFuritureFun(furnitureId);
        itemData.furnitureObj.SetActive(true);
        itemData.furnitureObj.transform.SetParent(allFurnitureRootData[placeId].furnitureRoot.transform);
        itemData.furnitureObj.transform.localPosition = Vector3.zero;
        // itemData.furnitureObj.transform.localScale = Vector3.one;
        furniturePlaceDicData[placeId].furnitureId = itemData.furnitureId;
        furniturePlaceDicData[placeId].furnitureObj = itemData.furnitureObj;
        if (!IsOneKey)
        {
            InterfaceHelper.SetFurnitureLightFun(itemData.furnitureObj, true);
        }
        FurnitureType furnitureType = allFurnitureRootData[placeId].furnitureType;
        int indexA = SachFurnitureDataFun(itemData.furnitureId, furnitureType);
        if (indexA != -1)
        {
            if (!furntureHasData[furnitureType][indexA].isInitial)
            {
                furntureHasData[furnitureType][indexA].hasNum -= 1;
                Debug.Log("数值在这里操作了减去1");
            }
        }
        if (furnitureTemporaryAfterPlaceDicData.ContainsKey(placeId))
        {
            furnitureTemporaryAfterPlaceDicData[placeId] = itemData.furnitureId;
        }
        else
        {
            furnitureTemporaryAfterPlaceDicData.Add(placeId, itemData.furnitureId);
        }
        var arrKeys = furniturePlaceDicData.Keys;

    }

    /// <summary>
    /// 收起家具得方法
    /// </summary>
    /// <param name="placeID"></param>
    public void PickUpFrameFun(int placeID, FurnitureType furnType, bool isAll)
    {
        int indexA = SachFurnitureDataFun(furniturePlaceDicData[placeID].furnitureId, furnType);
        if (indexA != -1)
        {
            if (!furntureHasData[furnType][indexA].isInitial)
            {
                if (furntureHasData[furnType].Count > 0)
                {
                    bool isinital = false;
                    for (int i = 0; i < furntureHasData[furnType].Count; i++)
                    {
                        if (furntureHasData[furnType][i].isInitial)
                        {
                            isinital = true;
                            ChangeFurnitureFun(furntureHasData[furnType][i].furnitureId, placeID, true);
                            break;
                        }
                    }
                    if (!isinital)
                    {
                        PackUpFurnitureFun(placeID);
                    }
                }
            }
        }
        else
        {
            if (!isAll)
            {
                ToastManager.Instance.ShowNewToast(string.Format("未摆放家具，无需收起"), 5f);
            }
        }
    }

    /// <summary>
    /// 收起家具
    /// </summary>
    public void PackUpFurnitureFun(int placeId)
    {
        if (furniturePlaceDicData[placeId].furnitureObj != null)
        {
            //回收旧家具
            Debug.Log("回收旧家具");
            FurnitureItemData itemA = new FurnitureItemData();
            itemA.furnitureId = furniturePlaceDicData[placeId].furnitureId;
            itemA.furnitureObj = furniturePlaceDicData[placeId].furnitureObj;
            InterfaceHelper.SetFurnitureLightFun(itemA.furnitureObj, false);
            FurnitureType furnitureType = allFurnitureRootData[placeId].furnitureType;
            int indexA = SachFurnitureDataFun(itemA.furnitureId, furnitureType);
            if (!furntureHasData[furnitureType][indexA].isInitial)
            {
                if (indexA != -1)
                {
                    furntureHasData[furnitureType][indexA].hasNum += 1;
                    Debug.Log("数值在这里操作了加上1");
                }
            }
            if (itemA.furnitureObj != null)
            {
                Destroy(itemA.furnitureObj);
            }
            if (furnitureTemporaryAfterPlaceDicData.ContainsKey(placeId))
            {
                furnitureTemporaryAfterPlaceDicData[placeId] = 0;
            }
            else
            {
                furnitureTemporaryAfterPlaceDicData.Add(placeId, 0);
            }
            furniturePlaceDicData[placeId].furnitureObj = null;
            furniturePlaceDicData[placeId].furnitureId = 0;
        }
    }
    //获取家具
    FurnitureItemData GetFuritureFun(int furnitureId)
    {
        furniture furnitureitemA = ManageMentClass.DataManagerClass.GetFurnitureTableFun(furnitureId);
        FurnitureItemData itemADataA = new FurnitureItemData();
        itemADataA.furnitureId = furnitureId;
        itemADataA.furnitureObj = GetResourcePrefab(furnitureitemA.furniture_model);
        return itemADataA;
    }

    /// <summary>
    /// 换藏品画
    /// </summary>
    /// <param name="furnitureId"></param>
    /// <param name="placeId"></param>
    public void ChangePictureFun(string key, int placeId)
    {
        //统统都换成假的（这里面只操作假数据）
        if (allFurniturePictureItemData.ContainsKey(placeId) && allFramePictureData.ContainsKey(key))
        {

            if (allTemporaryPlacePictureData.ContainsKey(placeId))
            {

                allTemporaryPlacePictureData[placeId] = key;
            }
            else
            {
                allTemporaryPlacePictureData.Add(placeId, key);
            }

            allFurniturePictureItemData[placeId].material.mainTexture = allFramePictureData[key].frameSprite.texture;
            InterfaceHelper.SetFurnitureLightFun(allFurniturePictureItemData[placeId].furnitureObj, true);
        }
    }

    /// <summary>
    /// 收起藏品的方法
    /// </summary>
    public void PickUpFramePictureFun(int placeID)
    {
        if (allFurniturePictureItemData.ContainsKey(placeID))
        {
            if (allTemporaryPlacePictureData.ContainsKey(placeID))
            {
                allTemporaryPlacePictureData[placeID] = null;
            }
            else
            {
                allTemporaryPlacePictureData.Add(placeID, null);
            }
            allFurniturePictureItemData[placeID].material.mainTexture = null;
        }
    }




    /// <summary>
    /// 在这里取消所有更改的家具数据
    /// </summary>
    public void ChageCancelAllFurnitureFun()
    {
        if (furnitureTemporaryAfterPlaceDicData.Count > 0)
        {
            var keys = furniturePlaceDicData.Keys;
            foreach (var key in keys)
            {
                if (!furnitureTemporaryPlaceDicData.ContainsKey(key))
                {
                    PackUpFurnitureFun(key);
                }
                else
                {
                    if (furnitureTemporaryPlaceDicData[key] != furniturePlaceDicData[key].furnitureId)
                    {
                        if (furnitureTemporaryPlaceDicData[key] > 0)
                        {
                            ChangeFurnitureFun(furnitureTemporaryPlaceDicData[key], key, false);
                        }
                        else
                        {
                            PackUpFurnitureFun(key);
                        }
                    }
                }
            }
        }
        furnitureTemporaryAfterPlaceDicData.Clear();
    }

    /// <summary>
    ///  寻找这个家具类型里面，已拥有且家具ID最大的一个
    /// </summary>
    public int SeachAllMaxFurnitureIDFun(FurnitureType furnitureType)
    {
        if (furntureHasData[furnitureType].Count > 0)
        {
            int furnID = -1;

            for (int i = 0; i < furntureHasData[furnitureType].Count; i++)
            {
                if (furntureHasData[furnitureType][i].hasNum > 0 && !furntureHasData[furnitureType][i].isInitial)
                {
                    if (furntureHasData[furnitureType][i].furnitureId > furnID)
                    {
                        furnID = furntureHasData[furnitureType][i].furnitureId;
                    }
                }
            }
            return furnID;
        }
        return -1;
    }


    /// <summary>
    /// 搜索所有的内容，看是否存在
    /// </summary>
    private int SachFurnitureDataFun(int furntureId, FurnitureType furnType)
    {
        if (furntureHasData.ContainsKey(furnType))
        {
            for (int i = 0; i < furntureHasData[furnType].Count; i++)
            {
                if (furntureHasData[furnType][i].furnitureId == furntureId)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    /// <summary>
    /// 获取预制体资源
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject GetResourcePrefab(string objName)
    {
        Debug.Log("输出一下具体的内容值这里的objName： " + objName);
        GameObject itemPrefab = Instantiate(ManageMentClass.ResourceControllerClass.ResLoadObjByPathNameFun(objName));
        return itemPrefab;
    }
    /// <summary>
    /// 房间里面是否有家具
    /// </summary>
    /// <returns></returns>
    public bool RoomIsHaveFuritureFun()
    {
        var keys = allFurnitureRootData.Keys;
        foreach (var key in keys)
        {
            if (furniturePlaceDicData.ContainsKey(key))
            {
                if (furniturePlaceDicData[key].furnitureObj != null)
                {
                    int furnid = furniturePlaceDicData[key].furnitureId;
                    Debug.Log("输出一下ID： ");
                    FurnitureType furnitureType = allFurnitureRootData[key].furnitureType;
                    int indexA = SachFurnitureDataFun(furnid, furnitureType);
                    if (!furntureHasData[furnitureType][indexA].isInitial)
                    {
                        if (indexA != -1)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 初始化设置藏品
    /// </summary>
    public void StartSetPictureFun(FramePictureData framePictureData, int placeId)
    {
        if (allFurniturePictureItemData.ContainsKey(placeId))
        {
            //存在这个ID
            allFurniturePictureItemData[placeId].product_id = framePictureData.product_id;
            allFurniturePictureItemData[placeId].material.mainTexture = framePictureData.frameSprite.texture;
        }
    }
    /// <summary>
    /// 在这里保存所有更改的家具数据（保存）
    /// </summary>
    public void ChangeSaveAllFurnitureFun()
    {
        Debug.Log("保存家具数据");
        if (furnitureTemporaryAfterPlaceDicData.Count > 0)
        {
            List<SetFurnitureData> furnitrueData = new List<SetFurnitureData>();
            var keys = furnitureTemporaryAfterPlaceDicData.Keys;
            foreach (var key in keys)
            {
                Debug.Log(" key : " + key);

                if (furnitureTemporaryPlaceDicData.ContainsKey(key))
                {
                    if (furnitureTemporaryAfterPlaceDicData[key] != furnitureTemporaryPlaceDicData[key])
                    {
                        SetFurnitureData furnData = new SetFurnitureData(key, furnitureTemporaryAfterPlaceDicData[key]);
                        furnitrueData.Add(furnData);
                        furnitureTemporaryPlaceDicData[key] = furnitureTemporaryAfterPlaceDicData[key];
                    }
                }
                else
                {
                    SetFurnitureData furnData = new SetFurnitureData(key, furnitureTemporaryAfterPlaceDicData[key]);
                    furnitrueData.Add(furnData);
                    furnitureTemporaryPlaceDicData.Add(key, furnitureTemporaryAfterPlaceDicData[key]);
                }
            }
            foreach (var item in furnitrueData)
            {
                Debug.Log(" 输出内容得值 " + JsonConvert.SerializeObject(item));
            }
            furnitureTemporaryAfterPlaceDicData.Clear();
            if (furnitrueData.Count > 0)
            {
                Debug.Log("输出一下保存的数据是什么： " + furnitrueData.ToJSON());
                //向服务器发送数据
                MessageManager.GetInstance().SetFurnitureToServerFun(furnitrueData);
            }
        }
    }

    /// <summary>
    /// 在这里保存所有更改的藏品图片
    /// </summary>
    public void ChangeSaveAllPictureFun()
    {
        var arrKeys = allTemporaryPlacePictureData.Keys;
        if (allTemporaryPlacePictureData.Count > 0)
        {
            Debug.Log("输出一下具体的内容：" + arrKeys.Count);
            List<ServerFrameData> arrFramdata = new List<ServerFrameData>();
            foreach (var key in arrKeys)
            {
                string pictureKey = allTemporaryPlacePictureData[key];
                if (allFurniturePictureItemData.ContainsKey(key))
                {
                    if (pictureKey != null)
                    {
                        //有数据，需要替换
                        if (allFramePictureData.ContainsKey(pictureKey))
                        {
                            if (allFurniturePictureItemData[key].OnlyID != allFramePictureData[pictureKey].OnlyID)
                            {

                                ServerFrameData serverFrameData = new ServerFrameData();
                                serverFrameData.id = allFurniturePictureItemData[key].id;
                                serverFrameData.nft_product_size_id = allFramePictureData[pictureKey].nft_product_size_id;
                                serverFrameData.product_id = allFramePictureData[pictureKey].product_id;
                                serverFrameData.picture = allFramePictureData[pictureKey].picture;
                                serverFrameData.land_id = ManageMentClass.DataManagerClass.landId;
                                serverFrameData.serial_no = allFurniturePictureItemData[key].serial_no;
                                arrFramdata.Add(serverFrameData);


                                allFurniturePictureItemData[key].product_id = allFramePictureData[pictureKey].product_id;
                            }
                        }
                    }
                    else
                    {
                        if (allFurniturePictureItemData[key].product_id != null && allFurniturePictureItemData[key].product_id != "")
                        {
                            //无数据，收起状态
                            ServerFrameData serverFrameData = new ServerFrameData();
                            serverFrameData.id = allFurniturePictureItemData[key].id;
                            serverFrameData.land_id = ManageMentClass.DataManagerClass.landId;
                            serverFrameData.serial_no = 0;
                            serverFrameData.nft_product_size_id = "";
                            serverFrameData.product_id = "";
                            serverFrameData.picture = "";
                            arrFramdata.Add(serverFrameData);
                            allFurniturePictureItemData[key].product_id = null;
                            allFurniturePictureItemData[key].serial_no = 0;
                        }
                    }
                }
            }
            Debug.Log("在这里看一下需要替换的藏品的个数： " + arrFramdata.Count);
            // 在这里清理掉所有的数据
            allTemporaryPlacePictureData.Clear();
            if (arrFramdata.Count > 0)
            {
                for (int i = 0; i < arrFramdata.Count; i++)
                {
                    string dataA = JsonConvert.SerializeObject(arrFramdata[i]);
                    Debug.Log("向服务器发送的藏品的内容 :     i : " + i + "   里面的数据：" + dataA);
                }
                //向服务器发送数据
                MessageManager.GetInstance().SetPictureToServerFun(arrFramdata, () =>
                {
                    MessageManager.GetInstance().GetFramePictureAction((jo) =>
                    {
                        // 如果为0，则有效，返回数据成功
                        var FrameData = jo["list"];
                        foreach (var item in FrameData)
                        {
                            string id = item["id"].ToString();
                            int serial_no = (int)item["serial_no"];
                            string picture = item["picture"].ToString();
                            string product_id = item["product_id"].ToString();
                            string nft_product_size_id = item["nft_product_size_id"].ToString();
                            if (allFurniturePictureItemData.ContainsKey(serial_no))
                            {
                                allFurniturePictureItemData[serial_no].product_id = product_id;
                                allFurniturePictureItemData[serial_no].id = id;
                                allFurniturePictureItemData[serial_no].serial_no = serial_no;
                                allFurniturePictureItemData[serial_no].OnlyID = product_id + "_" + nft_product_size_id;
                            }
                            else
                            {
                                FurniturePictureItemData furniturePictureItem = new FurniturePictureItemData();
                                furniturePictureItem.serial_no = serial_no;
                                furniturePictureItem.id = id;
                                furniturePictureItem.product_id = product_id;
                                allFurniturePictureItemData[serial_no].OnlyID = product_id + "_" + nft_product_size_id;
                                allFurniturePictureItemData.Add(serial_no, furniturePictureItem);
                            }
                        }
                    });
                });
            }
        }
    }

    /// <summary>
    /// 在这里取消所有更改的藏品图片
    /// </summary>
    public void ChageCancelAllPictureFun()
    {
        if (allTemporaryPlacePictureData.Count > 0)
        {
            var arrKeys = allTemporaryPlacePictureData.Keys;
            foreach (var key in arrKeys)
            {
                string pictureKey = allFurniturePictureItemData[key].OnlyID;

                if (pictureKey == null)
                {
                    allFurniturePictureItemData[key].material.mainTexture = null;
                }
                else
                {
                    if (allFurniturePictureItemData.ContainsKey(key) && allFramePictureData.ContainsKey(pictureKey))
                    {
                        Debug.Log("   输出一下具体的内容的值 进来了 ： " + key + "   pictureKey: " + pictureKey);
                        allFurniturePictureItemData[key].material.mainTexture = allFramePictureData[pictureKey].frameSprite.texture;
                    }
                }
            }
            // 在这里清理掉所有的数据
            allTemporaryPlacePictureData.Clear();
        }
    }
    /// <summary>
    /// 设置节点类型
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="rootData"></param>
    public FurnitureType SetFurnitureRootType(string objName)
    {
        var arrData = objName.Split('_');
        return ManageMentClass.MethodCollectionClass.GetFurnitureTypeFun(arrData[1]);
    }
    /// <summary>
    ///  初始化获取所有画框图像节点
    /// </summary>
    /// <param name="furnitureType"></param>
    /// <param name="huaRootObj"></param>
    public void SetFurniturePictureFun(FurnitureType furnitureType, GameObject huaRootObj, int placeId)
    {
        if (furnitureType == FurnitureType.hua)
        {
            if (huaRootObj.transform.childCount > 0)
            {
                GameObject pictureObj = huaRootObj.transform.GetChild(0).Find("M_zhuangshihua_change").gameObject;
                if (pictureObj != null)
                {
                    if (!allFurniturePictureItemData.ContainsKey(placeId))
                    {
                        FurniturePictureItemData itemData = new FurniturePictureItemData();
                        itemData.furnitureObj = pictureObj;
                        itemData.serial_no = placeId;
                        allFurniturePictureItemData.Add(placeId, itemData);
                    }
                    else
                    {
                        allFurniturePictureItemData[placeId].furnitureObj = pictureObj;
                    }
                    Material mat = new Material(Shader.Find("URP/Furniture/General"));
                    allFurniturePictureItemData[placeId].furnitureObj.GetComponent<MeshRenderer>().material = mat;
                    allFurniturePictureItemData[placeId].material = mat;
                }
                else
                {
                    Debug.Log("没有查找到画框");
                }
            }
        }
    }
    // <summary>
    /// (更换状态)收起藏品
    /// </summary>
    public void PackUpPictureFun(int placeId)
    {
        if (allFurniturePictureItemData.ContainsKey(placeId))
        {
            //存在这个ID
            allFurniturePictureItemData[placeId].id = null;
            allFurniturePictureItemData[placeId].material.mainTexture = null;
        }
    }

    public void GoOtherCleanDataFun()
    {
        PickUpAllFurnitureFun();
        CleanAllPictureFun();
        furnitureTemporaryPlaceDicData.Clear();
        furnitureTemporaryAfterPlaceDicData.Clear();
        allTemporaryPlacePictureData.Clear();
        // arrPostFurnitureTypeData.Clear();
    }


    //初始化场景中所有需要初始化的数据
    public void AllDataInitializeFun()
    {
        PickUpAllFurnitureFun();
        CleanAllPictureFun();



        //清除所有数据
        AgainLoadCleanDataFun();
    }




    /// <summary>
    /// 关闭所有发光物体
    /// </summary>
    public void CloseAllFurnitureLightFun()
    {
        if (arrTemporaryLight.Count > 0)
        {
            for (int i = 0; i < arrTemporaryLight.Count; i++)
            {
                Transform[] allChild = arrTemporaryLight[i].rootObj.transform.GetComponentsInChildren<Transform>();
                Debug.Log("输出一下子物体的个数：： " + allChild.Length);
                foreach (Transform child in allChild)
                {
                    if (child.gameObject.layer == 7)
                    {
                        child.gameObject.layer = 1;
                    }
                }
            }
        }
        arrTemporaryLight.Clear();
    }
    /// <summary>
    /// 清除所有家具
    /// </summary>
    public void PickUpAllFurnitureFun()
    {
        var keys = furniturePlaceDicData.Keys;
        if (keys.Count > 0)
        {
            foreach (var key in keys)
            {
                if (furniturePlaceDicData[key].furnitureObj != null)
                {
                    Destroy(furniturePlaceDicData[key].furnitureObj);
                    furniturePlaceDicData[key].furnitureObj = null;
                }
            }
        }
    }
    /// <summary>
    /// 清除所有的画
    /// </summary>
    public void CleanAllPictureFun()
    {
        var keys = allFurniturePictureItemData.Keys;
        foreach (var key in keys)
        {
            if (allFurniturePictureItemData[key].material != null)
            {
                allFurniturePictureItemData[key].material.mainTexture = null;
            }
        }
    }
    /// <summary>
    /// 购买道具后本地数据增加
    /// </summary>
    /// <param name="furnitureId"></param>
    public void BuyFurnAddCountFun(int furnitureId, int addCount)
    {
        Debug.Log("这里传进来的值是多少 个数为： " + addCount);
        string strType = ManageMentClass.DataManagerClass.GetFurnitureTableFun(furnitureId).furniture_type;
        FurnitureType type = ManageMentClass.MethodCollectionClass.GetFurnitureTypeFun(strType);
        if (furntureHasData.ContainsKey(type))
        {
            for (int i = 0; i < furntureHasData[type].Count; i++)
            {
                if (furntureHasData[type][i].furnitureId == furnitureId)
                {
                    furntureHasData[type][i].hasNum += addCount;
                    return;
                }
            }
        }
    }
    public bool IsHaveServerFurnTypeFun(FurnitureType type)
    {
        for (int i = 0; i < arrPostFurnitureTypeData.Count; i++)
        {
            if (type == arrPostFurnitureTypeData[i])
            {
                return true;
            }
        }
        return false;
    }
    public void AgainLoadCleanDataFun()
    {
        furniturePlaceDicData.Clear();
        furnitureTemporaryPlaceDicData.Clear();
        furnitureTemporaryAfterPlaceDicData.Clear();
        allFurnitureRootData.Clear();
        furntureHasData.Clear();
        allFurniturePictureItemData.Clear();
        arrPostFurnitureTypeData.Clear();
        allTemporaryPlacePictureData.Clear();
        CameraValueDataDic.Clear();
        ManageMentClass.DataManagerClass.CleanSpaceData();
    }

    /// <summary>
    /// 场景ID
    /// </summary>
    /// <param name="senceIndex"></param>
    public void SetSenceCameraDataFun(int senceIndex)
    {
        CameraValueDataDic.Clear();
        JArray jo = JArray.Parse(ManageMentClass.DataManagerClass.GetSceneListTableFun(senceIndex).camera_group);
        int Index = 0;
        foreach (var key in jo)
        {
            Index++;
            JArray pos = JArray.Parse(ManageMentClass.DataManagerClass.GetCameraListTableFun(key.ToString()).CameraPos);
            JArray Rot = JArray.Parse(ManageMentClass.DataManagerClass.GetCameraListTableFun(key.ToString()).CameraRoation);
            float Fov = float.Parse(ManageMentClass.DataManagerClass.GetCameraListTableFun(key.ToString()).CameraFovAxis);
            string CameraName = ManageMentClass.DataManagerClass.GetCameraListTableFun(key.ToString()).camera_name;
            string IconName = ManageMentClass.DataManagerClass.GetCameraListTableFun(key.ToString()).camera_icon;
            CameraValueData valueData = new CameraValueData();
            valueData.CameraPos = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
            valueData.CameraRoation = new Vector3((float)Rot[0], (float)Rot[1], (float)Rot[2]);
            valueData.CameraFovAxis = Fov;
            valueData.CameraName = CameraName;
            valueData.IconName = IconName;
            if (CameraValueDataDic.ContainsKey(Index))
            {
                CameraValueDataDic[Index] = valueData;
            }
            else
            {
                CameraValueDataDic.Add(Index, valueData);
            }

        }
    }

    /// <summary>
    /// 拷贝某个位置的模型
    /// </summary>
    /// <param name="placeID"></param>
    /// <returns></returns>
    public GameObject RoomExtraObjFun(int placeID)
    {

        if (extraObj != null)
        {
            Destroy(extraObj);
        }
        if (furniturePlaceDicData.ContainsKey(placeID))
        {
            furniture furnitureitemA = ManageMentClass.DataManagerClass.GetFurnitureTableFun(furniturePlaceDicData[placeID].furnitureId);

            return GetResourcePrefab(furnitureitemA.furniture_model);
        }
        return null;
    }

    public GameObject GetRootNodeFun()
    {
        return furnitureNode;
    }
    public GameObject GetPosNodeFun()
    {
        return furniturePosNode;
    }

    public T DeepCopyByReflect<T>(T obj)
    {
        //如果是字符串或值类型则直接返回
        if (obj is string || obj.GetType().IsValueType) return obj;
        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
            catch { }
        }
        return (T)retval;
    }
    public void SortFurnitureDataFun()
    {
        var arrKey = furntureHasData.Keys;
        if (arrKey.Count > 1)
        {
            foreach (var key in arrKey)
            {
                //List<FurntureHasDataClass>
                var arrValue = RoomFurnitureCtrl.Instance().furntureHasData[key];
                FurntureHasDataClass temp = new FurntureHasDataClass();

                //筛选出默认值
                for (int i = 0; i < arrValue.Count; i++)
                {
                    if (arrValue[i].isInitial)
                    {
                        temp = arrValue[0];
                        arrValue[0] = arrValue[i];
                        arrValue[i] = temp; ;
                    }
                }
                for (int i = 0; i < arrValue.Count; i++)
                {
                    for (int j = 0; j < arrValue.Count - 1; j++)
                    {
                        if (arrValue[j].hasNum > 0 && arrValue[j + 1].hasNum > 0)
                        {
                            if (arrValue[j].furnitureId < arrValue[j + 1].furnitureId)
                            {
                                temp = arrValue[j];
                                arrValue[j] = arrValue[j + 1];
                                arrValue[j + 1] = temp;
                            }
                        }
                        else if (arrValue[j].hasNum > 0 && arrValue[j + 1].hasNum <= 0)
                        {
                            temp = arrValue[j];
                            arrValue[j] = arrValue[j + 1];
                            arrValue[j + 1] = temp;
                        }
                        else if (arrValue[j].hasNum <= 0 && arrValue[j + 1].hasNum > 0)
                        {
                            temp = arrValue[j];
                            arrValue[j] = arrValue[j + 1];
                            arrValue[j + 1] = temp;
                        }
                        else if (arrValue[j].hasNum <= 0 && arrValue[j + 1].hasNum <= 0)
                        {
                            if (arrValue[j].furnitureId < arrValue[j + 1].furnitureId)
                            {
                                temp = arrValue[j];
                                arrValue[j] = arrValue[j + 1];
                                arrValue[j + 1] = temp;
                            }
                        }
                    }
                }

            }
        }
    }

}
