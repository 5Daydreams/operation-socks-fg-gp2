/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(AI))]
public class AICustomInspector : Editor
{
    // IDLE STATE
    private SerializedProperty _idleStateWayPoints;


//---------------------------------------------------------------------------------------------

    private void OnEnable()
    {
        // IDLE STATE
        _idleStateWayPoints = serializedObject.FindProperty("Testing._test");
    }
    
//---------------------------------------------------------------------------------------------

    public override void OnInspectorGUI()
    {
        // Draw default
        DrawDefaultInspector();
        // Update
        serializedObject.Update();
        // Get script
        var script = target as AI;

        // IDLE STATE
        if (script._useIdleState)
        {
            //EditorGUILayout.PropertyField(aiStateIdleNextState, false);
            EditorGUILayout.PropertyField(_idleStateWayPoints, true);
        }
        
        
        
        
        
        serializedObject.ApplyModifiedProperties();
    }
}/**/
