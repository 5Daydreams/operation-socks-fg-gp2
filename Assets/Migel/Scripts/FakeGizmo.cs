using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGizmo : MonoBehaviour
{

    [SerializeField] private AITest1 _aiTest;
    [SerializeField] private Material _radiusMaterialIdle;
    [SerializeField] private Material _radiusMaterialChase;
    
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _aiTest.GetComponent<AITest1>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }


    private void Update()
    {
        if (_aiTest.DebugGetAIState() == AiState.idle)
        {
            _meshRenderer.material = _radiusMaterialIdle;
        }
        else
        {
            _meshRenderer.material = _radiusMaterialChase;
        }
    }
}
