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
    public void getScore(double rt){
        string trialType = getTrialType(rt);
        int trialScore = getTrialScore(trialType, rt);
        totalScore = getNewTotalScore(trialScore);
        DataManager.Instance.data.currentTrial().saveTrial(rt, trialType, trialScore, totalScore);
        feedback.giveFeedback(trialType, totalScore, trialScore);
    }
    
    // Calculate score from rt
    public int calculateScore(double rt) {
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

    public string getTrialType(double rt){
        string trialType = "";
        if(rt < 0) {
            trialType = "early";
        }  else if(rt > trialManager.medianRT) {
            trialType = "slow";
        } else if(rt < trialManager.medianRT) {
            trialType = "fast";
        }

        return(trialType);
    }

    public int getTrialScore(string trialType, double rt){
        // get new median reaction time
        int trialScore = 0;
        if(trialType == "early") { // Early trial
            trialScore = -minScore; // negative min score
        }  else if(trialType == "slow") { // Slow trial
            trialScore = 0; // no points
        } else if(trialType == "fast") {
           trialScore = calculateScore(rt); // get new 
        }
        return(trialScore);
    }
    
    public int getNewTotalScore(int trialScore){
        // Calc new total score
        totalScore += trialScore;
        // Compare to min/max healthbar
        int scoreLowerBound = (int)healthBarManager.currentHealthBar.GetMinHealth();
        if(totalScore < scoreLowerBound) totalScore = scoreLowerBound; // Stop score going below healthbar minimum
        // Return
        return(totalScore);
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
