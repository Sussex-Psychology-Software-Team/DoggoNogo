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
    // https://stackoverflow.com/questions/394020/how-accurate-is-system-diagnostics-stopwatch
    public Stopwatch timer = new Stopwatch(); // High precision timer: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks

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
        // Start timer
        resetTimer();
    }

    void saveTrial(string trialType, string feedback){
        timer.Stop(); // Immediately stop timer
        bone.Hide(); // Hide bone - use an animation here
        double rt = timer.Elapsed.TotalSeconds;
        DataManager.Instance.data.currentTrial().saveTrial(rt, trialType); //consider changing data types ElapsedMilliseconds
        updateScore(rt);
        presentText(feedback); // Prompt for new trial starting
        StartCoroutine(DelayBeforeNextTrial());
    }

    void earlyPress(){
        // Save RT and begin trial again.
        timer.Reset();
        score.updateScore(-score.minScore); // Penalise as early trial
        StartCoroutine(DelayBeforeNextTrial());
    }
    
    void endISI(){
        presentText(""); // Hide last trial's feedback
        bone.Show(); // Show bone
    }

    void resetTimer(){
        timer.Reset();
        timer.Start();
    }

    void presentText(string text){ // Note to move this to feedback class
        feedbackText.color = Color.black;
        feedbackText.text = text; // Hide last trial's feedback
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
        // Delay Update 
        enabled = false; // https://docs.unity3d.com/ScriptReference/Behaviour-enabled.html
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        enabled = true; // Allow update to run - stops error on early press during initial delay
        //setup first trial
        newTrial();
    }


    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update(){
        // During ISI
        if(timer.Elapsed.TotalSeconds <= trialISI){
            //handle early presses
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                saveTrial("early", "Too quick! Wait until the bone has been thrown");
            }
        // After max trial time
        } else if(timer.Elapsed.TotalSeconds > score.maxRT){
            saveTrial("missed", "Too slow Doggo!\nAnother dog fetched the bone first!");
        // Response during stim presentation
        } else if(Input.GetKeyDown(KeyCode.DownArrow)){
            saveTrial("valid", "");
        }

    }
}
