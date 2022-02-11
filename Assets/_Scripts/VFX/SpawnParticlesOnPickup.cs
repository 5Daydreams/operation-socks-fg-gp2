using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticlesOnPickup : MonoBehaviour
{
    [SerializeField] private GrowAndFadeOverTime growingSock;

    public void SpawnPickupVFX()
    {
        Instantiate(growingSock,this.transform.position,growingSock.transform.rotation);
    }
}
