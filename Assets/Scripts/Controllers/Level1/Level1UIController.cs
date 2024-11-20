using System.Collections;
using UnityEngine;

public class Level1UIController : MonoBehaviour
{
    [SerializeField] private Level1UI levelUI;
    private int _currentHealthBarIndex = 0;

    private void OnEnable()
    {
        Level1Events.OnStageChanged += HandleStageChange;
        Level1Events.OnTrialCompleted += HandleTrialCompleted;
        Level1Events.OnLevelStarted += HandleLevelStarted;
        Level1Events.OnScoreUpdated += HandleScoreUpdated;
    }
    
    private void OnDisable()
    {
        Level1Events.OnStageChanged -= HandleStageChange;
        Level1Events.OnTrialCompleted -= HandleTrialCompleted;
        Level1Events.OnLevelStarted -= HandleLevelStarted;
        Level1Events.OnScoreUpdated -= HandleScoreUpdated;
    }
    
    private void HandleStageChange(int newStage, int targetScore)
    {
        StartCoroutine(HandleStageTransition(newStage, targetScore));
    }

    private IEnumerator HandleStageTransition(int newStage, int targetScore)
    {
        // Fill current health bar
        levelUI.UpdateHealthBar(_currentHealthBarIndex, targetScore);
        
        // Switch to new health bar
        _currentHealthBarIndex = newStage - 1;
        levelUI.ConfigureHealthBar(
            _currentHealthBarIndex, 
            targetScore, 
            new Color(0.06770712f, 0.5817609f, 0f, 1f)
        );

        // Handle level change animations
        yield return StartCoroutine(levelUI.HandleLevelTransition(newStage));
    }

    private void HandleLevelStarted()
    {
        _currentHealthBarIndex = 0;
    }

    private void HandleTrialCompleted(TrialResult result)
    {
        levelUI.DisplayTrialResult(result);
    }

    private void HandleScoreUpdated(int newScore)
    {
        levelUI.UpdateHealthBar(_currentHealthBarIndex, newScore);
    }
}