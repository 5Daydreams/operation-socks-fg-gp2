using System;
using _Code.Scriptables.TrackableValue;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;



[RequireComponent(typeof(NavMeshAgent))]
public class AI : MonoBehaviour
{
    [SerializeField] Trackable<Vector3> _playerPositionReference;

    private NavMeshAgent _navMeshAgent;
    private Transform _transform;
    private AnimateAI _animateAI;
    private AudioSource _audioSource;
    
    // States
    [Space(10)]
    public bool _useIdleState;
    [SerializeField] private bool _useChaseState;
    [SerializeField] private bool _useInvestigateState;
    //[SerializeField] private bool _useAlertState;
    [Space(10)]
    //[SerializeField] private bool _canBeDistracted;
    //[SerializeField] private bool _canBeStunned;
    //[SerializeField] private bool _canBeAlerted;

    [SerializeField] private AudioClip _chaseSound;
    [SerializeField] private AudioClip _investigateSound;
    [SerializeField] private AudioClip _detectedSound;
    
    // Mutual
    private bool _resetRotation;
    private float _currentRotatedAmount;
    private float _rotationToDo;
    private float _rotationSpeed;
    private RotationalDirection _rotationalDirection;
    private int _currentRotationalSequenceIndex;

    private float _currentCountDownTimer;
    private float _currentWayPointCountDownTimer;

    private float _currentExclamationMarkTimer;
    private float _maxExclamationMarkTimer;
    private bool _currentExclamationMarkTimerState;
    private bool _hasSavedCurrentExclamationMarkTimer;

    private bool _audioWasPlayed;

    private bool _detectedAnimationDone;
    private bool _detectedAnimationIsCurrentlyPlaying;

    // IDLE
    [Space(20f)]
    [Header("IDLE --------------------------------------------------------------------------------")]
    public IdleState _idleState;

    // CHASE
    [Space(20f)]
    [Header("CHASE --------------------------------------------------------------------------------")]
    public ChaseState _chaseState;
    
    // INVESTIGATE
    [Space(20f)]
    [Header("INVESTIGATE --------------------------------------------------------------------------------")]
    public InvestigateState _investigateState;
    
    /*
    // ALERT
    [Space(20f)]
    [Header("ALERT --------------------------------------------------------------------------------")]
    public AlertState _alertState;
    
    
    // Event states
    // DISTRACTED
    [Space(20f)]
    [Header("EVENT DISTRACTED --------------------------------------------------------------------------------")]
    public DistractedState _distractedState;
    
    // STUNNED
    [Space(20f)]
    [Header("EVENT STUNNED --------------------------------------------------------------------------------")]
    public StunnedState _stunnedState;
    
    // ON ALERTED
    [Space(20f)]
    [Header("EVENT ON ALERTED --------------------------------------------------------------------------------")]
    public OnAlertedState _onAlertedState;/**/
    
    // Internal states
    private InternalTransitionalLookAtState _internalTransitionalLookAtState;
    

    private AiState _currentState;
    private AiState _previousState;

    private bool _playerInVision;
    private bool _playerInProximity;
    private bool _playerWasDetected;
    private Vector3 _lastKnownPlayerPosition;
    
    private Action ExecuteState;


//...............................................................................

    #region Enable / Disable
    private void OnEnable()
    {
        /*
        if (_canBeDistracted)
        {
            EventManager.ONAiDistract += IncomingDistraction;
        }

        if (_canBeStunned)
        {
            EventManager.ONAiStun += IncomingStun;
        }

        if (_canBeAlerted == true)
        {
            EventManager.ONAiAlertActivate += IncomingAlertActivate;
            EventManager.ONAiAlertDeactivate += IncomingAlertDeactivate;
        }/**/

    }

    private void OnDisable()
    {
        /*
        if (_canBeDistracted)
        {
            EventManager.ONAiDistract -= IncomingDistraction;
        }

        if (_canBeStunned)
        {
            EventManager.ONAiStun -= IncomingStun;
        }
        
        if (_canBeAlerted == true)
        {
            EventManager.ONAiAlertActivate -= IncomingAlertActivate;
            EventManager.ONAiAlertDeactivate -= IncomingAlertDeactivate;
        }/**/

    }

    #endregion
    
//...............................................................................

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _transform = GetComponent<Transform>();
        _animateAI = GetComponent<AnimateAI>();
        _audioSource = GetComponent<AudioSource>();
        
        _maxExclamationMarkTimer = _idleState.reactionToNextStateTimer;
        _currentExclamationMarkTimer = _maxExclamationMarkTimer;
        
        _idleState.currentWayPoint = _idleState.startingWayPoint;

        SetState(AiState.idle);
    }


    private void Update()
    {
        // Exclamation Mark
        ExclamationMarkController();
        
        // Not null, invoke
        ExecuteState?.Invoke();
    }


