using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFW;
using UnityEngine.SceneManagement;

public class IntoOtherSpacePanel : BaseUIForm
{
    //其他空间
    public CircularScrollView.UICircularScrollView OtherSpace_ScrollView;
    public Text TopTipsNameText;
    private void Awake()
    {
        CurrentUIType.UIForms_Type = UIFormType.Fixed;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate; //半透明，不能穿透
        OtherSpace_ScrollView.Init(NormalCallBack);
        TopTipsNameText.text = TextTools.setCutAddString(ManageMentClass.DataManagerClass.login_Name, 7, "...");
        Debug.Log("ManageMentClass.DataManagerClass.login_Name：  " + ManageMentClass.DataManagerClass.login_Name);


        RigisterButtonObjectEvent("CloseBtn", p =>
        {
            InterfaceHelper.SetJoyStickState(true);
            CloseUIForm();
            //提示面板上的保存按钮
            //TipSaveBtnListenFun();
        });
    }

    public override void Display()
    {
        base.Display();
        OtherSpace_ScrollView.ShowList(ManageMentClass.DataManagerClass.OtherSpaceDataList.Count);
    }
    //回调
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
        if (index <= ManageMentClass.DataManagerClass.OtherSpaceDataList.Count)
        {
            OtherSpaceUIItem otherSpaceUIItem = cell.GetComponent<OtherSpaceUIItem>();
            otherSpaceUIItem.SetPrefabFun(ManageMentClass.DataManagerClass.OtherSpaceDataList[index - 1]);
            SetItemCallBackFun(otherSpaceUIItem);
        }
    }
    private void SetItemCallBackFun(OtherSpaceUIItem otherSpaceUIItem)
    {
        otherSpaceUIItem.OnClickAction -= ItemCallBackFun;
        otherSpaceUIItem.OnClickAction += ItemCallBackFun;
    }
    //点击了之后
    private void ItemCallBackFun(OtherSpaceData otherSpaceData)
    {
        if (otherSpaceData.StatusID == 1)
        {
            InterfaceHelper.SetJoyStickState(true);
            ManageMentClass.DataManagerClass.isOtherSpace = true;
            ManageMentClass.DataManagerClass.bClickOtherSpace = true;
            ManageMentClass.DataManagerClass.landId = otherSpaceData.LandID;
            //跳转
            RoomFurnitureCtrl.Instance().AllDataInitializeFun();
            //  UIManager.GetInstance().CloseAllShowPanel();
            UIManager.GetInstance().CloseAllShowPanel();

            //拉取初始化数据
            MessageManager.GetInstance().StartPersonSpaceDataFun(() =>
            {
                //数据拉取成功，可以向客厅跳转
                Singleton<ParlorController>.Instance.CheckAndCancelSelfIntercative(1);
                UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
                SendMessage("ToParlorSceneLoading", "SceneTransfer", 1);
                CloseUIForm();
            }, () =>
            {
                //数据拉取失败


            });
        }
        else
        {
            if (ManageMentClass.DataManagerClass.WebInto)
            {
                ToastManager.Instance.ShowNewToast("请前往App端购买", 5f);
            }
            else
            {
                string uRl = string.Format("aiera://www.aiera.com/page/nfttransfer/detail?id={0}&size_id={1}&order_id={2}", otherSpaceData.ProductID, otherSpaceData.NftProductSizeID, otherSpaceData.OrderID);
                Debug.Log("uRl: " + uRl);
                //点击了退出
                try
                {
                    SetTools.SetPortraitModeFun();
                    //显示top栏
                    SetTools.CloseGameFun();
                    SetTools.OpenWebUrl(uRl);

                }
                catch (System.Exception e)
                {
                    Debug.Log("这里的内容： " + e);
                }

            }
            
        }
    }
}
