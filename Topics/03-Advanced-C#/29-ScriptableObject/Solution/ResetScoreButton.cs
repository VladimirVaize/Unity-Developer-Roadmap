using UnityEngine;
using UnityEngine.UI;

public class ResetScoreButton : MonoBehaviour
{
    public GlobalScoreSO globalScore;
    
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null && globalScore != null)
        {
            button.onClick.AddListener(() => globalScore.ResetScore());
        }
    }
}
