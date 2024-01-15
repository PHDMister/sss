using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotateController : MonoBehaviour
{
    //多点触控管理类
    public TouchEvent touchEvent;
    //是否可以控制
    public event Action<Vector3> onDown, onDrag, onUp;
    //位置
    public Vector3 lastPointPos;
    //可以触控的范围
    public RectTransform touchArea;
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        touchEvent.AddCameraRoate(this);
    }
    /// <summary>
    /// 判断某一点是否在可触控范围（矩形）内，由TouchEvent调用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool inTouchArea(Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(touchArea, pos);
    }
    public void ReadTouch(TouchState touch, Vector3 touchPosition)
    {
        if (!transform.gameObject.activeSelf)
        {
            return;
        }
        if (touch == TouchState.down)
        {
            lastPointPos = touchPosition;
            //触发的手指按下的事件
            onDown.Invoke(touchPosition);
        }
        else if (touch == TouchState.move)
        {

            //记录之前的位置
            Vector3 prePointPos = lastPointPos;
            //记录偏移量
            Vector3 offset = touchPosition - prePointPos;
            //小圈跟着一起移动
            lastPointPos += offset;

            /*//限制摇杆的移动
            if ((point.position - circle.position).magnitude > maxOffset)
            {
                point.position = circle.position + (point.position - circle.position).normalized * maxOffset;
            }
            */
            onDrag.Invoke(offset);
        }
        else if (touch == TouchState.up)
        {
            //手指抬起
            //触发手指抬起事件
            onUp.Invoke(touchPosition);
        }
    }
}
