using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.SceneManagement;

public class EditModePanel : BaseUIForm
{
    private CircularScrollView.UICircularScrollView GameScroll_GameRecord;
    private int CameraPosIndex = 0;
    /*    private Vector3[] arrCameraPos = { new Vector3(0.25f, 1.46f, -2.83f), new Vector3(3f, 1.5f, 0.5f), new Vector3(3.6f, 1.46f, -0.12f), new Vector3(4.51f, 1.46f, 1.03f), new Vector3(0.43f, 1.46f, -5.09f), new Vector3(-4.02f, 1.46f, -0.7f), new Vector3(-4.3f, 1.46f, -3.49f) };
        private Vector3[] arrCameraRoationY = { new Vector3(0f, 180f, 0f), new Vector3(-16.5f, -140f, 0f), new Vector3(0f, -270f, 0f), new Vector3(0f, -19.5f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, -27.14f, 0f), new Vector3(0f, -90f, 0f) };
        private float[] arrCameraFovAxis = { 105f, 50f, 93f, 68, 68f, 71f, 72.6f };*/

    private int[][] arrCameraA = { new int[] { 3, 4, 5, 9, 12 }, new int[] { 1 }, new int[] { 7, 16, 17, 20 }, new int[] { 2, 13, 14, 15, 18, 19 }, new int[] { 6, 11 }, new int[] { 0, 8, 10, 21, 22 }, new int[] { 23, 24, 25, 26 } };

    private int[][] arrCameraB = { new int[] { 1, 3, 6, 7, 8, 12, 13, 14, 15 }, new int[] { 9, 10, 16, 17 }, new int[] { 4, 18 }, new int[] { 0, 2, 5 } };

    private List<BubbleTipItem> arrBubbleTipItemPoolList = new List<BubbleTipItem>();
    private List<GameObject> arrBubbleObjPoolList = new List<GameObject>();
    public List<GameObject> arrBubbleObjList = new List<GameObject>();



    // 气泡的根节点
    private GameObject BubbleObjRoot;
    //提示面板
    private GameObject TipPanel;
    public Text tipPanelText;
    private Text GasValue_Text;
    public Image GasValueBG_Image;
    private bool IsClickSaveBtn = false;

    private int CamerCount;

    public GameObject CameraBtnListPanel;
    public GameObject CameraListBg;
    public GameObject CamerListDiBtn;
    private int sceneID = 0;

    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Fixed;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
        CameraPosIndex = 0;
        GameScroll_GameRecord = transform.Find("CameraBtnListPanel/BG_Image/Scroll View").GetComponent<CircularScrollView.UICircularScrollView>();
        GameScroll_GameRecord.Init(NormalCallBack);


        if (BubbleObjRoot == null)
        {
            BubbleObjRoot = transform.Find("BubbleTipRoot").gameObject;
        }


