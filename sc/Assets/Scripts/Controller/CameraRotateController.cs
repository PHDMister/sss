using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRotateController : MonoBehaviour
{
    //��㴥�ع�����
    public TouchEvent touchEvent;
    //�Ƿ���Կ���
    public event Action<Vector3> onDown, onDrag, onUp;
    //λ��
    public Vector3 lastPointPos;
    //���Դ��صķ�Χ
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
    /// �ж�ĳһ���Ƿ��ڿɴ��ط�Χ�����Σ��ڣ���TouchEvent����
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
            //��������ָ���µ��¼�
            onDown.Invoke(touchPosition);
        }
        else if (touch == TouchState.move)
        {

            //��¼֮ǰ��λ��
            Vector3 prePointPos = lastPointPos;
            //��¼ƫ����
            Vector3 offset = touchPosition - prePointPos;
            //СȦ����һ���ƶ�
            lastPointPos += offset;

            /*//����ҡ�˵��ƶ�
            if ((point.position - circle.position).magnitude > maxOffset)
            {
                point.position = circle.position + (point.position - circle.position).normalized * maxOffset;
            }
            */
            onDrag.Invoke(offset);
        }
        else if (touch == TouchState.up)
        {
            //��ָ̧��
            //������ָ̧���¼�
            onUp.Invoke(touchPosition);
        }
    }
}
