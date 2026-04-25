using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TimeManager : MonoBehaviour
{
    // UI
    //[SerializeField] private TextMeshProUGUI _timeScaleText;
    //[SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private int _score;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Time.timeScale = 0.2f;
        if (Input.GetKeyDown(KeyCode.E))
            Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0f)
                Time.timeScale = 1f;
            else
                Time.timeScale = 0f;
        }

        // Update UI
        //_timeScaleText.text = $"Time Scale: {Time.timeScale:F1}";
        //_scoreText.text = $"Score: {_score}";
    }

    public void AddScore()
    {
        _score++;
    }
}
