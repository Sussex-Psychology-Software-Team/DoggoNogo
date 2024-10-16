using System;
using System.Collections.Generic; // Dictionaries

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
    // Stim spec
    public float[] screenSize;
    public float[] canvasSize;
    public float canvasScaleFactor;
    public float[] bonePosition;
    public float boneOrientation;
    public float boneScale;

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

    public void SaveStimulus(Dictionary<string, float> stimSpec){
        this.screenSize = new float[] {stimSpec["screenWidth"], stimSpec["screenHeight"]};
        this.canvasSize = new float[] {stimSpec["canvasWidth"], stimSpec["canvasHeight"]};
        this.canvasScaleFactor = stimSpec["canvasScale"];
        this.bonePosition = new float[] {stimSpec["x"], stimSpec["y"]};
        this.boneOrientation = stimSpec["rotation"];
        this.boneScale = stimSpec["scale"];
    }

    double RoundTime(double time, int dp) {
        return Math.Round(time * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }
}
