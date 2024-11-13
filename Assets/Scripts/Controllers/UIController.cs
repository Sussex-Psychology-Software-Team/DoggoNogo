public class UIController : MonoBehaviour {
    [SerializeField] private MainGameUI mainGameUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private UIAnimationController animationController;

    private void OnEnable() {
        GameEvents.OnGamePhaseChanged += HandleGamePhaseChanged;
        GameEvents.OnScoreUpdated += HandleScoreUpdated;
    }

    private void HandleGamePhaseChanged(GamePhase phase) {
        switch (phase) {
            case GamePhase.GameOver:
                ShowGameOver();
                break;
            case GamePhase.TrialInProgress:
                UpdateTrialUI();
                break;
        }
    }

    private async void ShowGameOver() {
        await animationController.FadeOut(mainGameUI.gameObject);
        gameOverUI.gameObject.SetActive(true);
        await animationController.FadeIn(gameOverUI.gameObject);
    }

    private void OnDisable() {
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
        GameEvents.OnScoreUpdated -= HandleScoreUpdated;
    }
}