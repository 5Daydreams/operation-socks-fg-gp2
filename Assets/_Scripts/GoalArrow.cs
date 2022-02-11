using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArrow : MonoBehaviour
{
    [SerializeField] private SockRegistry socks;
    private Transform ventReference;
    private bool isHoldingSocks => socks.GetCurrentCount() > 0;

    private void Awake()
    {
        ventReference = FindObjectOfType<DropOff>().transform;
    }

    void Update()
    {
        if (!isHoldingSocks)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        transform.GetChild(0).gameObject.SetActive(true);
        this.transform.LookAt(ventReference.position);
    }
}