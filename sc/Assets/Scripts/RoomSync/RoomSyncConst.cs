using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSyncConst
{
    //移动相关同步间隔
    public const float RoomSyncMoveInterval = 0.333f;
    //一次同步的有效刷新帧数  WebGL平台 不建议进行锁帧
    public static int SyncIntervalFrame = 0;
    //同步过程中 每秒帧数
    public const int SyncFrame = 60;
    //这只同步的帧数
    private static int _syncFrame = 60;
    public static void SetSyncFrame()
    {
        _syncFrame = UnityEngine.Application.targetFrameRate;
        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = 60;

        //UnityEngine.Application.targetFrameRate = SyncFrame;
        //SyncIntervalFrame = Mathf.CeilToInt(SyncFrame * RoomSyncMoveInterval);
    }
    public static void ResumeSyncFrame()
    {
        //UnityEngine.Application.targetFrameRate = -1;
    }

    //当消息包积压超过一秒的同步总量
    private static int _overStockMsgCount = 0;
    public static int OverStockMsgCount
    {
        get
        {
            if (_overStockMsgCount == 0)
            {
                _overStockMsgCount = Mathf.FloorToInt(1.0f / RoomSyncMoveInterval * 2f);
            }
            return _overStockMsgCount;
        }
    }
    //一秒中的位移同步量
    public static int SyncMoveCountOnFrame => Mathf.FloorToInt(1 / RoomSyncMoveInterval);
    

    //当本地没有服务器缓存移动包时 结束移动拟态的时间
    public static float NoCacheWaitRestateTime()
    {
        return 0.1f;
    }
}
