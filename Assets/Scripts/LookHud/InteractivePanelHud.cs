using System;
using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InteractivePanelHud : BaseLookPanelHud
{
    public InteractiveCfg CurShowGoName;
    protected float LastClickTime;
    protected Button btn;
    protected Image img;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Transform btnTrans = transform.GetChild(0);
        btn = btnTrans.GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);
        img = btnTrans.GetComponent<Image>();
    }

    private void OnBtnClick()
    {
        if (Time.realtimeSinceStartup - LastClickTime < 5) return;
        LastClickTime = Time.realtimeSinceStartup;

        if (CurShowGoName == null || string.IsNullOrEmpty(CurShowGoName.FurnitureName))
        {
            Debug.Log("当前点击的目标对象为空！！！  ");
            return;
        }

        if (SceneManager.GetActiveScene().name.ToLower().Contains("bedroom"))
        {
            //Debug.Log("11111111   CurShowGoName=" + CurShowGoName);
            InteractiveController inCon = InteractiveController.FindController(CurShowGoName.FurnitureName);
            GameObject go = CharacterManager.Instance().GetPlayerObj();
            PlayerControllerImp imp = go.GetComponent<PlayerControllerImp>();
            if (!imp) imp = go.AddComponent<PlayerControllerImp>();
            imp.IsSelf = true;
            imp.SyncEnable(true);
            imp.playerItem = go.GetComponent<PlayerItem>();
            inCon.DoAction(imp);
        }
        else if (ManageMentClass.DataManagerClass.SceneID == (int)LoadSceneType.parlorScene)
        {
            //发送协议  等待协议生成
            SitdownReq req = new SitdownReq();
            req.UserId = ManageMentClass.DataManagerClass.userId;
            req.Sitdown = CurShowGoName.ToString();
            req.Index = WebSocketAgent.Ins.NetView.GetCode;
            req.IsLeave = false;
            WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.SitdownReq, req, (code, data) =>
            {
                SitdownResp resp = SitdownResp.Parser.ParseFrom(data);
                if (resp.StatusCode > 0)
                {
                    //Debug.LogError("InteractivePanelHud  OnBtnClick   interactive  respose  code :" + resp.StatusCode);
                    ToastManager.Instance.ShowNewToast("该座位已经有人了哦~", 2);
                    return;
                }
                InteractiveController inCon = InteractiveController.FindController(CurShowGoName.FurnitureName);
                ulong selfUserId = ManageMentClass.DataManagerClass.userId;
                if (Singleton<ParlorController>.Instance.TryGetPlayerImp(selfUserId, out var imp))
                {
                    inCon.DoAction(imp);
                }
            });
        }
    }

    public void ChangeSprite(FurnitureType furnitureType)
    {
        string spriteName = GetSprite(furnitureType);
        if (img.sprite.name != spriteName)
        {
            Sprite sprite = ResourcesMgr.GetInstance().LoadSprrite("HUD", spriteName);
            img.sprite = sprite;
        }
    }

    private string GetSprite(FurnitureType furnitureType)
    {
        //switch (furnitureType)
        //{
        //    case FurnitureType.shafa: return "icon-interactive";
        //    case FurnitureType.BRbed: return "icon-chongwuwo";
        //}
        return "icon-interactive";
    }


}
