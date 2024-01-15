using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
public class GestureControl : MonoBehaviour
{
    //����ϵ��
    private float scaleFactor = 0.5f;
    private float minRadius = 1f;
    private float maxRadius = 7f;

    private float minRadiusT = 1f;
    private float maxRadiusT = 10f;

    private float minRadiusB= 1f;
    private float maxRadiusB= 10f;

    //��¼��һ���ֻ�����λ���ж��û�������Ŵ�����С����
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;

    private CinemachineFreeLook m_CinemachineFreeLook;

    //�������������¼��ָ˫ָ�ı任
    bool bInAreaTouch0 = true;
    bool bInAreaTouch1 = true;
    private float timer = 0f;
    private float duration = 2f;
    //��ʼ����Ϸ��Ϣ����
    void Start()
    {
        //m_CinemachineFreeLook = GameObject.Find("CM FreeLook1").GetComponent<Cinemachine.CinemachineFreeLook>();
        m_CinemachineFreeLook = CameraManager.Instance().GetFreeLook();
    }

    void Update()
    {
        //�жϴ�������Ϊ���㴥��
        if (Input.touchCount == 1)
        {
            if (!ManageMentClass.DataManagerClass.CameraController)
            {
                ManageMentClass.DataManagerClass.CameraController = true;
            }
        }
        else if (Input.touchCount > 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
            {
                bInAreaTouch0 = InterfaceHelper.inTouchStickArea(Input.GetTouch(0).position);
                bInAreaTouch1 = InterfaceHelper.inTouchStickArea(Input.GetTouch(1).position);
                if (timer > 0)
                    timer = 0;
            }

            if (!bInAreaTouch0 &&  !bInAreaTouch1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    if (ManageMentClass.DataManagerClass.CameraController)
                    {
                        ManageMentClass.DataManagerClass.CameraController = false;
                    }
                    bool bOpen = UIManager.GetInstance().IsOpend(FormConst.SHOPITEMPRVIEW_UIFORM);
                    Debug.Log("IsOpen ItemPreview " + bOpen);
                    if (!bOpen)
                    {
                        ScaleCamera();
                    }
                }
                else
                {
                    if (!ManageMentClass.DataManagerClass.CameraController)
                    {
                        ManageMentClass.DataManagerClass.CameraController = true;
                    }
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                if (!bInAreaTouch0)
                    bInAreaTouch0 = true;
                if (!bInAreaTouch1)
                    bInAreaTouch1 = true;
                if (!ManageMentClass.DataManagerClass.CameraController)
                {
                    ManageMentClass.DataManagerClass.CameraController = true;
                }
                if (timer > 0)
                    timer = 0;
            }
        }
        else
        {
            if (!ManageMentClass.DataManagerClass.CameraController)
            {
                ManageMentClass.DataManagerClass.CameraController = true;
            }
        }
    }

    /// <summary>
    /// ������������ͷ
    /// </summary>
    private void ScaleCamera()
    {
        //�������ǰ���㴥�����λ��
        var tempPosition1 = Input.GetTouch(0).position;
        var tempPosition2 = Input.GetTouch(1).position;


        float currentTouchDistance = Vector3.Distance(tempPosition1, tempPosition2);
        float lastTouchDistance = Vector3.Distance(oldPosition1, oldPosition2);

        //�����ϴκ����˫ָ����֮��ľ�����
        //Ȼ��ȥ����������ľ���
        //distance -= (currentTouchDistance - lastTouchDistance) * scaleFactor * Time.deltaTime;
        float offset = currentTouchDistance - lastTouchDistance;
        //float newFov = m_CinemachineFreeLook.m_Lens.FieldOfView + offset;
        //�Ѿ�������ס��min��max֮��
        //newFov = Mathf.Clamp(newFov, 40f, 100f);
        //Debug.Log("ScaleCamera minDistance = " + minDistance + " maxDistance = " + maxDistance + "  offset = " + offset + "   newFov = " + newFov);

        float startR0 = m_CinemachineFreeLook.m_Orbits[0].m_Radius;
        float endR0 = m_CinemachineFreeLook.m_Orbits[0].m_Radius;
        endR0 -= offset * scaleFactor;
        endR0 = Mathf.Clamp(endR0, minRadiusT, maxRadiusT);

        float startR1 = m_CinemachineFreeLook.m_Orbits[1].m_Radius;
        float endR1 = m_CinemachineFreeLook.m_Orbits[1].m_Radius;
        endR1 -= offset * scaleFactor;
        endR1 = Mathf.Clamp(endR1, minRadius, maxRadius);

        float startR2 = m_CinemachineFreeLook.m_Orbits[2].m_Radius;
        float endR2 = m_CinemachineFreeLook.m_Orbits[2].m_Radius;
        endR2 -= offset * scaleFactor;
        endR2 = Mathf.Clamp(endR2, minRadiusB, maxRadiusB);

        if (timer < duration)
        {
            float t = timer / duration;
            if (m_CinemachineFreeLook != null)
            {
                m_CinemachineFreeLook.m_Orbits[0].m_Radius = Mathf.Lerp(startR0, endR0, t);
                m_CinemachineFreeLook.m_Orbits[1].m_Radius = Mathf.Lerp(startR1, endR1, t);
                m_CinemachineFreeLook.m_Orbits[2].m_Radius = Mathf.Lerp(startR2, endR2, t);
            }
            timer += Time.deltaTime;
        }
        else
        {
            if (m_CinemachineFreeLook != null)
            {
                m_CinemachineFreeLook.m_Orbits[0].m_Radius = endR0;
                m_CinemachineFreeLook.m_Orbits[1].m_Radius = endR1;
                m_CinemachineFreeLook.m_Orbits[2].m_Radius = endR2;
            }
        }

        oldPosition1 = tempPosition1;
        oldPosition2 = tempPosition2;
    }

    public void SetFreeLook(Cinemachine.CinemachineFreeLook freeLook)
    {
        m_CinemachineFreeLook = freeLook;
    }
}