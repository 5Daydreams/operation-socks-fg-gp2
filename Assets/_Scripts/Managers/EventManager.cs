using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static EventManager Instance;

//**********************************************************************************************

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
//**********************************************************************************************

    // PLAYER & AI

    // Player detection
    public delegate void OnDetectPlayer(GameObject id);
    public static event OnDetectPlayer ONDetectPlayer;
    public void InvokeOnDetectPlayer(GameObject id)
    {
        ONDetectPlayer?.Invoke(id);
    }
    
    // AI alert activate
    public delegate void OnAiAlertActivate(GameObject id, Vector3 position, float radius, Vector3 point);
    public static event OnAiAlertActivate ONAiAlertActivate;
    public void InvokeOnAiAlertActivate(GameObject id, Vector3 position, float radius, Vector3 point)
    {
        ONAiAlertActivate?.Invoke(id, position, radius, point);
    }
    
    // AI alert deactivate
    public delegate void OnAiAlertDeactivate(GameObject id);
    public static event OnAiAlertDeactivate ONAiAlertDeactivate;
    public void InvokeOnAiAlertDeactivate(GameObject id)
    {
        ONAiAlertDeactivate?.Invoke(id);
    }
    /*
    // AI distract 
    public delegate void OnAiDistract(Vector3 position, float radius);
    public static event OnAiDistract ONAiDistract;
    public void InvokeOnAiDistract(Vector3 position, float radius)
    {
        ONAiDistract?.Invoke(position, radius);
    }
    
    // AI stunned
    public delegate void OnAiStun(GameObject id);
    public static event OnAiStun ONAiStun;
    public void InvokeOnAiStun(GameObject id)
    {
        ONAiStun?.Invoke(id);
    }/**/

//**********************************************************************************************

    // MOVER SPAWN   
    
    // Spawn
    public delegate void OnGameRestart();
    public static event OnGameRestart ONGameRestart;
    public void InvokeOnGameRestart()
    {
        ONGameRestart?.Invoke();
    }
    
    // Despawn
    public delegate void OnMoverDespawn(GameObject id);
    public static event OnMoverDespawn ONMoverDespawn;
    public void InvokeOnMoverDespawn(GameObject id)
    {
        ONMoverDespawn?.Invoke(id);
    }


}