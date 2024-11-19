using System;

// Core game flow events: Central event system for communication
// This is added to in other scripts, so they register to the event system and things get updated automatically.
public static class GameEvents {
    // Events: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
        // Object triggers event that triggers handlers which are attached in other scripts
    // Actions: https://learn.microsoft.com/en-us/dotnet/api/system.action-1?view=net-8.0
        // single param function with no return
        
        // Core game flow events only
        public static event Action<GamePhase> OnGamePhaseChanged;
        public static event Action<ExperimentData> OnExperimentComplete;
    
        // ReSharper disable Unity.PerformanceAnalysis
        public static void GamePhaseChanged(GamePhase phase) 
        {
            OnGamePhaseChanged?.Invoke(phase);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ExperimentComplete(ExperimentData data)
        {
            OnExperimentComplete?.Invoke(data);
        }
}