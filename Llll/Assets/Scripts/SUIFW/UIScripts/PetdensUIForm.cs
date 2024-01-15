using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PetdensUIForm : BaseUIForm
{
    public enum TableType
    {
        Furniture = 1,
        Action = 2,
    }

    public Text m_TextGas;
    public Image m_ImgGasBg;
    public Image m_BtnGas;

    public Text m_TextLove;
    public Image m_ImgLoveBg;
    public Image m_BtnLove;
    public Image m_ImgLoveIcon;
    public GameObject m_BtnAdopt;
    public GameObject m_BtnShop;
    public GameObject m_BtnModify;
    public GameObject m_BtnExchange;

    public GameObject m_BtnClean;

    public int oldValue = 0;
    //领养凭证数据
    ExchangeProofRecData exchangeProofData;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        m_BtnExchange.SetActive(false);

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnShop", p =>
            {
                InterfaceHelper.SetJoyStickState(false);
                ReceivePetShopServerFun();
            });
        RigisterButtonObjectEvent("BtnModify", p =>
            {
                OpenUIForm(FormConst.SceneEditorPanel);
                CloseUIForm();
            });

        RigisterButtonObjectEvent("BtnAdopt", p =>
            {
                InterfaceHelper.SetJoyStickState(false);
                OpenUIForm(FormConst.PETADOPTTIPS_UIFORM);
            });

        RigisterButtonObjectEvent("BtnExchange", p =>
         {
             OpenUIForm(FormConst.DogExchangCardShowTips);
             if (exchangeProofData != null)
             {
                 SendMessage("OpenDogExchangeCardShow", "Success", exchangeProofData);
             }
         });

        RigisterButtonObjectEvent("BtnClean", p =>
         {
             if (PetSpanManager.Instance().GetFecesNum() == 0)
             {
                 ToastManager.Instance.ShowPetToast("空间内非常干净，无需清理", 2f);
                 return;
             }
             PetSpanManager.Instance().OneKeyCleanFeces();
         });

        Transform btnAidStations = UnityHelper.FindTheChildNode(this.gameObject, "BtnAidStations");
        if (btnAidStations != null)
        {
            btnAidStations.gameObject.SetActive(false);//屏蔽救助站
        }
        RigisterButtonObjectEvent("BtnReturn", p =>
        {
            PetSpanManager.Instance().Clear();
            TransferEffectManager.Instance().bTransfer = true;
            UIManager.GetInstance().CloseUIForms(FormConst.HUD);
        });

        RigisterButtonObjectEvent("BtnHelp", p =>
         {
             OpenUIForm(FormConst.PETHELPTIPS_UIFORM);
         });

        ReceiveMessage("UpdataGasValue",
              p =>
              {
                  m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
                  //AdjustGasUI();
              }
         );

        ReceiveMessage("FeedSuccess",
              p =>
              {
                  MessageManager.GetInstance().RequestPetList(() =>
                  {
                      PetSpanManager.Instance().UpdatePet();
                  });

                  MessageManager.GetInstance().RequestGasValue();
              }
         );

        ReceiveMessage("AdoptSuccess", p =>
        {
            int num = PetSpanManager.Instance().GetPetNum();
            if (m_BtnAdopt != null)
            {
                m_BtnAdopt.SetActive(num < 5 && ManageMentClass.DataManagerClass.is_Owner);
            }
        });

        ReceiveMessage("FeedPetSuccess", UpdatePetStatus);

        ReceiveMessage("PetStateAnimationTypeMes", p =>
         {
             object[] args = p.Values as object[];
             int feedType = (int)args[0];
             int petId = (int)args[1];
             PetSpanManager.Instance().PlayPetAni(petId, feedType);
         });

        ReceiveMessage("BeginPetTrainAnimation", p =>
         {
             int petId = (int)p.Values;
             PetSpanManager.Instance().SetBeginTrainPetRigidbody(petId);
             PetSpanManager.Instance().SetTrainData(petId, true);
             PetSpanManager.Instance().PlayPetAni(petId, (int)PetStateAnimationType.Train);
         });

        ReceiveMessage("UpdateLoveCoin", p =>
         {
             m_TextLove.text = InterfaceHelper.GetCoinDisplay(ManageMentClass.DataManagerClass.loveCoin);
         });

        ReceiveMessage("RecExchangeProofNum", p =>
         {
             exchangeProofData = p.Values as ExchangeProofRecData;
             m_BtnExchange.SetActive(exchangeProofData.total > 0 && ManageMentClass.DataManagerClass.is_Owner);
         });


        ReceiveMessage("OpenPetExchangeSuccess", p =>
        {
            if (ManageMentClass.DataManagerClass.is_Owner)
            {
                m_BtnAdopt.SetActive(true);
            }
        });
    }

    /// <summary>
    /// 请求宠物商店数据
    /// </summary>
    public void ReceivePetShopServerFun()
    {
        PetShopTypeData petShopTypeData = new PetShopTypeData(1);
        string Strdata = JsonConvert.SerializeObject(petShopTypeData);
        MessageManager.GetInstance().RequestPetShopReceive(Strdata, (JObject jo) =>
         {
             //已经请求成功了
             var listData = jo["data"][0]["list"];
             ManageMentClass.DataManagerClass.ListPetShopItemId.Clear();
             ManageMentClass.DataManagerClass.DicPetShopItemData.Clear();

             foreach (var itemA in jo["data"])
             {
                 if ((int)itemA["type"] == 0)
                 {
                     Debug.Log("这里A");
                     foreach (var item in itemA["list"])
                     {
                         Debug.Log("这里B");
                         int id = (int)item["id"];
                         string jsonData = item.ToString();
                         PetShopItemData shopItemData = JsonUntity.FromJSON<PetShopItemData>(jsonData);
                         if (!ManageMentClass.DataManagerClass.DicPetShopItemData.ContainsKey(id))
                         {
                             ManageMentClass.DataManagerClass.DicPetShopItemData.Add(id, shopItemData);
                             ManageMentClass.DataManagerClass.ListPetShopItemId.Add(id);
                         }
                         else
                         {
                             ManageMentClass.DataManagerClass.DicPetShopItemData[id] = shopItemData;
                             ManageMentClass.DataManagerClass.ListPetShopItemId.Add(id);
                         }
                     }
                     break;
                 }
             }
             //打开商店面板
             OpenUIForm(FormConst.PETSHOPPANEL);
         });
    }



    public void UpdatePetStatus(KeyValuesUpdate kv)
    {
        int petId = (int)kv.Values;
        PetSpanManager.Instance().UpdatePetStatus(petId);
    }

    public void InitFun()
    {
        //请求领养凭证数量
        MessageManager.GetInstance().RequestExchangeProofNum((p) =>
        {
            exchangeProofData = p;
            m_BtnExchange.SetActive(p.total > 0 && ManageMentClass.DataManagerClass.is_Owner);
        });

        //请求爱心币列表
        MessageManager.GetInstance().RequestLoveCoinList((p) =>
        {
            PetSpanManager.Instance().loveCoinRecData = p;
            if (PetSpanManager.Instance().loveCoinRecData != null)
            {
                ManageMentClass.DataManagerClass.loveCoin = PetSpanManager.Instance().loveCoinRecData.remain_lovecoin;
                m_TextLove.text = InterfaceHelper.GetCoinDisplay(ManageMentClass.DataManagerClass.loveCoin);
                oldValue = ManageMentClass.DataManagerClass.loveCoin;
            }
            PetSpanManager.Instance().UpdatePet();
        });

        //请求粪便列表
        MessageManager.GetInstance().RequestFeces((p) =>
        {
            if (p == null || p.list == null)
                return;
            PetSpanManager.Instance().fecesRecData = p;
            PetSpanManager.Instance().UpdateFeces(p);
        });

        MessageManager.GetInstance().RequestEnableAdopt(1, () =>
        {
            //m_TextLove.text = ManageMentClass.DataManagerClass.petAdoptCostItemNum.ToString();
        });

        MessageManager.GetInstance().RequestPetList(() =>
        {
            int num = PetSpanManager.Instance().GetPetNum();
            if (num <= 0)
            {
                if (ManageMentClass.DataManagerClass.is_Owner)//不是房主不能操作领养
                {
                    OpenUIForm(FormConst.PETADOPTTIPS_UIFORM);
                }
            }

            if (m_BtnAdopt != null)
            {
                m_BtnAdopt.SetActive(num < 5 && ManageMentClass.DataManagerClass.is_Owner);
            }
            PetSpanManager.Instance().UpdatePet();
        });

        m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();

        if (m_BtnShop != null)
        {
            m_BtnShop.SetActive(true);
        }
        if (m_BtnClean != null)
        {
            m_BtnClean.SetActive(ManageMentClass.DataManagerClass.is_Owner);
        }

        if ((LoadSceneType)ManageMentClass.DataManagerClass.SceneID == LoadSceneType.ShelterScene)
        {
            m_BtnModify.SetActive(true);
        }
        else
        {
            try
            {
                Debug.Log("是否是房间的主人：  " + ManageMentClass.DataManagerClass.is_Owner);
                if (ManageMentClass.DataManagerClass.is_Owner)
                {
                    m_BtnModify.SetActive(true);
                }
                else
                {
                    m_BtnModify.SetActive(false);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("是否主人的报错： " + e.Message);
            }
        }
        if (!ManageMentClass.DataManagerClass.is_Owner)
        {
            m_BtnAdopt.SetActive(false);
        }
    }

    public override void Display()
    {
        base.Display();


        //AdjustGasUI();
        //AdjustLoveUI();
    }

    public override void Hiding()
    {
        base.Hiding();
    }

    public void OnPlaySelectAct(string triggerName)
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator(triggerName);
        }
    }

    public void AdjustGasUI()
    {
        m_TextGas.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(m_TextGas);
        float height = m_ImgGasBg.GetComponent<RectTransform>().sizeDelta.y;
        m_ImgGasBg.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + width, height);
        Vector2 textPos = m_TextGas.GetComponent<RectTransform>().anchoredPosition;
        m_BtnGas.GetComponent<RectTransform>().anchoredPosition = new Vector2(textPos.x + 50f, -57f);
    }

    public void AdjustLoveUI()
    {
        m_TextLove.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(m_TextLove);
        float height = m_ImgLoveBg.GetComponent<RectTransform>().sizeDelta.y;
        m_ImgLoveBg.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + width, height);
        Vector2 textPos = m_TextLove.GetComponent<RectTransform>().anchoredPosition;
        m_BtnLove.GetComponent<RectTransform>().anchoredPosition = new Vector2(textPos.x + 50f, -57f);
    }
}
