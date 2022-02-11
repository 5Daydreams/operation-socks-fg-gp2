using System.Collections;
using _Code.Scriptables.TrackableValue;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    [SerializeField] private TrackableFloat timerSO;
    [SerializeField] private TMPro.TMP_Text timerLabel;

    void Start()
    {
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        while (true)
        {
            timerSO.AddToValue(1);
            string minutes = Mathf.FloorToInt(timerSO.Value / 60).ToString("00"); 
            string seconds = Mathf.FloorToInt(timerSO.Value % 60).ToString("00");
            timerLabel.text = $"{string.Format("{00:00}:{01:00}", minutes, seconds)}";
            yield return new WaitForSeconds(1);
        }
    }
}
