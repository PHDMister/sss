using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class RightBarUIForm : BaseUIForm
{
    public enum TableType
    {
        Furniture = 1,
        Action = 2,
    }

    public Text m_TextGas;
    public Image m_ImageAct;
    private item m_SelectActItemData;

    // 个人信息的UI内容

    //空间名
    public Text SpaceNum_Text;
    //空间坐标
    public Text SpacePos_Text;
    // 建筑名字
    public Text BuildName_Text;
    // 头像
    public RawImage Head_RawImage;
    //用户名
    public Text PersonName_Text;

    //装修的按钮物体
    public GameObject ModifyBtnObj;

    //换形象按钮
    public GameObject BtnSet;


    public GameObject ReturnBtn;

    public RectTransform m_DragCameraRectTrans;
    public Image m_ImgGasBg;

    public Button OtherSpaceBtn;

    private float timer = 0f;
    private float clickInterval = 1f;//换形象商城点击间隔
    private bool bShopClicked = false;
    private bool bSetClicked = false;
    private bool bShopCanClick = true;
    private bool bSetCanClick = true;
    private float lastSendActionTime = 0;

    private int requestNum = 0;
    private bool bFirstGuide = false;
    public void Awake()
    {
        if (!ManageMentClass.DataManagerClass.is_Owner)
        {
            ModifyBtnObj.SetActive(false);
        }
        else
        {
            ModifyBtnObj.SetActive(true);
        }

        if (ReturnBtn != null)
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ReturnBtn.SetActive(false);
            }
            else
            {
                ReturnBtn.SetActive(true);
            }

        }

        //  BtnSet.SetActive(false);

        //窗体的性质
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;

        m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();

        //注册进入主城的事件
        RigisterButtonObjectEvent("BtnShop", p =>
            {
                bShopClicked = true;
                timer = 0;
                if (!bShopCanClick)
                {
                    return;
                }
                if (CharacterManager.Instance().GetPlayerObj() == null)
                {
                    return;
                }
                OpenUIForm(FormConst.SHOPNEWUIFORM);
                //MessageManager.GetInstance().RequestShopOutFitList((p) =>
                //{
                //    OpenUIForm(FormConst.SHOPNEWUIFORM);
                //    SendMessage("ReceiveShopOutFitData", "Success", p);
                //});
            });
        RigisterButtonObjectEvent("BtnModify", p =>
            {
                if (Singleton<ParlorController>.Instance.OwnerHasVisitor())
                {
                    ToastManager.Instance.ShowNewToast("有访客拜访无法装修，稍后试试哦～", 3);
                    return;
                }
                //检测并取消角色与建筑的交互
                Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative(1);

                MessageManager.GetInstance().RequestShopData((int)TableType.Furniture, () =>
                {
                });
                OpenUIForm(FormConst.EDITMODEPANEL);
                InterfaceHelper.SetJoyStickState(false);
                CloseUIForm();
            });
        RigisterButtonObjectEvent("BtnSet", p =>
            {
                bSetClicked = true;
                timer = 0;
                if (!bSetCanClick)
                {
                    return;
                }
                if (CharacterManager.Instance().GetPlayerObj() == null)
                {
                    return;
                }
                OpenUIForm(FormConst.PERSONALDATAPANEL);
                SendMessage("OpenPersonDataPanelRefreshUI", "Success", (ulong)0);
            });
        RigisterButtonObjectEvent("BtnChangeAct", p =>
            {
                GameObject actButton = UnityHelper.FindTheChildNode(this.gameObject, "BtnAction").gameObject;
                if (actButton != null)
                {
                    actButton.gameObject.SetActive(false);
                }
                OpenUIForm(FormConst.SETACTION_UIFORM);
            });
        RigisterButtonObjectEvent("BtnAction", p =>
        {
            if (m_SelectActItemData != null)
            {
                int itemId = m_SelectActItemData.item_id;
                animation m_Animation = ManageMentClass.DataManagerClass.GetAnimationTableFun(itemId);
                if (m_Animation != null)
                {
                    string actClipName = m_Animation.animation_model;
                    PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
                    if (playerItem != null)
                    {
                        if (playerItem.IsHaveState(actClipName))
                        {
                            //检测当前是否是交互状态
                            if (ManageMentClass.DataManagerClass.SceneID == 1)
                                Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative();

                            //播放表情动作
                            OnPlaySelectAct(actClipName);
                            //在挖宝房间内发送消息同步
                            if (ManageMentClass.DataManagerClass.SceneID == 4
                                || ManageMentClass.DataManagerClass.SceneID == 1
                                && Time.realtimeSinceStartup - lastSendActionTime > 3)
                            {
                                lastSendActionTime = Time.realtimeSinceStartup;
                                DoActionReq doAction = new DoActionReq();
                                doAction.Index = WebSocketAgent.Ins.NetView.GetCode;
                                doAction.UserId = ManageMentClass.DataManagerClass.userId;
                                doAction.Action = (uint)itemId;
                                WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.DoActionReq, doAction);
                            }
                        }
                        else
                        {
                            ToastManager.Instance.ShowNewToast(string.Format("暂未拥有{0}动作，敬请期待！", m_Animation.animation_name), 5f);
                        }
                    }
                }
            }
        });
        RigisterButtonObjectEvent("Return_Btn", p =>
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("没有退路了，继续探索元宇宙世界吧！", 5f);
            }
            else
            {
                //退出游戏
                OnClickReturnBtnFun();
            }
        });
        ReceiveMessage("SetActionItemSuccess", p =>
             {
                 m_SelectActItemData = p.Values as item;
                 if (m_SelectActItemData != null)
                 {
                     SetItemIcon(m_SelectActItemData.item_icon);
                 }
             }
        );
        //ReceiveMessage("ShopBuySuccess",
        //    p =>
        //    {
        //        Debug.Log("输出一下成功B");
        //        if (gameObject.activeSelf == false)
        //        {
        //            return;
        //        }
        //        int[] args = p.Values as int[];
        //        int m_BuyItemId = args[0];
        //        int m_BuyNum = args[1];

        //    }
        //);
        ReceiveMessage("UpdataGasValue",
          p =>
          {
              if (gameObject.activeSelf == false)
              {
                  return;
              }
              m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
              AdjustGasUI();
          }
      );

        ReceiveMessage("SetActionClose",
            p =>
         {
             GameObject actButton = UnityHelper.FindTheChildNode(this.gameObject, "BtnAction").gameObject;
             if (actButton != null)
             {
                 actButton.gameObject.SetActive(true);
             }
         }
         );

        ReceiveMessage("RefreshPlayerName", p =>
        {
            Debug.Log("输出一下具体的名字值： " + p.Values.ToString());

            PersonName_Text.text = TextTools.setCutAddString(p.Values.ToString(), 7, "...");
        });


        OtherSpaceBtn.onClick.AddListener(OnClickOtherSpaceFun);

        //DisableFunc();
    }

    public override void Display()
    {
        base.Display();

        bShopCanClick = true;
        bSetCanClick = true;

        m_TextGas.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
        SetCurActIcon();
        SetPersonDataFun();
        if (ManageMentClass.DataManagerClass.SpaceNum > 1)
        {
            OtherSpaceBtn.transform.gameObject.SetActive(true);
        }
        else
        {
            OtherSpaceBtn.transform.gameObject.SetActive(false);
        }

        if (ManageMentClass.DataManagerClass.isOtherSpace)
        {
            ManageMentClass.DataManagerClass.isOtherSpace = false;
            SceneLoadManager.Instance().InitSceneEditorFun();
        }

        AdjustGasUI();


        requestNum = 0;
        StartCoroutine(WaitOpenChat());
    }

    private IEnumerator WaitOpenChat()
    {
        CheckShowGuide();
        while (requestNum < 1)
        {
            yield return null;
        }

        if (!bFirstGuide)
        {
            openChatUI();
        }
    }


    /// <summary>
    /// 新手引导Tips
    /// </summary>
    private void CheckShowGuide()
    {
        CheckBeginLaunchReq req = new CheckBeginLaunchReq
        {
            UserId = ManageMentClass.DataManagerClass.userId
        };

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.CheckBeginLaunchReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            CheckBeginLaunchResp checkBeginLaunchResp = CheckBeginLaunchResp.Parser.ParseFrom(bytes);
            if (checkBeginLaunchResp.StatusCode == 0)
            {
                if ((int)checkBeginLaunchResp.IsComplete == 0)
                {
                    bFirstGuide = true;
                    OpenUIForm(FormConst.PARLORGUIDEPANEL);
                }
            }
            requestNum++;
        });
    }

    private void openChatUI()
    {
        if (UIManager.GetInstance().IsOpend(FormConst.UICHAT))
        {
            return;
        }
        if (ManageMentClass.DataManagerClass.SceneID != (int)LoadSceneType.parlorScene)
        {
            return;
        }
        OpenUIForm(FormConst.UICHAT);
        var param = new UIChatParam()
        {
            ChatTypes = new List<ChatType> { ChatType.Room },
            Controller = Singleton<ParlorController>.Instance,
        };
        MessageCenter.SendMessage("OnOpenUIChat", "param", param);
    }

    void OnClickOtherSpaceFun()
    {
        //点击其他空间
        MessageManager.GetInstance().RequestOtherSpaceData(() =>
        {
            InterfaceHelper.SetJoyStickState(false);
            OpenUIForm(FormConst.INTOOTHERSPACEPANEL);
        });
    }
    void OnClickReturnBtnFun()
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
    /// <summary>
    /// 设置个人信息
    /// </summary>
    public void SetPersonDataFun()
    {
        SpaceNum_Text.text = ManageMentClass.DataManagerClass.spce_Name;
        SpacePos_Text.text = ManageMentClass.DataManagerClass.build_XYZ;
        BuildName_Text.text = ManageMentClass.DataManagerClass.build_Name;
        // 头像
        if (ManageMentClass.DataManagerClass.Head_Texture != null)
        {
            Head_RawImage.texture = ManageMentClass.DataManagerClass.Head_Texture.texture;
        }
        PersonName_Text.text = TextTools.setCutAddString(ManageMentClass.DataManagerClass.login_Name.ToString(), 7, "...");
    }

    public void OnPlaySelectAct(string triggerName)
    {
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator(triggerName);
        }
        //GameStartController.PlayerItem.SetAnimator(triggerName);
    }

    public void SetCurActIcon()
    {
        int m_SelectActId = PlayerPrefs.GetInt("CurUseActId");
        if (m_SelectActId <= 0)
        {
            m_SelectActId = 2003;//默认问候
            PlayerPrefs.SetInt("CurUseActId", m_SelectActId);
        }
        m_SelectActItemData = ManageMentClass.DataManagerClass.GetItemTableFun(m_SelectActId);
        if (m_SelectActItemData != null)
        {
            string spriteName = m_SelectActItemData.item_icon;
            SetItemIcon(spriteName);
        }
    }
    public void SetItemIcon(string spriteName)
    {
        /*  var atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/Icon");
          Sprite sprite = atlas.GetSprite(spriteName);*/
        m_ImageAct.sprite = ManageMentClass.ResourceControllerClass.ResLoadIconByPathNameFun(spriteName);
    }



    /// <summary>
    /// 第一期禁用换形象换动作
    /// </summary>
    public void DisableFunc()
    {
        GameObject m_BtnSet = UnityHelper.FindTheChildNode(this.gameObject, "BtnSet").gameObject;
        if (m_BtnSet != null)
            m_BtnSet.gameObject.SetActive(false);
        GameObject m_BtnChangeAct = UnityHelper.FindTheChildNode(this.gameObject, "BtnChangeAct").gameObject;
        if (m_BtnChangeAct != null)
            m_BtnChangeAct.gameObject.SetActive(false);
        GameObject m_BtnAct = UnityHelper.FindTheChildNode(this.gameObject, "BtnAction").gameObject;
        if (m_BtnAct != null)
            m_BtnAct.gameObject.SetActive(false);
    }

    public RectTransform GetCameraRectTransform()
    {
        return m_DragCameraRectTrans;
    }

    public void AdjustGasUI()
    {
        m_TextGas.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(m_TextGas);
        float height = m_ImgGasBg.GetComponent<RectTransform>().sizeDelta.y;
        m_ImgGasBg.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + width, height);
    }

    public override void Redisplay()
    {
        base.Redisplay();
        bShopCanClick = true;
        bSetCanClick = true;
    }

    private void Update()
    {
        if (bShopClicked)
        {
            if (timer < clickInterval)
            {
                timer += Time.deltaTime;
                bSetCanClick = false;
            }

            if (timer >= clickInterval)
            {
                timer = 0;
                bSetCanClick = true;
                bShopCanClick = true;
                bShopClicked = false;
            }
        }

        if (bSetClicked)
        {
            if (timer < clickInterval)
            {
                timer += Time.deltaTime;
                bShopCanClick = false;
            }

            if (timer >= clickInterval)
            {
                timer = 0;
                bShopCanClick = true;
                bSetCanClick = true;
                bSetClicked = false;
            }
        }
    }
}
