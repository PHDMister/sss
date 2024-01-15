using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchState
{
    down,
    move,
    up
}
public class TouchEvent : MonoBehaviour
{
    //fingerId与摇杆的映射
    private Dictionary<int, JoyStick> id2JoyDic = new Dictionary<int, JoyStick>();
    //UI上所有的摇杆
    private List<JoyStick> joyList = new List<JoyStick>();

    //fingerId与控制旋转
    private Dictionary<int, CameraRotateController> CameraRoateDic = new Dictionary<int, CameraRotateController>();
    //UI上所有的控制旋转
    private List<CameraRotateController> CameraRoateList = new List<CameraRotateController>();

    /* //fingerId与摇杆的映射
     private Dictionary<int, JoyStick> id2JoyDic = new Dictionary<int, JoyStick>();
     //UI上所有的摇杆
     private List<JoyStick> joyList = new List<JoyStick>();*/

    private Dictionary<int, Touch> allListTouchID = new Dictionary<int, Touch>();

    private Dictionary<int, Touch> allLastListTouchID = new Dictionary<int, Touch>();


    private bool isMouseDown = true;

    /// <summary>
    /// 添加摇杆，由joystick在开始调用
    /// </summary>
    /// <param name="joy"></param>
    public void AddJoy(JoyStick joy)
    {
        joyList.Add(joy);
    }
    public void AddCameraRoate(CameraRotateController roateController)
    {
        CameraRoateList.Add(roateController);
    }
    void Update()
    {
        /* if (!ManageMentClass.DataManagerClass.IsControllerPlayer)
         {
             return;
         }*/
        //每次先把新的清楚


        allListTouchID.Clear();
        foreach (var touch in Input.touches)
        {
            //每次给新的赋值
            if (!allListTouchID.ContainsKey(touch.fingerId))
            {
                allListTouchID.Add(touch.fingerId, touch);
            }
            //对比上一帧的内容
            if (!allLastListTouchID.ContainsKey(touch.fingerId))
            {
                // 在老的ID里不存在，就是 刚触摸 begin
                foreach (var joy in joyList)
                {
                    if (joy.inTouchArea(touch.position) && !id2JoyDic.ContainsValue(joy))
                    {
                        id2JoyDic.Add(touch.fingerId, joy);
                        joy.ReadTouch(TouchState.down, touch.position);
                    }
                }
                //遍历控制人物旋转的控制旋转的类
                foreach (var cameraRoate in CameraRoateList)
                {
                    if (cameraRoate.inTouchArea(touch.position) && !CameraRoateDic.ContainsValue(cameraRoate))
                    {
                        CameraRoateDic.Add(touch.fingerId, cameraRoate);
                        cameraRoate.ReadTouch(TouchState.down, touch.position);
                    }
                }
            }
            else
            {
                // 在老的ID里存在，一直移动  move移动
                if (id2JoyDic.ContainsKey(touch.fingerId))
                {
                    id2JoyDic[touch.fingerId].ReadTouch(TouchState.move, touch.position);
                }
                if (CameraRoateDic.ContainsKey(touch.fingerId))
                {
                    CameraRoateDic[touch.fingerId].ReadTouch(TouchState.move, touch.position);
                }
            }
        }

        var keys = allLastListTouchID.Keys;
        if (keys.Count > 0)
        {
            foreach (var key in keys)
            {
                if (!allListTouchID.ContainsKey(key))
                {
                    // 这个 touh在这一帧就已经关掉了 结束

                    if (id2JoyDic.ContainsKey(key))
                    {
                        id2JoyDic[key].ReadTouch(TouchState.up, allLastListTouchID[key].position);
                        id2JoyDic.Remove(key);
                    }
                    if (CameraRoateDic.ContainsKey(key))
                    {
                        CameraRoateDic[key].ReadTouch(TouchState.up, allLastListTouchID[key].position);
                        CameraRoateDic.Remove(key);

                    }
                }
            }
            allLastListTouchID.Clear();
        }


        var nowKeys = allListTouchID.Keys;
        if (nowKeys.Count > 0)
        {
            foreach (var nowkey in nowKeys)
            {
                allLastListTouchID.Add(nowkey, allListTouchID[nowkey]);
            }
        }




        
        //Vector3 mousePos = Input.mousePosition;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("1111111111  GetMouseButtonDown");
        //    foreach (var joy in joyList)
        //    {
        //        if (joy.inTouchArea(mousePos))
        //        {
        //            isMouseDown = true;
        //            joy.ReadTouch(TouchState.down, mousePos);
        //        }
        //    }
        //}
        //else if (Input.GetMouseButton(0) && isMouseDown)
        //{
        //    foreach (var joy in joyList)
        //    {
        //        if (joy.inTouchArea(mousePos))
        //        {
        //            Debug.Log("1111111111  GetMouseButton  move");
        //            joy.ReadTouch(TouchState.move, mousePos);
        //        }
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0) && isMouseDown)
        //{
        //    isMouseDown = false;
        //    Debug.Log("1111111111  GetMouseButtonUp ");
        //    foreach (var joy in joyList)
        //    {
        //        if (joy.inTouchArea(mousePos))
        //        {
        //            joy.ReadTouch(TouchState.up, mousePos);
        //        }
        //    }
        //}












        /*
                if (id2JoyDic.ContainsKey(touch.fingerId))
                {

                }
                foreach (var joy in joyList)
                {

                }




                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("anxiadeneirongzhi  began joyList.Count: " + joyList.Count + " joy ID cunt:" + id2JoyDic.Keys.Count);
                    Debug.Log("anxiadeneirongzhi  began cameraList.Count: " + CameraRoateList.Count + " joy ID cunt:" + CameraRoateDic.Keys.Count);

                }
                if (id2JoyDic.ContainsKey(touch.fingerId))
                {
                    id2JoyDic[touch.fingerId].ReadTouch(TouchState.move, touch.position);
                    Debug.Log("joy jiance : " + touch.fingerId + "   touch.phase: " + touch.phase);
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        Debug.Log(" remove joy  : " + touch.fingerId);
                        id2JoyDic.Remove(touch.fingerId);
                    }
                }
                if (CameraRoateDic.ContainsKey(touch.fingerId))
                {
                    Debug.Log("camera jiance : " + touch.fingerId + "   touch.phase: " + touch.phase);
                    CameraRoateDic[touch.fingerId].ReadTouch(touch);
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        Debug.Log("remove cameraRoaterDic :" + touch.fingerId);
                        CameraRoateDic.Remove(touch.fingerId);
                    }
                }
            }*/
    }
}