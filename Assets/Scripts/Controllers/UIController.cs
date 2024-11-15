using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private MainGameUI mainGameUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private UIAnimationController animationController;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameObject introScreen;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnGamePhaseChanged += HandleGamePhaseChanged;
        GameEvents.OnScoreUpdated += HandleScoreUpdated;
    }

    private void OnDisable()
    {
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
        GameEvents.OnScoreUpdated -= HandleScoreUpdated;
    }

    public void UpdateScore(int newScore)
    {
        scoreDisplay.UpdateScore(newScore);
        GameEvents.ScoreUpdated(newScore);
    }

    private void HandleGamePhaseChanged(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.GameOver:
                ShowGameOver();
                break;
            case GamePhase.TrialInProgress:
                UpdateTrialUI();
                break;
        }
    }
    
    private void UpdateTrialUI()
    {
        if (mainGameUI != null)
        {
            mainGameUI.UpdateTrial(
                Level1Controller.Instance.CurrentTrialNumber,
                Level1Controller.Instance.TotalTrials
            );
        }
    }
    
    public void ShowGameScreen()
    {
        introScreen?.SetActive(false);
        mainGameUI.gameObject.SetActive(true);
    }
    
    private async void ShowGameOver()
    {
        await animationController.FadeOut(mainGameUI.gameObject);
        gameOverUI.gameObject.SetActive(true);
        await animationController.FadeIn(gameOverUI.gameObject);
    }

    private void HandleScoreUpdated(int score)
    {
        scoreDisplay.UpdateScore(score);
    }
}