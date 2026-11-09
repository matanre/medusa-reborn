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
    private PlayerController _ctrl;
    private float _lastClickTime = float.MinValue;

    public bool IsCrouching => _isCrouching;
    public bool CanCrouch => _ctrl.IsGrounded && !IsCrouching;
    public bool CanStandUp => _ctrl.IsGrounded && IsCrouching && !CielingTouchCheck();

    private void Awake()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Assert.NotNull(capsuleCollider, nameof(capsuleCollider));

        _ctrl = GetComponent<PlayerController>();
        Assert.NotNull(_ctrl, nameof(PlayerController));
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

        ModifyHeight();

        if (!_crouchAction.WasPressedThisFrame())
            return;

        // TODO: check if spam press prevention needed
        if (Time.time - _lastClickTime < crouchTransitionSpeed * 1.5)
        {
            // Not enough time has passed, spam detected
            return;
        }

        // Check validity of the action (either stand up or crouch)
        if (_isCrouching && CanStandUp)
        {
            _isCrouching = false;
        }
        else if (CanCrouch)
        {
            _isCrouching = true;
        }
    }

    private void ModifyHeight()
    {
        var targetYScale = _isCrouching ? (_defaultYScale * crouchHeightMultiplier) : _defaultYScale;
        float currScale = transform.localScale.y;
        float updatedScale = Mathf.MoveTowards(currScale, targetYScale, Time.deltaTime * crouchTransitionSpeed);

        transform.localScale = new Vector3(transform.localScale.x, updatedScale, transform.localScale.z);
    }
    private bool CielingTouchCheck()
    {
        //if (_isCrouching) return false;

        // If the player wants to stand up, check if there's a ceiling right above them
        // (Using a Physics CheckSphere or Raycast upward from the head)
        bool blockedAbove = Physics.CheckSphere(headTransform.position, overheadClearance, whatIsCeiling);

        if (blockedAbove)
        {
            // Force stay crouching until clear
            _isCrouching = true;
        }

        return blockedAbove;
    }
}