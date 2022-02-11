using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AI)), CanEditMultipleObjects]
public class WayPointEditor : Editor
{
    private void OnSceneGUI()
    {

        AI script = (AI) target;

        int loops = script._idleState.wayPoints.Length;

        if (script._useIdleState)
        {
            EditorGUI.BeginChangeCheck();

            for (var i = 0; i < loops; i++)
            {
                Vector3 position = Handles.PositionHandle(script._idleState.wayPoints[i], Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(script, "Change Look At Target Position");
                    script._idleState.wayPoints[i] = position;
                }
            }
        }
    }
}
