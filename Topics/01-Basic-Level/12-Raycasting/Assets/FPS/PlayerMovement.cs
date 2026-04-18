using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 15f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _deceleration = 25f;

    private Rigidbody _rb;
    private InputSystem_Actions _inputSystem;
    private Vector2 _moveInput;
    private bool _isJump = false;
    private bool _isGrounded = true;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning("Player is not have a Rigidbody component");
            enabled = false;
            return;
        }

        _inputSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Move.performed += OnMove;
        _inputSystem.Player.Move.canceled += OnMove;
        _inputSystem.Player.Jump.performed += OnJump;
        _inputSystem.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
        _inputSystem.Player.Move.performed -= OnMove;
        _inputSystem.Player.Move.canceled -= OnMove;
        _inputSystem.Player.Jump.performed -= OnJump;
        _inputSystem.Player.Jump.canceled -= OnJump;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            _isJump = true;
        else if (context.canceled)
            _isJump = false;
    }

    private void Start()
    {
        transform.position = new Vector3(0, 1, 0);
    }

    private void FixedUpdate()
    {
        Vector3 desiredDirection = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized;
        Vector3 desiredVelocity = desiredDirection * _movementSpeed;

        float currentAcceleration;
        if (_moveInput.magnitude > 0.1f)
            currentAcceleration = _acceleration;
        else
            currentAcceleration = _deceleration;

        _currentVelocity = Vector3.Lerp(_currentVelocity, desiredVelocity, currentAcceleration * Time.fixedDeltaTime);

        _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _movementSpeed);

        Vector3 force = (_currentVelocity - _rb.linearVelocity) * _rb.mass;
        _rb.AddForce(force, ForceMode.Force);

        if (_isJump && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isJump = false;
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    _isGrounded = true;
                    break;
                }
            }
        }
    }
}
