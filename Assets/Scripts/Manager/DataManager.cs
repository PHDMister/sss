using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DataManager
{
    //其他空间的数据
    public List<OtherSpaceData> OtherSpaceDataList = new List<OtherSpaceData>();

    public int[] CamerIdaData;

    // 链接是否为用户真实Token
    public bool isLinkEdition = false;

    //版本开关（ false 测试版） （ true 正式版）
    public bool isOfficialEdition = false;

    //测试版 版本号
    public string TestVersionNumber = "4.1.8";
    //正式版 版本号
    public string OfficialVersionNumber = "4.1.8";

    //版本号
    public string VersionsNumber;

    public int SceneID;

    //运行平台  1 = PC端，2 = 移动端
    public int PlatformType;

    //是否从网页端进入
    public bool WebInto = false;

    //app token
    public string tokenValue_App;
    // game token
    public string tokenValue_Game;
    // 土地ID
    public string landId;
    //userId
    public ulong userId;
    //roomId
    public uint roomId;

    // --------个人信息------

    //空间名称
    public string spce_Name;
    //建筑名称
    public string build_Name;
    //头像图
    public Sprite Head_Texture;
    //土地坐标
    public string build_XYZ;
    //用户姓名
    public string login_Name;
    // gas余额
    public int gas_Amount;
    //寻宝入场券
    public int ticket;
    // 当前人物ID
    public int hotman_ID;
    // 是否为房间主人 
    public bool is_Owner;
    //空间个数
    public int SpaceNum;
    //---------------
    //页数
    public int pageValue = 1;
    //每页个数
    public int pageSize = 30;
    //玩家是否有藏品
    public bool playerIsHavePicture = true;

    // 物品Icon字典
    public Dictionary<string, Sprite> AllIconTextureData = new Dictionary<string, Sprite>();


    // 整体的控制类
    public bool IsControllerPlayer = true;

    public bool CameraController = true;

    public bool CameraControllerPlayerRotation = true;
    initialContainer placeContainer;

    //宠物列表数据
    public Dictionary<int, PetListRecData> petListDataDic = new Dictionary<int, PetListRecData>();//宠物盒数据
    public Dictionary<int, PetModelRecData> petModelRecData = new Dictionary<int, PetModelRecData>();//宠物盒领养后宠物数据

    //宠物商店数据
    public Dictionary<int, PetShopItemData> DicPetShopItemData = new Dictionary<int, PetShopItemData>();//宠物商店数据
    //宠物商店物品ID集合
    public List<int> ListPetShopItemId = new List<int>();


    public int petEnableAdoptStatus = 0;
    public string petAdoptCostItem = "";
    public int petAdoptCostItemNum = 0;
    // 爱心币
    public int loveCoin = 0;


    //是否是其他空间
    public bool isOtherSpace = false;
    public bool bClickOtherSpace = false;

    public Dictionary<int, int> dicFurnAndActSortType = new Dictionary<int, int>();//存储家具动作当前排序类型
    public Dictionary<int, int> dicClothingSortType = new Dictionary<int, int>();//存储服装当前排序类型

    //挖宝
    public bool bInviteFromApp = false;
    public ulong InviteFromUserId = 0;//好友邀请者Id(app跳转会下发)
    public bool bUpdate = true;//版本是否更新
    public bool bTreasureOpen = false;
    public bool bTreasureEnd = false;

    public string YXAppKey = "41cda41818977a43c58dbb504241dfc9";
    public string YXToken = "";//云信SDK token
    public string YXAccid = "";// 云信SDK accid
    public ulong ChatRoomId;
    public string[] ChatRoomAddr;


    //个人资料
    public PersonUserData personUserData = new PersonUserData();

    //个人信息
    public PersonUserData selfPersonData = new PersonUserData();

    public DataManager()
    {
        BinaryDataMgr.Instance.LoadTable<itemContainer, item>();
        BinaryDataMgr.Instance.LoadTable<hotmanContainer, hotman>();
        BinaryDataMgr.Instance.LoadTable<animationContainer, animation>();
        BinaryDataMgr.Instance.LoadTable<furnitureContainer, furniture>();
        BinaryDataMgr.Instance.LoadTable<mallContainer, mall>();
        BinaryDataMgr.Instance.LoadTable<initialContainer, initial>();
        BinaryDataMgr.Instance.LoadTable<cameralistContainer, cameralist>();
        BinaryDataMgr.Instance.LoadTable<sceneContainer, scene>();
        BinaryDataMgr.Instance.LoadTable<petContainer, pet>();
        BinaryDataMgr.Instance.LoadTable<pet_keepingContainer, pet_keeping>();
        BinaryDataMgr.Instance.LoadTable<pet_adoptionContainer, pet_adoption>();
        BinaryDataMgr.Instance.LoadTable<petconditionContainer, petcondition>();
        BinaryDataMgr.Instance.LoadTable<pet_consumeContainer, pet_consume>();
        BinaryDataMgr.Instance.LoadTable<helpContainer, help>();
        BinaryDataMgr.Instance.LoadTable<trainContainer, train>();
        BinaryDataMgr.Instance.LoadTable<avatarContainer, avatar>();
        BinaryDataMgr.Instance.LoadTable<skinColorContainer, skinColor>();
        BinaryDataMgr.Instance.LoadTable<petHouseContainer, petHouse>();
        BinaryDataMgr.Instance.LoadTable<treasure_rewardContainer, treasure_reward>();
        BinaryDataMgr.Instance.LoadTable<treasure_ticketContainer, treasure_ticket>();
        BinaryDataMgr.Instance.LoadTable<emojiContainer, emoji>();
        Debug.Log("表的初始化");
    }

    public void CleanSpaceData()
    {
        //空间名称
        spce_Name = "";
        //建筑名称
        build_Name = "";
        //土地坐标
        build_XYZ = "";
        login_Name = "";
        // gas余额
        gas_Amount = 0;
        hotman_ID = 0;

        // 是否为房间主人 
        is_Owner = false;
    }
    
    /// <summary>
    /// 获取Item表中的数据
    /// </summary>
    public item GetItemTableFun(int key)
    {
        itemContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<itemContainer>("itemContainer");
        if (placeContainer != null)
        {
            item itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取hotman表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public hotman GetHotmanTableFun(int key)
    {
        hotmanContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<hotmanContainer>("hotmanContainer");
        if (placeContainer != null)
        {
            hotman itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取animation表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public animation GetAnimationTableFun(int key)
    {
        animationContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<animationContainer>("animationContainer");
        if (placeContainer != null)
        {
            animation itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取furniture表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public furniture GetFurnitureTableFun(int key)
    {

        furnitureContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<furnitureContainer>("furnitureContainer");
        if (placeContainer != null)
        {
            furniture itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        else
        {
            Debug.Log("这里报错了");
        }
        return null;
    }
    /// <summary>
    /// 获取mall表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public mall GetMallTableFun(int key)
    {

        mallContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<mallContainer>("mallContainer");
        if (placeContainer != null)
        {
            mall itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取Initial表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public initial GetInitialTableFun(int key)
    {
        if (placeContainer == null)
        {
            placeContainer = BinaryDataMgr.Instance.LoadTableById<initialContainer>("initialContainer");
        }
        if (placeContainer != null)
        {
            initial itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    public int GetInitialCountFun()
    {
        if (placeContainer == null)
        {
            placeContainer = BinaryDataMgr.Instance.LoadTableById<initialContainer>("initialContainer");
        }
        return placeContainer.dataDic.Keys.Count;
    }
    /// <summary>
    /// 获取镜头表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public cameralist GetCameraListTableFun(string key)
    {
        cameralistContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<cameralistContainer>("cameralistContainer");
        if (placeContainer != null)
        {
            cameralist itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取场景镜头表中的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public scene GetSceneListTableFun(int key)
    {
        sceneContainer placeContainer = BinaryDataMgr.Instance.LoadTableById<sceneContainer>("sceneContainer");
        if (placeContainer != null)
        {
            scene itemConfig;
            placeContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取宠物Pet表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public pet GetPetTableFun(int key)
    {
        petContainer m_petContainer = BinaryDataMgr.Instance.LoadTableById<petContainer>("petContainer");
        if (m_petContainer != null)
        {
            pet itemConfig;
            m_petContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取宠物Keeping表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public pet_keeping GetPetKeepingTableFun(int key)
    {
        pet_keepingContainer keeepingContainer = BinaryDataMgr.Instance.LoadTableById<pet_keepingContainer>("pet_keepingContainer");
        if (keeepingContainer != null)
        {
            pet_keeping itemConfig;
            keeepingContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取宠物Keeping表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public pet_keeping GetPetKeepingTableFun(int pet_type, int consume_type)
    {
        pet_keepingContainer keeepingContainer = BinaryDataMgr.Instance.LoadTableById<pet_keepingContainer>("pet_keepingContainer");
        if (keeepingContainer != null)
        {
            var result = keeepingContainer.dataDic.Where(item => item.Value.pet_type == pet_type && item.Value.consume_type == consume_type).Select(p => p.Value).FirstOrDefault();
            return result;
        }
        return null;
    }

    /// <summary>
    /// 获取宠物pet_adoption表
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public pet_adoption GetPetAdoptionTableFun(int key)
    {
        pet_adoptionContainer adoptionContainer = BinaryDataMgr.Instance.LoadTableById<pet_adoptionContainer>("pet_adoptionContainer");
        if (adoptionContainer != null)
        {
            pet_adoption itemConfig;
            adoptionContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    public petcondition GetPetConditionTable(int pet_type, int condition_type)
    {
        petconditionContainer petconditionContainer = BinaryDataMgr.Instance.LoadTableById<petconditionContainer>("petconditionContainer");
        if (petconditionContainer != null)
        {
            var result = petconditionContainer.dataDic.Where(item => item.Value.pet_type == pet_type && item.Value.condition_type == condition_type).Select(p => p.Value).FirstOrDefault();
            return result;
        }
        return null;
    }

    public pet_consume GetPetConsumeTable(int pet_type, int condition_type)
    {
        pet_consumeContainer petconsumeContainer = BinaryDataMgr.Instance.LoadTableById<pet_consumeContainer>("pet_consumeContainer");
        if (petconsumeContainer != null)
        {
            var result = petconsumeContainer.dataDic.Where(item => item.Value.pet_type == pet_type && item.Value.consume_type == condition_type).Select(p => p.Value).FirstOrDefault();
            return result;
        }
        return null;
    }

    public help GetHelpTable(int function_id)
    {
        helpContainer _helpContainer = BinaryDataMgr.Instance.LoadTableById<helpContainer>("helpContainer");
        if (_helpContainer != null)
        {
            help itemConfig;
            _helpContainer.dataDic.TryGetValue(function_id, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    public train GetTrainTableFun(int key)
    {
        trainContainer trainContainer = BinaryDataMgr.Instance.LoadTableById<trainContainer>("trainContainer");
        if (trainContainer != null)
        {
            train itemConfig;
            trainContainer.dataDic.TryGetValue(key, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 换装模型表
    /// </summary>
    /// <param name="item_Id"></param>
    /// <returns></returns>
    public avatar GetAvatarTableFun(long item_Id)
    {
        avatarContainer avatarContainer = BinaryDataMgr.Instance.LoadTableById<avatarContainer>("avatarContainer");
        if (avatarContainer != null)
        {

            avatar itemConfig;
            avatarContainer.dataDic.TryGetValue((int)item_Id, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }


    /// <summary>
    /// 获取换装默认模型数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public avatar GetAvatarDefaultDataFun(int avatar_type2, int avatar_default)
    {
        avatarContainer avatarContainer = BinaryDataMgr.Instance.LoadTableById<avatarContainer>("avatarContainer");
        if (avatarContainer != null)
        {
            var result = avatarContainer.dataDic.Where(item => item.Value.avatar_type2 == avatar_type2 && item.Value.avatar_default == avatar_default).Select(p => p.Value).FirstOrDefault();
            return result;
        }
        return null;
    }


    /// <summary>
    /// 换装数据表
    /// </summary>
    /// <param name="item_Id"></param>
    /// <returns></returns>
    public skinColor GetSkinColorTableFun(long item_Id)
    {
        skinColorContainer skinColorContainer = BinaryDataMgr.Instance.LoadTableById<skinColorContainer>("skinColorContainer");
        if (skinColorContainer != null)
        {
            skinColor itemConfig;
            skinColorContainer.dataDic.TryGetValue((int)item_Id, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }
    /// <summary>
    /// 宠物房间表
    /// </summary>
    /// <param name="item_Id"></param>
    /// <returns></returns>
    public petHouse GetPetHouseTableFun(int item_Id)
    {
        petHouseContainer petHouseContainer = BinaryDataMgr.Instance.LoadTableById<petHouseContainer>("petHouseContainer");
        if (petHouseContainer != null)
        {
            petHouse itemConfig;
            petHouseContainer.dataDic.TryGetValue(item_Id, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    public treasure_reward GetTreasureRewardTable(int item_Id)
    {
        treasure_rewardContainer treasureRewardContainer = BinaryDataMgr.Instance.LoadTableById<treasure_rewardContainer>("treasure_rewardContainer");
        if (treasureRewardContainer != null)
        {
            treasure_reward itemConfig;
            treasureRewardContainer.dataDic.TryGetValue(item_Id, out itemConfig);
            if (itemConfig != null)
            {
                return itemConfig;
            }
        }
        return null;
    }

    public treasure_ticket GetTreasureTicketTable(int ticketId)
    {
        treasure_ticketContainer treasureTicketContainer = BinaryDataMgr.Instance.LoadTableById<treasure_ticketContainer>("treasure_ticketContainer");
        if (treasureTicketContainer != null)
        {
            treasure_ticket itemConfig;
            treasureTicketContainer.dataDic.TryGetValue(ticketId, out itemConfig);
            return itemConfig;
        }
        return null;
    }

    public Dictionary<int, emoji> GetEmojiData()
    {
        emojiContainer emojiContainer = BinaryDataMgr.Instance.LoadTableById<emojiContainer>("emojiContainer");
        if (emojiContainer != null)
        {
            return emojiContainer.dataDic;
        }
        return null;
    }

    public emoji GetEmojiTable(int id)
    {
        emojiContainer emojiContainer = BinaryDataMgr.Instance.LoadTableById<emojiContainer>("emojiContainer");
        if (emojiContainer != null)
        {
            emoji itemConfig;
            emojiContainer.dataDic.TryGetValue(id, out itemConfig);
            return itemConfig;
        }
        return null;
    }
}
public class GetTokenData
{
    public GetTokenData(string name, string vcode, string type, string deviceId, string strCode, string strRemark)
    {
        username = name;
        v_code = vcode;
        app_type = type;
        device_id = deviceId;
        code = strCode;
        remark = strRemark;
    }
    public string username;
    public string v_code;
    public string app_type;
    public string device_id;
    public string code;
    public string remark;
}

/// <summary>
/// 土地区域ID数据
/// </summary>
public class AreaLanIdData
{
    public AreaLanIdData(string id)
    {
        area_id = id;
    }
    public string area_id;
}

//家具根节点的数据
public class FurnitureRootData
{
    public GameObject furnitureRoot;
    public FurnitureType furnitureType;
    public Vector3 furnitureRootPos;
    public int PlaceID;
}
//家具UI数据
public class FurnitureUIItemData
{
    public int furnitureId;
}
//家具放置点对应的家具
public class FurniturePlaceData
{
    public GameObject furnitureObj;
    public int furnitureId;
}
//家具池子的数据
public class FurnitureData
{
    public FurnitureData()
    {
        furnitureList = new List<FurnitureItemData>();
    }
    public List<FurnitureItemData> furnitureList;
    //剩余个数
    public int count;
}
//家具自身的数据
public class FurnitureItemData
{
    public GameObject furnitureObj;
    //  public bool Used;
    public int furnitureId;
}
//画框
public class FurniturePictureItemData
{
    public GameObject furnitureObj;
    public string id;
    public Material material;
    public string product_id;
    public int serial_no;
    public string OnlyID;
}
//自身landID 数据
public class PersonData
{
    public PersonData(string landID)
    {
        land_id = landID;
    }
    public string land_id;
}
//登录需要的数据
public class LoginData
{
    public LoginData(string gameId, string codeID)
    {
        game_id = gameId;
        code = codeID;
    }
    public string game_id;
    public string code;
}
//藏品数据
public class FrameData
{
    //装饰画id
    public string id;
    //序号
    public int serial_no;
    //图片URL
    public string picture;
    //藏品id
    public string product_id;
    //源文件id
    public string nft_product_size_id;
    //图片本身
    public Sprite sprite;
    //唯一ID
    public string OnlyID;
}
//藏品图片的数据
public class FramePictureData
{
    public string product_id;
    public string nft_product_size_id;
    public string picture;
    public string OnlyID;
    public Sprite frameSprite;
}

//藏品数据
public class ServerFrameData
{
    //装饰画id
    public string id;
    // 土地ID
    public string land_id;
    //序号
    public int serial_no;
    //藏品id
    public string product_id;
    //源文件id
    public string nft_product_size_id;
    //图片URL
    public string picture;
}

// 获取藏品（分页数据）
public class PageClass
{
    //页数
    public int page;
    //每页的个数
    public int page_size;
    public PageClass(int pageValue, int size)
    {
        page = pageValue;
        page_size = size;
    }
}
/// <summary>
/// 获取家具类型列表得数据
/// </summary>
public class FurntureTypeClass
{
    public FurntureTypeClass(string type)
    {
        furniture_type = type;
    }
    public string furniture_type;
}
/// <summary>
/// 家具存在个数
/// </summary>
public class FurntureHasDataClass
{
    //家具ID
    public int furnitureId;
    //拥有个数
    public int hasNum;
    //是否为初始的家具
    public bool isInitial;
}
public class ShopData
{
    public int item_type1;
    public ShopData(int _itemType)
    {
        item_type1 = _itemType;
    }
}

public class BuyData
{
    public int item_id;
    public int number;

    public BuyData(int id, int num)
    {
        item_id = id;
        number = num;
    }
}

public class UseData
{
    public int item_id;

    public UseData(int id)
    {
        item_id = id;
    }
}
public class UseListData
{

}
public class SetFurnitureData
{
    public int place_num;
    public int item_id;
    public SetFurnitureData(int placeID, int id)
    {
        place_num = placeID;
        item_id = id;
    }
}

public class SetFrurnitureToServerData
{
    public List<SetFurnitureData> list;
    public string land_id;
    public SetFrurnitureToServerData(List<SetFurnitureData> listData, string id)
    {
        list = listData;
        land_id = id;
    }
}
public class SetPictureToServerData
{
    public List<ServerFrameData> list;
    public SetPictureToServerData(List<ServerFrameData> listData)
    {
        list = listData;
    }
}


public class CharacterListData
{
    public CharacterListData()
    {

    }
}

public class CharacterReplaceData
{
    public int item_id;
    public CharacterReplaceData(int id)
    {
        item_id = id;
    }
}

public class CharacterExchangeData
{
    public int item_id;
    public CharacterExchangeData(int id)
    {
        item_id = id;
    }
}



public class CharacterData
{
    public int item_id;
    public int has_num;
    public int is_selected;
}


public class PlaceGameObj
{
    public int placeID;
    public GameObject rootObj;
}
/// <summary>
/// 镜头数据
/// </summary>
public class CameraValueData
{
    public Vector3 CameraPos;
    public Vector3 CameraRoation;
    public float CameraFovAxis;
    public string CameraName;
    public string IconName;
}

public class httpData
{
    public int code;
    public string msg;
    public object data;
}
//其他空间数据
public class OtherSpaceData
{

    public int ID;
    public int UserID;
    public string LandID;
    public string SpaceName;
    public string BuildXYZ;
    public string BuildName;
    //寄售类型 1 为可访问，2为购买
    public int StatusID;

    public int Price;
    public int OrderID;
    public int ProductID;
    public int NftProductSizeID;

}

/// <summary>
/// 宠物列表请求数据
/// </summary>
public class PetListData
{
    public string land_id;
    public PetListData(string landID)
    {
        land_id = landID;
    }
}

public class AidStationsPetListData
{
    public AidStationsPetListData()
    {

    }
}

/// <summary>
/// 宠物列表请求数据
/// </summary>
public class PetEnableAdoptData
{
    public int type;
    public PetEnableAdoptData(int _type)
    {
        type = _type;
    }
}

public class PetEnableAdoptRecData
{
    public int status;
    public string collection_name;
    public int collection_num;
}

/// <summary>
/// 领养请求
/// </summary>
public class PetAdoptData
{
    public string land_id;
    public PetAdoptData(string landID)
    {
        land_id = landID;
    }
}

public class AidStationAdoptData
{
    public AidStationAdoptData()
    {

    }
}
/// <summary>
/// 喂养请求
/// </summary>
public class PetFeedData
{
    public int pet_id;
    public int condition_type;
    public PetFeedData(int id, int typeID)
    {
        pet_id = id;
        condition_type = typeID;
    }
}
public class DogPetFeedData
{
    public int pet_id;
    public int condition_type;
    public int feed_time;
    public DogPetFeedData(int id, int typeID, int feedTime)
    {
        pet_id = id;
        condition_type = typeID;
        feed_time = feedTime;
    }
}

public class PetListRecData
{
    public int exp;
    public int lo_exp;
    public int status;
    public int id;
    public int keep_id;
    public int pet_type;
}

/// <summary>
/// 创建宠物请求数据
/// </summary>
public class CreatePetData
{
    public int pet_id;
    public CreatePetData(int id)
    {
        pet_id = id;
    }
}

public class CreatePetRecData
{
    public string pet_name;
    public string birthday;
    public string pet_number;
    public int pet_type;
}

public class PetModelRecData
{
    public int exp;
    public int lo_exp;
    public int id;
    public int keep_id;
    public int sleep_status;
    public int pet_type;
    public string pet_name;
    public string birthday;
    public string pet_number;
    public int pet_box_id;
    public List<PetCondition> pet_condition = new List<PetCondition>();
    public int status;
    public List<PetAttribute> pet_attribute = new List<PetAttribute>();
    public bool bAdopted = true;//客户端自己加的标识领养状态
    public int pet_remain_train_num;//剩余训练
    public int pet_train_total;//训练总数
    public TrainInfo train_info = new TrainInfo();
    public TrainResult train_result;
}

public class PetCondition
{
    public int cur_val;
    public int condition_type;
}
public class TrainInfo
{
    //训练结束时间
    public int train_end_time;
    //训练记录
    public int train_record_id;
    //训练配置id
    public int train_id;
}
public class TrainResult
{
    public string title;
    public int train_record_id;
    public List<TrainResultReward> reward = new List<TrainResultReward>();
}
public class PetAttribute
{
    public int cur_val;
    public int attribute_type;
}

public class TrainResultReward
{
    public string name;
    public int val;
}

public class PetAdoptV2ReqData
{
    public string pet_name;
    public string birthday;
    public string pet_number;
    public int pet_type;
    public int pet_id;
}
/// <summary>
/// 宠物（狗）进度数据
/// </summary>
public class PetSliderValueData
{
    public petcondition petcondition;
    public int cur_val;
}

/// <summary>
///用户GAS余额请求数据
/// </summary>
public class GasValueReqData
{
    public GasValueReqData()
    {

    }
}

public class LandIdReqData
{
    public string land_id;
    public LandIdReqData(string landId)
    {
        land_id = landId;
    }
}

public class AidStationLoveCoinReqData
{
    public AidStationLoveCoinReqData()
    {

    }
}

public class LoveCoinRecData
{
    public int remain_lovecoin;
    public List<PetLoveCoin> list;
}

public class PetLoveCoin
{
    public int pet_id;
    public int lovecoin;
}

public class LoveCoinReceiveReqData
{
    public int pet_id;
    public LoveCoinReceiveReqData(int petId)
    {
        pet_id = petId;
    }
}

public class LoveCoinReceiveRecData
{
    public int remain_lovecoin;
}

public class FecesRecData
{
    public List<PetFeces> list;
}

public class PetFeces
{
    public int id;
}

public class ClearFecesReqData
{
    public List<int> ids;
    public ClearFecesReqData(List<int> _ids)
    {
        ids = _ids;
    }
}

public class OnKeyClearFecesReqData
{
    public List<int> ids;
    public OnKeyClearFecesReqData(List<int> _ids)
    {
        ids = _ids;
    }
}

public class ClearFecesRecData
{
    public int remain_lovecoin;
    public int reward_lovecoin;
}
public class PetShopItemData
{
    //游戏物品ID
    public int id;
    //实物id
    public int item_id;
    //名字
    public string item_name;
    //剩余库存
    public int inventory;
    //限购数
    public int sale_quota;
    //货币类型 1：gas/2：爱心币
    public int coin_type;
    //已购数量
    public int buy_count;
    //价格
    public int price;
    //icon 图
    public Image icon_Image;
}

public class PetShopTypeData
{
    public int type;
    public PetShopTypeData(int typeId)
    {
        type = typeId;
    }
}
public class PetShopSendData
{
    public int id;
    public PetShopSendData(int buyId)
    {
        id = buyId;
    }
}
public class PetClearTrainResultData
{
    public int train_record_id;
    public PetClearTrainResultData(int trainRecordId)
    {
        train_record_id = trainRecordId;
    }
}

public class PetTrainData
{
    public int pet_id;
    public int train_id;


    public PetTrainData(int petID, int TrainID, int originId = 0)
    {
        pet_id = petID;
        train_id = TrainID;

    }
}
public class PetTrainFinishData
{
    public int train_record_id;
    public PetTrainFinishData(int trainRecordId)
    {
        train_record_id = trainRecordId;
    }
}
/// <summary>
/// 人物身上所穿戴的模型信息
/// </summary>
public class PlayerAvatarData
{
    public int item_id;
    public GameObject item_obj;

}


/// <summary>
/// 换装相关数据结构
/// </summary>
public class OutFitRecData
{
    public int avatar_type1;
    public List<SecondLevelData> second_data;
}

public class SecondLevelData
{
    public int avatar_type2;
    public List<ThreeLevelData> list;
}

public class ThreeLevelData
{
    public int avatar_id;
    public string avatar_name;
    public int avatar_type1;
    public int avatar_type2;
    public int avatar_rare;
    public int has_num;
    public int status;
}


public class MyOutFitRecData
{
    public int avatar_id;
}
public class MyOutFitSaveReqData
{
    public List<MyOutFitRecData> data = new List<MyOutFitRecData>();
}

public class ShopOutFitReqData
{
    public int item_type1;
    public ShopOutFitReqData(int type)
    {
        item_type1 = type;
    }
}

public class ShopOutFitRecData
{
    public int avatar_type1;
    public List<ShopOutFitItem> list;
}

public class ShopOutFitItem
{
    public int num;
    public int avatar_id;
    public string avatar_name;
    public int avatar_type1;
    public int avatar_type2;
    public int avatar_skin_color;
    public int avatar_rare;
    public int avatar_quantity;
    public int avatar_default;
    public int items_limited;
    public int items_limited_quantity;
    public int items_limited_quantity_sales;
    public string items_limited_time_on;
    public string items_limited_time_off;
    public int items_limited_time_off_timestamp;
    public int is_new;
    public int items_sale_status;
    public int has_num;
}

public class EmptyReqData
{
    public EmptyReqData()
    {

    }
}

public class RoomAreaData
{
    public RoomAreaData(int left, int right, int top, int bottom)
    {
        leftBoundary = left;
        rightBoundary = right;
        topBoundary = top;
        bottomBoundary = bottom;
    }
    public float leftBoundary;
    public float rightBoundary;
    public float topBoundary;
    public float bottomBoundary;
}

/// <summary>
/// 狗房间Item数据
/// </summary>
public class DogRoomItemData
{
    public int item_id;
    public string item_name;
    public int item_default;
    public int is_select;
    public int is_buy;

    public int is_UISelect;
}
public class DogServerData
{
    public DogServerData(string landId, int itemID)
    {
        land_id = landId;
        item_id = itemID;
    }
    public string land_id;
    public int item_id;
}
public class DogUIOutData
{
    public DogUIOutData(bool isout, int itemId)
    {
        isOut = isout;
        item_id = itemId;
    }
    public bool isOut;
    public int item_id;
}

public class ExchangeProofReqData
{
    public int pet_id;
    public ExchangeProofReqData(int petId)
    {
        pet_id = petId;
    }
}

public class ExchangeProofRecData
{
    public int total;
}

public class CharacterObjData
{
    public int ItemID;
    public GameObject playerObj;
}

public class TreasureFriendListRecData
{
    public string login_name;
    public string user_pic_url;
    public string age;
    public string constell;
    public string code;
    public string gender;
    public int state;
    public int user_id;
}

public class OtherPlayerAvatarData
{
    public OtherPlayerAvatarData(int modelId, GameObject playerObj)
    {
        modelType = modelId;
        playerModel = playerObj;
        avatarIds = new List<int>();
        avatarObjs = new Dictionary<int, PlayerAvatarData>();
    }
    public int modelType;
    public GameObject playerModel;
    public GameObject HeadBodyObj;
    public List<int> avatarIds;
    public Dictionary<int, PlayerAvatarData> avatarObjs;

}

public class TreasureExchangeReqData
{
    public int qty;
    public TreasureExchangeReqData(int num)
    {
        qty = num;
    }
}

public class TicketNumRecData
{
    public int ticket_num;
    public int is_use;//0当天未使用大于0当天使用了
}


public class CollectioListRecData
{
    public int icollection_id;
    public int collectiont_size_id;
    public string product_name;
    public int num;//拥有数量
    public int collection_quanity;//消耗数量
    public string collection_icon;
}

/// <summary>
/// 个人资料页数据
/// </summary>
public class PersonUserData
{
    //年龄
    public string age;
    //星座
    public string constell;
    //性别
    public string gender;
    //个人介绍
    public string explain;
    //名称
    public string login_name;

    //ID 
    public string code;
    
    //头像
    public string user_pic_url;
}

public class PersonUserSaveTopData
{
    public PersonUserSaveData data = new PersonUserSaveData();
}
/// <summary>
/// 个人资料页 用于保存的数据
/// </summary>
public class PersonUserSaveData
{
    //年龄
    public string age;
    //星座
    public string constell;
    //性别
    public string gender;
    //个人介绍
    public string explain;
    //名称
    public string login_name;

}
/// <summary>
/// 用于获取资料页总数量
/// </summary>
public class PersonNumData
{
    public string[] market_types;
    public ulong user_id;
}
public class PersonImageData
{
    public string market_type;
    public int page;
    public int page_size;
    public ulong user_id;
}
//图片信息
public class PersonPicture
{
    public string product_title;
    public string product_picture;
    public int num;
    public Sprite sprite;
}
/// <summary>
/// 个人资料（藏品，盲盒，等的数据）
/// </summary>
public class PersonPictureListData
{
    public bool isHaveCollection = false;
    public int collectionPage = 0;
    public int collectionMaxCount = 40;
    public int collectionNowCount = 0;
    public List<PersonPicture> collectionPictureList = new List<PersonPicture>();

    public bool isHaveLand = false;
    public int landPage = 0;
    public int landMaxCount = 30;
    public int landNowCount = 0;
    public List<PersonPicture> landPictureList = new List<PersonPicture>();


    public bool isHaveBox = false;
    public int boxPage = 0;
    public int boxMaxCount = 20;
    public int boxNowCount = 0;
    public List<PersonPicture> boxPictureList = new List<PersonPicture>();


    public bool isHaveNum = false;
    public int numPage = 0;
    public int numMaxCount = 20;
    public int numNowCount = 0;
    public List<PersonPicture> numPictureList = new List<PersonPicture>();

}
public class CustomData
{
    public ulong userID; //用户id
    public uint projectID; //项目id
    public uint roomID; //房间id
}


