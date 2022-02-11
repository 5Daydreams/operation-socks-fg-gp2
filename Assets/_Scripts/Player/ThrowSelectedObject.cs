using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Socks.PlayerMovement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ThrowSelectedObject : MonoBehaviour
{
    [SerializeField] private ThrowInput inputComponent;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private Rigidbody ammo;
    [SerializeField] private SockData stinkySocks;
    [SerializeField] private bool debugMode = true;

    private void Awake()
    {
        if (shotOrigin == null)
        {
            shotOrigin = this.transform;
        }
    }

    public void ThrowSelectedAmmo(InputAction.CallbackContext action)
    {
        if (!inputComponent.PlayerIsAiming())
        {
            return;
        }

        if (!debugMode)
        {
            if (stinkySocks.CollectedSocks.Count == 0)
            {
                return;
            }
        }
        
        Rigidbody shot = Instantiate(ammo, shotOrigin.position, quaternion.identity);

        ammo.useGravity = true;

        Vector3 throwSpeedVector = inputComponent.GetThrowForceVector();

        shot.AddForce(throwSpeedVector, ForceMode.Impulse);

        if (!debugMode)
        {
            //stinkySocks.CollectedSocks.RemoveAt(0);
        }
    }
}