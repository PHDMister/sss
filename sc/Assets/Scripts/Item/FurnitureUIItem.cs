using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class FurnitureUIItem : BaseUIForm
{
    private Button clickBtn;
    private string framePictureKey;
    private Image furnitureUIImage;
    private FurnitureType furnitureType;
    private GameObject tip_Panel;
    private Text numCount_Text;
    private Text FurnitureName_Text;
    //边框
    private GameObject FrameImg;

    FurntureHasDataClass furndata;

    private void Awake()
    {
        clickBtn = transform.GetComponent<Button>();
        furnitureUIImage = transform.Find("Icon_Img").GetComponent<Image>();
        tip_Panel = transform.Find("TipCount_Panel").gameObject;
        numCount_Text = transform.Find("TipCount_Panel/TipCount_Text").GetComponent<Text>();
        FurnitureName_Text = transform.Find("FurnitureName_Text").GetComponent<Text>();
        FrameImg = transform.Find("Frame_Img").gameObject;
        clickBtn.onClick.AddListener(OnClickListenFun);
    }
    void OnClickListenFun()
    {
        if (furnitureType == FurnitureType.hua)
        {

            SendMessage("ClickPictureUI", "data", framePictureKey);
        }
        else
        {
            SendMessage("ClickFurnitureUI", "data", furndata);
        }
        //  SendMessage("Furniture", "data", furnitureRootData);
    }
    //设置数据
    public void SetFurnitureUiItemDataFun(int indexId, FurnitureType furnitureTypeData)
    {
        furnitureType = furnitureTypeData;
        Debug.Log("输出一下这里的 RoomFurnitureCtrl.Instance().furntureHasData[furnitureTypeData]长度值： " + RoomFurnitureCtrl.Instance().furntureHasData[furnitureTypeData].Count + "  furnitureTypeData:  " + furnitureTypeData.ToString() + "    indexId::  " + indexId);
        furndata = RoomFurnitureCtrl.Instance().furntureHasData[furnitureTypeData][indexId - 1];
        Debug.Log("输出嫌疑啊item具体的值的内容： " + furndata.ToJSON());
        tip_Panel.SetActive(true);
        FrameImg.SetActive(false);
        FurnitureName_Text.gameObject.SetActive(true);
        FurnitureName_Text.text = TextTools.setCutAddString(ManageMentClass.DataManagerClass.GetItemTableFun(furndata.furnitureId).item_name, 7, "...");

        if (!furndata.isInitial)
        {
            numCount_Text.text = "X" + furndata.hasNum.ToString();
        }
        else
        {
            numCount_Text.text = "默认";
        }
        // itemData.furnitureId = furndata.furnitureId;
        string textName = ManageMentClass.DataManagerClass.GetItemTableFun(furndata.furnitureId).item_icon;
        if (textName != null)
        {
            Sprite texture = InterfaceHelper.GetIconFun(textName);
            if (texture != null)
            {
                furnitureUIImage.sprite = texture;
            }
            else
            {
                Debug.Log("没有找到，为空");
            }
        }
    }
    /// <summary>
    /// 藏品画类型的值
    /// </summary>
    /// <param name="key"></param>
    public void SetFramePictureDataFun(string key)
    {
        furnitureType = FurnitureType.hua;
        tip_Panel.SetActive(false);
        FrameImg.SetActive(true);
        FurnitureName_Text.gameObject.SetActive(false);
        if (RoomFurnitureCtrl.Instance().allFramePictureData.ContainsKey(key))
        {
            furnitureUIImage.sprite = RoomFurnitureCtrl.Instance().allFramePictureData[key].frameSprite;
            framePictureKey = key;
        }
    }
}
