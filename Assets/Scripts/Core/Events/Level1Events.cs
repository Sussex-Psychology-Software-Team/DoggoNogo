using System;
using System.Collections.Generic;

public static class Level1Events 
{
    // Trial events
    public static event Action<double> OnNewTrialStarted;
    public static event Action<TrialResult> OnTrialCompleted;
    public static event Action<Dictionary<string, float>> OnStimulusShown;
    
    // Level flow events
    public static event Action OnLevelStarted;
    public static event Action<int, int> OnStageChanged; // (newStage, targetScore)
    public static event Action OnLevelComplete;
    
    // UI events
    public static event Action<int> OnScoreUpdated;
    
    public static event Action<TrialState> OnTrialStateChanged;
    public static event Action<double> OnReactionTimeRecorded;
    public static event Action<float> OnMedianRTUpdated;
    public static event Action<string> OnInvalidResponse;


    public static void LevelStarted() => OnLevelStarted?.Invoke();
    public static void NewTrialStarted(double isi) => OnNewTrialStarted?.Invoke(isi);
    public static void StimulusShown(Dictionary<string, float> stimSpec) => OnStimulusShown?.Invoke(stimSpec);
    public static void TrialCompleted(TrialResult result) => OnTrialCompleted?.Invoke(result);
    public static void LevelComplete() => OnLevelComplete?.Invoke();
    public static void ScoreUpdated(int score) => OnScoreUpdated?.Invoke(score);
    
    public static void StageChanged(int stage, int targetScore) => 
        OnStageChanged?.Invoke(stage, targetScore);
    
    public static void TrialStateChanged(TrialState state) => OnTrialStateChanged?.Invoke(state);
    public static void ReactionTimeRecorded(double rt) => OnReactionTimeRecorded?.Invoke(rt);
    public static void MedianRTUpdated(float medianRT) => OnMedianRTUpdated?.Invoke(medianRT);
    public static void InvalidResponse(string reason) => OnInvalidResponse?.Invoke(reason);
}

public enum TrialState
{
    Ready,
    WaitingForStimulus,
    StimulusShown,
    WaitingForResponse,
    Complete
}