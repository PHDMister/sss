using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using System;
using Treasure;
using System.Linq;
using Google.Protobuf.Collections;

public class MessageManager : MonoBehaviour
{
    private static MessageManager _Instance = null;
    private Dictionary<int, MallServerData> m_MallServerData = new Dictionary<int, MallServerData>();

    public static MessageManager GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_MessageManager").AddComponent<MessageManager>();
        }
        return _Instance;
    }

    public enum TableType
    {
        Furniture = 1,
        Action = 2,
    }
    public void GetTokenFun(GetTokenData tokenData, Action callBackB)
    {
        StartCoroutine(GetTokenActionFun(tokenData, callBackB));
    }
    IEnumerator GetTokenActionFun(GetTokenData tokenData, Action callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();
        string data = JsonConvert.SerializeObject(tokenData);
        Debug.Log("输出一下json的数据内容： " + data);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetToken, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("获取内容： " + jo);
            if (jo["code"].ToString() == "0")
            {
                //如果为0 则有效，登录成功
                ManageMentClass.DataManagerClass.tokenValue_App = jo["data"]["token"].ToString();
                callBackB?.Invoke();
            }
            else
            {
                Debug.LogError("获取token报错： " + jo);
            }
        }
        else
        {
            Debug.LogError("获取token报错");
        }
    }
    /// <summary>
    /// 获取土地区域ID
    /// 沃德城土地，分为A区和B区
    /// </summary>
    /// <param name="callBackB"></param>
    public void GeLandtAreaIDListFun(Action<JObject> callBackB)
    {
        StartCoroutine(GetLandAreaIDListActionFun(callBackB));
    }
    IEnumerator GetLandAreaIDListActionFun(Action<JObject> callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetAreaLandList, ManageMentClass.DataManagerClass.tokenValue_Game, "{}"));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("获取内容： " + jo);
            if (jo["code"].ToString() == "0")
            {
                //如果为0 则有效，登录成功
                callBackB?.Invoke(jo);
            }
            else
            {
                Debug.LogError("获取土地区域ID报错： " + jo);
            }
        }
        else
        {
            Debug.LogError("获取土地区域ID报错");
        }
    }

    /// <summary>
    /// 获取土地区域ID
    /// 沃德城土地，分为A区和B区
    /// </summary>
    /// <param name="callBackB"></param>
    public void GetLandIDListFun(string areaId, Action<JObject> callBackB)
    {
        StartCoroutine(GetLandIDListActionFun(areaId, callBackB));
    }
    IEnumerator GetLandIDListActionFun(string areaId, Action<JObject> callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();

        AreaLanIdData areaLanIdData = new AreaLanIdData(areaId);
        string data = JsonConvert.SerializeObject(areaLanIdData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetLandIdList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("获取内容： " + jo);
            if (jo["code"].ToString() == "0")
            {
                //如果为0 则有效，登录成功
                callBackB?.Invoke(jo);
            }
            else
            {
                Debug.LogError("获取土地ID报错： " + jo);
            }
        }
        else
        {
            Debug.LogError("获取土地ID报错");
        }
    }



    /// <summary>
    /// hotdog登录，获取gameToken
    /// </summary>
    public void LoginHDFun(Action callbackA, Action callBackB)
    {
        StartCoroutine(LoginHDActionFun(callbackA, callBackB));
    }
    IEnumerator LoginHDActionFun(Action callbackA, Action callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();
        LoginData loginData = new LoginData(ManageMentClass.GameIDClass.Space, ManageMentClass.DataManagerClass.tokenValue_App);
        string data = JsonConvert.SerializeObject(loginData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.HD_Loging, ManageMentClass.DataManagerClass.tokenValue_App, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);

            Debug.Log($"<color=red>成功获取最新的GameToken：  {jo["data"]["token"]} </color>");
            if (jo["code"].ToString() == "0")
            {
                //如果为0 则有效，登录成功
                ManageMentClass.DataManagerClass.tokenValue_Game = jo["data"]["token"].ToString();
                callbackA?.Invoke();
            }
            else
            {
                callBackB?.Invoke();
            }
        }
        else
        {
            callBackB?.Invoke();
        }
    }
    /// <summary>
    /// 个人空间基础信息（名称，坐标，房间数据等）
    /// </summary>
    public void StartPersonSpaceDataFun(Action callbackA, Action callBackB)
    {
        StartCoroutine(StartPersonSpaceDataActionFun(callbackA, callBackB));
    }
    public IEnumerator StartPersonSpaceDataActionFun(Action callbackA, Action callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();
        PersonData personData = new PersonData(ManageMentClass.DataManagerClass.landId);
        string data = JsonConvert.SerializeObject(personData);

        Debug.Log("输出一下具体的内容： " + ManageMentClass.DataManagerClass.tokenValue_Game + "    data:   " + data);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.Game_Loging, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出游戏啊TokenGame 的值" + jo);
            if (jo["code"].ToString() == "0")
            {
                ManageMentClass.DataManagerClass.spce_Name = jo["data"]["space_name"].ToString();
                ManageMentClass.DataManagerClass.build_Name = jo["data"]["build_name"].ToString();
                ManageMentClass.DataManagerClass.build_XYZ = jo["data"]["build_xyz"].ToString();
                ManageMentClass.DataManagerClass.login_Name = jo["data"]["login_name"].ToString();
                ManageMentClass.DataManagerClass.gas_Amount = (int)jo["data"]["gas_amount"];
                ManageMentClass.DataManagerClass.SpaceNum = (int)jo["data"]["space_num"];
                int ownerValue = (int)jo["data"]["is_owner"];
                ManageMentClass.DataManagerClass.is_Owner = ownerValue == 1 ? true : false;
                var FrameData = jo["data"]["list"];
                foreach (var item in FrameData)
                {
                    int place_Num = (int)item["place_num"];
                    int item_Id = (int)item["item_id"];
                    if (!RoomFurnitureCtrl.Instance().furniturePlaceDicData.ContainsKey(place_Num))
                    {
                        FurniturePlaceData placeData = new FurniturePlaceData();
                        placeData.furnitureId = item_Id;
                        RoomFurnitureCtrl.Instance().furniturePlaceDicData.Add(place_Num, placeData);
                    }
                    else
                    {
                        RoomFurnitureCtrl.Instance().furniturePlaceDicData[place_Num].furnitureId = item_Id;
                    }
                }
                callbackA?.Invoke();
            }
            else
            {
                callBackB?.Invoke();
            }
        }
        else
        {
            callBackB?.Invoke();
        }
    }

    /// <summary>
    /// 拉取所有家具列表的信息
    /// </summary>
    /// <returns></returns>
    public void GetAllFurnitureDataFun(Action callbackA, Action callBackB)
    {
        StartCoroutine(GetAllFurntureDataAction(callbackA, callBackB));
    }
    public IEnumerator GetAllFurntureDataAction(Action callbackA, Action callBackB)
    {
        HttpRequest httpRequest = new HttpRequest();
        FurntureTypeClass typeData = new FurntureTypeClass("");
        string data = JsonConvert.SerializeObject(typeData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.SelfFurnitureProps, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出一下拉取列表时的数据值： " + jo.ToJSON());
            if (jo["code"].ToString() == "0")
            {
                var FrameData = jo["data"]["list"];
                int dataCount = ManageMentClass.DataManagerClass.GetInitialCountFun();
                foreach (var item in FrameData)
                {
                    FurntureHasDataClass furnitureHasData = new FurntureHasDataClass();
                    furnitureHasData.furnitureId = (int)item["item_id"];
                    furnitureHasData.hasNum = (int)item["has_num"];

                    string strType = ManageMentClass.DataManagerClass.GetFurnitureTableFun(furnitureHasData.furnitureId).furniture_type;
                    FurnitureType furnitureType = ManageMentClass.MethodCollectionClass.GetFurnitureTypeFun(strType);
                    if (dataCount > 0)
                    {
                        for (int i = 1; i <= dataCount; i++)
                        {
                            if (ManageMentClass.DataManagerClass.GetInitialTableFun(i).initial_itemID == furnitureHasData.furnitureId)
                            {
                                furnitureHasData.isInitial = true;
                            }
                        }
                    }
                    if (RoomFurnitureCtrl.Instance().furntureHasData.ContainsKey(furnitureType))
                    {
                        RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Add(furnitureHasData);
                    }
                    else
                    {
                        List<FurntureHasDataClass> listData = new List<FurntureHasDataClass>();
                        RoomFurnitureCtrl.Instance().furntureHasData.Add(furnitureType, listData);
                        RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Add(furnitureHasData);
                    }
                    //第一次判断是否是初始数据
                    bool isHave = RoomFurnitureCtrl.Instance().IsHaveServerFurnTypeFun(furnitureType);
                    if (!isHave)
                    {
                        Debug.Log("输出一下具体的UI的指的塑料袋放进： " + furnitureType);
                        RoomFurnitureCtrl.Instance().arrPostFurnitureTypeData.Add(furnitureType);
                    }

                }
                // 然后在这里把数据进行拆分排序
                RoomFurnitureCtrl.Instance().SortFurnitureDataFun();
                callbackA?.Invoke();
            }
            else
            {
                callBackB?.Invoke();
            }
        }
        else
        {
            callBackB?.Invoke();
        }
    }

    /// <summary>
    /// 下载个人头像
    /// </summary>
    public void DownPersonAvatarFun(Action callbackA)
    {
        StartCoroutine(DownPersonAcatarActionFun(callbackA));
    }

    IEnumerator DownPersonAcatarActionFun(Action callbackA)
    {
        if (ManageMentClass.DataManagerClass.Head_Texture == null)
        {
            var avatar = ManageMentClass.DataManagerClass.selfPersonData.user_pic_url;
            if (!string.IsNullOrEmpty(avatar))
            {
                HttpRequest httpRequest = new HttpRequest();
                StartCoroutine(httpRequest.DownloadImage(avatar));
                while (!httpRequest.isComPlete)
                {
                    yield return null;
                }
                if (httpRequest.isSucc)
                {
                    //下载成功
                    ManageMentClass.DataManagerClass.Head_Texture = httpRequest.downPictureList[0];
                    callbackA?.Invoke();
                    Debug.Log("下载成功A");
                }
                else
                {
                    // 下载失败

                    //使用默认头像
                    ManageMentClass.DataManagerClass.Head_Texture = InterfaceHelper.GetDefaultAvatarFun();
                    callbackA?.Invoke();
                    Debug.Log("下载失败A");
                }
            }
            else
            {
                //头像链接为空

                //使用默认头像
                ManageMentClass.DataManagerClass.Head_Texture = InterfaceHelper.GetDefaultAvatarFun();
                callbackA?.Invoke();
                Debug.Log("下载失败A");
            }
        }
        else
        {
            Debug.Log("下载失败C");
            //头像有图
            callbackA?.Invoke();
        }
    }



    /// <summary>
    /// 请求商店列表数据
    /// </summary>
    public void RequestShopData(int tabType, Action callback)
    {
        StartCoroutine(RequestData(tabType, callback));
    }
    IEnumerator RequestData(int tabType, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        ShopData shopData = new ShopData(tabType);
        string data = JsonConvert.SerializeObject(shopData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallProps, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                var listData = jo["data"]["list"];
                int index = 0;
                m_MallServerData.Clear();
                foreach (var item in listData)
                {
                    MallServerData _data = new MallServerData();
                    _data.num = (int)item["num"];
                    _data.item_id = (int)item["item_id"];
                    _data.item_name = item["item_name"].ToString();
                    _data.item_type1 = (int)item["item_type1"];
                    _data.sale_mode = (int)item["sale_mode"];
                    _data.product_number = item["product_number"].ToString();
                    _data.coin_type = (int)item["coin_type"];
                    _data.price = (int)item["price"];
                    _data.has_num = (int)item["has_num"];
                    _data.is_used = (int)item["is_used"];
                    if (_data.item_type1 == (int)TableType.Action)
                    {
                        animation m_AniConfig = ManageMentClass.DataManagerClass.GetAnimationTableFun(_data.item_id);
                        if (m_AniConfig.animation_initial > 0)//初始动作不加入商店列表
                        {
                            m_MallServerData.Add(index, _data);
                        }
                    }
                    else
                    {
                        m_MallServerData.Add(index, _data);
                    }
                    index += 1;
                }
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            Debug.Log("请求商店道具列表失败");
        }
    }
    public Dictionary<int, MallServerData> GetMallData()
    {
        return m_MallServerData;
    }
    /// <summary>
    /// 请求角色列表
    /// </summary>
    private Dictionary<int, CharacterData> m_CharacterServerData = new Dictionary<int, CharacterData>();
    public void RequestCharacterList(Action callback)
    {
        StartCoroutine(RequestCharacterData(callback));
    }

    IEnumerator RequestCharacterData(Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        CharacterListData characterData = new CharacterListData();
        string data = JsonConvert.SerializeObject(characterData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.CharacterList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                var listData = jo["data"]["list"];
                int index = 0;
                m_CharacterServerData.Clear();
                foreach (var item in listData)
                {
                    CharacterData _data = new CharacterData();
                    _data.item_id = (int)item["item_id"];
                    _data.has_num = (int)item["has_num"];
                    _data.is_selected = (int)item["is_selected"];
                    m_CharacterServerData.Add(index, _data);
                    index += 1;
                }
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
    }
    public Dictionary<int, CharacterData> GetCharacterData()
    {
        return m_CharacterServerData;
    }
    /// <summary>
    /// 获取其他空间个人信息
    /// </summary>
    /// <param name="callBack"></param>
    public void RequestOtherSpaceData(Action callBack)
    {
        StartCoroutine(RequestSpaceData(callBack));
    }
    IEnumerator RequestSpaceData(Action callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        PersonData personData = new PersonData(ManageMentClass.DataManagerClass.landId);
        string data = JsonConvert.SerializeObject(personData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.OtherSpace, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        Debug.Log("GameToken: " + ManageMentClass.DataManagerClass.tokenValue_Game + "   ManageMentClass.DataManagerClass.landId: " + ManageMentClass.DataManagerClass.landId);
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("获取空间信息成功： " + jo);
            if (jo["code"].ToString() == "0")
            {
                var listData = jo["data"]["list"];
                ManageMentClass.DataManagerClass.OtherSpaceDataList.Clear();
                foreach (var item in listData)
                {
                    Debug.Log("输出一下这里免得值item：  " + item["land_id"] + "   ManageMentClass.DataManagerClass.landId:   " + ManageMentClass.DataManagerClass.landId);
                    if ((string)item["land_id"] != ManageMentClass.DataManagerClass.landId)
                    {
                        OtherSpaceData otherSpaceData = new OtherSpaceData();

                        otherSpaceData.ID = (int)item["id"];
                        otherSpaceData.UserID = (int)item["user_id"];
                        otherSpaceData.LandID = (string)item["land_id"];
                        otherSpaceData.SpaceName = (string)item["name"];
                        otherSpaceData.BuildXYZ = (string)item["build_xyz"];
                        otherSpaceData.BuildName = (string)item["build_name"];
                        //  otherSpaceData.GasValue = (int)item["price"];
                        otherSpaceData.StatusID = (int)item["status"];
                        if (otherSpaceData.StatusID == 2)
                        {
                            otherSpaceData.Price = (int)item["price"];
                            otherSpaceData.OrderID = (int)item["order_id"];
                            otherSpaceData.ProductID = (int)item["product_id"];
                            otherSpaceData.NftProductSizeID = (int)item["nft_product_size_id"];
                        }
                        ManageMentClass.DataManagerClass.OtherSpaceDataList.Add(otherSpaceData);
                    }
                }
                Debug.Log(" ManageMentClass.DataManagerClass.OtherSpaceDataList的长度： " + ManageMentClass.DataManagerClass.OtherSpaceDataList.Count);
                callBack?.Invoke();
            }
        }
        else
        {
            Debug.Log("获取其他空间信息：" + httpRequest.value);
            ToastManager.Instance.ShowNewErrorToast("获取空间信息失败，请重试");
        }
    }

    /// <summary>
    /// 请求宠物列表数据
    /// </summary>
    public void RequestPetList(Action callback)
    {
        StartCoroutine(RequestPetData(callback));
    }
    IEnumerator RequestPetData(Action callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        if (!PetSpanManager.Instance().bInAidStations())
        {
            PetListData petListData = new PetListData(ManageMentClass.DataManagerClass.landId);
            string data = JsonConvert.SerializeObject(petListData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }
        else
        {
            AidStationsPetListData petListData = new AidStationsPetListData();
            string data = JsonConvert.SerializeObject(petListData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }

        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                var listData = jo["list"];
                ManageMentClass.DataManagerClass.petListDataDic.Clear();
                ManageMentClass.DataManagerClass.petModelRecData.Clear();
                int index1 = 0;
                int index2 = 0;
                foreach (var item in listData)
                {
                    int pet_type = (int)item["pet_type"];
                    if (pet_type == (int)PetType.PetBox)
                    {
                        index1++;
                        string jsonData = item.ToString();
                        PetListRecData _data = JsonUntity.FromJSON<PetListRecData>(jsonData);
                        if (_data != null)
                        {
                            if (!ManageMentClass.DataManagerClass.petModelRecData.ContainsKey(index1))
                            {
                                ManageMentClass.DataManagerClass.petListDataDic[index1] = _data;
                            }
                            else//位置已被进化的宠物占用
                            {
                                if (!ManageMentClass.DataManagerClass.petModelRecData.ContainsKey(index1 + 1))
                                {
                                    ManageMentClass.DataManagerClass.petListDataDic[index1 + 1] = _data;
                                    index1 += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        index2++;
                        string jsonData = item.ToString();
                        PetModelRecData _data = JsonUntity.FromJSON<PetModelRecData>(jsonData);
                        if (_data != null)
                        {
                            int pos = PetSpanManager.Instance().GetPetModelPos(_data);
                            pos = pos > 0 ? pos : index2;
                            ManageMentClass.DataManagerClass.petModelRecData[pos] = _data;
                        }
                    }
                }
                callBack?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 请求宠物列表数据
    /// </summary>
    public void RequestEnableAdopt(int type, Action callback)
    {
        StartCoroutine(RequestEnableAdoptData(type, callback));
    }
    IEnumerator RequestEnableAdoptData(int type, Action callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        PetEnableAdoptData petAdoptData = new PetEnableAdoptData(type);
        string data = JsonConvert.SerializeObject(petAdoptData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetEnableAdopt, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                ManageMentClass.DataManagerClass.petEnableAdoptStatus = jo["data"]["status"] != null ? (int)jo["data"]["status"] : 0;
                pet_adoption petAdoptionConfig = ManageMentClass.DataManagerClass.GetPetAdoptionTableFun(1);
                string costName = petAdoptionConfig != null ? petAdoptionConfig.collection_name : "";
                ManageMentClass.DataManagerClass.petAdoptCostItem = jo["data"]["collection_name"] != null ? (string)jo["data"]["collection_name"] : costName;
                ManageMentClass.DataManagerClass.petAdoptCostItemNum = jo["data"]["collection_num"] != null ? (int)jo["data"]["collection_num"] : 0;
                callBack?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 领养请求
    /// </summary>
    public void RequestAdopt(Action callback)
    {
        StartCoroutine(RequestAdoptData(callback));
    }
    IEnumerator RequestAdoptData(Action callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        if (!PetSpanManager.Instance().bInAidStations())
        {
            PetAdoptData petAdoptData = new PetAdoptData(ManageMentClass.DataManagerClass.landId);
            string data = JsonConvert.SerializeObject(petAdoptData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetAdopt, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }
        else
        {
            AidStationAdoptData petAdoptData = new AidStationAdoptData();
            string data = JsonConvert.SerializeObject(petAdoptData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetAdopt, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }

        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                ToastManager.Instance.ShowPetToast("领养成功", 3f);
                callBack?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 喂养请求
    /// </summary>
    public void RequestFeed(string data, Action<JObject> callback)
    {
        StartCoroutine(RequestFeedData(data, callback));
    }
    IEnumerator RequestFeedData(string data, Action<JObject> callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetFeed, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }





    public void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }

    /// <summary>
    /// 下拉二期领养时信息
    /// </summary>

    public delegate void Callback(CreatePetRecData data);
    public void RequestCreatePetInfo(int petId, Callback callback)
    {
        StartCoroutine(RequestCreatePetInfoData(petId, callback));
    }
    IEnumerator RequestCreatePetInfoData(int petId, Callback callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        CreatePetData createPetData = new CreatePetData(petId);
        string data = JsonConvert.SerializeObject(createPetData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.CreatePet, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                CreatePetRecData _data = JsonUntity.FromJSON<CreatePetRecData>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestPetAdoptV2(PetAdoptV2ReqData petData, Action callback)
    {
        if (petData == null)
            return;
        StartCoroutine(RequestPetAdoptV2Data(petData, callback));
    }
    IEnumerator RequestPetAdoptV2Data(PetAdoptV2ReqData petData, Action callback)
    {

        HttpRequest httpRequest = new HttpRequest();
        string data = JsonConvert.SerializeObject(petData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetAdoptV2, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// Gas更新
    /// </summary>
    public void RequestGasValue(Action callback = null)
    {
        StartCoroutine(RequestGasData(callback));
    }
    IEnumerator RequestGasData(Action callback = null)
    {
        HttpRequest httpRequest = new HttpRequest();
        GasValueReqData gasValueReqData = new GasValueReqData();
        string data = JsonConvert.SerializeObject(gasValueReqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GasValue, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                ManageMentClass.DataManagerClass.gas_Amount = (int)jo["gas"];
                SendMessage("UpdataGasValue", "data", null);
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }


    public delegate void LoveCoinCallback(LoveCoinRecData data);
    /// <summary>
    /// 爱心币列表
    /// </summary>
    /// <param name="callback"></param>
    public void RequestLoveCoinList(LoveCoinCallback callback)
    {
        StartCoroutine(RequestLoveCoinListData(callback));
    }
    IEnumerator RequestLoveCoinListData(LoveCoinCallback callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        if (!PetSpanManager.Instance().bInAidStations())
        {
            LandIdReqData loveCoinReqData = new LandIdReqData(ManageMentClass.DataManagerClass.landId);
            string data = JsonConvert.SerializeObject(loveCoinReqData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.LoveCoinList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }
        else
        {
            AidStationLoveCoinReqData loveCoinReqData = new AidStationLoveCoinReqData();
            string data = JsonConvert.SerializeObject(loveCoinReqData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.LoveCoinList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                LoveCoinRecData _data = JsonUntity.FromJSON<LoveCoinRecData>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public delegate void LoveCoinReceiveCallback(LoveCoinReceiveRecData data);
    /// <summary>
    /// 爱心币领取
    /// </summary>
    /// <param name="petId"></param>
    /// <param name="callback"></param>
    public void RequestLoveCoinReceive(int petId, LoveCoinReceiveCallback callback)
    {
        StartCoroutine(RequestLoveCoinReceivetData(petId, callback));
    }
    IEnumerator RequestLoveCoinReceivetData(int petId, LoveCoinReceiveCallback callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        LoveCoinReceiveReqData loveCoinReceiveReqData = new LoveCoinReceiveReqData(petId);
        string data = JsonConvert.SerializeObject(loveCoinReceiveReqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.LoveCoinReceive, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                LoveCoinReceiveRecData _data = JsonUntity.FromJSON<LoveCoinReceiveRecData>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 粪便列表
    /// </summary>
    /// <param name="data"></param>
    public delegate void FecesReceiveCallback(FecesRecData data);
    public void RequestFeces(FecesReceiveCallback callback)
    {
        StartCoroutine(RequestFecesData(callback));

    }
    IEnumerator RequestFecesData(FecesReceiveCallback callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        if (!PetSpanManager.Instance().bInAidStations())
        {
            LandIdReqData loveCoinReqData = new LandIdReqData(ManageMentClass.DataManagerClass.landId);
            string data = JsonConvert.SerializeObject(loveCoinReqData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.FecesList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }
        else
        {
            AidStationLoveCoinReqData loveCoinReqData = new AidStationLoveCoinReqData();
            string data = JsonConvert.SerializeObject(loveCoinReqData);
            StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.FecesList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        }

        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["list"].ToString();
                FecesRecData _data = new FecesRecData();
                _data.list = JsonUntity.FromJSON<List<PetFeces>>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public delegate void ClearFecesCallback(ClearFecesRecData data);
    /// <summary>
    /// 粪便清理
    /// </summary>
    /// <param name="fecesId"></param>
    public void RequestClearFeces(List<int> fecesIds, ClearFecesCallback callback)
    {
        StartCoroutine(RequestClearFecesData(fecesIds, callback));
    }
    IEnumerator RequestClearFecesData(List<int> fecesIds, ClearFecesCallback callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        ClearFecesReqData clearFecesReqData = new ClearFecesReqData(fecesIds);
        string data = JsonConvert.SerializeObject(clearFecesReqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ClearFeces, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                ClearFecesRecData _data = JsonUntity.FromJSON<ClearFecesRecData>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    //请求宠物商店数据
    public void RequestPetShopReceive(string data, Action<JObject> callback)
    {
        StartCoroutine(RequestPetShopReceiveData(data, callback));
    }
    IEnumerator RequestPetShopReceiveData(string data, Action<JObject> callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetShopReceive, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("商店数据错误，请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("商店数据请求失败，请重试", 5f);
        }
    }


    //宠物商店购买
    public void RequestPetShopBuyReceive(string data, Action<JObject> callback)
    {
        StartCoroutine(RequestPetShopBuyReceiveData(data, callback));
    }
    IEnumerator RequestPetShopBuyReceiveData(string data, Action<JObject> callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetShopBuyReceive, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {

                callback?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("购买失败，请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("购买失败，请重试", 5f);
        }
    }

    //清除训练成果弹框
    public void RequestPetClearTrainData(int trainRecordId, Action callBack)
    {
        StartCoroutine(RequestPetClearTrainDataCoroutine(trainRecordId, callBack));
    }
    IEnumerator RequestPetClearTrainDataCoroutine(int trainRecordId, Action callBack)
    {
        PetClearTrainResultData clearTrainResData = new PetClearTrainResultData(trainRecordId);
        string data = JsonConvert.SerializeObject(clearTrainResData);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetClearResultReceive, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("jo的内容： " + jo);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowPetToast("请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("请重试", 5f);
        }
    }

    //训练
    public void RequestPetTrain(int petID, int trainID, Action<JObject> callBack, Action failCallBack)
    {
        StartCoroutine(RequestPetTrainCoroutine(petID, trainID, callBack, failCallBack));
    }
    IEnumerator RequestPetTrainCoroutine(int petID, int trainID, Action<JObject> callBack, Action failCallBack)
    {
        PetTrainData clearTrainResData = new PetTrainData(petID, trainID);
        string data = JsonConvert.SerializeObject(clearTrainResData);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetTrain, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
            else
            {
                failCallBack?.Invoke();
                ToastManager.Instance.ShowPetToast(jo["msg"].ToString(), 5f);
            }
        }
        else
        {
            failCallBack?.Invoke();
            ToastManager.Instance.ShowPetToast("暂时不能训练，请稍后重试", 5f);
        }
    }

    //结束训练
    public void RequestPetTrainFinish(int recordId, Action<JObject> callBack)
    {
        StartCoroutine(RequestPetTrainFinishCoroutine(recordId, callBack));
    }
    IEnumerator RequestPetTrainFinishCoroutine(int recordId, Action<JObject> callBack)
    {
        PetTrainFinishData clearTrainResData = new PetTrainFinishData(recordId);
        string data = JsonConvert.SerializeObject(clearTrainResData);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PetTrainFinish, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowPetToast("不能结束训练，请稍后重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowPetToast("不能结束训练，请稍后重试", 5f);
        }
    }

    public delegate void OutFitRecDataCallBack(List<OutFitRecData> data);
    public void RequestOutFitList(OutFitRecDataCallBack callback)
    {
        StartCoroutine(RequestOutFitListData(callback));
    }
    IEnumerator RequestOutFitListData(OutFitRecDataCallBack callback)
    {
        EmptyReqData emptyReqData = new EmptyReqData();
        string data = JsonConvert.SerializeObject(emptyReqData);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MyOutFitList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                List<OutFitRecData> _data = JsonUntity.FromJSON<List<OutFitRecData>>(jsonData);
                callback(_data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestMyOutFitSave(MyOutFitSaveReqData _data, Action callback)
    {
        StartCoroutine(RequestMyOutFitSaveData(_data, callback));
    }
    IEnumerator RequestMyOutFitSaveData(MyOutFitSaveReqData _data, Action callback)
    {
        string data = JsonConvert.SerializeObject(_data);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MyOutFitSave, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 商店服装数据
    /// </summary>
    /// <param name="callback"></param>
    public delegate void ShopOutFitRecDataCallBack(List<ShopOutFitRecData> data);
    public void RequestShopOutFitList(ShopOutFitRecDataCallBack callback)
    {
        StartCoroutine(RequestShopOutFitListData(callback));
    }
    IEnumerator RequestShopOutFitListData(ShopOutFitRecDataCallBack callback)
    {
        ShopOutFitReqData _data = new ShopOutFitReqData(4);
        string data = JsonConvert.SerializeObject(_data);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ShopOutFitList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                List<ShopOutFitRecData> shopData = JsonUntity.FromJSON<List<ShopOutFitRecData>>(jsonData);
                callback(shopData);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestShopRedDotClear(Action callback)
    {
        StartCoroutine(RequestShopRedDotClearData(callback));
    }
    IEnumerator RequestShopRedDotClearData(Action callback)
    {
        EmptyReqData _data = new EmptyReqData();
        string data = JsonConvert.SerializeObject(_data);
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ShopRedDotClear, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }


    public void RequestOutFitBuy(BuyData data, Action callback)
    {
        StartCoroutine(RequestOutFitBuyData(data, callback));
    }
    IEnumerator RequestOutFitBuyData(BuyData data, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        string _data = JsonConvert.SerializeObject(data);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallBuy, ManageMentClass.DataManagerClass.tokenValue_Game, _data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                callback?.Invoke();
                MessageManager.GetInstance().RequestGasValue();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }


    /// <summary>
    /// 房间列表
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="callback"></param>
    public void RequestRoomList(string landID, Action callback)
    {
        StartCoroutine(RequestRoomListData(landID, callback));
    }
    IEnumerator RequestRoomListData(string landID, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        LandIdReqData landIdReqData = new LandIdReqData(landID);
        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
        {
            landIdReqData.land_id = "";
        }
        string _data = JsonConvert.SerializeObject(landIdReqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.DogRoomList, ManageMentClass.DataManagerClass.tokenValue_Game, _data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                string jsonData = jo["list"].ToString();
                Debug.Log("输出一下房间列表的信息： " + jsonData);
                ChangeRoomManager.Instance().AllDogRoomItemData = JsonUntity.FromJSON<List<DogRoomItemData>>(jsonData);
                Debug.Log("房间长度： " + ChangeRoomManager.Instance().AllDogRoomItemData.Count);
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewToast("房间列表获取失败，请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast("房间列表获取失败，请重试", 5f);
        }
    }
    /// <summary>
    /// 使用房间
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="callback"></param>
    public void RequestUseRoomBuy(int roomID, Action callback)
    {
        StartCoroutine(RequestUseRoomData(roomID, callback));
    }
    IEnumerator RequestUseRoomData(int roomID, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        DogServerData dogServer = new DogServerData(ManageMentClass.DataManagerClass.landId, roomID);

        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
        {
            dogServer.land_id = "";
        }
        string _data = JsonConvert.SerializeObject(dogServer);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.DogRoomUse, ManageMentClass.DataManagerClass.tokenValue_Game, _data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewToast("保存失败，请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast("保存失败，请重试", 5f);
        }
    }

    /// <summary>
    /// 购买房间
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="callback"></param>
    public void RequestBuyRoomBuy(int roomID, Action callback)
    {
        StartCoroutine(RequestBuyRoomData(roomID, callback));
    }
    IEnumerator RequestBuyRoomData(int roomID, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        DogServerData dogServer = new DogServerData(ManageMentClass.DataManagerClass.landId, roomID);
        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
        {
            dogServer.land_id = "";
        }
        string _data = JsonConvert.SerializeObject(dogServer);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.DogRoomBuy, ManageMentClass.DataManagerClass.tokenValue_Game, _data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewToast("购买失败，请重试", 5f);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast("购买失败，请重试", 5f);
        }
    }

    /// <summary>
    ///领养凭证兑换
    /// </summary>
    /// <param name="fecesId"></param>
    public void RequestExchangeProof(int petId, Action callback)
    {
        StartCoroutine(RequestExchangeProofData(petId, callback));
    }
    IEnumerator RequestExchangeProofData(int petId, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        ExchangeProofReqData reqData = new ExchangeProofReqData(petId);
        string data = JsonConvert.SerializeObject(reqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ExchangeProof, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    ///领养凭证数量
    /// </summary>
    /// <param name="fecesId"></param>
    public delegate void ExchangeProofRecDataCallBack(ExchangeProofRecData data);
    public void RequestExchangeProofNum(ExchangeProofRecDataCallBack callback)
    {
        StartCoroutine(RequestExchangeProofNumData(callback));
    }
    IEnumerator RequestExchangeProofNumData(ExchangeProofRecDataCallBack callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        EmptyReqData reqData = new EmptyReqData();
        string data = JsonConvert.SerializeObject(reqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ExchangeProofNum, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                ExchangeProofRecData param = JsonUntity.FromJSON<ExchangeProofRecData>(jsonData);
                callback?.Invoke(param);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }



    /// <summary>
    ///寻宝门票兑换
    /// </summary>
    /// <param name="fecesId"></param>
    public void RequestExchangeTickets(int num, Action callback)
    {
        StartCoroutine(RequestExchangeTicketsAction(num, callback));
    }
    IEnumerator RequestExchangeTicketsAction(int num, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        TreasureExchangeReqData reqData = new TreasureExchangeReqData(num);
        string data = JsonConvert.SerializeObject(reqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.TreasureExchange, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }
    /// <summary>
    /// 进入挖宝请求
    /// </summary>
    public void SendEnterTreasure()
    {
        EnterTreasureReq enterTreasureReq = new EnterTreasureReq();
        enterTreasureReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.EnterTreasureReq, enterTreasureReq);
    }
    /// <summary>
    /// 房间列表请求
    /// </summary>
    public void SendRoomList()
    {
        RoomListReq roomListReq = new RoomListReq();
        roomListReq.Start = 1;
        roomListReq.PageSize = 1000;
        roomListReq.SceneId = (uint)ManageMentClass.DataManagerClass.SceneID;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.RoomListReq, roomListReq);
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public void SendCreateRoom()
    {
        CreateRoomReq createRoomReq = new CreateRoomReq();
        createRoomReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.CreateRoomReq, createRoomReq);
    }
    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="roomId"></param>
    public void SendJoinRoom(uint roomId)
    {
        JoinRoomReq joinRoomReq = new JoinRoomReq();
        joinRoomReq.UserId = ManageMentClass.DataManagerClass.userId;
        joinRoomReq.RoomId = roomId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.JoinRoomReq, joinRoomReq);
    }

    /// <summary>
    /// 下载头像
    /// </summary>
    public delegate void DownLoadAvatarCallBack(Sprite sprite);
    public void DownLoadAvatar(string avatarUrl, DownLoadAvatarCallBack callback)
    {
        StartCoroutine(DownLoadAvatarCoroutine(avatarUrl, callback));
    }

    IEnumerator DownLoadAvatarCoroutine(string avatarUrl, DownLoadAvatarCallBack callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.DownloadImage(avatarUrl));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            //下载成功
            Sprite sprite = httpRequest.downPictureList[0];
            callback?.Invoke(sprite);
        }
        else
        {
            // 下载失败使用默认头像
            Sprite sprite = InterfaceHelper.GetDefaultAvatarFun();
            callback?.Invoke(sprite);
        }
    }

    /// <summary>
    /// 下载头像
    /// </summary>
    public delegate void DownLoadAvatarCallBack1(string url, Sprite sprite);
    public void DownLoadAvatar(string avatarUrl, DownLoadAvatarCallBack1 callback)
    {
        StartCoroutine(DownLoadAvatarCoroutine1(avatarUrl, callback));
    }

    IEnumerator DownLoadAvatarCoroutine1(string avatarUrl, DownLoadAvatarCallBack1 callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.DownloadImage(avatarUrl));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            //下载成功
            Sprite sprite = httpRequest.downPictureList[0];
            callback?.Invoke(avatarUrl, sprite);
        }
        else
        {
            // 下载失败使用默认头像
            Sprite sprite = InterfaceHelper.GetDefaultAvatarFun();
            callback?.Invoke(avatarUrl, sprite);
        }
    }


    /// <summary>
    /// 下载头像
    /// </summary>
    public delegate void DownLoadPictureCallBack(Sprite sprite);
    public void DownLoadPicture(string avatarUrl, DownLoadPictureCallBack sucCb, Action failCb)
    {
        StartCoroutine(DownLoadPictureCoroutine(avatarUrl, sucCb, failCb));
    }

    IEnumerator DownLoadPictureCoroutine(string avatarUrl, DownLoadPictureCallBack sucCb, Action failCb)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.DownloadImage(avatarUrl));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            //下载成功
            Sprite sprite = httpRequest.downPictureList[0];
            sucCb?.Invoke(sprite);
        }
        else
        {
            failCb?.Invoke();
        }
    }

    /// <summary>
    /// 获取寻宝券的数量
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="callback"></param>
    public void RequestGetTicketCount(Action callback)
    {
        StartCoroutine(RequestGetTicketCountAction(callback));
    }
    IEnumerator RequestGetTicketCountAction(Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();

        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.TicketsNum, ManageMentClass.DataManagerClass.tokenValue_Game, "null"));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                string jsonData = jo["data"].ToString();
                TicketNumRecData data = JsonUntity.FromJSON<TicketNumRecData>(jsonData);
                if (data != null)
                {
                    ManageMentClass.DataManagerClass.ticket = data.ticket_num;
                    TreasureModel.Instance.bRecTicketNum = true;
                }
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast("获取门票数量失败，请重试", 5f);
        }
    }

    ///寻宝好友列表
    /// </summary>
    /// <param name="fecesId"></param>
    public delegate void TreasureFriendListRecCallBack(List<TreasureFriendListRecData> data);
    public void RequestFriendList(TreasureFriendListRecCallBack callback)
    {
        StartCoroutine(RequestFriendListData(callback));
    }
    IEnumerator RequestFriendListData(TreasureFriendListRecCallBack callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        EmptyReqData reqData = new EmptyReqData();
        string data = JsonConvert.SerializeObject(reqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.FriendList, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["list"].ToString();
                List<TreasureFriendListRecData> param = JsonUntity.FromJSON<List<TreasureFriendListRecData>>(jsonData);
                callback?.Invoke(param);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    /// <summary>
    /// 邀请附近的人组队
    /// </summary>
    public void SendInviteJoinTeamReq(ulong from_user_id, ulong to_user_id, uint room_id)
    {
        InviteJoinTeamReq inviteJoinTeamReq = new InviteJoinTeamReq();
        inviteJoinTeamReq.FromUserId = from_user_id;
        inviteJoinTeamReq.ToUserId = to_user_id;
        inviteJoinTeamReq.RoomId = room_id;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.InviteJoinTeamReq, inviteJoinTeamReq, (code, bytes) =>
        {
            if (code != 0) return;
            InviteJoinTeamResp inviteJoinTeamResp = InviteJoinTeamResp.Parser.ParseFrom(bytes);
            if (inviteJoinTeamResp.StatusCode == 0)
            {
                object[] param = new object[] { (int)TreasureDiggingInviteTeamList.PageType.Near, inviteJoinTeamReq.ToUserId };
                SendMessage("InviteJoinTeamRespCb", "Success", param);
                Debug.Log($"[WebSocket] InviteJoinTeamResp Success");
            }
            else if (inviteJoinTeamResp.StatusCode == 230014)
            {
                ToastManager.Instance.ShowNewToast("当前无法接受邀请哦~", 5f);
            }
        });
    }

    /// <summary>
    /// 邀请好友
    /// </summary>
    /// <param name="from_user_id"></param>
    /// <param name="to_user_id"></param>
    public void SendInviteFriendReq(ulong from_user_id, ulong to_user_id)
    {
        InviteFriendReq inviteFriendReq = new InviteFriendReq();
        inviteFriendReq.FromUserId = from_user_id;
        inviteFriendReq.ToUserId = to_user_id;

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.InviteFriendReq, inviteFriendReq, (code, bytes) =>
        {
            if (code != 0) return;
            InviteFriendResp inviteFriendResp = InviteFriendResp.Parser.ParseFrom(bytes);
            if (inviteFriendResp.StatusCode == 0)
            {
                object[] param = new object[] { (int)TreasureDiggingInviteTeamList.PageType.Friend, inviteFriendReq.ToUserId };
                SendMessage("InviteJoinTeamRespCb", "Success", param);
                Debug.Log($"[WebSocket] InviteFriendResp Success");
            }
            else if (inviteFriendResp.StatusCode == 230014)
            {
                ToastManager.Instance.ShowNewToast("当前无法接受邀请哦~", 5f);
            }
        });
    }
    /// <summary>
    /// 邀请好友
    /// </summary>
    /// <param name="from_user_id"></param>
    /// <param name="to_user_id"></param>
    public delegate void TreasureRecordListRespCallBack(TreasureRecordListResp data);
    public void SendTreasureRecordReq(uint page, uint page_size, ulong user_id, TreasureRecordListRespCallBack callback, uint[] sceneIds = null)
    {
        TreasureRecordListReq treasureRecord = new TreasureRecordListReq();
        treasureRecord.Page = page;
        treasureRecord.PageSize = page_size;
        treasureRecord.UserId = user_id;
        treasureRecord.SceneIds.AddRange(sceneIds);
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.TreasureRecordListReq, treasureRecord, (code, bytes) =>
        {
            if (code != 0) return;
            TreasureRecordListResp resp = TreasureRecordListResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                callback?.Invoke(resp);
                Debug.Log($"[WebSocket] TreasureRecordListResp Success");
            }
        });
    }

    /// <summary>
    /// 邀请上线
    /// </summary>
    /// <param name="from_user_id"></param>
    /// <param name="to_user_id"></param>
    public void SendSummonBackReq(ulong from_user_id, ulong to_user_id, string land_id, Action callBack)
    {
        SummonBackReq req = new SummonBackReq();
        req.FromUserId = from_user_id;
        req.ToUserId = to_user_id;
        req.LandId = land_id;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.SummonBackReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            SummonBackResp resp = SummonBackResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                Debug.Log($"[WebSocket] SummonBackReq Success");
                Singleton<ParlorVirtualOwnerController>.Instance.Invated = true;
            }
            callBack?.Invoke();
        });
    }

    /// <summary>
    /// 请求队伍信息
    /// </summary>
    public delegate void TeamInfoReqCallBack(TeamInfoResp data);
    public void RequestTeamInfo(TeamInfoReqCallBack callback)
    {
        TeamInfoReq teamInfoReq = new TeamInfoReq();
        teamInfoReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.TeamInfoReq, teamInfoReq, (code, bytes) =>
        {
            if (code != 0) return;
            TeamInfoResp teamInfoResp = TeamInfoResp.Parser.ParseFrom(bytes);
            if (teamInfoResp.StatusCode == 0)
            {
                Debug.Log($"[WebSocket] TeamInfoResp Success");
                callback?.Invoke(teamInfoResp);
            }
        });
    }

    /// <summary>
    /// 开启时间请求
    /// </summary>
    public void RequestOpenTime(Action callback = null)
    {
        OpenTimeReq openTimeReq = new OpenTimeReq();
        openTimeReq.UserId = ManageMentClass.DataManagerClass.userId;

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.OpenTimeReq, openTimeReq, (code, bytes) =>
        {
            if (code != 0) return;
            OpenTimeResp openTimeResp = OpenTimeResp.Parser.ParseFrom(bytes);
            if (openTimeResp.StatusCode == 0)
            {
                ManageMentClass.DataManagerClass.CurTime = (int)openTimeResp.Cur;
                //TreasureModel.Instance.CurTime = (int)openTimeResp.Cur;
                //TreasureModel.Instance.StartTime = (int)openTimeResp.Start;
                //TreasureModel.Instance.EndTime = (int)openTimeResp.End;
                Debug.Log($"[WebSocket] OpenTimeResp  Cur: " + openTimeResp.Cur + " Start: " + openTimeResp.Start + " End: " + openTimeResp.End);
                callback?.Invoke();
            }
        });
    }

    /// <summary>
    /// 消耗寻宝券
    /// </summary>
    /// <param name="callback"></param>
    public void RequestCostTicket(Action callback)
    {
        StartCoroutine(RequestCostTicketData(callback));
    }
    IEnumerator RequestCostTicketData(Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        EmptyReqData reqData = new EmptyReqData();
        string data = JsonConvert.SerializeObject(reqData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.CostTicket, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if (jo["code"].ToString() == "0")
            {
                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestTreasureReward()
    {
        TreasureRewardConfListReq req = new TreasureRewardConfListReq();
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.TreasureRewardConfListReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            TreasureRewardConfListResp resp = TreasureRewardConfListResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                TreasureModel.Instance.RewardPreviewData = resp.List.ToList();
            }
        });
    }

    public delegate void RoomInfoReqCallBack(RoomInfoResp data);
    public void RequestRoomInfo(RoomInfoReqCallBack callback)
    {
        RoomInfoReq req = new RoomInfoReq();
        req.RoomId = ManageMentClass.DataManagerClass.roomId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.RoomInfoReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            RoomInfoResp resp = RoomInfoResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                callback?.Invoke(resp);
            }
        });
    }


    /// <summary>
    /// 获取寻宝兑换券消耗藏品数量
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="callback"></param>
    public delegate void RequestCollectionCallBack(List<CollectioListRecData> data);
    public void RequestCollectionCount(RequestCollectionCallBack callback)
    {
        StartCoroutine(RequestCollectionCountData(callback));
    }
    IEnumerator RequestCollectionCountData(RequestCollectionCallBack callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.CollectioList, ManageMentClass.DataManagerClass.tokenValue_Game, "null"));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                string jsonData = jo["list"].ToString();
                List<CollectioListRecData> data = JsonUntity.FromJSON<List<CollectioListRecData>>(jsonData);
                callback?.Invoke(data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestGetPersonData(Action callBack, ulong user_ID = 0)
    {
        StartCoroutine(RequestGetPersonDataIE(callBack, user_ID));
    }
    IEnumerator RequestGetPersonDataIE(Action callBack, ulong user_ID)
    {
        HttpRequest httpRequest = new HttpRequest();
        var request = new JObject();
        request["user_ID"] = user_ID;
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetPersonData, ManageMentClass.DataManagerClass.tokenValue_Game, request.ToString()));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出一下获取到的个人资料的数据： " + jo);
            if (jo["code"].ToString() == "0")
            {
                string jsonData = jo["data"].ToString();
                var personData = jsonData.FromJSON<PersonUserData>();
                ManageMentClass.DataManagerClass.personUserData = personData;
                if (user_ID == 0)
                {
                    ManageMentClass.DataManagerClass.selfPersonData = personData;
                    //更新用户聊天信息
                    ChatMgr.Instance.ImUpdateUserInfo();
                }
                callBack();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestSavePersonData(Action callBack, Action callBackB, string data)
    {
        StartCoroutine(RequestSavePersonDataIE(callBack, callBackB, data));
    }
    IEnumerator RequestSavePersonDataIE(Action callBack, Action callBackB, string data)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.SetPersonData, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出一下具体的内容只士大夫艰苦撒旦解放： " + jo.ToString());
            if (jo["code"].ToString() == "0")
            {
                callBack();
            }
            else
            {
                callBackB();
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            callBackB();
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestPersonAssetCountData(Action<JObject> callBack, string data)
    {
        StartCoroutine(RequestPersonAssetCountDataIE(callBack, data));
    }
    IEnumerator RequestPersonAssetCountDataIE(Action<JObject> callBack, string data)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetPersonAssetCount, ManageMentClass.DataManagerClass.tokenValue_Game, data));

        Debug.Log("疏忽一下token : " + ManageMentClass.DataManagerClass.tokenValue_Game + "   数据：  " + data);

        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出这里的值： " + jo);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }
    public void RequestPersonImageData(Action<JObject> callBack, string data)
    {
        StartCoroutine(RequestPersonImageDataIE(callBack, data));
    }
    IEnumerator RequestPersonImageDataIE(Action<JObject> callBack, string data)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetPersonProperty, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("输出这里的值： " + jo);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }
    /// <summary>
    /// 装修家具
    /// </summary>
    /// <param name="furnitrueData"></param>
    /// <param name="callBack"></param>
    public void SetFurnitureToServerFun(List<SetFurnitureData> furnitrueData)
    {
        StartCoroutine(PostFurnitureAction(furnitrueData));
    }
    private IEnumerator PostFurnitureAction(List<SetFurnitureData> furnitrueData)
    {
        HttpRequest httpRequest = new HttpRequest();
        SetFrurnitureToServerData serverData = new SetFrurnitureToServerData(furnitrueData, ManageMentClass.DataManagerClass.landId);
        string data = JsonConvert.SerializeObject(serverData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ReplaceFurniture, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            InterfaceHelper.CalcGameOutFun(jo["code"].ToString());
        }
    }
    /// <summary>
    /// 装修藏品
    /// </summary>
    /// <param name="frameData"></param>
    /// <param name="callBack"></param>
    public void SetPictureToServerFun(List<ServerFrameData> frameData, Action callBack)
    {
        StartCoroutine(PostPictureAction(frameData, callBack));
    }
    private IEnumerator PostPictureAction(List<ServerFrameData> frameData, Action callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        SetPictureToServerData serverData = new SetPictureToServerData(frameData);
        string data = JsonConvert.SerializeObject(serverData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.SetPicture, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        Debug.Log("输出一下数据  藏品： " + data);
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);

            Debug.Log("加载框的时候的值： " + jo);
            InterfaceHelper.CalcGameOutFun(jo["code"].ToString());
            callBack();
        }
    }
    /// <summary>
    /// 获取藏品数据
    /// </summary>
    public void GerFramePictureDataFun(Action<JObject> callBack)
    {
        StartCoroutine(GetFramePictureAction(callBack));
    }
    public IEnumerator GetFramePictureAction(Action<JObject> callBack)
    {
        HttpRequest httpRequest = new HttpRequest();
        PersonData personData = new PersonData(ManageMentClass.DataManagerClass.landId);
        string data = JsonConvert.SerializeObject(personData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.PlacePicture, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("加载框的时候的值 最后： " + jo);
            if (jo["code"].ToString() == "0")
            {
                callBack?.Invoke(jo);
            }
        }
    }

    public void RequestShellCount(Action<ShellData> callback)
    {
        StartCoroutine(RequestShellCountData(callback));
    }
    IEnumerator RequestShellCountData(Action<ShellData> callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetShellData, ManageMentClass.DataManagerClass.tokenValue_Game, "null"));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                string jsonData = jo["data"].ToString();
                ShellData data = JsonUntity.FromJSON<ShellData>(jsonData);
                callback?.Invoke(data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestShopData(Action<List<RainBowShopData>> callback)
    {
        StartCoroutine(RequestShopDataIE(callback));
    }

    IEnumerator RequestShopDataIE(Action<List<RainBowShopData>> callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.GetShopData, ManageMentClass.DataManagerClass.tokenValue_Game, "null"));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                string jsonData = jo["data"].ToString();
                var data = JsonUntity.FromJSON<List<RainBowShopData>>(jsonData);
                callback?.Invoke(data);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }

    public void RequestShopBuy(int itemId, Action callback)
    {
        StartCoroutine(RequestShopBuyIE(itemId, callback));
    }

    IEnumerator RequestShopBuyIE(int itemId, Action callback)
    {
        HttpRequest httpRequest = new HttpRequest();
        var jsonObject = new JObject
        {
            ["item_id"] = itemId,
            ["num"] = 1,
        };
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ShopBuy, ManageMentClass.DataManagerClass.tokenValue_Game, jsonObject.ToString()));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                var item_id = (uint)jo["data"]["item_id"];
                var has_num = (uint)jo["data"]["has_num"];
                var shell_amount = (uint)jo["data"]["shell_amount"];

                Singleton<BagMgr>.Instance.UpdateItem(item_id, has_num);
                Singleton<BagMgr>.Instance.SetShellNum(shell_amount);

                callback?.Invoke();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
        }
    }


    /// <summary>
    /// 水族馆详情请求
    /// </summary>
    public void SendAquariumReq(Action callback)
    {
        AquariumReq req = new AquariumReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.AquariumReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            AquariumResp resp = AquariumResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                Debug.Log($"[WebSocket] AquariumReq Success");
                Singleton<AquariumDataModel>.Instance.SysAquariumData(resp.Data);
            }
            callback?.Invoke();
        });
    }

    /// <summary>
    /// 鱼出售请求
    /// </summary>
    /// <param name="user_id"></param>
    /// <param name="fish_id"></param>
    /// <param name="sell_cnt"></param>
    public void SendSellReq(uint fish_id, uint sell_cnt, Action<string, SellResp> callback = null)
    {
        SellReq req = new SellReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;
        req.FishId = fish_id;
        req.Count = sell_cnt;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.SellReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            SellResp resp = SellResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                Debug.Log($"[WebSocket] AquariumReq Success");
                Singleton<AquariumDataModel>.Instance.UpdateFishData(fish_id, resp.ResidueCount, false);
                Singleton<BagMgr>.Instance.SetShellNum(resp.Balance);
                if (callback == null) MessageCenter.SendMessage("FishSellMsg", "Success", resp);
                callback?.Invoke("Success", resp);
            }
            else
            {
                if (callback == null) MessageCenter.SendMessage("FishSellMsg", "Fail", null);
                callback?.Invoke("Fail", resp);
            }

        });
    }

    public void SendRoomEnter(uint sceneId, Action<EnterTreasureResp> callback = null)
    {
        EnterTreasureReq enterTreasureReq = new EnterTreasureReq();
        enterTreasureReq.UserId = ManageMentClass.DataManagerClass.userId;
        enterTreasureReq.FromUserId = ManageMentClass.DataManagerClass.InviteFromUserId;
        enterTreasureReq.SceneId = sceneId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.EnterTreasureReq, enterTreasureReq, (code, bytes) =>
        {
            //房间信息
            EnterTreasureResp enterTreasureResp = EnterTreasureResp.Parser.ParseFrom(bytes);
            ManageMentClass.DataManagerClass.roomId = enterTreasureResp.RoomInfo.RoomId;
            ManageMentClass.CurSyncPlayerController.SetRoomData(enterTreasureResp.RoomInfo);
            //聊天
            ManageMentClass.DataManagerClass.YXAccid = enterTreasureResp.YunxinAccid;
            ManageMentClass.DataManagerClass.YXToken = enterTreasureResp.YunxinToken;
            ManageMentClass.DataManagerClass.ChatRoomId = enterTreasureResp.RoomInfo.ChatRoomId;
            ManageMentClass.DataManagerClass.ChatRoomAddr = enterTreasureResp.RoomInfo.ChatRoomAddr.ToArray();
            callback?.Invoke(enterTreasureResp);
        });
    }

    public void SendFreeShellCount(Action<GameCountResp> callback = null)
    {
        GameCountReq countReq = new GameCountReq();
        countReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.GameCountReq, countReq, (code, dataBytes) =>
        {
            GameCountResp resp = GameCountResp.Parser.ParseFrom(dataBytes);
            Singleton<RainbowBeachDataModel>.Instance.SetFreeShovleCount(resp.Count);
            callback?.Invoke(resp);
        });
    }

    public void SendGetBagItem(Action<BagInfoResp> callback = null)
    {
        Singleton<BagMgr>.Instance.Clear();
        BagInfoReq bagInfoReq = new BagInfoReq();
        bagInfoReq.UserId = ManageMentClass.DataManagerClass.userId;
        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.BagInfoReq, bagInfoReq, (code, bytes) =>
        {
            BagInfoResp bagInfoResp = BagInfoResp.Parser.ParseFrom(bytes);
            var tools = bagInfoResp.Tools;
            foreach (var tool in tools)
            {
                Singleton<BagMgr>.Instance.UpdateItem(tool.ToolId, tool.ToolCount);
            }
            callback?.Invoke(bagInfoResp);
        });
    }
}
