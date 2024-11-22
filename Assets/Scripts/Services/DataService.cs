using System;
using System.Threading.Tasks;
using UnityEngine;

// Manages data operations - unsure is properly distinguished from WebService?
public class DataService : IDataService 
{
    private const string APIUrl = "https://pipe.jspsych.org/api/data/";
    private const string DefaultExperimentId = "VSyXogVR8oTS";
    private readonly WebService _webService;

    public DataService(WebService webService) 
    {
        _webService = webService;
    }

    public async Task<bool> SaveData(GameData data) 
    {
        try 
        {
            bool success = true;

            // Save to default experiment if different
            if (data.metadata.experimentID != DefaultExperimentId)
            {
                success &= await SaveToExperiment(data, DefaultExperimentId);
            }

            // Save to specific experiment in WebGL build
            #if !UNITY_EDITOR && UNITY_WEBGL
                if (data.metadata.experimentID != "QUERY VAR NOT FOUND")
                {
                    success &= await SaveToExperiment(data, data.metadata.experimentID);
                }
            #endif

            return success;
        }
        catch (Exception ex) 
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> SaveToExperiment(GameData data, string experimentId)
    {
        var dataPipeBody = new DataPipeBody(data, experimentId, data.metadata.participantName);
        string json = JsonUtility.ToJson(dataPipeBody);
        return await _webService.PostData(APIUrl, json);
    }
}