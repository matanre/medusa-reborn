using NUnit.Framework.Internal;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera cam1;
    public CinemachineCamera cam2;
    public Collider switchArea;

    [Header("Settings")]
    public float switchThresholdInSeconds = 3.0f; // Cooldown between switches

    private float _switchTimer = 0f;
    private bool _isCam1Active = true;

    private void Awake()
    {
        if (switchArea == null)
            switchArea = GetComponent<Collider>();

        Assert.NotNull(switchArea, nameof(switchArea));
        Assert.NotNull(cam1, nameof(cam1));
        Assert.NotNull(cam2, nameof(cam2));

        if (!switchArea.isTrigger)
        {
            Debug.LogWarning($"{name}: {switchArea} collider must be a TRIGGER to work properly!");
        }
    }

    private void Start()
    {
        // by default, cam1 takes precedence
        // if another cam, i.e. cam3 should be default, set its priority higher than 10
        cam1.gameObject.SetActive(true);
        cam1.Priority = 10;

        cam2.gameObject.SetActive(true);
        cam2.Priority = 9;

        _switchTimer = switchThresholdInSeconds; // make sure first transition happens instantly
    }

    private void Update()
    {
        _switchTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_switchTimer < switchThresholdInSeconds)
            {
                return;
            }

            if (_isCam1Active)
            {
                SwitchForward();
            }
            else
            {
                SwitchBack();
            }
        }
    }

    private void Switch(CinemachineCamera c1, CinemachineCamera c2)
    {
        c1.Priority = 0;
        c2.Priority = 11;

        _switchTimer = 0;
        _isCam1Active = !_isCam1Active;

        Debug.Log($"SWITCH! Active camera: {c1.name} priority = {c1.Priority}");
    }

    public void SwitchForward()
    {
        Switch(cam1, cam2);
    }

    public void SwitchBack()
    {
        Switch(cam2, cam1);
    }

    private void OnDrawGizmos()
    {
        // If a switchArea collider is assigned, draw it
        if (switchArea != null)
        {
            // Set the color of the gizmo (e.g., semi-transparent green)
            Gizmos.color = new Color(.5f, 0f, 1f, 0.2f);

            // Check the type of collider and draw the appropriate shape
            if (switchArea is BoxCollider box)
            {
                // Draw a solid cube matching the box collider's size and center
                Gizmos.matrix = box.transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.DrawWireCube(box.center, box.size); // Adds a nice outline
            }
            else if (switchArea is SphereCollider sphere)
            {
                Gizmos.matrix = sphere.transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}
