using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveController : MonoBehaviour
{
    protected EasyJoystick easyJoystick;
    protected EasyTouch easyTouch;

    public JoyStick moveControllJoyStick;
    public CameraRotateController cameraRotateController;
    public Camera MinaCamera;
    public bool isFirst = true;
    public Vector2 aaaaa = Vector2.zero;

    protected bool isPlayRun = false;
    public bool isHaveJoyTouch = true;
    public int joystickTouchID = -1;
    protected bool isJoystickControlled = false;
    protected float joystickRadius;
    protected Vector2 startTouchPos;
    public GameObject abab;
    public int startInputCount = 0;

    protected bool IsMoveStart = false;
    public bool IsStartUpSyncMove;

    float moveXSpeed = 10f;
    float moveYSpeed = 0.1f;

    //位置
    protected Vector3 lastPointPos;
    private Action OnSendBeforeCheck;
    protected Action OnDragDownAction;
    protected Action OnDragAction;
    protected Action OnDragUpAction;



    protected virtual void Awake()
    {
        if (moveControllJoyStick != null)
        {
            moveControllJoyStick.touchEvent = Camera.main.GetComponent<TouchEvent>();
        }

        if (cameraRotateController != null)
        {
            cameraRotateController.touchEvent = Camera.main.GetComponent<TouchEvent>();
        }
    }
    protected virtual void Start()
    {
        RemoveTouchEvent();
        AddTouchEvent();
    }

    protected void On_JoystickMoveEnd(MovingJoystick move)
    {
        if (OnSendBeforeCheck != null)
        {
            OnSendBeforeCheck();
            return;
        }

#if UNITY_EDITOR
        unityController();
#endif
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            Vector2 rota = playerItem.transform.localEulerAngles;
            playerItem.SyncPlayerMove(rota.y, 0, 3);
        }
    }

    protected void On_JoystickMove(MovingJoystick move)
    {
        if (OnSendBeforeCheck != null)
        {
            OnSendBeforeCheck();
            return;
        }

#if UNITY_EDITOR
        unityController();
#endif
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            Vector2 rota = playerItem.transform.localEulerAngles;
            playerItem.SyncPlayerMove(rota.y, 0, 2);
        }
    }
    public void SetOnDrags(Action onDownAction, Action onDragAction, Action onUpAction)
    {
        OnDragDownAction = onDownAction;
        OnDragAction = onDragAction;
        OnDragUpAction = onUpAction;
    }

    /// <summary>
    /// 镜头旋转抬起
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    protected void CameraRotateController_onUp(Vector3 arg1)
    {
        if (IsStartUpSyncMove && !IsMoveStart && ManageMentClass.DataManagerClass.IsControllerPlayer)
        {
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem)
            {
                Vector3 V3 = playerItem.transform.localEulerAngles;
                playerItem.SyncPlayerMove(V3.y, 0, 12);
            }
        }
    }
    /// <summary>
    /// 镜头旋转按压
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    protected void CameraRotateController_onDrag(Vector3 arg1)
    {
        if (InterfaceHelper.IsClickUI() || !InterfaceHelper.bInRightHalfScreen())
        {
            return;
        }
        if (!ManageMentClass.DataManagerClass.IsControllerPlayer || !ManageMentClass.DataManagerClass.CameraController)
        {
            return;
        }
        var DeltaX = (arg1.x) * Time.deltaTime * moveXSpeed;
        var DeltaY = (arg1.y) * Time.deltaTime * moveYSpeed;
        Cinemachine.CinemachineFreeLook freeLook = CameraManager.Instance().GetFreeLook();
        if (freeLook != null)
        {
            freeLook.m_XAxis.Value += DeltaX;
            freeLook.m_YAxis.Value -= DeltaY;
        }
        //GameStartController.freeLook.m_XAxis.Value += DeltaX;
        //GameStartController.freeLook.m_YAxis.Value -= DeltaY;
        //Vector3 V3 = GameStartController.PlayerObj.transform.localEulerAngles;
        //站立时不改变角色面向 暂时注释
        if (ManageMentClass.DataManagerClass.CameraControllerPlayerRotation)
        {
            //Vector3 V3 = Vector3.zero;
            //GameObject playerObj = CharacterManager.Instance().GetPlayerObj();
            //if (playerObj != null)
            //{
            //    V3 = playerObj.transform.localEulerAngles;
            //    V3.y += DeltaX;
            //}
            ////GameStartController.PlayerItem.SetPlayerLocalAngle(V3);
            //PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            //if (playerItem != null)
            //{
            //    playerItem.SetPlayerLocalAngle(V3);
            //}
            //if (IsStartUpSyncMove && !IsMoveStart && ManageMentClass.DataManagerClass.IsControllerPlayer)
            //{
            //    playerItem?.SyncPlayerMove(V3.y, 0, 11);
            //}
        }

    }
    /// <summary>
    /// 镜头旋转按下
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    protected void CameraRotateController_onDown(Vector3 arg1)
    {
    }
    /// <summary>
    /// 摇杆移动按下
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    protected void MoveControllJoyStick_onDown(Vector3 arg1, Vector3 arg2, float maxDis)
    {
        if (OnSendBeforeCheck != null)
        {
            OnSendBeforeCheck();
            return;
        }
        if (IsStartUpSyncMove && ManageMentClass.DataManagerClass.IsControllerPlayer)
        {
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem != null)
            {
                Vector2 rota = playerItem.transform.localEulerAngles;
                playerItem.SyncPlayerMove(rota.y, 0, 1);
            }
        }
        OnDragDownAction?.Invoke();
    }
    /// <summary>
    /// 摇杆移动按压
    /// </summary>
    /// <param name="arg1">大圆盘位置</param>
    /// <param name="arg2">小圆盘位置</param>
    protected void MoveControllJoyStick_onDrag(Vector3 arg1, Vector3 arg2, float maxDis)
    {
        if (OnSendBeforeCheck != null)
        {
            OnSendBeforeCheck();
            return;
        }
        if (!ManageMentClass.DataManagerClass.IsControllerPlayer)
        {
            return;
        }
        Vector2 deltaPos = arg2 - arg1;
        // Debug.Log("这里处理Deltapos的值： " + deltaPos);
        if (deltaPos != Vector2.zero)
        {
            Vector2 tmp_Dir = (deltaPos - aaaaa);
            tmp_Dir = tmp_Dir.normalized;
            tmp_Dir = CalcDir(tmp_Dir);
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem != null)
            {
                playerItem.SetPlayerRotation(tmp_Dir, 2);
            }
            float nowDis = Vector3.Distance(arg1, arg2);
            if (nowDis < (maxDis * 0.9f))
            {
                if (playerItem != null)
                {
                    playerItem.SetAnimator("Walk");
                    playerItem.SetPlayerMove(3);
                }
            }
            else
            {
                if (playerItem != null)
                {
                    playerItem.SetAnimator("Run");
                    playerItem.SetPlayerMove(6);
                }
            }
            //判断当前是否需要开启网络同步
            if (IsStartUpSyncMove) playerItem?.SyncPlayerMove(playerItem.transform.localEulerAngles.y, nowDis, 2);
            OnDragAction?.Invoke();
        }
    }
    /// <summary>
    /// 摇杆移动抬起
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    protected void MoveControllJoyStick_onUp(Vector3 arg1, Vector3 arg2, float maxDis)
    {
        if (OnSendBeforeCheck != null)
        {
            OnSendBeforeCheck();
            return;
        }
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
        if (playerItem != null)
        {
            playerItem.SetAnimator("Idle");
        }
        if (IsStartUpSyncMove && ManageMentClass.DataManagerClass.IsControllerPlayer)
        {
            if (playerItem != null)
            {
                Vector2 rota = playerItem.transform.localEulerAngles;
                playerItem.SyncPlayerMove(rota.y, 0, 3);
            }
        }
        OnDragUpAction?.Invoke();
    }


    protected void unityController()
    {
        if (easyJoystick == null)
            return;
        Vector2 axisData = easyJoystick.JoystickAxis;

        if (CharacterManager.Instance().GetPlayerObj() == null)
        {
            return;
        }
        PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();

        Debug.Log("输出一下名字的内容： " + playerItem.transform.gameObject.name);
        // float swipeLength=easyJoystick.legth
        if (axisData != Vector2.zero)
        {
            if (isFirst)
            {
                isFirst = false;
                aaaaa = axisData;
            }
            // PlayerAnimator.SetBool("Run", true);
            //  isPlayRun = true;
            Vector2 tmp_Dir = (axisData - aaaaa);
            tmp_Dir = tmp_Dir.normalized;
            tmp_Dir = CalcDir(tmp_Dir);
            //设置旋转，
            //设置移动
            //   Debug.Log(" axisData: " + axisData + "  tmp_Dir " + tmp_Dir);
            //GameStartController.PlayerItem.SetPlayerMove(3);
            //GameStartController.PlayerItem.SetPlayerRotation(tmp_Dir, 3);
            //GameStartController.PlayerItem.SetAnimator("Walk");
            if (playerItem != null)
            {
                playerItem.SetPlayerMove(3);
                playerItem.SetPlayerRotation(tmp_Dir, 3);
                playerItem.SetAnimator("Walk");
            }
        }
        else
        {
            if (playerItem != null)
            {
                playerItem.SetAnimator("Idle");
            }
            //GameStartController.PlayerItem.SetAnimator("Idle");
            // PlayerAnimator.SetBool("Run", false);
            isFirst = true;
        }
    }
    protected void androidControllerFun()
    {

    }
    /// <summary>
    /// 做的角度转换
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected Vector2 CalcDir(Vector2 dir)
    {
        float angleValue = VectorAngle(Vector2.up, new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));
        Vector2 dirB = ChangeDirAngle(new Vector3(dir.x, 0, dir.y), angleValue);
        float angleValueB = VectorAngle(dir, new Vector2(dirB.x, dirB.y));
        return dirB;
    }
    /// <summary>
    /// 返回两个向量之间的夹角
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    protected float VectorAngle(Vector2 from, Vector2 to)
    {
        float angle;
        Vector3 cross = Vector3.Cross(from, to);
        angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }
    /// <summary>
    /// 将向量进行角度偏移
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="angleValue"></param>
    protected Vector2 ChangeDirAngle(Vector3 _dir, float angleValue)
    {
        _dir = Quaternion.AngleAxis(angleValue, Vector2.up) * _dir;
        return new Vector2(_dir.x, _dir.z);
    }
    protected void OnJoystickMove(MovingJoystick move)
    {
        if (move.joystickName != "MoveJoyStick")
        {
            return;
        }
        //MoveTouchControllerFun();
        SetMovePCControllerFun(move);
    }
    protected void MoveSatrtFun(MovingJoystick move)
    {
        Debug.Log("这里走进来了嘛");
        IsMoveStart = true;
    }
    protected void SetMovePCControllerFun(MovingJoystick move)
    {
        /*Vector2 axisData = move.joystickAxis;
        if (axisData != Vector2.zero)
        {
            *//*if (isFirst)
            {
                isFirst = false;
                aaaaa = axisData;
            }*//*
            // PlayerAnimator.SetBool("Run", true);
            //  isPlayRun = true;
            Vector2 tmp_Dir = (axisData - aaaaa);
            tmp_Dir = tmp_Dir.normalized;
            tmp_Dir = CalcDir(tmp_Dir);
            //设置旋转，
            //设置移动
            //   Debug.Log(" axisData: " + axisData + "  tmp_Dir " + tmp_Dir);
            GameStartController.PlayerItem.SetPlayerMove(3);
            GameStartController.PlayerItem.SetPlayerRotation(tmp_Dir, 2);
            GameStartController.PlayerItem.SetAnimator("Walk");
            Vector2 joystickRadiusa = easyJoystick.JoystickPositionOffset;
        }
        else
        {
            GameStartController.PlayerItem.SetAnimator("Idle");
            // PlayerAnimator.SetBool("Run", false);
            isFirst = true;
        }*/
    }
    // 检查触摸点是否控制摇杆
    protected bool CheckJoystickControl(Vector2 touchPos)
    {
        float DistanceA = Vector2.Distance(touchPos, new Vector2(-21f, -15f));
        Debug.Log("输出一下具体的内容值   距离和远近：   " + touchPos);
        return true;//DistanceA <= joystickRadius;
    }
    protected void MoveTouchControllerFun()
    {
        isPlayRun = false;
        isHaveJoyTouch = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (joystickTouchID != -1)
            {
                if (joystickTouchID == touch.fingerId)
                {
                    isHaveJoyTouch = true;
                }
            }
        }
        if (!isHaveJoyTouch)
        {

            isJoystickControlled = false;
            joystickTouchID = -1;
        }
        else
        {
        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began || IsMoveStart)
            {
                if (joystickTouchID == -1)
                {
                    IsMoveStart = false;
                    if (CheckJoystickControl(touch.position))
                    {
                        joystickTouchID = touch.fingerId;
                        startTouchPos = touch.position;
                        isJoystickControlled = true;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == joystickTouchID)
                {
                    // 停止摇杆控制
                    joystickTouchID = -1;
                    isJoystickControlled = false;
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (isJoystickControlled)
                {
                    // text.text = "Moved控制对： " + Input.touchCount + "   joystickTouchID: " + joystickTouchID + "   isJoystickControlled: " + isJoystickControlled;
                }
                if (touch.fingerId == joystickTouchID)
                {
                    //  text.text = "MovedIDDID： " + Input.touchCount + "   joystickTouchID: " + joystickTouchID + "   isJoystickControlled: " + isJoystickControlled;
                }
                if (touch.fingerId == joystickTouchID && isJoystickControlled)
                {
                    isJoystickControlled = true;
                    // 计算第一个触摸点的位移量并更新摇杆控制
                    Vector2 deltaPos = touch.position - startTouchPos;
                    Debug.Log("这里处理Deltapos的值： " + deltaPos);
                    PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();

                    if (deltaPos != Vector2.zero)
                    {
                        /*if (isFirst)
                        {
                            isFirst = false;
                            aaaaa = deltaPos;
                        }*/

                        isPlayRun = true;
                        Vector2 tmp_Dir = (deltaPos - aaaaa);
                        tmp_Dir = tmp_Dir.normalized;
                        tmp_Dir = CalcDir(tmp_Dir);
                        if (playerItem != null)
                        {
                            playerItem.SetPlayerRotation(tmp_Dir, 2);
                            playerItem.SetPlayerMove(3);
                            playerItem.SetAnimator("Walk");
                        }
                        //GameStartController.PlayerItem.SetPlayerRotation(tmp_Dir, 2);
                        //GameStartController.PlayerItem.SetPlayerMove(3);
                        //GameStartController.PlayerItem.SetAnimator("Walk");

                    }
                    else
                    {
                        if (playerItem != null)
                        {
                            playerItem.SetAnimator("Idle");
                        }
                        //GameStartController.PlayerItem.SetAnimator("Idle");
                        isFirst = true;
                    }
                }
            }
        }
        if (!isPlayRun)
        {
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem != null)
            {
                playerItem.SetAnimator("Idle");
            }
            //GameStartController.PlayerItem.SetAnimator("Idle"); 
        }
        startInputCount = Input.touchCount;
    }
    //移动摇杆结束  
    protected void OnJoystickMoveEnd(MovingJoystick move)
    {
        //停止时，角色恢复idle  
        if (move.joystickName == "MoveJoyStick")
        {
            PlayerItem playerItem = CharacterManager.Instance().GetPlayerItem();
            if (playerItem != null)
            {
                playerItem.SetAnimator("Idle");
            }
            //GameStartController.PlayerItem.SetAnimator("Idle");
        }
        IsMoveStart = true;
        isPlayRun = false;
        isHaveJoyTouch = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (joystickTouchID != -1)
            {
                if (joystickTouchID == touch.fingerId)
                {
                    isHaveJoyTouch = true;
                }
            }
        }
        if (!isHaveJoyTouch)
        {

            isJoystickControlled = false;
            joystickTouchID = -1;
        }
        else
        {
        }
    }



    protected PlayerItem playerItem1;
    protected bool IsKeyDown = false;

    protected virtual void Update()
    {
#if UNITY_EDITOR

        InputKekContrlMoveFun();
#else
        if (ManageMentClass.DataManagerClass.PlatformType == 1)
        {
            InputKekContrlMoveFun();
        }
#endif
    }

    public void InputKekContrlMoveFun()
    {
        //  if (ManageMentClass.DataManagerClass.isOfficialEdition) return;
        playerItem1 = CharacterManager.Instance().GetPlayerItem();
        Vector2 tmp_Dir = Vector2.zero;
        isPlayRun = false;
        if (Input.GetKey(KeyCode.W))
        {
            isPlayRun = true;
            tmp_Dir.y += 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            isPlayRun = true;
            tmp_Dir.y -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            isPlayRun = true;
            tmp_Dir.x -= 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            isPlayRun = true;
            tmp_Dir.x += 1;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)
                                        || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (!IsKeyDown)
            {
                IsKeyDown = true;
                IsMoveStart = true;
                if (OnSendBeforeCheck != null)
                {
                    OnSendBeforeCheck();
                    return;
                }
                Vector2 rota = playerItem1.transform.localEulerAngles;
                playerItem1.SyncPlayerMove(rota.y, 1, 1);
                OnDragDownAction?.Invoke();
            }
        }
        else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)
                                          && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            if (IsKeyDown)
            {
                IsKeyDown = false;
                IsMoveStart = false;
                if (OnSendBeforeCheck != null)
                {
                    OnSendBeforeCheck();
                    return;
                }
                Vector2 rota = playerItem1.transform.localEulerAngles;
                playerItem1.SyncPlayerMove(rota.y, 0, 3);
                OnDragUpAction?.Invoke();
            }
        }

        if (isPlayRun)
        {
            if (OnSendBeforeCheck != null)
            {
                OnSendBeforeCheck();
                return;
            }

            tmp_Dir = tmp_Dir.normalized;
            tmp_Dir = CalcDir(tmp_Dir);

            if (playerItem1 != null)
            {
                playerItem1.SetPlayerRotation(tmp_Dir, 2); 
                playerItem1.SetPlayerMove(3);
                playerItem1.SetAnimator("Walk");
                playerItem1.SyncPlayerMove(playerItem1.transform.localEulerAngles.y, 1, 2);
                OnDragAction?.Invoke();
            }
        }
        else if (playerItem1 && playerItem1.IsPlaying("Walk"))
        {
            playerItem1.SetAnimator("Idle");
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("点击了");
            lastPointPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            //记录之前的位置
            Vector3 prePointPos = lastPointPos;
            //记录偏移量
            Vector3 offset = Input.mousePosition - prePointPos;
            //小圈跟着一起移动 
            lastPointPos += offset;

            CameraRotateController_onDrag(offset);
        }
    }





    public void ResetMainCameraAndTouchEvent(Camera camera)
    {
#if !UNITY_EDITOR
        if (camera != null)
        {
            MinaCamera = camera;
        }
        CameraRotateController cameraRotateController = JoystickManager.Instance().GetCameraRotateController();
        if (cameraRotateController != null)
        {
            cameraRotateController.touchEvent = Camera.main.GetComponent<TouchEvent>();
        }

        JoyStick moveControllJoyStick = JoystickManager.Instance().GetJoyStick();
        if (moveControllJoyStick != null)
        {
            moveControllJoyStick.touchEvent = Camera.main.GetComponent<TouchEvent>();
        }
#endif
    }
    public void RemoveTouchEvent()
    {
#if !UNITY_EDITOR
        JoyStick moveControllJoyStick = JoystickManager.Instance().GetJoyStick();
        if (moveControllJoyStick != null)
        {
            Debug.Log("RemoveTouchEvent moveControllJoyStick");
            moveControllJoyStick.onDown -= MoveControllJoyStick_onDown;
            moveControllJoyStick.onDrag -= MoveControllJoyStick_onDrag;
            moveControllJoyStick.onUp -= MoveControllJoyStick_onUp;
        }

        CameraRotateController cameraRotateController = JoystickManager.Instance().GetCameraRotateController();
        if (cameraRotateController != null)
        {
            Debug.Log("RemoveTouchEvent cameraRotateController");
            cameraRotateController.onDown -= CameraRotateController_onDown;
            cameraRotateController.onDrag -= CameraRotateController_onDrag;
            cameraRotateController.onUp -= CameraRotateController_onUp;
        }
#endif
    }

    public void AddTouchEvent()
    {
#if !UNITY_EDITOR
        JoyStick moveControllJoyStick = JoystickManager.Instance().GetJoyStick();
        if (moveControllJoyStick != null)
        {
            moveControllJoyStick.Init();
            Debug.Log("AddTouchEvent moveControllJoyStick");
            moveControllJoyStick.onDown += MoveControllJoyStick_onDown;
            moveControllJoyStick.onDrag += MoveControllJoyStick_onDrag;
            moveControllJoyStick.onUp += MoveControllJoyStick_onUp;
        }

        CameraRotateController cameraRotateController = JoystickManager.Instance().GetCameraRotateController();
        if (cameraRotateController != null)
        {
            Debug.Log("AddTouchEvent cameraRotateController");
            cameraRotateController.Init();
            cameraRotateController.onDown += CameraRotateController_onDown;
            cameraRotateController.onDrag += CameraRotateController_onDrag;
            cameraRotateController.onUp += CameraRotateController_onUp;
        }
#endif
    }

    public void SetSendBeforeCheck(Action cb)
    {
        OnSendBeforeCheck = cb;
    }
    public void ClearSendBeforeCheck()
    {
        OnSendBeforeCheck = null;
    }
}
