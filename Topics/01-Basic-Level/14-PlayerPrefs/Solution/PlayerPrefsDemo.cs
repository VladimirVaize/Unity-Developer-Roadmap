using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsDemo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private Button _clickButton;
    [SerializeField] private Button _resetScoreButton;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TMP_InputField _playerNameInput;
    [SerializeField] private Button _clearAllButton;

    private int _currentScore = 0;
    private int _highScore;
    private string _playerName;

    private const string KEY_CURRENT_SCORE = "CurrentScore";
    private const string KEY_HIGH_SCORE = "HighScore";
    private const string KEY_VOLUME = "Volume";
    private const string KEY_PLAYER_NAME = "PlayerName";

    private const string DEFAULT_NAME = "Player";

    void Start()
    {
        LoadAllData();

        _clickButton.onClick.AddListener(OnClick);
        _resetScoreButton.onClick.AddListener(ResetScore);
        _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        _playerNameInput.onEndEdit.AddListener(OnNameChanged);
        _clearAllButton.onClick.AddListener(ClearAllData);
    }

    private void LoadAllData()
    {
        _currentScore = PlayerPrefs.GetInt(KEY_CURRENT_SCORE, 0);
        _highScore = PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);
        _volumeSlider.value = PlayerPrefs.GetFloat(KEY_VOLUME, 0.5f);
        _playerName = PlayerPrefs.GetString(KEY_PLAYER_NAME, DEFAULT_NAME);

        UpdateScoreUI();
        UpdateHighScoreUI();

        AudioListener.volume = _volumeSlider.value;
        _playerNameInput.text = _playerName;
    }

    private void SaveAllData()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_SCORE, _currentScore);
        PlayerPrefs.SetInt(KEY_HIGH_SCORE, _highScore);
        PlayerPrefs.SetFloat(KEY_VOLUME, _volumeSlider.value);
        PlayerPrefs.SetString(KEY_PLAYER_NAME, _playerName);

        PlayerPrefs.Save();
    }

    private void OnClick()
    {
        _currentScore += 10;
        UpdateScoreUI();

        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            UpdateHighScoreUI();
        }

        SaveAllData();
    }

    private void UpdateScoreUI()
    {
        _scoreText.text = "Score: " + _currentScore;
    }

    private void UpdateHighScoreUI()
    {
        _highScoreText.text = "High Score: " + _highScore;
    }

    private void ResetScore()
    {
        _currentScore = 0;
        UpdateScoreUI();
        SaveAllData();
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        SaveAllData();
    }

    private void OnNameChanged(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = DEFAULT_NAME;
            _playerNameInput.text = name;
        }

        _playerName = name;
        SaveAllData();
    }

    private void ClearAllData()
    {
        PlayerPrefs.DeleteAll();

        LoadAllData();
    }
}
