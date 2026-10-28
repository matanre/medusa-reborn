using System.Linq;
using UnityEngine;
using System;


[RequireComponent(typeof(FieldOfView))]
public partial class EnemySight : MonoBehaviour
{
    public FieldOfView fov;
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
        Collider[] targetsInRadius = Physics.OverlapSphere(fov.Position, fov.Distance, playerLayer);

        Collider playerCollider = targetsInRadius.Where(t => t.CompareTag("Player")).FirstOrDefault();

        if (playerCollider == null)
            return false;

        playerTransform = playerCollider.transform;
        Vector3 playerPosition = playerTransform.position;
        Vector3 dirToTarget = (playerPosition - fov.Position).normalized;

        // 2. Angle check
        if (Vector3.Angle(fov.Forward, dirToTarget) < fov.Angle / 2f)
        {
            float distToTarget = Vector3.Distance(fov.Position, playerPosition);

            // 3. Line of Sight (Raycast) check
            if (!Physics.Raycast(fov.Position, dirToTarget, distToTarget, obstacleLayer))
            {
                return true; // Player is visible!
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(fov.Position, fov.Distance);

        Vector3 leftDir = Quaternion.Euler(0, -fov.Angle / 2f, 0) * fov.Forward;
        Vector3 rightDir = Quaternion.Euler(0, fov.Angle / 2f, 0) * fov.Forward;

        Gizmos.DrawRay(fov.Position, leftDir * fov.Distance);
        Gizmos.DrawRay(fov.Position, rightDir * fov.Distance);
        Gizmos.DrawRay(fov.Position, fov.Forward * fov.Distance);
    }
}