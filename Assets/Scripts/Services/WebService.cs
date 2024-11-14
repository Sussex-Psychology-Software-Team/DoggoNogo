using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebService {
    public async Task<bool> PostData(string url, string jsonData) {
        using var www = UnityWebRequest.Post(url, jsonData, "application/json");
        try {
            await www.SendWebRequest();
            return www.result == UnityWebRequest.Result.Success;
        }
        catch (Exception ex) {
            Debug.LogError($"Web request failed: {ex.Message}");
            return false;
        }
    }
}