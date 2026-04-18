using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 10f;
    [SerializeField] private float _maxLookAngle = 80f;

    private GameObject _player;
    private InputSystem_Actions _inputSystem;
    private Vector2 _lookRotation;
    private float _xRotation = 0f;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogWarning("Player is not find!");
            enabled = false;
            return;
        }

        _inputSystem = new InputSystem_Actions();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        _inputSystem.Player.Enable();
        _inputSystem.Player.Look.performed += LookRotation;
        _inputSystem.Player.Look.canceled += LookRotation;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
        _inputSystem.Player.Look.performed -= LookRotation;
        _inputSystem.Player.Look.canceled -= LookRotation;
    }

    public void LookRotation(InputAction.CallbackContext context)
    {
        _lookRotation = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        transform.position = _player.transform.position + Vector3.up;

        float mouseX = _lookRotation.x * _sensitivity * Time.deltaTime;
        _player.transform.Rotate(Vector3.up, mouseX);

        float mouseY = _lookRotation.y * _sensitivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}
