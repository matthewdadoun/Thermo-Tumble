using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // This is the default input system asset.
    private PlayerInputActions _controls;

    // The rigid body of the character
    private Rigidbody _rb;

    // The move input used to determine how the player moves.
    private Vector2 _moveInput;

    // Whether the player is throwing or not.
    private bool _isThrowing;

    // The collider of this object
    private Collider _col;
    
    
    [Header("Ground checking")]
    
    // The extra height to check for when checking if the player is grounded
    [SerializeField]
    private float extraHeight = 0.05f;
    [SerializeField] private LayerMask groundMask;

    // Movement tuning variables
    [Header("Movement Tuning")]

    // The speed at which the player moves on the ground
    public float moveSpeed = 8f;
    // The acceleration of the player when moving
    public float accel = 60f;
    
    // The amount of force to apply when jumping
    public float jumpForce = 12f;
    // The acceleration of the player when jumping
    public float airAccel = 10f;


    // call this from Awake once

    void Awake()
    {
        // Create new player input actions object
        _controls = new PlayerInputActions();

        // Get the rigid body of the character
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        Cache();

        // Handlers for when move is performed
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        // Jump and throw handlers
        _controls.Player.Jump.performed += ctx => PerformJump();
        _controls.Player.PickUpThrow.performed += ctx => _isThrowing = true;
    }

    bool IsGrounded()
    {
        var b = _col.bounds;
        
        // Cast from the center straight down to just past the bottom of the bounds
        float rayLen = b.extents.y + extraHeight;
        Vector3 origin = b.center;
        
        if (Physics.Raycast(origin, Vector3.down, out var hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
        {
            // optional: require an "up-ish" surface to avoid walls
            return Vector3.Dot(hit.normal, Vector3.up) > 0.5f;
        }

        return false;
    }

    private void PerformJump()
    {
        if (!IsGrounded())
        {
            return;
        }
        
        // Add jumping force
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

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

    private void FixedUpdate()
    {
        // Retrieve velocity
        var currentVel = _rb.linearVelocity;

        // Store velocity X
        var targetVelX = _moveInput.x * moveSpeed;

        // Accelerate velocity using acceleration
        currentVel.x = Mathf.MoveTowards(currentVel.x, targetVelX, accel * Time.fixedDeltaTime);

        // Store current velocity
        _rb.linearVelocity = currentVel;
    }

    public Vector2 GetMoveInput() => _moveInput;
}