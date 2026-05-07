using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Global score (ScriptableObject)")]
    public GlobalScoreSO globalScore;
    
    [Tooltip("Text field for displaying score")]
    public TextMeshProUGUI scoreText;
    
    [Header("Formatting")]
    [SerializeField] private string prefix = "Score: ";
    [SerializeField] private string suffix = "";
    
    private void Start()
    {
        if (globalScore == null)
        {
            Debug.LogError("[ScoreUI] globalScore not assigned!");
            return;
        }
        
        globalScore.OnScoreChanged += UpdateScoreDisplay;
        
        UpdateScoreDisplay(globalScore.Score);
    }
    
    private void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{prefix}{newScore}{suffix}";
        }
    }
    
    private void OnDestroy()
    {
        if (globalScore != null)
        {
            globalScore.OnScoreChanged -= UpdateScoreDisplay;
        }
    }
}
