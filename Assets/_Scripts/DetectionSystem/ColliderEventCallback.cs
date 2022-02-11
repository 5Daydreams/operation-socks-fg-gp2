using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class ColliderEventCallback : MonoBehaviour
{
    [SerializeField] private UnityEvent callback;
    
    private void OnTriggerEnter(Collider other)
    {
        callback.Invoke();
    }
}
