using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProtoTestTool : Editor
{
    [MenuItem("Tools/ProxyTestTool")]
    static void OpenTestTool()
    {
        ProxyTools pt = FindObjectOfType<ProxyTools>();
        if (pt) return;
        GameObject go = new GameObject("[ProxyTestTool]");
        go.AddComponent<ProxyTools>();
    }

}
