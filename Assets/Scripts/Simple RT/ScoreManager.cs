using System; // Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Change scenes
using TMPro; // TextMeshProUGUI

public class ScoreManager : MonoBehaviour
{
    // Env refs
    public Feedback feedback; // For passing trialType and totalScore from here into giveFeedback
    public TrialManager trialManager; // For median RT
    public HealthBarManager healthBarManager; // For getting current level's min health

    // Settings ----------------
    // Levels
    public int nLevels = 3; // Number of levels to run
    public int level = 1; // Track progress
    // Trials
    public int nTrials = 60; // Rough number of trials per participant
    public int validTrialCount = 0;
    public int minTrials = 10; // Minimum number of trials per level
    // Scores
    public int minScore = 100; // Minimum score for fast trial
    public int maxScore = 200; // Max score for super fast trial
    public int totalScore = 0; // Tracks total score
    // RTs
    public double minRT = .15; // Minimum RT bound
    public double maxRT = .6; // Init Maximum RT bound to 600ms, to be changed later
    public double medianRT = 0;
    ArrayList sortedRTs = new(); // Store rts in ArrayList to allow for easier median computation and store as sorted list (i.e. sortedRTs.Sort() method)

    // ******************* METHODS/FUNCTIONS *******************
    public void ProcessTrialResult(double reactionTime) {
        string responseType = DetermineResponseType(reactionTime);
        bool validTrial = TestTrialValidity(responseType);
        int trialScore = CalculateTrialScore(responseType, reactionTime);
        UpdateTotalScore(trialScore);
        SaveTrialData(reactionTime, responseType, trialScore, validTrial);
        ProvideFeedback(responseType, trialScore);
        // Get new thresholds and bounds using this trials RT
        if(validTrial) UpdateThresholds(reactionTime);
    }

    // Response Types
    string DetermineResponseType(double reactionTime) {
        if (reactionTime < 0)
            return "early";
        else if (reactionTime > medianRT) // *2 makes things a bit easier
            return "slow";
        else if(reactionTime < maxRT)
            return "fast";
        else if(reactionTime >= maxRT)
            return "missed";
        else
            throw new ArgumentException("Could not determine response type", nameof(reactionTime));
    }

    bool TestTrialValidity(string trialType){
        if(trialType == "slow" || trialType == "fast"){
            validTrialCount++;
            return true;
        } else {
            return false;
        }
    }

    // Calculate score from rt
    int CalculateScore(double rt) {
        // Calculate score
        double clampedRT = Math.Clamp(rt, minRT, maxRT); // Clamp Reaction Time, ensures 0-1 when normalised
        double normalisedRT = (clampedRT - minRT) / (maxRT - minRT); // Normalise as proportion of range
        // Note normalisedRT will return NaN if minRT=maxRT
        // Debug.Log("calcScore normalisedRT: " + normalisedRT);
        double complementRT = (1 - normalisedRT); // Complement of proportion

        // Score
        int scoreRange = maxScore - minScore;
        double adjustedRT = complementRT * scoreRange; // Multiply proportion by range
        double bumpedScore = minScore + adjustedRT; // Bump up by minimum score
        double clampedScore = Math.Min(bumpedScore, maxScore); // Techically does nothing if min=100 max=200

        return (int)clampedScore;
    }
    
    int CalculateTrialScore(string responseType, double reactionTime){
        switch (responseType){
            case "early":
                return -minScore;
            case "missed":
            case "slow":
                return 0;
            case "fast":
                return CalculateScore(reactionTime);
            default:
                throw new ArgumentException("Invalid trial type", nameof(responseType));
        }
    }

    // Saving/ Presenting
    void UpdateTotalScore(int trialScore) {
        totalScore += trialScore;
        int scoreLowerBound = (int)healthBarManager.currentHealthBar.GetMinHealth();
        totalScore = Math.Max(totalScore, scoreLowerBound);
    }

    void SaveTrialData(double reactionTime, string responseType, int trialScore, bool validTrial){ // This could be better?
        DataManager.Instance.data.CurrentTrial().SaveTrial(reactionTime, responseType, trialScore, totalScore, medianRT, validTrial, validTrialCount);
    }

    void ProvideFeedback(string responseType, int trialScore) {
        feedback.giveFeedback(responseType, totalScore, trialScore);
    }

    // Threshold Functions
    void UpdateThresholds(double reactionTime){
        UpdateMedianRT(reactionTime);
        maxRT = medianRT*2; //Math.Max(scoreManager.minRT*2, medianRT*2); // Lowerbound on maxRT of minRT*2
    }

        // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    void UpdateMedianRT(double rt){
        medianRT = CalculateMedianRT(rt);
        // Adjust for starter trials
        int trialN = DataManager.Instance.data.trials.Count
        if(trialN <= 10){
            medianRT = MedianBurnInAdjustment(medianRT, trialN);
        }
    }

    double CalculateMedianRT(double rt){
        // Add to array and sort
        double clampedRT = Math.Clamp(rt, minRT, maxRT); // Clamp Reaction Time
        sortedRTs.Add(clampedRT); // Add to median score list
        sortedRTs.Sort(); // Note mutates original list
        // Get the median
        int size = sortedRTs.Count;
        int mid = size / 2;
        double middleValue = (double)sortedRTs[mid];
        double median = (size % 2 != 0) ? middleValue : (middleValue + (double)sortedRTs[mid - 1]) / 2;
        return median;
    }

    double MedianBurnInAdjustment(double median, int trialN){
        median += median * Math.Min(0, 1-trialN/10);
        return median;
    }

    // CONSIDER MOVING THIS ELSEWHERE - feedback or healthbar
    public int GetNewTargetScore(){
        // Calculate number of trials in current level.
        int nTrialsRemaining = nTrials - validTrialCount; // Number of trials remaining in task.
        int nTrialsPerLevelsRemaining = nTrialsRemaining / ((nLevels+1) - level); // Number of trials in each remaining level.
        // Calulate target score
        int nFastTrials = nTrialsPerLevelsRemaining/2; 
        int targetScore = minScore * nFastTrials;
        // Clamp to minimum
        int minTargetScore = (minTrials/2) * minScore; // e.g. 10 min trials = 10/2=5*100=500
        if(targetScore < minTargetScore) targetScore = minTargetScore;
        // Add target to current score
        if(nTrialsRemaining < nTrials) targetScore += totalScore;
        // Log Score and return
        Debug.Log("New Target Score: " + targetScore);
        return targetScore;
    }

    int GetL1nQuery(string queryVar, int defaultN = 60){
        int l1n;
        if (int.TryParse(queryVar, out l1n)) {
            return l1n;
        } else {
            return defaultN;
        }
    }

    void Start(){
        nTrials = GetL1nQuery(DataManager.Instance.data.metadata.l1n, 60);
        medianRT = minRT + ((maxRT-minRT)/2); // initialise median to half maximum RT  
    }
}