//..............................................................................................................

    #region Player detection and position
    /*private bool PlayerWithinDistance()
    {
        float distance = Vector3.Distance(_playerTransform.position, _transform.position);
        if (distance <= _alertRadius)
        {
            return true;
        }
        
        return false;
    }*/


    private void ExclamationMarkController()
    {
        switch (GetCurrentState())
        {
            case AiState.idle:

                ExclamationMarkTimer(false);
                _detectedAnimationDone = false; //TODO
                break;
            
            
            case AiState.internalIdle:
            case AiState.chase:
            case AiState.investigate:

                if (PlayerInVision() == true)
                {
                    if (_hasSavedCurrentExclamationMarkTimer)
                    {
                        ResetCountDownTimer(GetCurrentExclamationMarkTimer());
                        _hasSavedCurrentExclamationMarkTimer = false;
                    }
                    
                    ExclamationMarkTimer(true);
                    return;
                }

                _hasSavedCurrentExclamationMarkTimer = true;
                
                break;
        }
    }

    public void SetPlayerInVision(bool set)
    {
        _playerInVision = set;
    }
    private bool PlayerInVision()
    {
        return _playerInVision;
    }

    public void SetPlayerInProximity(bool set)
    {
        _playerInProximity = set;
    }

    private bool PlayerInProximity()
    {
        return _playerInProximity;
    }

    private void SetPlayerWasDetected(bool set)
    {
        _playerWasDetected = set;
    }
    
    private bool GetPlayerWasDetected()
    {
        return _playerWasDetected;
    }

    private void SetLastKnownPlayerPosition()
    {
        _lastKnownPlayerPosition = _playerPositionReference.Value;
    }
    
    private void SetLastKnownPlayerPosition(Vector3 position)
    {
        _lastKnownPlayerPosition = position;
    }
    
    private Vector3 GetLastKnownPlayerPosition()
    {
        return _lastKnownPlayerPosition;
    }
    
    #endregion
    
