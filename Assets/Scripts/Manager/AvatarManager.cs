using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Treasure;
using Google.Protobuf.Collections;
using UIFW;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 一级类型
/// </summary>
public enum Avatar_Type1
{
    //套装
    suit = 1,
    //头部
    heat = 2,
    //身体
    body = 3,
    //肤色
    skinColor = 4,
    //五官
    Features = 5

}
/// <summary>
/// 二级类型
/// </summary>
public enum Avatar_Type2
{
    //人物
    hotman = 1,
    //套装
    suit = 2,
    //头发
    hair = 3,
    //眼镜
    glasses = 4,
    //耳饰
    earring = 5,
    //项链
    necklace = 6,
    //上装
    Coat = 7,
    //下装
    Pants = 8,
    //鞋子
    shoes = 9,
    //腕饰
    wrist = 10,
    //眉毛
    eyebrow = 11,
    //肤色
    skinColor = 12,
    //脸型
    feature = 13,
    //眼睛
    eyes = 14,
    //嘴巴
    mouth = 15,
    //鼻子
    nose = 16,
    //耳朵
    ear = 17,
}

/// <summary>
/// 裸模类型
/// </summary>
public enum BodyType
{
    //胳膊
    Arm = 1,
    //手
    Hand = 2,
    //头
    Head = 3,
    //腿
    Leg = 4
}
public enum BodyActiveType
{
    //不显示胳膊和腿
    AllClose = 0,
    //显示手臂和腿
    AllShow = 1,
    //仅显示手臂
    ArmShow = 2,
    //仅显示腿
    LegShow = 3
}
public class AvatarManager : MonoBehaviour
{
    private static AvatarManager _instance = null;
    private GameObject playerGamePlayer = null;

    public GameObject avatarGamePlayer = null;

    private Transform _TransAvatarMgr = null;
    private GameObject clothingRootNode = null;
    private GameObject skeletonNode = null;


    //个人形象服装ID数据（此数据和服务器保持同步，用于接收服务器数据） 
    public MyOutFitSaveReqData myOutFitAvatarIdData = new MyOutFitSaveReqData();

    //个人形象服装ID 临时数据（此数据用于临时替换服装）
    public MyOutFitSaveReqData myOutFitAvatarIdData_tem = new MyOutFitSaveReqData();

    public Dictionary<int, int> myHeentAndColorData_tem = new Dictionary<int, int>();

    private int nowPlayerItemID = 0;// 0 为可换装的人物模型

    private int uiPlayerItemID = 0; // UI 层显示的换装模型ID

    //人物身上存在的模型(typeID, 数据)
    public Dictionary<int, PlayerAvatarData> PlayerAvatarModelDic = new Dictionary<int, PlayerAvatarData>();
    //人物身上存在的裸模
    public Dictionary<int, GameObject> PlayerBodyModelDic = new Dictionary<int, GameObject>();


    //UI显示人物身上存在的模型(typeID, 数据)
    public Dictionary<int, PlayerAvatarData> UIAvatarModelDic = new Dictionary<int, PlayerAvatarData>();
    //UI显示人物身上存在的裸模
    public Dictionary<int, GameObject> UIBodyModelDic = new Dictionary<int, GameObject>();


    //所有被实例化出来的衣服模型的对象池
    public Dictionary<int, List<GameObject>> AllAvatarModelDic = new Dictionary<int, List<GameObject>>();
    //所有被实例化出来的裸模的对象池
    public Dictionary<int, List<GameObject>> AllBodyModelDic = new Dictionary<int, List<GameObject>>();


    private List<int> listTypeIndex = new List<int>();


    private Dictionary<ulong, GameObject> AllOtherPlayerObj = new Dictionary<ulong, GameObject>();

    private Dictionary<ulong, OtherPlayerAvatarData> AllOtherPlayerAvatarData = new Dictionary<ulong, OtherPlayerAvatarData>();

    private List<SkinnedMeshRenderer> listSkinnedMeshRenderer = new List<SkinnedMeshRenderer>();

    private string[] BlendShapesName = new string[] { "FACE_01_JawOpen", "FACE_01_UpperEyelidLDown", "FACE_01_UpperEyelidRDown", "FACE_01_LowerEyelidLUp_2", "FACE_01_LowerEyelidRUp_2", "FACE_01_EyesLookDown", "FACE_01_EyesLookUp", "FACE_01_EyesLookL", "FACE_01_EyesLookR", "FACE_01_EyebrowLSad", "FACE_01_EyebrowRSad", "FACE_01_EyebrowLUp", "FACE_01_EyebrowRUp", "FACE_01_EyebrowLDown", "FACE_01_EyebrowRDown", "FACE_01_JawLeft", "FACE_01_JawRight", "FACE_01_SmileL", "FACE_01_SmileR", "FACE_01_LipsNarrowL", "FACE_01_LipsNarrowR", "FACE_01_UpperLipUp", "FACE_01_FrownL", "FACE_01_FrownR", "FACE_01_CheekLInflate", "FACE_01_CheekRInflate", "FACE_01_TongueOut2", "FACC_01_NoseShape_05", "FACC_01_FaceShape_03", "FACC_01_EyesIn", "FACC_01_EyesSmall", "FACC_01_EyesUp", "FACC_01_Lips", "FACC_01_Ears", "FACC_01_EarsElf" };

