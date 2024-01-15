using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveControllerImp : MoveController
{
    [Header("========================================="), SerializeField, Range(0, 10)]
    public static float JoyStickOffset = 2;
    [SerializeField, Range(0, 100)]
    private float MoveSlerpFact = 1; //移动的插值系数
    [SerializeField]
    private float WalkSpeed = 100; //移动速度
    [SerializeField]
    private float RunSpeed = 150; //跑动速度
    [SerializeField]
    private float RotaSlerpFact = 1;  //旋转的插值系数
    [SerializeField, Range(0, 100)]
    private float RotaSpeed = 10; //旋转速度
    [SerializeField]
    private float RotaAngleFact = 1; //旋转小于此值时直接设置跳过插值过程

    protected float RealWalkValue
    {
        get { return WalkSpeed * MoveSlerpFact * Time.deltaTime; }
    }
    protected float RealRunValue
    {
        get { return RunSpeed * MoveSlerpFact * Time.deltaTime; }
    }
    protected float RealRotaValue
    {
        get { return RotaSlerpFact * RotaSpeed * Time.deltaTime; }
    }
    protected float CurLocalRy
    {
        get { return transform.localEulerAngles.y; }
    }

    private float targetRY = 0;
    private Vector3 CurPos = Vector3.zero;
    private Vector3 LastPos = Vector3.zero;
    private float nowDis = 0;
    private bool HasSyncValue = false;
    private PlayerItem playerItem;

    private float moveSurValue = 0.1f;
    private float moveSurSlerpValue = 0.05f;
    private float rotaSurValue = 1f;
    private Vector3 moveBefor;
    private Vector3 moveMotionValue = Vector3.zero;
    private int RecordMotionFrame = 0;
    private int SeizingState = 0;
    public int CurSeizingState => SeizingState;
    private float rtNoCacheWaitRestateTime = 0;
    private Vector3 middPoint = Vector3.zero;
    private float radius = 0.28f;
    private int msgCountSlerpFact = 0;
    public bool IsIdle { get; private set; }

    protected override void Awake()
    {

    }

    protected override void Start()
    {
        playerItem = GetComponent<PlayerItem>();
        CharacterController cc = playerItem.GetPlayerCharacter();
        radius = cc.radius;
#if !UNITY_EDITOR
        JoyStick joyStick = JoystickManager.Instance().GetJoyStick();
        if (joyStick != null)
        {
            moveControllJoyStick = joyStick;
            JoyStickOffset = moveControllJoyStick.MaxOffset;
        }
#endif
        IsIdle = true;
        SetMotionFactByFrame();
    }

    protected override void Update()
    {
        UpdateMoveSlerpFact(msgCountSlerpFact);
        OnUpdate();
    }

    protected void OnDestroy()
    {
        playerItem = null;
    }
    protected void UpdateMoveSlerpFact(int msgCount)
    {
        if (msgCount > RoomSyncConst.OverStockMsgCount)
        {
            MoveSlerpFact = 2f;
            RotaSlerpFact = 2f;
        }
        else if (msgCount < RoomSyncConst.OverStockMsgCount && msgCount > RoomSyncConst.SyncMoveCountOnFrame)
        {
            MoveSlerpFact = 1.7f;
            RotaSlerpFact = 1.7f;
        }
        else
        {
            MoveSlerpFact = 1.33f;
            RotaSlerpFact = 1.33f;
        }
    }
    protected void OnUpdate()
    {
        if (!HasSyncValue) return;
 
        var offset = CurPos - transform.position;
        offset.y = 0;
        if (offset.sqrMagnitude >= moveSurSlerpValue)
        {
            IsIdle = false;
            //offset.y = 0; 

            Quaternion origiRota = Quaternion.LookRotation(offset);
            transform.rotation = Quaternion.Slerp(transform.rotation, origiRota, RealRotaValue);

            string animName = nowDis < JoyStickOffset * 0.9f ? "Walk" : "Run";
            //Debug.Log("1111111111111   nowDis="+nowDis+"   JoyStickOffset="+JoyStickOffset*0.9f);
            float moveValue = nowDis < JoyStickOffset * 0.9f ? RealWalkValue : RealRunValue;
            playerItem.SetAnimator(animName, MoveSlerpFact);
            playerItem.SetPlayerMove(moveValue);
            CheckMotionFact();
        }
        else
        {
            IsIdle = true;
            RecordMotionFrame = 0;
            //重置中间点
            middPoint = Vector3.zero;
            //旋转是否一致
            Quaternion tarQua = Quaternion.Euler(0, targetRY, 0);
            float angle = Quaternion.Angle(tarQua, transform.rotation);
            if (angle >= rotaSurValue)
            {
                Quaternion localRota = transform.rotation;
                Quaternion serverRota = Quaternion.Euler(0, targetRY, 0);
                transform.rotation = Quaternion.Slerp(localRota, serverRota, RealRotaValue);
            }
            //位置旋转同步之后 等待一个间隔
            if (rtNoCacheWaitRestateTime > 0)
            {
                rtNoCacheWaitRestateTime -= Time.deltaTime;
                if (rtNoCacheWaitRestateTime <= 0)
                {
                    DoKeepState();
                }
            }
        }
    }
    protected void CheckMotionFact()
    {
        if (RecordMotionFrame == 0) moveBefor = transform.position;
        RecordMotionFrame++;
        if (RecordMotionFrame > RoomSyncConst.SyncFrame)
        {
            RecordMotionFrame = 0;
            moveMotionValue = transform.position - moveBefor;
            if (moveMotionValue.sqrMagnitude <= 0.01f)
            {
                Debug.Log("11111111   没有位移  卡住了   move to " + CurPos);
                transform.position = CurPos;
                //if (!CheckEmptyPoint(CurPos, 0.8f))
                //{
                //    //位置被阻挡 走不过去 放弃目标点 就近停下
                //    middPoint = transform.position;
                //    CurPos = transform.position;
                //    SeizingState = 1;
                //    //Debug.Log("11111111   目标点附近有单位占据 走不过去 。。。。。。。。。");
                //}
                //else
                //{
                //    TryGetNewPos();
                //}
            }
        }
    }
    protected bool TryGetNewPos()
    {
        float bodyRad = radius * 4;
        Vector3 midVector3 = transform.position + (bodyRad * transform.right);
        midVector3.y = transform.position.y;
        if (CheckEmptyPoint(transform.position, midVector3, bodyRad * 2))
        {
            middPoint = midVector3;
            //Debug.Log("11111111   走右边   。。。。。。。。。");
            return true;
        }
        //看看左边能不能过
        midVector3 = transform.position + (bodyRad * -transform.right);
        midVector3.y = transform.position.y;
        if (CheckEmptyPoint(transform.position, midVector3, bodyRad * 2))
        {
            middPoint = midVector3;
            //Debug.Log("11111111   走左边  。。。。。。。。。");
            return true;
        }
        //Debug.Log("11111111   左右都被堵住了  这个放弃目标点   。。。。。。。。。");
        //左右都被堵住了  这个放弃目标点
        middPoint = transform.position;
        CurPos = transform.position;
        SeizingState = 1;
        return false;
    }
    protected bool CheckEmptyPoint(Vector3 origin, Vector3 tar, float dis)
    {
        if (Physics.Raycast(origin, (tar - origin).normalized, dis))
        {
            return false;
        }
        foreach (var player in Singleton<TreasuringController>.Instance.Players.Values)
        {
            if (Vector3.Distance(tar, player.transform.position) <= radius)
            {
                return false;
            }
        }
        return true;
    }
    protected bool CheckEmptyPoint(Vector3 tar, float dis)
    {
        foreach (var player in Singleton<TreasuringController>.Instance.Players.Values)
        {
            if (Vector3.Distance(tar, player.transform.position) <= dis)
            {
                return false;
            }
        }
        return true;
    }

    //====================== 摄像机旋转导致角色转向同步函数 ==========================
    public bool SyncRotaing(float ry, Vector3 curPos)
    {
        HasSyncValue = true;
        rtNoCacheWaitRestateTime = RoomSyncConst.NoCacheWaitRestateTime();

        //移动过程中卡住了  直接放弃本次目标点的移动  进行下一个目标点移动
        if (SeizingState == 1 && curPos == CurPos)
        {
            SeizingState = 0;
            rtNoCacheWaitRestateTime = RoomSyncConst.NoCacheWaitRestateTime();
            return true;
        }

        SeizingState = 0;
        targetRY = ry;
        CurPos = curPos;

        Quaternion tarQua = Quaternion.Euler(0, targetRY, 0);
        float angle = Quaternion.Angle(tarQua, transform.rotation);
        return angle <= rotaSurValue;
    }
    public bool SyncMoving(float ry, Vector3 curPos, float joyNowDis)
    {
        HasSyncValue = true;
        rtNoCacheWaitRestateTime = RoomSyncConst.NoCacheWaitRestateTime();

        if (nowDis != joyNowDis) Debug.Log($"SyncMoving  nowDis:{joyNowDis}  JoyStickOffset:{JoyStickOffset}  ");
        if (LastPos != curPos)
        {
            //重置卡住的标记
            SeizingState = 0;
            RecordMotionFrame = 0;
        }

        //Debug.Log("1111111  SyncMoving     target=" + CurPos + "  server:" + curPos + "   cur=" + transform.position);
        Vector3 offset = curPos != CurPos ? curPos - transform.position : CurPos - transform.position;
        offset.y = 0;
        //float disCurPos = curPos != CurPos ? Vector3.Distance(curPos, transform.position) : Vector3.Distance(CurPos, transform.position);
        if (offset.sqrMagnitude <= moveSurValue)
        {
            return true;
        }

        targetRY = ry;
        CurPos = curPos;
        LastPos = curPos;
        nowDis = joyNowDis;

        //Debug.Log("1111111  SyncMoving     curPos=" + CurPos + "  server:" + curPos + "   false !!! ");
        return false;
    }
    public void DoKeepState(int spCount = 0)
    {
        if (playerItem.IsPlaying("Walk") || playerItem.IsPlaying("Run"))
        {
            playerItem.SetAnimator("Idle");
        }
    }

    public void StopSyncValue()
    {
        HasSyncValue = false;
        CurPos = transform.position;
        LastPos = transform.position;
        msgCountSlerpFact = 0;
    }
    public void SetMotionFactByFrame()
    {
        //当前锁帧比率
        int targetFrame = Application.targetFrameRate;
        if (targetFrame >= 45 && targetFrame < 60)
        {
            WalkSpeed = 100;
            RunSpeed = 150;
        }
        else if (targetFrame == 60 || targetFrame == -1)
        {
#if UNITY_EDITOR
            //WalkSpeed = 100 * 2f;
            //RunSpeed = 150 * 2f;
            WalkSpeed = 100 * 1.33f;
            RunSpeed = 150 * 1.33f;
#else
            WalkSpeed = 100 * 1.33f;
            RunSpeed = 150 * 1.33f;
#endif
        }
    }
    public void SetMsgCountSlerpFact(int count)
    {
        msgCountSlerpFact = count;
    }
}