// STATES ............................................................................................................

    private void IdleState()
    {
        if (PlayerInVision() == true)
        { 
            _animateAI.SetPatrolAnim(false);
            SetMoveToPosition(_transform.position);
            SetLastKnownPlayerPosition();
            SetState(AiState.internalIdle);
            return;
        }
        
        if (PlayerInProximity() == true)
        {
            _animateAI.SetPatrolAnim(false);
            SetMoveToPosition(_transform.position);
            SetLastKnownPlayerPosition();
            InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _idleState.rotateTowardsPlayerSpeed, 
                _idleState.nextStateA, _idleState.nextStateB, _idleState.useSpecialProximityState, _idleState.nextStateSpecial);
            
            SetState(AiState.internalTransitionalLookAt);
            return;
        }/**/

        // At position
        if (AtPosition() == true)
        {
            _animateAI.SetPatrolAnim(false);
            
            if (WayPointCountDownTimer() == false)
            {
                return;
            }
            
            // Look for player
            if (_idleState.wayPointSettings[_idleState.currentWayPoint].lookForPlayer)
            {
                if (CountDownTimer() == false) // TODO
                {
                    Debug.Log("TIMER   " + _currentRotationalSequenceIndex);
                    return;
                }

                // Rotating
                if (RotateToLookForPlayer() == false)
                {
                    Debug.Log(_currentRotationalSequenceIndex);
                    return;
                }
                

                var array = _idleState.wayPointSettings[_idleState.currentWayPoint].rotationalSequence;

                SetNextRotationalSequenceIndex(array);
                ResetCurrentRotatedAmount();
                SetRotationalSequence(array);
                ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));
                
                if (_currentRotationalSequenceIndex != 0)
                {
                    return;
                }
                

                if (_idleState.wayPointSettings[_idleState.currentWayPoint].loopSequenceEndlessly == true)
                {
                    return;
                }
            }
            
            
            SetNextWayPoint();
            SetMoveToPosition(_idleState.wayPoints[_idleState.currentWayPoint]);
            
            ResetWayPointCountDownTimer(GetWayPointTimer());
            //Debug.Log(_idleState.currentWayPoint);
            return;
        }
        
        _animateAI.SetPatrolAnim(true);
    }
    

    private void ChaseState()
    {

        if (_detectedAnimationDone == false)
        {
            if (GetCurrentExclamationMarkTimerState() == true){
                if (_detectedAnimationIsCurrentlyPlaying == false)
                {
                    // DO animation
                    _animateAI.PlayDetectedAnim();
                    _detectedAnimationIsCurrentlyPlaying = true;
                    PlayAudio(_detectedSound);
                }

            if (_animateAI.DetectedAnimationIsDone() == true)
            {
                DetectedAnimationDone();
            }

                return;
            }
        }
        
        
        
        // Udating player position
        if (PlayerInVision() == true)
        {
            if (GetCurrentExclamationMarkTimerState() == false)
            {
                SetMoveToPosition(_transform.position);
                SetLastKnownPlayerPosition();
                SetState(AiState.internalIdle);
                return;
            }
            
            SetLastKnownPlayerPosition();
            ResetCurrentRotatedAmount();
        }
        else
        {
            if (PlayerInProximity() == true)
            {
                SetMoveToPosition(_transform.position);
                SetLastKnownPlayerPosition();
                InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _chaseState.rotateTowardsPlayerSpeed,
                    GetCurrentState(), _chaseState.nextState, _chaseState.useSpecialProximityState, _chaseState.nextStateSpecial);
                
                SetState(AiState.internalTransitionalLookAt);
                return;
            }
        }
        
        PlayAudio(_chaseSound);

    // Move to position
        SetMoveToPosition(GetLastKnownPlayerPosition());
        // At position
        if (AtPosition() == true)
        {
            
            _animateAI.SetChaseAnim(false);
            
            if (CountDownTimer() == false)
            {
                return;
            }

            // Rotating
            if (RotateToLookForPlayer() == false)
            {
                return;
            }
            
            var array = _chaseState.rotationalSequence;

            SetNextRotationalSequenceIndex(array);
            ResetCurrentRotatedAmount();
            SetRotationalSequence(array);
            ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));
                
            if (_currentRotationalSequenceIndex != 0)
            {
                return;
            }
            
            SetState(_chaseState.nextState);
            return;
        }
        
        _animateAI.SetChaseAnim(true);
    }
    
    private void InvestigateState()
    {
        if (PlayerInVision() == true)
        {
            _animateAI.SetPatrolAnim(false);
            SetState(_investigateState.nextStateA);
            return;
        }
        
        if (PlayerInProximity() == true)
        {
            _animateAI.SetPatrolAnim(false);
            SetMoveToPosition(_transform.position);
            SetLastKnownPlayerPosition();
            InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _investigateState.rotateTowardsPlayerSpeed, 
                _investigateState.nextStateA, _investigateState.nextStateB, _investigateState.useSpecialProximityState, _investigateState.nextStateSpecial);
            
            SetState(AiState.internalTransitionalLookAt);
            return;
        }
        
        PlayAudio(_investigateSound);
        
        // At position
        if (AtPosition() == true)
        {
            _animateAI.SetPatrolAnim(false);
            
            if (CountDownTimer() == false)
            {
                return;
            }

            // Rotating
            if (RotateToLookForPlayer() == false)
            {
                //Debug.Log(_currentRotationalSequenceIndex);
                return;
            }
            
            var array = _investigateState.rotationalSequence;

            SetNextRotationalSequenceIndex(array);
            ResetCurrentRotatedAmount();
            SetRotationalSequence(array);
            ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));
                
            if (_currentRotationalSequenceIndex != 0)
            {
                return;
            }
            
            SetState(_investigateState.nextStateB);
            return;
        }
        
        _animateAI.SetPatrolAnim(true);
    }

    /*
    private void AlertState()
    {
        if (PlayerInVision() == true)
        {
            if (GetCurrentExclamationMarkTimerState() == false)
            {
                SetMoveToPosition(_transform.position);
                SetLastKnownPlayerPosition();
                SetState(AiState.internalIdle);
                return;
            }
            
            //ResetCountDownTimer(_alertState.activeAlertTimer);
            SetLastKnownPlayerPosition();
            SetRotationSettingsWithCalculations(GetLastKnownPlayerPosition(), _alertState.rotateTowardsPlayerSpeed);

            RotateToLookForPlayer();
        }
        else
        {
            if (PlayerInProximity() == true)
            {
                /*
                SetLastKnownPlayerPosition();
                InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _alertState.rotateTowardsPlayerSpeed,
                    _alertState.nextStateA, _alertState.nextStateB, _alertState.useSpecialProximityState, _alertState.nextStateSpecial);
                
                SetState(AiState.internalTransitionalLookAt);
                
                return; 
                
                SetLastKnownPlayerPosition();
                SetRotationSettingsWithCalculations(GetLastKnownPlayerPosition(), _idleState.rotateTowardsPlayerSpeed);

                RotateToLookForPlayer();
            }
        }

        if (CountDownTimer() == true)
        {
            // Deactivate AI alert
            EventManager.Instance.InvokeOnAiAlertDeactivate(gameObject);
            
            if (PlayerInVision())
            {
                SetState(_alertState.nextStateA);
                return;
            }
            
            SetState(_alertState.nextStateB);
            
            return;
        }
        
        // Activate AI alert
        EventManager.Instance.InvokeOnAiAlertActivate(gameObject, _transform.position, _alertState.radius, GetLastKnownPlayerPosition());
    }/**/

    /*
    private void DistractedState()
    {
        if (PlayerInVision() == true)
        {
            SetState(_distractedState.nextStateA);
            return;
        }

        if (PlayerInProximity() == true)
        {
            SetMoveToPosition(_transform.position);
            SetLastKnownPlayerPosition();
            InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _distractedState.rotateTowardsPlayerSpeed,
                _distractedState.nextStateA, _distractedState.nextStateB, _distractedState.useSpecialProximityState, _distractedState.nextStateSpecial);
                
            SetState(AiState.internalTransitionalLookAt);
            return;
        }
        
        // At position
        if (AtPosition() == true)
        {
            SetState(AiState.internalDistracted);
        }
    }
    
    private void StunnedState()
    {
        if (CountDownTimer() == true)
        {
            SetState(AiState.internalStunned);
        }
    }/**/
    
    
