using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Interaction;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class InteractableSock : MonoBehaviour, Interactable
{
    [SerializeField] private SockData data;
    [SerializeField] private UnityEvent interactCallback;
    private AudioSource audioSource;
    
    private Coroutine interactRoutine;

    public void Activate()
    {
        gameObject.SetActive(true);
        data.RegisterLevel(this);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact(GameObject obj, EaseSock sock)
    {
        if (interactRoutine == null)
        {
            sock.EaseIT();
            interactRoutine = StartCoroutine(PlayInteraction(obj));
        }
    }
    
    private IEnumerator PlayInteraction(GameObject obj)
    {
        audioSource.Play();
        data.AddSock(this);
        interactCallback.Invoke();
        yield return new WaitForSeconds(audioSource.clip.length);
        obj.SetActive(false);
        gameObject.SetActive(false);
        interactRoutine = null;
    }

    public void Interact()
    {
        
    }
}
