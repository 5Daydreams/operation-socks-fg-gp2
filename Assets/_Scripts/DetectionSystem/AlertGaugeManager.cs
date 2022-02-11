using _Code.Scriptables.TrackableValue;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace _Scripts.DetectionSystem
{
    public class AlertGaugeManager : MonoBehaviour
    {
        [Range(0.01f, 10.0f)] [SerializeField] private float alertBuildupRate = 1.0f;
        [Range(0.01f, 10.0f)] [SerializeField] private float alertReduceRate = 1.0f;
        [SerializeField] private AI AIControllerRef;
        [SerializeField] private ExclamationShaderController shaderController;

        private float alertGaugeValue;
        private bool increaseAlert;

        private float CalculateExclamationValue()
        {
            float maxTime = AIControllerRef.GetReactionToNextStateTimer();
            float currTime = AIControllerRef.GetCurrentExclamationMarkTimer();

            float fill = (maxTime - currTime) / maxTime;

            return fill;
        }

        private void Update()
        {
            alertGaugeValue = CalculateExclamationValue();
            alertGaugeValue = Mathf.Clamp01(alertGaugeValue);
            shaderController.SetExclamationValue(alertGaugeValue);
        }
    }
}