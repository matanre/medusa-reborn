using System;
using UnityEngine;

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

    private float _defaultYScale;
    public bool IsCrouching { get; internal set; }

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

    void Update()
    {
        // 1. Handle the Ceiling Check (prevent standing up in low tunnels)
        if (!IsCrouching)
        {
            // If the player wants to stand up, check if there's a ceiling right above them
            // (Using a Physics CheckSphere or Raycast upward from the head)
            bool blockedAbove = Physics.CheckSphere(headTransform.position, overheadClearance, whatIsCeiling);

            if (blockedAbove)
            {
                // Force stay crouching until clear
                IsCrouching = true;
            }
        }

        // 2. Determine target scale
        var targetYScale = IsCrouching ? (_defaultYScale * crouchHeightMultiplier) : _defaultYScale;

        // 3. Smoothly transition scale using MoveTowards (safe from frame-rate snapping)
        float currScale = transform.localScale.y;
        float updatedScale = Mathf.MoveTowards(currScale, targetYScale, Time.deltaTime * crouchTransitionSpeed);

        // Apply the scale to the local transform (Y axis only)
        transform.localScale = new Vector3(transform.localScale.x, updatedScale, transform.localScale.z);
    }
}