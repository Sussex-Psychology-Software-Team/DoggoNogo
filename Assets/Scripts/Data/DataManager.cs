using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour {
    // Singleton instance - should be NonSerialized since it's runtime-only
    [System.NonSerialized]
    public static DataManager Instance;

    [System.NonSerialized]
    private Data data = new();

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (data == null){
            data = new Data();
        }
    }

    public int GetNTrialsFromQuery()
    {
        int defaultN = 60;
        if(int.TryParse(data.metadata.l1n, out int l1n)){ //inline declaration
            return l1n;
        } else {
            return defaultN;
        }
    }

    public void NewTrial(double isi)
    {
        data.AddNewTrial(isi);
    }
    
    public void Level1Started() {
        data.ClearTrials();
        data.metadata.startL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void StimuliShown(Dictionary<string, float> stimSpec)
    {
        data.CurrentTrial().SaveStimulus(stimSpec);
    }
    
    public void SaveTrial(double rt, string type, int score, int total, double threshold, bool validTrial, int validTrialCount) {
        data.CurrentTrial().SaveTrial(rt, type, score, total, threshold, validTrial, validTrialCount);
        Debug.Log(JsonUtility.ToJson(this));
    }

    public void Level1Ended()
    {
        data.metadata.endL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void SendData() {
        // Add end date
        data.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // Get id for current experimenters and send
        #if !UNITY_EDITOR && UNITY_WEBGL
            if(data.metadata.experimentID != "QUERY VAR NOT FOUND") SendDataToServer(data.metadata.experimentID, data.metadata.participantName);
        #endif
        // Send to our own location
        if(data.metadata.experimentID != "VSyXogVR8oTS") SendDataToServer("VSyXogVR8oTS", data.metadata.participantName);
    }

    // Abstract the data sending logic
    void SendDataToServer(string experimentID, string participantName) {
        DataPipeBody studyData = new(data, experimentID, participantName);
        string studyJSON = JsonUtility.ToJson(studyData);
        StartCoroutine(DataPipe(studyJSON));
    }

    IEnumerator DataPipe(string json) {
        using (UnityWebRequest www = UnityWebRequest.Post("https://pipe.jspsych.org/api/data/", json, "application/json")) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                Debug.Log("Form upload complete!");
            }
        }
    }
    
    void Start(){
        data.metadata.InitializeWebVariables(); // Needs to be done here to get UNITY_WEBGL to pass
    }

    public double GetCurrentThreshold()
    {
        double threshold = (data.level1.Count > 0) ? data.CurrentTrial().threshold : Calculations.normMean;
        return threshold;
    }
}
