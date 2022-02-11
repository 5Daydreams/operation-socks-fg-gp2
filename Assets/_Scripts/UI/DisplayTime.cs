using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    [SerializeField] private TrackableFloat timerSO;
    private TMPro.TMP_Text timerLabel;

    void Start()
    {
        timerLabel = GetComponent<TMPro.TMP_Text>();
        string minutes = Mathf.FloorToInt(timerSO.Value / 60).ToString("00"); 
        string seconds = Mathf.FloorToInt(timerSO.Value % 60).ToString("00");
        timerLabel.text = $"Your time: {string.Format("{00:00}:{01:00}", minutes, seconds)}";
    }

}
