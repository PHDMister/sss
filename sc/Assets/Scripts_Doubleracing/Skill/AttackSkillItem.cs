using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkillItem : MonoBehaviour
{

    private GameObject targetObj;
    private GameObject startObj;
    private float speed;
    private float allTime = 1;
    public void SetTargetFun(GameObject gameObject1, GameObject gameObject)
    {
        transform.position = gameObject1.transform.position;
        targetObj = gameObject;
        startObj = gameObject1;
    }
    // Update is called once per frame
    void Update()
    {
        if (targetObj != null)
        {
            transform.LookAt(targetObj.transform);
            float aaa = Vector3.Distance(transform.position, targetObj.transform.position);
            speed = aaa / allTime;
            allTime -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetObj.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetObj.transform.position) <= 0)
            {
                Destroy(transform.gameObject);
            }
        }
    }
}
