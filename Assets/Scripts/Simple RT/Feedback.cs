using System; // Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Change scenes
using TMPro; // TextMeshProUGUI

public class Feedback : MonoBehaviour
{
    // Element references
    public TextMeshProUGUI scoreText; // Score display text
    public TextMeshProUGUI scoreChange; // Score display text
    public TextMeshProUGUI feedbackText; // Feedback
    public HealthBarManager healthBarManager; // Contains reference to current selected health bar
    public Dog dog; // For changing dog sprite
    public Bone bone;
    public ScoreManager scoreManager;
    public TrialManager trialManager; // Contains endTask function

    // Main function for displaying feedback based on performance
    public void giveFeedback(string trialType, int newTotalScore, int trialScore=0){
        // feedback
        Color barColour = Color.green;
        string feedback = "";
        string changeTextHex = "";

        if(trialType == "early"){
            // Bar colour
            barColour = Color.red;
            // Feedback text
            feedbackText.color = Color.red;
            feedback = "TOO QUICK!\nWait until the bone has appeared.";
            changeTextHex = "#FF0000";
            // Animations + sounds
            dog.Bark();
            dog.TakeDamage();
            dog.StartJump(15);

        } else if(trialType == "slow"){
            // Bar colour
            barColour = Color.blue;
            // Feedback text
            feedbackText.color = Color.white;
            feedback = "A bit too slow!\nDoggo couldn't catch the bone.";
            changeTextHex = "#0000FF";
            // Animations + sounds
            dog.Surprised();
            dog.StartJump(30);

        } else if(trialType == "fast"){
            // Bar colour
            barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // "forest"
            // Feedback text
            feedbackText.color = Color.green;
            feedback = "GREAT!\nDoggo caught the bone!";
            // Animations + sounds
            bone.Eat();
            dog.Chew();
            dog.StartJump((int)(trialScore/2));
            changeTextHex = "#119400";

        } else if(trialType == "missed"){
            // Bar colour
            barColour = Color.blue;
            feedbackText.color = Color.red;
            // Feedback text
            feedback = "TOO SLOW!\nAnother dog got the bone first.";
            changeTextHex = "#0000FF";
            // Animations + sounds
            bone.Throw();
            //dog.whine();
        }

        displayFeedback(feedback, newTotalScore, trialScore, changeTextHex, barColour);
    }

    void displayFeedback(string feedback, int newTotalScore, int trialScore, string changeTextHex, Color barColour){
        scoreChange.enabled = true;
        feedbackText.enabled = true;
        feedbackText.text = feedback;
        scoreText.text = "Score: " + newTotalScore;
        string sign = trialScore >= 0 ? "+" : "";
        scoreChange.text = "<color=" + changeTextHex + ">" + sign + trialScore;
        // Display new score
        healthBarManager.currentHealthBar.SetColour(barColour);
        healthBarManager.currentHealthBar.SetHealth(newTotalScore); // Note do this prior to changing level to start healthbar on new minimum
        // Check level
        if(newTotalScore >= healthBarManager.currentHealthBar.GetMaxHealth()) changeLevel(); // Switch healthbars if above maximum - confusingly lost in here maybe??
    }

    // Level 1,2,3
    public void changeLevel(){
        scoreManager.level += 1;
        if(scoreManager.level>scoreManager.nLevels){
            trialManager.EndTask();
            return;
        } else {
            // Change dog
            dog.GetSprite(scoreManager.level);
            // Change Health Bars
            int newTargetScore = scoreManager.GetNewTargetScore();
            healthBarManager.SetNewHealthBar(scoreManager.level, newTargetScore);
            // Prompt new level
            Prompt("Level " +scoreManager.level+"!");
        }
    }

    // Use this for presenting standard black text in between trials
    public void Prompt(string text=""){
        feedbackText.color = Color.black;
        feedbackText.text = text;
    }

    public void Hide(){
        scoreChange.enabled = false;
        feedbackText.enabled = false;
    }

}
