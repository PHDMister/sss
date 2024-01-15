using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;

public interface ISyncPlayerController
{
    bool IsWorking { get; }
    void SetRoomData(Room data);
    void SetRoomUser(IEnumerable<RoomUserInfo> list);
    void UpdateSelfPlayerControllerImp(GameObject go);
    void CreateUserInfo(RepeatedField<RoomUserInfo> userInfos);
    void CreateUserInfo(RoomUserInfo userInfo);
    void CreateUserInfo(ulong userId, RepeatedField<long> avatarIds);
    void OnReproduceScene();
    void ReproduceUserState(RoomUserInfo userInfo, PlayerControllerImp imp = null, bool igRefCircle = false);
    void ReproduceUserState(ulong userId, bool igRefCircle = true);
}


public class BaseSyncController : ISyncPlayerController
{
    //Pos
    protected RoomBirthPoint PosGo;
    protected GameObject PcImpPool;
    protected RepeatedField<RoomUserInfo> UserInfos;
    public Room RoomData = new Room();
    public RoomUserInfo UserInfo
    {
        get
        {
            foreach (var userInfo in RoomData.UserList)
            {
                if (userInfo.UserId == SelfUserId) return userInfo;
            }
            return null;
        }
    }
    protected Dictionary<ulong, PlayerControllerImp> SyncPlayerList = new Dictionary<ulong, PlayerControllerImp>();
    public Dictionary<ulong, PlayerControllerImp> Players => SyncPlayerList;
    public RewardPush RewardPush { get; set; }
    protected int enterRoomCount = 0;
    public ulong SelfUserId => ManageMentClass.DataManagerClass.userId;
    protected bool _isWorking = false;
    public bool IsWorking => _isWorking;




    public virtual void Enter()
    {

    }

    public virtual void Leave()
    {

    }


