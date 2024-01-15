using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Treasure;

public enum LoadType
{
    Create,
    Join,
    TreasureEnd,
}

public class TreasureLoadManager : MonoBehaviour
{
    private Transform _TransSceneMgr = null;
    private static TreasureLoadManager _instance = null;
    private int displayProgress = 0;
    private float curProgress = 0f;
    private int toProgress = 0;

    public static TreasureLoadManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("_TreasureLoadManager").AddComponent<TreasureLoadManager>();
        }
        return _instance;
    }

    private void Awake()
    {
        _TransSceneMgr = this.gameObject.transform;
    }

    public void Load(LoadType loadType, object arg = null)
    {
        UIManager.GetInstance().ShowUIForms(FormConst.PETDENSLOADING);
        SetDesc(loadType);
        curProgress = 0f;
        displayProgress = 0;
        toProgress = 0;
        StartCoroutine(LoadScene(arg));
    }

    IEnumerator LoadScene(object arg)
    {
        while (curProgress <= 1f)
        {
            curProgress += Time.deltaTime;
            toProgress = (int)(curProgress * 100);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetProgress(displayProgress);
            }
            yield return null;
        }

        if (curProgress >= 1f)
        {
            UIManager.GetInstance().CloseUIForms(FormConst.PETDENSLOADING);
            MessageManager.GetInstance().SendMessage("TreasureLoadEnd", "Success", arg);
            yield break;
        }
    }

    public void SetProgress(int value)
    {
        PetdensLoading m_PetdensLoading = UIManager.GetInstance().GetUIForm(FormConst.PETDENSLOADING) as PetdensLoading;
        if (m_PetdensLoading != null)
        {
            m_PetdensLoading.SetProgress(value);
        }
    }

    public void SetDesc(LoadType loadType)
    {
        PetdensLoading m_PetdensLoading = UIManager.GetInstance().GetUIForm(FormConst.PETDENSLOADING) as PetdensLoading;
        if (m_PetdensLoading != null)
        {
            m_PetdensLoading.SetTips(loadType);
        }
    }
}
