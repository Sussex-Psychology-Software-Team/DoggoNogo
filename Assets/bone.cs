using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using Debug = UnityEngine.Debug;
using System.Collections.Generic; //for List
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#endif

public class bone : MonoBehaviour
{

    // ******************* SETUP *******************

    // Inter-stimulus Intervals --------------------------------
    // declare ISI array parameters/vars
    public float isi_low = .2f;
    public float isi_high = 3.5f;
    public float isi_step = .1f;
    public int isi_rep = 2; //how many times to repeat each isi
    private int isi_array_length;
    private float[] isi_array; // this stores all isis in single array - these are copied to data individually at start of each trial
    
    //shuffle function for ISIs (Fisher-Yates shuffle should be fine)  https://stackoverflow.com/questions/1150646/card-shuffling-in-c-sharp
    void Shuffle(float[] array) {
        System.Random r = new System.Random();
        for (int n=array.Length-1; n>0; --n) {
            int k = r.Next(n+1); //next random on system iterator
            (array[k], array[n]) = (array[n], array[k]); //use tuple to swap elements
        }
    }

    //timers
    public Stopwatch isi_timer = new Stopwatch(); // High precision timer
    public Stopwatch rt_timer = new Stopwatch(); // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks


    // Visuals --------------------------------
    // setup stim display vars
    private float s = 0.4145592f; // original image is too big - can probably just prefab this in future
    Color forest = new Color(0.06770712f, 0.5817609f, 0f, 1f); //colour of positive feedback text

    // Scorecard
    private int score = 0; //holds score
    public TextMeshProUGUI scoreText; // displays score
    public TextMeshProUGUI feedbackText; //feedback


    // Data --------------------------------
    // trial-level data (globals)
    private int trial_number = 0; //tracks trial number
    private int early_presses = 0; // counts early button presses
    private float isi; //stores each trial's isi

    //consider multidimensional or jagged array? https://stackoverflow.com/questions/597720/differences-between-a-multidimensional-array-and-an-array-of-arrays
    ArrayList rts_array = new(); // Store rts in ArrayList to allow for easier median computation
    
    //Create json-convertable struct to hold data, each trial stored individually https://forum.unity.com/threads/serialize-nested-objects-with-jsonutility.737624/
    [System.Serializable]
    public class Data {
        public List<Metadata> metadata;
        public List<Trial> trials;

        public Data() {
            metadata = new List<Metadata>();
            trials = new List<Trial>();
        }
    }

    [System.Serializable]
    public class Metadata {
        public string id;
        public string userAgent;
        public string date;
    }

    [System.Serializable]
    public class Trial {
        public int trial_n;
        public float isi;
        public double rt;
        public int score;
        public int early_presses;
    }

    Data data = new Data(); //create instance


    // Metadata Functions --------------------------------
    // Grab userAgent
    public class UA : MonoBehaviour { //https://stackoverflow.com/questions/72083612/detect-mobile-client-in-webgl
        [DllImport("__Internal")] // imports userAgent() from Assets/WebGL/Plugins/userAgent.jslib
        static extern string userAgent();
        public static string getUserAgent(){
            #if UNITY_EDITOR
                    return "EDITOR"; // value to return in Play Mode (in the editor)
            #elif UNITY_WEBGL
                    return userAgent(); // value based on the current browser
            #else
                    return "NO_UA";
            #endif
        }
    }

    //random ID generator
    System.Random rand = new System.Random(); 
    public const string characters = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public string GenerateString(int size) { //https://stackoverflow.com/a/9995960/7705626
        char[] chars = new char[size];
        for (int i=0; i < size; i++) {
            chars[i] = characters[rand.Next(characters.Length)];
        }
        return new string(chars);
    }

    // Start --------------------------------
    void Start()
    {
        // Create isi array
        isi_array_length = Mathf.CeilToInt((isi_high-isi_low)/isi_step +1); //round up for floating point errors
        isi_array = new float[isi_array_length*isi_rep]; //length of each set * number of repeats
        for (int j=0; j<isi_rep; j++) { //loop repeats of each number
            int set = isi_array_length*j; //add length of one set of numbers to current index
            for (int i=0; i<isi_array_length; i++) { //loop through each increment to isi
                isi_array[set+i] = isi_low + i * isi_step;
            }
        } // LOG: foreach (float value in isi_array){Debug.Log(value);}  
        Shuffle(isi_array); //shuffle array

        // Create metadata
        Metadata metadata = new Metadata(); // Create an instance of Metadata
        metadata.id = GenerateString(24); // Assign id
        metadata.userAgent = UA.getUserAgent(); // Assign userAgent
        metadata.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Assign date
        data.metadata.Add(metadata); // Add the metadata object to the list
        
        // initialise data arrays 
        //data = new float[isi_array_length,2];
        //rts = new double[isi_array_length*isi_rep];
        //setup first trial
        newTrial();
    }



