using System;
using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;


/// <summary>
/// 交互建筑  绑定交互建筑
/// </summary>
public class InteractiveController : MonoBehaviour
{
    public static List<InteractiveController> CacheList = new List<InteractiveController>();
    public static InteractiveController FindController(ulong userId)
    {
        if (CacheList.Count == 0) return null;
        return CacheList.Find(ic => ic.FindUnit(userId) != null);
    }
    public static InteractiveController FindController(string name)
    {
        if (CacheList.Count == 0) return null;
        return CacheList.Find(ic => ic.transform.parent.name == name);
    }
    public static void AllSreachPlayerInside()
    {
        CacheList.ForEach(ic => ic.SreachPlayerInside());
    }


    //家具类型
    public FurnitureType FurnitureType = FurnitureType.None;
    //交互信息 List
    public InteractiveUnit[] InteractiveUnit;
    //建筑Id  便于查找建筑
    public string BuildingId => transform.parent.name;
    //自己id
    protected ulong userId => ManageMentClass.DataManagerClass.userId;
    //计时器
    protected Timer SitDownAnimEndTimer;
    protected Timer GetUpAnimEndTimer;
    //缓存当前交互对象
    protected PlayerControllerImp CacheImp;
    //时间相关
    private float time = 0;
    private float interval = 0.2f;
    private bool pauseSreachPlayer = false;


