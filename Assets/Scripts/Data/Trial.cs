using System;

[System.Serializable]
public class Trial {
    public int trial_n;
    public double isi;
    public double rt;
    public string datetime;
    public int trialScore;
    public int totalScore;
    public string trialType;

    public Trial(int trialNumber, double isiVar) {
        trial_n = trialNumber;
        isi = isiVar;
        rt = -1.0;
        datetime = "";
        trialScore = 0;
        totalScore = -1;
        trialType = "missed";
    }

    public void SaveTrial(double rt, string type, int score, int total, double threshold) {
        this.rt = RoundTime(rt, 7);
        this.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.trialType = type;
        this.trialScore = score;
        this.totalScore = total;
        this.threshold = threshold;
    }

    double RoundTime(double time, int dp) {
        return Math.Round(time * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }
}
