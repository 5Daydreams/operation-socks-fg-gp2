using System;
using UnityEngine;
using UnityEngine.Events;

public class FrameEventTrigger : MonoBehaviour
{
    [Serializable]
    public struct FrameEventReference
    {
        public string name;
        public UnityEvent callback;

        public void Trigger()
        {
            callback.Invoke();
        }
    }

    [SerializeField] private FrameEventReference[] frameEvents;

    void TriggerByName(string eventName)
    {
        if (frameEvents.Length == 0)
        {
            throw new IndexOutOfRangeException("No frame events are available.");
        }

        for (int i = 0; i < frameEvents.Length; i++)
        {
            if (frameEvents[i].name != eventName)
            {
                continue;
            }

            frameEvents[i].Trigger();
        }
    }
}