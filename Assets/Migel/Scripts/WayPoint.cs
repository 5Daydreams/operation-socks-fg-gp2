/*
using System;
using UnityEditor;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    
    public Vector3[] Position;

    [HideInInspector] public GameObject ValidateObject;
    
//***************************************************************************************************

    private void OnDrawGizmos()
    {
        
        int loops = Position.Length;
        for (var i = 0; i < loops; i++)
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Position[i], 0.5f);
            
            Gizmos.color = Color.white;
            
            if (loops > 1)
            {
                if (loops == 2)
                {
                    if (i > 0) { continue; }
                    Gizmos.DrawLine(Position[i], Position[i + 1]);
                    Handles.Label(Position[0], "Position: " + i);
                    Handles.Label(Position[i + 1], "Position: " + i+1);
                    continue;
                }

                if (i == loops - 1)
                {
                    Gizmos.DrawLine(Position[i], Position[0]);
                    Handles.Label(Position[i], "Position: " + i);
                    continue;
                }
                
                Gizmos.DrawLine(Position[i], Position[i + 1]);
                Handles.Label(Position[i], "Position: " + i);
            }
        }
    }


    private void OnValidate()
    {
        if (ValidateObject == null) { return; }
        
        ValidateObject.transform.position = Position[0];
    }
}/**/