        if (TipPanel == null)
        {
            TipPanel = transform.Find("Tip_Panel").gameObject;
        }
        if (GasValue_Text == null)
        {
            GasValue_Text = transform.Find("GasValue_Panel/GasValue_Text").GetComponent<Text>();
        }
        arrBubbleObjPoolList.Clear();
        arrBubbleObjPoolList.Add(BubbleObjRoot.transform.GetChild(0).gameObject);
    }


    public void Start()
    {


        RigisterButtonObjectEvent("left_Button", p =>
        {
            Debug.Log("左边的按钮");
            ChangeToLeftCameraPosFun();
        });
        RigisterButtonObjectEvent("right_Button", p =>
        {
            Debug.Log("右边的按钮");
            ChangeToRigthCameraPosFun();
        });
        RigisterButtonObjectEvent("Close_Btn", p =>
        {
            Debug.Log("退出的按钮");
            //退出编辑模式
            IsClickSaveBtn = false;
            OutEditBtnListen();
        });
        RigisterButtonObjectEvent("Save_Btn", p =>
        {
            Debug.Log("保存数据按钮");
            SaveEditBtnListen();
            //保存数据按钮

        });
        RigisterButtonObjectEvent("TipSave_Btn", p =>
        {
            //提示面板上的保存按钮
            TipSaveBtnListenFun();
        });
        RigisterButtonObjectEvent("TipCancel_Btn", p =>
        {
            //提示面板上的取消按钮
            if (IsClickSaveBtn)
            {
                TipPanel.SetActive(false);
                IsClickSaveBtn = false;
            }
            else
            {
                TipCancelBtnListenFun();
            }
        });
        //一键装修按钮
        RigisterButtonObjectEvent("Finish_Btn", p =>
        {
            RoomFurnitureCtrl.Instance().OneKeyFinishAllFrameFun();
            SendMessage("RefshScrlllPanel", "data", "refsh");
            ToastManager.Instance.ShowNewToast("一键装修完成", 5f);
            Debug.Log("一键装修");
        });
        // 一键收起按钮
        RigisterButtonObjectEvent("PackUp_Btn", p =>
        {
            bool isHave = false;

            isHave = RoomFurnitureCtrl.Instance().RoomIsHaveFuritureFun();


            Debug.Log("输出一下是否存在： " + isHave);
            if (isHave)
            {
                UIManager.GetInstance().ShowUIForms(FormConst.PICKUPTIPPANEL);
            }
            else
            {
                ToastManager.Instance.ShowNewToast(string.Format("未摆放家具，无需收起"), 5f);
            }

        });
        // 场景镜头列表面板的底部按钮
        RigisterButtonObjectEvent("CameraListDI_Btn", p =>
        {
            CamerListDiBtn.SetActive(false);
            CameraListBg.SetActive(true);
            GameScroll_GameRecord.UpdateList();
        });
        // 场景镜头列表面板的顶部按钮
        RigisterButtonObjectEvent("CameraListClose_Btn", p =>
        {
            CameraListBg.SetActive(false);
            CamerListDiBtn.SetActive(true);
        });
        // 一键收起的提示面板的确认点击事件
        ReceiveMessage("PickTipAffirm",
              p =>
              {
                  //点击藏品图UI
                  string strAffirm = p.Values as string;
                  if (strAffirm == "affirm")
                  {
                      Debug.Log("确认更改");
                      PickUpAllFurnitureFun();
                      SendMessage("RefshScrlllPanel", "data", "refsh");
                  }
              }
         );
        ReceiveMessage("CamerUIMessage",
             p =>
             {
                 int cameraId = (int)p.Values;
                 CameraPosIndex = cameraId;
                 SetCameraPosFun(cameraId);
             }
        );

        // gameScrollPanle面板被关闭
        ReceiveMessage("GameScrollPanelBtn",
              p =>
              {
                  string strAffirm = p.Values as string;
                  if (strAffirm == "Close")
                  {
                      //刷新镜头面板
                      ReshCameraListPanelFun();
                      GameScroll_GameRecord.UpdateList();
                  }
              }
         );
        ReceiveMessage("UpdataGasValue",
         p =>
         {
             if (gameObject.activeSelf == false)
             {
                 return;
             }
             GasValue_Text.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
             AdjustGasUI();
         }
         );
        ReshOpenPanelFun();
        //刷新镜头面板
        ReshCameraListPanelFun();
    }
    public override void Display()
    {
        base.Display();
        CamerCount = RoomFurnitureCtrl.Instance().CameraValueDataDic.Keys.Count;
        GameScroll_GameRecord.ShowList(CamerCount);
        if (BubbleObjRoot == null)
        {
            BubbleObjRoot = transform.Find("BubbleTipRoot").gameObject;
        }


        if (TipPanel == null)
        {
            TipPanel = transform.Find("Tip_Panel").gameObject;
        }
        if (GasValue_Text == null)
        {
            GasValue_Text = transform.Find("GasValue_Panel/GasValue_Text").GetComponent<Text>();
        }

        if (TipPanel != null)
        {
            Debug.Log("打开了面板");
            ReshOpenPanelFun();
        }
        Debug.Log("这里的内容是多少");

        MessageCenter.SendMessage("CloseChatUI", KeyValuesUpdate.Empty);
    }

    private void NormalCallBack(GameObject cell, int index)
    {
        if (cell != null)
        {
            Debug.Log("cell存在： " + index);
        }
        else
        {
            Debug.Log("不存在： ");
        }
        cell.GetComponent<CameraUIItem>().SetCameraUIFun(index);
    }


    /// <summary>
    /// 保存按钮
    /// </summary>
    public void SaveEditBtnListen()
    {
        if (RoomFurnitureCtrl.Instance().furnitureTemporaryAfterPlaceDicData.Count > 0 || RoomFurnitureCtrl.Instance().allTemporaryPlacePictureData.Count > 0)
        {
            TipPanel.SetActive(true);
            SetTipShowTextFun(2);
            IsClickSaveBtn = true;
        }
        else
        {
            ToastManager.Instance.ShowNewToast("暂无需要保存的家具", 5f);
        }
    }
    public void SetTipShowTextFun(int typeInex)
    {

        if (typeInex == 1)
        {
            tipPanelText.text = "空间内容已有变更，是否保存";
        }
        else if (typeInex == 2)
        {
            tipPanelText.text = "是否保存当前装修内容";
        }

    }
    /// <summary>
    /// 打开面板是的刷新
    /// </summary>
    void ReshOpenPanelFun()
    {
        CameraPosIndex = 1;
        TipPanel.SetActive(false);
        //if (GameStartController.cameraBrain != null)
        //{
        //    GameStartController.cameraBrain.enabled = false;
        //}
        if (CameraManager.Instance().GetBrain() != null)
        {
            CameraManager.Instance().GetBrain().enabled = false;
        }
        GameObject playerObj = CharacterManager.Instance().GetPlayerObj();
        if (playerObj != null)
        {
            playerObj.transform.position = RoomFurnitureCtrl.Instance().startPos;
            playerObj.transform.localEulerAngles = RoomFurnitureCtrl.Instance().startEulerAngles;
        }
        //GameStartController.PlayerObj.transform.position = GameStartController.startPos;
        //GameStartController.PlayerObj.transform.localEulerAngles = GameStartController.startEulerAngles;
        SetCameraPosFun(CameraPosIndex);
        GasValue_Text.text = ManageMentClass.DataManagerClass.gas_Amount.ToString();
        AdjustGasUI();
        ChangeCameraSetBubbleFun();
    }

    void ReshCameraListPanelFun()
    {
        CameraBtnListPanel.SetActive(true);
        CameraListBg.SetActive(true);
        CamerListDiBtn.SetActive(false);
    }

    /// <summary>
    /// 退出按钮
    /// </summary>
    public void OutEditBtnListen()
    {
        if (RoomFurnitureCtrl.Instance().furnitureTemporaryAfterPlaceDicData.Count > 0 || RoomFurnitureCtrl.Instance().allTemporaryPlacePictureData.Count > 0)
        {
            TipPanel.SetActive(true);
            SetTipShowTextFun(1);
        }
        else
        {
            OutEditPanelFun();
        }
    }
    /// <summary>
    ///  提示面板上的保存按钮
    /// </summary>
    public void TipSaveBtnListenFun()
    {

        RoomFurnitureCtrl.Instance().ChangeSaveAllFurnitureFun();
        RoomFurnitureCtrl.Instance().ChangeSaveAllPictureFun();


        OutEditPanelFun();
        RoomFurnitureCtrl.Instance().CloseAllFurnitureLightFun();
    }
    /// <summary>
    ///  提示面板上的取消按钮
    /// </summary>
    public void TipCancelBtnListenFun()
    {

        RoomFurnitureCtrl.Instance().ChageCancelAllFurnitureFun();
        RoomFurnitureCtrl.Instance().ChageCancelAllPictureFun();


        OutEditPanelFun();
        RoomFurnitureCtrl.Instance().CloseAllFurnitureLightFun();
    }
    /// <summary>
    /// 退出编辑
    /// </summary>
    public void OutEditPanelFun()
    {
        //直接退出
        //关闭当前面板
        IsClickSaveBtn = false;
        CloseUIForm();
        //打开主界面（人物归位 到初始位置，可以操控）
        UIManager.GetInstance().CloseUIForms(FormConst.GAMESCROLLPANEL);
        UIManager.GetInstance().ShowUIForms(FormConst.RIGHT_BAR_UIFORM);
        InterfaceHelper.SetJoyStickState(true);
        RoomFurnitureCtrl.Instance().CloseAllFurnitureLightFun();
        //  ManageMentClass.DataManagerClass.arrPostFurnitureTypeData.Clear();
        //if (GameStartController.cameraBrain != null)
        //{
        //    GameStartController.cameraBrain.enabled = true;
        //}
        if (CameraManager.Instance().GetBrain() != null)
        {
            CameraManager.Instance().GetBrain().enabled = true;
        }
    }
    /// <summary>
    /// 一键收起所有家具
    /// </summary>
    void PickUpAllFurnitureFun()
    {
        RoomFurnitureCtrl.Instance().OneKeyPickUpAllFrameFun();
    }
    /// <summary>
    /// 相机向左改变位置
    /// </summary>
    public void ChangeToLeftCameraPosFun()
    {
        CameraPosIndex++;
        if (CameraPosIndex > CamerCount)
        {
            CameraPosIndex = 1;
        }
        Debug.Log("输出一下镜头的下表内容的值： " + CameraPosIndex);
        SetCameraPosFun(CameraPosIndex);
        RoomFurnitureCtrl.Instance().CloseAllFurnitureLightFun();
        UIManager.GetInstance().CloseUIForms(FormConst.GAMESCROLLPANEL);
        ReshCameraListPanelFun();
    }
    //相机向右改变位置
    public void ChangeToRigthCameraPosFun()
    {
        CameraPosIndex--;
        if (CameraPosIndex < 1)
        {
            CameraPosIndex = CamerCount;
        }
        SetCameraPosFun(CameraPosIndex);
        RoomFurnitureCtrl.Instance().CloseAllFurnitureLightFun();
        UIManager.GetInstance().CloseUIForms(FormConst.GAMESCROLLPANEL);
        ReshCameraListPanelFun();
    }
    public void SetCameraPosFun(int index)
    {
        Debug.Log("输出一下镜头的下标： " + index);

        if (RoomFurnitureCtrl.Instance().CameraValueDataDic.ContainsKey(index))
        {
            if (SceneManager.GetActiveScene().name == "gerenkongjian01")
            {
                ManageMentClass.DataManagerClass.CamerIdaData = arrCameraA[index - 1];
            }
            else if (SceneManager.GetActiveScene().name == "bedroom01")
            {
                ManageMentClass.DataManagerClass.CamerIdaData = arrCameraB[index - 1];
            }
            Camera.main.transform.position = RoomFurnitureCtrl.Instance().CameraValueDataDic[index].CameraPos;
            Camera.main.transform.localEulerAngles = RoomFurnitureCtrl.Instance().CameraValueDataDic[index].CameraRoation;
            Camera.main.GetComponent<Camera>().fieldOfView = RoomFurnitureCtrl.Instance().CameraValueDataDic[index].CameraFovAxis;
        }
    }

    public void ChangeCameraSetBubbleFun()
    {
        if (!CameraBtnListPanel.activeSelf)
        {
            ReshCameraListPanelFun();
        }
        //初始化关闭所有的气泡
        StartSetBubbleFun();
        //设置气泡
        SetBubblePos();
    }
    /// <summary>
    /// 初始化的时候全部关闭
    /// </summary>
    public void StartSetBubbleFun()
    {
        arrBubbleObjList.Clear();
        for (int i = 0; i < arrBubbleObjPoolList.Count; i++)
        {
            arrBubbleObjPoolList[i].gameObject.SetActive(false);
            if (arrBubbleObjPoolList[i].gameObject.GetComponent<BubbleTipItem>())
            {
                RemoveItemActionFun(arrBubbleObjPoolList[i].gameObject.GetComponent<BubbleTipItem>());
            }
        }
    }
    /// <summary>
    ///  设置气泡
    /// </summary>
    /// <param name="arrBubblePos"></param>
    public void SetBubblePos()
    {
        for (int i = 0; i < RoomFurnitureCtrl.Instance().GetPosNodeFun().transform.childCount; i++)
        {
            var arrData = RoomFurnitureCtrl.Instance().GetPosNodeFun().transform.GetChild(i).gameObject.name.Split('_');
            FurnitureType type = ManageMentClass.MethodCollectionClass.GetFurnitureTypeFun(arrData[0]);
            var bubbleItem = GetBubbleTipItemFun();
            arrBubbleObjList.Add(bubbleItem);
            BubbleTipItem bubb = bubbleItem.GetComponent<BubbleTipItem>();
            RefisterItemActionFun(bubb);
            bubb.initData(RoomFurnitureCtrl.Instance().allFurnitureRootData[i + RoomFurnitureCtrl.Instance().GetStartIndex()], type);
        }
    }
    /// <summary>
    /// 气泡的对象池
    /// </summary>
    /// <returns></returns>
    private GameObject GetBubbleTipItemFun()
    {
        for (int i = 0; i < arrBubbleObjPoolList.Count; i++)
        {
            if (!arrBubbleObjPoolList[i].activeSelf)
            {
                arrBubbleObjPoolList[i].gameObject.SetActive(true);
                return arrBubbleObjPoolList[i];
            }
        }
        var newBubble = Instantiate(arrBubbleObjPoolList[0]);
        newBubble.transform.SetParent(BubbleObjRoot.transform);
        arrBubbleObjPoolList.Add(newBubble);
        return newBubble;
    }
    /// <summary>
    /// 登记点击事件
    /// </summary>
    /// <param name="item"></param>
    public void RefisterItemActionFun(BubbleTipItem item)
    {
        item.OnClickAction += OnClickBubbleTipFun;
    }
    /// <summary>
    /// 移除点击事件
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemActionFun(BubbleTipItem item)
    {
        item.OnClickAction -= OnClickBubbleTipFun;
    }
    /// <summary>
    /// 点击场景中的按钮会响应的事件
    /// </summary>
    private void OnClickBubbleTipFun(FurnitureRootData furnitureRootData)
    {
        //在这里点击了，然后设置一下具体的事件方法
        Debug.Log("输出一下这里面的信息： " + furnitureRootData.PlaceID + "   " + furnitureRootData.furnitureType);
        OpenUIForm(FormConst.GAMESCROLLPANEL);
        CameraBtnListPanel.SetActive(false);
        SendMessage("Furniture", "data", furnitureRootData);
    }
    public bool IsVisible()
    {
        return transform.gameObject.activeSelf;
    }
    public void SetVisible(bool isOpen)
    {
        transform.gameObject.SetActive(isOpen);
    }
    public void AdjustGasUI()
    {
        GasValue_Text.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        float width = InterfaceHelper.CalcTextWidth(GasValue_Text);
        float height = GasValueBG_Image.transform.GetComponent<RectTransform>().sizeDelta.y;
        GasValueBG_Image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + width, height);
    }

    public override void Hiding()
    {
        base.Hiding();
        MessageCenter.SendMessage("OpenChatUI", KeyValuesUpdate.Empty);
    }
}
