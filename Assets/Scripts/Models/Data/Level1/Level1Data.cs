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
        sortedRTs.Add(newRT);
        sortedRTs.Sort();
        int mid = sortedRTs.Count / 2;
        currentMedianRT = sortedRTs.Count % 2 == 0 
            ? (float)((sortedRTs[mid] + sortedRTs[mid - 1]) / 2)
            : (float)sortedRTs[mid];
    }
}