    public static AvatarManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_AvatarManager").AddComponent<AvatarManager>();
        }
        return _instance;
    }
    private void Awake()
    {
        _TransAvatarMgr = this.gameObject.transform;
        DontDestroyOnLoad(_TransAvatarMgr);
        if (clothingRootNode == null)
        {
            clothingRootNode = new GameObject("_ClothingRootNode");
            DontDestroyOnLoad(clothingRootNode.transform);
        }
    }

    #region 封装的换装相关的接口（用于外部调用）

    /// <summary>
    /// 获取其他玩家的人物模型（同步时，重复获取人物的object）
    ///  参数 暂时为id。 应该传进来一个 roominfo
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject GetOtherPlayerPreFun(RoomUserInfo roomUserInfo)
    {
        //首先把之前模型身上的全部删除
        //然后添加新的模型
        //然后刷新骨骼
        Debug.Log("RoomuserInfo的长度： " + roomUserInfo.AvatarIds.Count + "    内容： " + roomUserInfo.AvatarIds + "   userId=" + roomUserInfo.UserId);
        avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(roomUserInfo.AvatarIds[0]);
        //要根据ID来判断此模型是否存在来决定 是增加，还是替换
        int playerModelTypeID = -1;
        if (avatarItem != null)
        {
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.hotman)
            {
                //特殊人物
                playerModelTypeID = avatarItem.avatar_id;
            }
            else
            {
                //换装人物
                playerModelTypeID = 0;
            }
        }
        else
        {
            Debug.LogError("avataritem为空");
        }
        if (!AllOtherPlayerAvatarData.ContainsKey(roomUserInfo.UserId))
        {
            //  playerModelTypeID = 1001;
            // OtherPlayerAvatarData otherPlayerAvatarData = new OtherPlayerAvatarData();
            GameObject charObj = CharacterManager.Instance().GetOtherPlayerHotManFun(playerModelTypeID);

            if (charObj.GetComponent<EventItemClick>() == null)
            {
                charObj.AddComponent<EventItemClick>();
            }
            if (charObj.GetComponent<PlayerItem>() != null)
            {
                charObj.GetComponent<PlayerItem>().SetUserID(roomUserInfo.UserId);
            }
            OtherPlayerAvatarData otherPlayerAvatarData = new OtherPlayerAvatarData(playerModelTypeID, charObj);
            AllOtherPlayerAvatarData.Add(roomUserInfo.UserId, otherPlayerAvatarData);
            Debug.Log("走的这里里面：" + charObj.name);
            if (playerModelTypeID != 0)
            {
                return charObj;
            }
            else
            {
                //   AddOtherBodyModelFun((int)BodyType.Head, roomUserInfo.UserId);
                Debug.Log("roomUserInfoAvatarIds:  " + roomUserInfo.AvatarIds + "     userInfo: " + roomUserInfo.UserId);
                for (int i = 0; i < roomUserInfo.AvatarIds.Count; i++)
                {
                    Debug.Log("输出一下roomUserInfo.AvatarIds[i]具体的值的内容： " + roomUserInfo.AvatarIds[i]);
                    avatar avatarItemData = ManageMentClass.DataManagerClass.GetAvatarTableFun(roomUserInfo.AvatarIds[i]);
                    Debug.Log("avataritemdata的值： " + JsonConvert.SerializeObject(avatarItemData));

                    if (avatarItemData != null)
                    {
                        ChangeOtherAvatarModelFun(avatarItemData, roomUserInfo.UserId);
                    }
                    else
                    {
                        ChangeOtherAvatarHeentAndColorFun(roomUserInfo.UserId, roomUserInfo.AvatarIds[i]);
                    }

                }
                return charObj;
            }
        }
        else
        {
            return AllOtherPlayerAvatarData[roomUserInfo.UserId].playerModel;
        }
    }

    /// <summary>
    /// 获取用于支持换装界面显示所用的GameObject
    /// </summary>
    /// <returns></returns>
    public GameObject GetUIShowPlayerObjFun()
    {
        Debug.Log("食醋胡天下具体i的链接发链接user ID：" + ManageMentClass.DataManagerClass.userId);
        InitPlayerAvatarDataFun();
        for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
        {
            Debug.Log("这里的数据内容A：   " + myOutFitAvatarIdData.data[i].avatar_id);
        }
        GameObject ShowObject = null;
        if (myOutFitAvatarIdData_tem.data.Count > 0)
        {
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(myOutFitAvatarIdData_tem.data[0].avatar_id);
            if (avatarItem != null)
            {
                if (avatarItem.avatar_type2 == (int)Avatar_Type2.hotman)
                {
                    // 在这里new 克里斯
                    if (avatarGamePlayer != null)
                    {
                        Destroy(avatarGamePlayer);
                        avatarGamePlayer = null;
                    }
                    ShowObject = CharacterManager.Instance().CreatPlayerObjFun(myOutFitAvatarIdData_tem.data[0].avatar_id);
                    uiPlayerItemID = myOutFitAvatarIdData_tem.data[0].avatar_id;
                    // ShowObject.transform.localPosition = new Vector3(playerGamePlayer.transform.position.x, playerGamePlayer.transform.position.y, playerGamePlayer.transform.position.z);
                    // ShowObject.transform.localEulerAngles = new Vector3(playerGamePlayer.transform.localEulerAngles.x, playerGamePlayer.transform.localEulerAngles.y, playerGamePlayer.transform.localEulerAngles.z);
                    ShowObject.SetActive(true);
                    avatarGamePlayer = ShowObject;
                    ShowObject.name = "AvatarUIObj";
                    UnityHelper.SetLayer(ShowObject, 8);
                    CamCtrl camCtrl = ShowObject.GetComponent<CamCtrl>();
                    if (camCtrl == null)
                    {
                        camCtrl = ShowObject.AddComponent<CamCtrl>();
                        camCtrl.modelObj = ShowObject.transform;
                    }

                }
                else
                {
                    if (avatarGamePlayer != null)
                    {
                        Destroy(avatarGamePlayer);
                        avatarGamePlayer = null;
                    }
                    ShowObject = CharacterManager.Instance().CreatPlayerObjFun(0);
                    uiPlayerItemID = 0;
                    //  ShowObject.transform.localPosition = new Vector3(playerGamePlayer.transform.position.x, playerGamePlayer.transform.position.y, playerGamePlayer.transform.position.z);
                    // ShowObject.transform.localEulerAngles = new Vector3(playerGamePlayer.transform.localEulerAngles.x, playerGamePlayer.transform.localEulerAngles.y, playerGamePlayer.transform.localEulerAngles.z);
                    ShowObject.SetActive(true);
                    avatarGamePlayer = ShowObject;
                    ShowObject.name = "AvatarUIObj";
                    for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
                    {
                        Debug.Log("输出一下在这里的数据是什么： " + myOutFitAvatarIdData_tem.data[i].avatar_id);
                        ChangeAvatarFun(myOutFitAvatarIdData_tem.data[i].avatar_id);
                    }
                    UnityHelper.SetLayer(ShowObject, 8);
                    CamCtrl camCtrl = ShowObject.GetComponent<CamCtrl>();
                    if (camCtrl == null)
                    {
                        camCtrl = ShowObject.AddComponent<CamCtrl>();
                        camCtrl.modelObj = ShowObject.transform;
                    }

                    // 在这里new 普通换装人物
                }
            }
            else
            {
                avatarGamePlayer = ShowObject;
            }
        }

        return ShowObject;
    }

    /// <summary>
    /// ��������ģ�ͣ�����Ui��ʾ
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public GameObject GetOtherUIPlayerObjFun(ulong userID)
    {
        if (AllOtherPlayerAvatarData.ContainsKey(userID))
        {
            GameObject obj = Instantiate(AllOtherPlayerAvatarData[userID].playerModel);
            obj.transform.SetParent(CharacterManager.Instance().transform);
            avatarGamePlayer = obj;
            obj.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
            obj.name = "AvatarUIObj";
            UnityHelper.SetLayer(obj, 8);
            CamCtrl camCtrl = obj.GetComponent<CamCtrl>();

            GameObject aaa = obj.transform.Find("Hud").gameObject;
            if (aaa != null)
            {
                Debug.Log("���ҵ���hug");
                aaa.SetActive(false);
            }
            if (camCtrl == null)
            {
                camCtrl = obj.AddComponent<CamCtrl>();
                camCtrl.modelObj = obj.transform;
            }
            return obj;
        }
        avatarGamePlayer = null;
        return null;
    }



    /// <summary>
    /// 回收人物角色
    /// </summary>
    public void RecyclePlayerPreFun(ulong userID)
    {
        ChangePlayerCleanDataFun(userID);
        AllOtherPlayerAvatarData.Remove(userID);
    }
    /// 刷新从服务端接收过来的数据
    /// </summary>
    public void ReceiveOutFitAvatarDataFun(List<OutFitRecData> outFitRecDatas)
    {
        myOutFitAvatarIdData.data.Clear();
        foreach (var data in outFitRecDatas)
        {
            foreach (var secondData in data.second_data)
            {
                foreach (var threeData in secondData.list)
                {
                    if (threeData.status == 1)
                    {
                        MyOutFitRecData outFitData = new MyOutFitRecData();
                        outFitData.avatar_id = threeData.avatar_id;
                        Debug.Log("这里得内容是数据BBB： " + threeData.avatar_id);
                        myOutFitAvatarIdData.data.Add(outFitData);
                    }
                }
            }
        }
        Debug.Log("输出一下从服务器返回过来的值： " + JsonConvert.SerializeObject(myOutFitAvatarIdData.data));
    }
    /// <summary>
    /// 刷新人物
    /// 根据正式数据，刷新人物的服装
    /// </summary>
    public void RefreshPlayerFun()
    {
        if (clothingRootNode == null)
        {
            clothingRootNode = new GameObject("_ClothingRootNode");
            DontDestroyOnLoad(clothingRootNode.transform);
        }
        //初始数据
        InitPlayerAvatarDataFun();
        //初始模型
        InitPlayerAvatarModelFun(true);

        Debug.Log("身上的模型的长度的内容： " + PlayerAvatarModelDic.Count);
        //刷新其他配饰
        RefreshAccessoryModelFun();
    }
    /// <summary>
    /// 换装的方法
    /// </summary>
    /// <param name="itemID">道具ID</param>
    /// <param name="avatar_type1">一级类型</param>
    public void ChangeAvatarFun(int itemID, bool isRealControPlayer = false)
    {
        avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(itemID);
        //要根据ID来判断此模型是否存在来决定 是增加，还是替换
        if (avatarItem != null)
        {

            if (avatarItem.avatar_type2 == (int)Avatar_Type2.hotman)
            {
                if (isRealControPlayer)
                {
                    //清理衣服和裸模
                    RemoveAllModelFun(isRealControPlayer);
                    //更换hotman
                    ChangePlayerModelFun(avatarItem.avatar_id, isRealControPlayer);
                }
                else
                {
                    //删除上个模型，设置新的模型
                    ChangeUIPlayerModelFun(avatarItem.avatar_id);
                }
            }
            else
            {
                if (isRealControPlayer)
                {
                    //更换Hotman
                    ChangePlayerModelFun(0, isRealControPlayer);
                }
                else
                {

                    ChangeUIPlayerModelFun(0);
                }
                //换衣服模型的逻辑
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
                //处理所有默认模型的显示
                DisposeAllDefaultAvatarFun(isRealControPlayer);
            }

        }
        else
        {

            skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(itemID);

            if (skinColor != null)
            {


                if (isRealControPlayer)
                {
                    //更换Hotman
                    ChangePlayerModelFun(0, isRealControPlayer);
                }
                else
                {

                    ChangeUIPlayerModelFun(0);
                }
                //换衣服模型的逻辑
                // ChangeAvatarModelFun(avatarItem, isRealControPlayer);

                GameObject avatarPlayer = playerGamePlayer;
                if (!isRealControPlayer && avatarGamePlayer != null)
                {
                    avatarPlayer = avatarGamePlayer;
                }
                AddBodyModelFun((int)BodyType.Head, avatarPlayer, isRealControPlayer);
                //处理所有默认模型的显示
                DisposeAllDefaultAvatarFun(isRealControPlayer);

                Debug.Log("进入到调整五官和肤色的功能: " + JsonConvert.SerializeObject(skinColor));

                if (skinColor.avatar_type1 == (int)Avatar_Type1.skinColor)
                {
                    //设置肤色
                    //  SetSkinColorFun();
                    //   SetSkinColorFun();


                    if (isRealControPlayer)
                    {
                        if (PlayerBodyModelDic.ContainsKey((int)BodyType.Head))
                        {
                            SetSkinColorFun(PlayerBodyModelDic[(int)BodyType.Head], skinColor);
                        }

                        if (PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.suit))
                        {
                            SetSkinColorFun(PlayerAvatarModelDic[(int)Avatar_Type2.suit].item_obj, skinColor);
                        }

                        if (PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.Coat))
                        {
                            SetSkinColorFun(PlayerAvatarModelDic[(int)Avatar_Type2.Coat].item_obj, skinColor);
                        }

                        if (PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.Pants))
                        {
                            SetSkinColorFun(PlayerAvatarModelDic[(int)Avatar_Type2.Pants].item_obj, skinColor);
                        }

                        if (PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.shoes))
                        {
                            SetSkinColorFun(PlayerAvatarModelDic[(int)Avatar_Type2.shoes].item_obj, skinColor);
                        }

                    }
                    else
                    {
                        if (UIBodyModelDic.ContainsKey((int)BodyType.Head))
                        {
                            SetSkinColorFun(UIBodyModelDic[(int)BodyType.Head], skinColor);
                        }

                        if (UIAvatarModelDic.ContainsKey((int)Avatar_Type2.suit))
                        {
                            SetSkinColorFun(UIAvatarModelDic[(int)Avatar_Type2.suit].item_obj, skinColor);
                        }

                        if (UIAvatarModelDic.ContainsKey((int)Avatar_Type2.Coat))
                        {
                            SetSkinColorFun(UIAvatarModelDic[(int)Avatar_Type2.Coat].item_obj, skinColor);
                        }

                        if (UIAvatarModelDic.ContainsKey((int)Avatar_Type2.Pants))
                        {
                            SetSkinColorFun(UIAvatarModelDic[(int)Avatar_Type2.Pants].item_obj, skinColor);
                        }

                        if (UIAvatarModelDic.ContainsKey((int)Avatar_Type2.shoes))
                        {
                            SetSkinColorFun(UIAvatarModelDic[(int)Avatar_Type2.shoes].item_obj, skinColor);
                        }
                    }
                    ReplaceAvatarTempDataFun(skinColor.avatar_type2, itemID);
                }
                else if (skinColor.avatar_type1 == (int)Avatar_Type1.Features)
                {
                    //设置五官
                    if (isRealControPlayer)
                    {
                        if (PlayerBodyModelDic.ContainsKey((int)BodyType.Head))
                        {
                            if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes)
                            {
                                //设置眼睛颜色
                                SetEyesColorFun(PlayerBodyModelDic[(int)BodyType.Head], skinColor);
                            }
                            else if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes || skinColor.avatar_type2 == (int)Avatar_Type2.feature || skinColor.avatar_type2 == (int)Avatar_Type2.mouth || skinColor.avatar_type2 == (int)Avatar_Type2.nose || skinColor.avatar_type2 == (int)Avatar_Type2.ear)
                            {
                                //设置五官
                                SetHeentFun(PlayerBodyModelDic[(int)BodyType.Head], skinColor);
                            }
                        }
                    }
                    else
                    {

                        if (UIBodyModelDic.ContainsKey((int)BodyType.Head))
                        {
                            if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes)
                            {
                                //设置眼睛颜色
                                SetEyesColorFun(UIBodyModelDic[(int)BodyType.Head], skinColor);
                            }
                            else if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes || skinColor.avatar_type2 == (int)Avatar_Type2.feature || skinColor.avatar_type2 == (int)Avatar_Type2.mouth || skinColor.avatar_type2 == (int)Avatar_Type2.nose || skinColor.avatar_type2 == (int)Avatar_Type2.ear)
                            {
                                //设置五官
                                SetHeentFun(UIBodyModelDic[(int)BodyType.Head], skinColor);
                            }
                        }
                    }
                    ReplaceAvatarTempDataFun(skinColor.avatar_type2, itemID);

                }
            }
            else
            {
                Debug.Log("weikong");
            }

            //换数据
        }
        Debug.Log("换完装的数据：： " + myOutFitAvatarIdData_tem.data.ToJSON());
    }




    /// <summary>
    /// 其他玩家换装的方法
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="avatarIds"></param>
    /// <returns></returns>
    public GameObject ChangeOtherAvatarFun(ulong userID, RepeatedField<long> avatarIds)
    {
        avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(avatarIds[0]);
        //要根据ID来判断此模型是否存在来决定 是增加，还是替换
        int playerModelTypeID = -1;
        if (avatarItem != null)
        {
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.hotman)
            {
                //特殊人物
                playerModelTypeID = avatarItem.avatar_id;
            }
            else
            {
                //换装人物
                playerModelTypeID = 0;
            }
        }
        else
        {
            Debug.LogError("avataritem为空");
        }

        // 换装
        if (AllOtherPlayerAvatarData[userID].modelType == playerModelTypeID)
        {
            if (playerModelTypeID != 0)
            {
                return AllOtherPlayerAvatarData[userID].playerModel;
            }
            else
            {
                for (int i = 0; i < avatarIds.Count; i++)
                {
                    //换装
                    avatar avatarItemData = ManageMentClass.DataManagerClass.GetAvatarTableFun(avatarIds[i]);
                    if (avatarItemData != null)
                    {
                        ChangeOtherAvatarModelFun(avatarItemData, userID);
                    }
                    else
                    {

                        skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(avatarIds[i]);

                        if (skinColor != null)
                        {
                            Debug.Log("进入到调整五官和肤色的功能: " + JsonConvert.SerializeObject(skinColor));

                            if (skinColor.avatar_type1 == (int)Avatar_Type1.skinColor)
                            {
                                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.suit))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.suit].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Coat))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Coat].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Pants))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Pants].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.shoes))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.shoes].item_obj, skinColor);
                                }
                            }
                            else if (skinColor.avatar_type1 == (int)Avatar_Type1.Features)
                            {
                                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                                {
                                    if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes)
                                    {
                                        //设置眼睛颜色
                                        SetEyesColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                    }
                                    else if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes || skinColor.avatar_type2 == (int)Avatar_Type2.feature || skinColor.avatar_type2 == (int)Avatar_Type2.mouth || skinColor.avatar_type2 == (int)Avatar_Type2.nose || skinColor.avatar_type2 == (int)Avatar_Type2.ear)
                                    {
                                        //设置五官
                                        SetHeentFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return AllOtherPlayerAvatarData[userID].playerModel;
        }
        else
        {

            Debug.Log("在这里输出下环任务了");
            Vector3 lastPos = AllOtherPlayerAvatarData[userID].playerModel.transform.position;
            Vector3 lastRoation = AllOtherPlayerAvatarData[userID].playerModel.transform.eulerAngles;
            AllOtherPlayerAvatarData[userID].modelType = playerModelTypeID;
            GameObject charObj = CharacterManager.Instance().GetOtherPlayerHotManFun(playerModelTypeID);
            charObj.SetActive(true);
            // 换人
            if (playerModelTypeID == 0)
            {
                //切换为可换装玩家

                ChangePlayerCleanDataFun(userID);
                AllOtherPlayerAvatarData[userID].playerModel = charObj;
                charObj.transform.position = lastPos;
                charObj.transform.eulerAngles = lastRoation;
                for (int i = 0; i < avatarIds.Count; i++)
                {
                    //换装
                    avatar avatarItemData = ManageMentClass.DataManagerClass.GetAvatarTableFun(avatarIds[i]);
                    if (avatarItemData != null)
                    {
                        ChangeOtherAvatarModelFun(avatarItemData, userID);
                    }
                    else
                    {
                        skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(avatarIds[i]);
                        if (skinColor != null)
                        {
                            Debug.Log("进入到调整五官和肤色的功能: " + JsonConvert.SerializeObject(skinColor));

                            if (skinColor.avatar_type1 == (int)Avatar_Type1.skinColor)
                            {
                                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.suit))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.suit].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Coat))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Coat].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Pants))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Pants].item_obj, skinColor);
                                }

                                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.shoes))
                                {
                                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.shoes].item_obj, skinColor);
                                }
                            }
                            else if (skinColor.avatar_type1 == (int)Avatar_Type1.Features)
                            {
                                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                                {
                                    if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes)
                                    {
                                        //设置眼睛颜色
                                        SetEyesColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                    }
                                    else if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes || skinColor.avatar_type2 == (int)Avatar_Type2.feature || skinColor.avatar_type2 == (int)Avatar_Type2.mouth || skinColor.avatar_type2 == (int)Avatar_Type2.nose || skinColor.avatar_type2 == (int)Avatar_Type2.ear)
                                    {
                                        //设置五官
                                        SetHeentFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // 切换为特殊玩家
                // 清理掉之前 可换装玩家
                ChangePlayerCleanDataFun(userID);
                AllOtherPlayerAvatarData[userID].playerModel = charObj;
                charObj.transform.position = lastPos;
                charObj.transform.eulerAngles = lastRoation;
            }
            return AllOtherPlayerAvatarData[userID].playerModel;
        }
    }


    /// <summary>
    /// 保存服装ID数据
    /// （将人物身上所有穿戴的数据，保存到正式的穿戴数据里，用于传给服务器）
    /// </summary>
    public void SaveOutfitAvatarIDFun()
    {
        myOutFitAvatarIdData.data.Clear();
        for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
        {
            MyOutFitRecData _data = new MyOutFitRecData();
            _data.avatar_id = myOutFitAvatarIdData_tem.data[i].avatar_id;
            myOutFitAvatarIdData.data.Add(_data);
            ChangeAvatarFun(myOutFitAvatarIdData_tem.data[i].avatar_id, true);
        }
        CharacterManager.Instance().PlayOtherPlayerSpecialEffect(playerGamePlayer);
    }
    /// <summary>
    /// 往正式数据里直接添加服装
    /// </summary>
    /// <param name="itemID"></param>
    public void AddOfficalAvatarDataFun(int itemID)
    {
        avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(itemID);
        if (avatarItem != null)
        {
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.hotman)
            {
                //删除所有的衣服
                RemoveAllAatarOfficialDataFun();
                //增加人物ID信息
                AddAvatarOfficialDataFun(avatarItem.avatar_id);
            }
            else
            {
                int hotmanID = FindOfficialDataFun((int)Avatar_Type2.hotman);
                if (hotmanID != -1)
                {
                    RemoveAvatarOfficialDataFun(hotmanID);
                }
                ChangeOfficialAvatarDataFun(avatarItem);
                DisposeAllAvatarOfficialDataFun();
            }
            ChangeAvatarFun(itemID, true);
        }
    }


    /// <summary>
    /// 回收UI层显示的换装人物
    /// </summary>
    public void RecycleUIShowPlayerFun()
    {
        RemoveAllModelFun(false);
        if (avatarGamePlayer != null)
        {
            Destroy(avatarGamePlayer);
            avatarGamePlayer = null;
        }
    }


    /// <summary>
    /// 设置人物的打开关闭
    /// </summary>
    /// <param name="isActive"></param>
 /*   public void SetOpenActiveFun(bool isActive)
    {
        if (clothingRootNode == null)
        {
            clothingRootNode = new GameObject("_ClothingRootNode");
            DontDestroyOnLoad(clothingRootNode.transform);
        }
        RefreshPlayerGameObject();
        if (isActive != playerGamePlayer.activeSelf)
        {
            playerGamePlayer.SetActive(isActive);
            var avatarKeys = PlayerAvatarModelDic.Keys;
            var bodyKeys = PlayerBodyModelDic.Keys;
            foreach (var key in avatarKeys)
            {
                if (isActive)
                {
                    PlayerAvatarModelDic[key].item_obj.transform.parent = playerGamePlayer.transform;
                }
                else
                {
                    PlayerAvatarModelDic[key].item_obj.transform.parent = clothingRootNode.transform;
                }
                PlayerAvatarModelDic[key].item_obj.SetActive(isActive);
            }
            foreach (var key in bodyKeys)
            {
                if (isActive)
                {
                    PlayerBodyModelDic[key].transform.parent = playerGamePlayer.transform;
                }
                else
                {
                    PlayerBodyModelDic[key].transform.parent = clothingRootNode.transform;
                }
                PlayerBodyModelDic[key].SetActive(isActive);
            }
        }
    }
*/

    #endregion

    #region 封装起来的逻辑方法（外部调用不需要关心）

    /// <summary>
    /// 其他玩家换装更换模型时，清理上次的模型和数据
    /// </summary>
    private void ChangePlayerCleanDataFun(ulong userID)
    {
        Destroy(AllOtherPlayerAvatarData[userID].playerModel);
        AllOtherPlayerAvatarData[userID].HeadBodyObj = null;
        AllOtherPlayerAvatarData[userID].avatarIds.Clear();
        AllOtherPlayerAvatarData[userID].avatarObjs.Clear();
    }
    /// <summary>
    /// 刷新其他配饰
    /// </summary>
    private void RefreshAccessoryModelFun()
    {
        listTypeIndex.Clear();
        var keys = PlayerAvatarModelDic.Keys;
        foreach (var key in keys)
        {
            if (!IsHaveTypeFun(key))
            {
                listTypeIndex.Add(key);
            }
        }
        for (int i = 0; i < listTypeIndex.Count; i++)
        {
            RemoveAvatarModelFun(listTypeIndex[i], true);
        }
    }
    private bool IsHaveTypeFun(int TypeID)
    {
        for (int i = 0; i < myOutFitAvatarIdData.data.Count; i++)
        {
            // Debug.LogError("输出一下这里的id的值：： " + myOutFitAvatarIdData.data[i].avatar_id);
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(myOutFitAvatarIdData.data[i].avatar_id);
            if (avatarItem != null)
            {
                if (avatarItem.avatar_type2 == TypeID)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }
        return false;
    }
    /// <summary>
    /// 换衣服换正式数据
    /// </summary>
    /// <param name="avatarItem"></param>
    private void ChangeOfficialAvatarDataFun(avatar avatarItem)
    {
        int avatarId = FindOfficialDataFun(avatarItem.avatar_id);
        if (avatarId != -1 && avatarId == avatarItem.avatar_id)
        {
            //更换的为相同的衣服
            return;
        }
        if (avatarItem.avatar_type2 == (int)Avatar_Type2.suit)
        {
            //更换的为套装的情况下
            Debug.Log("更换套装：这里的值");

            int coatID = FindOfficialDataFun((int)Avatar_Type2.Coat);
            if (coatID != -1)
            {
                RemoveAvatarOfficialDataFun(coatID);
            }
            int pantsID = FindOfficialDataFun((int)Avatar_Type2.Pants);
            if (pantsID != -1)
            {
                RemoveAvatarOfficialDataFun(pantsID);
            }
            int suitID = FindOfficialDataFun((int)Avatar_Type2.suit);
            if (suitID != -1)
            {
                RemoveAvatarOfficialDataFun(suitID);
            }
            //添加新模型
            AddAvatarOfficialDataFun(avatarItem.avatar_id);
        }
        else
        {
            //更换为单件的情况下（判断是否需要把套装脱掉，如果有，要把套装替换掉）
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.Coat || avatarItem.avatar_type2 == (int)Avatar_Type2.Pants)
            {
                int suitID = FindOfficialDataFun((int)Avatar_Type2.suit);
                if (suitID != -1)
                {
                    RemoveAvatarOfficialDataFun(suitID);
                }
            }
            //移除老模型
            int avatarID = FindOfficialDataFun(avatarItem.avatar_type2);
            if (avatarID != -1)
            {
                RemoveAvatarOfficialDataFun(avatarID);
            }
            //添加新模型
            AddAvatarOfficialDataFun(avatarItem.avatar_id);
        }
    }
    /// <summary>
    /// 处理所有默认的模型得正式数据
    /// </summary>
    private void DisposeAllAvatarOfficialDataFun()
    {

        int hairID = FindOfficialDataFun((int)Avatar_Type2.hair);
        if (hairID == -1)
        {
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.hair, 1);
            ChangeOfficialAvatarDataFun(avatarItem);
        }

        int suitID = FindOfficialDataFun((int)Avatar_Type2.suit);
        if (suitID == -1)
        {
            //外套
            int coatID = FindOfficialDataFun((int)Avatar_Type2.Coat);
            if (coatID == -1)
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Coat, 1);
                ChangeOfficialAvatarDataFun(avatarItem);
            }
            //裤子
            int pantsID = FindOfficialDataFun((int)Avatar_Type2.Pants);
            if (pantsID == -1)
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Pants, 1);
                ChangeOfficialAvatarDataFun(avatarItem);
            }
        }
        //默认鞋子
        int shoesID = FindOfficialDataFun((int)Avatar_Type2.shoes);
        if (shoesID == -1)
        {
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.shoes, 1);
            ChangeOfficialAvatarDataFun(avatarItem);
        }
        //默认眉毛
        int eysbrowID = FindOfficialDataFun((int)Avatar_Type2.eyebrow);
        if (eysbrowID == -1)
        {
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.eyebrow, 1);
            ChangeOfficialAvatarDataFun(avatarItem);
        }
    }
    /// <summary>
    /// 查找正式数据
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="typeID"></param>
    /// <returns></returns>
    private int FindOfficialDataFun(int typeID)
    {
        for (int i = 0; i < myOutFitAvatarIdData.data.Count; i++)
        {
            avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarTableFun(myOutFitAvatarIdData.data[i].avatar_id);

            if (avatarItem != null)
            {
                if (avatarItem.avatar_type2 == typeID)
                {
                    return myOutFitAvatarIdData.data[i].avatar_id;
                }
            }
        }
        return -1;
    }
    /// <summary>
    /// 将数据初始化
    /// （将服装的临时数据，初始化为服装正式数据）
    /// </summary>
    private void InitPlayerAvatarDataFun()
    {
        myOutFitAvatarIdData_tem.data.Clear();
        myHeentAndColorData_tem.Clear();
        Debug.Log("输出一下正式数据内容AAA： " + myOutFitAvatarIdData.data.ToJSON());
        for (int i = 0; i < myOutFitAvatarIdData.data.Count; i++)
        {
            MyOutFitRecData _data = new MyOutFitRecData();
            _data.avatar_id = myOutFitAvatarIdData.data[i].avatar_id;
            myOutFitAvatarIdData_tem.data.Add(_data);
        }
    }
    /// <summary>
    /// 将模型根据数据初始化
    /// </summary>
    private void InitPlayerAvatarModelFun(bool isRealControPlayer)
    {
        Debug.Log("输出一下正式数据内容BBB:   " + myOutFitAvatarIdData_tem.data.ToJSON());
        for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
        {

            ChangeAvatarFun(myOutFitAvatarIdData_tem.data[i].avatar_id, isRealControPlayer);
        }
    }
    private void ChangeUIPlayerModelFun(int avatar_id)
    {
        if (avatar_id != uiPlayerItemID)
        {

            if (avatar_id != 0)
            {
                RemoveAllAatarTempDataFun();
                AddAvatarTempDataFun(avatar_id);
            }
            else
            {
                RemoveAllAatarTempDataFun();
            }
            uiPlayerItemID = avatar_id;
            GameObject ShowObject = CharacterManager.Instance().CreatPlayerObjFun(avatar_id);
            ShowObject.name = "AvatarUIObj";
            ShowObject.SetActive(true);
            UnityHelper.SetLayer(ShowObject, 8);
            ShowObject.transform.localPosition = new Vector3(avatarGamePlayer.transform.position.x, avatarGamePlayer.transform.position.y, avatarGamePlayer.transform.position.z);
            ShowObject.transform.localEulerAngles = new Vector3(avatarGamePlayer.transform.localEulerAngles.x, avatarGamePlayer.transform.localEulerAngles.y, avatarGamePlayer.transform.localEulerAngles.z);
            CamCtrl camCtrl = ShowObject.GetComponent<CamCtrl>();
            if (camCtrl == null)
            {
                camCtrl = ShowObject.AddComponent<CamCtrl>();
                camCtrl.modelObj = ShowObject.transform;
            }
            if (avatar_id != 0)
            {
                RemoveAllModelFun(false);
            }
            if (avatarGamePlayer != null)
            {
                Destroy(avatarGamePlayer);
                avatarGamePlayer = null;
            }
            avatarGamePlayer = ShowObject;
        }
    }



    /// <summary>
    /// 更换人物模型（更换为hotman）
    /// </summary>
    private void ChangePlayerModelFun(int avatar_id, bool isRealControPlayer)
    {
        if (nowPlayerItemID != 0)
        {
            RemoveAvatarTempDataFun(nowPlayerItemID);
        }
        if (avatar_id != 0)
        {
            AddAvatarTempDataFun(avatar_id);
        }
        if (avatar_id != nowPlayerItemID)
        {
            //更改整个人物
            nowPlayerItemID = avatar_id;

            //刷新最新的人物
            if (isRealControPlayer)
            {
                Debug.Log("输出一下在这里更换人物了");
                CharacterManager.Instance().SetOneSelfHotmanFun(nowPlayerItemID);
            }
            else
            {
                GameObject newObj = CharacterManager.Instance().CreatPlayerObjFun(nowPlayerItemID);
                newObj.transform.localPosition = new Vector3(avatarGamePlayer.transform.position.x, avatarGamePlayer.transform.position.y, avatarGamePlayer.transform.position.z);
                newObj.transform.localEulerAngles = new Vector3(avatarGamePlayer.transform.localEulerAngles.x, avatarGamePlayer.transform.localEulerAngles.y, avatarGamePlayer.transform.localEulerAngles.z);
                newObj.SetActive(true);
                avatarGamePlayer = newObj;
            }
        }
        else if (CharacterManager.Instance().GetPlayerObj() == null)
        {
            nowPlayerItemID = avatar_id;
            CharacterManager.Instance().SetOneSelfHotmanFun(nowPlayerItemID);

            CharacterManager.Instance().CreatPlayerObjFun(nowPlayerItemID);
        }
        RefreshPlayerGameObject();
    }



    /// <summary>
    /// 移除所有模型
    /// </summary>
    private void RemoveAllModelFun(bool isRealControPlayer)
    {
        Debug.Log("在此刻清清除了所有的服装数据");
        //移除所有衣服
        RemoveAllAvatarFun(isRealControPlayer);
        //移除所有裸模
        RemoveAllBodyFun(isRealControPlayer);

    }
    /// <summary>
    /// 移除所有的衣服
    /// </summary>
    private void RemoveAllAvatarFun(bool isRealControPlayer)
    {
        //遍历从套装到眉毛
        for (int i = (int)Avatar_Type2.suit; i <= (int)Avatar_Type2.eyebrow; i++)
        {
            RemoveAvatarModelFun(i, isRealControPlayer);
        }

    }
    /// <summary>
    /// 移除所有裸模
    /// </summary>
    private void RemoveAllBodyFun(bool isRealControPlayer)
    {
        //遍历从套装到眉毛
        for (int i = (int)BodyType.Arm; i <= (int)BodyType.Leg; i++)
        {
            RemoveBodyModelFun(i, isRealControPlayer);
        }
    }
    /// <summary>
    /// 换衣服模型
    /// </summary>
    private void ChangeAvatarModelFun(avatar avatarItem, bool isRealControPlayer)
    {
        GameObject avatarPlayer = playerGamePlayer;
        if (!isRealControPlayer && avatarGamePlayer != null)
        {
            avatarPlayer = avatarGamePlayer;
        }
        if (isRealControPlayer)
        {
            if (PlayerAvatarModelDic.ContainsKey(avatarItem.avatar_type2))
            {
                if (PlayerAvatarModelDic[avatarItem.avatar_type2].item_id == avatarItem.avatar_id)
                {
                    //更换的为相同的衣服
                    return;
                }
            }
        }
        else
        {
            if (UIAvatarModelDic.ContainsKey(avatarItem.avatar_type2))
            {
                if (UIAvatarModelDic[avatarItem.avatar_type2].item_id == avatarItem.avatar_id)
                {
                    //更换的为相同的衣服
                    return;
                }
            }
        }
        if (avatarItem.avatar_type2 == (int)Avatar_Type2.suit)
        {
            //更换的为套装的情况下
            Debug.Log("更换套装：这里的值");
            RemoveAvatarModelFun((int)Avatar_Type2.Coat, isRealControPlayer);
            RemoveAvatarModelFun((int)Avatar_Type2.Pants, isRealControPlayer);
            RemoveAvatarModelFun((int)Avatar_Type2.suit, isRealControPlayer);
            //添加新模型
            AddAvatarModelFun(avatarItem, avatarPlayer, isRealControPlayer);
        }
        else
        {
            //更换为单件的情况下（判断是否需要把套装脱掉，如果有，要把套装替换掉）
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.Coat || avatarItem.avatar_type2 == (int)Avatar_Type2.Pants)
            {
                RemoveAvatarModelFun((int)Avatar_Type2.suit, isRealControPlayer);
            }
            //移除老模型
            RemoveAvatarModelFun(avatarItem.avatar_type2, isRealControPlayer);
            //添加新模型
            AddAvatarModelFun(avatarItem, avatarPlayer, isRealControPlayer);
        }
        //头不存在添加头
        AddBodyModelFun((int)BodyType.Head, avatarPlayer, isRealControPlayer);
        //处理所有裸模
        //  DisposeAllBodyFun(avatarItem);
    }
    /// <summary>
    /// 换其他人物衣服模型
    /// </summary>
    private void ChangeOtherAvatarModelFun(avatar avatarItem, ulong roomUserInfoId)
    {
        if (AllOtherPlayerAvatarData[roomUserInfoId].avatarObjs.ContainsKey(avatarItem.avatar_type2))
        {
            if (AllOtherPlayerAvatarData[roomUserInfoId].avatarObjs[avatarItem.avatar_type2].item_id == avatarItem.avatar_id)
            {
                // 更换的为相同的衣服
                return;
            }
        }
        if (avatarItem.avatar_type2 == (int)Avatar_Type2.suit)
        {
            //更换的为套装的情况下
            Debug.Log("更换套装：这里的值");
            RemoveOtherAvatarModelFun((int)Avatar_Type2.Coat, roomUserInfoId);
            RemoveOtherAvatarModelFun((int)Avatar_Type2.Pants, roomUserInfoId);
            RemoveOtherAvatarModelFun((int)Avatar_Type2.suit, roomUserInfoId);
            //添加新模型
            AddOtherAvatarModelFun(avatarItem, roomUserInfoId);
        }
        else
        {
            //更换为单件的情况下（判断是否需要把套装脱掉，如果有，要把套装替换掉）
            if (avatarItem.avatar_type2 == (int)Avatar_Type2.Coat || avatarItem.avatar_type2 == (int)Avatar_Type2.Pants)
            {
                RemoveOtherAvatarModelFun((int)Avatar_Type2.suit, roomUserInfoId);
            }
            //移除老模型
            RemoveOtherAvatarModelFun(avatarItem.avatar_type2, roomUserInfoId);
            //添加新模型
            AddOtherAvatarModelFun(avatarItem, roomUserInfoId);
        }
        //头不存在添加头
        AddOtherBodyModelFun((int)BodyType.Head, roomUserInfoId);
        //处理所有裸模
        //  DisposeAllBodyFun(avatarItem);
    }

    private void ChangeOtherAvatarHeentAndColorFun(ulong userID, long avatarID)
    {
        skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(avatarID);
        if (skinColor != null)
        {
            Debug.Log("进入到调整五官和肤色的功能: " + JsonConvert.SerializeObject(skinColor));

            if (skinColor.avatar_type1 == (int)Avatar_Type1.skinColor)
            {
                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                {
                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                }

                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.suit))
                {
                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.suit].item_obj, skinColor);
                }

                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Coat))
                {
                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Coat].item_obj, skinColor);
                }

                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.Pants))
                {
                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.Pants].item_obj, skinColor);
                }

                if (AllOtherPlayerAvatarData[userID].avatarObjs.ContainsKey((int)Avatar_Type2.shoes))
                {
                    SetSkinColorFun(AllOtherPlayerAvatarData[userID].avatarObjs[(int)Avatar_Type2.shoes].item_obj, skinColor);
                }
            }
            else if (skinColor.avatar_type1 == (int)Avatar_Type1.Features)
            {

                if (AllOtherPlayerAvatarData[userID].HeadBodyObj != null)
                {
                    if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes)
                    {
                        //设置眼睛颜色
                        SetEyesColorFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                    }
                    else if (skinColor.avatar_type2 == (int)Avatar_Type2.eyes || skinColor.avatar_type2 == (int)Avatar_Type2.feature || skinColor.avatar_type2 == (int)Avatar_Type2.mouth || skinColor.avatar_type2 == (int)Avatar_Type2.nose || skinColor.avatar_type2 == (int)Avatar_Type2.ear)
                    {
                        //设置五官
                        SetHeentFun(AllOtherPlayerAvatarData[userID].HeadBodyObj, skinColor);
                    }

                }
            }
        }
    }

    /// <summary>
    /// 处理所有默认的模型
    /// </summary>
    void DisposeAllDefaultAvatarFun(bool isRealControPlayer)
    {
        if (isRealControPlayer)
        {
            //默认头发
            if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.hair))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.hair, 1);
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
            if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.suit))
            {
                //默认外套
                if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.Coat))
                {
                    avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Coat, 1);
                    ChangeAvatarModelFun(avatarItem, isRealControPlayer);
                }
                //默认裤子
                if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.Pants))
                {
                    avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Pants, 1);
                    ChangeAvatarModelFun(avatarItem, isRealControPlayer);
                }
            }
            //默认鞋子
            if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.shoes))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.shoes, 1);
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
            //默认眉毛
            if (!PlayerAvatarModelDic.ContainsKey((int)Avatar_Type2.eyebrow))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.eyebrow, 1);
                if (avatarItem == null)
                {
                    Debug.Log("输出一下这个得avataritem为空");
                }
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
        }
        else
        {
            //默认头发
            if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.hair))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.hair, 1);
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
            if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.suit))
            {
                //默认外套
                if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.Coat))
                {
                    avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Coat, 1);
                    ChangeAvatarModelFun(avatarItem, isRealControPlayer);
                }
                //默认裤子
                if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.Pants))
                {
                    avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.Pants, 1);
                    ChangeAvatarModelFun(avatarItem, isRealControPlayer);
                }
            }
            //默认鞋子
            if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.shoes))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.shoes, 1);
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
            //默认眉毛
            if (!UIAvatarModelDic.ContainsKey((int)Avatar_Type2.eyebrow))
            {
                avatar avatarItem = ManageMentClass.DataManagerClass.GetAvatarDefaultDataFun((int)Avatar_Type2.eyebrow, 1);
                if (avatarItem == null)
                {
                    Debug.Log("输出一下这个得avataritem为空");
                }
                ChangeAvatarModelFun(avatarItem, isRealControPlayer);
            }
        }
    }
    /// <summary>
    /// 添加裸模
    /// </summary>
    private void AddBodyModelFun(int bodyTypeID, GameObject playerGame, bool isRealControPlayer)
    {

        if (isRealControPlayer)
        {
            if (!PlayerBodyModelDic.ContainsKey(bodyTypeID))
            {
                GameObject bodyObj = TakeBodyPoolFun(bodyTypeID);
                //把模型添加到人物身上
                SetModelLayerFun(bodyObj);
                bodyObj.SetActive(true);
                bodyObj.transform.parent = playerGame.transform;//playerGamePlayer.transform;
                bodyObj.transform.localPosition = Vector3.zero;
                bodyObj.transform.localScale = Vector3.one;
                PlayerBodyModelDic.Add(bodyTypeID, bodyObj);
                RefreshSkeletionFun(bodyObj, playerGame);
            }
        }
        else
        {
            if (!UIBodyModelDic.ContainsKey(bodyTypeID))
            {
                GameObject bodyObj = TakeBodyPoolFun(bodyTypeID);
                //把模型添加到人物身上
                SetModelLayerFun(bodyObj);
                bodyObj.SetActive(true);
                bodyObj.transform.parent = playerGame.transform;//playerGamePlayer.transform;
                bodyObj.transform.localPosition = Vector3.zero;
                bodyObj.transform.localScale = Vector3.one;
                UIBodyModelDic.Add(bodyTypeID, bodyObj);
                RefreshSkeletionFun(bodyObj, playerGame);
            }
        }


    }
    /// <summary>
    /// 添加其他人物裸模
    /// </summary>
    /// <param name="boydrTypeID"></param>
    /// <param name="roomUserId"></param>
    private void AddOtherBodyModelFun(int boydrTypeID, ulong roomUserId)
    {
        if (AllOtherPlayerAvatarData[roomUserId].HeadBodyObj == null)
        {
            GameObject bodyPrefab = InstantiateBodyFun(boydrTypeID);
            SetModelLayerFun(bodyPrefab);
            bodyPrefab.SetActive(true);
            bodyPrefab.transform.parent = AllOtherPlayerAvatarData[roomUserId].playerModel.transform;
            bodyPrefab.transform.localPosition = Vector3.zero;
            bodyPrefab.transform.localScale = Vector3.one;
            AllOtherPlayerAvatarData[roomUserId].HeadBodyObj = bodyPrefab;
            // var clothSkeletonNode = AllOtherPlayerAvatarData[roomUserId].playerModel.transform.Find("Dummy001").gameObject;
            RefreshSkeletionFun(bodyPrefab, AllOtherPlayerAvatarData[roomUserId].playerModel);
            CharacterManager.Instance().PlayOtherPlayerSpecialEffect(bodyPrefab);
        }
    }



    /// <summary>
    /// 移除裸模
    /// </summary>
    private void RemoveBodyModelFun(int bodyTypeID, bool isRealControPlayer)
    {
        if (isRealControPlayer)
        {
            if (PlayerBodyModelDic.ContainsKey(bodyTypeID))
            {
                if (clothingRootNode == null)
                {
                    clothingRootNode = new GameObject("_ClothingRootNode");
                    DontDestroyOnLoad(clothingRootNode.transform);
                }
                PlayerBodyModelDic[bodyTypeID].transform.parent = clothingRootNode.transform;
                PlayerBodyModelDic[bodyTypeID].SetActive(false);
                RecycleBodyPoolFun(bodyTypeID, PlayerBodyModelDic[bodyTypeID]);
                PlayerBodyModelDic.Remove(bodyTypeID);
            }
        }
        else
        {
            if (UIBodyModelDic.ContainsKey(bodyTypeID))
            {
                if (clothingRootNode == null)
                {
                    clothingRootNode = new GameObject("_ClothingRootNode");
                    DontDestroyOnLoad(clothingRootNode.transform);
                }
                UIBodyModelDic[bodyTypeID].transform.parent = clothingRootNode.transform;
                UIBodyModelDic[bodyTypeID].SetActive(false);
                RecycleBodyPoolFun(bodyTypeID, UIBodyModelDic[bodyTypeID]);
                UIBodyModelDic.Remove(bodyTypeID);
            }
        }


    }
    /// <summary>
    /// 添加衣服模型
    /// </summary>
    private void AddAvatarModelFun(avatar avatarItem, GameObject playerGame, bool isRealControPlayer)
    {

        if (isRealControPlayer)
        {
            if (PlayerAvatarModelDic.ContainsKey(avatarItem.avatar_type2))
            {
                return;
            }
        }
        else
        {
            if (UIAvatarModelDic.ContainsKey(avatarItem.avatar_type2))
            {
                return;
            }
        }


        GameObject cloth = TakeAvatarPoolFun(avatarItem);
        //把模型添加到人物身上
        SetModelLayerFun(cloth);
        cloth.SetActive(true);
        cloth.transform.parent = playerGame.transform;//playerGamePlayer.transform;
        cloth.transform.localPosition = Vector3.zero;
        cloth.transform.localScale = Vector3.one;
        PlayerAvatarData playerAvatarData = new PlayerAvatarData();
        playerAvatarData.item_id = avatarItem.avatar_id;
        playerAvatarData.item_obj = cloth;

        if (isRealControPlayer)
        {
            PlayerAvatarModelDic.Add(avatarItem.avatar_type2, playerAvatarData);
        }
        else
        {
            UIAvatarModelDic.Add(avatarItem.avatar_type2, playerAvatarData);
        }

        RefreshSingleModelColorFun(isRealControPlayer, cloth);
        RefreshSkeletionFun(cloth, playerGame);
        AddAvatarTempDataFun(playerAvatarData.item_id);

    }
    /// <summary>
    /// 移除衣服模型
    /// </summary>
    private void RemoveAvatarModelFun(int avatar_type2, bool isRealControPlayer = false)
    {
        if (isRealControPlayer)
        {
            //判断身上是否有此类着装
            if (PlayerAvatarModelDic.ContainsKey(avatar_type2))
            {
                if (clothingRootNode == null)
                {
                    clothingRootNode = new GameObject("_ClothingRootNode");
                    DontDestroyOnLoad(clothingRootNode.transform);
                }
                PlayerAvatarModelDic[avatar_type2].item_obj.SetActive(false);
                PlayerAvatarModelDic[avatar_type2].item_obj.transform.parent = clothingRootNode.transform;
                //回收到对象池
                RecycleAvatarPoolFun(PlayerAvatarModelDic[avatar_type2], avatar_type2);
                RemoveAvatarTempDataFun(PlayerAvatarModelDic[avatar_type2].item_id);

                //清除数据
                PlayerAvatarModelDic.Remove(avatar_type2);

            }
        }
        else
        {
            Debug.Log("输出一下这里清除了所有衣服了： " + avatar_type2);
            //判断身上是否有此类着装
            if (UIAvatarModelDic.ContainsKey(avatar_type2))
            {
                if (clothingRootNode == null)
                {
                    clothingRootNode = new GameObject("_ClothingRootNode");
                    DontDestroyOnLoad(clothingRootNode.transform);
                }
                UIAvatarModelDic[avatar_type2].item_obj.SetActive(false);
                UIAvatarModelDic[avatar_type2].item_obj.transform.parent = clothingRootNode.transform;
                //回收到对象池
                RecycleAvatarPoolFun(UIAvatarModelDic[avatar_type2], avatar_type2);
                RemoveAvatarTempDataFun(UIAvatarModelDic[avatar_type2].item_id);

                //清除数据
                UIAvatarModelDic.Remove(avatar_type2);

            }
        }
    }

    /// <summary>
    /// 移除其他人物衣服模型
    /// </summary>
    public void RemoveOtherAvatarModelFun(int avatar_type2, ulong userInfoID)
    {
        if (AllOtherPlayerAvatarData[userInfoID].avatarObjs.ContainsKey(avatar_type2))
        {
            Destroy(AllOtherPlayerAvatarData[userInfoID].avatarObjs[avatar_type2].item_obj);
            AllOtherPlayerAvatarData[userInfoID].avatarObjs.Remove(avatar_type2);
        }
    }


    /// <summary>
    /// 添加其他人物衣服模型
    /// </summary>
    private void AddOtherAvatarModelFun(avatar avatarItem, ulong userInfoID)
    {
        //Debug.Log("添加新模型的内容" + avatarItem.avatar_id);
        if (!AllOtherPlayerAvatarData[userInfoID].avatarObjs.ContainsKey(avatarItem.avatar_type2))
        {
            GameObject ColthPrefab = InstantiateAvatarFun(avatarItem);
            //把模型添加到人物身上
            SetModelLayerFun(ColthPrefab);
            ColthPrefab.SetActive(true);
            ColthPrefab.transform.parent = AllOtherPlayerAvatarData[userInfoID].playerModel.transform;
            ColthPrefab.transform.localPosition = Vector3.zero;
            ColthPrefab.transform.localScale = Vector3.one;
            // var clothSkeletonNode = AllOtherPlayerAvatarData[userInfoID].playerModel.transform.Find("Dummy001").gameObject;

            Debug.Log("输出一下其他人物刷新骨骼逻辑");

            RefreshSkeletionFun(ColthPrefab, AllOtherPlayerAvatarData[userInfoID].playerModel);
            PlayerAvatarData playerAvatarData = new PlayerAvatarData();
            playerAvatarData.item_id = avatarItem.avatar_id;
            playerAvatarData.item_obj = ColthPrefab;
            AllOtherPlayerAvatarData[userInfoID].avatarObjs.Add(avatarItem.avatar_type2, playerAvatarData);
            CharacterManager.Instance().PlayOtherPlayerSpecialEffect(ColthPrefab);
        }
    }
    /// <summary>
    /// 刷新单个服装颜色
    /// </summary>
    /// <param name="isRealControPlayer"></param>
    /// <param name="model"></param>
    private void RefreshSingleModelColorFun(bool isRealControPlayer, GameObject model)
    {
        MyOutFitSaveReqData outFitData = myOutFitAvatarIdData_tem;
        if (isRealControPlayer)
        {
            outFitData = myOutFitAvatarIdData;
        }
        foreach (var item in outFitData.data)
        {
            skinColor skinColor = ManageMentClass.DataManagerClass.GetSkinColorTableFun(item.avatar_id);
            if (skinColor != null)
            {
                if (skinColor.avatar_type1 == (int)Avatar_Type1.skinColor)
                {
                    SetSkinColorFun(model, skinColor);
                }
            }
        }
    }


    /// <summary>
    /// 回收到衣服对象池
    /// </summary>
    /// <param name="playerAvatarData"></param>
    private void RecycleAvatarPoolFun(PlayerAvatarData playerAvatarData, int avatarType)
    {

        Destroy(playerAvatarData.item_obj);
        /*if (AllAvatarModelDic.ContainsKey(playerAvatarData.item_id))
        {
            playerAvatarData.item_obj.SetActive(false);
            AllAvatarModelDic[playerAvatarData.item_id].Add(playerAvatarData.item_obj);
        }
        else
        {
            var modelList = new List<GameObject>();
            playerAvatarData.item_obj.SetActive(false);
            modelList.Add(playerAvatarData.item_obj);
            AllAvatarModelDic.Add(playerAvatarData.item_id, modelList);
        }*/

    }
    /// <summary>
    /// 从衣服对象池中获取
    /// </summary>
    private GameObject TakeAvatarPoolFun(avatar avatarItem)
    {
        /* if (AllAvatarModelDic.ContainsKey(avatarItem.avatar_id))
         {
             for (int i = 0; i < AllAvatarModelDic[avatarItem.avatar_id].Count; i++)
             {
                 if (!AllAvatarModelDic[avatarItem.avatar_id][i].activeSelf)
                 {
                     var gameModel = AllAvatarModelDic[avatarItem.avatar_id][i];
                     AllAvatarModelDic[avatarItem.avatar_id].RemoveAt(i);
                     return gameModel;
                 }
             }
         }*/
        GameObject ColthPrefab = InstantiateAvatarFun(avatarItem);
        CharacterManager.Instance().PlayOtherPlayerSpecialEffect(ColthPrefab);
        return ColthPrefab;
    }
    /// <summary>
    /// 回收到裸模对象池
    /// </summary>
    /// <param name="bodyTypeId"></param>
    /// <param name="bodyObj"></param>
    private void RecycleBodyPoolFun(int bodyTypeId, GameObject bodyObj)
    {
        /*if (AllBodyModelDic.ContainsKey(bodyTypeId))
        {
            bodyObj.SetActive(false);
            AllBodyModelDic[bodyTypeId].Add(bodyObj);
        }
        else
        {
            var bodyList = new List<GameObject>();
            bodyList.Add(bodyObj);
            AllBodyModelDic.Add(bodyTypeId, bodyList);
        }*/
        Destroy(bodyObj);
    }
    /// <summary>
    /// 从裸模对象池中获取
    /// </summary>
    /// <param name="bodyTypeId"></param>
    /// <returns></returns>
    private GameObject TakeBodyPoolFun(int bodyTypeId)
    {
        /* if (AllBodyModelDic.ContainsKey(bodyTypeId))
         {
             for (int i = 0; i < AllBodyModelDic[bodyTypeId].Count; i++)
             {
                 if (!AllBodyModelDic[bodyTypeId][i].activeSelf)
                 {
                     var gameModel = AllBodyModelDic[bodyTypeId][i];
                     AllBodyModelDic[bodyTypeId].RemoveAt(i);
                     return gameModel;
                 }
             }
         }*/
        GameObject bodyPrefab = InstantiateBodyFun(bodyTypeId);
        CharacterManager.Instance().PlayOtherPlayerSpecialEffect(bodyPrefab);
        return bodyPrefab;
    }
    /// <summary>
    /// 实例化衣服
    /// </summary>
    private GameObject InstantiateAvatarFun(avatar avatarItem)
    {
        GameObject itemPrefab = Instantiate(ManageMentClass.ResourceControllerClass.ResLoadAvatarByTypeFun(avatarItem.avatar_type2, avatarItem.avatar_model));
        CharacterManager.Instance().PlayOtherPlayerSpecialEffect(itemPrefab);
        return itemPrefab;
    }
    /// <summary>
    /// 实例化身体裸模
    /// </summary>
    private GameObject InstantiateBodyFun(int bodyId)
    {
        GameObject itemPrefab = Instantiate(ManageMentClass.ResourceControllerClass.ResLoadBodyByTypeFun(bodyId));
        CharacterManager.Instance().PlayOtherPlayerSpecialEffect(itemPrefab);
        return itemPrefab;
    }
    /// <summary>
    /// 刷新骨骼信息
    /// </summary>
    private void RefreshSkeletionFun(GameObject go, GameObject playerObj)
    {
        Debug.Log("输出一下Go的名字： " + go.name + " PlayerObj 的名称： " + playerObj.name);
        if (playerObj.transform.Find("Dummy001") == null)
        {
            return;
        }
        GameObject skeletonNode = playerObj.transform.Find("Dummy001").gameObject;
        SkinnedMeshRenderer render = go.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform[] newBones = new Transform[render.bones.Length];
        for (int i = 0; i < render.bones.GetLength(0); ++i)
        {
            GameObject bone = render.bones[i].gameObject;
            var aaa = FindChildRecursion(skeletonNode.transform, bone.name);
            newBones[i] = aaa;
        }
        render.bones = newBones;
        GameObject goSkeletonNode = go.transform.Find("Dummy001").gameObject;
        if (goSkeletonNode != null)
        {
            Destroy(goSkeletonNode);
        }
    }
    private Transform FindChildRecursion(Transform t, string name)
    {
        foreach (Transform child in t)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                Transform ret = FindChildRecursion(child, name);
                if (ret != null)
                {
                    return ret;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// mesh合批处理
    /// </summary>
    private void MeshBatchingFun()
    {

    }
    /// <summary>
    /// 刷新最新的人物
    /// </summary>
    private void RefreshPlayerGameObject()
    {
        playerGamePlayer = CharacterManager.Instance().GetPlayerObj();
        if (playerGamePlayer.CompareTag("AvatarPlayer") && skeletonNode == null)
        {
            skeletonNode = playerGamePlayer.transform.Find("Dummy001").gameObject;
        }
    }
    /// <summary>
    /// 是否是可换装的人物模型
    /// </summary>
    /// <returns></returns>
    private bool IsAvatarPlayer()
    {
        if (playerGamePlayer.CompareTag("AvatarPlayer"))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 删除指定服装id
    /// 临时数据（用于预览）
    /// </summary>
    /// <param name="id"></param>
    private void RemoveAvatarTempDataFun(int id)
    {
        for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
        {
            if (myOutFitAvatarIdData_tem.data[i].avatar_id == id)
            {
                myOutFitAvatarIdData_tem.data.RemoveAt(i);
                return;
            }
        }

    }
    /// <summary>
    /// 删除所有服装id
    /// 临时数据（用于预览）
    /// </summary>
    private void RemoveAllAatarTempDataFun()
    {
        myOutFitAvatarIdData_tem.data.Clear();
        myHeentAndColorData_tem.Clear();
    }
    /// <summary>
    /// 添加指定服装Id
    /// 临时数据（用于预览）
    /// </summary>
    private void AddAvatarTempDataFun(int id)
    {
        for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
        {
            if (myOutFitAvatarIdData_tem.data[i].avatar_id == id)
            {
                return;
            }
        }
        MyOutFitRecData data = new MyOutFitRecData();
        data.avatar_id = id;
        myOutFitAvatarIdData_tem.data.Add(data);
    }
    /// <summary>
    /// 替换指定（五官、肤色）数据
    /// 临时数据
    /// </summary>
    /// <param name="id"></param>
    private void ReplaceAvatarTempDataFun(int typeID, int newID)
    {
        if (myHeentAndColorData_tem.ContainsKey(typeID))
        {
            for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
            {
                if (myOutFitAvatarIdData_tem.data[i].avatar_id == myHeentAndColorData_tem[typeID])
                {
                    myOutFitAvatarIdData_tem.data[i].avatar_id = newID;
                    myHeentAndColorData_tem[typeID] = newID;
                    return;
                }
            }
        }
        else
        {
            myHeentAndColorData_tem.Add(typeID, newID);
        }
        //处理所有默认的脸型和角色
        InitAllHeentAndColorTempFun();
    }
    /// <summary>
    /// 处理所有默认脸型和颜色
    /// </summary>
    private void InitAllHeentAndColorTempFun()
    {
        var keys = myHeentAndColorData_tem.Keys;
        bool isHave = false;
        if (keys.Count > 0)
        {
            foreach (var key in keys)
            {
                isHave = false;
                for (int i = 0; i < myOutFitAvatarIdData_tem.data.Count; i++)
                {
                    if (myOutFitAvatarIdData_tem.data[i].avatar_id == myHeentAndColorData_tem[key])
                    {
                        isHave = true;
                    }
                }
                if (!isHave)
                {
                    MyOutFitRecData data = new MyOutFitRecData();
                    data.avatar_id = myHeentAndColorData_tem[key];
                    myOutFitAvatarIdData_tem.data.Add(data);
                }
            }
        }
    }


    /// <summary>
    /// 删除指定服装id
    /// 正式数据
    /// </summary>
    /// <param name="id"></param>
    private void RemoveAvatarOfficialDataFun(int id)
    {
        for (int i = 0; i < myOutFitAvatarIdData.data.Count; i++)
        {
            if (myOutFitAvatarIdData.data[i].avatar_id == id)
            {
                myOutFitAvatarIdData.data.RemoveAt(i);
                return;
            }
        }
    }
    /// <summary>
    /// 删除所有服装id
    /// 正式数据
    /// </summary>
    private void RemoveAllAatarOfficialDataFun()
    {
        myOutFitAvatarIdData.data.Clear();
    }
    /// <summary>
    /// 添加指定服装Id
    /// 正式数据
    /// </summary>
    private void AddAvatarOfficialDataFun(int id)
    {
        for (int i = 0; i < myOutFitAvatarIdData.data.Count; i++)
        {
            if (myOutFitAvatarIdData.data[i].avatar_id == id)
            {
                return;
            }
        }
        MyOutFitRecData data = new MyOutFitRecData();
        data.avatar_id = id;
        myOutFitAvatarIdData.data.Add(data);
    }
    /// <summary>
    /// 设置layer
    /// </summary>
    /// <param name="obj"></param>
    private void SetModelLayerFun(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.layer = 8;
        }
    }


    //设置肤色
    private void SetSkinColorFun(GameObject headObj, skinColor skinColor)
    {
        Debug.Log("输出一下headobj的名字：  " + headObj.name);
        listSkinnedMeshRenderer.Clear();
        Color baseColor = new Color();
        Color addColor = new Color();

        for (int i = 0; i < headObj.transform.childCount; i++)
        {
            if (headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    listSkinnedMeshRenderer.Add(skinnedMeshRenderer);
                }
                break;
            }
        }
        if (listSkinnedMeshRenderer.Count > 0)
        {
            for (int i = 0; i < listSkinnedMeshRenderer.Count; i++)
            {
                for (int j = 0; j < listSkinnedMeshRenderer[i].materials.Length; j++)
                {
                    if (listSkinnedMeshRenderer[i].materials[j].name == "DefaultSkin (Instance)")
                    {
                        if (skinColor.avatar_data_skinColor != null && skinColor.avatar_data_skinColor != "null")
                        {
                            JObject jo = JObject.Parse(skinColor.avatar_data_skinColor);
                            Debug.Log("这里的肤色值： " + jo.ToString());
                            if (jo["_BaseColor"] != null)
                            {
                                if (ColorUtility.TryParseHtmlString(jo["_BaseColor"].ToString(), out baseColor))
                                {
                                    listSkinnedMeshRenderer[i].materials[j].SetColor("_BaseColor", baseColor);
                                }

                            }
                            if (jo["_AddColor"] != null)
                            {
                                if (ColorUtility.TryParseHtmlString(jo["_AddColor"].ToString(), out addColor))
                                {
                                    listSkinnedMeshRenderer[i].materials[j].SetColor("_AddColor", addColor);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 设置眼睛颜色
    /// </summary>
    /// <param name="headObj"></param>
    /// <param name="skinColor"></param>
    private void SetEyesColorFun(GameObject headObj, skinColor skinColor)
    {
        listSkinnedMeshRenderer.Clear();
        Color baseColor = new Color();
        for (int i = 0; i < headObj.transform.childCount; i++)
        {
            if (headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    listSkinnedMeshRenderer.Add(skinnedMeshRenderer);
                }
                break;
            }
        }
        if (listSkinnedMeshRenderer.Count > 0)
        {
            for (int i = 0; i < listSkinnedMeshRenderer.Count; i++)
            {
                for (int j = 0; j < listSkinnedMeshRenderer[i].materials.Length; j++)
                {
                    if (listSkinnedMeshRenderer[i].materials[j].name == "DefaultEye (Instance)")
                    {
                        if (skinColor.avatar_data_skinColor != null && skinColor.avatar_data_skinColor != "null")
                        {
                            JObject jo = JObject.Parse(skinColor.avatar_data_skinColor);
                            if (jo["_BaseColor"] != null)
                            {
                                if (ColorUtility.TryParseHtmlString(jo["_BaseColor"].ToString(), out baseColor))
                                {
                                    listSkinnedMeshRenderer[i].materials[j].SetColor("_BaseColor", baseColor);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //设置五官
    private void SetHeentFun(GameObject headObj, skinColor skinColor)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = null;

        for (int i = 0; i < headObj.transform.childCount; i++)
        {
            if (headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
            {
                skinnedMeshRenderer = headObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
        if (skinnedMeshRenderer != null && skinColor.avatar_data_faceShapes != null && skinColor.avatar_data_faceShapes != "null")
        {
            JObject jo = JObject.Parse(skinColor.avatar_data_faceShapes);

            for (int i = 0; i < BlendShapesName.Length; i++)
            {
                if (jo[BlendShapesName[i]] != null)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(i, (float)jo[BlendShapesName[i]]);
                }
            }
        }
    }



    #endregion
}
