using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextTools
{
    public static string CutOutString(string strData, int cutCount, string addStr)
    {
        string str = strData;
        if (strData != null && cutCount != 0)
        {
            int charcacterCount = strData.Length;
            if (cutCount < charcacterCount)
            {
                str = strData.Substring(0, cutCount);
            }
            else if (cutCount == charcacterCount)
            {
                str = strData;
            }
            if (addStr != "")
            {
                str += addStr;
            }
        }
        return str;
    }
    public static string setCutAddString(string strData, int cutCount, string addStr)
    {
        string str = strData;
        if (strData != null && cutCount != 0)
        {
            int charcacterCount = strData.Length;
            if (cutCount < charcacterCount)
            {
                str = strData.Substring(0, cutCount);
                if (addStr != "")
                {
                    str += addStr;
                }
            }
        }
        return str;
    }
}
