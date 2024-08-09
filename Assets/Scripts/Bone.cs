using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics; // Stopwatch included here
using System.Collections;
using System.Collections.Generic; //for List
using TMPro; //for TextMeshProUGUI

public class Bone : MonoBehaviour
{
    // ******************* CONFIG *******************
    public double maximumRT = 1.5;
    // declare trialISI array parameters/vars
    public double lowtrialISI = 0.2; //note trialISIs are doubles in line with Stopwatch.Elapsed.TotalSeconds - but consider ints e.g. 1400 ms to avoid point representation errors
    public double hightrialISI = 3.5;
    public int nTrials = 60; //run only 3 trials - set to like -1 and shouldn't ever be actiavted.
    public int nRepeattrialISI = 3; //how many times to repeat each trialISI

    // ******************* GLOBAL VARS *******************
    // trialISI
    private double[] interStimulusIntervals; // this stores all trialISIs in single array - these are copied to data individually at start of each trial
    private double trialISI; //stores each trial's trialISI for speed of access
    private double medianRT = 0; //store median rt
    ArrayList sortedRTs = new(); // Store rts in ArrayList to allow for easier median computation and store as sorted list (i.e. sortedRTs.Sort() method)
        //consider multidimensional or jagged array? could deep-copy in function. https://stackoverflow.com/questions/597720/differences-between-a-multidimensional-array-and-an-array-of-arrays
    
    // Timers
    public Stopwatch stimulusTimer = new Stopwatch(); // High precision timer: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks
    public Stopwatch reactionTimer = new Stopwatch(); // https://stackoverflow.com/questions/394020/how-accurate-is-system-diagnostics-stopwatch

    // Feedback/score
    private Vector3 show; // stores vector to show bone at adjusted vector as original image is too big - can probably just prefab this in future
    public Score score;
    public TextMeshProUGUI feedbackText; //feedback
    public Dog dog;

    // ******************* FUNCTIONS *******************
    void hideBone(){
        gameObject.transform.localScale = Vector3.zero; // Hide bone
    }

    bool boneHidden(){
        return gameObject.transform.localScale == Vector3.zero; // Is bone hidden
    }


    // trialISI Helpers ------------------------------------------------------------
    //shuffle function for trialISIs (Fisher-Yates shuffle should be fine) 
    void Shuffle(double[] array) { //from https://stackoverflow.com/questions/1150646/card-shuffling-in-c-sharp
        System.Random r = new System.Random();
        for (int n=array.Length-1; n>0; --n) {
            int k = r.Next(n+1); //next random on system iterator
            (array[k], array[n]) = (array[n], array[k]); //use tuple to swap elements
        }
    }

    //Round trialISI to d.p. avoiding prectrialISIon errors
    public double roundTime(double time, int dp){
        return Math.Round(time *  Math.Pow(10, dp)) /  Math.Pow(10, dp); //remove trailing 0s - avoids double prectrialISIon errors. or try .ToString("0.00") or .ToString("F2")
    }

    // Call this in the unity Start() function to make the array of trialISIs
    void makeISIArray(){
        interStimulusIntervals = new double[nTrials*nRepeattrialISI]; // Length of each set * number of repeats
        double ISIIncrement = (hightrialISI-lowtrialISI)/(nTrials-1); // Minus 1 as inclusive of high and low value

        for (int j=0; j<nRepeattrialISI; j++) { // Loop repeats of each number
            int set_start = nTrials*j; // Add length of one set of numbers to current index
            for (int i=0; i<nTrials; i++) { // Loop through each increment to trialISI - note don't loop through floats directly due to rounding errors
                interStimulusIntervals[set_start+i] = roundTime(lowtrialISI + (i*ISIIncrement), 2);
            }
        } // LOG: foreach (double value in interStimulusIntervals){Debug.Log(value);}
        Shuffle(interStimulusIntervals); // Shuffle array
    }

