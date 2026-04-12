using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _activationDistance = 10f;
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _dotThreshold = 0.7f;
    [SerializeField] private float _sideThreshold = 0.1f;
    [SerializeField] private float _debugTimerStep = 0.5f;

    [Header("Visual")]
    [SerializeField] private Color _activeColor = Color.red;
    [SerializeField] private Color _inactiveColor = Color.gray;

    private Transform _player;
    private Renderer _renderer;
    private bool _isActive = false;
    private float _shootTimer = 0f;
    private float _debugTimer = 0f;
    private float _deactivationDistance;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Player not found! Add tag 'Player' to the player GameObject.");
            enabled = false;
            return;
        }
        _player = playerObj.transform;
        _renderer = GetComponent<Renderer>();

        _deactivationDistance = _activationDistance + 0.5f;
    }
    
    void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (!_isActive && distance < _activationDistance)
        {
            ActivateTurret(true);
        }
        else if (_isActive && distance > _deactivationDistance)
        {
            ActivateTurret(false);
        }

        if (!_isActive) return;

        Vector3 toPlayer = (_player.position - transform.position).normalized;

        _debugTimer += Time.deltaTime;
        if (_debugTimer >= _debugTimerStep)
        {
            Vector3 cross = Vector3.Cross(transform.forward, toPlayer);
            if (cross.y > _sideThreshold) 
                Debug.Log("Player is to the right");
            else if (cross.y < -_sideThreshold) 
                Debug.Log("Player is to the left");
            _debugTimer = 0f;
        }

        float step = _rotationSpeed * Time.deltaTime;
        Quaternion target = Quaternion.LookRotation(toPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, step);

        float dot = Vector3.Dot(transform.forward, toPlayer);
        if (dot > _dotThreshold)
        {
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= _fireRate)
            {
                Shoot();
                _shootTimer = 0f;
            }
        }
        else
        {
            _shootTimer = 0f;
        }
    }

    private void ActivateTurret(bool active)
    {
        _isActive = active;
        Debug.Log(active ? "Turret activated!" : "Turret deactivated!");

        if (_renderer != null)
        {
            _renderer.material.color = active ? _activeColor : _inactiveColor;
        }
    }

    private void Shoot()
    {
        Debug.Log("Pew! Pew!");
    }
}
