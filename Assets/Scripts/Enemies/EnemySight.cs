using System.Linq;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] private FieldOfView fov;
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

        Transform enemyViewTransform;
        Vector3 enemyViewPosition;
        float viewDistance, viewAngle;
        TryGetFovParams(out enemyViewTransform, out enemyViewPosition, out viewDistance, out viewAngle);

        // 1. Distance check
        Collider[] targetsInRadius = Physics.OverlapSphere(enemyViewPosition, viewDistance, playerLayer);

        Collider playerCollider = targetsInRadius.Where(t => t.gameObject.CompareTag("Player")).FirstOrDefault();

        if (playerCollider == null)
            return false;

        playerTransform = playerCollider.transform;
        Vector3 playerPosition = playerTransform.position;
        Vector3 dirToTarget = (playerPosition - enemyViewPosition).normalized;

        // 2. Angle check
        if (Vector3.Angle(enemyViewTransform.forward, dirToTarget) < viewAngle / 2f)
        {
            float distToTarget = Vector3.Distance(enemyViewPosition, playerPosition);

            // 3. Line of Sight (Raycast) check
            if (!Physics.Raycast(enemyViewPosition, dirToTarget, distToTarget, obstacleLayer))
            {
                return true; // Player is visible!
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Transform enemyViewTransform;
        Vector3 enemyViewPosition;
        float viewDistance, viewAngle;
        TryGetFovParams(out enemyViewTransform, out enemyViewPosition, out viewDistance, out viewAngle);

        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(enemyViewPosition, viewDistance);

        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * enemyViewTransform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * enemyViewTransform.forward;

        Gizmos.DrawRay(enemyViewTransform.position, leftDir * viewDistance);
        Gizmos.DrawRay(enemyViewTransform.position, rightDir * viewDistance);
        Gizmos.DrawRay(enemyViewTransform.position, enemyViewTransform.forward * viewDistance);
    }

    private void TryGetFovParams(out Transform enemyViewTransform, out Vector3 enemyViewPosition, out float viewDistance, out float viewAngle)
    {
        enemyViewTransform = fov == null ? transform : fov.transform;
        enemyViewPosition = enemyViewTransform.position;
        viewDistance = fov == null ? 8f : fov.radius;
        viewAngle = fov == null ? 100f : fov.angle;
    }
}