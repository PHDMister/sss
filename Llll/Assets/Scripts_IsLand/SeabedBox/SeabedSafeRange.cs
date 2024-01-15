using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

//海底休息区的范围
public class SeabedSafeRange : MonoBehaviour
{
    public const string Event_Start = "SeabedSafeRange_Event_Start";
    public static SeabedSafeRange Ins;

    public Vector3 Center;
    public float Radius;
    [Range(0.1f, 2f)]
    public float TickInterval = 0.3f;
    protected float TickTemp;
    protected bool IsStop = false;
    protected void Awake()
    {
        Ins = this;
        transform.position = Center;

        MessageCenter.AddMsgListener(Event_Start, OnStartHandler);
    }

    private void OnStartHandler(KeyValuesUpdate kv)
    {
        TickTemp = 0;
        if (kv.Key == "start") IsStop = false;
        if (kv.Key == "pause") IsStop = true;
    }

    protected void Update()
    {
        if (IsStop) return;
        if (UIManager.GetInstance().IsOpend(FormConst.PETDENSLOADING))
            return;

        TickTemp += Time.deltaTime;
        if (TickTemp >= TickInterval)
        {
            TickTemp = 0;
            GameObject go = CharacterManager.Instance().GetPlayerObj();
            if (!Include(go.transform.position) && !Singleton<RainbowSeabedController>.Instance.HasDivingEquipment)
            {
                IsStop = true;
               Singleton<RainbowSeabedController>.Instance.PlayerTriggerTreasure();
            }
        }
    }

    protected void OnDestroy()
    {
        Ins = null;
        MessageCenter.RemoveMsgListener(Event_Start, OnStartHandler);
    }


    public bool Include(Vector3 pos)
    {
        Vector3 temp = transform.position;
        temp.y = 0;
        pos.y = 0;
        float len = Vector3.Distance(transform.position, pos);
        return len < Radius;
    }



#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosX.DrawWireDisc(transform.position, transform.up, Radius);
    }
#endif

}
