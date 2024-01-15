using UIFW;
using UnityEngine;


//硬件网络状态监测  当前的2G 3G 4G 5G wifi 是否在链接
public class NetworkStateMonitor : MonoSingleton<NetworkStateMonitor>
{
    public const string Event_NetworkStateChanged = "Event_NetworkStateChanged";

    public float CheckInterval = 1f;
    private float rTime = 0;
    private bool isEnable = false;

    public override void Init()
    {
        rTime = 0;
        DontDestroyOnLoad(gameObject);
    }

    public void StartCheck()
    {
        //Debug.Log("111111111111  NetWorkChengedHandle   StartCheck isEnable  true " );
        isEnable = true;
        try
        {
            SetTools.SetWindowCustomPropDefine();
        }
        catch
        {
            Debug.Log("111111111111  NetWorkChengedHandle   StartCheck  SetWindowCustomPropDefine fail  " );
        }

        try
        {
            bool networkState = SetTools.GetNetworkAvailable();
            Debug.Log("111111111111  NetWorkChengedHandle    networkState:" + networkState);
        }
        catch
        {
            Debug.Log("111111111111  NetWorkChengedHandle    networkState  GetNetworkAvailable  call   fail ");
            isEnable = false;
        }

        try
        {
            string platform = SetTools.GetEnvironmentPlatform();
            Debug.Log("111111111111  NetWorkChengedHandle    platform:" + platform);
        }
        catch
        {
            Debug.Log("111111111111  NetWorkChengedHandle   platform   GetEnvironmentPlatform  fail  ");
        }

        try
        {
            bool isforeground = SetTools.GetAppIsForeground();
            Debug.Log("111111111111  NetWorkChengedHandle    isforeground:" + isforeground);
        }
        catch
        {
            Debug.Log("111111111111  NetWorkChengedHandle    isforeground  GetAppIsForeground  call   fail ");
        }

    }

    protected void Update()
    {
        if (!isEnable) return;
        rTime += Time.deltaTime;
        if (rTime >= CheckInterval)
        {
            rTime = 0;
            bool networkState = SetTools.GetNetworkAvailable();
            NetWorkChangedHandle(networkState);
        }
    }

    protected void NetWorkChangedHandle(bool networkState)
    {
        //Debug.Log("111111111111  NetWorkChengedHandle   networkState=" + networkState);
        if (networkState == false)
        {
            isEnable = false;
            //打开重登界面
            WebSocketAgent.Ins.ResetWebsocket();
            if (UIManager.GetInstance().IsOpend(FormConst.WAITNETLOADING))
                UIManager.GetInstance().CloseUIForms(FormConst.WAITNETLOADING);
            if (!UIManager.GetInstance().IsOpend(FormConst.NETRECONNECTPANEL))
                UIManager.GetInstance().ShowUIForms(FormConst.NETRECONNECTPANEL);
        }
        //MessageCenter.SendMessage(Event_NetworkStateChanged, "", networkState);
    }


    #region Application Function
    public void OnApplicationQuit()
    {
        try
        {
            WebSocketAgent.Ins.DisConnect();
        }
        catch
        {
        }
    }
    #endregion
}
