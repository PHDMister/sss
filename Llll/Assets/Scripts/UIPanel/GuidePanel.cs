using UnityEngine;
using UnityEngine.UI;
using UIFW;
using System.Collections.Generic;
using Treasure;

public class GuidePanel : BaseUIForm
{
    public Button m_BtnConfirm;
    public Text m_TxtBtn;
    public Button m_BtnLeft;
    public Button m_BtnRight;
    public List<Image> m_PointList;
    public ScrollRectCenter m_ScrollRectCenter;


    private int seconds;
    private float timer;
    private int m_CurCenterIdx = 0;

    private void Awake()
    {
        OnAwake();
        AddEvent();
    }

    private void AddEvent()
    {
        m_BtnConfirm.onClick.AddListener(OnBtnConfirmClicked);
    }

    private void OnBtnConfirmClicked()
    {
        BeginLaunchCompleteReq req = new BeginLaunchCompleteReq();
        req.UserId = ManageMentClass.DataManagerClass.userId;

        WebSocketAgent.SendMsg((uint)MessageId.Types.Enum.BeginLaunchCompleteReq, req, (code, bytes) =>
        {
            if (code != 0) return;
            BeginLaunchCompleteResp resp = BeginLaunchCompleteResp.Parser.ParseFrom(bytes);
            if (resp.StatusCode == 0)
            {
                CloseUIForm();
            }
        });
    }

    protected virtual void OnAwake()
    {
        //窗体的性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        ReceiveMessage("DragScrollViewEnd", p =>
        {
            int curIndex = (int)p.Values;
            m_CurCenterIdx = curIndex;
            SetPointState(curIndex);
        });

        m_BtnLeft.onClick.AddListener(OnClickLeftArrow);
        m_BtnRight.onClick.AddListener(OnClickRightArrow);
    }

    public override void Display()
    {
        base.Display();
        InterfaceHelper.SetJoyStickState(false);
        seconds = 3;
        SetBtnState(seconds);
        SetPointState(m_CurCenterIdx);
    }

    public override void Hiding()
    {
        base.Hiding();
        InterfaceHelper.SetJoyStickState(true);
        //MessageCenter.SendMessage("OpenChatUI", KeyValuesUpdate.Empty);
    }

    private void Update()
    {
        if (seconds > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                seconds -= 1;
                timer = 0;
                SetBtnState(seconds);
            }
        }
    }

    public void OnClickLeftArrow()
    {
        m_CurCenterIdx--;
        if (m_CurCenterIdx <= 0)
            m_CurCenterIdx = 0;
        m_ScrollRectCenter.MoveScrollRectByIndex(m_CurCenterIdx, m_PointList.Count);
        SetPointState(m_CurCenterIdx);
    }

    public void OnClickRightArrow()
    {
        m_CurCenterIdx++;
        if (m_CurCenterIdx >= m_PointList.Count - 1)
            m_CurCenterIdx = m_PointList.Count - 1;
        m_ScrollRectCenter.MoveScrollRectByIndex(m_CurCenterIdx, m_PointList.Count);
        SetPointState(m_CurCenterIdx);
    }

    public void SetBtnState(int second)
    {
        if (second > 0)
        {
            SetIcon(m_BtnConfirm.GetComponent<Image>(), "Guide", "di03_disable");
            m_BtnConfirm.enabled = false;
            m_TxtBtn.text = string.Format("我知道了({0}s)", seconds);
        }
        else
        {
            SetIcon(m_BtnConfirm.GetComponent<Image>(), "Guide", "di03");
            m_BtnConfirm.enabled = true;
            m_TxtBtn.text = string.Format("{0}", "我知道了");
        }
    }

    public void SetPointState(int curIdx)
    {
        if (m_PointList == null)
            return;
        for (int i = 0; i < m_PointList.Count; i++)
        {
            if (i == curIdx)
            {
                m_PointList[i].sprite = ResourcesMgr.GetInstance().LoadSprrite("Guide", "PointLight");
                m_PointList[i].SetNativeSize();
            }
            else
            {
                m_PointList[i].sprite = ResourcesMgr.GetInstance().LoadSprrite("Guide", "PointNormal");
                m_PointList[i].SetNativeSize();
            }
        }
        SetArrowBtnState(curIdx);
    }

    public void SetArrowBtnState(int index)
    {
        if (m_PointList.Count == 1)
        {
            m_BtnLeft.gameObject.SetActive(false);
            m_BtnRight.gameObject.SetActive(false);
        }
        else
        {
            if (index <= 0)
            {
                m_BtnLeft.gameObject.SetActive(false);
                m_BtnRight.gameObject.SetActive(true);
            }
            else if (index > 0 && index < m_PointList.Count - 1)
            {
                m_BtnLeft.gameObject.SetActive(true);
                m_BtnRight.gameObject.SetActive(true);
            }
            else
            {
                m_BtnLeft.gameObject.SetActive(true);
                m_BtnRight.gameObject.SetActive(false);
            }
        }
    }
}
