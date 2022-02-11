using UnityEngine;
using UnityEngine.AI;

public class AINavMesh : MonoBehaviour
{
    
    [SerializeField] private Transform _movePositionTransform;
    private NavMeshAgent _navMeshAgent;

//------------------------------------------------------------------------------------
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        
        _navMeshAgent.destination = _movePositionTransform.position;
    }
}
