using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace _Code.Scriptables.TrackableValue.TrackableObservers
{
    public class TrackableVector3Observer : TrackableObserver<Vector3>
    {
        [SerializeField] protected Vector3 _trackableVec3;
    }
}