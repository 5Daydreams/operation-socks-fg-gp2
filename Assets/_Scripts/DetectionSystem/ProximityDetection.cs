using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using UnityEngine;
using UnityEngine.Events;

public class ProximityDetection : MonoBehaviour
{
    [SerializeField] private Trackable<bool> playerIsSneaking;

    [Header("Viewcone Parameters")] [SerializeField]
    private Transform raycastOrigin;

    [SerializeField] private float radius;

    [Header("Player References")] [SerializeField]
    private Trackable<Vector3> playerPosTrackable;

    [SerializeField] private string playerTag;

    [Header("Callback if player within range and visible")] [SerializeField]
    private UnityEvent<bool> callback;

    private Vector3 origin;
    private bool raycastFoundPlayer;

    private void Awake()
    {
        if (raycastOrigin == null)
        {
            raycastOrigin = this.transform;
        }

        origin = raycastOrigin.position;
    }

    private bool PlayerWithinProximityRadius()
    {
        if (playerIsSneaking.Value)
        {
            return false;
        }
        
        Vector3 directionToTaget = playerPosTrackable.Value - origin;

        if (directionToTaget.sqrMagnitude > radius * radius)
        {
            return false;
        }

        Ray ray = new Ray(origin, directionToTaget);
        RaycastHit hit;

        Color rayColor = Color.red;
        // Raycast up to the viewRange of the AI
        if (Physics.Raycast(ray, out hit, radius))
        {
            raycastFoundPlayer = hit.collider.gameObject.CompareTag(playerTag);
            if (raycastFoundPlayer)
            {
                rayColor = Color.green;
            }
        }
        else
        {
            raycastFoundPlayer = false;
        }

        Debug.DrawRay(origin, directionToTaget, rayColor);

        return raycastFoundPlayer;
    }

    private void FixedUpdate()
    {
        origin = raycastOrigin.position;

        raycastFoundPlayer = PlayerWithinProximityRadius();
        callback.Invoke(raycastFoundPlayer);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, radius);
    }
}