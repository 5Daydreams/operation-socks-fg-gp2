
using System;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
public struct RotationalSequence
{
    public float rotationToDo;
    public RotationalDirection rotationalDirection;
    public float rotationSpeed;
    public Vector2 waitTimeInterval;
}


[Serializable]
public struct WayPointSettings
{
    public Vector2 waitTimeInterval;
    public bool lookForPlayer;
    public bool loopSequenceEndlessly;
    public RotationalSequence[] rotationalSequence;
}






[Serializable]
public struct IdleState
{
    [Header("Player in sight")] [Min(0f)] 
    public float reactionToNextStateTimer;

    [Header("Player in proximity")] [Min(0f)]
    public float rotateTowardsPlayerSpeed;

    [Header("A = player in sight after reaction timer | B = if A is false")]
    public AiState nextStateA;
    public AiState nextStateB;

    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;

    [Header("Waypoint setup")]
    [Min(0f)] public float moveSpeed;
    public WayPointMovement wayPointMovement;
    [Min(0)] public int startingWayPoint;
    public Vector3[] wayPoints;
    public WayPointSettings[] wayPointSettings;
    
    [HideInInspector] public int currentWayPoint;
    [HideInInspector] public bool patrolUp;
}


[Serializable]
public struct InvestigateState
{
    [Header("Player in proximity")]
    [Min(0f)] public float rotateTowardsPlayerSpeed;
    
    [Header("A = player in sight while investigating | B = if A is false after investigation")]
    public AiState nextStateA;
    public AiState nextStateB;
    
    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;
    
    [Header("Investigating")]
    [Min(0f)] public float moveSpeed;
    public RotationalSequence[] rotationalSequence;
}


[Serializable]
public struct ChaseState
{
    [Header("Player in proximity")]
    [Min(0f)] public float rotateTowardsPlayerSpeed;

    [Header("If player not caught or found")]
    public AiState nextState;
    
    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;
    
    [Header("Chasing")]
    [Min(0f)] public float moveSpeed;
    public RotationalSequence[] rotationalSequence;
}

/*
[Serializable]
public struct AlertState
{
    [Header("Player in sight")]
    [Min(0f)] public float radius;
    [Min(0f)] public float activeAlertTimer;
    
    [Header("Player in proximity")]
    [Min(0f)] public float rotateTowardsPlayerSpeed;
    
    [Header("A = player in sight after alert | B = if A is false after alert")]
    public AiState nextStateA;
    public AiState nextStateB;
    
    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;
}

// States that are set by events .................................................................

[Serializable]
public struct DistractedState
{
    [Header("Player in proximity")]
    [Min(0f)] public float rotateTowardsPlayerSpeed;
    
    [Header("A = player in sight while distracted | B = if A false after distracted")]
    public AiState nextStateA;
    public AiState nextStateB;
    
    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;
    
    [Header("Distracted")]
    [Min(0f)] public float moveSpeed;
    [Min(0f)] public float lookAtTimer;
    [Min(0f)] public float stopDistanceFromDistraction;
    [HideInInspector] public Vector3 position;
}

[Serializable]
public struct StunnedState
{
    [Header("Player in proximity")]
    [Min(0f)] public float rotateTowardsPlayerSpeed;
    
    [Header("A = player in sight after stun | B = if A false after stun")]
    public AiState nextStateA;
    public AiState nextStateB;
    
    [Header("player was in proximity but never seen when looked for")]
    public bool useSpecialProximityState;
    public AiState nextStateSpecial;
    
    [Header("Stunned")]
    [Min(0f)] public float stunnedTimer;
    public float rotationSpeed;
}

[Serializable]
public struct OnAlertedState
{
    [Header("A = player detected while alerted | B = after alerted")]
    public AiState alertedState;

    [Header("Alerted")] public bool overrideMoveSpeed;
    [Min(0f)] public float moveSpeed;
    
    [HideInInspector] public bool isAlerted;
    [HideInInspector] public GameObject alerterID;
    
    //[SerializeField] private AiState _alertStateNextStateB; 
}/**/

// States that are internal for transsitions .................................................................

[Serializable]
public struct InternalTransitionalLookAtState
{
    public bool useSpecialProximityState;
    public float rotationSpeed;
    public Vector3 position;
    public AiState nextStateA;
    public AiState nextStateB;
    public AiState proximitySpecialCaseState;
}

/**/
