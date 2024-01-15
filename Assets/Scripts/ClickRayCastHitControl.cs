using UnityEngine;

public class ClickRayCastHitControl : MonoBehaviour
{
    private string TAG = "ClickRayCastHitControl_";
    Ray ray;
    RaycastHit hit;
    GameObject obj;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                obj = hit.collider.gameObject;
                Debug.Log(TAG + "���У� name = " + obj.name + "���У� tag = " + obj.tag);
                //ͨ������
                if (obj.name.Equals("Cube"))
                {
                    Debug.Log("����" + obj.name);
                }
                //ͨ����ǩ
                if (obj.tag == "CubeRed")
                {
                    Debug.Log("����" + obj.name);
                }
            }
        }
    }
}