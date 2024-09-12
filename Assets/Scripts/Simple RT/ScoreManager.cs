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
    public int minTrials = 10; // Minimum number of trials per level
    // Scores
    public int minScore = 100; // Minimum score for fast trial
    public int maxScore = 200; // Max score for super fast trial
    public int totalScore = 0; // Tracks total score
    // RTs
    public double minRT = .15; // Minimum RT bound
    public double maxRT = .6; // Init Maximum RT bound to 600ms, to be changed later

    // ******************* METHODS/FUNCTIONS *******************
    public void ProcessTrialResult(double reactionTime) {
        string trialType = DetermineTrialType(reactionTime);
        int trialScore = CalculateTrialScore(trialType, reactionTime);
        UpdateTotalScore(trialScore);
        SaveTrialData(reactionTime, trialType, trialScore);
        ProvideFeedback(trialType, trialScore);
    }
    
    string DetermineTrialType(double reactionTime) {
        if (reactionTime < 0)
            return "early";
        else if (reactionTime > trialManager.medianRT) // *2 makes things a bit easier
            return "slow";
        else
            return "fast";
    }

    // Calculate score from rt
    private int CalculateTrialScore(string trialType, double reactionTime){
        switch (trialType){
            case "early":
                return -minScore;
            case "slow":
                return 0;
            case "fast":
                return calculateScore(reactionTime);
            default:
                throw new ArgumentException("Invalid trial type", nameof(trialType));
        }
    }
    
    void UpdateTotalScore(int trialScore) {
        totalScore += trialScore;
        int scoreLowerBound = (int)healthBarManager.currentHealthBar.GetMinHealth();
        totalScore = Math.Max(totalScore, scoreLowerBound);
    }

    void SaveTrialData(double reactionTime, string trialType, int trialScore){
        DataManager.Instance.data.currentTrial().saveTrial(reactionTime, trialType, trialScore, totalScore);
    }

    void ProvideFeedback(string trialType, int trialScore) {
        feedback.giveFeedback(trialType, totalScore, trialScore);
    }

    // CONSIDER MOVING THIS ELSEWHERE - feedback or healthbar
    public int GetNewTargetScore(){
        // Calculate number of trials in current level.
        int nTrialsRemaining = nTrials - DataManager.Instance.data.trials.Count; // Number of trials remaining in task.
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
}
