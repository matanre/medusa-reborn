using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float pushSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private float gravityScale = 2.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 15.0f; // Speed at which player smoothly rotates to face movement direction

    [Header("Ground Check Settings")]
    public bool isGrounded = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.05f, 0f);

    public bool IsGrounded { get; private set; }
    public bool IsCrouching => crouchHandler.IsCrouching;
    public bool IsPushing { get; private set; }
    public bool FacingRight { get; private set; } = true;
    public float ExternalSpeedMultiplier { get; set; } = 1.0f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerCrouch crouchHandler;
    private Vector2 moveInput;
    private bool jumpRequested;
    //private bool crouchPressed;
    //private InputAction crouchAction;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        crouchHandler = GetComponentInChildren<PlayerCrouch>();
    }

    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        InputAction crouchAction = playerInput.actions["Crouch"];
        crouchHandler.Init(crouchAction);
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        isGrounded = IsGrounded; // debug check
        ApplyMovement();
        ApplyCustomGravity();

        if (jumpRequested)
        {
            ExecuteJump();
            jumpRequested = false;
        }
    }

    private void Update()
    {
        HandleFacingDirection();
        //CheckCrouch();
    }

    // --- NEW INPUT SYSTEM MESSAGE CALLBACKS ---
    // Automatically called by the Player Input component if set to "Send Messages"

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded && !IsCrouching)
        {
            jumpRequested = true;
        }
    }

    ////public void OnCrouch(InputValue value)
    ////{
    ////    if (value.isPressed && IsGrounded)
    ////    {
    ////        crouchPressed = true;
    ////    }
    ////}

    //// ------------------------------------------

    //private void CheckCrouch()
    //{
    //    if (crouchAction != null && IsGrounded)
    //    {
    //        crouchHandler.CrouchPressed = crouchAction.IsPressed();
    //    }
    //}

    private void CheckGroundStatus()
    {
        if (groundCheck != null)
        {
            // CheckSphere returns true if any collider on groundLayer intersects the sphere
            IsGrounded = Physics.CheckSphere(groundCheck.position + groundCheckOffset, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback Raycast using Capsule Collider bottom
            float rayLength = (capsuleCollider.height * 0.5f) + 0.15f;
            IsGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Renders ground check sphere in Scene view: GREEN when grounded, RED when airborne
        Vector3 checkPos = groundCheck != null ? groundCheck.position + groundCheckOffset : transform.position;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }


    private void ApplyMovement()
    {
        float currentSpeed = moveSpeed * ExternalSpeedMultiplier;
        if (IsCrouching) currentSpeed *= crouchSpeedMultiplier;

        // moveInput.x = Left/Right (X axis)
        // moveInput.y = Forward/Backward (Z axis in 3D space)
        Vector3 targetVelocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, moveInput.y * currentSpeed);
        
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            Debug.Log($"{name}: inputs (x, y) = ({moveInput.x}, {moveInput.y})");
        }

        rb.linearVelocity = targetVelocity;
    }

    private void HandleFacingDirection()
    {
        // 1. Guard Clause: Stop immediately if there is no significant input
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        Vector3 targetDirection = Vector3.zero;

        // 2. Determine cardinal direction based on the stronger axis input
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            if (moveInput.x > 0.1f)
                targetDirection = Vector3.right;    // Face Right (+X)
            else if (moveInput.x < -0.1f)
                targetDirection = Vector3.left;     // Face Left (-X)
        }
        else
        {
            if (moveInput.y > 0.1f)
                targetDirection = Vector3.forward;  // Face Away / Deep (+Z)
            else if (moveInput.y < -0.1f)
                targetDirection = Vector3.back;     // Face Camera / Down (-Z)
        }

        // 3. Smoothly and safely rotate towards target direction without spinning bugs
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // RotateTowards prevents quaternion flipping / 360-degree spin glitches
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * 50f * Time.deltaTime // Multiplied by 50 to match typical degrees/sec speeds
            );

            // Ensure physics doesn't retain unwanted angular spin
            if (rb != null)
            {
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    //private void HandleFacingDirection()
    //{
    //    // 1. Guard Clause: Stop immediately if there is no significant input (deadzone threshold)
    //    if (moveInput.sqrMagnitude < 0.01f)
    //        return;

    //    Vector3 targetDirection = Vector3.zero;

    //    // 2. Determine cardinal direction based on the stronger axis input
    //    if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
    //    {
    //        // Primary horizontal movement (A / D)
    //        if (moveInput.x > 0.1f)
    //            targetDirection = Vector3.right;    // Face Right (+X)
    //        else if (moveInput.x < -0.1f)
    //            targetDirection = Vector3.left;     // Face Left (-X)
    //    }
    //    else
    //    {
    //        // Primary depth movement (W / S)
    //        if (moveInput.y > 0.1f)
    //            targetDirection = Vector3.forward;  // Face Away / Deep (+Z)
    //        else if (moveInput.y < -0.1f)
    //            targetDirection = Vector3.back;     // Face Camera / Down (-Z)
    //    }

    //    // 3. Smoothly rotate towards target direction
    //    if (targetDirection != Vector3.zero)
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //}


    private void ExecuteJump()
    {
        if (!IsGrounded)
        {
            Debug.Log($"{name}: Cannot jump whilst not grounded.");
            return;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyCustomGravity()
    {
        if (!IsGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityScale - 1f) * rb.mass);
        }
    }
}