using System.Collections;
using System.Collections.Generic;
using UIFW;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{

    public LoadSceneType SceneType = LoadSceneType.parlorScene;
    public bool CheckRoomMaster = true;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("SpaceTransferGateTrigger OnTriggerEnter " + other.gameObject.name);
        if (other.CompareTag("Player") || other.CompareTag("AvatarPlayer"))
        {
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            if (CheckRoomMaster && !ManageMentClass.DataManagerClass.is_Owner) return;
            HUDManager.Instance().ShowPetEntry();
            MessageCenter.SendMessage("TransferScene", "Transfer", (int)SceneType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AvatarPlayer"))
        {
            if (other.gameObject != CharacterManager.Instance().GetPlayerObj()) return;
            HUDManager.Instance().HidePetEntry();
        }
        //Debug.Log("SpaceTransferGateTrigger OnTriggerExit " + other.gameObject.name);
    }
}