// INTERNAL STATES ....................................................................................................

    private void InternalIdleState()
    {
        if (PlayerInVision() == true)
        {
            SetLastKnownPlayerPosition();
            SetRotationSettingsWithCalculations(GetLastKnownPlayerPosition(), _idleState.rotateTowardsPlayerSpeed);

            RotateToLookForPlayer();
        }
        else
        {
            if (PlayerInProximity() == true)
            {
                SetLastKnownPlayerPosition();
                InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _idleState.rotateTowardsPlayerSpeed,
                    _idleState.nextStateA, _idleState.nextStateB, _idleState.useSpecialProximityState, _idleState.nextStateSpecial);
                
                SetState(AiState.internalTransitionalLookAt);
                return;
            }
        }

        if (CountDownTimer() == false)
        {
            return;
        }/**/

        if (PlayerInVision() == true)
        {
            SetState(_idleState.nextStateA);
            return;
        }
        
        SetState(_idleState.nextStateB);
    }

    /*
    private void InternalDistractedState()
    {
        if (PlayerInVision() == true)
        {
            SetState(_distractedState.nextStateA);
            return;
        }
        
        if (PlayerInProximity() == true)
        {
            SetLastKnownPlayerPosition();
            InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _distractedState.rotateTowardsPlayerSpeed,
                _distractedState.nextStateA, _distractedState.nextStateB, _distractedState.useSpecialProximityState, _distractedState.nextStateSpecial);
                
            SetState(AiState.internalTransitionalLookAt);
            return;
        }
        
        if (CountDownTimer() == true)
        {
            SetState(_distractedState.nextStateB);
        }
    }
    
    private void InternalStunnedState()
    {
        if (PlayerInVision() == true)
        {
            SetState(_stunnedState.nextStateA);
            return;
        }
        
        if (PlayerInProximity() == true)
        {
            SetLastKnownPlayerPosition();
            InternalTransitionalLookAtStateStartUp(GetLastKnownPlayerPosition(), _stunnedState.rotateTowardsPlayerSpeed,
                _stunnedState.nextStateA, _stunnedState.nextStateB, _stunnedState.useSpecialProximityState, _stunnedState.nextStateSpecial);
                
            SetState(AiState.internalTransitionalLookAt);
            return;
        }
        
        if (RotateToLookForPlayer() == true)
        {
            SetState(_stunnedState.nextStateB);
        }
    }/**/
    

//..............................................................................................................

    private void InternalTransitionalLookAtState()
    {
        if (PlayerInVision() == true)
        {
            SetState(_internalTransitionalLookAtState.nextStateA);
            return;
        }

        if (PlayerInProximity() == true)
        {
            SetLastKnownPlayerPosition();
            _internalTransitionalLookAtState.position = GetLastKnownPlayerPosition();
            SetRotationSettingsWithCalculations(_internalTransitionalLookAtState.position, _internalTransitionalLookAtState.rotationSpeed);
        }

        if (RotateToLookForPlayer() == true)
        {
            
            if (_internalTransitionalLookAtState.useSpecialProximityState)
            {
                SetState(_internalTransitionalLookAtState.proximitySpecialCaseState);
                return;
            }
            
            SetState(_internalTransitionalLookAtState.nextStateB);
        }
    }

// SET STATE ..................................................................................................

    private void SetState(AiState state)
    {
        _previousState = _currentState;
        _currentState = state;
        
        // Debug - show state
        Debug.Log(_currentState);

        SetExecutableState();
    }

    private AiState GetCurrentState()
    {
        return _currentState;
    }
    
    private AiState GetPreviousState()
    {
        return _previousState;
    }

