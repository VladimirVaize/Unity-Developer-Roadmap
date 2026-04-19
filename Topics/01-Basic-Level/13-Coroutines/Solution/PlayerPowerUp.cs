using System.Collections;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    private float _baseSpeed;
    private float _currentSpeed;
    private PlayerMovement _playerMovement;
    private Coroutine _activePowerUpRoutine;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        _baseSpeed = _playerMovement.MovementSpeed;
    }

    void Start()
    {
        _currentSpeed = _baseSpeed;
    }

    public void ActivateSpeedBoost(float duration, float boostPower)
    {
        _currentSpeed = _baseSpeed * boostPower;

        if (_activePowerUpRoutine != null)
            StopCoroutine(_activePowerUpRoutine);

        _activePowerUpRoutine = StartCoroutine(SpeedBoostCoroutine(duration, boostPower));
    }

    IEnumerator SpeedBoostCoroutine(float duration, float boostPower)
    {
        _playerMovement.MovementSpeed = _currentSpeed;

        transform.localScale = Vector3.one * boostPower;

        yield return new WaitForSeconds(duration);

        _playerMovement.MovementSpeed = _baseSpeed;

        transform.localScale = Vector3.one;
    }
}
