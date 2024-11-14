using System;
using UnityEngine;

public class Level1ScoreController : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;

    public TrialResult ProcessTrial(double reactionTime, Level1Data levelData)
    {
        string responseType = DetermineResponseType(reactionTime, levelData.currentMedianRT);
        bool validTrial = IsValidTrial(responseType);
        int trialScore = CalculateTrialScore(responseType, reactionTime);
        
        if (validTrial) levelData.validTrialCount++;
        levelData.currentScore += trialScore;
        levelData.currentScore = Mathf.Max(levelData.currentScore, 0);

        return new TrialResult
        {
            ReactionTime = reactionTime,
            ResponseType = responseType,
            TrialScore = trialScore,
            TotalScore = levelData.currentScore,
            Threshold = levelData.currentMedianRT,
            ValidTrial = validTrial,
            ValidTrialCount = levelData.validTrialCount
        };
    }

    private string DetermineResponseType(double rt, float medianRT)
    {
        if (rt < 0) return "early";
        if (rt > gameConfig.MaxReactionTime) return "missed";
        return rt > medianRT ? "slow" : "fast";
    }

    private bool IsValidTrial(string responseType)
    {
        return responseType is "slow" or "fast";
    }

    private int CalculateTrialScore(string responseType, double reactionTime)
    {
        return responseType switch
        {
            "early" => gameConfig.PenaltyScore,
            "missed" or "slow" => 0,
            "fast" => CalculateFastScore(reactionTime),
            _ => throw new ArgumentException("Invalid response type")
        };
    }

    private int CalculateFastScore(double rt)
    {
        double normalizedRT = (Math.Clamp(rt, gameConfig.MinReactionTime, gameConfig.MaxReactionTime) - gameConfig.MinReactionTime) 
            / (gameConfig.MaxReactionTime - gameConfig.MinReactionTime);
        double scoreRange = gameConfig.MaxScore - gameConfig.MinScore;
        return (int)(gameConfig.MinScore + ((1 - normalizedRT) * scoreRange));
    }
}