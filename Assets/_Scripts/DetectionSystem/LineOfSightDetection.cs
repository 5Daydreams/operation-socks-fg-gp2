using _Code.Scriptables.TrackableValue;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Events;


public class LineOfSightDetection : MonoBehaviour
{
    [Header(
        "Disclaimer: the view cone targets the player's CENTER. Not the collider, \nso the cone angle is actually 'smaller' than it looks")]
    [Header("Viewcone Parameters")]
    [SerializeField]
    private Transform raycastOrigin;

    [SerializeField] private ConeViewDepthReader coneVisual;
    [SerializeField] private Vector2 viewAngles;
    [SerializeField] private float viewRange;

    [Header("Player References")] [SerializeField]
    private Trackable<Vector3> playerPosTrackable;

    [SerializeField] private string playerTag;

    [Header("Callback if player within range and visible")] [SerializeField]
    private UnityEvent<bool> callback;

    private Vector3 origin;
    private bool raycastFoundPlayer;

    private void Awake()
    {
        if (raycastOrigin == null)
        {
            raycastOrigin = this.transform;
        }

        origin = raycastOrigin.position;
        coneVisual.MaxDistance = viewRange;
        coneVisual.SightAngle = viewAngles.x;
        coneVisual.ApplyChanges();
    }

    bool PlayerWithinVisionCone()
    {
        Vector3 directionToTaget = playerPosTrackable.Value - origin;

        if (directionToTaget.sqrMagnitude > viewRange * viewRange)
        {
            return false;
        }

        // horizontal-vec && vertical-plane-projected-vector
        Vector3 forwardDirectionFlat = (transform.rotation * Vector3.forward);
        forwardDirectionFlat.y = 0;

        Vector3 directionToTagetFlat = directionToTaget;
        directionToTagetFlat.y = 0;
        directionToTagetFlat.Normalize();

        float dotValueFlat = Mathf.Abs(Vector3.Angle(forwardDirectionFlat.normalized, directionToTagetFlat.normalized));
        float dotValueVert = Mathf.Abs(Vector3.Angle(directionToTaget.normalized, directionToTagetFlat.normalized));

        // Divide the angles by two to match the gizmos:
        // the angle is "how wide" the cones are, but half of the angle is the maximum angle the target can have 
        // towards the vision cone
        bool horizontalAngleCheck = dotValueFlat < Mathf.Abs(0.5f * viewAngles.x);
        bool verticalAngleCheck = dotValueVert < Mathf.Abs(0.5f * viewAngles.y);

        bool withinAngles = (horizontalAngleCheck && verticalAngleCheck);

        if (!withinAngles)
        {
            Debug.DrawRay(origin, directionToTaget, Color.red);
            return false;
        }

        Ray ray = new Ray(origin, directionToTaget);
        RaycastHit hit;

        Color rayColor = Color.red;
        // Raycast up to the viewRange of the AI
        if (Physics.Raycast(ray, out hit, viewRange))
        {
            raycastFoundPlayer = hit.collider.gameObject.CompareTag(playerTag);
            if (raycastFoundPlayer)
            {
                rayColor = Color.green;
            }
        }
        else
        {
            raycastFoundPlayer = false;
        }

        Debug.DrawRay(origin, directionToTaget, rayColor);

        return raycastFoundPlayer;
    }

    private void FixedUpdate()
    {
        origin = raycastOrigin.position;

        raycastFoundPlayer = PlayerWithinVisionCone();
        callback.Invoke(raycastFoundPlayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerPosTrackable == null)
        {
            return;
        }

        Vector3 gizmoOrigin;

        if (raycastOrigin != null)
        {
            gizmoOrigin = raycastOrigin.position;
        }
        else
        {
            gizmoOrigin = this.transform.position;
        }

        Quaternion VerticalRot1 = Quaternion.AngleAxis(viewAngles.y * 0.5f, transform.right);
        Quaternion VerticalRot2 = Quaternion.AngleAxis(viewAngles.y * 0.5f, -transform.right);

        Vector3 cachedV1 = VerticalRot1 * transform.forward * viewRange;
        Vector3 cachedV2 = VerticalRot2 * transform.forward * viewRange;

        Quaternion horizontalRot1 = Quaternion.AngleAxis(viewAngles.x * 0.5f, -transform.up);
        Quaternion horizontalRot2 = Quaternion.AngleAxis(viewAngles.x * 0.5f, transform.up);

        Vector3 cachedH11 = horizontalRot1 * cachedV1;
        Vector3 cachedH12 = horizontalRot2 * cachedV1;
        Vector3 cachedH21 = horizontalRot1 * cachedV2;
        Vector3 cachedH22 = horizontalRot2 * cachedV2;

        Gizmos.color = Color.green;

        // I am very much aware of how badly hardcoded this is, I just didn't want to deal with the for-loop shenanigans
        Gizmos.DrawLine(gizmoOrigin, gizmoOrigin + cachedH11);
        Gizmos.DrawLine(gizmoOrigin, gizmoOrigin + cachedH12);
        Gizmos.DrawLine(gizmoOrigin, gizmoOrigin + cachedH21);
        Gizmos.DrawLine(gizmoOrigin, gizmoOrigin + cachedH22);

        Gizmos.DrawLine(gizmoOrigin + cachedH11, gizmoOrigin + cachedH12);
        Gizmos.DrawLine(gizmoOrigin + cachedH12, gizmoOrigin + cachedH22);
        Gizmos.DrawLine(gizmoOrigin + cachedH22, gizmoOrigin + cachedH21);
        Gizmos.DrawLine(gizmoOrigin + cachedH21, gizmoOrigin + cachedH11);

        Gizmos.DrawLine(gizmoOrigin + transform.forward * viewRange, gizmoOrigin + cachedH11);
        Gizmos.DrawLine(gizmoOrigin + transform.forward * viewRange, gizmoOrigin + cachedH12);
        Gizmos.DrawLine(gizmoOrigin + transform.forward * viewRange, gizmoOrigin + cachedH21);
        Gizmos.DrawLine(gizmoOrigin + transform.forward * viewRange, gizmoOrigin + cachedH22);

        Gizmos.DrawLine(gizmoOrigin, gizmoOrigin + transform.forward * viewRange);
    }
}