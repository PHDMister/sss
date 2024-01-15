using System.Collections.Generic;
using UnityEngine;

public class BagMgr : ISingleton
{
    //贝壳
    protected int Shell;
    public int ShellNum => Shell;


    private Dictionary<uint, BagItem> _dic;
    public void Init()
    {
        var itemContainer = BinaryDataMgr.Instance.LoadTableById<island_itemContainer>(nameof(island_itemContainer));
        var dataDic = itemContainer.dataDic;

        _dic = new Dictionary<uint, BagItem>(dataDic.Count);
        foreach (var data in dataDic)
        {
            _dic[(uint)data.Key] = new BagItem(data.Value);
        }
    }

    /// <summary>
    /// 通过道具id获取背包item
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BagItem GetItem(int id)
    {
        _dic.TryGetValue((uint)id, out var bagItem);
        if (bagItem == null)
        {
            Debug.LogError("不存在的item id: " + id);
        }
        return bagItem;
    }
    
    public BagItem GetItem(uint id)
    {
        return GetItem((int)id);
    }
    
    public void UpdateItem(uint id, uint num)
    {
        var item = GetItem(id);
        item.UpdateCount(num);
    }

    /// <summary>
    /// 获取背包的所有道具
    /// </summary>
    /// <returns></returns>
    public Dictionary<uint, BagItem> GetBagItems()
    {
        var temp = new Dictionary<uint, BagItem>();
        foreach (var pair in _dic)
        {
            var item = pair.Value;
            if (item.Count > 0)
            {
                temp[pair.Key] = item;
            }
        }
        return temp;
    }

    public void SetShellNum(uint num)
    {
        Shell = (int)num;
    }
    public void AddShellNum(int num)
    {
        Shell += num;
    }
    public void DecShellNum(int num)
    {
        Shell = Mathf.Max(Shell - num, 0);
    }

    public void Clear() {
        foreach (var item in _dic)
        {
            item.Value.UpdateCount(0);
        }
        //Shell = 0;
    }
}