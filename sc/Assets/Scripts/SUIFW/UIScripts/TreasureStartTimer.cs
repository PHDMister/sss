using System;
using System.Collections;
using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;
using UnityTimer;

public class TreasureStartTimer : MonoBehaviour
{
    private static TreasureStartTimer _instance = null;
    public static TreasureStartTimer Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("TreasureStartTimer").AddComponent<TreasureStartTimer>();
        }
        return _instance;
    }

    private List<Material> materials;
    private int[] numArry = new int[6];
    //private bool bExchangeOpend = false;//兑换面板是否打开
    private bool bExchangeToast = false;
    private float m_Timer = 0;

    private GameObject m_WallObj;
    private Animator animator;

    public void Init()
    {
        if (m_WallObj == null)
        {
            m_WallObj = GameObject.FindGameObjectWithTag("_TagWall");
            animator = m_WallObj.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

        if (materials == null)
        {
            materials = new List<Material>();
            Transform hourNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Hour_tens");
            List<Material> hourNum2Materials = InterfaceHelper.FindChildMaterials(hourNum2Trans);
            materials.AddRange(hourNum2Materials);
            Transform hourNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Hour_ones");
            List<Material> hourNum1Materials = InterfaceHelper.FindChildMaterials(hourNum1Trans);
            materials.AddRange(hourNum1Materials);
            Transform MinNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Min_tens");
            List<Material> MinNum2Materials = InterfaceHelper.FindChildMaterials(MinNum2Trans);
            materials.AddRange(MinNum2Materials);
            Transform MinNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Min_ones");
            List<Material> MinNum1Materials = InterfaceHelper.FindChildMaterials(MinNum1Trans);
            materials.AddRange(MinNum1Materials);
            Transform secNum2Trans = UnityHelper.FindTheChildNode(m_WallObj, "Sec_tens");
            List<Material> secNum2Materials = InterfaceHelper.FindChildMaterials(secNum2Trans);
            materials.AddRange(secNum2Materials);
            Transform secNum1Trans = UnityHelper.FindTheChildNode(m_WallObj, "Sec_ones");
            List<Material> secNum1Materials = InterfaceHelper.FindChildMaterials(secNum1Trans);
            materials.AddRange(secNum1Materials);
        }

        if (TreasureModel.Instance.StartTime <= 0 && TreasureModel.Instance.EndTime <= 0)//全天开放
        {
            m_WallObj.SetActive(ManageMentClass.DataManagerClass.ticket <= 0);
            SetWallState(ManageMentClass.DataManagerClass.ticket > 0);
            TreasureEndTimer.Instance().Init();
        }
        else
        {
            long seconds = (long)TreasureModel.Instance.StartTime - CalcTools.GetTimeStamp();
            m_Timer = (float)seconds;
            if (seconds > 0)
            {
                if (TreasureModel.Instance.bSelf())
                {
                    if (animator != null)
                    {
                        animator.enabled = false;
                    }
                    m_WallObj.SetActive(true);
                    SetWallState(true);
                    Timer.RegisterRealTimeNoLoop(seconds, OnOpenTreasureComplete, OnOpenTreasureUpdate);
                }
            }
            else
            {
                if (TreasureModel.Instance.bSelf())
                {
                    if (TreasureModel.Instance.IsTreasureOpen())
                    {
                        m_WallObj.SetActive(ManageMentClass.DataManagerClass.ticket <= 0);
                        SetWallState(ManageMentClass.DataManagerClass.ticket > 0);
                        MessageManager.GetInstance().SendMessage("TreasureOpen", "Opend", null);
                        TreasureEndTimer.Instance().Init();
                    }
                    else
                    {
                        m_WallObj.SetActive(true);
                        SetWallState(true);
                    }
                }
            }
        }
    }

    void OnOpenTreasureComplete()
    {
        if (TreasureModel.Instance.bSelf())
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play("MagicWallOpen");
            }
            m_WallObj.SetActive(ManageMentClass.DataManagerClass.ticket <= 0);
            SetWallState(ManageMentClass.DataManagerClass.ticket > 0);
            MessageManager.GetInstance().SendMessage("TreasureOpen", "Opend", null);
        }
        TreasureEndTimer.Instance().Init();
    }

    void OnOpenTreasureUpdate(float time)
    {
        float second = m_Timer - time;
        ShowTimer(second);
    }

    void ShowTimer(float seconds)
    {
        int hour = (int)(seconds / 3600);
        int residue = (int)(seconds % 3600);
        int minute = residue / 60;
        int second = residue % 60;

        int hour2 = hour < 10 ? 0 : hour / 10;
        int hour1 = hour < 10 ? hour : hour % 10;
        int minute2 = minute < 10 ? 0 : minute / 10;
        int minute1 = minute < 10 ? minute : minute % 10;
        int second2 = second < 10 ? 0 : second / 10;
        int second1 = second < 10 ? second : second % 10;

        numArry[0] = hour2;
        numArry[1] = hour1;
        numArry[2] = minute2;
        numArry[3] = minute1;
        numArry[4] = second2;
        numArry[5] = second1;

        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            if (material != null)
            {
                material.SetFloat("_Number", numArry[i]);
            }
        }
    }

    private void Update()
    {
        //if (TreasureModel.Instance.bSelf())
        //{
            if (TreasureModel.Instance.bRecTicketNum)
            {
                if (TreasureModel.Instance.IsTreasureOpen() && ManageMentClass.DataManagerClass.ticket <= 0)
                {
                    m_WallObj.SetActive(true);
                    SetWallState(false);

                    ulong userId = ManageMentClass.DataManagerClass.userId;
                    if (Singleton<TreasuringController>.Instance.TryGetPlayerImp(userId, out var imp))
                    {
                        if (imp.playerItem.transform.localPosition.x > 5f && imp.playerItem.transform.localPosition.x <= 6.5f)
                        {
                            if (!bExchangeToast)
                            {
                                ToastManager.Instance.ShowNewToast("请到NPC“寻宝使者一粒喵”处兑换寻宝入场券哦~", 5f);
                                bExchangeToast = true;

                                //RoomUserInfo selfUserInfo = Singleton<TreasuringController>.Instance.GetSelfUserInfo();
                                //GameObject selfPlayerGo = CharacterManager.Instance().GetPlayerObj();
                                //if (!TreasureCircle.IsInside(selfUserInfo.CircleIndex, selfPlayerGo.transform.position))
                                //{
                                //    ToastManager.Instance.ShowNewToast("请到NPC“寻宝使者一粒喵”处兑换寻宝入场券哦~", 5f);
                                //    bExchangeToast = true;
                                //}
                            }
                        }
                        else
                        {
                            if (bExchangeToast)
                            {
                                bExchangeToast = false;
                            }
                        }
                    }
                }
                else if (TreasureModel.Instance.IsTreasureOpen() && ManageMentClass.DataManagerClass.ticket > 0)
                {
                    m_WallObj.SetActive(false);
                    SetWallState(true);
                }
            }
        //}
    }
    private void SetWallState(bool bShow)
    {
        for (int i = 0; i < m_WallObj.transform.childCount; i++)
        {
            Transform child = m_WallObj.transform.GetChild(i);
            child.gameObject.SetActive(bShow);
        }
        m_WallObj.GetComponent<MeshRenderer>().enabled = bShow;
    }
}
