using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform _TransCameraFreeLook = null;
    public Cinemachine.CinemachineFreeLook cameraFreeLook = null;
    public Cinemachine.CinemachineBrain cameraBrain;

    private static CameraManager _instance =null;
    public static CameraManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_CameraManager").AddComponent<CameraManager>();
        }
        return _instance;
    }

    public void Awake()
    {
        string path = string.Format("{0}{1}", SysDefine.SYS_PATH_GAMECOMPONENT, "CM FreeLook1");
        GameObject cMFreeLookObj = ResourcesMgr.GetInstance().LoadAsset(path, true);
        if (cMFreeLookObj == null)
            return;
        _TransCameraFreeLook = cMFreeLookObj.transform;
        cMFreeLookObj.name = "CM FreeLook1";
        cMFreeLookObj.transform.localPosition = new Vector3(-5.422f, 2.0205f, 10.61f);
        cMFreeLookObj.transform.localEulerAngles = Vector3.zero;
        cMFreeLookObj.transform.localScale = Vector3.one;
        cameraFreeLook = cMFreeLookObj.GetComponent<Cinemachine.CinemachineFreeLook>();
        cameraBrain = Camera.main.gameObject.GetComponent<Cinemachine.CinemachineBrain>();
        DontDestroyOnLoad(cMFreeLookObj);
        DontDestroyOnLoad(transform);
    }

    public void SetFollow(Transform follow,Transform look)
    {
        if(cameraFreeLook != null)
        {
            cameraFreeLook.Follow = follow;
            cameraFreeLook.LookAt = look;
        }
        AddGestureControl();
    }

    public Cinemachine.CinemachineFreeLook GetFreeLook()
    {
        return cameraFreeLook;
    }
    public Cinemachine.CinemachineBrain GetBrain()
    {
        if (cameraBrain==null)
        {
            cameraBrain = Camera.main.gameObject.GetComponent<Cinemachine.CinemachineBrain>();
        }
        return cameraBrain;
    }

    public void AddGestureControl()
    {
        if(Camera.main.gameObject.GetComponent<GestureControl>() == null)
        {
            Camera.main.gameObject.AddComponent<GestureControl>();
        }
    }
}
