using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlorSceneStart : MonoBehaviour
{


    /// <summary>
    /// 所有家具的根节点
    /// </summary>
    public static GameObject furnitureNode;
    /// <summary>
    /// 所有家具的位置根节点
    /// </summary>
    public static GameObject furniturePosNode;


    private void Awake()
    {
        RoomFurnitureCtrl.Instance().startPos = new Vector3(-5.4f, 0, 5.564f);
        RoomFurnitureCtrl.Instance().startEulerAngles = new Vector3(0f, 180f, 0f);
        RoomFurnitureCtrl.Instance().SetSenceCameraDataFun(1);
        GameObject furnitureNode = GameObject.Find("AllFurniture_Root");
        GameObject furniturePosNode = GameObject.Find("AllFurniturePos_Root");
        RoomFurnitureCtrl.Instance().InitNodeFun(furnitureNode, furniturePosNode);

        // 初始化所有家具位置点
        RoomFurnitureCtrl.Instance().GetAllFurnitureNodeFun();
        //初始化家具
        RoomFurnitureCtrl.Instance().SetStartFurnitureFun();
        //初始化藏品
        RoomFurnitureCtrl.Instance().SetStartPictureFun();

    }
}
