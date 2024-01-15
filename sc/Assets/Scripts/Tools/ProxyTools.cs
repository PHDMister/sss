using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class ProxyTools : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_WEBGL
    private int ClickCount = 0;
    private float ClickStartTime = 0;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(ManageMentClass.DataManagerClass.isOfficialEdition) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickCount == 0) ClickStartTime = Time.realtimeSinceStartup;
            ClickCount++;
            float intval = Time.realtimeSinceStartup - ClickStartTime;
            Debug.Log($"intval:{intval}  clickCount:{ClickCount}");
            if (intval >= 1 && intval <= 1.5f && ClickCount >= 6)
            {
                ClickStartTime = 0;
                ClickCount = 0;
                UIManager.GetInstance().ShowUIForms("WebSocketTest");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            float intval = Time.realtimeSinceStartup - ClickStartTime;
            if (intval > 1.5f)
            {
                ClickStartTime = 0;
                ClickCount = 0;
            }
        }
    }
#endif
}
