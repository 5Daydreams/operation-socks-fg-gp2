using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EaseE : EaseObject
{
    [SerializeField] private EaseData easeData;

    private void OnEnable()
    {
        SetEase(easeData);
        SetStartEaseValues();
        StartCoroutine(PlayEase(easeData));
    }
    
    protected override void SetStartEaseValues()
    {
        easeData.startValue = easeData.startValueInitial + transform.position;
        easeData.endValue = easeData.endValueInitial + transform.position;
    }

    protected override void EvaluateEase(float t)
    {
        transform.position = Vector3.Lerp(easeData.startValue, easeData.endValue, t);
    }
}
