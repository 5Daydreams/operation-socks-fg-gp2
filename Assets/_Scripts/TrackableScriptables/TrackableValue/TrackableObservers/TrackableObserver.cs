using UnityEngine;
using UnityEngine.Events;

namespace _Code.Scriptables.TrackableValue.TrackableObservers
{
    public class TrackableObserver<T> : MonoBehaviour
    {
        [SerializeField] protected Trackable<T> _trackable;
        [SerializeField] protected UnityEvent<T> _callback;

        protected virtual void OnEnable()
        {
            _trackable.CallbackOnValueChanged.AddListener(_callback.Invoke);
        }

        protected virtual void OnDisable()
        {
            _trackable.CallbackOnValueChanged.RemoveListener(_callback.Invoke);
        }
    }
}
