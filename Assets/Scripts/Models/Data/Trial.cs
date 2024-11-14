using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trial
{
    public int trialNumber;
    public double isi;
    public double rt;
    public string start;
    public string end;
    public int trialScore;
    public int totalScore;
    public string responseType;
    public double threshold;
    public bool validTrial;
    public int validTrialCount;
    // Stim spec
    public float screenWidth;     // Current display width
    public float screenHeight;    // Current display height
    public float canvasWidth;
    public float canvasHeight;
    public float canvasScaleFactor;
    public float stimulusX;
    public float stimulusY;
    public float stimulusOrientation;
    public float stimulusScale;
    
    public Trial(int trialN, double isi) 
    {
        trialNumber = trialN;
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.isi = isi;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    public void SaveTrial(TrialResult result)
    {
        rt = result.ReactionTime;
        end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        responseType = result.ResponseType;
        trialScore = result.TrialScore;
        totalScore = result.TotalScore;
        threshold = result.Threshold;
        validTrial = result.ValidTrial;
        validTrialCount = result.ValidTrialCount;
    }

    public void SaveStimulus(Dictionary<string, float> stimSpec){
        this.canvasWidth = stimSpec["canvasWidth"];
        this.canvasHeight = stimSpec["canvasHeight"];
        this.canvasScaleFactor = stimSpec["canvasScale"];
        this.stimulusX = stimSpec["x"];
        this.stimulusY = stimSpec["y"];
        this.stimulusOrientation = stimSpec["rotation"];
        this.stimulusScale = stimSpec["scale"];
    }
}
