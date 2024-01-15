using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;

public class DogDecorateItem : BaseUIForm
{

    public Button clickBtn;

    private int itemIndex;

    public Text Name_Text;

    public Text GasValue_Text;

    public GameObject Lock_Obj;

    public Image Icon_Img;

    public GameObject Star_Obj;

    public GameObject SelectObj;

    private void Awake()
    {
        clickBtn.onClick.AddListener(OnClickListenFun);

        ReceiveMessage("RefreshRoomItemSelect", p =>
        {
            int index = (int)p.Values;
            if (index != itemIndex)
            {
                SetSelectImgFun(false);
            }
        });

    }
    void OnClickListenFun()
    {

        ChangeRoomManager.Instance().SetUISelectFun(itemIndex - 1);

        SendMessage("ClickDogDecorateItem", "data", itemIndex);

        SendMessage("RefreshRoomItemSelect", "data", itemIndex);

        SetSelectImgFun(true);
    }
    /// <summary>
    /// 设置房间的数据
    /// </summary>
    /// <param name="index"></param>
    public void SetDogDecorateItemDataFun(int index)
    {
        DogRoomItemData itemData = ChangeRoomManager.Instance().AllDogRoomItemData[index - 1];
        petHouse petHouse = ManageMentClass.DataManagerClass.GetPetHouseTableFun(itemData.item_id);
        Debug.Log("petHouse.item_icon：    "+ petHouse.item_icon);
        Sprite texture = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(petHouse.item_icon);
        Icon_Img.sprite = texture;
        itemIndex = index;
        Star_Obj.SetActive(true);
        if (itemData.is_buy == 1)
        {
            Lock_Obj.SetActive(false);
        }
        else
        {
            if (itemData.item_default != 0)
            {
                Star_Obj.SetActive(false);
                Lock_Obj.SetActive(false);
            }
            else
            {
                Lock_Obj.SetActive(true);
                GasValue_Text.text = petHouse.item_price + "";
            }
        }
        if (itemData.is_UISelect == 1)
        {
            SelectObj.SetActive(true);
        }
        else
        {
            SelectObj.SetActive(false);
        }
        Name_Text.text = petHouse.item_name;
    }
    /// <summary>
    /// 设置选中开关
    /// </summary>
    /// <param name="isSelect"></param>
    public void SetSelectImgFun(bool isSelect)
    {
        SelectObj.SetActive(isSelect);
    }

}
