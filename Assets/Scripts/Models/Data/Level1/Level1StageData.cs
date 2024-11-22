using UnityEngine;

[System.Serializable]
public class Level1StageData
{
    // Public
    public int CurrentStage { get; private set; } = 1;
    public int TotalStages { get; } = 3;
    public int CurrentTargetScore { get; set; }
    // Private
    private int MinTrialsPerStage { get; } = 10;
    private int NTrials { get; }
    
    public Level1StageData(Metadata metadata, GameConfig gameConfig)
    {
        NTrials = int.TryParse(metadata.l1n, out int trials) ? trials : 60;
        CurrentTargetScore = CalculateTargetScore(0, gameConfig.MinScore);
    }
    
    public int CalculateTargetScore(int validTrialsCompleted, int minScore)
    {
        int trialsRemaining = NTrials - validTrialsCompleted;
        int stagesRemaining = TotalStages + 1 - CurrentStage;
        int trialsPerStage = trialsRemaining / stagesRemaining;
        
        // Calculate target
        int expectedFastTrials = trialsPerStage / 2;
        int targetScore = minScore * expectedFastTrials;
        
        // Apply minimum
        int minimumTargetScore = (MinTrialsPerStage / 2) * minScore;
        targetScore = Mathf.Max(targetScore, minimumTargetScore);
        
        Debug.Log($"New Target Score for Stage {CurrentStage}: {targetScore}");
        CurrentTargetScore = targetScore;
        return targetScore;
    }

    public bool AdvanceStage()
    {
        if (CurrentStage >= TotalStages) return false;
        CurrentStage++;
        return true;
    }
}