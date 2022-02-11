using System;
using _Code.Scriptables.TrackableValue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code.Socks.PlayerMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementTopDown : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TrackableVector3 playerRaycastPosReference;
        
        [Header("Horizontal Movement")] 
        [Range(1.0f, 10.0f)] [SerializeField] private float baseSpeed = 5.0f;
        [Range(1.0f, 10.0f)] [SerializeField] private float turnSpeed = 5.0f;

        [Header("Sneak Movement")]
        [Range(1.0f, 10.0f)] [SerializeField] private float baseSneakSpeed = 3.0f;

        [Header("Jump/Fall")]
        [Range(1.0f, 10.0f)] [SerializeField] private float gravity = 3.0f;
        [Range(1.0f, 10.0f)] [SerializeField] private float maxFallSpeed = 5.0f;

        [Header("Roll")] 
        [Range(0.5f, 5.0f)] [SerializeField] private float rollSpeedMultiplier;
        [SerializeField] private float rollCooldown;

        private CharacterController controller;
        private Animator animator;
        
        private float speed = 1.0f;
        private Vector3 inputValue;

        private float rollTimer;
        private bool pressedRoll;
        private bool canRoll;
        
        [SerializeField] private TrackableBool isSneakinig;
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsSneaking = Animator.StringToHash("IsSneakinig");

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (playerRaycastPosReference == null)
            {
                throw new ArgumentNullException("Player is not updating it's position to the AI, please insert the playerPos scriptable into the inspector reference");
            }
        }

        public void Move(InputAction.CallbackContext action)
        {
            inputValue.x = action.ReadValue<Vector2>().normalized.x;
            inputValue.z = action.ReadValue<Vector2>().normalized.y;
        }

        public void TrySneak(InputAction.CallbackContext action)
        {
            isSneakinig.Value = action.phase == InputActionPhase.Performed;
        }

        public void TryRoll(InputAction.CallbackContext action)
        {
            pressedRoll = action.ReadValue<float>() > 0f;
        }
        
        private void VerticalMovementLogic()
        {
            if (controller.isGrounded)
            {
                inputValue.y = 0.0f;
            }
            else
            {
                inputValue.y -= gravity * Time.deltaTime;
                inputValue.y = Mathf.Clamp(inputValue.y, -maxFallSpeed, inputValue.y);
            }
        }

        private void AdjustRotation()
        {
            // vector3.y is the "up" vector, I am removing it and making a new vector
            // which is the "horizontal components" of the character movement 
            Vector3 horizontalMovement = new Vector3(inputValue.x, 0, inputValue.z);
            bool noHorizontal = Mathf.Approximately(horizontalMovement.magnitude, 0);

            // I gotta do this check, otherwise the character will face Vector3.Right
            // because of how Mathf.atan2() works
            if (noHorizontal)
            {
                return;
            }

            float faceRotationAngle = Mathf.Atan2(horizontalMovement.z, horizontalMovement.x) * Mathf.Rad2Deg;
            // the -90 degress is there to match the player's 3D model, which faces the Z axis and not the X axis
            Quaternion rotation = Quaternion.AngleAxis(faceRotationAngle-90, Vector3.down);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        }

        private void Update()
        {
            // The raycast position needs to be towards the player's body,
            // but the transform.position corresponds to the player's feet,
            // so a half-height vector is added to the raycast probing position
            playerRaycastPosReference.Value = this.transform.position + transform.up * (controller.height * 0.5f);
            playerRaycastPosReference.Value = this.transform.position + transform.up * (controller.height * 0.5f);
            
            animator.SetBool(IsRunning, inputValue.magnitude > 0);
            animator.SetBool(IsSneaking, isSneakinig.Value);
            
            ResetValues();

            if (rollTimer >= 0)
            {
                rollTimer -= Time.deltaTime;
                canRoll = rollTimer < 0; // if cooldown < 0, player can roll again
            }

            // canRoll is checking the cooldown, pressedRoll is checking for input
            if (pressedRoll && canRoll)
            {
                speed = baseSpeed * rollSpeedMultiplier;
                pressedRoll = false;
                rollTimer = rollCooldown;
            }

            if (isSneakinig.Value)
            {
                PerformSneak();
            }

            // if no input is detected and the player isn't falling, stop the movement Logic
            if (Mathf.Approximately(inputValue.magnitude, 0) && controller.isGrounded)
            {
                return;
            }

            //apply movement
            controller.Move(inputValue * speed * Time.deltaTime);

            VerticalMovementLogic();

            AdjustRotation();
        }
        
        private void ResetValues()
        {
            speed = baseSpeed;
        }

        private void PerformSneak()
        {
            speed = baseSneakSpeed;
        }
    }
}