// SET EXECUTABLE STATE ........................................................................................

    private void SetExecutableState()
    {
        
        ResetAudioWasPlayed();
        
        // Subscribe
        switch (_currentState)
        {
            case AiState.idle:
                ExecuteState = IdleState;
                IdleStateStartUp();
                break;
            
            case AiState.chase:
                ExecuteState = ChaseState;
                ChaseStateStartUp();
                break;
            
            case AiState.investigate:
                ExecuteState = InvestigateState;
                InvestigateStateStartUp();
                break;
            /*
            case AiState.eventDistracted:
                ExecuteState = DistractedState;
                DistractedStateStartUp();
                break;
            
            case AiState.eventStunned:
                ExecuteState = StunnedState;
                StunnedStateStartUp();
                break;
            
            case AiState.alert:
                ExecuteState = AlertState;
                AlertStartUp();
                break;/**/
            
            
            // INTERNAL
            case AiState.internalIdle:
                ExecuteState = InternalIdleState;
                //ResetCountDownTimer(_idleState.reactionToNextStateTimer);
                ResetCountDownTimer(GetCurrentExclamationMarkTimer());
                //IdleStateStartUp();
                break;
            /*
            case AiState.internalDistracted:
                ExecuteState = InternalDistractedState;
                break;
            
            case AiState.internalStunned:
                ExecuteState = InternalStunnedState;
                break;/**/
            
            // INTERNAL TRANSITIONAL
            case AiState.internalTransitionalLookAt:
                ExecuteState = InternalTransitionalLookAtState;
                _animateAI.SetPatrolAnim(false);
                _animateAI.SetChaseAnim(false);
                break;
            
        }

        // Unsubscribe
        /*
        switch (_previousState)
        {
            case AiState.alert:
                // Deactivate AI alert
                EventManager.Instance.InvokeOnAiAlertDeactivate(gameObject);
                break;
            
            
            
            
        }/**/
        
    }

// STATE START UP ...............................................................................................
    private void IdleStateStartUp()
    {
        SetMoveSpeed(_idleState.moveSpeed);
        SetAutoBreaking(false);
        SetMoveToPosition(_idleState.wayPoints[_idleState.currentWayPoint]);
        ResetStoppingDistance();
        
        ResetWayPointCountDownTimer(GetWayPointTimer());
        
        ResetCurrentRotationalSequenceIndex();
        ResetCurrentRotatedAmount();
        RotationalSequence[] array = _idleState.wayPointSettings[_idleState.currentWayPoint].rotationalSequence;
        SetRotationalSequence(array);
        ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));
        
        //OnAlertedStateOverrideMoveSpeed();
    }

    private void ChaseStateStartUp()
    {
        SetMoveSpeed(_chaseState.moveSpeed);
        SetAutoBreaking(true);
        ResetStoppingDistance();

        ResetCurrentRotationalSequenceIndex();
        ResetCurrentRotatedAmount();
        RotationalSequence[] array = _chaseState.rotationalSequence;
        SetRotationalSequence(array);
        ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));
        
        //OnAlertedStateOverrideMoveSpeed();
    }

    private void InvestigateStateStartUp()
    {
        SetMoveSpeed(_investigateState.moveSpeed);
        SetAutoBreaking(false);
        SetMoveToPosition(_lastKnownPlayerPosition);
        ResetStoppingDistance();

        ResetCurrentRotationalSequenceIndex();
        ResetCurrentRotatedAmount();
        RotationalSequence[] array = _investigateState.rotationalSequence;
        SetRotationalSequence(array);
        ResetCountDownTimer(GetRotationalSequenceWaitTimeIntervalTimer(array));

        //OnAlertedStateOverrideMoveSpeed();
    }
    
    /*
    private void AlertStartUp()
    {
        ResetCountDownTimer(_alertState.activeAlertTimer);
    }
    
    private void DistractedStateStartUp()
    {
        SetMoveSpeed(_distractedState.moveSpeed);
        SetAutoBreaking(false);
        SetMoveToPosition(_distractedState.position);
        ResetStoppingDistance();
        //SetStoppingDistance(_distractedState.stopDistanceFromDistraction);
        
        ResetCountDownTimer(_distractedState.lookAtTimer);
    }
    
    private void StunnedStateStartUp()
    {
        ResetCountDownTimer(_stunnedState.stunnedTimer);
        
        SetRotationSettings(_stunnedState.rotationSpeed, 360f, RotationalDirection.left);
    }/**/
    
    
    // INTERNAL
    private void InternalTransitionalLookAtStateStartUp(Vector3 position, 
        float rotationSpeed, AiState nextStateA, AiState nextStateB, bool useSpecialState, AiState specialState)
    {
        _internalTransitionalLookAtState.useSpecialProximityState = useSpecialState;
        _internalTransitionalLookAtState.rotationSpeed = rotationSpeed;
        _internalTransitionalLookAtState.position = position;
        _internalTransitionalLookAtState.nextStateA = nextStateA;
        _internalTransitionalLookAtState.nextStateB = nextStateB;
        _internalTransitionalLookAtState.proximitySpecialCaseState = specialState;

        SetRotationSettingsWithCalculations(position, rotationSpeed);
    }
    

