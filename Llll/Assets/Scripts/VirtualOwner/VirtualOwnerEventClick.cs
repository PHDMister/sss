using System;
using UIFW;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualOwnerEventClick : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        MessageCenter.SendMessage("OnVirtualOwnerClick", KeyValuesUpdate.Empty);;   
    }
}