using System;

// Central event system for communication
// This is added to in other scripts, so they register to the event system and things get updated automatically.
public static class GameEvents {
    // Events: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
        // Object triggers event that triggers handlers which are attached in other scripts
    // Actions: https://learn.microsoft.com/en-us/dotnet/api/system.action-1?view=net-8.0
        // single param function with no return
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