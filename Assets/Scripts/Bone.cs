using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic; //for List
using TMPro; //for TextMeshProUGUI

public class Bone : MonoBehaviour
{
    // ******************* CONFIG *******************
    public double max_response_time = 1.5;
    // declare ISI array parameters/vars
    public double isi_low = 0.2; //note ISIs are doubles in line with Stopwatch.Elapsed.TotalSeconds - but consider ints e.g. 1400 ms to avoid point representation errors
    public double isi_high = 3.5;
    public int n_trials = 60; //run only 3 trials - set to like -1 and shouldn't ever be actiavted.
    public int isi_rep = 3; //how many times to repeat each isi

    // ******************* GLOBAL VARS *******************
    // isi
    private double[] isi_array; // this stores all isis in single array - these are copied to data individually at start of each trial
    private double isi; //stores each trial's isi for speed of access
    private double median_rt = 0; //store median rt
    ArrayList rts_array = new(); // Store rts in ArrayList to allow for easier median computation and store as sorted list (i.e. rts_array.Sort() method)
        //consider multidimensional or jagged array? could deep-copy in function. https://stackoverflow.com/questions/597720/differences-between-a-multidimensional-array-and-an-array-of-arrays
    
    //timers
    public Stopwatch isi_timer = new Stopwatch(); // High precision timer: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks
    public Stopwatch rt_timer = new Stopwatch(); // https://stackoverflow.com/questions/394020/how-accurate-is-system-diagnostics-stopwatch

    // Display
    private Vector3 show; // or 0.4145592f original image is too big - can probably just prefab this in future
    public Dog dog;
    public Score score;
    public TextMeshProUGUI scoreText; // displays score
    public TextMeshProUGUI feedbackText; //feedback

    // ******************* FUNCTIONS *******************
    // ISI Helpers ------------------------------------------------------------
    //shuffle function for ISIs (Fisher-Yates shuffle should be fine) 
    void Shuffle(double[] array) { //from https://stackoverflow.com/questions/1150646/card-shuffling-in-c-sharp
        System.Random r = new System.Random();
        for (int n=array.Length-1; n>0; --n) {
            int k = r.Next(n+1); //next random on system iterator
            (array[k], array[n]) = (array[n], array[k]); //use tuple to swap elements
        }
    }

    //call this in the unity Start() function to make the array of ISIs
    void makeISIArray(){
        isi_array = new double[n_trials*isi_rep]; //length of each set * number of repeats
        double isi_step = (isi_high-isi_low)/(n_trials-1); //minus 1 as inclusive of high and low value

        for (int j=0; j<isi_rep; j++) { //loop repeats of each number
            int set_start = n_trials*j; //add length of one set of numbers to current index
            for (int i=0; i<n_trials; i++) { //loop through each increment to isi - note don't loop through floats directly due to rounding errors
                isi_array[set_start+i] = roundTime(isi_low + (i*isi_step), 2);
            }
        } // LOG: foreach (double value in isi_array){Debug.Log(value);}  
        Shuffle(isi_array); //shuffle array
    }

    // RT Helpers ------------------------------------------------------------
    // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    public static double median(ArrayList array) { 
        //can maybe remove some of the doubles here?
        int size = array.Count;
        array.Sort(); //note mutates original list
        //get the median
        int mid = size / 2;
        double mid_value = (double)array[mid];
        double median = (size % 2 != 0) ? mid_value : (mid_value + (double)array[mid - 1]) / 2;
        return median;
    }

    //Round ISI to d.p. avoiding precision errors
    public double roundTime(double time, int dp){
        return Math.Round(time *  Math.Pow(10, dp)) /  Math.Pow(10, dp); //remove trailing 0s - avoids double precision errors. or try .ToString("0.00") or .ToString("F2")
    }


    // TRIAL MANAGEMENT ------------------------------------------------------------
    void newTrial() { //function to reset variables and set-up for a new trials
        //reset vars
        isi = isi_array[DataManager.Instance.data.trials.Count]; // new isi
        DataManager.Instance.data.newTrial(isi);   // Create an instance of a Trial
        gameObject.transform.localScale = Vector3.zero; // reset stim
        resetTimers();
    }

    void resetTimers(){
        // reset timers
        isi_timer.Reset();
        isi_timer.Start();
        rt_timer.Reset();
    }

    void endTask(){
        // Load next scene
        SceneManager.LoadScene("End");
    }

    // ******************* UNITY *******************
    void Start()
    {
        //global vector for showing bone
        float s = gameObject.transform.localScale.x;
        show = new Vector3(s,s,0);

        DataManager.Instance.data.metadata.retry = DataManager.Instance.data.metadata.retry++; //get retry number - note probably don't want this persisting on single computer between sessions in future....

        //Create ISI array
        makeISIArray();

        //setup first trial
        newTrial();
    }


    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update()
    {
       if(isi_timer.IsRunning){ //if in isi/ not waiting for reaction
            //handle early presses
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                // Store piece
                if(DataManager.Instance.data.trials.Count>1 && DataManager.Instance.data.currentTrial().early_presses.Count == 0 && DataManager.Instance.data.lastTrial().rt == -1.0){ //if not on first trial, first press of current trial when last was missed
                    double rt = max_response_time + isi_timer.Elapsed.TotalSeconds; //the current time since last trial ended + max trial time
                    DataManager.Instance.data.lastTrial().rt = rt; //store in the last reaction time
                    score.set(0); //add minimum score and display message
                    resetTimers(); //restart the isi
                } else {
                    score.set(-100); // minus 2 points for an early press
                    //save early presses
                    DataManager.Instance.data.earlyPress(isi_timer.Elapsed.TotalSeconds);
                }
            }

            //when timer runs out
            if(isi_timer.Elapsed.TotalSeconds >= isi){ // or just access by current trial.isi? timer.ElapsedMilliseconds less precise int, Elapsed.TotalSeconds = double, timer.ElapsedTicks most precise
                feedbackText.text = ""; //hide feedback
                gameObject.transform.localScale = show; //show bone - make z 0?
                //timers
                isi_timer.Stop();
                rt_timer.Start();
            }

        } else { //when waiting for input
            if((median_rt>0 && rt_timer.Elapsed.TotalSeconds>(median_rt+.1))){ //if time is greater than (median + 100 msec) or 1.5sec hide the bone
                gameObject.transform.localScale = Vector3.zero; //hide bone
            }
            
            if(rt_timer.Elapsed.TotalSeconds > max_response_time){ //if greater than max trial time end trial and move on.
                newTrial();
            } else if(Input.GetKeyDown(KeyCode.DownArrow)){ //if not, on reaction
                // Store rt
                rt_timer.Stop();
                double rt = rt_timer.Elapsed.TotalSeconds;
                DataManager.Instance.data.currentTrial().saveRT(rt); //consider changing data types ElapsedMilliseconds
                score.set(score.calculateScore(rt));
                // next trial
                if(DataManager.Instance.data.trials.Count == isi_array.Length-1 || DataManager.Instance.data.trials.Count == n_trials){
                    endTask();
                } else {
                    newTrial();
                }
            }
        }
    }
}
