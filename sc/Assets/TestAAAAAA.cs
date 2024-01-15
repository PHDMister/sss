using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

public class TestAAAAAA : MonoBehaviour
{
    public Text TextA;
    public Button button;
    // Start is called before the first frame update
    public void AAAAAAFun()
    {
        if (TextA.text != "")
        {
            SelfConfigData.selfNumber = TextA.text;
        }
        SetTools.ChangeSystemStatusBar(false);
        Singleton<GameCommonMgr>.Instance.Init();
        UIManager.GetInstance().ShowUIForms(FormConst.LOGINLOADINGPANEL);
    }
}
