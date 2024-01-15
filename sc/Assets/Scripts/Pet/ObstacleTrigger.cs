using UIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ObstacleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PetItem petItem = this.gameObject.GetComponent<PetItem>();
        if (other.CompareTag("_TagObstacle") || (other.transform.parent != null && other.transform.parent.CompareTag("_TagObstacle")) /*|| other.CompareTag("_TagBox") || (other.transform.parent != null && other.transform.parent.CompareTag("_TagBox"))*/)
        {
            if (petItem != null)
            {
                PetModelRecData petModelRecData = petItem.GetPetData();
                if (petModelRecData != null)
                {
                    int petId = petModelRecData.id;
                    PetSpanManager.Instance().dicEnterObstacle[petId] = true;
                }
            }
            Debug.Log("ObstacleTrigger OnTriggerEnter  " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PetItem petItem = this.gameObject.GetComponent<PetItem>();
        if (other.CompareTag("_TagObstacle") || (other.transform.parent != null && other.transform.parent.CompareTag("_TagObstacle")) /*|| other.CompareTag("_TagBox") || (other.transform.parent != null && other.transform.parent.CompareTag("_TagBox"))*/)
        {
            if (petItem != null)
            {
                PetModelRecData petModelRecData = petItem.GetPetData();
                if (petModelRecData != null)
                {
                    int petId = petModelRecData.id;
                    PetSpanManager.Instance().dicEnterObstacle[petId] = false;
                }
            }
            Debug.Log("ObstacleTrigger OnTriggerExit  " + other.name);
        }
    }
}
