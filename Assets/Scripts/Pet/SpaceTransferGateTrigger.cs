using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SpaceTransferGateTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SpaceTransferGateTrigger OnTriggerEnter " + other.gameObject.name);
        if(other.CompareTag("Player")|| other.CompareTag("AvatarPlayer"))
        {
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            HUDManager.Instance().ShowPetEntry();
            SendMessage("TransferScene", "Transfer", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("AvatarPlayer"))
        {
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            HUDManager.Instance().HidePetEntry();
        }
        Debug.Log("SpaceTransferGateTrigger OnTriggerExit " + other.gameObject.name);
    }
    //private void OnTriggerStay(Collider other)
    //{
    //}
    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }
}
