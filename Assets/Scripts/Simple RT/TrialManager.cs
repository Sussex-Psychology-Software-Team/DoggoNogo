using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Diagnostics; // Stopwatch included here
using System.Collections;
using System.Collections.Generic; //for List
using System.Runtime.InteropServices; // DllImport
using TMPro; //for TextMeshProUGUI


public class TrialManager : MonoBehaviour
{
    public float[] ISIRange = { 1f, 4f }; // Input floats to Random.Range to return a float
    public double medianRT; // Used by other scripts too
    
    // ISI global vars
    double trialISI; // Stores each trial's trialISI for speed of access
    ArrayList sortedRTs = new(); // Store rts in ArrayList to allow for easier median computation and store as sorted list (i.e. sortedRTs.Sort() method)
    // Timer - https://stackoverflow.com/questions/394020/how-accurate-is-system-diagnostics-stopwatch
    Stopwatch timer = new Stopwatch(); // High precision timer: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks

    // Feedback/score
    public ScoreManager scoreManager; // Get score based on RT
    public Bone bone; // Show/hide bone depending on timer
    public Feedback feedback; // For prompting new trial states (and missed trial for now)

    // ******************* FUNCTIONS *******************
    // RT Helpers ------------------------------------------------------------
    // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    double calcMedianRT(double rt) {
        // Add to array and sort
        sortedRTs.Add(rt); // Add to median score list
        sortedRTs.Sort(); // Note mutates original list
        // Get the median
        int size = sortedRTs.Count;
        int mid = size / 2;
        double middleValue = (double)sortedRTs[mid];
        double median = (size % 2 != 0) ? middleValue : (middleValue + (double)sortedRTs[mid - 1]) / 2;
        return median;
    }

    // TRIAL MANAGEMENT ------------------------------------------------------------
    // Coroutine to delay the start of a new trial and show feedback
    IEnumerator DelayBeforeNextTrial(float delay = 1f) {
        yield return new WaitForSeconds(delay); // Wait for 1 second
        newTrial(); // Start new trial after delay
    }

    void newTrial() { //function to reset variables and set-up for a new trials
        bone.Hide();
        // Get new maxRT
        scoreManager.maxRT = Math.Max(scoreManager.minRT*2, medianRT*2); // Lowerbound on maxRT of minRT*2
        // Get new ISI
        trialISI = UnityEngine.Random.Range(ISIRange[0], ISIRange[1]); // New trialISI
        // Create new trial in data structure
        DataManager.Instance.data.newTrial(trialISI); // Create an instance of a Trial
        // Prompt participant that new trial is starting
        feedback.Prompt("Getting a new bone for Doggo..."); // Prompt for new trial starting
        // Start timer
        RestartTimer();
    }

    void EndISI(){
        bone.Show();
        feedback.Prompt("");
    }

    void EndTrial(bool missed = false){ // Don't forget missed trials here
        timer.Stop(); // Immediately stop timer
        if(missed){
            feedback.giveFeedback("missed", scoreManager.totalScore, 0);
        } else {
            // Get score from RT
            double rt = timer.Elapsed.TotalSeconds - trialISI; // subtract ISI from time elapsed during press
            scoreManager.ProcessTrialResult(rt); // Probs don't need score anywhere
            medianRT = calcMedianRT(rt);
        }
        /////// ---------------
        StartCoroutine(DelayBeforeNextTrial());
        // presentText(feedback); // Prompt for new trial starting
    }   


    void RestartTimer(){
        timer.Reset();
        timer.Start();
    }

    // Consider moving this to trial manager
    public void endTask(){ // called from feedback
        // Load next scene
        SceneManager.LoadScene("End");
    }

    // ******************* UNITY *******************
    void Start(){
        medianRT = scoreManager.maxRT/2; // initialise median to half maximum RT
        feedback.Prompt("Get ready to catch the bone by pressing ↓...");
        StartCoroutine(DelayBeforeNextTrial(2f));
        Debug.Log(JsonUtility.ToJson(DataManager.Instance.data.metadata));
        Debug.Log(Utility.GetQueryVariable("osf"));
    }

    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update(){
        if(timer.IsRunning){
            // If ISI ended show bone
            if(timer.Elapsed.TotalSeconds > trialISI && bone.Hidden()){
                EndISI();
            // If > max trial time = missed trial
            } else if(timer.Elapsed.TotalSeconds > (trialISI + scoreManager.maxRT)){
                //Missed Trial
                EndTrial(true);
            }
            // On down arrow (early or valid press)
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                EndTrial();
            }
        }
    }
}
