using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetItem : MonoBehaviour
{
    private PetListRecData data = null;
    private PetModelRecData petData = null;
    public void SetPetBoxData(PetListRecData _data)
    {
        data = _data;
    }

    public PetListRecData GetPetBoxData()
    {
        return data;
    }

    public void SetPetData(PetModelRecData petdata)
    {
        petData = petdata;
    }

    public PetModelRecData GetPetData()
    {
        return petData;
    }
}
