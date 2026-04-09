using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _camera;
    private PlayerControls _controls;

    private Vector2 _moveInput;
    private Vector3 _cameraOffset;
    private bool _isJump = false;
    private bool _isGrounded = true;

    [SerializeField] private float _movementSpeed = 15f;
    [SerializeField] private float _jumpForce = 5f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = FindAnyObjectByType<Camera>();
        _controls = new PlayerControls();

        Debug.Log("[Awake()]");
    }

    private void OnEnable()
    {
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Move.canceled += OnMove;
        _controls.Player.Jump.performed += OnJump;
        _controls.Player.Jump.canceled += OnJump;

        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Move.canceled -= OnMove;
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Jump.canceled -= OnJump;

        _controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();

        Debug.Log($"[OnMove()] {_moveInput}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            _isJump = true;
        else if (context.canceled)
            _isJump = false;

        Debug.Log($"[OnJump()] {_isJump}");
    }

    void Start()
    {
        transform.position = new Vector3(0, 1, 0);
        _cameraOffset = new Vector3(0, 2, -5);

        Debug.Log("[Start()]");
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y);
        _rb.AddForce(movement * _movementSpeed, ForceMode.Acceleration);

        if (_isJump && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isJump = false;
            _isGrounded = false;
        }

        Debug.Log("[FixedUpdate()]");
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

    private void LateUpdate()
    {
        _camera.transform.position = _rb.transform.position + _cameraOffset;
        _camera.transform.LookAt(_rb.transform.position);

        Debug.Log("[LateUpdate()]");
    }
}
