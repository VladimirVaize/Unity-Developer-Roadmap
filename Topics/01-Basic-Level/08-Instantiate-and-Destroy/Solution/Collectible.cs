using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private bool _isNegative;

    private int _damage = 10;

    private GameObject _player;
    private Player _playerData;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogWarning("Player is not found!");
            enabled = false;
            return;
        }

        _playerData = _player.GetComponent<Player>();
        if (_playerData == null)
        {
            Debug.LogWarning("Player don't have a component 'Player'");
            enabled = false;
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (_isNegative)
            {
                _playerData.GetDamage(_damage);
            }
            else
            {
                _playerData.GetCoin();
            }
        }
        Destroy(gameObject);
    }
}
