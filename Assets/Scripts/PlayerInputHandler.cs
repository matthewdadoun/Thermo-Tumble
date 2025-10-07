using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // This is the default input system asset.
    private PlayerInputActions _controls;
    
    
    private Vector2 _moveInput;
    private bool _isJumping;
    private bool _isThrowing;

    void Awake()
    {
        _controls = new PlayerInputActions();

        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

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

    void Update()
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

    public Vector2 GetMoveInput() => _moveInput;
}