//..............................................................................................................
    /*
    private void IncomingAlertActivate(GameObject id, Vector3 position, float radius, Vector3 point)
    {
        if (_canBeAlerted == false || id == gameObject)
        {
            return;
        }

        if (WithinDistance(_transform.position, position, radius))
        {
            if (_onAlertedState.isAlerted == true && _onAlertedState.alerterID != id)
            {
                float lastDistance = Vector3.Distance(GetLastKnownPlayerPosition(), point);
                float newDistance = Vector3.Distance(position, point);

                if (lastDistance < newDistance)
                {
                    return;
                }
            }
            
            SetLastKnownPlayerPosition(point);
            _onAlertedState.isAlerted = true;
            _onAlertedState.alerterID = id;
            SetState(_onAlertedState.alertedState);
        }
    }
    
    private void IncomingAlertDeactivate(GameObject id)
    {
        if (_canBeAlerted == false || _onAlertedState.isAlerted == false)
        {
            return;
        }

        if (_onAlertedState.alerterID == id)
        {
            _onAlertedState.isAlerted = false;
            _onAlertedState.alerterID = null;
        }
    }
    /*
    private void IncomingDistraction(Vector3 position, float radius)
    {
        if (_canBeDistracted == false)
        {
            return;
        }
        
        if (WithinDistance(_transform.position, position, radius))
        {
            _distractedState.position = position;
            SetState(AiState.eventDistracted);
        }
    }
    
    private void IncomingStun(GameObject id)
    {
        if (_canBeStunned == false)
        {
            return;
        }
    }/**/

//..............................................................................................................

    private void SetMoveToPosition(Vector3 position)
    {
        _navMeshAgent.destination = position;
    }

    private bool AtPosition()
    {
        return (_navMeshAgent.remainingDistance == 0f);
    }

    private void SetMoveSpeed(float speed)
    {
        _navMeshAgent.speed = speed;
        _navMeshAgent.acceleration = speed;
    }

    private void SetAutoBreaking(bool set)
    {
        _navMeshAgent.autoBraking = set;
    }

    private void SetStoppingDistance(float set)
    {
        _navMeshAgent.stoppingDistance = set;
    }

    private void ResetStoppingDistance()
    {
        _navMeshAgent.stoppingDistance = 0f;
    }
    

