using System; // DateTime
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices; // DllImport
using UnityEngine;
using UnityEngine.Networking; //Unity Web Request

public class DataManager : MonoBehaviour
{
    // Creates json-convertable structures to hold data, each trial stored individually https://forum.unity.com/threads/serialize-nested-objects-with-jsonutility.737624
    
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

    // Create global instance of data for use throughout
    public Data data = new Data();


    // Superclass data object which holds the classes below
    [System.Serializable]
    public class Data {
        public Metadata metadata;
        public List<Trial> trials; //make array of isi_length

        public Data() {
            metadata = new Metadata();
            trials = new List<Trial>();
        }

        // Methods - creating/finding trials
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

        public void ClearTrials() { // Clear trials on restart
            this.trials.Clear();
        }
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
            retry = 0;
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
    
    // Each trial
    [System.Serializable]
    public class Trial {
        public int trial_n;
        public double isi;
        public double rt;
        public string datetime;
        public int trialScore;
        public int totalScore;
        public string trialType;

        // Constructor
        public Trial(int trialNumber, double isiVar)
        {
            trial_n = trialNumber;
            isi = isiVar;
            rt = -1.0; // Indicates no response
            datetime = "";
            trialScore = 0; // Initialize score
            totalScore = 0;
        }

        private double roundTime(double time, int dp){
            return Math.Round(time *  Math.Pow(10, dp)) /  Math.Pow(10, dp); //remove trailing 0s - avoids double precision errors. or try .ToString("0.00") or .ToString("F2")
        }

        public void saveTrial(double rt, string type){
            this.rt = roundTime(rt,7); // round off to avoid precision errors - 7 is length of ElapsedTicks anyway.
            this.datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.trialType = type;
        }
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

    public void sendData(){
        // Save data
        this.data.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Save end date of experiment
        // Log data
        Debug.Log(JsonUtility.ToJson(this.data));

        // Send data with DataPipe
        DataPipeBody body = new DataPipeBody(); //create instance
        string json = JsonUtility.ToJson(body); //Note double encoding is necessary here as looks like datapipe parses this as an object on their end too
        StartCoroutine(dataPipe(json));

        // Clear trials after sending data
        this.data.ClearTrials();
    }

    public IEnumerator dataPipe(string json){ //sends data - IEnumerator can run over sever frames and wait for 'OK' response from OSF server
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

}
