using TMPro;
using UnityEngine;

public class EndGameView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI percentileText;
    [SerializeField] private EndGameDogView dogView;
    [SerializeField] private EndGameScoreView scoreView;
    
    [Header("Services")]
    [SerializeField] private FullscreenManager fullscreenManager;

    private EndGameController _controller;
    
    private void Start()
    {
        _controller = EndGameController.Instance;
        InitializeEndScreen();
    }

    private void InitializeEndScreen()
    {
        fullscreenManager.ToggleFullscreen();
        DisplayScore(_controller.CalculatePercentileScore());
    }

    private void DisplayScore(float percentileScore)
    {
        percentileText.text = $"You completed the game, congratulations!\n\n" +
                              $"Your reflexes were faster and more accurate than {percentileScore:F0}% of people. Well done!\n\n" +
                              $"<size=70%>You can now close the tab.";
        
        scoreView.AnimateScore((int)percentileScore);
    }
}