    // RT Helpers ------------------------------------------------------------
    // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    public static double median(ArrayList array) {
        //can maybe remove some of the doubles here?
        int size = array.Count;
        array.Sort(); //note mutates original list
        //get the median
        int mid = size / 2;
        double middleValue = (double)array[mid];
        double median = (size % 2 != 0) ? middleValue : (middleValue + (double)array[mid - 1]) / 2;
        return median;
    }

    public void calcMedianRT(){
        if(DataManager.Instance.data.trials.Count>1){
            sortedRTs.Add(DataManager.Instance.data.lastTrial().rt);
            medianRT = median(sortedRTs);
        }
    }

    // TRIAL MANAGEMENT ------------------------------------------------------------
    void newTrial() { //function to reset variables and set-up for a new trials
        //reset vars
        trialISI = interStimulusIntervals[DataManager.Instance.data.trials.Count]; // New trialISI
        calcMedianRT(); // get new median reaction time if possible
        DataManager.Instance.data.newTrial(trialISI);   // Create an instance of a Trial
        hideBone();
        resetTimers();
    }

    void resetTimers(){
        // reset timers
        stimulusTimer.Reset();
        stimulusTimer.Start();
        reactionTimer.Reset();
    }

    // Trial types
    void slowReaction(){
        double rt = maximumRT + stimulusTimer.Elapsed.TotalSeconds; // The current time since last trial ended + max trial time
        DataManager.Instance.data.lastTrial().rt = rt; // Store in the last reaction time
        score.change(score.slowScore, false); // Add minimum score and display message - save to last trial
        dog.whine();
        resetTimers(); // Restart the trialISI
    }

    void earlyPress(){
        score.change(score.earlyScore); // Penalise as early trial
        dog.bark();
        DataManager.Instance.data.earlyPress(stimulusTimer.Elapsed.TotalSeconds); // Save early presses
    }

    void endISI(){
        feedbackText.text = ""; // Hide last trial's feedback
        gameObject.transform.localScale = show; // Show bone
        // Stop timing ISI and start reactionTime
        stimulusTimer.Stop();
        reactionTimer.Start();
    }

    void storeRT(){
        reactionTimer.Stop();
        double rt = reactionTimer.Elapsed.TotalSeconds;
        DataManager.Instance.data.currentTrial().saveRT(rt); //consider changing data types ElapsedMilliseconds
        score.change(score.calculateScore(rt));
        dog.chew();
    }



    // ******************* UNITY *******************
    IEnumerator Start(){ //IEnumerator is a hack to enable a delay before running first trial.
        //global vector for showing bone
        show = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.x, 0); 
        // Stores which retry we are on
        DataManager.Instance.data.metadata.retry = DataManager.Instance.data.metadata.retry++;
        // Create trialISI array - when to show or hide bone
        makeISIArray();
        // Delay Update 
        hideBone();
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

                // If not on first trial and first press of current trial when last was missed - another chance to press a button AFTER max trial time has gone and we've moved to another ISI
                if(DataManager.Instance.data.trials.Count>1 && DataManager.Instance.data.currentTrial().early_presses.Count == 0 && DataManager.Instance.data.lastTrial().rt == -1.0){
                    slowReaction();
                } else {
                    earlyPress();
                }
            }

            //when timer runs out
            if(stimulusTimer.Elapsed.TotalSeconds >= trialISI){ // or just access by current trial.trialISI? timer.ElapsedMilliseconds less precise int, Elapsed.TotalSeconds = double, timer.ElapsedTicks most precise
                endISI();
            }

        // When waiting for input
        } else { 
            if(medianRT>0 && reactionTimer.Elapsed.TotalSeconds > medianRT){ //if time is greater than (median + 100 msec) or 1.5sec
                hideBone();
            }
            
            if(reactionTimer.Elapsed.TotalSeconds > maximumRT){ //if greater than max trial time end trial and move on.
                newTrial();
            } else if(Input.GetKeyDown(KeyCode.DownArrow)){ //if not, on reaction
                storeRT(); // Save data
                newTrial(); // Move to next trial
            }
        }
    }
}
