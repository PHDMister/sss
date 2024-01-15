using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBirthPoint : MonoBehaviour
{
    public static RoomBirthPoint Ins;

    public Transform[] Points;
    public Vector3 GizmosSize = Vector3.one;

    private void Awake()
    {
        Ins = this;
        GizmosSize = GizmosSize * 0.2f;
    }

    private void OnDestroy()
    {
        Ins = null;
    }

    public Transform GetPoint(int index)
    {
        if (index < 0) return null;
        if (Points != null && Points.Length > 0 && index < Points.Length)
        {
            return Points[index];
        }
        return null;
    }





#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Points != null && Points.Length > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in Points)
            {
                Gizmos.DrawCube(point.position, GizmosSize);
            }
        }
    }
#endif

}
