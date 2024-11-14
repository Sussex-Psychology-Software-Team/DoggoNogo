using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private FeedbackView feedbackView;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private HealthBarView healthBar;

    public void UpdateScore(int score)
    {
        scoreDisplay.UpdateScore(score);
    }

    public void UpdateTrial(int currentTrial, int totalTrials)
    {
        // Update trial progress if needed
    }
}