using UnityEngine;

public class Level1UIController : MonoBehaviour
{
    [SerializeField] private FeedbackView feedbackView;

    private void OnEnable()
    {
        Level1Events.OnTrialCompleted += HandleTrialCompleted;
        Level1Events.OnLevelStarted += HandleLevelStarted;
        Level1Events.OnTrialStateChanged += HandleTrialStateChanged;
        Level1Events.OnInvalidResponse += HandleInvalidResponse;
    }

    private void OnDisable()
    {
        Level1Events.OnTrialCompleted -= HandleTrialCompleted;
        Level1Events.OnLevelStarted -= HandleLevelStarted;
        Level1Events.OnTrialStateChanged -= HandleTrialStateChanged;
        Level1Events.OnInvalidResponse -= HandleInvalidResponse;
    }
    
    private void HandleLevelStarted()
    {
        // Maybe show elements here? like feedback view
    }
    private void HandleTrialStateChanged(TrialState state)
    {
        // Maybe handle pausing and stuff here?
    }
    private void HandleTrialCompleted(TrialResult result)
    {
        // Not sure this feels right - handle response recorded here maybe.
        feedbackView.GiveFeedback(result);
    }
    
    private void HandleInvalidResponse(string reason)
    {
        // Probably no need for this event - that's extracted elsewhere.
        // feedbackView.ShowInvalidResponse(reason);
    }
    
}