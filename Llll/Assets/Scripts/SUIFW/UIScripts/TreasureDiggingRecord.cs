using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TreasureDiggingRecord : BaseUIForm
{
    public CircularScrollView.UICircularScrollView m_RecordVerticalScroll;
    public CircularScrollView.UICircularScrollView m_RewardVerticalScroll;

    public Toggle m_Toggle_Record;
    public Toggle m_Toggle_Reward;
    public Text m_Toggle_Record_Txt;
    public Text m_Toggle_Reward_Txt;
    public GameObject m_NoneRecord;

    private List<TreasureRecord> m_RecordListData = new List<TreasureRecord>();
    public enum PageType
    {
        Record = 1,
        Reward = 2,
    }
    private PageType m_PageType;

    public void Awake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        m_NoneRecord.SetActive(false);
        m_Toggle_Record.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Record;
                SetPageToggleTextColor(m_PageType);
                m_RecordVerticalScroll.gameObject.SetActive(true);
                m_RewardVerticalScroll.gameObject.SetActive(false);
                ShowRecordList();
            }
        });

        m_Toggle_Reward.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                m_PageType = PageType.Reward;
                SetPageToggleTextColor(m_PageType);
                m_RecordVerticalScroll.gameObject.SetActive(false);
                m_RewardVerticalScroll.gameObject.SetActive(true);
                m_NoneRecord.SetActive(false);
                ShowRewardList();
            }
        });

        RigisterButtonObjectEvent("BtnClose", p =>
        {
            CloseUIForm();
        });
    }

    private void SetPageToggleTextColor(PageType pageType)
    {
        switch (pageType)
        {
            case PageType.Record:
                if (m_Toggle_Record_Txt != null)
                    m_Toggle_Record_Txt.color = new Color(10f / 255f, 40f / 255f, 61f / 255f, 255f / 255f);
                if (m_Toggle_Reward_Txt != null)
                    m_Toggle_Reward_Txt.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 229f / 255f);
                break;
            case PageType.Reward:
                if (m_Toggle_Record_Txt != null)
                    m_Toggle_Record_Txt.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 229f / 255f);
                if (m_Toggle_Reward_Txt != null)
                    m_Toggle_Reward_Txt.color = new Color(10f / 255f, 40f / 255f, 61f / 255f, 255f / 255f);
                break;
        }
    }

    public void ShowRecordList()
    {
        MessageManager.GetInstance().SendTreasureRecordReq(1, 100, ManageMentClass.DataManagerClass.userId, (p) =>
        {
            m_RecordListData = p.List.ToList();
            SortRecordList();
            SetRecordListInfo();
        });
    }

    public void SortRecordList()
    {
        if (m_RecordListData == null)
            return;
        //获取时间降序
        m_RecordListData.Sort((x, y) => -x.CreatedAt.CompareTo(y.CreatedAt));
    }

    public void ShowRewardList()
    {
        SetRewardListInfo();
    }


    public void SetRecordListInfo()
    {
        if (m_RecordListData == null)
        {
            m_NoneRecord.SetActive(true);
            return;
        }
        int count = m_RecordListData.Count;
        m_RecordVerticalScroll.Init(InitRecordListInfoCallBack);
        m_RecordVerticalScroll.ShowList(count);
        m_RecordVerticalScroll.ResetScrollRect();
        m_NoneRecord.SetActive(count <= 0);
    }

    public void InitRecordListInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        cell.gameObject.SetActive(true);
        RecordItem recordItem = cell.transform.GetComponent<RecordItem>();
        if (recordItem != null)
        {
            TreasureRecord data = m_RecordListData[index - 1];
            if (data == null)
                return;
            recordItem.SetDesc(data.UserName, data.RewardName, data.RewardQuantity);
            recordItem.SetTime(data.CreatedAt);
        }
    }

    public void SetRewardListInfo()
    {
        int count = TreasureModel.Instance.RewardPreviewData.Count;
        m_RewardVerticalScroll.Init(InitRewardListInfoCallBack);
        m_RewardVerticalScroll.ShowList(count);
        m_RewardVerticalScroll.ResetScrollRect();
    }

    public void InitRewardListInfoCallBack(GameObject cell, int index)
    {
        if (cell == null)
        {
            return;
        }
        cell.SetActive(true);
        RewardItem rewardItem = cell.transform.GetComponent<RewardItem>();
        if (rewardItem != null)
        {
            TreasureRewardConfData data = TreasureModel.Instance.RewardPreviewData[index - 1];
            if (data == null)
                return;
            rewardItem.SetName(data.RewardName, data.RewardQuantity);
            rewardItem.SetIcon(data.RewardIcon);
            rewardItem.SetRate(data.RewardId, TreasureModel.Instance.RewardPreviewData);
            rewardItem.PlayStarAni();
        }
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);

        SetDefaultSelectPage();
        ShowRecordList();
    }

    public void SetDefaultSelectPage()
    {
        m_PageType = PageType.Record;
        m_Toggle_Record.isOn = true;
        m_Toggle_Reward.isOn = false;
        SetPageToggleTextColor(m_PageType);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        SetDefaultSelectPage();
    }

    public Dictionary<int, treasure_reward> GetRewardList()
    {
        treasure_rewardContainer treasureRewardContainer = BinaryDataMgr.Instance.LoadTableById<treasure_rewardContainer>("treasure_rewardContainer");
        if (treasureRewardContainer != null)
        {
            return treasureRewardContainer.dataDic;
        }
        return null;
    }

    public PageType GetPageType()
    {
        return m_PageType;
    }
}
