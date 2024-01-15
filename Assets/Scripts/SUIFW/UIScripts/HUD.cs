using System.Collections;
using System.Collections.Generic;
using Treasure;
using UnityEngine;
using UIFW;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.U2D;

public class HUD : BaseUIForm
{
    public Image m_ImgIcon = null;
    public Text m_TextName = null;
    protected int LevelCache;
    public void Awake()
    {
        //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;  //弹出窗体
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Pentrate;
        //CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        RigisterButtonObjectEvent("BtnPetdens", p =>
        {
            PetSpanManager.Instance().Clear();
            TransferEffectManager.Instance().bTransfer = true;
            UIManager.GetInstance().CloseUIForms(FormConst.HUD);
        });

        ReceiveMessage("TransferEffectEnd", p =>
        {
            //int leveIdx = (int)p.Values;
            //if(leveIdx == 1)
            //{
            //    UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
            //    SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.dogScene);
            //}
            //else
            //{
            //    UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
            //    SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.parlorScene);
            //}

            //当前是要离开客厅 
            if (SceneManager.GetActiveScene().name.ToLower().Contains("gerenkongjian"))
            {
                Singleton<ParlorSyncNetView>.Instance.LeaveRoom();
            }
            //当前是要离开卧室
            if (SceneManager.GetActiveScene().name.ToLower().Contains("bedroom01"))
            {
                RoomFurnitureCtrl.Instance().GoOtherCleanDataFun();
            }
            switch (LevelCache)
            {
                case 1:
                    UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
                    SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.parlorScene);
                    break;
                case 2:
                    LeaveRoomReq req1 = new LeaveRoomReq();
                    req1.UserId = ManageMentClass.DataManagerClass.userId;
                    WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.LeaveRoomReq, req1, (code, dataString) =>
                    {
                        if (code > 0) Debug.LogError("HUD LeaveRoomReq is  error ");
                    });
                    UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
                    SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.dogScene);
                    break;
                case 3:
                case 4:
                    break;
                case 5:
                    LeaveRoomReq req = new LeaveRoomReq();
                    req.UserId = ManageMentClass.DataManagerClass.userId;
                    WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.LeaveRoomReq, req, (code, dataString) =>
                    {
                        if (code > 0) Debug.LogError("HUD LeaveRoomReq is  error ");
                    });
                    UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
                    SendMessage("TransferLoading", "SceneTransfer", (int)LoadSceneType.BedRoom);
                    break;
            }
        });

        ReceiveMessage("TransferScene", p =>
         {
             int level = (int)p.Values;
             LevelCache = level;
             if (level == 1)
             {
                 //m_TextName.text = "个人空间>>";
                 m_TextName.text = "";
                 /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/HUD");
                  Sprite sprite = atlas.GetSprite("icon-kongjian");*/
                 m_ImgIcon.sprite = ManageMentClass.ResourceControllerClass.ResLoadHUDByPathNameFun("icon-kongjian");
             }
             else if (level == 2)
             {
                 //m_TextName.text = "宠物窝>>";
                 m_TextName.text = "";
                 /* SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UIRes/Atlas/HUD");
                   Sprite sprite = atlas.GetSprite("icon-chongwuwo"); */
                 m_ImgIcon.sprite = ManageMentClass.ResourceControllerClass.ResLoadHUDByPathNameFun("icon-chongwuwo"); ;
             }
             else if (level == 5)
             {
                 //m_TextName.text = "卧室>>";
                 m_TextName.text = "";
                 m_ImgIcon.sprite = ManageMentClass.ResourceControllerClass.ResLoadHUDByPathNameFun("icon-bedroom"); ;
             }
         });
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(true);
    }

    public override void Hiding()
    {
        base.Hiding();
    }
}
