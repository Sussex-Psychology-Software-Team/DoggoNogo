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
    public ScoreManager scoreManager;
    public TrialManager trialManager; // Contains endTask function

    public void giveFeedback(string trialType, int newTotalScore){
        // feedback 
        Color barColour = Color.green;
        string feedback = "";

        if(trialType == "early"){
            barColour = Color.red;
            feedbackText.color = Color.red;
            feedback = "TOO QUICK!\nWait until the bone has appeared.";
            dog.takeDamage();
            dog.bark();

        } else if(trialType == "slow"){
            barColour = Color.blue;
            feedbackText.color = Color.white;
            feedback = "A bit too slow!\nDoggo couldn't catch the bone.";
            dog.whine();

        } else if(trialType == "fast"){
            barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // "forest"
            feedbackText.color = Color.green;
            feedback = "GREAT!\nDoggo caught the bone!";
            dog.chew();

        } else if(trialType == "missed"){
            barColour = Color.blue;
            feedbackText.color = Color.red;
            feedback = "TOO SLOW!\nAnother dog got the bone first.";
        }

        feedbackText.text = feedback;
        scoreText.text = "Score: " + newTotalScore;
        // Display new score
        healthBarManager.currentHealthBar.setColour(barColour);
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
    public void Prompt(string text){
        feedbackText.color = Color.black;
        feedbackText.text = text;
    }

}
