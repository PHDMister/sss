using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private Slider m_Slider;
    private Text m_Text;
    private Text tip_Text;
    private Text tip_ModuleLoad_Text;
    private float m_NomSchedule;
    private void Awake()
    {
        m_Slider = transform.Find("m_Slider").GetComponent<Slider>();
        m_Text = transform.Find("m_Text").GetComponent<Text>();
        tip_Text = transform.Find("tip_Text").GetComponent<Text>();
        tip_ModuleLoad_Text = transform.Find("tip_ModuleLoad_Text").GetComponent<Text>();
        SetSliderValueFun(0.013f);
    }
    private void OnEnable()
    {
        SetSliderValueFun(0.013f);
        SetSliderTextFun(0 * 100 + "%");
    }
    public void LoadNextLevel()
    {
        StopAllCoroutines();
        StartCoroutine(LoadLevel());
        setTipTextFun("load");
    }
    IEnumerator LoadLevel()
    {
        m_NomSchedule = 0.9f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //加载完是否自动跳转
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress > 0.013f)
            {
                SetSliderValueFun(operation.progress);
            }
            else
            {
                SetSliderValueFun(0.013f);
            }
            SetSliderTextFun(operation.progress * 100 + "%");
            if (operation.progress >= 0.9f)
            {
                if (m_NomSchedule < 1)
                {
                    m_NomSchedule += 0.1f;
                }
                else
                {
                    m_NomSchedule = 1;
                }
                SetSliderValueFun(m_NomSchedule);
                SetSliderTextFun(m_NomSchedule * 100 + "%");
                if (m_NomSchedule >= 1)
                {
                    // 在这里再成功跳转
                    operation.allowSceneActivation = true;
                    //StopCoroutine(LoadLevel());
                    StopAllCoroutines();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SetSliderValueFun(float _value)
    {
        m_Slider.value = _value;
    }
    public void SetSliderTextFun(string _Text)
    {
        m_Text.text = _Text;
    }
    public void setTipTextFun(string type)
    {
        switch (type)
        {
            case "login":
                tip_Text.text = "正在登录游戏...";
                break;
            case "down":
                tip_Text.text = "正在下载资源...";
                break;
            case "load":
                tip_Text.text = "正在加载场景...";
                break;
            default:
                break;
        }
    }

    public void SetModuleLoadTipFun(string bundleName)
    {
        tip_ModuleLoad_Text.text = bundleName;
    }
}
