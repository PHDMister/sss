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
                Debug.Log(TAG + "点中： name = " + obj.name + "点中： tag = " + obj.tag);
                //通过名字
                if (obj.name.Equals("Cube"))
                {
                    Debug.Log("点中" + obj.name);
                }
                //通过标签
                if (obj.tag == "CubeRed")
                {
                    Debug.Log("点中" + obj.name);
                }
            }
        }
    }
}