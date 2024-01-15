using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TreasureDiggingTicketExchangeConfirm : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public GameObject m_ItemNormal;
    public GameObject m_ItemMax;
    public RectTransform m_GridLayoutGroup;
    public Button m_BtnExchange;
    public Text m_BuyNumText;
    public Text m_TextExchange;

    private List<CollectioListRecData> m_CollectionListData = new List<CollectioListRecData>();
    private int m_BuyNum;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnCancel", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnExchange", p =>
        {
            SetExchangeBtnState(2);
            OnClickExchange();
        });

        ReceiveMessage("TicketExchangeConfirm", p =>
         {
             object[] parm = p.Values as object[];
             if (parm == null || parm.Length < 2)
                 return;
             m_CollectionListData = parm[0] as List<CollectioListRecData>;
             m_BuyNum = (int)parm[1];
             SetCollectionInfo();
             SetBuyNum(m_BuyNum);
         });

        ReceiveMessage("SetIconEnd", p =>
        {
            object[] parm = p.Values as object[];
            if (parm == null || parm.Length < 2)
                return;
            int collectionId = (int)parm[0];
            Sprite sprite = parm[1] as Sprite;
            if (sprite != null)
            {
                TreasureModel.Instance.m_CollectionSpriteDic[collectionId] = sprite;
            }
        });
    }

    private void OnClickExchange()
    {
        MessageManager.GetInstance().RequestExchangeTickets(m_BuyNum, () =>
        {
            SetExchangeBtnState(1);
            CloseUIForm();
            OpenUIForm(FormConst.TREASUREEXCHANGESCCESIPS);
            SendMessage("TreasureExchangeTicket", "Success", null);
        });
    }

    public void SetCollectionInfo()
    {
        int count = m_CollectionListData.Count;
        if (count <= 1)
        {
            if (m_ItemMax)
            {
                m_ItemMax.SetActive(true);
                m_VerticalScroll.m_CellGameObject = m_ItemMax;
            }
            if (m_ItemNormal != null)
            {
                m_ItemNormal.SetActive(false);
            }
            m_GridLayoutGroup.GetComponent<GridLayoutGroup>().cellSize = new Vector2(500f, 195f);
        }
        else
        {
            if (m_ItemNormal)
            {
                m_ItemNormal.SetActive(true);
                m_VerticalScroll.m_CellGameObject = m_ItemNormal;
            }
            if (m_ItemMax != null)
            {
                m_ItemMax.SetActive(false);
            }
            m_GridLayoutGroup.GetComponent<GridLayoutGroup>().cellSize = new Vector2(360f, 140f);
        }
        m_VerticalScroll.Init(InitCollectionInfoCallBack);
        m_VerticalScroll.ShowList(count);
        m_VerticalScroll.ResetScrollRect();
    }

    public void InitCollectionInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        CollectionItem m_CollectIconItem = cell.transform.GetComponent<CollectionItem>();
        if (m_CollectIconItem != null)
        {
            CollectioListRecData data = m_CollectionListData[index - 1];
            if (data == null)
                return;

            m_CollectIconItem.SetCollectionData(data);
            m_CollectIconItem.SetName(data.product_name, data.collection_quanity, m_BuyNum);
            m_CollectIconItem.SetHaveNum(data.num);
            m_CollectIconItem.SetNoneState(false);
            m_CollectIconItem.SetNumState(false);
            m_CollectIconItem.SetNamePosition(new Vector2(62.8f, 0f));

            if (!TreasureModel.Instance.m_CollectionSpriteDic.ContainsKey(data.icollection_id))
            {
                m_CollectIconItem.SetIcon(data.collection_icon);
            }
            else
            {
                Sprite sprite = TreasureModel.Instance.m_CollectionSpriteDic[data.icollection_id];
                if (sprite != null)
                {
                    m_CollectIconItem.SetIcon(sprite);
                }
            }

            m_CollectIconItem.SetIcon(data.collection_icon);
            m_CollectIconItem.SetNoneState(false);
        }

        if (index == m_CollectionListData.Count)
        {
            if (m_GridLayoutGroup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_GridLayoutGroup);
            }
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        SetExchangeBtnState(1);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }

    public void SetBuyNum(int num)
    {
        if (m_BuyNumText != null)
        {
            m_BuyNumText.text = string.Format("兑换{0}张挖宝券", num);
        }
    }

    public void SetExchangeBtnState(int state)
    {
        switch (state)
        {
            case 1:
                Sprite sprite1 = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "button-queding_enable");
                m_BtnExchange.GetComponent<Image>().sprite = sprite1;
                m_BtnExchange.interactable = true;
                m_TextExchange.text = "确定";
                break;
            case 2:
                Sprite sprite2 = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "button-queding_disable");
                m_BtnExchange.GetComponent<Image>().sprite = sprite2;
                m_BtnExchange.interactable = false;
                m_TextExchange.text = "兑换中...";
                break;
        }
    }
}
