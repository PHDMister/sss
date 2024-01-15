using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;


public class PetShopItem : BaseUIForm
{
    //商品图标Icon
    public Image icon_Img;


    //商品名称
    public Text itemName_Text;

    //爱心币数量
    public Text loveValue_Text;

    public GameObject select_Img;


    public Image btnMaskImg;



    private bool isCanBuy = false;


    private int ID;

    //设置item
    public void SetItemFun(int index)
    {
        if (index == 0)
        {
            select_Img.SetActive(true);
        }
        else
        {
            select_Img.SetActive(false);
        }
        isCanBuy = true;
        Debug.Log("lis的长度： " + ManageMentClass.DataManagerClass.ListPetShopItemId.Count + "  index:  " + index);
        ID = ManageMentClass.DataManagerClass.ListPetShopItemId[index];
        PetShopItemData shopItemData = ManageMentClass.DataManagerClass.DicPetShopItemData[ID];
        if (shopItemData != null)
        {
            itemName_Text.text = shopItemData.item_name;
            loveValue_Text.text = shopItemData.price + "";

            item m_ItemConfig = ManageMentClass.DataManagerClass.GetItemTableFun(shopItemData.item_id);
            if (m_ItemConfig != null)
            {
                Sprite texture = InterfaceHelper.GetPetShopIconFun(m_ItemConfig.item_icon);
                if (texture != null)
                {
                    icon_Img.sprite = texture;
                }
                else
                {
                    Debug.Log("没有找到，为空");
                }
            }
            AdjustGasUI();
        }
    }
    public void Awake()
    {
        RigisterButtonObjectEvent("ItemBuy_Btn",

            p => OnClickBuyBtnFun()

            );
        RigisterButtonObjectEvent("Click_Button",

           p => OnClickSelectFun()

           );
        ReceiveMessage("PetShopItemClick", p =>
        {
            int id = (int)p.Values;
            if (id == ID)
            {
                select_Img.SetActive(true);
            }
            else
            {
                select_Img.SetActive(false);
            }
        });

    }
    /// <summary>
    /// 购买
    /// </summary>
    public void OnClickBuyBtnFun()
    {
        if (isCanBuy)
        {
            SendMessage("PetShopItemBuyBtn", "Success", ID);
        }
        else
        {
            ToastManager.Instance.ShowPetToast("无法购买", 5f);
        }
    }

    public void OnClickSelectFun()
    {
        Debug.Log("输出一下正在点击宠物商城的ICON");
        SendMessage("PetShopItemClick", "Success", ID);
    }


    /// <summary>
    /// 适配
    /// </summary>
    public void AdjustGasUI()
    {
        loveValue_Text.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(loveValue_Text);
        /* float height = loveValueDi_Image.transform.GetComponent<RectTransform>().sizeDelta.y;
         loveValueDi_Image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(100 + width, height);*/
    }
    public void IsOpenMaskFun(bool isOpenMask)
    {
        /* iconMask_Img.gameObject.SetActive(isOpenMask);
         btnNomImg.gameObject.SetActive(!isOpenMask);*/
        btnMaskImg.gameObject.SetActive(isOpenMask);
    }

}
