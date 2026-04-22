using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private int _coins;
    [SerializeField] private int _health = 20;
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private float _pushForce = 10f;
    [SerializeField] private int _coinReward = 10;

    private Rigidbody _rb;
    private bool _isAlive = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isAlive) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            _health = Mathf.Max(0, _health - _damageAmount);

            if (_rb != null)
            {
                Vector3 force = (transform.position - collision.transform.position) * _pushForce;
                _rb.AddForce(force, ForceMode.Impulse);
            }

            Debug.Log($"Damage! Health: {_health}");

            if (_health <= 0)
            {
                _isAlive = false;
                Debug.Log("Game Over!");
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAlive) return;

        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            _coins += _coinReward;
            Debug.Log($"Coin collected! Score: {_coins}");
        }
    }
}
