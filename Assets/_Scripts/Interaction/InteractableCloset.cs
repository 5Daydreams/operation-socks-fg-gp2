using _Scripts.Interaction;
using UnityEngine;

public class InteractableCloset : MonoBehaviour, Interactable
{
    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        audioSource.Play();
    }
}
