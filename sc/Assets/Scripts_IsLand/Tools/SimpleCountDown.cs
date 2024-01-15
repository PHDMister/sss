using System;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

[RequireComponent(typeof(Text))]
public class SimpleCountDown : MonoBehaviour
{
    public Text Text;
    private float rtTime;
    private int endTime;
    private bool isStartCd;
    private int index;
    private Action<int> onEndAction;
    private string startExt;
    protected void Awake()
    {
        if (!Text) Text = GetComponent<Text>();
        Text.text = "";
    }

    protected void Update()
    {
        if (!isStartCd) return;
        if (!Text) return;
        if (endTime <= 0) return;
        rtTime -= Time.deltaTime;
        if (rtTime <= 0)
        {
            rtTime = 1;
            Text.text = CaluTime();
            if (endTime <= 0)
            {
                onEndAction?.Invoke(index);
                onEndAction = null;
            }
        }
    }

    protected void OnDisable()
    {
        index = -1;
        endTime = 0;
        rtTime = 1;
        Text.text = "";
        isStartCd = false;
        onEndAction = null;
        startExt = "";
    }

    public void SetEndTime(long timestamp, int index = -1, string startExt = "", Action<int> onEndAction = null)
    {
        endTime = (int)timestamp;
        isStartCd = true;
        this.index = index;
        this.onEndAction = onEndAction;
        this.startExt = startExt;
        if (!Text) Text = GetComponent<Text>();
        Text.text = CaluTime();
    }

    protected string CaluTime()
    {
        int residue = endTime - ManageMentClass.DataManagerClass.CurTime;
        int hour = residue / 3600;
        int minute = residue % 3600 / 60;
        int second = residue % 3600 % 60;
        if (residue <= 0) endTime = 0;
        if (string.IsNullOrEmpty(startExt)) return $"{hour:00}:{minute:00}:{second:00}";
        return $"{startExt}{hour:00}:{minute:00}:{second:00}";
    }

}
