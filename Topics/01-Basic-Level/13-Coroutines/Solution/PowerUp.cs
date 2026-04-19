using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _duration = 5f;
    [SerializeField, Range(1.1f, 2f)] private float _boostPower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            PlayerPowerUp playerPowerUp = other.GetComponent<PlayerPowerUp>();
            if (playerPowerUp != null)
                playerPowerUp.ActivateSpeedBoost(_duration, _boostPower);
            else
                Debug.LogWarning("Player has no PlayerPowerUp component!");

            Destroy(gameObject, 0.5f);
        }
    }
}
