using System; // Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Change scenes
using TMPro; // TextMeshProUGUI

public class Score : MonoBehaviour
{
    // Element references
    public TextMeshProUGUI scoreText; // Score display text
    public TextMeshProUGUI feedbackText; // Feedback
    public List<HealthBar> healthBars; // List of all 3 healthbars
    HealthBar healthBar; // Holds reference health bar for current level
    public Dog dog; // For changing dog sprite

    // Settings ----------------
    // Levels
    public int nLevels = 3; // Number of levels to run
    int level = 1; // Track progress
    // Trials
    public int nTrials = 60; // Rough number of trials per participant
    public int minTrials = 10;
    // Scores
    public int minScore = 100; // Minimum score for fast trial
    public int maxScore = 200; // Max score for super fast trial
    int totalScore = 0;
    // RTs
    public double minRT = .15; // Minimum RT bound
    public double maxRT = .6; // Init Maximum RT bound to 600ms, to be changed later

    // ******************* METHODS/FUNCTIONS *******************
    // Calculate score from rt
    public int calculateScore(double rt) {
        // Calculate score
        double clampedRT = Math.Clamp(rt, minRT, maxRT); // Clamp Reaction Time, ensures 0-1 when normalised
        double normalisedRT = (clampedRT - minRT) / (maxRT - minRT); // Normalise as proportion of range
        Debug.Log("calcScore normalisedRT: " + normalisedRT);
        double complementRT = (1 - normalisedRT); // Complement of proportion


        // Score
        double scoreRange = maxScore - minScore;
        double adjustedRT = complementRT * scoreRange; // Multiply proportion by range
        double bumpedScore = minScore + adjustedRT; // Bump up by minimum score
        double clampedScore = Math.Min(bumpedScore, maxScore); // Techically does nothing if min=100 max=200

        return (int)clampedScore;
    }

    // Save current score and display
    public void updateScore(int trialScore){
        // Give visual feedback
        string trialType = getTrialType(trialScore);
        giveFeedback(trialType);

        // Calc new total score
        totalScore += trialScore;
        // Compare to min/max healthbar
        if(totalScore >= healthBar.GetMaxHealth()) changeLevel(); // Switch healthbars if above maximum
        else if(totalScore < healthBar.GetMinHealth()) totalScore = (int)healthBar.GetMinHealth(); // Stop score going below healthbar minimum
        // Display new score
        healthBar.SetHealth(totalScore); // Note do this prior to changing level to start healthbar on new minimum
        scoreText.text = "Score: " + totalScore;
        //Save data
        DataManager.Instance.data.currentTrial().trialScore = trialScore;
        DataManager.Instance.data.currentTrial().totalScore = totalScore;
    }

    public string getTrialType(int trialScore){
        if(trialScore == -minScore){ // Early Press
            return "early";
        } else if(trialScore == 0){ // Slow trial
            return "slow";
        } else { // Good trial
            return "fast";
        }
    }

    public void giveFeedback(string trialType){
        // feedback 
        Color barColour = Color.green;
        string feedback = "";

        if(trialType == "early"){
            barColour = Color.red;
            feedbackText.color = Color.red;
            feedback = "TOO QUICK!\nWait until the bone has appeared.";
            dog.takeDamage();

        } else if(trialType == "slow"){
            barColour = Color.blue;
            feedbackText.color = Color.white;
            feedback = "TOO SLOW!\nDoggo couldn't catch the bone.";
            dog.whine();

        } else if(trialType == "fast"){
            barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // "forest"
            feedbackText.color = Color.green;
            feedback = "GREAT!\nDoggo caught the bone!";
            dog.chew();
        } else if(trialType == "missed"){
            barColour = Color.blue;
            feedbackText.color = Color.red;
            feedback = "TOO SLOW!\nDoggo is upset";
        }

        feedbackText.text = feedback;
        healthBar.setColour(barColour);
    }
    
    // Level 1,2,3
    public void changeLevel(){
        level++;
        if(level>nLevels){
            endTask();
            return;
        }
        dog.NextSprite();
        feedbackText.text = "Level " +level+"!";
        nextHealthBar();
    }

    // Switch healthbars
    public void nextHealthBar(){
        // Get previous healthbar maximum
        int previousMaximum = (int)healthBar.GetMaxHealth(); // Minimum set to last healthbar's maximum
        // Fill current healthbar
        healthBar.SetHealth(previousMaximum);
        // Change healthbar from array
        healthBar = healthBars[level-1];
        // Set min and max for new healthbar
        healthBar.SetMinHealth(previousMaximum); // Healthbar minimum to last healthbar's maximum
        healthBar.SetMaxHealth(getNewTargetScore()); // Set healthbar maximum
    }
    
    // get
    public int getNewTargetScore(){
        // Calculate number of trials in current level.
        int nTrialsRemaining = nTrials - DataManager.Instance.data.trials.Count; // Number of trials remaining in task.
        int nTrialsPerLevelsRemaining = nTrialsRemaining / ((nLevels+1) - level); // Number of trials in each remaining level.
        // Calulate target score
        int nFastTrials = nTrialsPerLevelsRemaining/2; 
        int targetScore = minScore * nFastTrials;
        // Clamp to minimum
        int minTargetScore = minTrials/2 * minScore; // e.g. 10 min trials = 10/2=5*100=500
        if(targetScore < minTargetScore) targetScore = minTargetScore;
        // Add target to current score
        if(nTrialsRemaining < nTrials) targetScore += totalScore;
        Debug.Log("New Target Score: " + targetScore);
        return targetScore;
    }

    void endTask(){
        // Load next scene
        SceneManager.LoadScene("End");
    }


    // Start is called before the first frame update
    void Start() {
        healthBar = healthBars[0]; // Load up first healthbar
        healthBar.SetMaxHealth(getNewTargetScore());
    }


}
