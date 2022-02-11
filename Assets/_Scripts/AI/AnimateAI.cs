using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int PlayerDetected = Animator.StringToHash("playerDetected");
    private static readonly int IsChasing = Animator.StringToHash("isChasing");
    private static readonly int IsPatrolling = Animator.StringToHash("isPatrolling");

    public void SetPatrolAnim(bool value)
    {
        animator.SetBool(IsPatrolling, value);
    }
    
    public void SetChaseAnim(bool value)
    {
        animator.SetBool(IsChasing, value);
    }
    
    public void PlayDetectedAnim()
    {
        animator.SetTrigger(PlayerDetected);
    }

    public bool DetectedAnimationIsDone()
    {
        return !animator.GetCurrentAnimatorStateInfo(0).IsName("playerDetected");
    }

}
