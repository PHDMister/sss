using System.Collections.Generic;
using UnityEngine;

public class ShopMgr : ISingleton
{
    private Dictionary<int, List<int>> _dic;
    public void Init()
    {
        _dic = new Dictionary<int, List<int>>();
    }

    public void UpdateShops(List<RainBowShopData> list)
    {
        _dic.Clear();
        foreach (var data in list)
        {
            var itemIds = new List<int>();
            foreach (var item in data.items)
            {
                itemIds.Add(item.item_id);
            }
            _dic.Add(data.item_place, itemIds);
        }
    }

    public List<int> GetShop(int place)
    {
        _dic.TryGetValue(place, out var list);
        if (list == null)
        {
            Debug.LogError("place not exist");
        }
        return list;
    }

    public Dictionary<int, List<int>> GetShops()
    {
        return _dic;
    }
}
