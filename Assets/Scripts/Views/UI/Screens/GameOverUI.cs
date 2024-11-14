using TMPro;
using UnityEngine;

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