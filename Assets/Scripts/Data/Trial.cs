using System;

[System.Serializable]
public class Trial {
    public int trialNumber;
    public double isi;
    public double? rt;
    public string datetime;
    public int trialScore;
    public int totalScore;
    public string responseType;
    public double threshold;
    public bool validTrial;
    public int validTrialCount;

    public Trial(int trialN, double isi) {
        trialNumber = trialN;
        this.isi = isi;
    }

    public void SaveTrial(double? rt, string type, int score, int total, double threshold, bool validTrial, int validTrialCount) {
        if(rt.HasValue) this.rt = RoundTime(rt.Value, 7);
        this.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.responseType = type;
        this.trialScore = score;
        this.totalScore = total;
        this.threshold = threshold;
        this.validTrial = validTrial;
        this.validTrialCount = validTrialCount;
    }

    double RoundTime(double time, int dp) {
        return Math.Round(time * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }
}
