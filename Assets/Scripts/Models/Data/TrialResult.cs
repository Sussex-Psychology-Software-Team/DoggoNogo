
// Trial result data structure for saving in Trial
[System.Serializable]
public class TrialResult
{
    public double ReactionTime { get; set; }
    public string ResponseType { get; set; }
    public int TrialScore { get; set; }
    public int TotalScore { get; set; }
    public double Threshold { get; set; }
    public bool ValidTrial { get; set; }
    public int ValidTrialCount { get; set; }
}