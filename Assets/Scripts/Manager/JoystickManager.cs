using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : MonoBehaviour
{
    private Transform _TransJoystick = null;
    private static JoystickManager _instance = null;
    public static JoystickManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_JoystickManager").AddComponent<JoystickManager>();
        }
        return _instance;
    }
    private void Awake()
    {
#if !UNITY_EDITOR
        if (ManageMentClass.DataManagerClass.PlatformType == 2)
        {
            _TransJoystick = this.transform;
            string path = string.Format("{0}{1}", SysDefine.SYS_PATH_GAMECOMPONENT, "JoyStick");
            GameObject joystickObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
            if (joystickObj == null)
                return;
            joystickObj.name = "JoyStick";
            joystickObj.transform.localPosition = Vector3.zero;
            joystickObj.transform.localEulerAngles = Vector3.zero;
            joystickObj.transform.localScale = Vector3.one;
            joystickObj.transform.SetParent(_TransJoystick);
            if (joystickObj.GetComponentInChildren<CameraRotateController>() != null)
            {
                joystickObj.GetComponentInChildren<CameraRotateController>().touchEvent = Camera.main.gameObject.GetComponent<TouchEvent>();
            }
            if (joystickObj.GetComponentInChildren<JoyStick>() != null)
            {
                joystickObj.GetComponentInChildren<JoyStick>().touchEvent = Camera.main.gameObject.GetComponent<TouchEvent>();
            }
            DontDestroyOnLoad(_TransJoystick);
        }
#endif
    }

    public JoyStick GetJoyStick()
    {
        if (ManageMentClass.DataManagerClass.PlatformType == 1)
        {
            return null;
        }
        return _TransJoystick.GetComponentInChildren<JoyStick>();
    }

    public CameraRotateController GetCameraRotateController()
    {
        if (ManageMentClass.DataManagerClass.PlatformType == 1)
        {
            return null;
        }
        return _TransJoystick.GetComponentInChildren<CameraRotateController>();
    }

    public Transform GetJoystickObj()
    {
        if (ManageMentClass.DataManagerClass.PlatformType == 1)
        {
            return null;
        }
        return _TransJoystick;
    }
}
