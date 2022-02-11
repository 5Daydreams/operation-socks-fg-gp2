using UnityEngine;

public class Example : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.ONGameRestart += GameRestart;
    }

    private void OnDisable()
    {
        EventManager.ONGameRestart -= GameRestart;
    }


    private void GameRestart()
    {
        // all code that will be called when event is invoked
    }

    private void CallTheEvent()
    {
        EventManager.Instance.InvokeOnGameRestart();
    }
}
