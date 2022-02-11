using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using UnityEditor;
using UnityEngine;

public class WallHeightController : MonoBehaviour
{
    [SerializeField] private Trackable<Vector3> playerPosReference;
    [SerializeField] private float offsetDrop;
    [SerializeField] private float maxDropPos;
    [SerializeField] private float dropHeight;
    [SerializeField] private float floorPos;
    [SerializeField] private float duration;
    private Vector3 basePos;

    private float startY;
    private float endY;

    private void Awake()
    {
#if UNITY_EDITOR
        if (playerPosReference == null)
        {
            EditorApplication.isPlaying = false;
            throw new NullReferenceException("Player position reference not set in following game object: " +
                                             this.gameObject.name);
        }
#endif

        basePos = this.transform.position;

        startY = floorPos;
        endY = startY + dropHeight;
    }
    
    private void AdjustWallPosition()
    {
        float signedDistance = (playerPosReference.Value - this.transform.position).z + offsetDrop;
        signedDistance = Mathf.Clamp(signedDistance, 0, maxDropPos) / maxDropPos;
        
        float displacementHeight = 
            EasingUtility.Ease(EasingUtility.GetFunction(EasingUtility.Style.Sine, EasingUtility.Mode.InOut), 
                startY, endY, signedDistance / duration);
        
        //float displacementHeight = Mathf.Lerp(0, dropHeight, signedDistance);

        this.transform.position = new Vector3(transform.position.x, -displacementHeight, transform.position.z);
    }

    private void Update()
    {
        AdjustWallPosition();
    }
}