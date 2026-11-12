using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 5.0f;
    [SerializeField] private bool restrictToXAxis = true; // Set to false if you want 3D depth pushing

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure continuous collision detection for accurate physics pushing
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnCollisionStay(Collision collision)
    {
        // Check if the object colliding with this is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Get the player's horizontal velocity direction
                Vector3 playerDir = new Vector3(playerRb.linearVelocity.x, 0f, restrictToXAxis ? 0f : playerRb.linearVelocity.z);

                if (playerDir.sqrMagnitude > 0.01f)
                {
                    // Apply a gentle force in the direction the player is pushing
                    Vector3 forceDir = new Vector3(playerDir.normalized.x, 0f, restrictToXAxis ? 0f : playerDir.normalized.z);
                    rb.AddForce(forceDir * pushForce, ForceMode.Force);
                }
            }
        }
    }
}