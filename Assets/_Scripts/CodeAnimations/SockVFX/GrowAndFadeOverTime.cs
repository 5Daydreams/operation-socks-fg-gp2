using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GrowAndFadeOverTime : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float peakSize;
    [SerializeField] private AnimationCurve positionOverTime;
    [SerializeField] private AnimationCurve sizeOverTime;
    [SerializeField] private AnimationCurve alphaOvertime;
    private float currentLifetime;
    private float lerpValue;
    private Color currentColor;
    private MeshRenderer sockRenderer;
    private MaterialPropertyBlock mpb;
    private Transform cameraTransform;
    private Vector3 startingPos;

    private void Awake()
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        currentLifetime = duration;
        cameraTransform = Camera.main.transform;
        
        sockRenderer = this.GetComponent<MeshRenderer>();
        currentColor = sockRenderer.sharedMaterial.color;
        startingPos = this.transform.position;
    }

    void Update()
    {
        currentLifetime -= Time.deltaTime;
        lerpValue = (duration - currentLifetime)/duration;
        
        float easedLerpValue = positionOverTime.Evaluate(lerpValue);
        Vector3 targetPos = cameraTransform.position - cameraTransform.up - cameraTransform.right + cameraTransform.forward * 0.35f;
        this.transform.position = Vector3.Lerp(startingPos, targetPos, easedLerpValue);
        
        currentColor.a = alphaOvertime.Evaluate(easedLerpValue);
        this.transform.localScale = Vector3.one * sizeOverTime.Evaluate(easedLerpValue) * peakSize;

        mpb.SetColor("_BaseColor",currentColor);
        mpb.SetColor("_EmissionColor",currentColor);
        
        sockRenderer.SetPropertyBlock(mpb);
    }
}