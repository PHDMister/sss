using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
         offset = new Vector3(0, -2, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            this.transform.position = target.position - offset;
        }
    }
}
