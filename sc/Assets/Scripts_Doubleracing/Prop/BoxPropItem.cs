using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPropItem : MonoBehaviour
{
    public int propId = -1;
    void Start()
    {
        transform.tag = "Box";
    }
    private void SetBoxpropId(int id)
    {
        propId = id;
    }

    public int GetBoxPropId()
    {
        return propId;
    }
    public void OnTriFun()
    {
        StartCoroutine(ResetFun());
    }
    IEnumerator ResetFun()
    {
        yield return new WaitForSeconds(0.4f);
        SetActiveFun(false);
        yield return new WaitForSeconds(0.6f);
        SetActiveFun(true);

    }
    void SetActiveFun(bool isActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }
}
