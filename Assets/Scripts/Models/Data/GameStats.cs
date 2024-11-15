using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// List of response stats over trials - Part of GameData structure.
    // I wonder if this is necessary - could grab these from the current trial.
    // Missing the initial few trials buffer on threshold, and adding maxRT on missing trial
[System.Serializable]
public class GameStats
{
    public int CurrentScore { get; private set; }
    public int TotalValidTrials { get; private set; }
    public float AverageReactionTime { get; private set; }
    
    private List<float> _reactionTimes = new();

    public void UpdateScore(int scoreChange)
    {
        CurrentScore = Mathf.Max(0, CurrentScore + scoreChange);
    }

    public void RecordTrial(float reactionTime, bool isValid)
    {
        if (!isValid) return;
        TotalValidTrials++;
        _reactionTimes.Add(reactionTime);
        AverageReactionTime = _reactionTimes.Average();
    }

    public void Reset()
    {
        CurrentScore = 0;
        TotalValidTrials = 0;
        AverageReactionTime = 0;
        _reactionTimes.Clear();
    }
}