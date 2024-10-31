using UnityEngine;
using TMPro; // TextMeshProUGUI
using System.Collections; // IEnumerator

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
    public AudioSource backgroundMusic;
    // Global var
    public bool changingLevel = false;

    // Main function for displaying feedback based on performance
    public void GiveFeedback(string trialType, int newTotalScore, int trialScore=0){
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
            feedbackText.color = Color.blue;
            feedback = "ALMOST!\nDoggo missed the bone";
            changeTextHex = "#0000FF";
            // Animations + sounds
            dog.Surprised();
            dog.StartJump(30);

        } else if(trialType == "fast"){
            // Bar colour
            barColour = new Color(0.06770712f, 0.5817609f, 0f, 1f); // "forest"
            // Feedback text
            feedbackText.color = Color.green;
            feedback = "GREAT!\nDoggo caught the bone in the air";
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
            feedback = "TOO SLOW!\nThe bone has been stolen by another animal...";
            changeTextHex = "#0000FF";
            // Animations + sounds
            bone.Throw();
            //dog.whine();
        }

        DisplayFeedback(feedback, newTotalScore, trialScore, changeTextHex, barColour);
    }

    void DisplayFeedback(string feedback, int newTotalScore, int trialScore, string changeTextHex, Color barColour){
        scoreChange.enabled = true;
        feedbackText.enabled = true;
        feedbackText.text = feedback;
        scoreText.text = "Score: " + newTotalScore;
        string sign = trialScore >= 0 ? "+" : "";
        scoreChange.text = "<color=" + changeTextHex + ">" + sign + trialScore;
        // Display new score
        healthBarManager.currentHealthBar.SetColour(barColour);
        healthBarManager.currentHealthBar.SetHealth(newTotalScore); // Note do this prior to changing level to start healthbar on new minimum
    }

    // Level 1,2,3
    public IEnumerator ChangeLevel(int level, int targetScore){
        // Clear prompts
        changingLevel = true;
        backgroundMusic.Stop();
        scoreChange.text = ""; 
        // Change dog
        Prompt("What? Doggo is changing...");
        yield return StartCoroutine(dog.Evolve(level));
        // Prompt new level
        Prompt("Level " + level +"!\n<size=80%>Press the down arrow <sprite name=\"down_arrow\"> to continue");
        // Reset Health Bars and score
        healthBarManager.SetNewHealthBar(level, targetScore);
        scoreText.text = "Score: 0";

        // Change background music pitch before resume function called in trialmanager
        if(level==2){
            backgroundMusic.pitch = 1.15f;
        } else if(level==3){
            backgroundMusic.pitch = 1.3f;
        }
        changingLevel = false;
    }

    public void ResumeBackgroundMusic(){
        if(!backgroundMusic.isPlaying) backgroundMusic.Play();
    }

    // Use this for presenting standard black text in between trials
    public void Prompt(string text=""){
        scoreChange.text = "";
        feedbackText.color = Color.white;
        feedbackText.text = text;
    }

    public void Hide(){
        scoreChange.enabled = false;
        feedbackText.enabled = false;
    }

}
