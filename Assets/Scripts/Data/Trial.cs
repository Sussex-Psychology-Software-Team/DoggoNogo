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
        trialScore = -1;
        totalScore = -1;
        trialType = "";
    }

    public void saveTrial(double rt, string type, int score, int total) {
        this.rt = roundTime(rt, 7);
        this.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.trialType = type;
        this.trialScore = score;
        this.totalScore = total;
    }

    double roundTime(double time, int dp) {
        return Math.Round(time * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }
}
