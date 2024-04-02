using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using Debug = UnityEngine.Debug;
using System.Collections.Generic; //for List
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class bone : MonoBehaviour
{

    // ----- SETUP -----
    // declare ISI array parameters/vars
    public float isi_low = .2f;
    public float isi_high = 3.5f;
    public float isi_step = .1f;
    public int isi_rep = 2; //how many times to repeat each isi
    private int isi_array_length;
    private float[] isi_array;

    // setup stim display vars
    private float s = 0.4145592f; // original image is too big - can probably just prefab this in future
    private int trial_number = 0;
    private float isi;
    public Stopwatch isi_timer = new Stopwatch(); // High precision timer
    public Stopwatch rt_timer = new Stopwatch(); // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks
    Color forest = new Color(0.06770712f, 0.5817609f, 0f, 1f);

    //data   //private float[,] data; //consider jagged array: https://stackoverflow.com/questions/597720/differences-between-a-multidimensional-array-and-an-array-of-arrays
    private double[] rts;
    ArrayList rts_array = new(); //same as rts - not optimal but easy to code
    private double[] early_presses; //stores early button press as nested array
    
    [Serializable] public class Data {
        public string id; //player id
        public int[] early_presses;
        public int[] score; //store score one each trial
        public double[] rts;
        public float[] isi_array;
    }

    Data data = new Data();

    //Scorecard
    public TextMeshProUGUI scoreText; //scorecard
    public TextMeshProUGUI feedbackText; //feedback
    private int score = 0;
    
    //shuffle function for ISIs (Fisher-Yates shuffle should be fine)  https://stackoverflow.com/questions/1150646/card-shuffling-in-c-sharp
    void Shuffle(float[] array) {
        System.Random r = new System.Random();
        for (int n=array.Length-1; n>0; --n) {
            int k = r.Next(n+1); //next random on system iterator
            (array[k], array[n]) = (array[n], array[k]); //use tuple to swap elements
        }
    }


    // Start is called before the first frame update
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
        data.isi_array = isi_array; //store array

        // initialise data arrays //data = new float[isi_array_length,2];
        rts = new double[isi_array_length*isi_rep];
        //setup first trial
        newTrial();
    }



    // Update is called once per frame - maybe use FixedUpdate for inputs?
    void Update()
    {
        if(isi_timer.IsRunning){ //if in isi/ not waiting for reaction
            //handle early presses
            if(Input.GetKeyDown("space")){
                Debug.Log("EARLY SPACEBAR PRESS");
                score -= 2;
                if(score<0){
                    score = 0;
                }
                scoreText.text = "Score: " + score;
                feedbackText.color = Color.red;
                feedbackText.text = "TOO QUICK!\nWait until the bone has appeared.";
                data.early_presses[trial_number] += 1;
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
                rts[trial_number] = rt; //being lazy and using two copies of rt arrays here
                rts_array.Add(rt); //ArrayList version for easier median, could deep-copy in function.
                // median
                double median_rt = MedianArray(rts_array);
                
                //float m = (float)(median_rt-rt==0 ? rt : median_rt-rt); // if no difference then return rt
                //float log_m = m<0 ? Mathf.Log(1+Mathf.Abs(m))*-1 : Mathf.Log(1+m); //cannot take negative log
                //double before_rounding = 1/rt * log_m;
                //int logscore = (int)Math.Round(before_rounding); //final score for this method
                //int mscore = (int)Math.Round(1/rt + (median_rt-rt)*1.5); //simple method
                Debug.Log(median_rt);

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
                data.score[trial_number] = score;
                // END OF TRIAL
                trial_number++;
                newTrial();
            }
        }

        if(trial_number == isi_array.Length){
            data.rts = rts;
            //end exp
            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
        }
    }


    //function to reset variables and set-up for a new trials
    void newTrial() {
        isi = isi_array[trial_number]; // new isi
        data.early_presses[trial_number] = 0; //consider storing this counter in it's own variable and adding to array later
        gameObject.transform.localScale = Vector3.zero; // reset stim

        // reset timers
        rt_timer.Reset();
        isi_timer.Reset();
        isi_timer.Start();
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
