using System;
using UnityEngine;
using UnityEngine.UI;


public class Fade : EaseObject
{
    private Image image;
    [SerializeField] public EaseData easeData;
    
    private void OnEnable()
    {
        image = GetComponent<Image>();
        SetEase(easeData);
    }

    public void FadeOut()
    {
        SetStartEaseValues();
        StartCoroutine(PlayEase(easeData));
    }
    
    public void FadeIn()
    {
        SetStartEaseValues();
        StartCoroutine(PlayEase(easeData));
    }
    
    protected override void EvaluateEase(float t)
    {
        Vector3 value = Vector3.Lerp(easeData.startValue, easeData.endValue, t);
        image.color = new Color(image.color.r, image.color.g, image.color.b, value.x);
        if (image.color.a == 0)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void SetStartEaseValues()
    {
        if (image.color.a == 255)
        {
            easeData.startValue = easeData.startValueInitial;
            easeData.endValue = easeData.endValueInitial;
        }
        else
        {
            easeData.startValue = easeData.endValueInitial;
            easeData.endValue = easeData.startValueInitial;
        }
    }

}
