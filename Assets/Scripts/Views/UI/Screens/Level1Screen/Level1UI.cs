using UnityEngine;

// Main game screen - I think this is Level1 UI and needs to be updated with dog and bone views, and other methods
public class Level1UI : MonoBehaviour
{
    [SerializeField] private FeedbackView feedbackView;

    private void OnEnable()
    {
        Level1Events.OnStageChanged += HandleStageChange;
    }
    
    private void HandleStageChange(int newStage, int targetScore)
    {
        // Play evolution animation
        StartCoroutine(feedbackView.ChangeLevel(newStage, targetScore));
    }
    
    public void Feedback(TrialResult result)
    {
        feedbackView.GiveFeedback(result);
    }
    
    private void OnDisable()
    {
        Level1Events.OnStageChanged -= HandleStageChange;
    }
}