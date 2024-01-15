using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLink
{
    //彩虹集市 (测试链接)
    public string RainbowBazaar_Test = "http://24nyae.cn/RainbowExchange/index.html?jumpTab=2";

    //彩虹集市 (正式链接)
    public string RainbowBazaar_Official = "http://www.hotdogapp.cn/RainbowExchange_Pro/index.html?jumpTab=2";





    /// <summary>
    /// 获取彩虹集市的链接
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
