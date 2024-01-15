using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//控制拖尾粒子UV流动
public class UVscroll : MonoBehaviour
{
    public int materialId = 0;
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        rend.materials[materialId].SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }

}
