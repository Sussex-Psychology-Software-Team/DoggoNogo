public static class GameEvents {
    public static event Action<TrialResult> OnTrialCompleted;
    public static event Action<GamePhase> OnGamePhaseChanged;
    public static event Action<int> OnScoreUpdated;
    
    public static void TrialCompleted(TrialResult result) {
        OnTrialCompleted?.Invoke(result);
    }
    
    public static void GamePhaseChanged(GamePhase phase) {
        OnGamePhaseChanged?.Invoke(phase);
    }
    
    public static void ScoreUpdated(int newScore) {
        OnScoreUpdated?.Invoke(newScore);
    }
}