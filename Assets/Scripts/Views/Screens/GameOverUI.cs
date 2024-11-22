using TMPro;
using UnityEngine;

// End screen (I think) it's missing most stuff, although might be trying to be end of Level 1?
// Don't think initialize is ever called either.

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI trialsCompletedText;

    public void Initialize(int finalScore, int trialsCompleted)
    {
        finalScoreText.text = $"Final Score: {finalScore}";
        trialsCompletedText.text = $"Trials Completed: {trialsCompleted}";
    }
}