using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EaseData", menuName = "Ease")]
public class EaseData : ScriptableObject
{
    [Header("Transform")]
    [SerializeField] internal EasingUtility.Mode easeMode;
    [SerializeField] internal EasingUtility.Style easeStyle;
    internal EasingUtility.Function EaseFunction;
    
    [SerializeField] internal Vector3 startValueInitial;
    [SerializeField] internal Vector3 endValueInitial;
    
    internal Vector3 startValue;
    internal Vector3 endValue;

    [SerializeField] public EaseObject.AnimationStyle animationStyle;
    [SerializeField] internal float duration;

    private void Awake()
    {
        startValue = Vector3.zero;
        endValue = Vector3.zero;
    }
}
