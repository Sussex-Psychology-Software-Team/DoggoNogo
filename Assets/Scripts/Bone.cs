using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; //Unity Web Request
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic; //for List
using System.Runtime.InteropServices; //for DllImport
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

    // Score
    public int score = 0; //holds score
    public TextMeshProUGUI scoreText; // displays score
    public TextMeshProUGUI feedbackText; //feedback
    public HealthBar healthBar;
    public int stage = 1;
    public int target_score = 1000;
    public int n_trials_stage1 = 0;


    // ******************* DATA *******************
    //Create json-convertable structures to hold data, each trial stored individually https://forum.unity.com/threads/serialize-nested-objects-with-jsonutility.737624

    // Grab userAgent https://docs.unity3d.com/Manual/web-interacting-code-example.html
    [DllImport("__Internal")] // imports userAgent() from Assets/WebGL/Plugins/userAgent.jslib
    static extern string userAgent();

    // Metadata structure
    [System.Serializable]
    public class Metadata {
        public string id;
        public string name;
        public string userAgent;
        public string start;
        public string end;
        public int retry;

        // Functions for initialising metadata
        //random ID generator
        string randomId(int size) { //https://stackoverflow.com/a/9995960/7705626
            System.Random rand = new System.Random(); 
            string characters = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string rand_id = "";
            for (int i=0; i < size; i++) {
                rand_id += characters[rand.Next(characters.Length)];
            }
            return rand_id;
        }

        public string getUserAgent(){
            #if UNITY_EDITOR
                    return "EDITOR"; // value to return in Play Mode (in the editor)
            #elif UNITY_WEBGL
                    return userAgent(); // value based on the current browser
            #else
                    return "NO_UA";
            #endif
        }

        // Class constructor
        public Metadata(){//string id, string name, string UserA, string start){
            id = randomId(24);
            userAgent = getUserAgent(); // Assign userAgent
            start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Assign date
            //GetInt is not allowed to be called from a MonoBehaviour constructor (or instance field initializer), call it in Awake or Start instead:
                //name = PlayerPrefs.GetString("Name", "No Name");
                //retry = PlayerPrefs.GetInt("Retry", 0); //get retry number
        }
    }

    // Trials model
    [System.Serializable]
    public class Trial {
        public int trial_n;
        public double isi;
        public double rt;
        public string datetime;
        public int score;
        public List<EarlyPress> early_presses;

        public Trial(int trial_number, double isi_var){
            trial_n = trial_number;
            isi = isi_var;
            rt = -1.0; //this indicates no response
            datetime = "";
            score = score;
            early_presses = new List<EarlyPress>();
        }
    }

    // Early presses model (stored within trials)
    [System.Serializable]
    public class EarlyPress {
        //private static int early_counter = 0; //need this to reset when Trial is created?
        public int count;
        public double stopwatch;
        public string datetime;

        public EarlyPress(int press_count, double time){
            count = press_count;
            stopwatch = time;
            datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // Overall data structure which holds the above
    [System.Serializable]
    public class Data {
        public Metadata metadata;
        public List<Trial> trials; //make array of isi_length

        public Data() {
            metadata = new Metadata();
            trials = new List<Trial>();
        }

        // Methods
        public void newTrial(double isi){ //adds a new trial using the isi
            this.trials.Add(new Trial(this.trials.Count+1, isi));
        }

        public Trial currentTrial(){ //returns current trial to add to the current data obj
            int count = this.trials.Count;
            return this.trials[count-1];
        }

        public Trial lastTrial(){ //returns trial before current trial
            int count = this.trials.Count;
            return this.trials[count-2];
        }

        public void earlyPress(double rt){ //add early press based on rt
            int count = this.currentTrial().early_presses.Count+1;
            EarlyPress early_press = new EarlyPress(count, rt);
            this.currentTrial().early_presses.Add(early_press);
        }
    }
    
    //create global instance of data for use throughout
    Data data = new Data();




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
        isi = isi_array[data.trials.Count]; // new isi
        data.newTrial(isi);   // Create an instance of a Trial
        gameObject.transform.localScale = Vector3.zero; // reset stim

        if(score>=target_score){
            target_score += stageTarget(); //note += to increase target amount
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
        Debug.Log(JsonUtility.ToJson(data));

        // Save data
        PlayerPrefs.SetInt("Score", score); //save score to local copy for use in end screen
        PlayerPrefs.Save();
        data.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Save end date of experiment

        // Send datapipe Data
        DataPipeBody body = new DataPipeBody(data); //create instance
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
        
        public DataPipeBody(Data data_obj){
            experimentID = "VSyXogVR8oTS";
            filename = data_obj.metadata.name + "_" + data_obj.metadata.id + ".json";
            data = JsonUtility.ToJson(data_obj);
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
            feedbackText.color = barColour;
        } else if(change == 1){
            barColour = forest;
            feedbackText.color = Color.blue;
        } else {
            barColour = forest;
            feedbackText.color = barColour;
        }

        // Set bounds on change in score to 3rds of health bar
        int newScore = score + change;
        int maxHealth = (int)healthBar.GetMaxHealth();
        if(newScore<0){ 
            newScore = 0; //lowerbound on score of 0
        } else if(score>=maxHealth/3 && newScore<=maxHealth/3){ //if score currently >1/3 healthbar and new score would put it below this
            newScore = maxHealth/3;
        } else if(score>=(maxHealth/3)*2 && newScore<=(maxHealth/3)*2){ //if score currently >2/3 healthbar and new score would put it below this
            newScore = (maxHealth/3)*2;
        }
        
        // Update UI 
        scoreText.text = "Score: " + newScore;
        feedbackText.text = feedback;
        // Update data
        healthBar.SetHealth(newScore, barColour);
        data.currentTrial().score = newScore; //take score out of global var soon
        score = newScore;
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
            text = "GREAT!\nDoggo caught the bone!";
        } else { //if too slow
            final_score = 0;
            text = "Oh no!\nDoggo didn't get a bone.";
        }
        Debug.Log(final_score);

        //finally handle changing the score
        changeScore((int)final_score, text);
    }

    // Calculate score where participant will enter into new stage of game
    public int stageTarget(){
        //n_trials and n_trials_stage1 are defined up top
        int n_trials_stage2 = 0;
        if(stage==1){ n_trials_stage1 = data.trials.Count;
        } else if(stage==2){ n_trials_stage2 = data.trials.Count; }
        stage++;
        int target = ((n_trials - n_trials_stage1 + n_trials_stage2) / (8-(2*stage)) )*100;
        if(target<500){ target=500; }
        return target;
    }




    // ******************* UNITY *******************
    void Start()
    {
        //global vector for showing bone
        float s = gameObject.transform.localScale.x;
        show = new Vector3(s,s,0);

        //PlayerPrefs is an issue inside the class constructor so call here
        data.metadata.name = PlayerPrefs.GetString("Name", "No Name"); // must be done here?
        data.metadata.retry = PlayerPrefs.GetInt("Retry", 0); //get retry number - note probably don't want this persisting on single computer between sessions in future....

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
            if(Input.GetKeyDown("space")){
                // Store piece
                if(data.trials.Count>1 && data.currentTrial().early_presses.Count == 0 && data.lastTrial().rt == -1.0){ //if not on first trial, first press of current trial when last was missed
                    double rt = max_response_time + isi_timer.Elapsed.TotalSeconds; //the current time since last trial ended + max trial time
                    data.lastTrial().rt = rt; //store in the last reaction time
                    changeScore(1, "Too slow! Doggo mad!"); //add minimum score and display message
                    resetTimers(); //restart the isi
                } else {
                    changeScore(-2, "TOO QUICK!\nWait until the bone has appeared."); // minus 2 points for an early press
                    //save early presses
                    data.earlyPress(isi_timer.Elapsed.TotalSeconds);
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
            } else if(Input.GetKeyDown("space")){ //if not, on reaction
                // Store rt
                rt_timer.Stop();
                data.currentTrial().datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                double rt = rt_timer.Elapsed.TotalSeconds; //consider changing data types ElapsedMilliseconds
                data.currentTrial().rt = roundTime(rt,7); // round off to avoid precision errors - 7 is length of ElapsedTicks anyway.
                
                // CALCULATE SCORE ******************************
                calcScore(rt);
                //******************************

                // next trial
                if(data.trials.Count == isi_array.Length-1 || data.trials.Count == n_trials ){
                    endExp();
                } else {
                    newTrial();
                }
            }
        }
    }
}