    protected void Awake()
    {
        CacheList.Add(this);
    }
    protected void Start()
    {
        time = interval;
    }
    protected void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = interval;
            if (!pauseSreachPlayer)
                SreachPlayerInside();
        }
    }
    protected void OnDestroy()
    {
        CacheList.Remove(this);
        foreach (var unit in InteractiveUnit) unit.Clear();
        SitDownAnimEndTimer?.Cancel();
        SitDownAnimEndTimer = null;
        GetUpAnimEndTimer?.Cancel();
        GetUpAnimEndTimer = null;
        CacheImp = null;
        time = 0;
    }
    //检测目标是否在范围内
    protected void SreachPlayerInside()
    {
        GameObject pGo = CharacterManager.Instance().GetPlayerObj();
        ulong userId = ManageMentClass.DataManagerClass.userId;
        if (pGo == null) return;
        foreach (var unit in InteractiveUnit)
        {
            if (unit.IsTriggerZone(pGo.transform.position))
            {
                if (!unit.HasPlayer())
                {
                    unit.EnterUnit(userId, this.gameObject, FurnitureType);
                }
            }
            else
            {
                unit.LeaveUnit(userId);
            }
        }
    }
    protected InteractiveUnit FindUnit(GameObject go, bool IsNear = false)
    {
        if (IsNear == false)
        {
            foreach (var unit in InteractiveUnit)
            {
                if (unit.IsTriggerZone(go.transform.position))
                {
                    return unit;
                }
            }
            return null;
        }

        float minLen = 100000;
        Vector3 pos = go.transform.position;
        InteractiveUnit minIntUnit = null;
        foreach (var unit in InteractiveUnit)
        {
            float dis = unit.TiggerDistance(pos);
            if (dis < minLen)
            {
                minLen = dis;
                minIntUnit = unit;
            }
        }
        return minIntUnit;
    }
    public InteractiveUnit FindUnit(ulong userId)
    {
        foreach (var unit in InteractiveUnit)
        {
            if (unit.HasPlayer(userId))
            {
                return unit;
            }
        }
        return null;
    }
    public InteractiveUnit FindUnit(string posName)
    {
        foreach (var unit in InteractiveUnit)
        {
            if (unit.PosTrans.name == posName)
            {
                return unit;
            }
        }
        return null;
    }


    //交互托管
    public void DoAction(PlayerControllerImp imp, bool immExe = false)
    {
        CacheImp = imp;
        SreachPlayerInside();
        //当前的角色
        if (imp.IsSelf)
        {
            if (immExe) SelfDoActionImm(imp);
            else SelfDoAction(imp);
            return;
        }

        //其他同步的角色 
        if (immExe) OtherDoActionImm(imp);
        else OtherDoAction(imp);
    }
    protected void SelfDoAction(PlayerControllerImp imp)
    {
        //执行过程中 不接受摇杆指令    
        Singleton<ParlorController>.Instance.SetSelfMoveCheck(() => { });
        //将模型放到指定位置
        InteractiveUnit interactiveUnit = null;
        if (imp.IsSelf)
            interactiveUnit = FindUnit(CharacterManager.Instance().GetPlayerObj(), false);
        else
            interactiveUnit = FindUnit(imp.playerItem.gameObject, false);

        if (interactiveUnit == null)
        {
            Debug.LogError(" Interactive Controller  name:" + this.name + "  Interactive Controller is null");
            return;
        }
        //设置交互状态
        imp.IsInteractive = interactiveUnit.GetSitInfo(BuildingId);
        pauseSreachPlayer = true;
        interactiveUnit.HideHud();
        //设置交互位置和旋转
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        //设置摄像机的旋转参数
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = false;
        Cinemachine.CinemachineFreeLook camerFreeLook = CameraManager.Instance().cameraFreeLook;
        float angle = imp.playerItem.transform.eulerAngles.y;
        camerFreeLook.m_XAxis.Value = angle;
        camerFreeLook.m_YAxis.Value = 0.5f;

        //关闭名字HUD
        EnableHud(imp, false);
        //播放坐下动画
        imp.playerItem.SetAnimator(interactiveUnit.SitDownAnim);
        //等待坐下动画完成
        float animLen = imp.playerItem.GetCurAnimLength(interactiveUnit.SitDownAnim);
        SitDownAnimEndTimer = Timer.RegisterRealTimeNoLoop(animLen, () =>
        {
            //清除计时器
            SitDownAnimEndTimer?.Cancel();
            SitDownAnimEndTimer = null;
            //重新设置设置摇杆的触发事件 触发站起动作
            Singleton<ParlorController>.Instance.SetSelfMoveCheck(OnCancelSelfInteractiveHandle);
            imp.PauseImp = false;
        });
    }
    protected void OtherDoAction(PlayerControllerImp imp)
    {
        //将模型放到指定位置
        InteractiveUnit interactiveUnit = FindUnit(imp.playerItem.gameObject, true);
        if (interactiveUnit == null)
        {
            Debug.LogError(" Interactive Controller  name:" + this.name + "  Interactive Controller is null");
            return;
        }
        imp.IsInteractive = interactiveUnit.GetSitInfo(BuildingId);
        interactiveUnit.AddPlayer(imp.UserInfo.UserId);
        imp.moveController.StopSyncValue();
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        //关闭名字HUD
        EnableHud(imp, false);
        //播放坐下动画
        imp.playerItem.SetAnimator(interactiveUnit.SitDownAnim);
        float animLen = imp.playerItem.GetCurAnimLength(interactiveUnit.SitDownAnim);
        SitDownAnimEndTimer = Timer.RegisterRealTimeNoLoop(animLen, () =>
        {
            //清除计时器
            SitDownAnimEndTimer?.Cancel();
            SitDownAnimEndTimer = null;
        });
    }
    public void SelfDoActionImm(PlayerControllerImp imp)
    {
        //将模型放到指定位置
        InteractiveUnit interactiveUnit = FindUnit(imp.UserInfo.UserId);
        if (interactiveUnit == null)
        {
            Debug.LogError(" Interactive Controller  name:" + this.name + "  Interactive Controller is null");
            return;
        }
        imp.IsInteractive = interactiveUnit.GetSitInfo(BuildingId);
        pauseSreachPlayer = true;
        interactiveUnit.HideHud();
        interactiveUnit.AddPlayer(imp.UserInfo.UserId);
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        //设置摄像机的旋转参数
        ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = false;
        //关闭名字HUD
        EnableHud(imp, false);
        //播放坐下动画
        imp.playerItem.SetAnimator(interactiveUnit.SitDownAnim, 10);
        float time = imp.playerItem.GetCurAnimLength(interactiveUnit.SitDownAnim);
        Timer.RegisterRealTimeNoLoop(time / 10.0f, () =>
        {
            imp.playerItem.SetAnimatorSpeed(1);
            //设置摄像机旋转
            Cinemachine.CinemachineFreeLook camerFreeLook = CameraManager.Instance().cameraFreeLook;
            float angle = imp.playerItem.transform.eulerAngles.y;
            camerFreeLook.m_XAxis.Value = angle;
            camerFreeLook.m_YAxis.Value = 0.5f;
        });
        //重新设置设置摇杆的触发事件 触发站起动作
        Singleton<ParlorController>.Instance.SetSelfMoveCheck(OnCancelSelfInteractiveHandle);
        imp.PauseImp = false;
    }
    public void OtherDoActionImm(PlayerControllerImp imp)
    {
        //将模型放到指定位置
        InteractiveUnit interactiveUnit = FindUnit(imp.playerItem.gameObject, false);
        if (interactiveUnit == null)
        {
            Debug.LogError(" Interactive Controller  name:" + this.name + "  Interactive Controller is null");
            return;
        }
        imp.IsInteractive = interactiveUnit.GetSitInfo(BuildingId);
        interactiveUnit.AddPlayer(imp.UserInfo.UserId);
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        //关闭名字HUD
        EnableHud(imp, false);
        //播放坐下动画
        imp.playerItem.SetAnimator(interactiveUnit.SitDownAnim, 10);
        float time = imp.playerItem.GetCurAnimLength(interactiveUnit.SitDownAnim);
        Timer.RegisterRealTimeNoLoop(time / 10.0f, () => { imp.playerItem.SetAnimatorSpeed(1); });
    }
    public void CannelOtherAction(PlayerControllerImp imp)
    {
        SitDownAnimEndTimer?.Cancel();
        SitDownAnimEndTimer = null;
        ulong userId = imp.IsSelf ? ManageMentClass.DataManagerClass.userId : imp.UserInfo.UserId;
        InteractiveUnit interactiveUnit = FindUnit(userId);
        if (interactiveUnit == null)
        {
            Debug.LogError("CannelAction  interactiveUnit  is  null !!!");
            return;
        }
        imp.IsInteractive = "";
        interactiveUnit.DelPlayer(userId);
        interactiveUnit.SetPreSit(0);
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        //imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        imp.playerItem.SetAnimator(interactiveUnit.GetUpAnim);
        //名字HUD
        EnableHud(imp, true);
    }
    public void CannelSelfActionImm(PlayerControllerImp imp)
    {
        SitDownAnimEndTimer?.Cancel();
        SitDownAnimEndTimer = null;
        ulong userId = imp.IsSelf ? ManageMentClass.DataManagerClass.userId : imp.UserInfo.UserId;
        InteractiveUnit interactiveUnit = FindUnit(userId);
        if (interactiveUnit == null)
        {
            Debug.LogError("CannelAction  interactiveUnit  is  null !!!");
            return;
        }
        imp.IsInteractive = "";
        interactiveUnit.DelPlayer(userId);
        interactiveUnit.SetPreSit(0);
        imp.playerItem.transform.position = interactiveUnit.PosTrans.position;
        imp.playerItem.transform.rotation = interactiveUnit.PosTrans.rotation;
        imp.playerItem.SetAnimator("Idle");
        //名字HUD
        EnableHud(imp, true);
        //清除摇杆的拦截事件
        Singleton<ParlorController>.Instance.ClearSelfMoveCheck();
        pauseSreachPlayer = false;
    }
    protected void OnCancelSelfInteractiveHandle()
    {
        //设置摇杆空转 阻挡操作
        Singleton<ParlorController>.Instance.SetSelfMoveCheck(() => { });
        string curScene = SceneManager.GetActiveScene().name.ToLower();
        if (CheckNeedSendMsgForCancel(curScene))
        {
            //发送协议  等待协议生成
            SitdownReq req = new SitdownReq();
            req.UserId = ManageMentClass.DataManagerClass.userId;
            req.Sitdown = Singleton<ParlorController>.Instance.SelfImp.IsInteractive;
            req.Index = WebSocketAgent.Ins.NetView.GetCode;
            req.IsLeave = true;
            WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.SitdownReq, req, (code, data) =>
            {
                SitdownResp resp = SitdownResp.Parser.ParseFrom(data);
                if (resp.StatusCode > 0)
                {
                    Debug.LogError("InteractivePanelHud  OnBtnClick   interactive  respose  code :" + resp.StatusCode);
                    return;
                }
                OnInteractivePlayEnd(resp);
            });
        }
        else
        {
            OnInteractivePlayEnd(null);
        }
    }
    protected bool CheckNeedSendMsgForCancel(string curScene)
    {
        if (curScene.Contains("gerenkongjian"))
        {
            return true;
        }
        return false;
    }
    protected void OnInteractivePlayEnd(SitdownResp resp)
    {
        GameObject go = CharacterManager.Instance().GetPlayerObj();
        PlayerControllerImp imp = go.GetComponent<PlayerControllerImp>();
        if (!imp) imp = go.AddComponent<PlayerControllerImp>();
        imp.IsInteractive = "";
        //播放站起的动画
        InteractiveUnit interactiveUnit = FindUnit(imp.playerItem.gameObject, false);
        imp.playerItem.SetAnimator(interactiveUnit.GetUpAnim);
        //等待站起动作播放完成
        AnimationClip getupAnim = imp.playerItem.GetAnimClip(interactiveUnit.GetUpAnim);
        float animLen = getupAnim != null
            ? imp.playerItem.GetCurAnimLength(interactiveUnit.GetUpAnim)
            : imp.playerItem.GetCurAnimLength(interactiveUnit.SitDownAnim);

        GetUpAnimEndTimer = Timer.RegisterRealTimeNoLoop(animLen, () =>
        {
            GetUpAnimEndTimer?.Cancel();
            GetUpAnimEndTimer = null;
            //清除摇杆的拦截事件
            Singleton<ParlorController>.Instance.ClearSelfMoveCheck();
            //开启摄像机和人物旋转
            ManageMentClass.DataManagerClass.CameraControllerPlayerRotation = true;
            //开启名字HUD
            EnableHud(imp, true);
            //开启继续搜索
            pauseSreachPlayer = false;
        });
        interactiveUnit.Clear();
    }
    protected void EnableHud(PlayerControllerImp imp, bool isShow)
    {
        //名字HUD
        Transform hudTransform = imp.playerItem.transform.Find("Hud");
        if (hudTransform) hudTransform.gameObject.SetActive(isShow);
    }


