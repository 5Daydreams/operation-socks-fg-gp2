using System.Collections;
using _Scripts.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class DropOff : MonoBehaviour, Interactable
{
    [SerializeField] private SockRegistry sockRegistry;
    [SerializeField] private UnityEvent callback;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void Interact()
    {
        sockRegistry.DropOff();
        audioSource.Play();
        callback.Invoke();
    }
}
