using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 7f;
    [SerializeField, Range(10f, 15f)] private float _cameraDistance;

    private GameObject _camera;
    private int _coinCount = 0;
    private int _health = 100;
    private Rigidbody _rb;

    private void Awake()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (_camera == null)
        {
            Debug.LogWarning("MainCamera is not found!");
            enabled = false;
            return;
        }

        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning("Rigidbody is not found!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        transform.position = Vector3.up * 1.2f;
        _camera.transform.position = transform.position - (Vector3.forward * _cameraDistance);
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(moveX, 0, 0) * _movementSpeed * Time.fixedDeltaTime;

        _rb.MovePosition(_rb.position + movement);
    }

    private void LateUpdate()
    {
        _camera.transform.position = transform.position - (Vector3.forward * _cameraDistance);
    }

    public void GetCoin()
    {
        _coinCount++;
        Debug.Log($"Player get a coin. Coin count: {_coinCount}");
    }

    public void GetDamage(int damage)
    {
        if (damage > 0)
        {
            _health = Mathf.Max(_health - damage, 0);
            Debug.Log($"Player get {damage} damage. Health: {_health}");

            if (_health == 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
    }
}
