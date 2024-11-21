using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

// Handles data operations: Save trial data, Manages web communication, Tracks game progress
    // Seems to contain the data object too? or just a reference to it?
public class DataController : MonoBehaviour
{
    public static DataController Instance { get; private set; }
    public GameData GameData { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;

    private IDataService _dataService;
    private WebService _webService;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialise
        GameData = new GameData();
        GameData.metadata.InitializeWebVariables();
        InitializeServices();
    }

    private void InitializeServices()
    {
        _webService = new WebService();
        _dataService = new DataService(_webService);
    }

    public int GetNTrialsFromQuery()
    {
        int defaultN = gameConfig.DefaultTrialCount;
        return int.TryParse((ReadOnlySpan<char>)GameData.metadata.l1n, out int l1N) ? l1N : defaultN;
    }

    public void NewTrial(double isi)
    {
        GameData.AddNewTrial(isi);
    }

    public void Level1Started()
    {
        GameData.ClearTrials();
        GameData.metadata.startL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.Level1);
    }

    public void StimuliShown(Dictionary<string, float> stimSpec)
    {
        GameData.CurrentTrial()?.SaveStimulus(stimSpec);
    }

    public void SaveTrial(TrialResult result)
    {
        var currentTrial = GameData.CurrentTrial();
        currentTrial?.SaveTrial(result);
    }

    public void Level1Ended()
    {
        GameData.metadata.endL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
    }

    public double GetCurrentThreshold()
    {
        return (GameData.level1.Count > 0) ? GameData.CurrentTrial().threshold : gameConfig.InitialMedianRT;
    }

    public async Task SendData()
    {
        GameData.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        #if !UNITY_EDITOR && UNITY_WEBGL
            if (gameData.metadata.experimentID != "QUERY VAR NOT FOUND")
            {
                await SendDataToServer(gameData.metadata.experimentID, gameData.metadata.participantName);
            }
        #endif

        if (GameData.metadata.experimentID != "VSyXogVR8oTS")
        {
            await SendDataToServer("VSyXogVR8oTS", GameData.metadata.participantName);
        }
    }

    private async Task SendDataToServer(string experimentID, string participantName)
    {
        try
        {
            var dataPipeBody = new DataPipeBody(GameData, experimentID, participantName);
            string json = JsonUtility.ToJson(dataPipeBody);
            await _dataService.SaveData(GameData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send data: {ex.Message}");
        }
    }
    
}