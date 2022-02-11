using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseERot : EaseObject
{
    [SerializeField] private EaseData easeRot;
    
    private void OnEnable()
    { ;
        SetEase(easeRot);
        SetStartEaseValues();
        StartCoroutine(PlayEase(easeRot));
    }
    
    protected override void SetStartEaseValues()
    {
        easeRot.startValue = easeRot.startValueInitial + transform.rotation.eulerAngles;
        easeRot.endValue = easeRot.endValueInitial + transform.rotation.eulerAngles;
    }

    protected override void EvaluateEase(float t)
    {
        transform.rotation = Quaternion.Euler(Vector3.Lerp(easeRot.startValue, easeRot.endValue, t));
    }
}
