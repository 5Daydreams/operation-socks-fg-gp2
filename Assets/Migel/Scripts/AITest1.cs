using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;



[RequireComponent(typeof(NavMeshAgent))]
public class AITest1 : MonoBehaviour
{
    [SerializeField] private Transform _gotoTransform;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _alertRadius;
    [SerializeField] private GameObject _playerDetection;

    private NavMeshAgent _navMeshAgent;

    public Material[] Materials;

    private AiState _currentState;
    private AiState _previousState;

    private bool _playerInVision;
    private Vector3 _lastKnownPlayerPosition;

    private Transform _transform;

    private Action ExecuteState;


//...............................................................................

    private void OnEnable()
    {
        //EventManager.ONDetectPlayer += PlayerInVision;
    }

    private void OnDisable()
    {
        //EventManager.ONDetectPlayer -= PlayerInVision;
    }

//...............................................................................

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _transform = GetComponent<Transform>();
        
        
        SetState(AiState.idle);
    }


    private void Update()
    {
        // Not null, invoke
        ExecuteState?.Invoke();
    }


//..............................................................................................................


    /*private bool PlayerWithinDistance()
    {
        float distance = Vector3.Distance(_playerTransform.position, _transform.position);
        if (distance <= _alertRadius)
        {
            return true;
        }
        
        return false;
    }*/
    

    public void SetPlayerInVision(bool set)
    {
        _playerInVision = set;
    }
    private bool PlayerInVision()
    {
        return _playerInVision;
    }
    
//..............................................................................................................

    private void IdleStateStanding()
    {
        if (PlayerInVision() == true)
        {
            SetState(AiState.chase);
        }
    }

    private void ChaseState()
    {
        // Udating player position
        if (PlayerInVision() == true)
        {   
            _lastKnownPlayerPosition = _playerTransform.position;
        }

        // Goto position
        _navMeshAgent.destination = _lastKnownPlayerPosition;
        // Debug
        _gotoTransform.position = _lastKnownPlayerPosition;
        // At position
        if (_navMeshAgent.remainingDistance == 0f)
        {
            SetState(AiState.idle);
        }
    }
    

//..............................................................................................................

    private void SetState(AiState state)
    {
        _previousState = _currentState;
        _currentState = state;
        
        // Debug
        Debug.Log($"{_currentState}");
        gameObject.GetComponent<MeshRenderer>().material = Materials[(int)_currentState];
        
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

//..............................................................................................................
    
    private void SetExecutableState()
    {
        
        // Subscribe
        switch (_currentState)
        {
            case AiState.idle:
                ExecuteState = IdleStateStanding;
                break;
            
            case AiState.chase:
                ExecuteState = ChaseState;
                break;
            
        }

        // Unsubscribe
        switch (_previousState)
        {
            case AiState.idle:
                //ExecuteState -= IdleStateStanding;
                break;
            
            case AiState.chase:
                //ExecuteState -= ChaseState;
                break;
            
        }
        
    }

//..............................................................................................................

    private void OnDrawGizmos()
    {
        //Gizmos.color = Materials[(int) _currentState].color;
        //Gizmos.DrawWireSphere(transform.position, _alertRadius);

        /*if (_currentState == AIState.chase)
        {
            Vector3 position = _lastKnownPlayerPosition;
            position.y -= 2;
            
            Gizmos.color = Materials[(int) _currentState].color;
            Gizmos.DrawSphere(position, 0.5f);
        }*/
    }
    

    public AiState DebugGetAIState()
    {
        return _currentState;
    }
    
}
