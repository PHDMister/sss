using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrlManager : MonoBehaviour
{
    private Transform _TransPlayerCtrl = null;
    private static PlayerCtrlManager _instance = null;
    public static PlayerCtrlManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_PlayerCtrlManager").AddComponent<PlayerCtrlManager>();
        }
        return _instance;
    }

    public void Awake()
    {
        _TransPlayerCtrl = this.gameObject.transform;
    }

    public void Init()
    {
        Transform moveControllerTrans = _TransPlayerCtrl.Find("MoveController");
        if(moveControllerTrans == null)
        {
            string path = string.Format("{0}{1}", SysDefine.SYS_PATH_GAMECOMPONENT, "MoveController");
            GameObject playerCtrlObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
            if (playerCtrlObj == null)
                return;
            playerCtrlObj.name = "MoveController";
            playerCtrlObj.transform.localPosition = Vector3.zero;
            playerCtrlObj.transform.localEulerAngles = Vector3.zero;
            playerCtrlObj.transform.localScale = Vector3.one;
            playerCtrlObj.transform.SetParent(_TransPlayerCtrl);
            MoveController moveController = playerCtrlObj.GetComponent<MoveController>();
            if (moveController == null)
            {
                moveController = playerCtrlObj.AddComponent<MoveController>();
            }
#if !UNITY_EDITOR
            JoyStick joyStick = JoystickManager.Instance().GetJoyStick();
            if (joyStick != null)
            {
                moveController.moveControllJoyStick = joyStick;
            }
            CameraRotateController cameraRotateController = JoystickManager.Instance().GetCameraRotateController();
            if (cameraRotateController != null)
            {
                moveController.cameraRotateController = cameraRotateController;
            }
            moveController.MinaCamera = Camera.main;
#endif
        }
        DontDestroyOnLoad(_TransPlayerCtrl);
        Debug.Log("PlayerCtrlManager.Init End");
    }

    public void ResetMainCamera()
    {
        MoveController moveController = _TransPlayerCtrl.GetComponentInChildren<MoveController>();
        if(moveController != null)
        {
            moveController.ResetMainCameraAndTouchEvent(Camera.main);
        }
    }

    public void AddTouchEvent()
    {
        MoveController moveController = _TransPlayerCtrl.GetComponentInChildren<MoveController>();
        if (moveController != null)
        {
            moveController.AddTouchEvent();
        }
    }
    public MoveController MoveController()
    {
        return  _TransPlayerCtrl.GetComponentInChildren<MoveController>();
    }
    public void RemoveTouchEvent()
    {
        MoveController moveController = _TransPlayerCtrl.GetComponentInChildren<MoveController>();
        if (moveController != null)
        {
            moveController.RemoveTouchEvent();
        }
    }
}
