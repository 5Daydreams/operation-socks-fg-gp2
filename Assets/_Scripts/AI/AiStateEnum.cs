

public enum AiState
{
    idle,
    investigate,
    chase,
    alert,
    internalIdle,
    //internalDistracted,
    //internalStunned,
    internalTransitionalLookAt
    //eventDistracted,
    //eventStunned,
    //none
}


public enum WayPointMovement
{
    cycle,
    cycleReversed,
    patrol,
    patrolReversed,
    random
}

public enum RotationalDirection
{
    left = -1,
    right = 1
}
