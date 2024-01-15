using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PetTransferGateTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AvatarPlayer"))
        {
            // Debug.Log("OnTriggerEnter !!!");
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            HUDManager.Instance().ShowPetEntry();
            SendMessage("TransferScene", "Transfer", 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AvatarPlayer"))
        {
            // Debug.Log("OnTriggerExit !!!");
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            HUDManager.Instance().HidePetEntry();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //  Debug.Log("OnTriggerStay !!!");
    }
    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }
}
