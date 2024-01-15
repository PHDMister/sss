using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedRoomStart : MonoBehaviour
{
    private void Awake()
    {
        RoomFurnitureCtrl.Instance().startPos = new Vector3(-8.4f, 0, 3.37f);
        RoomFurnitureCtrl.Instance().startEulerAngles = new Vector3(0f, 90f, 0f);
        RoomFurnitureCtrl.Instance().GoOtherCleanDataFun();
        RoomFurnitureCtrl.Instance().SetSenceCameraDataFun(2);
        GameObject furnitureNode = GameObject.Find("AllBedRoomFurniture_Root");
        GameObject furniturePosNode = GameObject.Find("AllBedRoomFurniture_Pos");
        RoomFurnitureCtrl.Instance().InitNodeFun(furnitureNode, furniturePosNode);

        // 初始化所有家具位置点
        RoomFurnitureCtrl.Instance().GetAllFurnitureNodeFun();
        //初始化家具
        RoomFurnitureCtrl.Instance().SetStartFurnitureFun();
        //初始化藏品
        RoomFurnitureCtrl.Instance().SetStartPictureFun();

        //初始化额外的家具
        SetExtraObjFun();
    }

    private void SetExtraObjFun()
    {
        GameObject extraObj = RoomFurnitureCtrl.Instance().RoomExtraObjFun(1);
        extraObj.SetActive(true);
        extraObj.transform.position = new Vector3(-1.88f, 0f, 2.822f);
    }


}
