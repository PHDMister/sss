using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trestts : MonoBehaviour
{

    public CircularScrollView.UICircularScrollView rewardListScroll;
    // Start is called before the first frame update
    void Start()
    {
        rewardListScroll.Init(NormalCallBack);
        rewardListScroll.ShowList(50);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void NormalCallBack(GameObject cell, int index)
    { }
}
