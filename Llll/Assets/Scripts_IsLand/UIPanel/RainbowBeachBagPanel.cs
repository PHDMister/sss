//<Tools\GenUICode>工具生成, UI变化重新生成
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Google.Protobuf.Collections;
using SuperScrollView;
using Treasure;
using UnityEngine.UI;
using UIFW;

public class RainbowBeachBagPanel : BaseUIForm
{

	// UI VARIABLE STATEMENT START
	private Toggle toggle_tab1; 
	private Toggle toggle_tab2; 
	private ScrollRect scrollrect_itemScroll; 
	private ScrollRect scrollrect_rewardScroll; 
	private Button button_btnClose;
	private Text txtEmpty;
	// UI VARIABLE STATEMENT END

	// UI VARIABLE ASSIGNMENT START
	private void Awake() 
	{
		toggle_tab1 = FindComp<Toggle>("group/center/tab1"); 
		toggle_tab2 = FindComp<Toggle>("group/center/tab2"); 
		scrollrect_itemScroll = FindComp<ScrollRect>("group/center/itemScroll"); 
		scrollrect_rewardScroll = FindComp<ScrollRect>("group/center/rewardScroll"); 
		button_btnClose = FindComp<Button>("group/btnClose"); 
		txtEmpty = FindComp<Text>("group/center/txtEmpty"); 

		OnAwake(); 
		AddEvent(); 

	}
	// UI VARIABLE ASSIGNMENT END

	// UI EVENT REGISTER START
	private void AddEvent() 
	{
		toggle_tab1.onValueChanged.AddListener(Ontab1ValueChanged);
		toggle_tab2.onValueChanged.AddListener(Ontab2ValueChanged);
		scrollrect_itemScroll.onValueChanged.AddListener(OnitemScrollValueChanged);
		scrollrect_rewardScroll.onValueChanged.AddListener(OnrewardScrollValueChanged);
		RigisterCompEvent(button_btnClose, OnbtnCloseClicked);
		
		ReceiveMessage("OnRewardPush", p =>
		{
			if (toggle_tab2.isOn)
			{
				Ontab2ValueChanged(true);
			}
		});
	}
	// UI EVENT REGISTER END

	// UI EVENT FUNC START
	private void Ontab1ValueChanged(bool active)
	{
		if (!active)
		{
			return;
		}
		scrollrect_itemScroll.gameObject.SetActive(true);
		scrollrect_rewardScroll.gameObject.SetActive(false);

		if (_bagItems == null)
		{
			_bagItems = GetBagItems();
		}
		
		int count1 = _bagItems.Count / rowCount;
		if (_bagItems.Count % rowCount > 0)
		{
			count1++;
		}
		//count1 is the total row count
		_view1.SetListItemCount(count1,false);
		_view1.RefreshAllShownItem();

		txtEmpty.gameObject.SetActive(_bagItems.Count == 0);
		txtEmpty.text = "暂无道具哦 ~";
	}
	
	private void Ontab2ValueChanged(bool active)
	{
		if (!active)
		{
			return;
		}
		scrollrect_itemScroll.gameObject.SetActive(false);
		scrollrect_rewardScroll.gameObject.SetActive(true);
		
		MessageManager.GetInstance().SendTreasureRecordReq(1, 100, ManageMentClass.DataManagerClass.userId, (p) =>
		{
			_recordListDatas = p.List.ToList();
			_recordListDatas.Sort((x, y) => x.CreatedAt > y.CreatedAt ? -1:1);
			_view2.SetListItemCount(_recordListDatas.Count);
			
			txtEmpty.gameObject.SetActive(_recordListDatas.Count == 0);
			txtEmpty.text = "暂无奖励哦 ~";
			
		}, new [] {(uint)LoadSceneType.RainbowBeach, (uint)LoadSceneType.ShenMiHaiWan, (uint)LoadSceneType.HaiDiXingKong});
	}
	
	private void OnitemScrollValueChanged(Vector2 arg)
	{

	}
	private void OnrewardScrollValueChanged(Vector2 arg)
	{

	}
	private void OnbtnCloseClicked(GameObject go)
	{
		CloseUIForm();
	}
	// UI EVENT FUNC END

	private List<BagItem> _bagItems;

	private List<TreasureRecord> _recordListDatas;
	
	private LoopListView2 _view1;
	private LoopListView2 _view2;
	
	private const int rowCount = 4;
	private void OnAwake() 
	{
		CurrentUIType.UIForms_Type = UIFormType.PopUp;
		CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
		CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Translucence;

		_view1 = scrollrect_itemScroll.GetComponent<LoopListView2>();
		_view2 = scrollrect_rewardScroll.GetComponent<LoopListView2>();
		
		_view1.InitListView(0, OnGetItemByIndex1);
		_view2.InitListView(0, OnGetItemByIndex2);
	}

	private LoopListViewItem2 OnGetItemByIndex1(LoopListView2 listView, int index)
	{
		if (index < 0 )
		{
			return null;
		}
		//create one row
		LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
		//update all items in the row
		for (int i = 0; i< rowCount; ++i)
		{
			int itemIndex = index * rowCount + i;
			UIBagItem uiBagItem = item.transform.GetChild(i).GetComponent<UIBagItem>();
			if(itemIndex >= _bagItems.Count)
			{
				uiBagItem.gameObject.SetActive(false);
				continue;
			}
			var data = _bagItems[itemIndex];
			//update the subitem content.
			if (data != null)
			{
				uiBagItem.gameObject.SetActive(true);
				uiBagItem.SetData(data);
			}
			else
			{
				uiBagItem.gameObject.SetActive(false);
			}
		}
		return item;
	}

	private LoopListViewItem2 OnGetItemByIndex2(LoopListView2 view, int index)
	{
		if (index < 0 || index >= _recordListDatas.Count)
		{
			return null;
		}
        
		var item = view.NewListViewItem("UIBagRewardItem");
		var uiRewardItem = item.GetComponent<UIBagRewardItem>();

		var data = _recordListDatas[index];
		uiRewardItem.SetData(data);
		return item;
	}
	
	public override void Display() 
	{
		base.Display();
		toggle_tab1.SetIsOnWithoutNotify(true);
		Ontab1ValueChanged(true);

		toggle_tab2.SetIsOnWithoutNotify(false);
	}
	
	private List<BagItem> GetBagItems()
	{
		var dir = Singleton<BagMgr>.Instance.GetBagItems();
		var list = dir.Values.ToList();
		//list.Sort();
		return list;
	}

	public override void Hiding() 
	{
		base.Hiding();
		_bagItems = null;
		_recordListDatas = null;
		
		_view1.SetListItemCount(0);
		_view2.SetListItemCount(0);
	}

	public override void Redisplay() 
	{
		base.Redisplay();
	}

	public override void Freeze() 
	{
		base.Freeze();
	}

}