    public virtual void SetRoomData(Room data)
    {
        RoomData = data;
    }
    public virtual void SetRoomUser(IEnumerable<RoomUserInfo> list)
    {
        RoomData.UserList.Clear();
        RoomData.UserList.AddRange(list);
    }
    //清理相关
    public virtual void ClearPlayerControllerImpPool()
    {
        if (!PcImpPool) return;
        int childCount = PcImpPool.transform.childCount;
        if (childCount == 0) return;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(PcImpPool.transform.GetChild(i).gameObject);
        }
    }
    public virtual void ClearOtherPlayerModel()
    {
        foreach (var imp in SyncPlayerList.Values)
        {
            if (imp.IsSelf)
            {
                imp.SyncEnable(false);
                //DelHudPanel(imp.playerItem.gameObject);
            }
            else
            {
                ulong userId = imp.UserInfo.UserId;
                PlayerItem playerItem = imp.playerItem;
                GameObject.Destroy(imp.gameObject);
                DelHudPanel(playerItem.gameObject);
                AvatarManager.Instance().RecyclePlayerPreFun(userId);
                // GameObject.Destroy(imp.playerItem.gameObject);
            }
        }
        SyncPlayerList.Clear();
    }
    public virtual void ClearRoomBirthPoint()
    {
        if (PosGo) GameObject.Destroy(PosGo);
        PosGo = null;
    }
    public virtual void RemoveRoomPlayer(ulong userId)
    {
        if (TryGetPlayerImp(userId, out var pcImp))
        {
            SyncPlayerList.Remove(userId);
            PlayerItem playerItem = pcImp.playerItem;
            playerItem.SetAnimator("Idle");
            MoveControllerImp pcController = pcImp.moveController;
            DelHudPanel(playerItem.gameObject);
            pcController.StopSyncValue();

            GameObject.Destroy(pcImp.gameObject);
            AvatarManager.Instance().RecyclePlayerPreFun(userId);
        }
    }
    public virtual void DelHudPanel(GameObject go)
    {
        Transform hud = go.transform.Find("Hud");
        if (hud && hud.childCount > 0)
            GameObject.Destroy(hud.transform.GetChild(0).gameObject);
    }
    public void LoadPlayerControllerImpPool()
    {
        if (PcImpPool != null) return;
        GameObject go = GameObject.Find("[PcImpPool]");
        if (go)
        {
            PcImpPool = go;
            return;
        }
        PcImpPool = new GameObject("PcImpPool");
        PcImpPool.name = "[PcImpPool]";
        GameObject.DontDestroyOnLoad(PcImpPool);
    }
    //加载必要组件
    public virtual void LoadTreasureDiggingBirthPoint()
    {
        if (PosGo) return;
        GameObject posInt = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/TreasureHuntBeginPos", true);
        posInt.name = "[TreasureHuntBeginPos]";
        PosGo = posInt.GetComponent<RoomBirthPoint>();
    }
    //UI相关
    protected virtual void OpenUI(string ui, bool isRedirection = false)
    {
        if (!UIManager.GetInstance().IsOpend(ui))
        {
            UIManager.GetInstance().ShowUIForms(ui, isRedirection);
        }
    }
    protected virtual void CloseUI(string ui)
    {
        if (UIManager.GetInstance().IsOpend(ui))
        {
            UIManager.GetInstance().CloseUIForms(ui);
        }
    }


    //初始化相关
    protected T AddComp<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (!comp) comp = go.AddComponent<T>();
        return comp;
    }
    public LookFollowHud AddHudPanel(GameObject go, RoomUserInfo userInfo, bool isSelf, uint state = 0)
    {
        Transform namePanel = go.transform.Find("Hud/NamePanel");
        if (namePanel)
        {
            LookFollowHud nameFollowHud = namePanel.GetComponent<LookFollowHud>();
            nameFollowHud.SetNetOfflinkEnable(CheckOfflinkIcon(state, userInfo.UserId));
            nameFollowHud.SetPlayerName(userInfo.UserName, isSelf);
            return nameFollowHud;
        }

        GameObject hudPanel = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/NamePanel", true);
        hudPanel.name = "NamePanel";
        Transform parent = go.transform.Find("Hud");
        LookFollowHud lookFollowHud = hudPanel.GetComponent<LookFollowHud>();
        lookFollowHud.ChangeParent(parent);
        lookFollowHud.SetNetOfflinkEnable(CheckOfflinkIcon(state, userInfo.UserId));
        lookFollowHud.SetPlayerName(userInfo.UserName, isSelf);
        return lookFollowHud;
    }
    public void SetPlayerNetOfflinkEnable(ulong userId, uint netState)
    {
        if (SyncPlayerList.TryGetValue(userId, out var pc))
            pc.LookFollowHud.SetNetOfflinkEnable(CheckOfflinkIcon(netState, userId));
    }
    protected bool CheckOfflinkIcon(uint netStateCode, ulong userId)
    {
        NetworkState.Types.Enum netState = (NetworkState.Types.Enum)netStateCode;
        //Debug.Log("111111111111111   CheckOfflinkIcon   netState=" + netState + "  userId=" + userId);
        switch (netState)
        {
            case NetworkState.Types.Enum.None:
            case NetworkState.Types.Enum.Online:
                return false;
            case NetworkState.Types.Enum.Offline:
            case NetworkState.Types.Enum.Reconnecting:
                return true;
        }
        return false;
    }



    //创建角色
    public virtual void CreateUserInfo(RepeatedField<RoomUserInfo> userInfos)
    {
        UserInfos = userInfos;
        foreach (RoomUserInfo userInfo in userInfos)
        {
            CreateUserInfo(userInfo);
        }
    }
    public virtual void CreateUserInfo(RoomUserInfo userInfo)
    {
        if (Players.ContainsKey(userInfo.UserId)) return;
        if (userInfo.AvatarIds.Count == 0) return;
        if (userInfo.UserId == ManageMentClass.DataManagerClass.userId)
        {
            GameObject go = CharacterManager.Instance().GetPlayerObj();
            if (go == null) go = AvatarManager.Instance().GetOtherPlayerPreFun(userInfo);
            PlayerControllerImp imp = AddComp<PlayerControllerImp>(go);
            imp.IsSelf = true;
            imp.SyncEnable(true);
            imp.LookFollowHud = AddHudPanel(go, userInfo, imp.IsSelf, userInfo.NetworkState);
            imp.UserInfo = userInfo;
            imp.playerItem = go.GetComponent<PlayerItem>();
            Players[userInfo.UserId] = imp;
            SetPos(go, userInfo.CurPos);
        }
        else
        {
            GameObject go = AvatarManager.Instance().GetOtherPlayerPreFun(userInfo);
            go.name = "player_" + userInfo.UserId;
            CharacterManager.Instance().PlayOtherPlayerSpecialEffect(go);
            PlayerControllerImp imp = CreateControllerImp(userInfo.UserId.ToString());
            imp.playerItem = go.GetComponent<PlayerItem>();
            imp.moveController = go.GetComponent<MoveControllerImp>();
            imp.UserInfo = userInfo;
            imp.IsSelf = false;
            imp.LookFollowHud = AddHudPanel(go, userInfo, imp.IsSelf, userInfo.NetworkState);
            imp.ResetForStart();
            Players[userInfo.UserId] = imp;
            if (!go.activeSelf) go.SetActive(true);
            SetPos(go, userInfo.CurPos);
            imp.moveController.StopSyncValue();
        }
    }
    public virtual void CreateUserInfo(ulong userId, RepeatedField<long> avatarIds)
    {
        if (SyncPlayerList.TryGetValue(userId, out var imp))
        {
            if (userId == ManageMentClass.DataManagerClass.userId)
            {
                GameObject go = CharacterManager.Instance().GetPlayerObj();
                imp.playerItem = go.GetComponent<PlayerItem>();
                imp.LookFollowHud = AddHudPanel(go, imp.UserInfo, imp.IsSelf);
            }
            else
            {
                GameObject go = AvatarManager.Instance().ChangeOtherAvatarFun(userId, avatarIds);
                go.name = "player_" + userId;
                CharacterManager.Instance().PlayOtherPlayerSpecialEffect(go);
                imp.playerItem = go.GetComponent<PlayerItem>();
                imp.moveController = go.GetComponent<MoveControllerImp>();
                imp.LookFollowHud = AddHudPanel(go, imp.UserInfo, imp.IsSelf, imp.UserInfo.NetworkState);
                imp.RefPipeline();
            }
        }
    }
    public virtual void UpdateSelfPlayerControllerImp(GameObject go)
    {
        PlayerControllerImp newImp = AddComp<PlayerControllerImp>(go);
        RoomUserInfo newUserInfo = UserInfo;
        newImp.IsSelf = true;
        newImp.SyncEnable(true);
        newImp.UserInfo = newUserInfo;
        newImp.LookFollowHud = AddHudPanel(go, newUserInfo, true, newUserInfo.NetworkState);
        newImp.playerItem = go.GetComponent<PlayerItem>();
        SyncPlayerList[ManageMentClass.DataManagerClass.userId] = newImp;
    }
    public virtual PlayerControllerImp CreateControllerImp(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = PcImpPool.transform;
        go.transform.localPosition = Vector3.zero;
        return go.AddComponent<PlayerControllerImp>();
    }
    protected virtual void SetPos(GameObject go, Move move = null)
    {
        if (move != null && move.Pos != null)
        {
            if (move.Pos.Y <= -100)
            {
                Transform trans = PosGo.GetPoint(SyncPlayerList.Count - 1);
                go.transform.position = trans.position;
                go.transform.localEulerAngles = trans.localEulerAngles;
            }
            else
            {
                go.transform.position = new Vector3(move.Pos.X / 1000f, move.Pos.Y / 1000f, move.Pos.Z / 1000f);
                go.transform.rotation = Quaternion.Euler(0, move.Dir.Y / 1000f, 0);
            }
        }
        else
        {
            Transform trans = PosGo.GetPoint(SyncPlayerList.Count - 1);
            go.transform.position = trans.position;
            go.transform.localEulerAngles = trans.localEulerAngles;
        }
    }
    //自身数据
    public virtual RoomUserInfo GetSelfUserInfo()
    {
        return TreasureModel.Instance.TeamUserList.Find(info => info.UserId == SelfUserId);
    }
    public virtual bool TryGetPlayerImp(ulong useid, out PlayerControllerImp imp)
    {
        if (SyncPlayerList.TryGetValue(useid, out imp))
        {
            return true;
        }
        return false;
    }

    //还原表现相关 
    public virtual void OnReproduceScene()
    {

    }
    public virtual void ReproduceUserState(RoomUserInfo userInfo, PlayerControllerImp imp = null, bool igRefCircle = false)
    {

    }
    public virtual void ReproduceUserState(ulong userId, bool igRefCircle = true)
    {
        if (SyncPlayerList.TryGetValue(userId, out var pcImp))
        {
            ReproduceUserState(pcImp.UserInfo, pcImp, igRefCircle);
        }
    }

    //显示或者隐藏相关
    public virtual void ShowOrHideOtherRoomPlayer(bool bShow)
    {
        foreach (var imp in Players)
        {
            if (imp.Key != ManageMentClass.DataManagerClass.userId)
            {
                ulong userId = imp.Key;
                Transform playerRootTrans = CharacterManager.Instance().GetTransCharacter();
                GameObject playerObj = playerRootTrans.transform.Find("player_" + userId).gameObject;
                if (playerObj != null)
                {
                    playerObj.SetActive(bShow);
                }
            }
        }
    }

    public virtual void SetSelfMoveCheck(Action cb)
    {
        PlayerCtrlManager.Instance().MoveController().SetSendBeforeCheck(cb);
    }
    public virtual void ClearSelfMoveCheck()
    {
        PlayerCtrlManager.Instance().MoveController().ClearSendBeforeCheck();
    }

    public virtual void SetMoveNormal(bool value)
    {
        if (value) ClearSelfMoveCheck();
        else SetSelfMoveCheck(() => { });
    }


}
