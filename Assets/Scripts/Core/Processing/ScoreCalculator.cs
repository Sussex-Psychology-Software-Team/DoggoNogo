using System;
using System.Collections.Generic;

// Calculates scores based on reaction times, Manages scoring rules, Handles score updates
public class ScoreCalculator
{
    private readonly GameConfig _config;

    public ScoreCalculator(GameConfig config)
    {
        _config = config;
    }

    public int CalculateScore(string responseType, double reactionTime)
    {
        return responseType switch
        {
            "early" => _config.PenaltyScore,
            "missed" or "slow" => 0,
            "fast" => CalculateFastScore(reactionTime),
            _ => throw new ArgumentException("Invalid response type")
        };
    }

    private int CalculateFastScore(double rt)
    {
        double normalizedRT = (Math.Clamp(rt, _config.MinReactionTime, _config.MaxReactionTime) 
                               - _config.MinReactionTime) / (_config.MaxReactionTime - _config.MinReactionTime);
        return (int)(_config.MinScore + ((1 - normalizedRT) * (_config.MaxScore - _config.MinScore)));
    }
}