using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private GameObject _bulletPref;
    [SerializeField] private float _bulletSpeed = 50f;

    private Transform _player;
    private Rigidbody _playerRigidbody;
    private bool _isGrounded = true;

    private void Awake()
    {
        if(_bulletPref == null)
        {
            Debug.LogWarning("Bullet prefab not assigned!");
            enabled = false;
            return;
        }

        if (_bulletPref.GetComponent<Rigidbody>() == null)
        {
            Debug.LogWarning("Bullet prefab must have a Rigidbody component!");
            enabled = false;
            return;
        }

        _player = transform;
        _playerRigidbody = GetComponent<Rigidbody>();
        if (_playerRigidbody == null)
        {
            Debug.LogError("Rigidbody required!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(_bulletPref, _player.position + _player.forward, _player.rotation);

            //Old
            //bullet.GetComponent<Rigidbody>().velocity = _player.forward * _bulletSpeed;

            //New (Unity 6+)
            bullet.GetComponent<Rigidbody>().linearVelocity = _player.forward * _bulletSpeed;
        }
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        float speed = _movementSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = _movementSpeed * 1.5f;
        }

        Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.fixedDeltaTime;

        //transform.Translate(movement);
        // Has problems:
        // - Passing through walls at high speed
        // - Incorrect collisions

        _playerRigidbody.MovePosition(_playerRigidbody.position + movement);

        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            _playerRigidbody.MoveRotation(Quaternion.Slerp(_playerRigidbody.rotation, targetRotation, 10f * Time.fixedDeltaTime));
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

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
