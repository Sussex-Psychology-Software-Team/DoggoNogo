public interface IGameState {
    int CurrentTrialNumber { get; }
    int TotalTrials { get; }
    int CurrentScore { get; }
    GamePhase CurrentPhase { get; }
    void StartNewTrial();
    void EndTrial(TrialResult result);
}