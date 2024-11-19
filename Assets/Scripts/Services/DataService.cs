using System;
using System.Threading.Tasks;
using UnityEngine;

// Manages data operations - unsure is properly distinguished from WebService?
public class DataService : IDataService {
    private readonly string _apiUrl = "https://pipe.jspsych.org/api/data/";
    private readonly WebService _webService;

    public DataService(WebService webService) {
        this._webService = webService;
    }

    public async Task<bool> SaveData(GameData data) {
        try {
            var dataPipeBody = new DataPipeBody(data, 
                data.metadata.experimentID, 
                data.metadata.participantName);
            
            string json = JsonUtility.ToJson(dataPipeBody);
            return await _webService.PostData(_apiUrl, json);
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to save data: {ex.Message}");
            return false;
        }
    }
}