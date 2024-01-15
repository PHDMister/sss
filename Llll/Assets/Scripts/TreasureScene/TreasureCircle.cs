using System;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

//挖宝的圈
public class TreasureCircle : MonoBehaviour
{
    public static Dictionary<uint, TreasureCircle> Circles = new Dictionary<uint, TreasureCircle>();
    public static bool Contains(uint circle, ulong userId, Vector3 pos)
    {
        if (Circles.TryGetValue(circle, out TreasureCircle tc))
        {
            if (tc.SelfContains(userId))
            {
                return IsInside(circle, pos);
            }
        }
        return false;
    }
    public static bool IsInside(uint circle, Vector3 pos)
    {
        if (Circles.TryGetValue(circle, out TreasureCircle tc))
        {
            float dis = Vector3.Distance(tc.transform.position, pos);
            if (dis <= tc.Radius) return true;
        }
        return false;
    }



    public const string Event_Enable = "TreasureCircle_Enable";
    public const string Event_PlayerChanged = "TreasureCircle_InsidePlayerChanged";
    public const string Event_RewardBoxHandle = "TreasureCircle_RewardBoxHandle";
    public const string Event_AppointRewardBoxHandle = "TreasureCircle_AppointRewardBoxHandle";

    public Vector3 Center => transform.position;
    [Range(0.01f, 3)] public float Radius = 1;
    [Range(0.1f, 1)] public float SensorInterval = 1f;
    public int CircleId = 0;

    //当前圈内的玩家
    private List<ulong> curPlayerList = new List<ulong>(1);
    private List<ulong> tmpPlayerList = new List<ulong>(1);
    private float rtTime = 0;
    private Transform Effect;
    public bool IsEnable = true;
    private GameObject RewardBox;

    protected void Awake()
    {
        Effect = transform.GetChild(0);
        string[] strs = name.Split('_');
        if (strs.Length < 2) return;
        CircleId = int.Parse(strs[1]);
        Circles[(uint)CircleId] = this;

        //Key : id    Value: bool 
        MessageCenter.AddMsgListener(Event_Enable, OnTreasureCircleEnable);
        //MessageCenter.AddMsgListener(Event_RewardBoxHandle, OnShowRewardBoxHandle);
        MessageCenter.AddMsgListener(Event_AppointRewardBoxHandle, OnAppointRewardBoxHandle);
    }

    protected void OnDestroy()
    {
        Circles.Remove((uint)CircleId);

        //Key : id    Value: bool 
        MessageCenter.RemoveMsgListener(Event_Enable, OnTreasureCircleEnable);
        //MessageCenter.RemoveMsgListener(Event_RewardBoxHandle, OnShowRewardBoxHandle);
        MessageCenter.RemoveMsgListener(Event_AppointRewardBoxHandle, OnAppointRewardBoxHandle);

        RewardBox = null;
    }

    protected void Update()
    {
        if (!IsEnable) return;
        //感知周围角色， 记录角色id
        rtTime += Time.deltaTime;
        if (rtTime >= SensorInterval)
        {
            rtTime = 0;
            SreachPlayerInside();
        }
    }

