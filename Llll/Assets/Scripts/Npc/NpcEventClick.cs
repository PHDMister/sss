using System.Collections.Generic;
using UIFW;
using UnityEngine;
using UnityEngine.EventSystems;

public class NpcEventClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TreasureModel.Instance.bInNpcNear)
        {
            MessageCenter.SendMessage("CloseChatUI", KeyValuesUpdate.Empty);
            UIManager.GetInstance().CloseUIForms(FormConst.TREASUREDIGGINGMAINMENU);
            UIManager.GetInstance().ShowUIForms(FormConst.TREASUREDIGGINGNPCTALK);
        }
    }
}
