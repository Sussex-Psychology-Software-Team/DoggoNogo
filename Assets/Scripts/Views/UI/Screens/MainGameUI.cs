using UnityEngine;

// Main game screen - I think this is Level1 UI and needs to be updated with dog and bone views, and other methods
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