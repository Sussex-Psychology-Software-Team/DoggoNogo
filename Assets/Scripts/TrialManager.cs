using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Diagnostics; // Stopwatch included here
using System.Collections;
using System.Collections.Generic; //for List
using TMPro; //for TextMeshProUGUI


public class TrialManager : MonoBehaviour
{
    // ******************* CONFIG *******************
    public float[] ISIRange = { 1f, 4f }; // Input floats to Random.Range to return a float

    // ******************* GLOBAL VARS *******************
    // trialISI
    double trialISI; // Stores each trial's trialISI for speed of access
    ArrayList sortedRTs = new(); // Store rts in ArrayList to allow for easier median computation and store as sorted list (i.e. sortedRTs.Sort() method)
    
    // Timers
    public Stopwatch stimulusTimer = new Stopwatch(); // High precision timer: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks
    public Stopwatch reactionTimer = new Stopwatch(); // https://stackoverflow.com/questions/394020/how-accurate-is-system-diagnostics-stopwatch

    // Feedback/score
    public Score score;
    public TextMeshProUGUI feedbackText; //feedback
    public Dog dog;
    public Bone bone;

    // ******************* FUNCTIONS *******************
    // RT Helpers ------------------------------------------------------------
    // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    public double calcMedianRT() {
        // Add to array and sort
        sortedRTs.Add(DataManager.Instance.data.currentTrial().rt);
        sortedRTs.Sort(); // Note mutates original list
        // Get the median
        int size = sortedRTs.Count;
        int mid = size / 2;
        double middleValue = (double)sortedRTs[mid];
        double median = (size % 2 != 0) ? middleValue : (middleValue + (double)sortedRTs[mid - 1]) / 2;
        Debug.Log("median RT: " + median);
        return median;
    }

    // TRIAL MANAGEMENT ------------------------------------------------------------
    // Coroutine to delay the start of a new trial and show feedback
    IEnumerator DelayBeforeNextTrial() {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        newTrial(); // Start new trial after delay
    }

    void newTrial() { //function to reset variables and set-up for a new trials
        bone.Hide();
        // Get new ISI
        trialISI = UnityEngine.Random.Range(ISIRange[0], ISIRange[1]); // New trialISI
        // Create new trial in data structure
        DataManager.Instance.data.newTrial(trialISI); // Create an instance of a Trial
        // Prompt participant that new trial is starting
        presentText("Getting a new bone for Doggo..."); // Prompt for new trial starting
        // Start stimulus timer
        resetTimers(); // Start timers again
    }

    void earlyPress(){
        // Save RT and begin trial again.
        stimulusTimer.Stop(); // Stop timer
        DataManager.Instance.data.earlyPress(stimulusTimer.Elapsed.TotalSeconds); // Save early presses
        score.updateScore(-score.minScore); // Penalise as early trial
        StartCoroutine(DelayBeforeNextTrial());
    }
    
    void endISI(){
        presentText(""); // Hide last trial's feedback
        bone.Show(); // Show bone
        // Stop timing ISI and start reactionTime
        stimulusTimer.Stop();
        reactionTimer.Start();
    }

    void resetTimers(){
        // reset timers
        reactionTimer.Reset();
        stimulusTimer.Reset();
        stimulusTimer.Start();
    }

    void presentText(string text){ // Note to move this to feedback class
        feedbackText.color = Color.black;
        feedbackText.text = text; // Hide last trial's feedback
    }

    void saveRT(){
        reactionTimer.Stop();
        double rt = reactionTimer.Elapsed.TotalSeconds;
        DataManager.Instance.data.currentTrial().saveRT(rt); //consider changing data types ElapsedMilliseconds
        updateScore(rt);
        Debug.Log(DataManager.Instance.data.currentTrial());
    }

    void updateScore(double rt){
        // Update median and maxRT
        double medianRT = calcMedianRT(); // get new median reaction time if possible;
        // Note atm minRT must be > maxRT, see score.calculateScore
        score.maxRT = Math.Min(score.minRT*2, medianRT*2); // Lowerbound on maxRT of minRT
        // Get trial score nd feedback audio
        int trialScore;
        if(rt > medianRT){ // Slow trial
            trialScore = 0; // = 0
        } else { // Fast trial
            trialScore = score.calculateScore(rt);
        }
        // Save and update UI
        score.updateScore(trialScore);
    }

    // ******************* UNITY *******************
    IEnumerator Start(){ //IEnumerator is a hack to enable a delay before running first trial.
        //global vector for showing bone
        // Stores which retry we are on
        DataManager.Instance.data.metadata.retry = DataManager.Instance.data.metadata.retry++;
        // Delay Update 
        bone.Hide();
        enabled = false; // https://docs.unity3d.com/ScriptReference/Behaviour-enabled.html
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        enabled = true; // Allow update to run - stops error on early press during initial delay
        //setup first trial
        newTrial();
    }


    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update(){
        //if in trialISI/ not waiting for reaction
        if(stimulusTimer.IsRunning){ 
            //handle early presses
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                earlyPress();
            } else if(stimulusTimer.Elapsed.TotalSeconds >= trialISI){ // or just access by current trial.trialISI? timer.ElapsedMilliseconds less precise int, Elapsed.TotalSeconds = double, timer.ElapsedTicks most precise
                endISI(); //when timer runs out
            }
        // When waiting for input
        } else if(reactionTimer.IsRunning){
            // On response
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                saveRT(); // Save data
                StartCoroutine(DelayBeforeNextTrial());
            // end trial if greater than max trial time
            } else if(reactionTimer.Elapsed.TotalSeconds > score.maxRT){
                presentText("Too slow!\nAnother dog stole the bone!");
                reactionTimer.Stop();
                StartCoroutine(DelayBeforeNextTrial());
            }
        }
    }
}
