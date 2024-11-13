public interface IUIState
{
    // UI State properties
    bool IsGameScreenVisible { get; }
    bool IsGameOverScreenVisible { get; }
    bool IsPaused { get; }
    
    // UI State methods
    void ShowGameScreen();
    void ShowGameOverScreen();
    void ShowPauseScreen();
    void HideAllScreens();
    
    // UI Updates
    void UpdateScore(int score, int totalScore);
    void UpdateTrialProgress(int currentTrial, int totalTrials);
    void UpdateTimeRemaining(float timeRemaining);
    
    // UI Animations
    Task FadeIn(UIScreen screen);
    Task FadeOut(UIScreen screen);
    
    // UI Events
    event System.Action<bool> OnPauseStateChanged;
    event System.Action<UIScreen> OnScreenChanged;
}

// Helper enum for UI screens
public enum UIScreen
{
    None,
    Game,
    GameOver,
    Pause
}