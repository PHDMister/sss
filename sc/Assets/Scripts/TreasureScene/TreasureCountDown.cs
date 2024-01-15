using System.Collections;
using System.Collections.Generic;
using Treasure;
using UnityEngine;
using UnityEngine.UI;

public class TreasureCountDown : MonoBehaviour
{
    public static int CtTime = 180;
    public static int CtTime1 = 144;

    public Text Text;
    private float RtTime;
    private int endTime;

    protected void Awake()
    {
        if (!Text) Text = GetComponent<Text>();
        Text.text = "";
        CtTime = (int)TreasureModel.Instance.TreasureEfficiencyTime;
        float buffFact = (100 - TreasureModel.Instance.TreasureEfficiencyBuff) * 0.01f;
        CtTime1 = Mathf.FloorToInt(CtTime * buffFact);
    }

    public void SetEndTime(int partnerCount)
    {
        int time = partnerCount >= 2 ? CtTime1 : CtTime;
        RoomUserInfo userInfo = Singleton<TreasuringController>.Instance.GetSelfUserInfo();
        if (userInfo.Start > 0) endTime = (int)userInfo.Start + time;
        else endTime = TreasureModel.Instance.CurTime + time;
        //Debug.Log($"1111111111111   TreasureCountDown   Start:{userInfo.Start}  curTime:{TreasureModel.Instance.CurTime}  endTime:{endTime} ");
        RtTime = 1;
        Text.text = CaluTime();
    }
    public void SetEndTimes(int etime)
    {
        this.endTime = etime;
    }
    public void ResetEndTimeOnLeavePartner(int partnerCount)
    {
        if (endTime == 0) SetEndTime(partnerCount);
        //Debug.Log($"1111111111111   ResetEndTimeOnLeavePartner   EndTime:{endTime}   ");
        int squ = endTime - TreasureModel.Instance.CurTime;
        //Debug.Log($"1111111111111   ResetEndTimeOnLeavePartner   EndTime:{endTime}  curTime:{TreasureModel.Instance.CurTime}  squTime:{squ} ");
        endTime = endTime + Mathf.CeilToInt(squ * 0.25f);
        //Debug.Log($"1111111111111   ResetEndTimeOnLeavePartner  reset last  EndTime:{endTime}   ");
        Text.text = CaluTime();

    }


    // Update is called once per frame
    protected void Update()
    {
        if (!Text) return;
        if (endTime <= 0) return;
        RtTime -= Time.deltaTime;
        if (RtTime <= 0)
        {
            RtTime = 1;
            Text.text = CaluTime();
        }
    }

    protected string CaluTime()
    {
        int residue = endTime - TreasureModel.Instance.CurTime;
        int minute = residue / 60;
        int second = residue % 60;
        if (residue <= 0) endTime = 0;
        return $"{minute:00}:{second:00}";
    }

}
