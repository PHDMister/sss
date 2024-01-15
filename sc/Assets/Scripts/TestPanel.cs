using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ls : MonoBehaviour
{
    public CircularScrollView.UICircularScrollView m_VerticalScroll;
    private void Start()
    {
        m_VerticalScroll.Init(InitItemCallBack);
        m_VerticalScroll.ShowList(50);
    }
    private void InitItemCallBack(GameObject cell, int index)
    {
        if (cell != null)
        {
            Debug.Log("cell´æÔÚ£º " + index);
        }
        else
        {
            Debug.Log("²»´æÔÚ£º ");
        }

        //cell.transform.Find("Text1").GetComponent<Text>().text = index.ToString();
        //cell.transform.Find("Text2").GetComponent<Text>().text = index.ToString();

    }
}
