using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class JoyStick : MonoBehaviour
{
    //��㴥�ع�����
    public TouchEvent touchEvent;

    //ҡ��λ���Ƿ�̶�
    public bool isFixedArea = false;
    //�Ƿ���ʾҡ���ڷ�Χ��
    public bool isInsideArea = true;
    //�Ƿ���תҡ�˵Ĵ�Ȧ
    public bool isNeedPointDirect = true;
    //�м��ҡ�˾����Ե��ƫ��ֵ
    public float InsideAreaOffset = -25;
    //���Դ��صķ�Χ
    private RectTransform touchArea;
    //
    public RectTransform touchAreaRect;
    //dead state
    public RectTransform deadRect;
    //ҡ�˵Ĵ�Ȧ
    public RectTransform circle;

    //ҡ�˵�СȦ
    public RectTransform point;

    //ҡ�������ĵ�������
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
    // ��Բ�̵�λ�ã�СԲ�̵�λ��
    public event Action<Vector3, Vector3, float> onDown, onDrag, onUp;
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (isFixedArea)
        {
            // ����̶�λ�ã���ѿɴ���������Ϊ��Ȧ������
            //touchArea = circle;
            touchArea = touchAreaRect;
        }
        else
        {
            touchArea = this.GetComponent<RectTransform>();
        }
        InterfaceHelper.JoySticktouchArea = touchArea;
        //�����Ȧ����С�뾶��ΪСȦ������ƶ�����
        maxOffset = Mathf.Min(touchArea.rect.width / 2, touchArea.rect.height / 2);
        touchEvent.AddJoy(this);
        pointSizeHalf = new Vector3(point.rect.width * 0.5f, point.rect.y * 0.5f, 0);
        pointRangeRect = new Rect(touchArea.rect.center, touchArea.rect.size - point.rect.size / 2);
        if (isHasDeadState) SetCircleState(false);
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
            //��������ָ���µ��¼�
            // circle.position = touch.position;
            SetCircleState(true);
            onDown?.Invoke(touchArea.position, positionA, maxOffset);
        }
        else if (touch == TouchState.move)
        {
            //��¼֮ǰ��λ��
            Vector3 prePointPos = point.position;
            //��¼ƫ����
            Vector3 offset = new Vector3(positionA.x, positionA.y, 0) - prePointPos;
            //СȦ����һ���ƶ�
            point.position += offset;
            //����ҡ�˵��ƶ�
            if ((point.position - touchArea.position).magnitude > maxOffset)
            {
                point.position = touchArea.position + (point.position - touchArea.position).normalized * maxOffset;
            }
            //�����һ��ƫ����
            xAxis = offset.x / maxOffset;
            yAxis = offset.y / maxOffset;
            //����
            if (isInsideArea)
            {
                Vector3 dir = point.position - touchArea.position;
                float dis = Vector3.Distance(point.position, touchArea.position);
                dis = Mathf.Min(dis + InsideAreaOffset, pointRangeRect.width * 0.5f);
                point.position = touchArea.position + dir.normalized * dis;
            }
            //��ת
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
            //��ָ̧��
            //����СȦ��λ�õ���Ȧ������
            point.position = touchArea.position;
            circle.rotation = Quaternion.identity;
            point.rotation = Quaternion.identity;


            SetCircleState(false);
            //������ָ̧���¼�
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
            //��������ָ���µ��¼�
            // circle.position = touch.position;
            onDown.Invoke(circle.position, positionA, maxOffset);
        }
        else if (Input.GetMouseButton(0))
        {
            //��¼֮ǰ��λ��
            Vector3 prePointPos = point.position;
            //��¼ƫ����
            Vector3 offset = new Vector3(positionA.x, positionA.y, 0) - prePointPos;
            //СȦ����һ���ƶ�
            point.position += offset;
            //����ҡ�˵��ƶ�
            if ((point.position - circle.position).magnitude > maxOffset)
            {
                point.position = circle.position + (point.position - circle.position).normalized * maxOffset;
            }
            //�����һ��ƫ����
            xAxis = offset.x / maxOffset;
            yAxis = offset.y / maxOffset;
            onDrag.Invoke(circle.position, point.position, maxOffset);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //��ָ̧��
            //����СȦ��λ�õ���Ȧ������
            point.position = circle.position;
            //������ָ̧���¼�
            onUp.Invoke(circle.position, point.position, maxOffset);
        }
    }
    /// <summary>
    /// �ж�ĳһ���Ƿ��ڿɴ��ط�Χ�����Σ��ڣ���TouchEvent����
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