using System; // Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshProUGUI

public class Score : MonoBehaviour
{
    // Element references
    public TextMeshProUGUI scoreText; // displays score
    public TextMeshProUGUI feedbackText; //feedback
    public List<HealthBar> healthBars; // List of all 3 healthbars
    HealthBar healthBar; // Holds reference health bar for current level
    public Dog dog;

    // Settings ----------------
    // Levels
    public int nLevels = 3; // Number of levels to run
    public int totalTrials = 60; // Rough number of trials per participant
    // Scores
    public int slowScore = -100; // Score for early trial = -100 (flat rate)
    public int earlyScore = 0; // Score for slow (> threshold) trial
    public int minScore = 100; // Minimum score for fast trial
    public int maxScore = 200; // Max score for super fast trial 
    // RTs - boundaries on min/max RT of interest to avoid overshooting
    public double minRT = .2; // Minimum RT bound
    public double maxRT = .6; // Maximum RT bound

    // Track progress
    int level = 1; // Stages 1-3
    int score = 0;

    // ******************* METHODS/FUNCTIONS *******************

    public void set(int trialScore){
        ///int trialScore = calculateScore(rt);
        // Display
        giveFeedback(trialScore);
        score = score+trialScore;
        healthBar.SetHealth(score);
        // Switch healthbars if above maximum
        if(score >= healthBar.GetMaxHealth()) changeLevel(); // if health bars are 0-x then score needs to be reset or added to max
        // Display
        scoreText.text = "Score: " + score;
        // Save
        DataManager.Instance.data.currentTrial().score = score;
    }

    // PUT NEW SCORE CALCULATIONS IN HERE
    public int calculateScore(double rt) {
        Debug.Log("rt");
        Debug.Log(rt);

        // Calculate score
        double clampedRT = Math.Clamp(rt, minRT, maxRT); // Clamp Reaction Time, ensures 0-1 when normalised
        double normalisedRT = (clampedRT - minRT) / (maxRT - minRT); // Normalise as proportion of range
        double complementRT = (1 - normalisedRT); // Complement of proportion

        // Score
        double scoreRange = maxScore - minScore;
        double adjustedRT = complementRT * scoreRange; // Multiply promportion by range
        double finalScore = minScore + adjustedRT; // Bump up by minimum score
        double clampedScore = Math.Min(finalScore, maxScore); // Techically does nothing if min=100 max=200

        Debug.Log("clampedScore");
        Debug.Log((int)clampedScore);
        
        return (int)clampedScore;
    }

    public void giveFeedback(int score){
        // Set default colours
        Color barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // 'forest' - colour of positive feedback
        feedbackText.color = Color.white;
        string feedback;

        if(score == slowScore || score < 0){ //if too slow
            barColour = Color.red;
            feedbackText.color = Color.red;
            feedback = "TOO QUICK!\nWait until the sausage has appeared.";

        } else if(score == earlyScore || score == 0){ // If early press
            barColour = Color.blue;
            feedback = "Too slow!\nThe sausage went bad...";

        } else if(score <= (((maxScore-minScore)/2)+minScore)) { // If less than half way to max score
            feedback = "Yum!\nDoggo fetched the sausage!";

        } else { //if middling score
            feedback = "GREAT!\nDoggo caught the sausage!";
        }

        feedbackText.text = feedback;
        healthBar.setColour(barColour);
    }
    
    public void changeLevel(){
        level++;
        if(level>nLevels) return;
        feedbackText.text = "Level " +level+"!";
        nextHealthBar();
    }

    public void nextHealthBar(){
        float previousTarget = healthBar.GetMaxHealth(); // Minimum set to last healthbar's maximum
        healthBar = healthBars[level-1];
        healthBar.SetMinHealth((int)previousTarget); // Healthbar minimum to last healthbar's maximum
        healthBar.SetMaxHealth(getNewTargetScore()); // Set healthbar maximum
    }

    public int getNewTargetScore(){
        // Calculate number of trials in current level.
        int nTrialsRemaining = totalTrials - DataManager.Instance.data.trials.Count; // Number of trials remaining in task.
        int nTrialsPerLevelsRemaining = nTrialsRemaining / ((nLevels+1) - level); // Number of trials in each remaining level.

        // Calulate target score
        int nFastTrials = nTrialsPerLevelsRemaining/2;
        int targetScore = minScore * nFastTrials;

        return targetScore;
    }

    // Start is called before the first frame update
    void Start() {
        healthBar = healthBars[0];
    }

}
