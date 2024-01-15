using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;

public class RainbowBeachDataModel : ISingleton
{
    //免费挖贝壳  剩余次数
    private uint freeShovleCount = 0;
    public uint FreeShovleCount => freeShovleCount;
    //当前抢到的红包信息
    public uint RobShellCount = 0;
    public string RobShellName = "";

    private List<HongbaoData> AllRoomDatas = new List<HongbaoData>();
    public List<HongbaoData> TempAllRoomDatas => AllRoomDatas;

    private List<HongbaoData> CurRoomDatas = new List<HongbaoData>();
    public List<HongbaoData> TempCurRoomDatas => CurRoomDatas;

    private HongbaoData HongbaoEmtry = new HongbaoData();

    public RepeatedField<WelfareData> RankList = new RepeatedField<WelfareData>();

    private Dictionary<string, Sprite> HeadCache = new Dictionary<string, Sprite>();

    public bool OtherShareHongbao = false;
    public void Init()
    {
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.GrabHongbaoResp, GrabHongbaoRespCallBack);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.GameCountPush, UpdateGameCountPush);
        WebSocketAgent.AddProxyMsg((uint)MessageId.Types.Enum.SendHongbaoPush, OnOtherSendHongbaoHandle);
    }

    private void OnOtherSendHongbaoHandle(uint arg1, ByteString arg2)
    {
        SendHongbaoPush push = SendHongbaoPush.Parser.ParseFrom(arg2);
        if (push.FromUserId == ManageMentClass.DataManagerClass.userId) return;
        OtherShareHongbao = true;
        MessageCenter.SendMessage("UpdateShellRedPointData", KeyValuesUpdate.Empty);
        MessageCenter.SendMessage("UpdateSendAllRoom", KeyValuesUpdate.Empty);
    }

    //红包抢没抢到的结果
    private void GrabHongbaoRespCallBack(uint arg1, ByteString arg2)
    {
        GrabHongbaoResp resp = GrabHongbaoResp.Parser.ParseFrom(arg2);
        //您已领过该红包
        if (resp.StatusCode == 230022)
        {
            ToastManager.Instance.ShowNewToast("您已领过该红包", 2);
            return;
        }
        //红包已经被抢完
        if (resp.StatusCode == 230023)
        {
            UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHLATER);
            return;
        }
        //红包过期
        if (resp.StatusCode == 230024)
        {
            ToastManager.Instance.ShowNewToast("红包已过期", 2);
            return;
        }
        RobShellCount = resp.Amount;
        RobShellName = resp.OwnerName;
        ulong id = resp.HongbaoId;
        HongbaoData hongbaoData = CurRoomDatas.Find(data => data.Id == id);
        if (hongbaoData != null) hongbaoData.HasGrab = true;
        //刷缓存数据
        UIManager.GetInstance().ShowUIForms(FormConst.RAINBOWBEACHGIFT);
    }
    private void UpdateGameCountPush(uint arg1, ByteString arg2)
    {
        GameCountPush push = GameCountPush.Parser.ParseFrom(arg2);
        freeShovleCount = push.Count;
        MessageCenter.SendMessage("UpdateFreeShellsOnMainUI", KeyValuesUpdate.Empty);
    }

    public void HongbaoListAllRoomDatas(RepeatedField<HongbaoData> list)
    {
        AllRoomDatas.Clear();
        AllRoomDatas.AddRange(list);
    }
    public HongbaoData GetAllRoomData(int index)
    {
        if (index >= AllRoomDatas.Count) return null;
        return AllRoomDatas[index];
    }
    public void HongbaoListCurRoomDatas(RepeatedField<HongbaoData> list)
    {
        CurRoomDatas.Clear();
        CurRoomDatas.AddRange(list);
    }
    public HongbaoData GetCurRoomData(int index)
    {
        if (index >= CurRoomDatas.Count) return null;
        return CurRoomDatas[index];
    }
    public void SetFreeShovleCount(uint count)
    {
        freeShovleCount = count;
        MessageCenter.SendMessage("UpdateFreeShellsOnMainUI", KeyValuesUpdate.Empty);
    }

    //排行榜
    public WelfareData GetSelfData()
    {
        ulong selfUserId = ManageMentClass.DataManagerClass.userId;
        foreach (var item in RankList)
        {
            if (item.UserId == selfUserId)
            {
                return item;
            }
        }
        return null;
    }
    public void AddHeadCache(string url, Sprite sprite)
    {
        HeadCache[url] = sprite;
    }
    public bool TryGetHeadCache(string url, out Sprite sprite)
    {
        sprite = null;
        if (HeadCache.TryGetValue(url, out sprite))
        {
            return true;
        }
        return false;
    }
    public void ClearHeadCache()
    {
        foreach (var sprite in HeadCache)
        {
            Sprite.DestroyImmediate(sprite.Value, true);
        }
        HeadCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
