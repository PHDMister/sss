using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.Drawing.Chart;
using UIFW;
using UnityEngine;

//挖宝的圈
public class NpcCircle : MonoBehaviour
{
    private GameObject m_NpcBubbleParent;
    private GameObject m_NpcBubble;

    public Vector3 Center => transform.position;
    [Range(0.01f, 3)] public float Radius = 1;
    [Range(0.1f, 1)] public float SensorInterval = 1f;

    private float rtTime = 0;

    protected void Awake()
    {
        GameObject m_NpcObj = GameObject.FindGameObjectWithTag("_TagNpc");
        if (m_NpcObj != null)
        {
            m_NpcBubble = UnityHelper.FindTheChildNode(m_NpcObj, "Bubble").gameObject;
            if (m_NpcBubble != null)
            {
                if (m_NpcBubble.activeSelf)
                {
                    m_NpcBubble.SetActive(false);
                }

                m_NpcBubbleParent = m_NpcBubble.transform.parent.gameObject;
                if (m_NpcBubbleParent != null)
                {
                    Canvas m_Cavans = m_NpcBubbleParent.GetComponent<Canvas>();
                    if (m_Cavans != null)
                    {
                        m_Cavans.worldCamera = Camera.main;
                    }
                }
                EventTriggerListener.Get(m_NpcBubble).onClick = (p) => OnClickBubble();
            }
        }
    }

    protected void OnDestroy()
    {

    }

    protected void Update()
    {
        //感知周围角色， 记录角色id
        rtTime += Time.deltaTime;
        if (rtTime >= SensorInterval)
        {
            rtTime = 0;
            SreachPlayerInside();
        }
    }

    //检测目标是否在范围内
    protected void SreachPlayerInside()
    {
        ulong userId = ManageMentClass.DataManagerClass.userId;
        if (Singleton<TreasuringController>.Instance.TryGetPlayerImp(userId, out var imp))
        {
            float dis = imp.Distance(transform.position);
            if (dis <= Radius)
            {
                OnNpcTriggerEnter();
            }
            else
            {
                OnNpcTriggerExit();
            }
        }
    }

    protected void OnNpcTriggerEnter()
    {
        TreasureModel.Instance.bInNpcNear = true;
        SetBubbleVisable(true);
    }

    protected void OnNpcTriggerExit()
    {
        TreasureModel.Instance.bInNpcNear = false;
        SetBubbleVisable(false);
    }

    private void SetBubbleVisable(bool bShow)
    {
        if (m_NpcBubble != null)
        {
            m_NpcBubble.SetActive(bShow);
        }
    }

    private void OnClickBubble()
    {
        MessageCenter.SendMessage("CloseChatUI", KeyValuesUpdate.Empty);
        UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGMAINMENU);
        UIManager.GetInstance().ShowUIForms(FormConst.TREASUREDIGGINGNPCTALK);
    }

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosX.DrawWireDisc(transform.position, transform.up, Radius);
    }
#endif

}