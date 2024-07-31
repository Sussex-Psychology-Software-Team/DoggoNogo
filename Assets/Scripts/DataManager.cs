using System; // DateTime
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices; // DllImport

public class DataManager : MonoBehaviour
{
    //Create json-convertable structures to hold data, each trial stored individually https://forum.unity.com/threads/serialize-nested-objects-with-jsonutility.737624
    // Singleton pattern - ensures only 1 instance of DataManager exists

    public static DataManager Instance; // Static means values stored in class member shared by all the instances of class. 

    private void Awake()
    {
        // Only create new instance if doesn't exist
        if (Instance != null) {
            Destroy(gameObject); // gameObject carried forward from editor
            return;
        }

        Instance = this; // Store the current instance so other calls link to same.
        DontDestroyOnLoad(gameObject); // Don't destroy when scene changes
    }


    //create global instance of data for use throughout
    public Data data = new Data();

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

}
