using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class DataController : MonoBehaviour
{
    public static DataController Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    
    private GameData _gameData;
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
        InitializeServices();
    }

    private void Start()
    {
        if (_gameData == null)
        {
            _gameData = new GameData();
        }
        _gameData.metadata.InitializeWebVariables();
    }

    private void InitializeServices()
    {
        _webService = new WebService();
        _dataService = new DataService(_webService);
    }

    public int GetNTrialsFromQuery()
    {
        int defaultN = gameConfig.DefaultTrialCount;
        return int.TryParse((ReadOnlySpan<char>)_gameData.metadata.l1n, out int l1N) ? l1N : defaultN;
    }

    public void NewTrial(double isi)
    {
        _gameData.AddNewTrial(isi);
    }

    public void Level1Started()
    {
        _gameData.ClearTrials();
        _gameData.metadata.startL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.Level1);
    }

    public void StimuliShown(Dictionary<string, float> stimSpec)
    {
        _gameData.CurrentTrial()?.SaveStimulus(stimSpec);
    }

    public void SaveTrial(TrialResult result)
    {
        var currentTrial = _gameData.CurrentTrial();
        currentTrial?.SaveTrial(result);
    }

    public void Level1Ended()
    {
        _gameData.metadata.endL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
    }

    public double GetCurrentThreshold()
    {
        return (_gameData.level1.Count > 0) ? _gameData.CurrentTrial().threshold : gameConfig.InitialMedianRT;
    }

    public async Task SendData()
    {
        _gameData.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        #if !UNITY_EDITOR && UNITY_WEBGL
            if (gameData.metadata.experimentID != "QUERY VAR NOT FOUND")
            {
                await SendDataToServer(gameData.metadata.experimentID, gameData.metadata.participantName);
            }
        #endif

        if (_gameData.metadata.experimentID != "VSyXogVR8oTS")
        {
            await SendDataToServer("VSyXogVR8oTS", _gameData.metadata.participantName);
        }
    }

    private async Task SendDataToServer(string experimentID, string participantName)
    {
        try
        {
            var dataPipeBody = new DataPipeBody(_gameData, experimentID, participantName);
            string json = JsonUtility.ToJson(dataPipeBody);
            await _dataService.SaveData(_gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send data: {ex.Message}");
        }
    }
}