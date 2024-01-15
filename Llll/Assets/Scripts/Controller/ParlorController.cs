using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;

/// <summary>
/// 客厅同步控制器
/// </summary>
public class ParlorController : BaseSyncController, ISingleton
{
    //交互面板
    public static InteractivePanelHud InteractivePanel = null;

    //Pos
    public Vector3 BirthPos;
    public Quaternion BirthRota;
    public Room RoomData;

    public PlayerControllerImp SelfImp => SyncPlayerList[SelfUserId];

    public void Init()
    {

    }
    public void SavePosRota(Vector3 pos, Quaternion rota)
    {
        BirthPos = pos;
        BirthRota = rota;
    }

    //进入退出的总方法
    public override void Enter()
    {
        if (InteractivePanel == null)
        {
            GameObject panel = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/InteractivePanel", true);
            InteractivePanel = panel.GetComponent<InteractivePanelHud>();
            InteractivePanel.gameObject.SetActive(false);
        }
        //加载控制器池
        LoadPlayerControllerImpPool();
        //添加家具交互事件通知
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.SitdownPush, OnInteractiveHandle);
    }
    public override void Leave()
    {
        UserInfos = null;
        ClearOtherPlayerModel();
        ClearPlayerControllerImpPool();
        //清除事件
        WebSocketAgent.DelProxyMsg((uint)MessageId.Types.Enum.SitdownPush, OnInteractiveHandle);
        //清除
        if (InteractivePanel != null) GameObject.Destroy(InteractivePanel.gameObject);
        InteractivePanel = null;
    }
    //检测
    public InteractivePanelHud CheckPanelHud()
    {
        if (InteractivePanel == null)
        {
            GameObject panel = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/InteractivePanel", true);
            InteractivePanel = panel.GetComponent<InteractivePanelHud>();
            InteractivePanel.gameObject.SetActive(false);
        }
        return InteractivePanel;
    }
    public void CheckAndCancelSelfIntercative(int from = 0)
    {
        InteractiveController icontroller = InteractiveController.FindController(SelfUserId);
        PlayerControllerImp imp = null;
        bool hasImp = TryGetPlayerImp(SelfUserId, out imp);
        //发送协议
        if (hasImp && !string.IsNullOrEmpty(imp.IsInteractive))
        {
            //发送协议  等待协议生成
            SitdownReq req = new SitdownReq();
            req.UserId = SelfUserId;
            req.Sitdown = imp.IsInteractive;
            req.Index = WebSocketAgent.Ins.NetView.GetCode;
            req.IsLeave = true;
            WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.SitdownReq, req, (code, dataString) =>
                {
                    imp.IsInteractive = "";
                });
        }
        //立即取消交互动作
        if (icontroller != null) icontroller.CannelSelfActionImm(imp);
        //从装修一键收起过来的 重置角色到到门口的位置
        if (from == 1 && imp != null)
        {
            CharacterManager.Instance().SetCharacterPosAndRotation(BirthPos, BirthRota);
            float y = imp.playerItem.transform.localEulerAngles.y;
            imp.playerItem.SyncPlayerMove(y, 1, 3);
            imp.playerItem.SetAnimator("Idle");
            ClearSelfMoveCheck();
        }
    }
    public bool HasVisitor()
    {
        return SyncPlayerList.Count > 1;
    }
    public bool OwnerHasVisitor()
    {
        if (!ManageMentClass.DataManagerClass.is_Owner) return false;
        return SyncPlayerList.Count > 1;
    }
    //事件
    protected void OnInteractiveHandle(uint code, ByteString dataByte)
    {
        SitdownPush sitdownPush = SitdownPush.Parser.ParseFrom(dataByte);
        if (sitdownPush.FromUserId == ManageMentClass.DataManagerClass.userId) return;
        if (TryGetPlayerImp(sitdownPush.FromUserId, out var imp))
        {
            InteractiveCfg cfg = InteractiveCfg.Parser(sitdownPush.Sitdown);
            if (cfg == null) return;
            InteractiveController inCon = InteractiveController.FindController(cfg.FurnitureName);
            if (inCon == null) return;
            InteractiveUnit inUnit = inCon.FindUnit(cfg.SubPosName);
            if (inUnit == null) return;
            if (!sitdownPush.IsLeave) inUnit.SetPreSit(sitdownPush.FromUserId);
            imp.AttachInteractiveStateMsg(sitdownPush);
        }
    }

    //行为
    public void DoInterativeAction(SitdownPush sitdownPush)
    {
        InteractiveCfg cfg = InteractiveCfg.Parser(sitdownPush.Sitdown);
        InteractiveController inCon = InteractiveController.FindController(cfg.FurnitureName);
        if (inCon != null && TryGetPlayerImp(sitdownPush.FromUserId, out var imp))
        {
            inCon.DoAction(imp, false);
        }
    }
    public void CancelInteractiveAction(SitdownPush sitdownPush)
    {
        InteractiveCfg cfg = InteractiveCfg.Parser(sitdownPush.Sitdown);
        InteractiveController inCon = InteractiveController.FindController(cfg.FurnitureName);
        if (inCon != null && TryGetPlayerImp(sitdownPush.FromUserId, out var imp))
        {
            inCon.CannelOtherAction(imp);
        }
    }

    public override void UpdateSelfPlayerControllerImp(GameObject go)
    {
        if (RoomData == null) return;
        PlayerControllerImp newImp = AddComp<PlayerControllerImp>(go);
        RoomUserInfo newUserInfo = null;
        foreach (var userInfo in RoomData.UserList)
        {
            if (userInfo.UserId == SelfUserId)
            {
                newUserInfo = userInfo;
                break;
            }
        }
        if (newUserInfo == null)
        {
            Debug.Log("ParlorController  RoomUserInfo  is  null  ");
            return;
        }
        newImp.IsSelf = true;
        newImp.SyncEnable(true);
        newImp.UserInfo = newUserInfo;
        newImp.LookFollowHud = AddHudPanel(go, newUserInfo, true, newUserInfo.NetworkState);
        newImp.playerItem = go.GetComponent<PlayerItem>();
        SyncPlayerList[ManageMentClass.DataManagerClass.userId] = newImp;
    }
    public override void RemoveRoomPlayer(ulong userId)
    {
        base.RemoveRoomPlayer(userId);
        InteractiveController inCon = InteractiveController.FindController(userId);
        if (inCon == null) return;
        InteractiveUnit unit = inCon.FindUnit(userId);
        if (unit == null) return;
        unit.DelPlayer(userId);
        unit.SetPreSit(0);
    }

    //设置出生点
    protected override void SetPos(GameObject go, Move move = null)
    {
        if (move != null && move.Pos != null)
        {
            go.transform.position = new Vector3(move.Pos.X / 1000f, move.Pos.Y / 1000f, move.Pos.Z / 1000f);
            go.transform.rotation = Quaternion.Euler(0, move.Dir.Y / 1000f, 0);
        }
        else
        {
            go.transform.position = BirthPos;
            go.transform.rotation = BirthRota;
        }
    }



    public override void OnReproduceScene()
    {
        //立即搜索一次范围
        InteractiveController.AllSreachPlayerInside();
        //还原所有其他角色的状态
        foreach (var imp in SyncPlayerList.Values)
        {
            ReproduceUserState(imp.UserInfo, imp);
        }
    }

    public override void ReproduceUserState(RoomUserInfo userInfo, PlayerControllerImp imp = null, bool igRefCircle = false)
    {
        if (userInfo == null)
        {
            Debug.LogError("1111111  self  userInfo  is  null ");
            return;
        }
        if (imp == null) imp = SyncPlayerList[userInfo.UserId];
        imp.UserInfo = userInfo;
        if (!string.IsNullOrEmpty(imp.UserInfo.Sitdown))
        {
            //根据交互建筑ID确定交互建筑
            //将角色绑定在建筑上  调用建筑的交互函数
            InteractiveCfg cfg = InteractiveCfg.Parser(imp.UserInfo.Sitdown);
            InteractiveController inCon = InteractiveController.FindController(cfg.FurnitureName);
            if (inCon)
            {
                //有家具 则还原情景
                if (imp.IsSelf) inCon.SelfDoActionImm(imp);
                else inCon.OtherDoActionImm(imp);
            }
            else
            {
                //没有家具 则重置状态
                CharacterManager.Instance().SetCharacterPosAndRotation(BirthPos, BirthRota);
                imp.playerItem.SetAnimator("Idle");
                if (imp.IsSelf)
                {
                    float y = imp.playerItem.transform.localEulerAngles.y;
                    imp.playerItem.SyncPlayerMove(y, 1, 3);
                    ClearSelfMoveCheck();
                }
            }
        }
    }
    public override void ReproduceUserState(ulong userId, bool igRefCircle = true)
    {
        if (SyncPlayerList.TryGetValue(userId, out var pcImp))
        {
            ReproduceUserState(pcImp.UserInfo, pcImp, igRefCircle);
        }
    }

 

}
