using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CommonLoadingScene : MonoBehaviour
{
    private static string _nextModule;

    protected Text loadingProcessTxt;

    protected Text loadingInfoTxt;

    protected float progress;

    public static void Create(string nextModule, LoadingSceneType loadingType)
    {
        _nextModule = nextModule;

        Debug.Log("common loading scene Create...");
        //SceneManager.LoadScene("CommonLoadingScene1");

    }

    protected virtual void Awake() {
        loadingProcessTxt = GameObject.Find("Canvas/UIContainer/LoadingProcessTxt").GetComponent<Text>();
        loadingInfoTxt = GameObject.Find("Canvas/UIContainer/LoadingInfoTxt").GetComponent<Text>();
    }

    protected virtual void Start() {
        StartCoroutine(TestWait());
    }

    private IEnumerator TestWait()
    {
        yield return new WaitForSeconds(2);

        Switch();
    }

    protected void Switch()
    {
        ModuleMgr.GetInstance().SwitchModule(_nextModule);
    }

    public void UpdateProcess(float process)
    {
        Debug.Log(process);
        this.progress = process;
        loadingProcessTxt.text = Math.Floor(process).ToString() + "%";
        onProcess(process);
    }

    protected virtual void onProcess(float process) { 
        
        
    }

    public void UpdateLoadingInfo(string loadingInfo) { 
        loadingInfoTxt.text = loadingInfo; 
    }
}

public enum LoadingSceneType 
{
    CommonLoadingScene0 = 0,
    CommonLoadingScene1 = 1,
}
