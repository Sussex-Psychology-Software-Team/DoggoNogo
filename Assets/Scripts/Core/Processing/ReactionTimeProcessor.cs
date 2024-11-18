using System;
using System.Collections.Generic;

// Processes reaction times: Calculates median RT, Determines response types, Updates thresholds
public class ReactionTimeProcessor
{
    private readonly GameConfig _config;
    private readonly List<double> _sortedRTs = new();
    private double _currentMedianRT;

    public ReactionTimeProcessor(GameConfig config)
    {
        _config = config;
        _currentMedianRT = config.InitialMedianRT;
    }

    public string DetermineResponseType(double rt)
    {
        if (rt < 0) return "early";
        if (rt > _config.MaxReactionTime) return "missed";
        return rt > _currentMedianRT ? "slow" : "fast";
    }

    public void UpdateMedianRT(double rt)
    {
        double clampedRT = Math.Clamp(rt, _config.MinReactionTime, _config.MaxReactionTime);
        _sortedRTs.Add(clampedRT);
        _sortedRTs.Sort();
        
        int size = _sortedRTs.Count;
        int mid = size / 2;
        _currentMedianRT = size % 2 != 0 
            ? _sortedRTs[mid] 
            : (_sortedRTs[mid] + _sortedRTs[mid - 1]) / 2;
    }

    public double GetCurrentThreshold() => _currentMedianRT;
}