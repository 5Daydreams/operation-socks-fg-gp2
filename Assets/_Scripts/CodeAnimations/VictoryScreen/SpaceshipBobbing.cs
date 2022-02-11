using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipBobbing : MonoBehaviour
{
    [SerializeField] private float bobbingAmplitude;
    [SerializeField] private float bobbingSpeed;
    [SerializeField] private Vector3 oscillationDirection;
    private Vector3 startingPos;

    private void Awake()
    {
        startingPos = transform.position;
        oscillationDirection.Normalize();
    }

    void Update()
    {
        Vector3 oscillationPos = oscillationDirection * Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        this.transform.position = startingPos + oscillationPos;
    }
}
