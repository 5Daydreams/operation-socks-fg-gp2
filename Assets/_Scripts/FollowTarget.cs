using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _initialTarget;
    [Range(2.0f,20.0f)][SerializeField] private float _followOffset;
    [Range(2.0f,10.0f)][SerializeField] private float _followSpeed;
    private Transform _target;

    void Start()
    {
        _target = _initialTarget;
    }

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    void LateUpdate()
    {
        Vector3 followerPosition = _target.position + _followOffset * new Vector3(0,1,-1);

        this.transform.position = Vector3.Lerp(this.transform.position, followerPosition,Time.deltaTime * _followSpeed);
    }
}
