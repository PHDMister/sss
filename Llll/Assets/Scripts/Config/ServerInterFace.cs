using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInterFace
{
    //获取token的
    public string GetToken = "/aiera/v2/hotdog/user_account/code_login";
    // 获取土地的区域ID的集合
    public string GetAreaLandList = "/aiera/v1/game/metaverse/area/list";
    //获取某个区域中拥有的土地ID的集合
    public string GetLandIdList = "/aiera/v1/game/metaverse/land/list";
    //hotdog登录接口
    public string HD_Loging = "/aiera/v1/game/commapi/hd_login";
    //个人空间获取初始信息接口
    public string Game_Loging = "/aiera/v1/game/space/basic/info";
    //商城道具列表接口
    public string MallProps = "/aiera/v1/game/space/mall/list";
    //商城道具列表接口
    public string MallBuy = "/aiera/v1/game/space/item/buy";
    //个人家具列表数据
    public string SelfFurnitureProps = "/aiera/v1/game/space/furniture/list";
    //动作使用
    public string UseAction = "/aiera/v1/game/space/animation/use";
    //动作使用列表
    public string UseActionList = "/aiera/v1/game/space/animation/userlog";
    //
    public string ReplaceFurniture = "/aiera/v1/game/space/basic/save";
    //装修
    public string SetPicture = "/aiera/v1/game/metaverse/set/decorative/painting";

    //角色列表
    public string CharacterList = "/aiera/v1/game/space/hotman/list";
    //兑换角色
    public string ExchangeCharacter = "/aiera/v1/game/space/item/exchange";
    //角色替换
    public string ReplaceCharacter = "/aiera/v1/game/space/hotman/replace";

    //装饰画列表
    public string PlacePicture = "/aiera/v1/game/metaverse/decorative/painting/list";
    //其他空间
    public string OtherSpace = "/aiera/v1/game/space/userspacelist";

    //宠物列表
    public string PetList = "/aiera/v1/game/pet/num";

    //是否有资格领养
    public string PetEnableAdopt = "/aiera/v1/game/pet/has";

    //领养
    public string PetAdopt = "/aiera/v1/game/pet/adopt";

    //喂养
    public string PetFeed = "/aiera/v1/game/pet/feeding";

    //创建宠物获取编号
    public string CreatePet = "/aiera/v1/game/pet/pet_number";

    //二期领养
    public string PetAdoptV2 = "/aiera/v1/game/pet/pet_adopt";

    //用户GAS余额
    public string GasValue = "/aiera/v1/game/gas/balance";

    //爱心币列表(爱心币产出)
    public string LoveCoinList = "/aiera/v1/game/pet/lovecoin/list";

    //爱心币领取
    public string LoveCoinReceive = "/aiera/v1/game/pet/lovecoin/receive";

    //粪便列表
    public string FecesList = "/aiera/v1/game/pet/feces/list";

    //粪便清理
    public string ClearFeces = "/aiera/v1/game/pet/feces/clear";

    //宠物兑换凭证
    public string ExchangeProof = "/aiera/v1/game/pet/exchange_proof";

    //宠物兑换凭证
    public string ExchangeProofNum = "/aiera/v1/game/pet/proof_num";

    //宠物商店数据
    public string PetShopReceive = "/aiera/v1/game/pet/product/list";

    //宠物商店购买
    public string PetShopBuyReceive = "/aiera/v1/game/pet/product/buy";

    //宠物训练
    public string PetTrain = "/aiera/v1/game/pet/train";

    //结束训练
    public string PetTrainFinish = "/aiera/v1/game/pet/train/finish";

    //清除训练成果弹框
    public string PetClearResultReceive = "/aiera/v1/game/pet/train-result/clear";

    //****************************************************换装相关************************************************
    //当前拥有服装列表
    public string MyOutFitList = "/aiera/v1/game/space/my/avatar/list";
    //我的个人形象
    public string MyOutFit = "/aiera/v1/game/space/my/image";
    //我的个人形象保存
    public string MyOutFitSave = "/aiera/v1/game/space/my/image/save";
    //商店服装列表
    public string ShopOutFitList = "/aiera/v1/game/space/mall/list-new";
    //商店红点清除
    public string ShopRedDotClear = "/aiera/v1/game/space/mall/red-point/clear";
    //****************************************************宠物场景替换************************************************************
    //宠物场景列表
    public string DogRoomList = "/aiera/v1/game/pet/space/house_list";
    //场景使用
    public string DogRoomUse = "/aiera/v1/game/pet/space/house_use";
    //场景保存(购买)
    public string DogRoomBuy = "/aiera/v1/game/pet/space/house_save";
    //****************************************************挖宝************************************************************
    //消耗藏品列表
    public string CollectioList = "/aiera/v1/game/ticket/nft_list";
    // 兑换门票
    public string TreasureExchange = "/aiera/v1/game/ticket/exchange";
    //门票数量
    public string TicketsNum = "/aiera/v1/game/ticket/num";
    //好友列表
    public string FriendList = "/aiera/v1/game/ticket/frend_list";
    //使用寻宝券
    public string CostTicket = "/aiera/v1/game/ticket/use";


    //****************************************************个人资料************************************************************
    //获取个人资料
    public string GetPersonData = "/aiera/v1/game/ticket/userprofile";
    //设置个人资料
    public string SetPersonData = "/aiera/v1/game/ticket/userprofile_edit";
    // 个人资产
    public string GetPersonProperty = "/aiera/v1/game/ticket/userasset";
    //个人资产总量
    public string GetPersonAssetCount = "/aiera/v1/game/ticket/userassetcount";


    //****************************************************彩虹沙滩************************************************************
    //获取贝壳数量
    public string GetShellData = "/aiera/v1/game/island/usershell";
    public string GetShopData = "/aiera/v1/game/island/mall/list";
    public string ShopBuy = "/aiera/v1/game/island/mall/buy";
}
