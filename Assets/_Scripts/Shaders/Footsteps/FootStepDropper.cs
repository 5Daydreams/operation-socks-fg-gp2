using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using UnityEngine;
using UnityEngine.VFX;

public class FootStepDropper : MonoBehaviour
{
    [SerializeField] private TrackableVector3 playerPos;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField, Range(1.0f, 10.0f)] private float radius = 5;

    private void Update()
    {
        visualEffect.SetVector3("PlayerPos", playerPos.Value);
        visualEffect.SetFloat("CircleRadius", radius);
    }

    public void PlayStepVFX()
    {
        visualEffect.Play();
    }
}