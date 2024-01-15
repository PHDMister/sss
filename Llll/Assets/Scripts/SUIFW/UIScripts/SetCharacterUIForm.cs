using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


public class SetCharacterUIForm : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    public Button m_Button_Used;
    public Button m_Button_Replace;
    public Button m_button_NoGas;
    public Button m_Button_Exchange;
    public Button m_Button_Buy;
    public GameObject m_Obj_Exchange;
    public GameObject m_Obj_Price;
    public Text m_Text_Price;
    public Text m_Text_Exchange;
    public Image m_Img_Character;



    public enum TableType
    {
        None = 0,
        Furniture = 1,
        Action = 2,
    }

    public enum SaleMode
    {
        None = 0,
        Money = 1,
        Exchange = 2,
    }
    private Dictionary<int, item> m_CharacterItemConfigData = new Dictionary<int, item>();
    private Dictionary<int, ItemData> m_CharacterItem = new Dictionary<int, ItemData>();
    private Dictionary<int, CharacterData> m_CharacterServerData = new Dictionary<int, CharacterData>();

    private item m_SelectCharacterItem;
    private CharacterData m_SelectCharacterData;
    private CharacterData m_CurUseCharacter = null;
    private int m_LastCharacterId = 0;

    void Awake()
    {
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.ImPenetrable;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;

        //注册按钮事件
        RigisterButtonObjectEvent("BtnClose", p =>
         {
             CloseUIForm();
         });

        RigisterButtonObjectEvent("Bg", p =>
        {
            CloseUIForm();
        });

        RigisterButtonObjectEvent("BtnBuy", p =>
        {
            OnClickBuy();
        });

        RigisterButtonObjectEvent("BtnReplace", p =>
         {
             OnClickReplace();
         }
        );
        RigisterButtonObjectEvent("BtnExchange", p =>
        {
            OnClickExchange();
        });


        ReceiveMessage("SetCharacterItem",
            p =>
            {
                object[] args = p.Values as object[];
                m_SelectCharacterData = args[0] as CharacterData;
                m_SelectCharacterItem = args[1] as item;

                SetCharacterImg(m_SelectCharacterItem.item_icon);

                if (m_CurUseCharacter != null && m_SelectCharacterData != null)
                {
                    mall m_SelectMallItemConfig = ManageMentClass.DataManagerClass.GetMallTableFun(m_SelectCharacterData.item_id);
                    int m_SaleMode = m_SelectMallItemConfig == null ? 2 : m_SelectMallItemConfig.sale_mode;
                    int m_CurGas = ManageMentClass.DataManagerClass.gas_Amount;
                    m_Button_Used.gameObject.SetActive(m_SelectCharacterData.is_selected > 0);
                    m_Button_Replace.gameObject.SetActive(m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num > 0);
                    m_Button_Exchange.gameObject.SetActive(m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num <= 0);

                    //替换
                    if (m_SelectCharacterData.is_selected > 0 || m_SelectCharacterData.has_num > 0)
                    {
                        m_button_NoGas.gameObject.SetActive(false);
                        m_Obj_Exchange.gameObject.SetActive(false);
                        m_Obj_Price.gameObject.SetActive(false);
                        m_Button_Buy.gameObject.SetActive(false);
                    }

                    if (m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num <= 0)
                    {
                        //购买
                        if (m_SaleMode == (int)SaleMode.Money)
                        {
                            m_button_NoGas.gameObject.SetActive(m_SelectMallItemConfig != null && m_CurGas < m_SelectMallItemConfig.price);
                            m_Obj_Exchange.gameObject.SetActive(false);
                            m_Obj_Price.gameObject.SetActive(true);
                            m_Button_Buy.gameObject.SetActive(m_SelectMallItemConfig != null && m_CurGas >= m_SelectMallItemConfig.price);
                            m_Text_Price.text = string.Format("{0}", m_SelectMallItemConfig == null ? 0 : m_SelectMallItemConfig.price);
                        }
                        else //兑换
                        {
                            m_button_NoGas.gameObject.SetActive(false);
                            m_Obj_Price.gameObject.SetActive(false);
                            m_Button_Buy.gameObject.SetActive(false);
                            m_Obj_Exchange.gameObject.SetActive(true);
                            m_Button_Exchange.gameObject.SetActive(true);
                            m_Text_Exchange.text = string.Format("⌈{0}⌋兑换", m_SelectMallItemConfig == null ? "" : m_SelectMallItemConfig.collection_name);
                        }
                    }

                }


                if (m_SelectCharacterItem != null)
                {
                    SetCharacterItemSelectState(m_SelectCharacterItem.item_id);
                }
            }
        );

        Clear();
    }

    private void Start()
    {

    }

    private void ShowCharacterServerData()
    {
        GetCharacterItemConfigData();
        int count = m_CharacterServerData.Count;
        m_VerticalScroll.Init(InitCharacterItemInfoCallBack);
        m_VerticalScroll.ShowList(count);
        if (m_CurUseCharacter != null)
        {
            m_SelectCharacterItem = ManageMentClass.DataManagerClass.GetItemTableFun(m_CurUseCharacter.item_id);
            if (m_SelectCharacterItem != null)
            {
                SetCharacterImg(m_SelectCharacterItem.item_icon);
            }

            m_Button_Used.gameObject.SetActive(true);
            m_Button_Replace.gameObject.SetActive(false);
            m_button_NoGas.gameObject.SetActive(false);
            m_Button_Exchange.gameObject.SetActive(false);
            m_Button_Buy.gameObject.SetActive(false);
            m_Obj_Exchange.gameObject.SetActive(false);
            m_Obj_Price.gameObject.SetActive(false);
        }
    }

    private void UpdateCharacterServerData()
    {
        GetCharacterItemConfigData();
        int count = m_CharacterServerData.Count;
        m_VerticalScroll.Init(InitCharacterItemInfoCallBack);
        m_VerticalScroll.ShowList(count);

        mall m_SelectMallItemConfig = ManageMentClass.DataManagerClass.GetMallTableFun(m_SelectCharacterData.item_id);
        int m_SaleMode = m_SelectMallItemConfig == null ? 2 : m_SelectMallItemConfig.sale_mode;
        int m_CurGas = ManageMentClass.DataManagerClass.gas_Amount;

        m_Button_Used.gameObject.SetActive(m_SelectCharacterData.is_selected > 0);
        m_Button_Replace.gameObject.SetActive(m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num > 0);
        m_Button_Exchange.gameObject.SetActive(m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num <= 0);

        //替换
        if (m_SelectCharacterData.is_selected > 0 || m_SelectCharacterData.has_num > 0)
        {
            m_button_NoGas.gameObject.SetActive(false);
            m_Obj_Exchange.gameObject.SetActive(false);
            m_Obj_Price.gameObject.SetActive(false);
            m_Button_Buy.gameObject.SetActive(false);
        }

        if (m_SelectCharacterData.is_selected <= 0 && m_SelectCharacterData.has_num <= 0)
        {
            //购买
            if (m_SaleMode == (int)SaleMode.Money)
            {
                m_button_NoGas.gameObject.SetActive(m_SelectMallItemConfig != null && m_CurGas < m_SelectMallItemConfig.price);
                m_Obj_Exchange.gameObject.SetActive(false);
                m_Obj_Price.gameObject.SetActive(true);
                m_Button_Buy.gameObject.SetActive(m_SelectMallItemConfig != null && m_CurGas >= m_SelectMallItemConfig.price);
                m_Text_Price.text = string.Format("{0}", m_SelectMallItemConfig == null ? 0 : m_SelectMallItemConfig.price);
            }
            else //兑换
            {
                m_button_NoGas.gameObject.SetActive(false);
                m_Obj_Price.gameObject.SetActive(false);
                m_Button_Buy.gameObject.SetActive(false);
                m_Obj_Exchange.gameObject.SetActive(true);
                m_Button_Exchange.gameObject.SetActive(true);
                m_Text_Exchange.text = string.Format("⌈{0}⌋兑换", m_SelectMallItemConfig == null ? "" : m_SelectMallItemConfig.collection_name);
            }
        }
    }
    private void InitCharacterItemInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }

        ItemData itemData = cell.transform.GetComponent<ItemData>();
        if (itemData != null)
        {
            CharacterData character;
            m_CharacterServerData.TryGetValue(index - 1, out character);
            item item;
            m_CharacterItemConfigData.TryGetValue(character.item_id, out item);
            itemData.SetItemIcon(item.item_icon);
            itemData.SetItemName(item.item_name);
            itemData.SetItemData(item);
            itemData.SetCharacterServerData(character);
            itemData.SetItemTabType(item.item_type1);
            itemData.SetItemId(item.item_id);
            itemData.SetItemSelectState(m_SelectCharacterItem != null && m_SelectCharacterItem.item_id == item.item_id);
            itemData.SetItemUsingState(character.is_selected == 1);
            itemData.SetItemLockState(false);
            m_CharacterItem[item.item_id] = itemData;
            if (character.is_selected == 1)
            {
                m_CurUseCharacter = character;
                m_LastCharacterId = character.item_id;
                CharacterManager.Instance().SetLastCharacterId(character.item_id);
            }
        }
    }

    private Dictionary<int, item> GetItemConfigData()
    {
        itemContainer m_itemContainer = BinaryDataMgr.Instance.LoadTableById<itemContainer>("itemContainer");
        if (m_itemContainer != null)
        {
            return m_itemContainer.dataDic;
        }
        return new Dictionary<int, item>();
    }

    private void GetCharacterItemConfigData()
    {
        Dictionary<int, item> itemConfigData = GetItemConfigData();
        foreach (var item in m_CharacterServerData)
        {
            item itemConfig;
            itemConfigData.TryGetValue(item.Value.item_id, out itemConfig);
            m_CharacterItemConfigData[item.Value.item_id] = itemConfig;
        }
    }

    private void Clear()
    {
        m_CharacterServerData.Clear();
        m_CharacterItemConfigData.Clear();
        m_CharacterItem.Clear();
        m_SelectCharacterItem = null;
    }

    public void SetCharacterItemSelectState(int id)
    {
        foreach (var item in m_CharacterItem)
        {
            if (item.Value.GetItemId() == id)
            {
                item.Value.SetItemSelectState(true);
            }
            else
            {
                item.Value.SetItemSelectState(false);
            }
        }
    }


    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        m_CharacterServerData = MessageManager.GetInstance().GetCharacterData();
        SortCharacterData();
        ShowCharacterServerData();
        SetCharacterItemSelectState(m_SelectCharacterItem.item_id);
    }

    public override void Redisplay()
    {
        base.Redisplay();
        InterfaceHelper.SetJoyStickState(false);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
    }

    public void SortCharacterData()
    {
        SortHelper.Sort(m_CharacterServerData.Values.ToArray(), (a, b) =>
         {
             int m_CharacterA = a.item_id;
             int m_CharacterB = b.item_id;
             return m_CharacterA > m_CharacterB;
         }
        );
    }

    public void SetCharacterImg(string imgName)
    {
      /*  SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
        Sprite sprite = atlas.GetSprite(imgName);*/

        m_Img_Character.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(imgName);
        m_Img_Character.SetNativeSize();
    }

    public void OnClickBuy()
    {

    }

    public void OnClickReplace()
    {
        ResetCharacterReplace();
    }

    public void OnClickExchange()
    {
        ResetCharacterExchange();
    }

    public void ResetCharacterReplace()
    {
        StartCoroutine(RequestCharacterReplaceData());
    }

    IEnumerator RequestCharacterReplaceData()
    {
        HttpRequest httpRequest = new HttpRequest();
        CharacterReplaceData characterReplaceData = new CharacterReplaceData(m_SelectCharacterData.item_id);
        string data = JsonConvert.SerializeObject(characterReplaceData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ReplaceCharacter, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                MessageManager.GetInstance().RequestCharacterList(() =>
                  {
                      m_CharacterServerData = MessageManager.GetInstance().GetCharacterData();
                      SortCharacterData();
                      ShowCharacterServerData();
                  });
                //ChangeModel(m_SelectCharacterData.item_id);
             //   CharacterManager.Instance().ChangeRole(m_SelectCharacterData.item_id);
                CloseUIForm();
                PlayerPrefs.SetInt("CurCharacterId", m_SelectCharacterData.item_id);
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
    }

    public void ResetCharacterExchange()
    {
        StartCoroutine(RequestCharacterExchangeData());
    }

    IEnumerator RequestCharacterExchangeData()
    {
        HttpRequest httpRequest = new HttpRequest();
        CharacterExchangeData characterExchangeData = new CharacterExchangeData(m_SelectCharacterData.item_id);
        string data = JsonConvert.SerializeObject(characterExchangeData);
        StartCoroutine(httpRequest.PostRequest(ManageMentClass.ServerInterFaceClass.ExchangeCharacter, ManageMentClass.DataManagerClass.tokenValue_Game, data));
        while (!httpRequest.isComPlete)
        {
            yield return null;
        }
        if (httpRequest.isSucc)
        {
            JObject jo = JObject.Parse(httpRequest.value);
            if ((int)jo["code"] == 0)
            {
                MessageManager.GetInstance().RequestCharacterList(() =>
                {
                    m_CharacterServerData = MessageManager.GetInstance().GetCharacterData();
                    SortCharacterData();
                    for(int i = 0; i < m_CharacterServerData.Count; i++)
                    {
                        if(m_CharacterServerData[i].is_selected == 1)
                        {
                            m_CurUseCharacter = m_CharacterServerData[i];
                        }
                        if(m_CharacterServerData[i].item_id == m_SelectCharacterData.item_id)
                        {
                            m_SelectCharacterData = m_CharacterServerData[i];
                        }
                    }
                    m_SelectCharacterItem = ManageMentClass.DataManagerClass.GetItemTableFun(m_SelectCharacterData.item_id);
                    SetCharacterImg(m_SelectCharacterItem.item_icon);
                    //ChangeModel(m_SelectCharacterData.item_id);
                  //  CharacterManager.Instance().ChangeRole(m_SelectCharacterData.item_id);
                    UpdateCharacterServerData();
                });
            }
            else
            {
                ToastManager.Instance.ShowNewErrorToast(httpRequest.value);
            }
        }
    }

    public void ChangeModel(int curCharacterId)
    {
        if (m_LastCharacterId == curCharacterId)
            return;
        hotman m_LastHotManConfig = ManageMentClass.DataManagerClass.GetHotmanTableFun(m_LastCharacterId);
        hotman m_CurHotManConfig = ManageMentClass.DataManagerClass.GetHotmanTableFun(curCharacterId);
        string m_CurHotManName = "";
        string m_LastHotManName = "";
        if (m_CurHotManConfig != null && m_LastHotManConfig != null)
        {
            m_CurHotManName = m_CurHotManConfig.hotman_mode;
            m_LastHotManName = m_LastHotManConfig.hotman_mode;
            GameObject root = GameObject.Find("Character");
            GameObject m_CurHotManObj = root.transform.Find(m_CurHotManName).gameObject;
            GameObject m_LastHotManObj = root.transform.Find(m_LastHotManName).gameObject;

            if (m_CurHotManObj != null)
            {
                m_CurHotManObj.gameObject.SetActive(true);
                if (m_LastHotManObj != null)
                {
                    Vector3 curPos = new Vector3(m_LastHotManObj.transform.position.x, m_LastHotManObj.transform.position.y, m_LastHotManObj.transform.position.z);
                    Vector3 curLocalEulerAngles = new Vector3(m_LastHotManObj.transform.localEulerAngles.x, m_LastHotManObj.transform.localEulerAngles.y, m_LastHotManObj.transform.localEulerAngles.z);
                  //  GameStartController.InitSelectCharacter(m_CurHotManObj, curPos, curLocalEulerAngles);
                }
            }
            if (m_LastHotManObj != null)
            {
                m_LastHotManObj.gameObject.SetActive(false);
            }
            List<Material> materials = InterfaceHelper.FindChildMaterials(m_CurHotManObj.transform);
          //  GameStartController.ResetMaterialProperty(materials, 0);
        }
    }
}
