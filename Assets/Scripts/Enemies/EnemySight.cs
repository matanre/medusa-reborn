using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [Header("Field of View")]
    [SerializeField] private float viewDistance = 8.0f;
    [SerializeField] private float viewAngle = 60.0f; // Total arc angle
    [SerializeField] private Transform eyesTransform;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    /// <summary>
    /// A simple 3-step check:
    /// 1. Distance Check - Is the player close enough to even be considered?
    /// 2. Angle Check - Is the player standing inside the forward view cone?
    /// 3. Line of Sight Check - Are there no obstacles in the way between the enemy and the player?
    /// </summary>
    /// <returns>TRUE if all 3 checks are correct, FALSE otherwise.</returns>
    public bool CanSeePlayer(out Transform playerTransform)
    {
        playerTransform = null;

        // 1. Distance check
        Collider[] targetsInRadius = Physics.OverlapSphere(eyesTransform.position, viewDistance, playerLayer);

        foreach (Collider target in targetsInRadius)
        {
            Vector3 dirToTarget = (target.transform.position - eyesTransform.position).normalized;

            // 2. Angle check
            if (Vector3.Angle(eyesTransform.forward, dirToTarget) < viewAngle / 2f)
            {
                float distToTarget = Vector3.Distance(eyesTransform.position, target.transform.position);

                // 3. Line of Sight (Raycast) check
                if (!Physics.Raycast(eyesTransform.position, dirToTarget, distToTarget, obstacleLayer))
                {
                    playerTransform = target.transform;
                    return true; // Player is visible!
                }
            }
        }

        return false;
    }

    // Free, built-in debug view in Scene View (Zero performance impact in built game!)
    private void OnDrawGizmosSelected()
    {
        if (eyesTransform == null) eyesTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyesTransform.position, viewDistance);

        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * eyesTransform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * eyesTransform.forward;

        Gizmos.DrawRay(eyesTransform.position, leftDir * viewDistance);
        Gizmos.DrawRay(eyesTransform.position, rightDir * viewDistance);
    }
}