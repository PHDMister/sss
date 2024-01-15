using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFecesItem : MonoBehaviour
{
    private PetFeces petFeces;
    public void SetPetFecesData(PetFeces _data)
    {
        petFeces = _data;
    }

    public PetFeces GetPetFecesData()
    {
        return petFeces;
    }
}
