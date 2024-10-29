using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour {
    public static DataManager Instance;

    public Data data = new Data();

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
}