    //检测目标是否在范围内
    protected void SreachPlayerInside()
    {
        #region old code
        //List<ulong> curInsidePlayers = new List<ulong>();
        //foreach (var playerImp in Singleton<TreasuringController>.Instance.Players)
        //{
        //    float dis = playerImp.Value.Distance(transform.position);
        //    if (dis <= Radius) curInsidePlayers.Add(playerImp.Key);
        //}
        ////自己是否离开圈
        //List<ulong> leavePlayers = curPlayerList.Except(curInsidePlayers).ToList();
        //if (leavePlayers.Contains(ManageMentClass.DataManagerClass.userId))
        //{
        //    Debug.Log("11111111111   玩家离开了 圈" + CircleId);
        //    MessageCenter.SendMessage(Event_PlayerChanged, new KeyValuesUpdate("leave", CircleId));
        //}
        ////自己是否进入圈
        //List<ulong> insidePlayer = curInsidePlayers.Except(curPlayerList).ToList();
        //if (insidePlayer.Contains(ManageMentClass.DataManagerClass.userId))
        //{
        //    Debug.Log("11111111111   玩家进入了 圈" + CircleId);
        //    MessageCenter.SendMessage(Event_PlayerChanged, new KeyValuesUpdate("enter", CircleId));
        //}
        //curPlayerList.Clear();
        //curPlayerList.AddRange(curInsidePlayers);
        #endregion

        tmpPlayerList.Clear();
        ulong userId = ManageMentClass.DataManagerClass.userId;
        if (Singleton<TreasuringController>.Instance.TryGetPlayerImp(userId, out var imp))
        {
            float dis = imp.Distance(transform.position);
            if (dis <= Radius) tmpPlayerList.Add(userId);
        }
        if (tmpPlayerList.Count >= 1 && curPlayerList.Count == 0)
        {
            Debug.Log("11111111111   玩家进入了 圈" + CircleId);
            curPlayerList.Add(userId);
            MessageCenter.SendMessage(Event_PlayerChanged, "enter", CircleId);
        }
        else if (tmpPlayerList.Count == 0 && curPlayerList.Count >= 1)
        {
            Debug.Log("11111111111   玩家离开了 圈" + CircleId);
            curPlayerList.Clear();
            MessageCenter.SendMessage(Event_PlayerChanged, "leave", CircleId);
        }
        else if (tmpPlayerList.Count == 1 && curPlayerList.Count == 1)
        {
            //玩家一致在圈内
            MessageCenter.SendMessage(Event_PlayerChanged, "stay", CircleId);
        }
    }

    protected void OnTreasureCircleEnable(KeyValuesUpdate kv)
    {
        int id = int.Parse(kv.Key);
        if (id != CircleId) return;
        int enableValue = Convert.ToInt32(kv.Values);
        bool enable = enableValue == 1;
        IsEnable = enable;
        Effect.gameObject.SetActive(enable);
        curPlayerList.Clear();
    }

    private void OnShowRewardBoxHandle(KeyValuesUpdate kv)
    {
        if (kv.Key == "show")
        {
            if (RewardBox) return;
            ulong userId = Convert.ToUInt64(kv.Values);
            if (Singleton<TreasuringController>.Instance.TryGetPlayerImp(userId, out var imp))
            {
                float dis = imp.Distance(transform.position);
                if (dis > Radius) return;
            }

            //创建宝箱
            GameObject box = ResourcesMgr.GetInstance().LoadAsset("Prefabs/Treasure/TreasureBox", false);
            box.transform.position = this.transform.position;
            box.transform.localEulerAngles = Vector3.zero;
            RewardBox = box;
            Animator animator = box.GetComponent<Animator>();
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Idle");
        }
        else if (kv.Key == "hide")
        {
            if (RewardBox) GameObject.Destroy(RewardBox);
            RewardBox = null;
        }
    }

    private void OnAppointRewardBoxHandle(KeyValuesUpdate kv)
    {
        if (kv.Key == "show")
        {
            uint cid = Convert.ToUInt32(kv.Values);
            if (cid != CircleId) return;
            if (RewardBox != null) return;
            //创建宝箱
            GameObject box = ResourcesMgr.GetInstance().LoadAsset("Prefabs/Treasure/TreasureBox", false);
            box.transform.position = this.transform.position;
            box.transform.localEulerAngles = Vector3.zero;
            RewardBox = box;
            Animator animator = box.GetComponent<Animator>();
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Idle");
        }
        else if (kv.Key == "hide")
        {
            if (RewardBox) GameObject.Destroy(RewardBox);
            RewardBox = null;
        }
    }


    private bool SelfContains(ulong userId)
    {
        return curPlayerList.Contains(userId);
    }



#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosX.DrawWireDisc(transform.position, transform.up, Radius);
    }

    [ContextMenu("EnableLogic")]
    protected void EnableGameObjcet()
    {
        if (Effect) Effect.gameObject.SetActive(true);
        IsEnable = true;
    }
    [ContextMenu("DisableLogic")]
    protected void DisableGameObjcet()
    {
        if (Effect) Effect.gameObject.SetActive(false);
        IsEnable = false;
    }
#endif

}