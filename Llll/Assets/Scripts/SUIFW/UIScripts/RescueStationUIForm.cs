using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RescueStationUIForm : BaseUIForm
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
    public GameObject m_BtnExchange;

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

        RigisterButtonObjectEvent("BtnAdopt", p =>
            {
                InterfaceHelper.SetJoyStickState(false);
                OpenUIForm(FormConst.AIDSTATIONSADOPT);
            });

        RigisterButtonObjectEvent("BtnModify", p =>
        {
            OpenUIForm(FormConst.SceneEditorPanel);
            CloseUIForm();
        });
        RigisterButtonObjectEvent("BtnExchange", p =>
        {
            OpenUIForm(FormConst.DogExchangCardShowTips);
            if (exchangeProofData != null)
            {
                SendMessage("OpenDogExchangeCardShow", "Success", exchangeProofData);
            }
        });

        RigisterButtonObjectEvent("BtnReturn", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("没有退路了，继续探索元宇宙世界吧！", 5f);
            }
            else
            {
                OnClickReturn();
            }
           
        });

        RigisterButtonObjectEvent("BtnHelp", p =>
         {
             OpenUIForm(FormConst.SHELTERPETHELPTIPSUIFORM);
         });

        RigisterButtonObjectEvent("BtnLive", p =>
        {
#if !UNITY_EDITOR
            try
            {
                SetTools.showVideoPlayer();
                Debug.Log("WebGL Platform Play");
            }
            catch (System.Exception e)
            {
                  Debug.Log("Exception： " + e);
            }
#else
            OpenUIForm(FormConst.AIDSTATIONSLIVE);
#endif
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
                m_BtnAdopt.SetActive(num < 2);
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
            if (PetSpanManager.Instance().bInAidStations())
            {
                m_BtnExchange.SetActive(exchangeProofData.total > 0);
            }
            else
            {
                if (ManageMentClass.DataManagerClass.is_Owner)
                {
                    m_BtnExchange.SetActive(exchangeProofData.total > 0);
                }
                else
                {
                    m_BtnExchange.SetActive(false);
                }
            }
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
            if (PetSpanManager.Instance().bInAidStations())
            {
                m_BtnExchange.SetActive(p.total > 0);
            }
            else
            {
                if (ManageMentClass.DataManagerClass.is_Owner)
                {
                    m_BtnExchange.SetActive(p.total > 0);
                }
                else
                {
                    m_BtnExchange.SetActive(false);
                }
            }
        });

        //Gas值
        MessageManager.GetInstance().RequestGasValue(() =>
        {
            m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
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

        MessageManager.GetInstance().RequestEnableAdopt(2, () =>
        {
            //m_TextLove.text = ManageMentClass.DataManagerClass.petAdoptCostItemNum.ToString();
        });

        MessageManager.GetInstance().RequestPetList(() =>
        {
            int num = PetSpanManager.Instance().GetPetNum();
            if (num <= 0)
            {
                OpenUIForm(FormConst.AIDSTATIONSADOPT);
            }

            if (m_BtnAdopt != null)
            {
                m_BtnAdopt.SetActive(num < 2);
            }
            PetSpanManager.Instance().UpdatePet();
        });

        if (m_BtnShop != null)
        {
            m_BtnShop.SetActive(true);
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

    void OnClickReturn()
    {
        //点击了退出
        try
        {
            SetTools.SetPortraitModeFun();
            //显示top栏
            SetTools.CloseGameFun();
        }
        catch (System.Exception e)
        {
            Debug.Log("这里的内容： " + e);
        }
    }
}
