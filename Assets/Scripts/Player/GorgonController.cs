using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class GorgonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private float gravityScale = 2.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 15.0f; // Speed at which player smoothly rotates to face movement direction

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")]
    [SerializeField] private float normalHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private Vector3 normalCenter = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, -0.5f, 0);

    public bool IsGrounded { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool FacingRight { get; private set; } = true;
    public float ExternalSpeedMultiplier { get; set; } = 1.0f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector2 moveInput;
    private bool jumpRequested;
    private bool crouchPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
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
        HandleCrouchState();
        HandleFacingDirection();
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

    public void OnCrouch(InputValue value)
    {
        crouchPressed = value.isPressed;
    }

    // ------------------------------------------

    private void CheckGroundStatus()
    {
        if (groundCheck != null)
        {
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            IsGrounded = Physics.Raycast(transform.position, Vector3.down, normalHeight * 0.5f + 0.1f, groundLayer);
        }
    }

    
    private void ApplyMovement()
    {
        float currentSpeed = moveSpeed * ExternalSpeedMultiplier;
        if (IsCrouching) currentSpeed *= crouchSpeedMultiplier;

        // moveInput.x = Left/Right (X axis)
        // moveInput.y = Forward/Backward (Z axis in 3D space)
        Vector3 targetVelocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, moveInput.y * currentSpeed);

        rb.linearVelocity = targetVelocity;
    }

    private void HandleFacingDirection()
    {
        // Create a 3D movement direction vector on the X-Z plane
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // Only rotate if there is actual input to prevent snapping back to 0 when stopping
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // Calculate target rotation towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Smoothly interpolate (Slerp) rotation on the Y-axis
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Keep FacingRight updated if needed for other logic
            FacingRight = moveInput.x >= 0f;
        }
    }

    //private void ApplyMovement()
    //{
    //    float currentSpeed = moveSpeed * ExternalSpeedMultiplier;
    //    if (IsCrouching) currentSpeed *= crouchSpeedMultiplier;

    //    rb.linearVelocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, 0f);
    //}

    //private void HandleFacingDirection()
    //{
    //    if (moveInput.x > 0.1f && !FacingRight)
    //    {
    //        FacingRight = true;
    //        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    //    }
    //    else if (moveInput.x < -0.1f && FacingRight)
    //    {
    //        FacingRight = false;
    //        transform.rotation = Quaternion.Euler(0f, -90f, 0f);
    //    }
    //}

    private void ExecuteJump()
    {
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

    private void HandleCrouchState()
    {
        if (crouchPressed && !IsCrouching)
        {
            IsCrouching = true;
            capsuleCollider.height = crouchHeight;
            capsuleCollider.center = crouchCenter;
        }
        else if (!crouchPressed && IsCrouching)
        {
            bool ceilingAbove = Physics.Raycast(transform.position, Vector3.up, normalHeight * 0.6f);
            if (!ceilingAbove)
            {
                IsCrouching = false;
                capsuleCollider.height = normalHeight;
                capsuleCollider.center = normalCenter;
            }
        }
    }
}