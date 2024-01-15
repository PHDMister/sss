using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    private static ToastManager _ins;
    public static ToastManager Instance
    {
        get
        {
            return _ins;
        }
    }

    public int poolCount = 9;
    public GameObject toastFab;
    public GameObject newToastPrefab;
    public GameObject petToastPrefab;
    private Queue<FlyTip> toastPool;
    private Queue<FlyTip> newToastPool;
    private Queue<FlyTip> petToastPool;

    void Awake()
    {
        _ins = this;
        toastPool = new Queue<FlyTip>();
        newToastPool = new Queue<FlyTip>();
        petToastPool = new Queue<FlyTip>();
    }

    private FlyTip GetToast()
    {
        if (toastPool.Count > 0)
        {
            return toastPool.Dequeue();
        }
        else
        {
            FlyTip flyTip = Instantiate(toastFab).GetComponent<FlyTip>();
            flyTip.gameObject.name = "ToastPrefab";
            flyTip.transform.SetParent(transform);
            return flyTip;
        }
    }

    private FlyTip GetNewToast()
    {
        if (newToastPool.Count > 0)
        {
            return newToastPool.Dequeue();
        }
        else
        {
            FlyTip flyTip = Instantiate(newToastPrefab).GetComponent<FlyTip>();
            flyTip.gameObject.name = "NewToastPrefab";
            flyTip.transform.SetParent(transform);
            return flyTip;
        }
    }

    private FlyTip GetPetToast()
    {
        if (petToastPool.Count > 0)
        {
            return petToastPool.Dequeue();
        }
        else
        {
            FlyTip flyTip = Instantiate(petToastPrefab).GetComponent<FlyTip>();
            flyTip.gameObject.name = "PetToastPrefab";
            flyTip.transform.SetParent(transform);
            return flyTip;
        }
    }

    public void CheckToast(FlyTip flyTip)
    {
        if(flyTip.gameObject.name.Equals("ToastPrefab"))
        {
            if (toastPool.Count > poolCount)
            {
                Destroy(flyTip.gameObject);
            }
            else
            {
                toastPool.Enqueue(flyTip);
            }
        }
        else if(flyTip.gameObject.name.Equals("NewToastPrefab"))
        {
            if (newToastPool.Count > poolCount)
            {
                Destroy(flyTip.gameObject);
            }
            else
            {
                newToastPool.Enqueue(flyTip);
            }
        }
        else if(flyTip.gameObject.name.Equals("PetToastPrefab"))
        {
            if (petToastPool.Count > poolCount)
            {
                Destroy(flyTip.gameObject);
            }
            else
            {
                petToastPool.Enqueue(flyTip);
            }
        }
    }
    /// <summary>
    /// 黑底白字Toast
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public FlyTip ShowToast(string str)
    {
        return ShowToast(str, FlyTip.DURING_SHOR);
    }

    public FlyTip ShowToast(string str, float duringTime)
    {
        return ShowToast(str, Color.black, Color.white, duringTime);
    }

    public FlyTip ShowToast(string str, Color backColor, Color textColor)
    {
        return ShowToast(str, backColor, textColor, FlyTip.DURING_SHOR);
    }

    /// <summary>
    /// 新样式Toast
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public FlyTip ShowNewToast(string str)
    {
        return ShowNewToast(str, FlyTip.DURING_SHOR);
    }

    public FlyTip ShowNewToast(string str, float duringTime)
    {
        return ShowNewToast(str, Color.white, Color.black, duringTime);
    }

    public FlyTip ShowNewToast(string str, Color backColor, Color textColor)
    {
        return ShowNewToast(str, backColor, textColor, FlyTip.DURING_SHOR);

    }

    /// <summary>
    /// 宠物样式Toast
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public FlyTip ShowPetToast(string str)
    {
        return ShowPetToast(str, FlyTip.DURING_SHOR);
    }

    public FlyTip ShowPetToast(string str, float duringTime)
    {
        return ShowPetToast(str, Color.white, Color.black, duringTime);
    }

    public FlyTip ShowPetToast(string str, Color backColor, Color textColor)
    {
        return ShowPetToast(str, backColor, textColor, FlyTip.DURING_SHOR);

    }

    public FlyTip ShowToast(string str, Color backColor, Color textColor, float duringTime)
    {
        FlyTip result = GetToast();
        result.SetColor(backColor, textColor);
        result.SetText(str);
        result.SetDuring(duringTime);
        result.SetPositionAndSize(FlyTip.Position.Center);
        result.Show();
        return result;
    }


    public FlyTip ShowNewToast(string str, Color backColor, Color textColor, float duringTime)
    {
        FlyTip result = GetNewToast();
        //result.SetColor(backColor, textColor);
        result.SetText(str);
        result.SetDuring(duringTime);
        result.SetPositionAndSize(FlyTip.Position.Center);
        result.Show();
        return result;
    }

    public FlyTip ShowPetToast(string str, Color backColor, Color textColor, float duringTime)
    {
        FlyTip result = GetPetToast();
        //result.SetColor(backColor, textColor);
        result.SetText(str);
        result.SetDuring(duringTime);
        result.SetPositionAndSize(FlyTip.Position.Center);
        result.Show();
        return result;
    }

    public void ShowErrorToast(string errorStr)
    {
        httpData requestData = JsonUntity.FromJSON<httpData>(errorStr);
        if (requestData != null)
        {
            ShowToast(requestData.msg, 5f);
        }
    }

    public void ShowNewErrorToast(string errorStr)
    {
        httpData requestData = JsonUntity.FromJSON<httpData>(errorStr);
        if (requestData != null)
        {
            ShowNewToast(requestData.msg, 3f);
        }
    }
}