    // ******************* TRIAL RUNNER *******************

    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update()
    {
        if(isi_timer.IsRunning){ //if in isi/ not waiting for reaction
            //handle early presses
            if(Input.GetKeyDown("space")){
                Debug.Log("EARLY SPACEBAR PRESS");
                score -= 2; // minus 2 points for an early press
                if(score<0){ 
                    score = 0; //lowerbound on score of 0
                }
                scoreText.text = "Score: " + score;
                feedbackText.color = Color.red;
                feedbackText.text = "TOO QUICK!\nWait until the bone has appeared.";
                early_presses++;
            }

            //when timer runs out
            if(isi_timer.Elapsed.TotalSeconds >= isi){ // Note timer.ElapsedMilliseconds less precise e.e. 1400, timer.ElapsedTicks is more precise
                feedbackText.text = ""; //hide feedback
                gameObject.transform.localScale = new Vector3(s,s,s); //show bone
                //timers
                isi_timer.Stop();
                rt_timer.Start();
            }

        } else { //when waiting for input
            if(Input.GetKey("space")){
                
                //get data
                rt_timer.Stop();
                double rt = rt_timer.Elapsed.TotalSeconds; //consider changing data types
                //rts[trial_number] = rt; //being lazy and using two copies of rt arrays here
                rts_array.Add(rt); //ArrayList version for easier median, could deep-copy in function.
                // median
                double median_rt = MedianArray(rts_array);
                
                // CALCULATE SCORE ******************************

                //float m = (float)(median_rt-rt==0 ? rt : median_rt-rt); // if no difference then return rt
                //float log_m = m<0 ? Mathf.Log(1+Mathf.Abs(m))*-1 : Mathf.Log(1+m); //cannot take negative log
                //double before_rounding = 1/rt * log_m;
                //int logscore = (int)Math.Round(before_rounding); //final score for this method
                //int mscore = (int)Math.Round(1/rt + (median_rt-rt)*1.5); //simple method
                Debug.Log(median_rt);

                //******************************

                if(rt<(median_rt+.1)){ //if within 100ms of median
                    score += 3;
                    feedbackText.color = forest;
                    feedbackText.text = "YUMMY!\nDoggo caught the bone!";       
                } else {
                    score += 1;
                    feedbackText.color = Color.blue;
                    feedbackText.text = "Good!\nDoggo fetched the bone.";
                }
                scoreText.text = "Score: " + score;
                //store data
                //data.score[trial_number] = score;
                // END OF TRIAL
                trial_number++;
                saveData(isi,rt,score,early_presses);
                newTrial();
            }
        }

        if(trial_number == isi_array.Length){
            //data.rts = rts;
            //end exp
            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
        }
    }

    // HELPERS --------------------------------
    //function to reset variables and set-up for a new trials
    void newTrial() {
        isi = isi_array[trial_number]; // new isi
        early_presses = 0; //consider storing this counter in it's own variable and adding to array later
        gameObject.transform.localScale = Vector3.zero; // reset stim

        // reset timers
        rt_timer.Reset();
        isi_timer.Reset();
        isi_timer.Start();
    }

    void saveData(float isi, double rt, int score, int early_presses){
        Trial trial_data = new Trial(); // Create an instance of a Trial
        trial_data.trial_n = trial_number;
        trial_data.isi = Mathf.Round(isi *  Mathf.Pow(10f, 1)) /  Mathf.Pow(10f, 1); //rounds isi to decimals to avoid floating point
        trial_data.rt = rt;
        trial_data.score = score;
        trial_data.early_presses = early_presses;
        data.trials.Add(trial_data); // Add the metadata object to the list

        string json = JsonUtility.ToJson(data);
        Debug.Log(json);
    }

    // slow but simple median function - quicker algorithms here: https://stackoverflow.com/questions/4140719/calculate-median-in-c-sharp
    public static double MedianArray(ArrayList array) {
        int size = array.Count;
        array.Sort(); //note mutates original list
        //get the median
        int mid = size / 2;
        double mid_value = (double)array[mid];
        double median = (size % 2 != 0) ? mid_value : (mid_value + (double)array[mid - 1]) / 2;
        return median;
    }

}
