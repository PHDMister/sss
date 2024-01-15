using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePowerUp : MonoBehaviour
{

    public GameObject pickupEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        Instantiate(pickupEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
