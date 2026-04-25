using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    private TimeManager _timeManager;

    void Start()
    {
        _timeManager = FindAnyObjectByType<TimeManager>();
        if(_timeManager == null)
        {
            Debug.LogWarning("TimeManager not find");
            enabled = false;
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _timeManager.AddScore();
            Destroy(gameObject);
        }
    }
}
