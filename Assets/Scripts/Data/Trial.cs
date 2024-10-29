using System;
using System.Collections.Generic; // Dictionaries
using UnityEngine;

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
    public float screenWidth;
    public float screenHeight;
    public float canvasWidth;
    public float canvasHeight;
    public float canvasScaleFactor;
    public float boneX;
    public float boneY;
    public float boneOrientation;
    public float boneScale;

    public Trial(int trialN, double isi) {
        trialNumber = trialN;
        this.isi = isi;
        this.screenWidth = Screen.width; // Directly accessing the screen width
        this.screenHeight = Screen.height;
    }

    public void SaveTrial(double rt, string type, int score, int total, double threshold, bool validTrial, int validTrialCount) {
        this.rt = type == "missed" ? null : RoundTime(rt, 7);
        this.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.responseType = type;
        this.trialScore = score;
        this.totalScore = total;
        this.threshold = threshold;
        this.validTrial = validTrial;
        this.validTrialCount = validTrialCount;
    }

    public void SaveStimulus(Dictionary<string, float> stimSpec){
        this.canvasWidth = stimSpec["canvasWidth"];
        this.canvasHeight = stimSpec["canvasHeight"];
        this.canvasScaleFactor = stimSpec["canvasScale"];
        this.boneX = stimSpec["x"];
        this.boneY = stimSpec["y"];
        this.boneOrientation = stimSpec["rotation"];
        this.boneScale = stimSpec["scale"];
    }

    double RoundTime(double time, int dp) {
        return Math.Round(time * Math.Pow(10, dp)) / Math.Pow(10, dp);
    }
}
