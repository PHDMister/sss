using System;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : BaseUIForm
{
    private Text txtName;
    private Image imgIcon;
    private Text txtDes;
    private Button btnBuy;
    private Image imgEnable;
    private Button notEnough;
    private Text txtPrice;

    private int _itemId;
    void Awake()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        imgIcon = transform.Find("imgIcon").GetComponent<Image>();
        txtDes = transform.Find("txtDes").GetComponent<Text>();
        btnBuy = transform.Find("btnBuy").GetComponent<Button>();
        imgEnable = transform.Find("btnBuy/imgEnable").GetComponent<Image>();
        notEnough = transform.Find("notEnough").GetComponent<Button>();
        txtPrice = transform.Find("btnBuy/Content/txtPrice").GetComponent<Text>();
            
        btnBuy.onClick.AddListener(OnBtnBuyClick);
        notEnough.onClick.AddListener(OnNotEnoughClick);
    }

    public void SetData(int itemId)
    {
        _itemId = itemId;
        
        var item = Singleton<BagMgr>.Instance.GetItem(itemId);
        txtName.text = item.Name;
        txtDes.text = getDesByItemType(item.Type, item.BigProbability);
        SetIcon(imgIcon,"ShellIcon", item.Icon);
        txtPrice.text = item.Price.ToString();

        var enough = Singleton<BagMgr>.Instance.ShellNum >= item.Price;
        //是否足够
        notEnough.gameObject.SetActive(!enough);
        //替换button底图
        var icon = enough ? "jingliname-bg_573_1955" : "jingliname-bg";
        SetIcon(imgEnable, "RainbowBeachShop", icon);
    }

    private void OnBtnBuyClick()
    {
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHSHOPCONFIRMPOP);
        var param = new RainbowBeachShopConfirmPopParam
        {
            itemId = _itemId
        };
        MessageCenter.SendMessage("OnRainbowBeachShopConfirmPopOpen", new KeyValuesUpdate("", param));
    }

    private void OnNotEnoughClick()
    {
        try
        {
            var url = "aiera://www.aiera.com/page/new/flutter/nft/shell";
            SetTools.SetPortraitModeFun();
            SetTools.CloseGameFun();
            
            SetTools.OpenWebUrl(url);
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }

    private string getDesByItemType(int type, int value)
    {
        if (type == 1)
        {
            return $"大奖概率：{value}%";
        }
        else if (type == 3)
        {
            return $"大鱼概率：{value}%";
        }
        else if (type == 4)
        {
            return $"活动时间：{getTimeDes(value)}";

        }else if (type == 5)
        {
            return $"大奖概率：{value}%";
        }else
        {
            return "";   
        }
    }

    private string getTimeDes(int time)
    {
        var value1 = time / 60;
        var value2 = time % 60;
        if (value1 == 0)
        {
            return value2 + "s";
        }else if (value2 == 0)
        {
            return value1 + "min";
        }
        else
        {
            return value1 + "min" + " " + value2 + "s";
        }
    }
}