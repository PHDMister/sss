using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameScrollPanel : BaseUIForm
{
    private CircularScrollView.UICircularScrollView GameScroll_GameRecord;
    private FurnitureRootData furnitureRootData;
    private Text TipNameText;
    FurnitureType furnitureType;
    //  private GameObject Shop_Panel;
    private List<string> arrPictureKeys = new List<string>();
    //  private Button pickUpBtn;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
        GameScroll_GameRecord = transform.Find("Scroll_Panel/Scroll View").GetComponent<CircularScrollView.UICircularScrollView>();
        GameScroll_GameRecord.Init(NormalCallBack);
        TipNameText = transform.Find("Scroll_Panel/TipName_Text").GetComponent<Text>();
        RigisterButtonObjectEvent("Close_Btn", p =>
             {
                 UIManager.GetInstance().CloseUIForms(FormConst.GAMESCROLLPANEL);
                 furnitureRootData = null;
                 SendMessage("GameScrollPanelBtn", "data", "Close");
             });
        RigisterButtonObjectEvent("PickUP_Button", p =>
        {
            Debug.Log("收起这里面的内容");
            PickUpFun();
        });
        ReceiveMessage("Furniture",
               p =>
               {
                   furnitureRootData = p.Values as FurnitureRootData;
                   Debug.Log("输出一下根节点的名字： " + furnitureRootData.furnitureRoot.name + " 位置： " + furnitureRootData.PlaceID + "  类型： " + furnitureRootData.furnitureType);
                   // 点击不同的家具根节点按钮
                   CalcFurnitureTypeFun(furnitureRootData);
                   //发光
                   if (RoomFurnitureCtrl.Instance().arrTemporaryLight.Count > 0)
                   {
                       if (RoomFurnitureCtrl.Instance().arrTemporaryLight[0].placeID != furnitureRootData.PlaceID)
                       {
                           InterfaceHelper.SetFurnitureLightFun(RoomFurnitureCtrl.Instance().arrTemporaryLight[0].rootObj, false);
                           RoomFurnitureCtrl.Instance().arrTemporaryLight.Clear();
                           PlaceGameObj placeData = new PlaceGameObj();
                           placeData.placeID = furnitureRootData.PlaceID;
                           placeData.rootObj = furnitureRootData.furnitureRoot;
                           RoomFurnitureCtrl.Instance().arrTemporaryLight.Add(placeData);
                           InterfaceHelper.SetFurnitureLightFun(placeData.rootObj, true);
                       }
                   }
                   else
                   {
                       PlaceGameObj placeData = new PlaceGameObj();
                       placeData.placeID = furnitureRootData.PlaceID;
                       placeData.rootObj = furnitureRootData.furnitureRoot;
                       RoomFurnitureCtrl.Instance().arrTemporaryLight.Add(placeData);
                       InterfaceHelper.SetFurnitureLightFun(placeData.rootObj, true);
                   }
                   if (GameScroll_GameRecord == null)
                   {
                       Debug.Log("速出i小阿三的放假啦看见对方老大");
                   }
                   GameScroll_GameRecord.UpdateList();
               }
          );
        ReceiveMessage("ClickFurnitureUI",
               p =>
               {
                   // 点击家具UI
                   FurntureHasDataClass itemData = p.Values as FurntureHasDataClass;

                   if (!itemData.isInitial && itemData.hasNum <= 0)
                   {
                       /* Debug.Log("家具为0的值： ");
                        OpenUIForm(FormConst.SHOPITEMPRVIEW_UIFORM);
                        Debug.Log("itemData.furnitureId的内容： " + itemData.furnitureId);
                        SendMessage("ShopFurntureItem", "ItemClick", itemData.furnitureId);*/
                   }
                   else
                   {
                       Debug.Log("这里的itemdata的内容值： " + itemData.ToJSON() + "  这里的值： " + furnitureRootData.PlaceID);
                       RoomFurnitureCtrl.Instance().ChangeFurnitureFun(itemData.furnitureId, furnitureRootData.PlaceID, false);
                       GameScroll_GameRecord.UpdateList();
                   }
               }
          );
        ReceiveMessage("ClickPictureUI",
               p =>
               {
                   //点击藏品图UI
                   string PictureKey = p.Values as string;
                   RoomFurnitureCtrl.Instance().ChangePictureFun(PictureKey, furnitureRootData.PlaceID);
               }
          );
        ReceiveMessage("ShopBuySuccess",
           p =>
           {
               UIManager.GetInstance().CloseUIForms(FormConst.SHOPITEMPRVIEW_UIFORM);
               GameScroll_GameRecord.UpdateList();
           }

       );

        ReceiveMessage("RefshScrlllPanel",
               p =>
               {
                   //一键装修，一键收起
                   GameScroll_GameRecord.UpdateList();
               }
          );
    }





    /// <summary>
    /// 收起功能
    /// </summary>
    void PickUpFun()
    {
        if (furnitureRootData != null)
        {
            //检测并取消角色与建筑的交互
            Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative(1);

            int placeID = furnitureRootData.PlaceID;
            if (furnitureRootData.furnitureType == FurnitureType.hua)
            {
                //收起画
                RoomFurnitureCtrl.Instance().PickUpFramePictureFun(placeID);
            }
            else if (furnitureRootData.furnitureType != FurnitureType.None)
            {
                //收起家具
                RoomFurnitureCtrl.Instance().PickUpFrameFun(placeID, furnitureRootData.furnitureType, false);
                GameScroll_GameRecord.UpdateList();
            }
        }
    }
    /// <summary>
    ///  处理不同类型得藏品和家具
    /// </summary>
    /// <param name="furnitureType"></param>
    void CalcFurnitureTypeFun(FurnitureRootData rootData)
    {
        furnitureType = rootData.furnitureType;
        switch (furnitureType)
        {
            case FurnitureType.hua:
                arrPictureKeys.Clear();
                var arrKeys = RoomFurnitureCtrl.Instance().allFramePictureData.Keys;
                foreach (var item in arrKeys)
                {
                    arrPictureKeys.Add(item);
                }
                GameScroll_GameRecord.ShowList(arrPictureKeys.Count);
                TipNameText.text = "藏品摆放";
                break;
            case FurnitureType.None:
                Debug.Log("报错，类型为空");
                break;
            default:
                bool isHave = IsHaveServerFurnTypeFun(furnitureType);
                Debug.Log("输出一下FurniturType的值： " + furnitureType + " IsHave的值： " + isHave + RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Count);
                if (!isHave)
                {
                    RoomFurnitureCtrl.Instance().arrPostFurnitureTypeData.Add(furnitureType);
                    StartCoroutine(GetPostAction());
                }
                else
                {
                    GameScroll_GameRecord.ShowList(RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Count);
                }
                TipNameText.text = "家具摆放";
                break;
        }
    }
    public bool IsHaveServerFurnTypeFun(FurnitureType type)
    {
        for (int i = 0; i < RoomFurnitureCtrl.Instance().arrPostFurnitureTypeData.Count; i++)
        {
            if (type == RoomFurnitureCtrl.Instance().arrPostFurnitureTypeData[i])
            {
                return true;
            }
        }
        return false;
    }
    public IEnumerator GetPostAction()
    {

        HttpRequest httpRequest = new HttpRequest();
        FurntureTypeClass typeData = new FurntureTypeClass(furnitureRootData.furnitureType.ToString());
        string data = JsonConvert.SerializeObject(typeData);
        Debug.Log("数据得内容： " + data);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.SelfFurnitureProps, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            if (RoomFurnitureCtrl.Instance().furntureHasData.ContainsKey(furnitureType))
            {
                RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Clear();
            }
            else
            {
                List<FurntureHasDataClass> listData = new List<FurntureHasDataClass>();
                RoomFurnitureCtrl.Instance().furntureHasData.Add(furnitureType, listData);
            }
            JObject jo = JObject.Parse(httpRequest.value);
            Debug.Log("加载框的时候的值： " + jo);
            if (jo["code"].ToString() == "0")
            {
                var FrameData = jo["data"]["list"];
                //  bool isFirst = false;
                int dataCount = ManageMentClass.DataManagerClass.GetInitialCountFun();
                foreach (var item in FrameData)
                {
                    // int hasNum = (int)item["has_num"];
                    FurntureHasDataClass furnitureHasData = new FurntureHasDataClass();
                    furnitureHasData.furnitureId = (int)item["item_id"];
                    furnitureHasData.hasNum = (int)item["has_num"];
                    /*if (!isFirst)
                    {*/
                    Debug.Log("输出一下长度：" + dataCount);
                    if (dataCount > 0)
                    {
                        for (int i = 1; i <= dataCount; i++)
                        {
                            if (ManageMentClass.DataManagerClass.GetInitialTableFun(i).initial_itemID == furnitureHasData.furnitureId)
                            {
                                furnitureHasData.isInitial = true;
                                break;
                            }
                        }
                    }
                    //第一次判断是否是初始数据
                    Debug.Log("输出一下是否为初始的值：  " + furnitureHasData.isInitial);
                    /*  }
                      isFirst = true;*/
                    RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Add(furnitureHasData);
                }
                GameScroll_GameRecord.ShowList(RoomFurnitureCtrl.Instance().furntureHasData[furnitureType].Count);
            }
        }
    }
    private void NormalCallBack(GameObject cell, int index)
    {
        if (cell != null)
        {
            Debug.Log("cell存在： " + index);
        }
        else
        {
            Debug.Log("不存在： ");
        }
        if (furnitureType == FurnitureType.hua)
        {
            //设置画的方法
            SetPictureFrameData(cell, arrPictureKeys[index - 1]);
        }
        else
        {
            //设置里面的数据
            SetItemData(cell, index);
        }
    }
    /// <summary>
    /// 设置物体的数据和icon等信息
    /// </summary>
    /// <param name="cell"></param>
    public void SetItemData(GameObject cell, int index)
    {
        FurnitureUIItem uiItem = cell.GetComponent<FurnitureUIItem>();
        //这里面要填写 物品ID   目前测试先传一个index
        uiItem.SetFurnitureUiItemDataFun(index, furnitureType);
    }
    public void SetPictureFrameData(GameObject cell, string key)
    {
        FurnitureUIItem uiItem = cell.GetComponent<FurnitureUIItem>();
        //这里面要填写 物品ID   目前测试先传一个index
        uiItem.SetFramePictureDataFun(key);
    }
    // Update is called once per frame
    public bool IsVisible()
    {
        return transform.gameObject.activeSelf;
    }
    public void SetVisible(bool isOpen)
    {
        transform.gameObject.SetActive(isOpen);
    }
}
