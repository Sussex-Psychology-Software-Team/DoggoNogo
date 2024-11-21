using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Level1UIController : MonoBehaviour
{
    [FormerlySerializedAs("levelUI")] [SerializeField] private Level1ViewManager levelViewManager;
    private int _currentHealthBarIndex = 0;
    private Level1IntroductionView _introView;

    private void Awake()
    {
        _introView = levelViewManager.GetIntroView();
    }

    private void OnEnable()
    {
        // Intro events
        Level1Events.OnIntroStarted += HandleIntroStarted;
        Level1Events.OnIntroComplete += HandleIntroComplete;
        
        // Existing gameplay events
        Level1Events.OnStageChanged += HandleStageChange;
        Level1Events.OnTrialCompleted += HandleTrialCompleted;
        Level1Events.OnLevelStarted += HandleLevelStarted;
        Level1Events.OnScoreUpdated += HandleScoreUpdated;
    }
    
    private void OnDisable()
    {
        // Intro events
        Level1Events.OnIntroStarted -= HandleIntroStarted;
        Level1Events.OnIntroComplete -= HandleIntroComplete;
        
        // Existing gameplay events
        Level1Events.OnStageChanged -= HandleStageChange;
        Level1Events.OnTrialCompleted -= HandleTrialCompleted;
        Level1Events.OnLevelStarted -= HandleLevelStarted;
        Level1Events.OnScoreUpdated -= HandleScoreUpdated;
    }

    private void HandleIntroStarted()
    {
        _introView.Initialize();
    }

    private void HandleIntroComplete()
    {
        levelViewManager.SwitchToGameplayUI();
    }

    // Existing handlers...
    private void HandleStageChange(int newStage, int targetScore)
    {
        StartCoroutine(HandleStageTransition(newStage, targetScore));
    }

    private IEnumerator HandleStageTransition(int newStage, int targetScore)
    {
        levelViewManager.UpdateHealthBar(_currentHealthBarIndex, targetScore);
        _currentHealthBarIndex = newStage - 1;
        levelViewManager.ConfigureHealthBar(
            _currentHealthBarIndex, 
            targetScore, 
            new Color(0.06770712f, 0.5817609f, 0f, 1f)
        );
        yield return StartCoroutine(levelViewManager.HandleLevelTransition(newStage));
    }

    private void HandleLevelStarted()
    {
        _currentHealthBarIndex = 0;
    }

    private void HandleTrialCompleted(TrialResult result)
    {
        levelViewManager.DisplayTrialResult(result);
    }

    private void HandleScoreUpdated(int newScore)
    {
        levelViewManager.UpdateHealthBar(_currentHealthBarIndex, newScore);
    }
}