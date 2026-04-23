using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _shootDistance = 50f;

    private InputSystem_Actions _inputSystem;
    private bool _attack = false;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Attack.performed += Attack;
        _inputSystem.Player.Attack.canceled += Attack;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Disable();
        _inputSystem.Player.Attack.performed -= Attack;
        _inputSystem.Player.Attack.canceled -= Attack;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
            _attack = true;
        else if (context.canceled)
            _attack = false;
    }

    void Update()
    {
        if (_attack)
        {
            int layerMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Projectile"));

            RaycastHit hit;
            if (Physics.Raycast(_firePoint.position, _firePoint.forward, out hit, _shootDistance, layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Instantiate(_laserPrefab, _firePoint.position, _firePoint.rotation);
                    Debug.Log("Shot at enemy!");
                }
                else if (hit.collider.CompareTag("Wall"))
                {
                    Debug.Log("Path blocked by a wall. Cannot shoot.");
                }
                else
                {
                    Debug.Log($"Hit {hit.collider.tag}, cannot shoot");
                }
            }
            else
            {
                Instantiate(_laserPrefab, _firePoint.position, _firePoint.rotation);
                Debug.Log("Free shot");
            }
            _attack = false;
        }
    }
}
