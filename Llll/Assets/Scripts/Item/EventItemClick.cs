using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class EventItemClick : BaseUIForm, IPointerClickHandler
{
    private void Awake()
    {
        if (!Camera.main.GetComponent<PhysicsRaycaster>())
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }
    ulong userid = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击了具体的内容： " + eventData.pointerClick.gameObject.name);
        userid = 0;
        if (eventData.pointerClick.gameObject.GetComponent<PlayerItem>())
        {
            userid = eventData.pointerClick.gameObject.GetComponent<PlayerItem>().GetUserID();
            Debug.Log("输出一下具体的内容只 USerID : " + userid);
            if (userid != 0)
            {
                MessageManager.GetInstance().RequestGetPersonData(() =>
                {
                    OpenUIForm(FormConst.PERSONALDATAPANEL);
                    SendMessage("OpenPersonDataPanelRefreshUI", "Success", userid);
                }, userid);
            }
        }
    }
}
