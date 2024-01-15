using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelShaderCtrl : MonoBehaviour
{
    // Update is called once per frame

    /// <summary>
    /// 钓鱼场景
    /// </summary>
    void Update()
    {
        
        if (CharacterManager.Instance().GetPlayerObj() != null)
        {
            Shader.SetGlobalVector("_playPos", CharacterManager.Instance().GetPlayerObj().transform.position);
        }
    }
}
