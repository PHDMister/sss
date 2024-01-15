using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyTipsUIForm : BaseUIForm
{

    public Text m_TextCost;
    public Text m_TextName;

    private int m_ItemPrice = 0;
    private int m_ItemId = 0;
    private string m_ItemName = "";
    private int m_ItemType = 0;
    private int m_BuyNum = 0;

    public enum TableType
    {
        Furniture = 1,
        Action = 2,
        HotMan = 3,
    }

    void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;


        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnConfirm",
            p => OnBuyConfirm()
            );

        RigisterButtonObjectEvent("BtnCancel",
            p => OnBuyCancel()
            );

        ReceiveMessage("ShopFurntureBuy",
            p =>
            {
                object[] args = p.Values as object[];
                MallServerData m_MallServerData = (MallServerData)args[0];
                item m_ItemData = (item)args[1];
                m_ItemId = m_ItemData.item_id;
                m_BuyNum = (int)args[2];
                int itemType = m_ItemData.item_type1;
                m_ItemType = itemType;
                string typeName = "";
                switch (itemType)
                {
                    case 1:
                        typeName = "家具";
                        break;
                    case 2:
                        typeName = "动作";
                        break;
                    case 3:
                        typeName = "hotman";
                        break;
                }
                m_ItemName = m_ItemData.item_name;
                m_ItemPrice = m_MallServerData.price * m_BuyNum;
                SetBuyItemConfirmInfo(m_ItemName, typeName, m_ItemPrice);
            }
        );

        ReceiveMessage("ShopActionBuy",
            p =>
            {
                string[] args = p.Values as string[];
                int.TryParse(args[0], out m_ItemId);
                int itemType;
                int.TryParse(args[1], out itemType);
                m_ItemType = itemType;
                string typeName = "";
                switch (itemType)
                {
                    case 1:
                        typeName = "家具";
                        break;
                    case 2:
                        typeName = "动作";
                        break;
                    case 3:
                        typeName = "hotman";
                        break;
                }
                m_ItemName = args[2];
                int.TryParse(args[3], out m_ItemPrice);
                SetBuyItemConfirmInfo(m_ItemName, typeName, m_ItemPrice);
            }
        );
    }


    private void OnBuyConfirm()
    {
        if (ManageMentClass.DataManagerClass.gas_Amount < m_ItemPrice)
        {
            CloseUIForm();
            OpenUIForm(FormConst.SHOPGASTIPS_UIFORM);
        }
        else
        {
            if (m_ItemType == (int)TableType.Furniture)
            {
                RequestFurntureBuy();
            }
            else
            {
                RequestActionBuy();
            }
        }
    }

    public void RequestFurntureBuy()
    {
        StartCoroutine(RequestFurntureData());
    }
    IEnumerator RequestFurntureData()
    {
        HttpRequest httpRequest = new HttpRequest();
        BuyData buyData = new BuyData(m_ItemId, m_BuyNum);
        string data = JsonConvert.SerializeObject(buyData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallBuy, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                CloseUIForm();
                ToastManager.Instance.ShowNewToast(string.Format("恭喜获得{0}*{1}", m_ItemName, m_BuyNum), 5f);
                int[] args = new int[] { m_ItemId, m_BuyNum };
                MessageManager.GetInstance().RequestGasValue();
                RoomFurnitureCtrl.Instance().BuyFurnAddCountFun(m_ItemId, m_BuyNum);
                SendMessage("ShopBuySuccess", "BuySucess", args);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast(string.Format("购买{0}失败", m_ItemName), 5f);
        }
    }

    public void RequestActionBuy()
    {
        StartCoroutine(RequestData());
    }

    IEnumerator RequestData()
    {
        HttpRequest httpRequest = new HttpRequest();
        BuyData buyData = new BuyData(m_ItemId, 1);
        string data = JsonConvert.SerializeObject(buyData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.MallBuy, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                ToastManager.Instance.ShowNewToast(string.Format("恭喜获得{0}*{1}", m_ItemName, 1), 5f);
                int[] args = new int[] { m_ItemId, 1 };
                MessageManager.GetInstance().RequestGasValue();
                RoomFurnitureCtrl.Instance().BuyFurnAddCountFun(m_ItemId, 1);
                SendMessage("ShopBuySuccess", "BuySucess", args);
                CloseUIForm();
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast(string.Format("购买{0}失败", m_ItemName), 5f);
        }
    }

    private void OnBuyCancel()
    {
        CloseUIForm();
    }

    private void SetBuyItemConfirmInfo(string itemName, string itemType, int itemPrice)
    {
        m_TextCost.text = string.Format("{0},", itemPrice);
        m_TextName.text = string.Format("购买{0}“{1}”", itemType, itemName);
    }
}
