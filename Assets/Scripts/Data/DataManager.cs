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

    public void sendData() {
        data.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log(JsonUtility.ToJson(data));

        DataPipeBody body = new DataPipeBody(data);
        string json = JsonUtility.ToJson(body);

        StartCoroutine(dataPipe(json));
        data.ClearTrials();
    }

    IEnumerator dataPipe(string json) {
        using (UnityWebRequest www = UnityWebRequest.Post("https://pipe.jspsych.org/api/data/", json, "application/json")) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
