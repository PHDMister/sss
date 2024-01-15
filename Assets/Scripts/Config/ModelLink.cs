using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLink
{
    //�ʺ缯�� (��������)
    public string RainbowBazaar_Test = "http://24nyae.cn/RainbowExchange/index.html?jumpTab=2";

    //�ʺ缯�� (��ʽ����)
    public string RainbowBazaar_Official = "http://www.hotdogapp.cn/RainbowExchange_Pro/index.html?jumpTab=2";





    /// <summary>
    /// ��ȡ�ʺ缯�е�����
    /// </summary>
    /// <returns></returns>
    public string GetRainBowBazaarData()
    {
        string rainbow = "";
        if (ManageMentClass.DataManagerClass.isOfficialEdition)
        {
            rainbow = ManageMentClass.ModelLinkClass.RainbowBazaar_Official;
        }
        else
        {
            rainbow = ManageMentClass.ModelLinkClass.RainbowBazaar_Test;
        }
        return rainbow + "&token=" + ManageMentClass.DataManagerClass.tokenValue_App;
    }
}