//..............................................................................................................


    private void SetNextWayPoint()
    {

        switch (_idleState.wayPointMovement)
        {
            
            case WayPointMovement.cycle:
                
                _idleState.currentWayPoint++;
                // Reset
                if (_idleState.currentWayPoint == _idleState.wayPoints.Length)
                {
                    _idleState.currentWayPoint = 0;
                }
                
                break;
            
            
            case WayPointMovement.cycleReversed:
                
                _idleState.currentWayPoint--;
                // Reset
                if (_idleState.currentWayPoint < 0)
                {
                    _idleState.currentWayPoint = _idleState.wayPoints.Length-1;
                }
                
                break;
            
            
            case WayPointMovement.patrol:
            case WayPointMovement.patrolReversed:

                if (_idleState.patrolUp == true)
                {
                    _idleState.currentWayPoint++;
                }
                else
                {
                    _idleState.currentWayPoint--;
                }
                
                // Reset
                if (_idleState.currentWayPoint < 0)
                {
                    _idleState.patrolUp = !_idleState.patrolUp;
                    _idleState.currentWayPoint += 2;
                }
                
                if (_idleState.currentWayPoint == _idleState.wayPoints.Length)
                {
                    _idleState.patrolUp = !_idleState.patrolUp;
                    _idleState.currentWayPoint -= 2;
                }
                
                break;
            
            
            case WayPointMovement.random:

                int previousWaypoint = _idleState.currentWayPoint;

                do 
                {
                    _idleState.currentWayPoint = Random.Range(0, _idleState.wayPoints.Length);
                    
                } while (_idleState.currentWayPoint == previousWaypoint);
                
                break;
            
        }
        
    }

    private float GetWayPointTimer()
    {
        Vector2 interval = _idleState.wayPointSettings[_idleState.currentWayPoint].waitTimeInterval;
        float timer = Random.Range(interval.x, interval.y);

        return timer;
    }
    
    private bool RotateToLookForPlayer()
    {
        
        _currentRotatedAmount += _rotationSpeed * Time.deltaTime;
        if (_currentRotatedAmount >= _rotationToDo)
        {
            ResetCurrentRotatedAmount();
            return true;
        }

        _transform.Rotate(Vector3.up, _rotationSpeed * (int)_rotationalDirection * Time.deltaTime);
        
        return false;
    }

    private void SetRotationSettings(float speed, float rotation, RotationalDirection direction)
    {
        _rotationSpeed = speed;
        _rotationToDo = rotation;
        _rotationalDirection = direction;
    }

    private void SetRotationSettingsWithCalculations(Vector3 position, float rotationSpeed)
    {
        float rotationToDo = CalculateNeededRotation(position);
        int direction = CalculateShortestRotationalDirection(position);

        ResetCurrentRotatedAmount();
        SetRotationSettings(rotationSpeed, rotationToDo, (RotationalDirection)direction);
    }
    

    private void ResetCurrentRotatedAmount()
    {
        _currentRotatedAmount = 0;
    }

    private void SetRotationalSequence(RotationalSequence[] sequenceArray)
    {
        SetRotationSettings(GetRotationalSequenceSpeed(sequenceArray), 
        GetRotationalSequenceRotation(sequenceArray), 
        GetRotationalSequenceRotationalDirection(sequenceArray));
    }

    private bool SetNextRotationalSequenceIndex(RotationalSequence[] sequenceArray) // TODO
    {
        _currentRotationalSequenceIndex++;
        if (_currentRotationalSequenceIndex >= sequenceArray.Length)
        {
            ResetCurrentRotationalSequenceIndex();
            return false;
        }

        return true;
    }
    
    private void ResetCurrentRotationalSequenceIndex()
    {
        _currentRotationalSequenceIndex = 0;
    }

    private float GetRotationalSequenceSpeed(RotationalSequence[] sequence)
    {
        return sequence[_currentRotationalSequenceIndex].rotationSpeed;
    }
    
    private float GetRotationalSequenceRotation(RotationalSequence[] sequence)
    {
        return sequence[_currentRotationalSequenceIndex].rotationToDo;
    }
    
    private RotationalDirection GetRotationalSequenceRotationalDirection(RotationalSequence[] sequence)
    {
        return sequence[_currentRotationalSequenceIndex].rotationalDirection;
    }

    private float GetRotationalSequenceWaitTimeIntervalTimer(RotationalSequence[] sequence)
    {
        Vector2 interval = sequence[_currentRotationalSequenceIndex].waitTimeInterval;
        float timer = Random.Range(interval.x, interval.y);

        return timer;
    }
    
    private void LookAtPlayer()
    {
        _transform.LookAt(_playerPositionReference.Value);
    }

    private bool CountDownTimer()
    {
        _currentCountDownTimer -= Time.deltaTime;
        if (_currentCountDownTimer <= 0f)
        {
            return true;
        }

        return false;
    }

    private void ResetCountDownTimer(float reset)
    {
        _currentCountDownTimer = reset;
    }

    private bool WayPointCountDownTimer()
    {
        _currentWayPointCountDownTimer -= Time.deltaTime;
        if (_currentWayPointCountDownTimer <= 0f)
        {
            return true;
        }

        return false;
    }
    
    private void ResetWayPointCountDownTimer(float reset)
    {
        _currentWayPointCountDownTimer = reset;
    }

    private bool ExclamationMarkTimer(bool countDown)
    {
        if (countDown == true)
        {
            _currentExclamationMarkTimer -= Time.deltaTime;
            if (_currentExclamationMarkTimer <= 0)
            {
                _currentExclamationMarkTimer = 0f;
                SetCurrentExclamationMarkTimerState(true);
                return true;
            }

            SetCurrentExclamationMarkTimerState(false);
            return false;
        }
        
        _currentExclamationMarkTimer += Time.deltaTime;
        if (_currentExclamationMarkTimer >= _maxExclamationMarkTimer)
        {
            _currentExclamationMarkTimer = _maxExclamationMarkTimer;
            return true;
        }

        return false;
    }

    private void ResetExclamationMarkTimer()
    {
        _currentExclamationMarkTimer = _maxExclamationMarkTimer;
    }
    
    public float GetCurrentExclamationMarkTimer()
    {
        return _currentExclamationMarkTimer;
    }

    private bool GetCurrentExclamationMarkTimerState()
    {
        return _currentExclamationMarkTimerState;
    }

    private void SetCurrentExclamationMarkTimer(float set)
    {
        _currentExclamationMarkTimer = set;
    }

    private void SetCurrentExclamationMarkTimerState(bool set)
    {
        _currentExclamationMarkTimerState = set;
    }

    public float GetReactionToNextStateTimer()
    {
        return _idleState.reactionToNextStateTimer;
    }

    private bool WithinDistance(Vector3 positionA, Vector3 positionB, float distance)
    {
        if (Vector3.Distance(positionA, positionB) <= distance)
        {
            return true;
        }

        return false;
    }

    private float CalculateNeededRotation(Vector3 position)
    {
        Vector3 transformPosition = _transform.position;
        return Vector3.Angle(transform.forward.normalized, (position - transformPosition).normalized);
    }

    private int CalculateShortestRotationalDirection(Vector3 position)
    {
        Vector3 transformPosition = _transform.position;
        Vector3 axis = Vector3.Cross(transform.forward.normalized, (position - transformPosition).normalized);
        float sign = Vector3.Project(axis, Vector3.up).y;
        sign = sign / Mathf.Abs(sign);
        int direction = (int) sign;

        return direction;
    }

    private void PlayAudio(AudioClip audioClip)
    {
        if (_audioWasPlayed == true)
        {
            return;
        }
        
        _audioSource.clip = audioClip;
        _audioSource.Play();
        _audioWasPlayed = true;
    }

    private void ResetAudioWasPlayed()
    {
        _audioWasPlayed = false;
    }
    
    public void DetectedAnimationDone()
    {
        _detectedAnimationDone = true;
        _detectedAnimationIsCurrentlyPlaying = false;
    }
    
    /*
    private void OnAlertedStateOverrideMoveSpeed()
    {
        if (_onAlertedState.isAlerted == true)
        {
            if (_onAlertedState.overrideMoveSpeed == true)
            {
                SetMoveSpeed(_onAlertedState.moveSpeed); 
            }
        }
    }/**/

