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

        if(trialType == "early"){
            // Bar colour
            barColour = Color.red;
            // Feedback text
            feedbackText.color = Color.red;
            feedback = "TOO QUICK!\nWait until the bone has appeared.";
            // Animations + sounds
            dog.bark();
            dog.takeDamage();
            dog.startJump(20);
            
        } else if(trialType == "slow"){
            // Bar colour
            barColour = Color.blue;
            // Feedback text
            feedbackText.color = Color.white;
            feedback = "A bit too slow!\nDoggo couldn't catch the bone.";
            // Animations + sounds
            dog.surprised();
            dog.startJump(20);

        } else if(trialType == "fast"){
            // Bar colour
            barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // "forest"
            // Feedback text
            feedbackText.color = Color.green;
            feedback = "GREAT!\nDoggo caught the bone!";
            // Animations + sounds
            dog.chew();
            dog.startJump(trialScore/4);

        } else if(trialType == "missed"){
            // Bar colour
            barColour = Color.blue;
            feedbackText.color = Color.red;
            // Feedback text
            feedback = "TOO SLOW!\nAnother dog got the bone first.";
            // Animations + sounds
            bone.Throw();
            //dog.whine();
        }

        displayFeedback(feedback, newTotalScore, barColour);
    }

    void displayFeedback(string feedback, int newTotalScore, Color barColour){
        feedbackText.text = feedback;
        scoreText.text = "Score: " + newTotalScore;
        // Display new score
        healthBarManager.currentHealthBar.setColour(barColour);
        // Check level
        if(newTotalScore >= healthBarManager.currentHealthBar.GetMaxHealth()) changeLevel(); // Switch healthbars if above maximum - confusingly lost in here maybe??
        healthBarManager.currentHealthBar.SetHealth(newTotalScore); // Note do this prior to changing level to start healthbar on new minimum
    }

    // Level 1,2,3
    public void changeLevel(){
        scoreManager.level += 1;
        if(scoreManager.level>scoreManager.nLevels){
            trialManager.endTask();
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

}
