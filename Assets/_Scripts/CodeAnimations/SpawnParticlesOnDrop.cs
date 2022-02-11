using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticlesOnDrop : MonoBehaviour
{
    [SerializeField] private GameObject dropParticles;
    [SerializeField] private Vector3 positionOffset = Vector3.up;

    public void SpawnDropParticles()
    {
        Instantiate(dropParticles, transform.position + positionOffset, dropParticles.transform.rotation);
    }
}