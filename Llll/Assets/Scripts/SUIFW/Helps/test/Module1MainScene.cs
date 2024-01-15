using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Module1MainScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("1");
        Canvas canvs = GameObject.FindObjectOfType<Canvas>();
        Debug.Log("2");

        Button btn = canvs.transform.Find("Button").GetComponent<Button>();
        Debug.Log("3");

        btn.onClick.AddListener(OnClickBtn);
        Debug.Log("4");


  

            

    }



    private void OnClickBtn()
    {
        Debug.Log("click");
        ModuleMgr.GetInstance().SwitchModuleAsync("module2");

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
