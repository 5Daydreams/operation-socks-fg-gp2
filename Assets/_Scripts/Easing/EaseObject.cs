using System.Collections;
using UnityEngine;

public abstract class EaseObject : MonoBehaviour
{
    public enum AnimationStyle
    {
        Once,
        Loop,
        PingPong
    }

    protected abstract void EvaluateEase(float t);
    
    protected abstract void SetStartEaseValues();

    protected void SetEase(EaseData easeData)
    {
        easeData.EaseFunction = EasingUtility.GetFunction(easeData.easeStyle, easeData.easeMode);
    }
    
    protected IEnumerator PlayEase(EaseData easeData)
    {
        float t = 0;
        while (t < 1)
        {
            EvaluateEase(easeData.EaseFunction(t));
            t += GetT(easeData.duration);
            yield return null;
        }
        EvaluateEase(easeData.EaseFunction(1));
        
        if (easeData.animationStyle == AnimationStyle.Loop)
        {
            StartCoroutine(PlayEase(easeData));
        }
        else if (easeData.animationStyle == AnimationStyle.PingPong)
        {
            t = 1;
            while (t > 0)
            {
                EvaluateEase(easeData.EaseFunction(t));
                t -= GetT(easeData.duration);
                yield return null;
            }
            EvaluateEase(easeData.EaseFunction(1));
            StartCoroutine(PlayEase(easeData));
        }
    }
    
    private float GetT(float duration)
    {
        return Time.deltaTime / duration;
    }
    
}
