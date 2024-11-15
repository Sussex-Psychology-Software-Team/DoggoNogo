using System.Collections.Generic;

[System.Serializable]
public class Level1Data
{
    public float currentMedianRT;
    public int currentScore;
    public int validTrialCount;
    public readonly List<double> SortedRTs = new();

    public Level1Data(GameConfig config)
    {
        currentMedianRT = config.InitialMedianRT;
        currentScore = 0;
        validTrialCount = 0;
    }

    public void UpdateMedianRT(double newRT)
    {
        SortedRTs.Add(newRT);
        SortedRTs.Sort();
        int mid = SortedRTs.Count / 2;
        currentMedianRT = SortedRTs.Count % 2 == 0 
            ? (float)((SortedRTs[mid] + SortedRTs[mid - 1]) / 2)
            : (float)SortedRTs[mid];
    }
}