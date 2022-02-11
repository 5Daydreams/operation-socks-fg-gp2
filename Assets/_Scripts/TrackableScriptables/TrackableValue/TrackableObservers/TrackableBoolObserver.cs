using UnityEngine;
using UnityEngine.Events;

namespace _Code.Scriptables.TrackableValue.TrackableObservers
{
    public class TrackableBoolObserver : TrackableObserver<bool>
    {
        [SerializeField] protected UnityEvent<bool> _invertedCallback;
        protected virtual void OnEnable()
        {
            base.OnEnable();
            _trackable.CallbackOnValueChanged.AddListener(value => _invertedCallback.Invoke(!value));
        }

        protected virtual void OnDisable()
        {
            base.OnDisable();
            _trackable.CallbackOnValueChanged.RemoveListener(value => _invertedCallback.Invoke(!value));
        }
    }
}