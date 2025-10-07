using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // This is the default input system asset.
    private PlayerInputActions _controls;
    
    // The rigid body of the character
    private Rigidbody _rb;
    
    // The move input used to determine how the player moves.
    private Vector2 _moveInput;
    
    // Whether the player is jumping or not.
    private bool _isJumping;
    
    // Whether the player is throwing or not.
    private bool _isThrowing;
    
    // Movement tuning variables
    [Header("Movement Tuning")]

    // The speed at which the player moves on the ground
    public float moveSpeed = 8f;
    // The acceleration of the player when moving
    public float accel = 60f;
    // The acceleration of the player when jumping
    public float airAccel = 10f;

    void Awake()
    {
        
        // Create new player input actions object
        _controls = new PlayerInputActions();
        
        // Get the rigid body of the character
        _rb = GetComponent<Rigidbody>();

        // Handlers for when move is performed
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        // Jump and throw handlers
        _controls.Player.Jump.performed += ctx => _isJumping = true;
        _controls.Player.PickUpThrow.performed += ctx => _isThrowing = true;
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
