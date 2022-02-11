using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Code.Socks.PlayerMovement
{
    public class ThrowInput : MonoBehaviour
    {
        [Range(5, 30)] [SerializeField] private float maxThrowStrength = 10;
        [Range(20, 50)] [SerializeField] private float rangeTuneFactor = 40;
        [Range(0, 90)] [SerializeField] private float throwAngle;
        [SerializeField] private Transform shotOrigin;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Camera mainCam;

        // These constants seem to render a good enough line - which can be fine tuned later if necessary
        private const int MAX_RAYCAST_ITERATIONS = 30;
        private const float LINE_STEP = 0.1f;
        private const float ITERATION_FALLOF = 0.002226f;
        private float throwRangeMaxInternal => 3*Mathf.Sqrt(maxThrowStrength);

        private float throwForce = 0.0f;
        private bool isAiming;
        private Vector2 mousePos = Vector2.zero;
        private Vector3 throwSpeedVector = Vector3.positiveInfinity;

        private void Awake()
        {
            if (shotOrigin == null)
            {
                shotOrigin = this.transform;
            }

            isAiming = false;
            throwSpeedVector = CalculateThrowSpeedVector();
        }

        private void Update()
        {
            if (isAiming)
            {
                CastLinePreview();
            }
        }

        public bool PlayerIsAiming()
        {
            return isAiming;
        }

        public void RefreshPointerClick(InputAction.CallbackContext action)
        {
            if (action.canceled)
            {
                lineRenderer.enabled = false;
                isAiming = false;
            }
            else
            {
                lineRenderer.enabled = true;
                isAiming = true;
            }
        }
        
        public void RefreshPointerPos(InputAction.CallbackContext action)
        {
            Vector2 position = action.ReadValue<Vector2>();
            mousePos = position;
        }
        
        public void CastLinePreview()
        {
            throwSpeedVector = CalculateThrowSpeedVector();

            List<Vector3> pathPoints = new List<Vector3>();

            Vector3 iteration = shotOrigin.position;
            Vector3 iterationDirection = throwSpeedVector;

            for (int i = 0; i < MAX_RAYCAST_ITERATIONS; i++)
            {
                pathPoints.Add(iteration);

                Ray ray = new Ray(iteration, iterationDirection.normalized);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit, LINE_STEP))
                {
                    break;
                }

                iteration += iterationDirection * LINE_STEP;
                iterationDirection += Physics.gravity * (LINE_STEP + i * ITERATION_FALLOF);
                // The falloff is required to adjust the "artificial" line into the "real" line
                // for the physics simulation - otherwise the line renderer calls are too expensive
            }

            lineRenderer.positionCount = pathPoints.Count;
            lineRenderer.SetPositions(pathPoints.ToArray());
        }

        private Vector3 CalculateThrowSpeedVector()
        {
            Vector2 playerScreenPos = mainCam.WorldToScreenPoint(transform.position);
            Vector2 screenDeltaPos = mousePos - playerScreenPos;

            Vector3 ConvertFromScreenToWorld(Vector3 vec)
            {
                Vector3 result = new Vector3
                {
                    x = vec.x / Screen.width,
                    y = 0,
                    z = vec.y / Screen.height
                };

                return result;
            }

            Vector3 launchVec = ConvertFromScreenToWorld(screenDeltaPos).normalized;
            throwForce = ConvertFromScreenToWorld(screenDeltaPos).magnitude * rangeTuneFactor;

            Vector3 axis = Vector3.Cross(launchVec.normalized, Vector3.up);
            Quaternion rot = Quaternion.AngleAxis(throwAngle, axis);

            throwForce = Mathf.Clamp(throwForce, 0, throwRangeMaxInternal);

            return rot * (launchVec * throwForce);
        }

        public Vector3 GetThrowForceVector()
        {
            return CalculateThrowSpeedVector();
        }
    }
}