using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallEventManagerRestart : MonoBehaviour
{
    public void InvokeEventManagerRestart()
    {
        EventManager.Instance.InvokeOnGameRestart();
    }
    
}