//..............................................................................................................

    private void OnValidate()
    {

        if (_transform == null)
        {
            _transform = GetComponent<Transform>();
        }

        if (_navMeshAgent == null)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Move to starting way point
        if (_idleState.wayPoints != null)
        {
            int arraySize = _idleState.wayPoints.Length;
            if (arraySize > 0)
            {
                if (_idleState.startingWayPoint >= arraySize)
                {
                    _idleState.startingWayPoint = arraySize - 1;
                }

                _transform.position = _idleState.wayPoints[_idleState.startingWayPoint];
            }
        }

    // Set patrol
    if (_idleState.wayPointMovement == WayPointMovement.patrol)
        {
            _idleState.patrolUp = true;
        }
        else if (_idleState.wayPointMovement == WayPointMovement.patrolReversed)
        {
            _idleState.patrolUp = false;
        }
        
    }
    

    private void OnDrawGizmos()
    {
        
        if (_useIdleState)
        {
            #if UNITY_EDITOR
            
            int loops = _idleState.wayPoints.Length;
            for (var i = 0; i < loops; i++)
            {

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_idleState.wayPoints[i], 0.5f);

                Gizmos.color = Color.white;

                if (loops == 1)
                {
                    Handles.Label(_idleState.wayPoints[0], "Position: " + i);
                }
                
                if (loops > 1)
                {
                    if (loops == 2)
                    {
                        if (i > 0)
                        {
                            continue;
                        }

                        if (_idleState.wayPointMovement != WayPointMovement.random)
                        {
                            Gizmos.DrawLine(_idleState.wayPoints[i], _idleState.wayPoints[i + 1]);
                        }
                        //Gizmos.DrawLine(_idleStateWayPoints[i], _idleStateWayPoints[i + 1]);
                        Handles.Label(_idleState.wayPoints[0], "Position: " + i);
                        Handles.Label(_idleState.wayPoints[i + 1], "Position: " + 1);
                        continue;
                    }

                    if (i == loops - 1)
                    {
                        if (_idleState.wayPointMovement == WayPointMovement.cycle || _idleState.wayPointMovement == WayPointMovement.cycleReversed
                            && _idleState.wayPointMovement != WayPointMovement.random)
                        {
                            Gizmos.DrawLine(_idleState.wayPoints[i], _idleState.wayPoints[0]); 
                        }
                        Handles.Label(_idleState.wayPoints[i], "Position: " + i);
                        continue;
                    }

                    if (_idleState.wayPointMovement != WayPointMovement.random)
                    {
                        Gizmos.DrawLine(_idleState.wayPoints[i], _idleState.wayPoints[i + 1]);
                    }

                    Handles.Label(_idleState.wayPoints[i], "Position: " + i);
                }
            }
            #endif
        }

        /*
        if (_useAlertState)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_transform.position, _alertState.radius);
        }/**/


        if (_navMeshAgent.hasPath == true)
        {
            Gizmos.color = Color.green;

            Vector3 position = _navMeshAgent.destination;
            Vector3 position1 = new Vector3(position.x, position.y, position.z);
            position1.y -= 3;
            Vector3 position2 = new Vector3(position.x, position.y, position.z);
            position2.y += 5;
            
            Gizmos.DrawLine(position1, position2);
        }

        if (GetCurrentState() == AiState.internalTransitionalLookAt)
        {
            Gizmos.color = Color.magenta;

            Vector3 position = _internalTransitionalLookAtState.position;
            Vector3 position1 = new Vector3(position.x, position.y, position.z);
            position1.y -= 3;
            Vector3 position2 = new Vector3(position.x, position.y, position.z);
            position2.y += 5;
            
            Gizmos.DrawLine(position1, position2);
        }

    }
}
