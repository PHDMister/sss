using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FurnitureType
{
    //沙发
    shafa,
    //卧室门
    woshimen,
    //植物
    zhiwu,
    //吸顶灯
    xidingdeng,
    //杂物
    zawu,
    //左装饰墙
    zhuangshiqiangL,
    //左装饰墙
    zhuangshiqiangR,
    //茶几
    chaji,
    //床边柜
    chuangbiangui,
    //换鞋椅
    huanxieyi,
    //墙角柜
    qiangjiaogui,
    //入户柜
    ruhugui,
    //餐厅窗
    cantingchuang,
    //电视柜
    TVgui,
    //餐桌
    canzhuo,
    //餐厅凳
    cantingdeng,
    //窗户
    chuang,
    //画
    hua,
    //卧室床
    BRbed,
    //卧室窗户
    BRwindow,
    //卧室床头架
    BRchuangtoujia,
    //卧室床头桌
    BRchuangtouzhuo,
    //卧室马桶
    BRmatong,
    //卧室书桌
    BRshuzhuo,
    //卧室灯
    BRwoshideng,
    //卧室柜
    BRwoshigui,
    //卧室洗漱台
    BRxishutai,
    //卧室衣柜
    BRyigui,
    //卧室浴缸
    BRyugang,
    //卧室厕所门
    BRWCmen,

    //空类型
    None
}
//宠物动画状态
public enum PetStateAnimationType
{
    //待机
    Idle,
    //喂食
    Feed,
    //清洁
    Clean,
    //玩具
    Toy,
    //治疗
    Cure,
    //休息
    Sleep,
    //训练
    Train,
}

public enum LoadSceneType
{
    //客厅
    parlorScene = 1,
    //狗窝
    dogScene = 2,
    //救助站
    ShelterScene = 3,
    //挖宝
    TreasureDigging = 4,
    //卧室
    BedRoom = 5,


    //彩虹沙滩
    RainbowBeach = 6,
    //神秘海湾
    ShenMiHaiWan = 7,
    //海底星空
    HaiDiXingKong = 8,



    ModuleTest1,
    ModuleTest2
}
 





public class MethodCollection
{
    //根据根节点名字获取类型
    public FurnitureType GetFurnitureTypeFun(string rootName)
    {
        if (rootName != null)
        {
            switch (rootName)
            {
                case "woshimen":
                    return FurnitureType.woshimen;
                case "xidingdeng":
                    return FurnitureType.xidingdeng;
                case "zawu":
                    return FurnitureType.zawu;
                case "zhiwu":
                    return FurnitureType.zhiwu;
                case "zhuangshiqiangL":
                    return FurnitureType.zhuangshiqiangL;
                case "zhuangshiqiangR":
                    return FurnitureType.zhuangshiqiangR;
                case "chaji":
                    return FurnitureType.chaji;
                case "chuangbiangui":
                    return FurnitureType.chuangbiangui;
                case "huanxieyi":
                    return FurnitureType.huanxieyi;
                case "qiangjiaogui":
                    return FurnitureType.qiangjiaogui;
                case "ruhugui":
                    return FurnitureType.ruhugui;
                case "shafa":
                    return FurnitureType.shafa;
                case "TVgui":
                    return FurnitureType.TVgui;
                case "canzhuo":
                    return FurnitureType.canzhuo;
                case "cantingdeng":
                    return FurnitureType.cantingdeng;
                case "cantingchuang":
                    return FurnitureType.cantingchuang;
                case "chuang":
                    return FurnitureType.chuang;
                case "zhuangshihua":
                    return FurnitureType.hua;
                case "BRbed":
                    return FurnitureType.BRbed;
                case "BRwindow":
                    return FurnitureType.BRwindow;
                case "BRchuangtoujia":
                    return FurnitureType.BRchuangtoujia;
                case "BRchuangtouzhuo":
                    return FurnitureType.BRchuangtouzhuo;
                case "BRmatong":
                    return FurnitureType.BRmatong;
                case "BRshuzhuo":
                    return FurnitureType.BRshuzhuo;
                case "BRwoshideng":
                    return FurnitureType.BRwoshideng;
                case "BRwoshigui":
                    return FurnitureType.BRwoshigui;
                case "BRxishutai":
                    return FurnitureType.BRxishutai;
                case "BRyigui":
                    return FurnitureType.BRyigui;
                case "BRyugang":
                    return FurnitureType.BRyugang;
                case "BRWCmen":
                    return FurnitureType.BRWCmen;
                default:
                    return FurnitureType.None;
            }
        }
        return FurnitureType.None;
    }


    /// <summary>
    /// 根据类型返回数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetFurnitureNameFun(FurnitureType type)
    {
        return type.ToString();
    }

}
