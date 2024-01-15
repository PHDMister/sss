using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class JoyStick : MonoBehaviour
{
    //多点触控管理类
    public TouchEvent touchEvent;

    //摇杆位置是否固定
    public bool isFixedArea = false;
    //是否显示摇杆在范围内
    public bool isInsideArea = true;
    //是否旋转摇杆的大圈
    public bool isNeedPointDirect = true;
    //中间的摇杆距离边缘的偏移值
    public float InsideAreaOffset = -25;
    //可以触控的范围
    private RectTransform touchArea;
    //
    public RectTransform touchAreaRect;
    //dead state
    public RectTransform deadRect;
    //摇杆的大圈
    public RectTransform circle;

    //摇杆的小圈
    public RectTransform point;

    //摇杆离中心的最大距离
    private float maxOffset;
    public float MaxOffset { get => maxOffset; }
    public float xAxis = 0;
    public float yAxis = 0;

    private bool isHasDeadState = false;
    private Vector3 pointSizeHalf;
    private Rect pointRangeRect;
    // public UnityEvent onDown, onDrag, onUp;

    public void Awake()
    {
        touchEvent = Camera.main.transform.GetComponent<TouchEvent>();
        isHasDeadState = deadRect != null;
    }
    // 大圆盘的位置，小圆盘的位置
    public event Action<Vector3, Vector3, float> onDown, onDrag, onUp;
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (isFixedArea)
        {
            // 如果固定位置，则把可触控区域设为大圈的区域
            //touchArea = circle;
            touchArea = touchAreaRect;
        }
        else
        {
            touchArea = this.GetComponent<RectTransform>();
        }
        InterfaceHelper.JoySticktouchArea = touchArea;
        //计算大圈的最小半径作为小圈的最大移动距离
        maxOffset = Mathf.Min(touchArea.rect.width / 2, touchArea.rect.height / 2);
        touchEvent.AddJoy(this);
        pointSizeHalf = new Vector3(point.rect.width * 0.5f, point.rect.y * 0.5f, 0);
        pointRangeRect = new Rect(touchArea.rect.center, touchArea.rect.size - point.rect.size / 2);
        if (isHasDeadState) SetCircleState(false);
        MoveControllerImp.JoyStickOffset = maxOffset;
    }

    public void SetCircleState(bool isShow)
    {
        if (isHasDeadState)
        {
            if (circle.gameObject.activeSelf != isShow) circle.gameObject.SetActive(isShow);
            if (point.gameObject.activeSelf != isShow) point.gameObject.SetActive(isShow);
            if (point.gameObject.activeSelf == isShow) deadRect.gameObject.SetActive(!isShow);
        }
        else
        {
            //circle.gameObject.SetActive(isShow);
            //point.gameObject.SetActive(isShow);
            if (point.gameObject.activeSelf) deadRect.gameObject.SetActive(false);
        }
    }

    public void ReadTouch(TouchState touch, Vector3 positionA)
    {
        if (!transform.gameObject.activeSelf)
        {
            return;
        }
        if (touch == TouchState.down)
        {
            //触发的手指按下的事件
            // circle.position = touch.position;
            SetCircleState(true);
            onDown?.Invoke(touchArea.position, positionA, maxOffset);
        }
        else if (touch == TouchState.move)
        {
            //记录之前的位置
            Vector3 prePointPos = point.position;
            //记录偏移量
            Vector3 offset = new Vector3(positionA.x, positionA.y, 0) - prePointPos;
            //小圈跟着一起移动
            point.position += offset;
            //限制摇杆的移动
            if ((point.position - touchArea.position).magnitude > maxOffset)
            {
                point.position = touchArea.position + (point.position - touchArea.position).normalized * maxOffset;
            }
            //计算归一化偏移量
            xAxis = offset.x / maxOffset;
            yAxis = offset.y / maxOffset;
            //限制
            if (isInsideArea)
            {
                Vector3 dir = point.position - touchArea.position;
                float dis = Vector3.Distance(point.position, touchArea.position);
                dis = Mathf.Min(dis + InsideAreaOffset, pointRangeRect.width * 0.5f);
                point.position = touchArea.position + dir.normalized * dis;
            }
            //旋转
            if (isNeedPointDirect)
            {
                Vector3 dir = point.position - touchArea.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                angle -= 90;
                circle.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                point.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            onDrag?.Invoke(touchArea.position, point.position, maxOffset);
        }
        else if (touch == TouchState.up)
        {
            //手指抬起
            //重置小圈的位置到大圈的中心
            point.position = touchArea.position;
            circle.rotation = Quaternion.identity;
            point.rotation = Quaternion.identity;


            SetCircleState(false);
            //触发手指抬起事件
            onUp?.Invoke(touchArea.position, point.position, maxOffset);
        }
    }

    public void ReadTouchEditor(Vector3 positionA)
    {
        if (!transform.gameObject.activeSelf)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //触发的手指按下的事件
            // circle.position = touch.position;
            onDown.Invoke(circle.position, positionA, maxOffset);
        }
        else if (Input.GetMouseButton(0))
        {
            //记录之前的位置
            Vector3 prePointPos = point.position;
            //记录偏移量
            Vector3 offset = new Vector3(positionA.x, positionA.y, 0) - prePointPos;
            //小圈跟着一起移动
            point.position += offset;
            //限制摇杆的移动
            if ((point.position - circle.position).magnitude > maxOffset)
            {
                point.position = circle.position + (point.position - circle.position).normalized * maxOffset;
            }
            //计算归一化偏移量
            xAxis = offset.x / maxOffset;
            yAxis = offset.y / maxOffset;
            onDrag.Invoke(circle.position, point.position, maxOffset);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //手指抬起
            //重置小圈的位置到大圈的中心
            point.position = circle.position;
            //触发手指抬起事件
            onUp.Invoke(circle.position, point.position, maxOffset);
        }
    }
    /// <summary>
    /// 判断某一点是否在可触控范围（矩形）内，由TouchEvent调用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool inTouchArea(Vector2 pos)
    {
        Rect rect = touchArea.rect;
        rect.x += touchArea.position.x;
        rect.y += touchArea.position.y;
        return rect.Contains(pos);
    }
    public void OnDownTest() { print("down"); }
    public void OnDragTest() { print("drag"); }
    public void OnUpTest() { print("up"); }
}