#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (InteractiveUnit != null && InteractiveUnit.Length > 0)
        {
            foreach (var unit in InteractiveUnit)
            {
                unit.OnDrawGizmos();
            }
        }
    }
#endif
}

//交互单元
[Serializable]
public class InteractiveUnit
{
    [Header("<坐标>")]
    public Vector3 RangePos = Vector3.zero;
    public Vector3 RangeOffset = Vector3.zero;
    public Transform PosTrans;
    [Header("<动画>")]
    public string SitDownAnim;
    public string SitDownIdleAnim;
    public string GetUpAnim;
    [Header("<圆形>")]
    public float Radius = 0.5f;

    //预约玩家 防止延时时座位重复
    protected List<ulong> unitList = new List<ulong>(1);
    private Vector3 relativePos = Vector3.zero;
    private ulong preSitId = 0;

    public bool IsTriggerZone(Vector3 pos)
    {
        return Vector3.Distance(pos, RangePos) <= Radius;
    }
    public float TiggerDistance(Vector3 pos)
    {
        return Vector3.Distance(pos, RangePos);
    }
    public bool HasPlayer()
    {
        return unitList.Count > 0;
    }
    public bool HasPlayer(ulong userid)
    {
        return unitList.Contains(userid);
    }
    public void AddPlayer(ulong userId)
    {
        if (!unitList.Contains(userId)) unitList.Add(userId);
    }
    public void DelPlayer(ulong userId)
    {
        unitList.Remove(userId);
    }
    public void SetPreSit(ulong userId)
    {
        preSitId = userId;
        if (preSitId > 0)
        {
            InteractivePanelHud interactivePanel = Singleton<ParlorController>.Instance.CheckPanelHud();
            if (preSitId != ManageMentClass.DataManagerClass.userId)
            {
                interactivePanel.transform.SetParent(null);
                interactivePanel.gameObject.SetActive(false);
            }
        }
    }
    public void EnterUnit(ulong userId, GameObject go, FurnitureType furnitureType)
    {
        //有同步角色预约当前的位置
        if (preSitId > 0) return;
        //没其他角色的预约
        if (!unitList.Contains(userId))
        {
            unitList.Add(userId);
            InteractivePanelHud interactivePanel = Singleton<ParlorController>.Instance.CheckPanelHud();
            interactivePanel.gameObject.SetActive(true);
            InteractiveCfg interactiveCfg = new InteractiveCfg(go.transform.parent.name, PosTrans.name);
            interactivePanel.CurShowGoName = interactiveCfg;
            interactivePanel.transform.SetParent(go.transform);
            interactivePanel.transform.position = GetRelativePos();
            interactivePanel.ChangeSprite(furnitureType);
        }
    }
    public void LeaveUnit(ulong userId)
    {
        if (unitList.Contains(userId))
        {
            unitList.Remove(userId);
            InteractivePanelHud interactivePanel = Singleton<ParlorController>.Instance.CheckPanelHud();
            if (interactivePanel.gameObject.activeSelf)
            {
                interactivePanel.transform.SetParent(null);
                interactivePanel.gameObject.SetActive(false);
            }
            interactivePanel.CurShowGoName = null;
        }
        if (preSitId == userId) preSitId = 0;
    }
    public void HideHud()
    {
        InteractivePanelHud interactivePanel = Singleton<ParlorController>.Instance.CheckPanelHud();
        if (interactivePanel.gameObject.activeSelf)
        {
            interactivePanel.transform.SetParent(null);
            interactivePanel.gameObject.SetActive(false);
        }
    }
    public Vector3 GetRelativePos()
    {
        if (relativePos == Vector3.zero)
        {
            relativePos = new Vector3(RangePos.x + RangeOffset.x, 1.5f + RangeOffset.y, RangePos.z + RangeOffset.z);
        }
        return relativePos;
    }
    public void Clear()
    {
        unitList.Clear();
        preSitId = 0;
    }
    public string GetSitInfo(string front)
    {
        return front + "/" + PosTrans.name;
    }
#if UNITY_EDITOR
    private Vector3 up = new Vector3(0f, 1, 0f);
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosX.DrawWireDisc(RangePos, up, Radius);
    }
#endif
}
public class InteractiveCfg
{
    public string FurnitureName;
    public string SubPosName;

    public InteractiveCfg(string fName, string subPos)
    {
        FurnitureName = fName;
        SubPosName = subPos;
    }

    public static InteractiveCfg Parser(string interString)
    {
        //Debug.Log("111111111111   interString=" + interString);
        if (string.IsNullOrEmpty(interString)) return null;
        string[] inters = interString.Split('/');
        if (inters.Length < 2) return null;
        return new InteractiveCfg(inters[0], inters[1]);
    }

    public override string ToString()
    {
        return FurnitureName + "/" + SubPosName;
    }
}