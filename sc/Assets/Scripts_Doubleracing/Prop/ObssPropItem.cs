using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObssPropItem : MonoBehaviour
{
    void Start()
    {
        transform.tag = "Obss";
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            PlayerCharacter_Imp playerCharacter_Imp = other.gameObject.GetComponent<PlayerCharacter_Imp>();

            if (playerCharacter_Imp)
            {
                if (playerCharacter_Imp.characterProtectState != MoveControllerBase.CharacterProtectState.Ing)
                {
                    transform.gameObject.SetActive(false);
                  //  Destroy(transform.gameObject);
                }
            }
        }
    }
}
