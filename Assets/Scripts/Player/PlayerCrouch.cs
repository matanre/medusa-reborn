using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;

    [Header("Crouch Settings")]
    [SerializeField] [Range(0.1f, 0.9f)] private float crouchHeightMultiplier = 0.7f;
    [SerializeField] private float crouchTransitionSpeed = 2f;

    [Header("Ceiling Check (Standing Up)")]
    public Transform headTransform; // Assign a transform positioned at the player's head
    public float overheadClearance = 0.6f; // How much space is needed to stand up
    public LayerMask whatIsCeiling;       // Layers that count as a roof/tunnel

    private bool _isInitialized = false;
    private InputAction _crouchAction;
    private float _defaultYScale;
    private bool _isCrouching;
    public bool IsCrouching => _isCrouching;

    private void Awake()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Assert.NotNull(capsuleCollider, nameof(capsuleCollider));
    }

    void Start()
    {
        // Save the normal scale of the visual model at start
        _defaultYScale = transform.localScale.y;
    }

    public void Init(InputAction crouchAction)
    {
        _crouchAction = crouchAction;
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized) return;

        if (_crouchAction.WasPressedThisFrame())
        {
            _isCrouching = !_isCrouching;
        }

        CheckCieling();

        var targetYScale = _isCrouching ? (_defaultYScale * crouchHeightMultiplier) : _defaultYScale;
        float currScale = transform.localScale.y;
        float updatedScale = Mathf.MoveTowards(currScale, targetYScale, Time.deltaTime * crouchTransitionSpeed);

        transform.localScale = new Vector3(transform.localScale.x, updatedScale, transform.localScale.z);
    }

    private void CheckCieling()
    {
        if (_isCrouching) return;

        // If the player wants to stand up, check if there's a ceiling right above them
        // (Using a Physics CheckSphere or Raycast upward from the head)
        bool blockedAbove = Physics.CheckSphere(headTransform.position, overheadClearance, whatIsCeiling);

        if (blockedAbove)
        {
            // Force stay crouching until clear
            _isCrouching = true;
        }
    }
}