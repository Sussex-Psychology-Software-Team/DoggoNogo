using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; //Unity Web Request
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

    // Score
    public int score = 0; //holds score
    public TextMeshProUGUI scoreText; // displays score
    public TextMeshProUGUI feedbackText; //feedback
    public HealthBar healthBar; //holds reference to coloured health bar
    public int stage = 1; //stages 1-3
    public int[] score_ratchet = {0,1000,0,0}; //tracks the max score for each stage
    public int n_trials_stage1 = 0; //number of trials to reach end of stage 1


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

        if(score>=score_ratchet[stage]){
            scoreTarget(); //note += to increase target amount
            changeScore(0, "Level " + stage + "!");
        }
        resetTimers();
    }

    void resetTimers(){
        // reset timers
        isi_timer.Reset();
        isi_timer.Start();
        rt_timer.Reset();
    }

    void endExp(){
        Debug.Log(JsonUtility.ToJson(DataManager.Instance.data));

        // Save data
        PlayerPrefs.SetInt("Score", score); //save score to local copy for use in end screen
        PlayerPrefs.Save();
        DataManager.Instance.data.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Save end date of experiment

        // Send datapipe Data
        DataPipeBody body = new DataPipeBody(); //create instance
        string json = JsonUtility.ToJson(body); //Note double encoding is necessary here as looks like datapipe parses this as an object on their end too
        StartCoroutine(sendData(json));

        // Load next scene
        SceneManager.LoadScene("End");
    }


    // SENDING DATA -------------------------------------
    [System.Serializable] //class to format the data as expected by datapipe
    public class DataPipeBody{
        public string experimentID;
        public string filename;
        public string data; //json string of data object
        
        public DataPipeBody(){
            experimentID = "VSyXogVR8oTS";
            filename = DataManager.Instance.data.metadata.name + "_" + DataManager.Instance.data.metadata.id + ".json";
            data = JsonUtility.ToJson(DataManager.Instance.data);
        }
    }

    IEnumerator sendData(string json){ //sends data - IEnumerator can run over sever frames and wait for 'OK' response from OSF server
        using (UnityWebRequest www = UnityWebRequest.Post("https://pipe.jspsych.org/api/data/", json, "application/json")) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            }
            else {
                Debug.Log("Form upload complete!");
            }
        }
    }


    // SCORE -------------------------------------
    void changeScore(int change, string feedback){
        // set colours
        Color forest = new Color(0.06770712f, 0.5817609f, 0f, 1f); //colour of positive feedback text
        Color barColour;
        if(change<0){ //if score is being reduced
            barColour = Color.red;
            feedbackText.color = Color.red;
        } else if(change == 1){
            barColour = forest;
            feedbackText.color = Color.white;
        } else {
            barColour = forest;
            feedbackText.color = Color.white;
        }

        // Update healthbar but comparing score to ratchet
        float maxHealth = healthBar.GetMaxHealth();
        float healthBarScore = (float)(score + change);
        
        //ratchet healthbar separately
        float health_bar_ratchet = maxHealth*(((float)stage-1f)/3f);
        if(healthBarScore < health_bar_ratchet){ 
            healthBarScore = health_bar_ratchet;
        }
        healthBar.SetHealth((int)healthBarScore, barColour);

        //compare new score to ratchet
        int newScore = score + change;
        if(newScore < score_ratchet[stage-1]){
            newScore = score_ratchet[stage-1];
        }
        // Update data
        DataManager.Instance.data.currentTrial().score = newScore; //take score out of global var soon
        score = newScore;

        // Update UI 
        scoreText.text = "Score: " + newScore;
        feedbackText.text = feedback;
    }

    // PUT NEW SCORE CALCULATIONS IN HERE
    void calcScore(double rt, double min_rt=.2, double max_rt=.6, double min_score=.1, double max_score = .2) {
        // Calculate score
        double final_score;
        string text;
        if(rt<max_rt){ // if not way too slow
            // calculate realtive score
            double clamped = Math.Clamp(rt, min_rt, max_rt); //clamp rt between ranges
            double relative = (clamped - min_rt) / (max_rt - min_rt); //normalise as proportion of range
            double reversed = (1 - relative); //reverse
            // score bonus
            double max_score_bonus = max_score - min_score;
            double score_bonus = reversed * max_score_bonus;
            double min_add = min_score + score_bonus;
            if (min_add > max_score) { min_add = max_score; } //clamp
            final_score = min_add*1000;
            text = "GREAT!\nDoggo caught the sausage!";
        } else { //if too slow
            final_score = 0;
            text = "Oh no!\nDoggo didn't get a sausage.";
        }
        Debug.Log(final_score);

        //finally handle changing the score
        changeScore((int)final_score, text);
    }

    // Calculate score where participant will enter into new stage of game
    void scoreTarget(){
        //n_trials and n_trials_stage1 are defined up top
        int n_trials_stage2 = 0;
        if(stage==1){ n_trials_stage1 = DataManager.Instance.data.trials.Count;
        } else if(stage==2){ n_trials_stage2 = DataManager.Instance.data.trials.Count; }
        stage++;
        if(stage==4){
            endExp();
            return;
        }
        dog.ChangeSprite();
        int target = ((n_trials - (n_trials_stage1 + n_trials_stage2)) / (8-(2*stage)) )*100;
        if(target<500){ target=500; }
        score_ratchet[stage] = score_ratchet[stage-1]+target;
        float new_health = (float)score_ratchet[stage] * (1f/ (float)stage )*3f;
        healthBar.SetMaxHealth((int)new_health); //set healthbar maximum
        Debug.Log("New Max health: ");
        Debug.Log((1/stage)*3);
        Debug.Log(score_ratchet[stage]*(1/stage)*3);
    }

    // ******************* UNITY *******************
    void Start()
    {
        //global vector for showing bone
        float s = gameObject.transform.localScale.x;
        show = new Vector3(s,s,0);

        //PlayerPrefs is an issue inside the class constructor so call here
        DataManager.Instance.data.metadata.name = PlayerPrefs.GetString("Name", "No Name"); // must be done here?
        DataManager.Instance.data.metadata.retry = PlayerPrefs.GetInt("Retry", 0); //get retry number - note probably don't want this persisting on single computer between sessions in future....

        //Create ISI array
        makeISIArray();

        //setup first trial
        newTrial();
        healthBar.SetMaxHealth(score_ratchet[stage]*3);
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
                    changeScore(0, "Too slow! Doggo mad!"); //add minimum score and display message
                    resetTimers(); //restart the isi
                } else {
                    changeScore(-100, "TOO QUICK!\nWait until the sausage has appeared."); // minus 2 points for an early press
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
                DataManager.Instance.data.currentTrial().datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                double rt = rt_timer.Elapsed.TotalSeconds; //consider changing data types ElapsedMilliseconds
                DataManager.Instance.data.currentTrial().rt = roundTime(rt,7); // round off to avoid precision errors - 7 is length of ElapsedTicks anyway.
                
                // CALCULATE SCORE ******************************
                calcScore(rt);
                //******************************

                // next trial
                if(DataManager.Instance.data.trials.Count == isi_array.Length-1 || DataManager.Instance.data.trials.Count == n_trials ){
                    endExp();
                } else {
                    newTrial();
                }
            }
        }
    }
}
