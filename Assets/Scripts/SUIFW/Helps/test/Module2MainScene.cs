using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityEngine.UI;


public class Module2MainScene : MonoBehaviour
{
    Sprite btnCenterTown;
    Sprite btnLandList;
    GameObject image;
    private int status = 0;

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Button btn = canvas.transform.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(OnClickBtn);

        //测试读取ab内asset加载后存入ht缓存
        GameObject testPanel = ResourcesMgr.GetInstance().LoadAssetInBundle("module2.assetbundle","TestPanel", true);
        Destroy(testPanel);

        //测试从缓存读取
        GameObject testPanelCache = ResourcesMgr.GetInstance().LoadAssetInBundle("module2.assetbundle", "TestPanel", true);

        //测试读取ab内图集
        btnCenterTown = ResourcesMgr.GetInstance().LoadSpriteInBundle("module2.assetbundle", "module2Atlas1", "btnCenterTown");
        btnLandList = ResourcesMgr.GetInstance().LoadSpriteInBundle("module2.assetbundle", "module2Atlas1", "btnLandList");

        testPanelCache.transform.SetParent(canvas.transform);
        testPanelCache.transform.localPosition = new Vector3(0, -200, 0);

        image = testPanelCache.transform.Find("Image").gameObject;
        Button imageBtn = image.GetComponent<Button>();
        imageBtn.onClick.AddListener(OnClickImage);




    }


    private void OnClickImage()
    {
        if (status == 0)
        {
            image.GetComponent<Image>().sprite = btnLandList;
            status = 1;
        }
        else if (status == 1)
        {
            image.GetComponent<Image>().sprite = btnCenterTown;
            status = 0;
        }
    }


    private void OnClickBtn()
    {
        Debug.Log("click");
        ModuleMgr.GetInstance().SwitchModuleAsync("module1");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

