using System.Collections.Generic;
using Google.Protobuf.Collections;
using Treasure;
using UIFW;
using UnityEngine;

public class AquariumDataModel : ISingleton
{
    private RepeatedField<FishTypeMap> aquariumData;
    
    public void Init()
    {
     
    }

    //同步海洋博物馆系统数据
    public void SysAquariumData(RepeatedField<FishTypeMap> aquarium)
    {
        if ( aquarium == null )
            Debug.LogError("海洋博物馆数据同步异常");
        aquariumData = aquarium;
    }
    
    //获取鱼的数量
    public int GetFishCount( int fishId )
    {
        if ( aquariumData != null )
        {
            for (int i = 0; i < aquariumData.Count; i++)
            {
                RepeatedField<FishData> datas = aquariumData[i].List;
                
                for (int j = 0; j < datas.Count; j++)
                {
                    if ( (int)datas[j].FishId == fishId )
                    {
                        return (int)datas[j].Count;
                    }
                }
            }
        }
        
        return 0;
    }
    
    //更新鱼的数量
    public void UpdateFishData( uint fishId , uint cnt , bool isAdd = true)
    {
        if ( aquariumData != null )
        {
            //更新鱼的数量
            for (int i = 0; i < aquariumData.Count; i++)
            {
                RepeatedField<FishData> datas = aquariumData[i].List;
                
                for (int j = 0; j < datas.Count; j++)
                {
                    if ( (int)datas[j].FishId == fishId )
                    {
                        if ( isAdd )
                        {
                            datas[j].Count += cnt;
                        }
                        else
                        {
                            datas[j].Count = cnt;
                        }
                    }
                }
            }
            //海洋博物馆数据更新
            MessageCenter.SendMessage("AquariumDataMsg", "Update" , null);
        }
        else
        {
            Debug.LogError("海洋博物馆数据未同步");
        }
    }
    
}