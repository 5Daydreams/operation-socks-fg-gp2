using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockReset : MonoBehaviour
{
    private static SockReset instance;
    
    void Awake() 
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
    
    
}
