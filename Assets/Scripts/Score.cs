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
    public int totalTrials = 60; // Rough number of trials per participant
    public int level1TargetScore = 1000; // First level target score and minimum
    public int minLevelTargetScore = 500; // First level target score and minimum
    // Scores
    public int earlyScore = -100; // Score for early trial = -100 (flat rate)
    public int slowScore = 0; // Score for slow (> threshold) trial
    public int minScore = 100; // Minimum score for fast trial
    public int maxScore = 200; // Max score for super fast trial 
    // RTs - boundaries on min/max RT of interest to avoid overshooting
    public double minRT = .15; // Minimum RT bound
    public double maxRT = .6; // Init Maximum RT bound to 600ms, to be changed later

    // Track progress
    int level = 1; // Stages 1-3
    int score = 0;

    // ******************* METHODS/FUNCTIONS *******************
    // Calculate score from rt
    public int calculateScore(double rt) {
        // Calculate score
        double clampedRT = Math.Clamp(rt, minRT, maxRT); // Clamp Reaction Time, ensures 0-1 when normalised
        double normalisedRT = (clampedRT - minRT) / (maxRT - minRT); // Normalise as proportion of range
        double complementRT = (1 - normalisedRT); // Complement of proportion

        // Score
        double scoreRange = maxScore - minScore;
        double adjustedRT = complementRT * scoreRange; // Multiply promportion by range
        double finalScore = minScore + adjustedRT; // Bump up by minimum score
        double clampedScore = Math.Min(finalScore, maxScore); // Techically does nothing if min=100 max=200

        return (int)clampedScore;
    }

    // Save current score and display
    public void change(int trialScore){
        // Give visual feedback
        giveFeedback(trialScore);

        // Calc new total score
        score = score+trialScore;
        // Change healthbars
        if(score >= healthBar.GetMaxHealth()) changeLevel(); // Switch healthbars if above maximum
        else if(score < healthBar.GetMinHealth()) score = (int)healthBar.GetMinHealth(); // Stop score going below healthbar minimum
        // Display new score
        healthBar.SetHealth(score); // Note do this prior to changing level to start healthbar on new minimum
        scoreText.text = "Score: " + score;

        //Save data
        DataManager.Instance.data.currentTrial().trialScore = trialScore;
        DataManager.Instance.data.currentTrial().totalScore = score;
    }

    public void giveFeedback(int trialScore){
        // Set default colours
        Color barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // 'forest' - colour of positive feedback
        feedbackText.color = Color.white;
        string feedback = "";

        if(trialScore == earlyScore){ //if too slow
            barColour = Color.red;
            feedbackText.color = Color.red;
            dog.takeDamage();
            feedback = "TOO QUICK!\nWait until the bone has appeared.";

        } else if(trialScore == slowScore){ // If early press
            barColour = Color.blue;
            feedback = "Too slow!\nThe bone went bad...";

        } else if(trialScore <= (((maxScore-minScore)/2)+minScore)) { // If less than half way to max score
            feedback = "Yum!\nDoggo fetched the bone!";

        } else { //if middling score
            feedback = "GREAT!\nDoggo caught the bone!";
            feedbackText.color = Color.green;
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
        int nTrialsRemaining = totalTrials - DataManager.Instance.data.trials.Count; // Number of trials remaining in task.
        int nTrialsPerLevelsRemaining = nTrialsRemaining / ((nLevels+1) - level); // Number of trials in each remaining level.
        // Calulate target score
        int nFastTrials = nTrialsPerLevelsRemaining/2; 
        int targetScore = minScore * nFastTrials;
        // Clamp to minimum
        if(targetScore < minLevelTargetScore) targetScore = minLevelTargetScore;
        // Add target to current score
        targetScore += score;
        return targetScore;
    }

    void endTask(){
        // Load next scene
        SceneManager.LoadScene("End");
    }


    // Start is called before the first frame update
    void Start() {
        healthBar = healthBars[0]; // Load up first healthbar
        healthBar.SetMaxHealth(level1TargetScore);
    }


}
