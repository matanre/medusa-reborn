using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider))]
public class CameraSwitchZone : MonoBehaviour
{
    public static CinemachineCamera ActiveCamera { get; private set; }

    [Header("References")]
    [Tooltip("The Cinemachine Camera to switch to when the player enters this zone.")]
    public CinemachineCamera targetCamera;

    [Header("Settings")]
    [Tooltip("Cooldown in seconds before the camera switch can happen again.")]
    public float cooldownTime = 4.0f;

    [Header("Debug")]
    public Color gizmoColor = Color.mediumPurple * new Color(1,1,1,0.2f);

    // Static timer shared globally across all CameraSwitchZone instances
    private static float _lastSwitchTime = float.MinValue;

    private bool _isInsideZone = false;

    private void OnTriggerStay(Collider other)
    {
        if (ActiveCamera == targetCamera) return;

        if (other.CompareTag("Player"))
        {
            _isInsideZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isInsideZone = false;
    }

    private void LateUpdate()
    {
        if (!_isInsideZone) return;

        if (Time.time - _lastSwitchTime < cooldownTime)
        {
            return;
        }

        if (targetCamera == null)
        {
            Debug.LogWarning($"Target Camera is not assigned on trigger zone: {gameObject.name}", this);
            return;
        }

        // Find all Cinemachine cameras in the scene, set them to 0, and activate the target one
        CinemachineCamera[] allCameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in allCameras)
        {
            cam.Priority = (cam == targetCamera) ? 11 : 0;
        }

        // Record the global switch time
        _lastSwitchTime = Time.time;
        ActiveCamera = targetCamera;
    }

    // Automatically draws this zone's collider gizmo in the Scene view
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Color wireColor = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);

            if (col is BoxCollider box)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.color = wireColor;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
                Gizmos.color = wireColor;
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}