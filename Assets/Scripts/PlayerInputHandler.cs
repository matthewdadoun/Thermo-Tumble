using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerInputHandler : MonoBehaviour
{
    // This is the default input system asset.
    private PlayerInputActions _controls;

    // The rigid body of the character
    private Rigidbody _rb;

    // The move input used to determine how the player moves.
    private Vector2 _moveInput;

    /*// Whether the player is throwing or not.
    private bool _isThrowing;*/

    // The collider of this object
    private Collider _col;

    // Whether the player is holding an object
    private bool _isHolding;

    // Store last input direction
    private Vector3 _lastInputDirection;


    [FormerlySerializedAs("extraHeight")]
    [Header("Ground checking")]

    // The extra height to check for when checking if the player is grounded
    [SerializeField]
    private float extraTraceLen = 0.05f;

    [SerializeField] private LayerMask groundMask;

    // Movement tuning variables
    [Header("Movement Tuning")]

    // The speed at which the player moves on the ground
    public float moveSpeed = 8f;

    // The acceleration of the player when moving
    public float accel = 60f;

    [Header("Jump Tuning")] [SerializeField]
    private float jumpVelocity = 12f; // upward velocity on takeoff

    [SerializeField] private float jumpCutMultiplier = 0.5f; // scales v.y when you release early (0.4–0.7 seconds)
    [SerializeField] private float extraFallGravity = 25f; // extra downward acceleration when falling
    [SerializeField] private float coyoteTime = 0.12f; // grace after leaving the ground - allow the player to jump for a few frames in midair
    [SerializeField] private float jumpBuffer = 0.12f; // press slightly early, still jumps
    /*[SerializeField] private float groundProbeExtra = 0.05f; // bounds-based probe cushion*/

    // input state
    private bool _jumpHeld; // true while the button is down
    private float _jumpBufferTimeElapsed; // counts down
    private float _coyoteTimeElapsed; // counts down
    
    [Header("Hold / Grab Tuning")]
    // Store the held grabbable object
    private IGrabbable _heldGrabbable;
    // The hold point for grabbable
    [SerializeField] GameObject holdPoint;

    // call this from Awake once

    void Awake()
    {
        // Create a new player input actions object
        _controls = new PlayerInputActions();

        // Get the rigid body of the character
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        Cache();

        // Handlers for when a move is performed
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        /*_controls.Player.PickUpThrow.performed += ctx => _isThrowing = true;*/
    }


    // Jump callback - hooked to "Jump" input action
    public void OnJump(InputAction.CallbackContext ctx)
    {
        // Callbacks for when jump started, performed, or canceled
        if (ctx.started || ctx.performed)
        {
            _jumpHeld = true;
            _jumpBufferTimeElapsed = jumpBuffer;
        }

        if (ctx.canceled)
        {
            _jumpHeld = false;
        }
    }


    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (ctx.started /*|| ctx.performed*/)
        {
            // If we are holding onto a grabbable object
            if (_heldGrabbable != null)
            {
                // Perform throw
                _heldGrabbable.OnThrow(_lastInputDirection);
                _heldGrabbable = null;
                return;
            }
            
            var b = _col.bounds;

            // Cast from center of box + extra length
            var rayLen = b.extents.x + extraTraceLen;
            var origin = b.center;

            // Perform raycast, don't run if hasn't hit anything
            if (!Physics.Raycast(origin, _lastInputDirection, out var hit, rayLen/*, groundMask, QueryTriggerInteraction.Ignore*/))
            {
                return;
            }

            // If grabbable, call OnGrab
            var grabbable = hit.collider.gameObject.GetComponent<IGrabbable>();
            grabbable?.OnGrab(holdPoint.transform);
            
            // Store the held grabbable
            _heldGrabbable = grabbable;
            _isHolding = true;
        }
    }

    /*private void PerformJump(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded())
        {
            return;
        }

        // Add jumping force
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }*/

    void Cache()
    {
        if (!_rb) _rb = GetComponent<Rigidbody>();
        if (!_col) _col = GetComponent<Collider>();
    }

    void OnEnable()
    {
        _controls.Player.Enable();
    }

    void OnDisable()
    {
        _controls.Player.Disable();
    }

    /*void Update()
    {
        // Pass this info to your movement / throw logic
        Debug.Log("Move: " + _moveInput);

        if (_isJumping)
        {
            Debug.Log("Jump!");
            _isJumping = false;
        }
        if (_isThrowing)
        {
            Debug.Log("Throw!");
            _isThrowing = false;
        }
    }
    */

    // Callback for whether the character is grounded
    bool IsGrounded()
    {
        var b = _col.bounds;

        // Cast from the center straight down to just past the bottom of the bounds
        float rayLen = b.extents.y + extraTraceLen;
        Vector3 origin = b.center;

        if (Physics.Raycast(origin, Vector3.down, out var hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(origin, origin + Vector3.down * rayLen, Color.red, 0.1f);

            // optional: require an "up-ish" surface to avoid walls
            return Vector3.Dot(hit.normal, Vector3.up) > 0.5f;
        }

        return false;
    }

    private void FixedUpdate()
    {
        // Update movement / jump logic
        UpdateMove();
        UpdateJump();
    }

    // To be called every fixed update for movement logic
    private void UpdateMove()
    {
        var currentVel = _rb.linearVelocity;

        // Store velocity X
        var targetVelX = _moveInput.x * moveSpeed;

        // Speed up velocity using acceleration
        currentVel.x = Mathf.MoveTowards(currentVel.x, targetVelX, accel * Time.fixedDeltaTime);

        if (currentVel.magnitude > 0.5f)
        {
            // Store the last input direction
            _lastInputDirection = currentVel.normalized;
        }

        // Store current velocity
        _rb.linearVelocity = currentVel;
    }

    // To be called every fixed update for jump logic
    private void UpdateJump()
    {
        // Update coyote and jump buffer times
        _coyoteTimeElapsed = IsGrounded() ? coyoteTime : Mathf.Max(0f, _coyoteTimeElapsed - Time.fixedDeltaTime);
        _jumpBufferTimeElapsed = Mathf.Max(0f, _jumpBufferTimeElapsed - Time.fixedDeltaTime);

        // --- Consume buffered jump if within the coyote window
        if (_jumpBufferTimeElapsed > 0f && _coyoteTimeElapsed > 0f)
        {
            // Perform a crisp takeoff for 1 frame
            var currentVel = _rb.linearVelocity;
            currentVel.y = jumpVelocity;

            // Update velocity
            _rb.linearVelocity = currentVel;

            // Reset the jump / coyote time buffers
            _jumpBufferTimeElapsed = 0f;
            _coyoteTimeElapsed = 0f;
        }

        // --- Variable height shaping
        var vel = _rb.linearVelocity;

        // If the player is rising and the jump is NO longer held - cut it
        if (!_jumpHeld && vel.y > 0f)
        {
            vel.y *= jumpCutMultiplier; // instant trim feels great
            _rb.linearVelocity = vel;
        }

        // If falling, apply extra fall gravity
        if (vel.y < 0f)
        {
            _rb.AddForce(Vector3.down * extraFallGravity, ForceMode.Acceleration);
        }
    }

    public Vector2 GetMoveInput() => _moveInput;
}