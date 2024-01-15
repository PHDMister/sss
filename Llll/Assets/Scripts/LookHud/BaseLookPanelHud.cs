using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 3d UI  HUD  挂在有Canvas组件的节点上
/// </summary>
[RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
public class BaseLookPanelHud : MonoBehaviour
{
    public float lookSpeep = 60;
    public Camera Main;

    protected virtual void Awake()
    {
        Main = Camera.main;
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Main;
    }

    protected virtual void Update()
    {
        Vector3 offset = Main.transform.position - transform.position;
        offset.y = 0;
        Quaternion origiRota = Quaternion.LookRotation(offset);
        transform.rotation = Quaternion.Slerp(transform.rotation, origiRota, lookSpeep * Time.deltaTime);
    }
}
