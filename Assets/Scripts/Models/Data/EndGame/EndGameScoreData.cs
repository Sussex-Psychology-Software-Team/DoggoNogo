using System;

[System.Serializable]
public class EndGameScoreData
{
    public float percentileScore;
    public int totalTrials;
    public float finalThreshold;
    public DateTime completionTime;

    public EndGameScoreData()
    {
        completionTime = DateTime.Now;
    }
}