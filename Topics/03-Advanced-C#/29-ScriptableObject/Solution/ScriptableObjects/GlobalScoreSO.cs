using UnityEngine;

[CreateAssetMenu(fileName = "GlobalScore", menuName = "Game/Global Score")]
public class GlobalScoreSO : ScriptableObject
{
    [SerializeField] private int score = 0;
    
    public System.Action<int> OnScoreChanged;
    
    public int Score => score;
    
    public void AddScore(int value)
    {
        if (value < 0)
        {
            Debug.LogWarning("Attempted to add negative value via AddScore. Use SubtractScore.");
            return;
        }
        
        score += value;
        Debug.Log($"[GlobalScore] Added {value} points. Total: {score}");
        OnScoreChanged?.Invoke(score);
    }
    
    public void SubtractScore(int value)
    {
        score -= value;
        Debug.Log($"[GlobalScore] Subtracted {value} points. Total: {score}");
        OnScoreChanged?.Invoke(score);
    }
    
    public void ResetScore()
    {
        score = 0;
        Debug.Log("[GlobalScore] Score reset to 0");
        OnScoreChanged?.Invoke(score);
    }
    
    public void OnEnable()
    {
        // Uncomment for auto-reset at game start:
        // score = 0;
    }
}
