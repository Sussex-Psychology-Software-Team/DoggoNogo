using System; // DateTime
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; // DllImport
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Creates json-convertable structures to hold data, each trial stored individually https://forum.unity.com/threads/serialize-nested-objects-with-jsonutility.737624
    
    // Singleton pattern - ensures only 1 instance of DataManager exists
    public static DataManager Instance; // Static means values stored in class member shared by all the instances of class. 

    // Create global instance of data for use throughout
    public Data data = new Data();

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

        // Constructor
        public Metadata()
        {
            id = GenerateRandomId(24);
            userAgent = GetUserAgent();
            start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // Generates a random ID
        private static string GenerateRandomId(int size)
        {
            var rand = new System.Random();
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randId = new char[size];
            for (var i = 0; i < size; i++)
            {
                randId[i] = characters[rand.Next(characters.Length)];
            }
            return new string(randId);
        }

        // Retrieves the user agent
        private static string GetUserAgent()
        {
            #if UNITY_EDITOR
                return "EDITOR"; // Value to return in Play Mode (in the editor)
            #elif UNITY_WEBGL
                return userAgent(); // Value based on the current browser
            #else
                return "NO_UA";
            #endif
        }
    }
    

    [System.Serializable]
    public class Trial {
        public int trial_n;
        public double isi;
        public double rt;
        public string datetime;
        public int score;
        public List<EarlyPress> early_presses;

        // Constructor
        public Trial(int trialNumber, double isiVar)
        {
            trial_n = trialNumber;
            isi = isiVar;
            rt = -1.0; // Indicates no response
            datetime = "";
            score = 0; // Initialize score
            early_presses = new List<EarlyPress>();
        }
    }

    // Early presses model (stored within trials)
    [System.Serializable]
    public class EarlyPress {
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
