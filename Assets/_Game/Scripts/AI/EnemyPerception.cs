using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Vision")]
    [SerializeField] private float sightRange = 8f;

    [Range(1f, 360f)]
    [SerializeField] private float fieldOfView = 110f;

    [SerializeField] private float eyeHeight = 0.5f;
    [SerializeField] private float targetHeight = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool canSeeTarget;

    public Transform Target => target;
    public bool CanSeeTarget => canSeeTarget;

    private void Update()
    {
        canSeeTarget = CheckVision();
    }

    private bool CheckVision()
    {
        if (target == null)
            return false;

        Vector3 eyePosition =
            transform.position +
            Vector3.up * eyeHeight;

        Vector3 targetPosition =
            target.position +
            Vector3.up * targetHeight;

        Vector3 directionToTarget =
            targetPosition - eyePosition;

        float distanceToTarget =
            directionToTarget.magnitude;

        // Too far away.
        if (distanceToTarget > sightRange)
            return false;

        Vector3 horizontalDirection =
            new Vector3(
                directionToTarget.x,
                0f,
                directionToTarget.z
            );

        if (horizontalDirection.sqrMagnitude < 0.001f)
            return true;

        float angleToTarget =
            Vector3.Angle(
                transform.forward,
                horizontalDirection.normalized
            );

        // Target is outside the enemy's field of view.
        if (angleToTarget > fieldOfView * 0.5f)
            return false;

        // Check whether something is blocking vision.
        if (Physics.Raycast(
            eyePosition,
            directionToTarget.normalized,
            out RaycastHit hit,
            distanceToTarget + 0.1f,
            ~0,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == target ||
                hit.transform.IsChildOf(target))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 eyePosition =
            transform.position +
            Vector3.up * eyeHeight;

        Gizmos.DrawWireSphere(
            transform.position,
            sightRange
        );

        Vector3 leftBoundary =
            Quaternion.AngleAxis(
                -fieldOfView * 0.5f,
                Vector3.up
            ) * transform.forward;

        Vector3 rightBoundary =
            Quaternion.AngleAxis(
                fieldOfView * 0.5f,
                Vector3.up
            ) * transform.forward;

        Gizmos.DrawRay(
            eyePosition,
            leftBoundary * sightRange
        );

        Gizmos.DrawRay(
            eyePosition,
            rightBoundary * sightRange
        );
    }
}