using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPreviewUIForm : BaseUIForm
{
    private mall m_MallData;
    private item m_ItemData;

    private int m_BuyNum = 1;
    private int m_CurCoin = 0;

    public InputField m_InputField;
    public Text m_NameText;
    public Text m_PriceText;
    public Text m_HaveNumText;
    public Image m_ImgBuyBg;
    public GameObject m_BtnBuy;
    private Vector2 m_InitAnchoredPosition;
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

        m_InputField.interactable = false;
        m_CurCoin = ManageMentClass.DataManagerClass.gas_Amount;
        /* 按钮的注册  */
        RigisterButtonObjectEvent("BtnClose",
            p => CloseUIForm()
        );

        RigisterButtonObjectEvent("BtnSub",
            p => SubBuyNum()
        );

        RigisterButtonObjectEvent("BtnAdd",
            p => AddBuyNum()
        );

        RigisterButtonObjectEvent("BtnBuy",
            p => OnBuyItem()
        );

        ReceiveMessage("ShopFurntureItem",
            p =>
            {
                int m_ItemId = (int)p.Values;
                m_MallData = ManageMentClass.DataManagerClass.GetMallTableFun(m_ItemId);
                m_ItemData = ManageMentClass.DataManagerClass.GetItemTableFun(m_ItemId);
                SetBuyItemInfo();
            }
       );

        ReceiveMessage("ShopActionItem",
            p =>
            {
                int m_ItemId = (int)p.Values;
                m_MallData = ManageMentClass.DataManagerClass.GetMallTableFun(m_ItemId);
                m_ItemData = ManageMentClass.DataManagerClass.GetItemTableFun(m_ItemId);
                SetBuyItemInfo();
            }
        );
        m_InitAnchoredPosition = m_BtnBuy.transform.GetComponent<RectTransform>().anchoredPosition;
    }

    private void SubBuyNum()
    {
        m_BuyNum--;
        if (m_BuyNum < 1)
            m_BuyNum = 1;
        SetBuyItemPrice();
    }
    private void AddBuyNum()
    {
        m_BuyNum++;
        int canBuyNum = Math.Max(1, m_CurCoin / m_MallData.price);
        int maxNum = Math.Min(m_BuyNum, canBuyNum);
        if (m_BuyNum > maxNum)
            m_BuyNum = maxNum;
        SetBuyItemPrice();
    }
    private void SetBuyItemInfo()
    {
        GameObject m_TextBuyObj = UnityHelper.FindTheChildNode(this.gameObject, "TextBuy").gameObject;
        GameObject m_BtnAdd = UnityHelper.FindTheChildNode(this.gameObject, "BtnAdd").gameObject;
        GameObject m_BtnSub = UnityHelper.FindTheChildNode(this.gameObject, "BtnSub").gameObject;

        if (m_ItemData.item_type1 == (int)TableType.Furniture)
        {
            m_InputField.gameObject.SetActive(true);
            if (m_TextBuyObj != null)
                m_TextBuyObj.SetActive(true);
            if (m_BtnAdd != null)
                m_BtnAdd.SetActive(true);
            if (m_BtnSub != null)
                m_BtnSub.SetActive(true);
        }
        else if (m_ItemData.item_type1 == (int)TableType.Action)
        {
            m_InputField.gameObject.SetActive(false);
            if (m_TextBuyObj != null)
                m_TextBuyObj.SetActive(false);
            if (m_BtnAdd != null)
                m_BtnAdd.SetActive(false);
            if (m_BtnSub != null)
                m_BtnSub.SetActive(false);
        }

        m_BuyNum = 1;
        m_InputField.text = m_BuyNum.ToString();

        m_NameText.text = m_ItemData.item_name;
        if (m_MallData != null)
        {
            m_PriceText.text = string.Format("{0}", m_MallData.price * m_BuyNum);
            AdjustBuyUI();
        }

        MallServerData _data = GetCurItemServerData(m_ItemData.item_id);
        if (_data != null)
        {
            m_HaveNumText.text = string.Format("(已拥有{0})", _data.has_num);
        }
        RTManager.GetInstance().LoadRTModel(m_ItemData.item_id);
    }
    private void SetBuyItemPrice()
    {
        m_PriceText.text = string.Format("{0}", m_MallData.price * m_BuyNum);
        m_InputField.text = m_BuyNum.ToString();
        AdjustBuyUI();
    }

    private void OnBuyItem()
    {
        if (m_ItemData.item_type1 == (int)TableType.Furniture)
        {
            CloseUIForm();
            OpenUIForm(FormConst.SHOPBUYTIPS_UIFORM);
            MallServerData m_MallServerData = GetCurItemServerData(m_ItemData.item_id);
            object[] args = new object[] { m_MallServerData, m_ItemData, m_BuyNum };
            SendMessage("ShopFurntureBuy", "FurntureBuy", args);
        }
        else if (m_ItemData.item_type1 == (int)TableType.Action)
        {
            CloseUIForm();
            OpenUIForm(FormConst.SHOPBUYTIPS_UIFORM);
            string[] args = new string[] { m_ItemData.item_id.ToString(), m_ItemData.item_type1.ToString(), m_ItemData.item_name, (m_MallData.price).ToString() };
            SendMessage("ShopActionBuy", "ActionBuy", args);
        }
    }

    public void RequestBuy()
    {
        StartCoroutine(RequestData());
    }
    IEnumerator RequestData()
    {
        HttpRequest httpRequest = new HttpRequest();
        BuyData buyData = new BuyData(m_ItemData.item_id, m_BuyNum);
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
                ToastManager.Instance.ShowNewToast(string.Format("恭喜获得{0}*{1}", m_ItemData.item_name, m_BuyNum), 5f);
                int[] args = new int[] { m_ItemData.item_id, m_BuyNum };
                MessageManager.GetInstance().RequestGasValue();
                Debug.Log("购买c: " + m_BuyNum);
                RoomFurnitureCtrl.Instance().BuyFurnAddCountFun(m_ItemData.item_id, m_BuyNum);
                SendMessage("ShopBuySuccess", "BuySucess", args);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
        else
        {
            ToastManager.Instance.ShowNewToast(string.Format("购买{0}失败", m_ItemData.item_name), 5f);
        }
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public MallServerData GetCurItemServerData(int itemId)
    {
        Dictionary<int, MallServerData> mallDic = MessageManager.GetInstance().GetMallData();
        MallServerData data = null;
        for (int i = 0; i < mallDic.Count; i++)
        {
            if (mallDic[i].item_id == itemId)
            {
                data = mallDic[i];
                break;
            }
        }
        return data;
    }

    public void AdjustBuyUI()
    {
        m_PriceText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(m_PriceText);
        float height = m_ImgBuyBg.GetComponent<RectTransform>().sizeDelta.y;
        float widthBtn = m_BtnBuy.transform.GetComponent<RectTransform>().sizeDelta.x;
        m_ImgBuyBg.GetComponent<RectTransform>().sizeDelta = new Vector2(131f + width + widthBtn / 2, height);
        m_BtnBuy.GetComponent<RectTransform>().anchoredPosition = new Vector3(m_InitAnchoredPosition.x + width, m_InitAnchoredPosition.y, 0);
    }
}
