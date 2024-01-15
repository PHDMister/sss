using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TreasureDiggingTicketExchange : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public GameObject m_ItemNormal;
    public GameObject m_ItemMax;
    public RectTransform m_GridLayoutGroup;
    public Button m_BtnExchange;
    public Button m_BtnCancel;
    public Button m_BtnSub10;
    public Button m_BtnSub1;
    public Button m_BtnAdd1;
    public Button m_BtnAdd10;
    public Text m_BuyNumText;
    private Transform m_ParentTrans;

    private List<CollectioListRecData> m_CollectionListData = new List<CollectioListRecData>();
    private int m_BuyNum = 1;
    private const int Max_Buy_Num = 10;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        m_ParentTrans = UnityHelper.FindTheChildNode(gameObject, "Parent");
        if (m_ParentTrans != null)
        {
            m_ParentTrans.gameObject.SetActive(false);
        }

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
            OnClickExchange();
        });

        RigisterButtonObjectEvent("SubBtn10", p =>
         {
             OnClickSub10();
         });

        RigisterButtonObjectEvent("SubBtn1", p =>
        {
            OnClickSub1();
        });

        RigisterButtonObjectEvent("AddBtn1", p =>
        {
            OnClickAdd1();
        });

        RigisterButtonObjectEvent("AddBtn10", p =>
        {
            OnClickAdd10();
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

        if (m_BtnSub10 != null)
        {
            m_BtnSub10.gameObject.SetActive(false);
        }
        if (m_BtnAdd10 != null)
        {
            m_BtnAdd10.gameObject.SetActive(false);
        }
    }

    private void OnClickExchange()
    {
        if (!bCanExchange())
        {
            m_BuyNum = 1;
            SetBuyNum(m_BuyNum);
            SetBtnState();
            return;
        }
        CloseUIForm();
        OpenUIForm(FormConst.TREASUREDIGGINGTICKETEXCHANGECONFIRM);

        KeyValuesUpdate kvs = new KeyValuesUpdate("", new object[] { m_CollectionListData, m_BuyNum });
        MessageCenter.SendMessage("TicketExchangeConfirm", kvs);
    }

    private void OnClickSub10()
    {
        if (!bCanExchange())
        {
            m_BuyNum = 1;
            SetBuyNum(m_BuyNum);
            return;
        }

        if (m_BuyNum - Max_Buy_Num > 0)
        {
            m_BuyNum -= Max_Buy_Num;
        }
        SetBuyNum(m_BuyNum);
    }

    private void OnClickSub1()
    {
        if (!bCanExchange())
        {
            m_BuyNum = 1;
            SetBuyNum(m_BuyNum);
            SetBtnState();
            return;
        }

        if (m_BuyNum - 1 > 0)
        {
            m_BuyNum -= 1;
        }
        SetBuyNum(m_BuyNum);
        SetBtnState();
    }

    private void OnClickAdd1()
    {
        if (!bCanExchange())
        {
            m_BuyNum = 1;
            SetBuyNum(m_BuyNum);
            SetBtnState();
            return;
        }

        int canBuyNum = GetCanBuyNum();
        if (canBuyNum > 0)
        {
            if (m_BuyNum < canBuyNum)
            {
                m_BuyNum++;
            }
            else
            {
                m_BuyNum = canBuyNum;
            }

            if (m_BuyNum >= Max_Buy_Num)
            {
                m_BuyNum = Max_Buy_Num;
            }
        }
        SetBuyNum(m_BuyNum);
        SetBtnState();
    }

    private void OnClickAdd10()
    {
        if (!bCanExchange())
        {
            m_BuyNum = 1;
            SetBuyNum(m_BuyNum);
            return;
        }

        int canBuyNum = GetCanBuyNum();
        if (canBuyNum > 0)
        {
            if (m_BuyNum < canBuyNum)
            {
                m_BuyNum++;
            }
            else
            {
                m_BuyNum = canBuyNum;
            }

            if (m_BuyNum >= Max_Buy_Num)
            {
                m_BuyNum = Max_Buy_Num;
            }
        }
        SetBuyNum(m_BuyNum);
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
            m_CollectIconItem.SetName(data.product_name, data.collection_quanity * m_BuyNum);
            m_CollectIconItem.SetHaveNum(data.num);
            m_CollectIconItem.SetNumState(true);
            m_CollectIconItem.SetNamePosition(new Vector2(62.8f, 26.2f));

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
            m_CollectIconItem.SetNoneState(data.num < data.collection_quanity);
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
        if (m_ParentTrans != null)
        {
            m_ParentTrans.gameObject.SetActive(false);
        }

        m_BuyNum = 1;
        MessageManager.GetInstance().RequestCollectionCount((p) =>
        {
            if (m_ParentTrans != null)
            {
                m_ParentTrans.gameObject.SetActive(true);
            }
            m_CollectionListData = p;
            SetCollectionInfo();
            SetBuyNum(m_BuyNum);
            SetBtnState();
        });
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        MessageCenter.SendMessage("OpenChatUI", KeyValuesUpdate.Empty);
    }

    public void SetBtnState()
    {
        if (bCanExchange())
        {
            if (m_BtnExchange != null)
            {
                m_BtnExchange.interactable = true;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnExchangeEnable");
                m_BtnExchange.GetComponent<Image>().sprite = sprite;
            }
        }
        else
        {
            if (m_BtnExchange != null)
            {
                m_BtnExchange.interactable = false;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnExchangeDisable");
                m_BtnExchange.GetComponent<Image>().sprite = sprite;
            }

            if (m_BtnAdd1 != null)
            {
                m_BtnAdd1.interactable = false;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnDisable");
                m_BtnAdd1.GetComponent<Image>().sprite = sprite;
            }

            if (m_BtnSub1 != null)
            {
                m_BtnSub1.interactable = false;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnDisable");
                m_BtnSub1.GetComponent<Image>().sprite = sprite;
            }
            return;
        }

        int canBuyNum = GetCanBuyNum();
        if (canBuyNum > 1 && (m_BuyNum >= canBuyNum || m_BuyNum >= Max_Buy_Num))
        {
            if (m_BtnAdd1 != null)
            {
                m_BtnAdd1.interactable = false;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnDisable");
                m_BtnAdd1.GetComponent<Image>().sprite = sprite;
            }

            if (m_BtnSub1 != null)
            {
                m_BtnSub1.interactable = true;
                Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnEnable");
                m_BtnSub1.GetComponent<Image>().sprite = sprite;
            }
        }
        else
        {
            if (m_BuyNum <= 1)
            {
                if (m_BtnSub1 != null)
                {
                    m_BtnSub1.interactable = false;
                    Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnDisable");
                    m_BtnSub1.GetComponent<Image>().sprite = sprite;
                }

                if (m_BtnAdd1 != null)
                {
                    m_BtnAdd1.interactable = true;
                    Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnEnable");
                    m_BtnAdd1.GetComponent<Image>().sprite = sprite;
                }
            }
            else
            {
                if (m_BtnSub1 != null)
                {
                    m_BtnSub1.interactable = true;
                    Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnEnable");
                    m_BtnSub1.GetComponent<Image>().sprite = sprite;
                }

                if (m_BtnAdd1 != null)
                {
                    m_BtnAdd1.interactable = true;
                    Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("TreasureDigging", "btnEnable");
                    m_BtnAdd1.GetComponent<Image>().sprite = sprite;
                }
            }
        }
    }

    public void SetBuyNum(int num)
    {
        if (m_BuyNumText != null)
        {
            m_BuyNumText.text = num.ToString();
        }
    }

    public bool bCanExchange()
    {
        if (m_CollectionListData == null)
            return false;
        for (int i = 0; i < m_CollectionListData.Count; i++)
        {
            if (m_CollectionListData[i].num < m_CollectionListData[i].collection_quanity)
            {
                return false;
            }
        }
        return true;
    }

    public int GetCanBuyNum()
    {
        int buyNum = 10;//最大限制购买10个
        for (int i = 0; i < m_CollectionListData.Count; i++)
        {
            int canBuyNum = m_CollectionListData[i].num / m_CollectionListData[i].collection_quanity;
            if (canBuyNum < buyNum)
            {
                buyNum = canBuyNum;
            }
        }
        return buyNum;
    }

}
