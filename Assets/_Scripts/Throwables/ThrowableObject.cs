using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ThrowableObject : MonoBehaviour
{
    [SerializeField] private Vector3 objectVelocity;

    private void Update()
    {
        RefreshObjectPosition();
    }

    [ContextMenu("CastPreview")]
    public void RefreshObjectPosition()
    {
        Vector3 position = this.transform.position;
        Vector3 velocity = objectVelocity;

        Ray ray = new Ray(position, velocity.normalized);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, Time.deltaTime))
        {
            return;
        }

        position += velocity * Time.deltaTime;
        velocity += Physics.gravity * Time.deltaTime;

        this.transform.position = position;
        objectVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("Ground"))
        {
            
        }
    }
}