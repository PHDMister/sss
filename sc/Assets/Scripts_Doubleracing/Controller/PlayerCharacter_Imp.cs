using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Imp : MoveControllerBase
{

    public bool isPlayer = false;


    // Start is called before the first frame update
    public override void On_Start()
    {
        base.On_Start();
    }
    public override void On_Update()
    {
        base.On_Update();

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Obss"))
        {
            Debug.Log("碰到障碍物了");
          //  SetCarStateFun(CharacterState.beHit);
        }
        if (other.CompareTag("Box"))
        {
            Debug.Log("碰到盒子了");

        }
    }
    private void OnTriggerStay(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {

    }
    public void TriBoxFun(Collider other)
    {
       /* BoxPropItem boxPropItem = other.gameObject.GetComponent<BoxPropItem>();
        int propId = boxPropItem.GetBoxPropId();
        boxPropItem.OnTriFun();
        if (propId == 1)
        {

        }
        else if (propId == 2)
        {

        }*/
    }
}
