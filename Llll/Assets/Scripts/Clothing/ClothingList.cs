using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

public class ClothingList:MonoBehaviour
{
    public GameObject Item;
    public GameObject Default;

    private List<LoopListViewItem2> _itemList;
    private Dictionary<string, ItemPool> _itemPoolDic;
    private void Awake()
    {
        _itemList = new List<LoopListViewItem2>();
        _itemPoolDic = new Dictionary<string, ItemPool>();

        AddItemPool(Item);
        AddItemPool(Default);
    }

    private void AddItemPool(GameObject obj)
    {
        var itemPool = new ItemPool();
        itemPool.Init(obj,0,0,0, transform.GetComponent<RectTransform>());
        _itemPoolDic.Add(obj.name, itemPool);
    }

    private void OnDestroy()
    {
        foreach (var pair in _itemPoolDic)
        {
            pair.Value.DestroyAllItem();
        }
        _itemPoolDic.Clear();
    }

    private ItemPool GetPool(string key)
    {
        _itemPoolDic.TryGetValue(key, out var itemPool);
        if (itemPool == null)
        {
            itemPool = new ItemPool();
            _itemPoolDic[name] = itemPool;
        }

        return itemPool;
    }

    public void SetData(List<ThreeLevelData> dataList, int rowIndex, int rowCount)
    {
        foreach (var item in _itemList)
        {
            var itemPool = GetPool(item.name);
            itemPool.RecycleItem(item);
            item.gameObject.SetActive(false);
        }
        _itemList.Clear();

        for (int i = 0; i< rowCount; ++i){
            int itemIndex = rowIndex * rowCount + i;
            if (itemIndex < dataList.Count)
            {
                var item = GetItem(Item.name, i);
                var clothingItem = item.GetComponent<ClothingItem>();
                var data = dataList[itemIndex];
                clothingItem.SetIndex(itemIndex);
                clothingItem.SetData(data);

            }else if (itemIndex == dataList.Count)
            {
               var item = GetItem(Default.name, i);
               var clothingDefault = item.GetComponent<ClothingDefault>();
               clothingDefault.SetIndex(itemIndex);
               clothingDefault.SetData();
            }
        }  
    }

    private LoopListViewItem2 GetItem(string name, int index)
    {
        var itemPool = GetPool(name);
        var item = itemPool.GetItem();
        item.name = name;
        _itemList.Add(item);
        item.GetComponent<RectTransform>().anchoredPosition = new Vector2(index * 225f, 0) ;
        return item;
    }
}
