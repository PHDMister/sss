using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.UI;

public class LoginGameStart : MonoBehaviour
{
    //在这里打开登录面板
    private void Awake()
    {
       /* SetTools.ChangeSystemStatusBar(false);
        Singleton<GameCommonMgr>.Instance.Init();*/
        UIManager.GetInstance().ShowUIForms(FormConst.LOGINLOADINGPANEL);
    }

}
