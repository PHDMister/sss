using System;
using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityTimer;

//海底宝箱
public class SeabedRewardBox : MonoBehaviour
{
    public const string Event_PlayerChanged = "SeabedRewardBox_InsidePlayerChanged";
    public static Dictionary<int, SeabedRewardBox> DicBoxs = new Dictionary<int, SeabedRewardBox>();
    public static SeabedRewardBox GetBox(int index)
    {
        if (DicBoxs.TryGetValue(index, out var box))
        {
            return box;
        }
        return null;
    }



    public int BoxIndex;
    public float Radius;
    public GameObject Box;
    [Range(0.1f, 2f)]
    public float TickInterval = 0.2f;
    protected float TickTemp;
    private List<ulong> curPlayerList = new List<ulong>(1);
    private List<ulong> tmpPlayerList = new List<ulong>(1);
    protected bool IsSreach = true;

    //判断是否可以打开宝箱
    protected void Awake()
    {
        if (BoxIndex == 0)
        {
            string[] indexStrings = gameObject.name.Split('_');
            BoxIndex = int.Parse(indexStrings[1]);
        }
        DicBoxs[BoxIndex] = this;
    }
    protected void OnDestroy()
    {
        DicBoxs.Remove(BoxIndex);
    }

    //播放宝箱打开动画
    protected void Update()
    {
        //搜索
        if (IsSreach)
        {
            TickTemp += Time.deltaTime;
            if (TickTemp > TickInterval)
            {
                TickTemp = 0;
                SreachPlayerInside();
            }
        }
    }
    //检测目标是否在范围内
    protected void SreachPlayerInside()
    {
        tmpPlayerList.Clear();
        ulong userId = ManageMentClass.DataManagerClass.userId;
        GameObject go = CharacterManager.Instance().GetPlayerObj();
        if (go == null) return;
        if (Include(go.transform.position))
        {
            tmpPlayerList.Add(userId);
        }
        if (tmpPlayerList.Count >= 1 && curPlayerList.Count == 0)
        {
            curPlayerList.Add(userId);
            MessageCenter.SendMessage(Event_PlayerChanged, "enter", BoxIndex);
        }
        else if (tmpPlayerList.Count == 0 && curPlayerList.Count >= 1)
        {
            curPlayerList.Clear();
            MessageCenter.SendMessage(Event_PlayerChanged, "leave", BoxIndex);
        }
    }


    public bool Include(Vector3 pos)
    {
        Vector3 temp = Box.transform.position;
        temp.y = 0;
        pos.y = 0;
        float len = Vector3.Distance(Box.transform.position, pos);
        return len < Radius;
    }

    public void OpenBox(Action callback)
    {
        IsSreach = false;
        Box.gameObject.SetActive(true);
        Singleton<RainbowSeabedController>.Instance.SetMoveNormal(false);
        //播放宝箱动画
        Animator animator = Box.GetComponent<Animator>();
        float boxTime = animator.GetAnimLength("Open");
        animator.Play("Open", 0, 0);
        Timer.RegisterRealTimeNoLoop(boxTime, () =>
        {
            Singleton<RainbowSeabedController>.Instance.SetMoveNormal(true);
            //隐藏宝箱
            HideBox();
            //奖励回调
            callback?.Invoke();
        });
    }
    //显示宝箱
    public void ShowBox()
    {
        IsSreach = true;
        Box.gameObject.SetActive(true);
        curPlayerList.Clear();
        tmpPlayerList.Clear();
    }
    //隐藏宝箱
    public void HideBox()
    {
        IsSreach = false;
        Animator animator = Box.GetComponent<Animator>();
        animator.Play("Close", 0, 1);
        animator.Update(Time.deltaTime);
        Box.gameObject.SetActive(false);
        curPlayerList.Clear();
        tmpPlayerList.Clear();
    }



#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosX.DrawWireDisc(transform.position, transform.up, Radius);
    }
#endif
}
