using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Scriptables.TrackableValue;
using _Scripts.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private SockRegistry sockRegistry;
    private PlayerInput playerInput;
    
    private bool isInteracting;
    [SerializeField] private GameObject ButtonE;
    [SerializeField] private EaseSock UIsock;

    [Header("Easing the hide")] 
    [SerializeField] private TrackableBool isHiding;
    
    //[SerializeField] private EasingUtility.Mode easeMode;
    [SerializeField] private EasingUtility.Style easeStyle;
    EasingUtility.Function easeFunction;
    
    private Vector3 endPosition;
    private Vector3 prevPosition;
    
    [SerializeField] private float duration;

    private Coroutine easeRoutine;

    public void TryInteract(InputAction.CallbackContext action)
    {
        isInteracting = action.phase == InputActionPhase.Performed;
    }
    
    private void Awake()
    {
        sockRegistry.ResetInventory();
        playerInput = GetComponent<PlayerInput>();
        ButtonE.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInteracting && other.TryGetComponent<DebugInteractable>(out var debugObject))
        {
            debugObject.Interact();
        }
        
        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            Vector3 buttonPos = other.transform.position;
            buttonPos.y += other.GetComponent<Collider>().bounds.size.y;
            ButtonE.transform.position = buttonPos;
            ButtonE.SetActive(true);
        }

        if (isInteracting && other.TryGetComponent<InteractableSock>(out var interactableSock))
        {
            SockInteraction(interactableSock);
        }
        else if (isInteracting && other.TryGetComponent<DropOff>(out var dropOff))
        {
            DropOffInteraction(dropOff);
        }
        else if (isInteracting && easeRoutine == null && 
                 other.TryGetComponent<InteractableCloset>(out var interactableCloset))
        {
            if (!isHiding.Value)
            {
                HideInteraction(interactableCloset);
            }
            else
            {
                UnhideInteraction(interactableCloset);
            }
        }
        isInteracting = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            ButtonE.SetActive(false);
        }
    }

    private void HideInteraction(InteractableCloset closet)
    {
        ToggleControls();
        isHiding.Value = true;
        closet.Interact();
        
        easeFunction = EasingUtility.GetFunction(easeStyle, EasingUtility.Mode.Out);
        prevPosition = transform.position;
        endPosition = closet.transform.position + Vector3.up * 0.2f;
        easeRoutine = StartCoroutine(EasePosition(prevPosition, endPosition, false));
    }

    private void UnhideInteraction(InteractableCloset closet)
    {
        isHiding.Value = false;
        closet.Interact();
        
        easeFunction = EasingUtility.GetFunction(easeStyle, EasingUtility.Mode.In);
        easeRoutine = StartCoroutine(EasePosition(endPosition, prevPosition, true));
    }
    
    private void SockInteraction(InteractableSock sock)
    {
        if (sockRegistry.GetCurrentCount() < sockRegistry.SockInventoryCapacity)
        {
            sock.Interact(ButtonE, UIsock);
        }
    }

    private void DropOffInteraction(DropOff dropOff)
    {
        if (sockRegistry.GetCurrentCount() != 0)
        {
            dropOff.Interact();
        }
    }

    private void ToggleControls()
    {
        if (playerInput.actions["Move"].enabled)
        {
            playerInput.actions["Move"].Disable();
        }
        else
        {
            playerInput.actions["Move"].Enable();
        }
    }
    
    private IEnumerator EasePosition(Vector3 start, Vector3 end, bool toggle)
    {
        if (start == end)
        {
            yield break;
        }
        float t = 0;
        transform.position = start;
        while (t < 1)
        {
            t += Time.fixedDeltaTime / duration;
            transform.position = EasingUtility.Ease(easeFunction, start, end, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = EasingUtility.Ease(easeFunction, start, end, 1);
        if (toggle)
        {
            ToggleControls();            
        }
        easeRoutine = null;
    }

}
