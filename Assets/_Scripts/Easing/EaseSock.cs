using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseSock : EaseObject
{
    [SerializeField] private EaseData easeData;
    private RectTransform rect;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        SetEase(easeData);
        SetStartEaseValues();
    }

    public void EaseIT()
    {
        StartCoroutine(PlayEase(easeData));
    }
    
    protected override void SetStartEaseValues()
    {
        easeData.startValue = easeData.startValueInitial;
        easeData.endValue = easeData.endValueInitial;
    }

    protected override void EvaluateEase(float t)
    {
        rect.localScale = Vector3.Lerp(easeData.startValue, easeData.endValue, t);
    }
}
