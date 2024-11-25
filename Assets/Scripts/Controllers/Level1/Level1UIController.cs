using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// So this contains references to the views and UI, whilst level 1 controller doesn't.
// Really some of the handlers should feature in level1 controller
public class Level1UIController : MonoBehaviour
{
    
    [SerializeField] private Level1ViewManager levelViewManager;
    private int _currentHealthBarIndex = 0;
    private Level1IntroductionView _introView;

    private void Awake()
    {
        _introView = levelViewManager.GetIntroView();
    }

    private void OnEnable()
    {
        // Setup events
        Level1Events.OnLevel1Start += HandleLevel1Start;
        Level1Events.OnIntroAnimationComplete += HandleIntroAnimationComplete;
        
        Level1Events.OnIntroComplete += HandleIntroComplete;
        Level1Events.OnLevelStarted += HandleLevelStarted;
        
        // Level change events
        Level1Events.OnStageChanged += HandleStageChange;
        Level1Events.OnTrialCompleted += HandleTrialCompleted;
        Level1Events.OnScoreUpdated += HandleScoreUpdated;
    }
    
    private void OnDisable()
    {
        // Setup events
        Level1Events.OnLevel1Start -= HandleLevel1Start;
        Level1Events.OnIntroAnimationComplete -= HandleIntroAnimationComplete;
        
        Level1Events.OnIntroComplete -= HandleIntroComplete;
        Level1Events.OnLevelStarted -= HandleLevelStarted;

        // Level change events
        Level1Events.OnStageChanged -= HandleStageChange;
        Level1Events.OnTrialCompleted -= HandleTrialCompleted;
        Level1Events.OnScoreUpdated -= HandleScoreUpdated;
    }
    
    private void HandleLevel1Start()
    {
        _introView.PlayInstructionsSignAnimation();
    }

    private void HandleIntroAnimationComplete()
    {
        Level1Controller.Instance.AllowIntroContinue();
    }
    
    private void HandleIntroComplete()
    {
        levelViewManager.SwitchToGameplayUI();
        Level1Controller.Instance.AllowReadyContinue();
    }
    
    private void HandleLevelStarted()
    {
        levelViewManager.ClearInstructions();
        _currentHealthBarIndex = 0;
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

    private void HandleTrialCompleted(TrialResult result)
    {
        levelViewManager.DisplayTrialResult(result);
    }

    private void HandleScoreUpdated(int newScore)
    {
        levelViewManager.UpdateHealthBar(_currentHealthBarIndex, newScore);
    }
}