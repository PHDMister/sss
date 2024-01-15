using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class NpcHud : MonoBehaviour
{
    public float lookSpeep = 60;
    private Camera Main;
    // Start is called before the first frame update
    protected void Start()
    {
        Main = Camera.main;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Main == null) Main = Camera.main;
        Vector3 offset = Main.transform.position - transform.position;
        offset.y = 0;
        Quaternion origiRota = Quaternion.LookRotation(offset);
        transform.rotation = Quaternion.Slerp(transform.rotation, origiRota, lookSpeep * Time.deltaTime);
    }

    public void SetActive(bool enable)
    {
        this.gameObject.SetActive(enable);
    }
}
