using System;
using System.Collections.Generic;
using UnityEngine;

// Individual trial data
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
    
    public Trial(int trialN, double isi) // Constructor
    {
        trialNumber = trialN;
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.isi = isi;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    public void SaveTrial(TrialResult result) // Add vars on response
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

    public void SaveStimulus(Dictionary<string, float> stimulusSpecifications){ // Add stimulus specifications on show
        this.canvasWidth = stimulusSpecifications["canvasWidth"];
        this.canvasHeight = stimulusSpecifications["canvasHeight"];
        this.canvasScaleFactor = stimulusSpecifications["canvasScale"];
        this.stimulusX = stimulusSpecifications["x"];
        this.stimulusY = stimulusSpecifications["y"];
        this.stimulusOrientation = stimulusSpecifications["rotation"];
        this.stimulusScale = stimulusSpecifications["scale"];
    }
}