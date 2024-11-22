using System;
using System.Threading.Tasks;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController Instance { get; private set; }
    public GameData GameData { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;

    private IDataService _dataService;

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

    private void Start()
    {
        InitializeGameData();
    }

    private void InitializeServices()
    {
        var webService = new WebService();
        _dataService = new DataService(webService);
    }

    private void InitializeGameData()
    {
        _gameData = new GameData();
        _gameData.metadata.InitializeWebVariables();
    }
    
    public void UpdateGamePhase(GamePhase phase)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        switch (phase)
        {
            case GamePhase.Level1:
                if (_gameData != null) _gameData.metadata.startL1 = timestamp;
                break;
            case GamePhase.GameOver:
                if (_gameData != null)
                {
                    _gameData.metadata.endL1 = timestamp;
                    _gameData.metadata.end = timestamp;
                }
                break;
        }
    }

    public async Task SaveExperimentData(ExperimentData experimentData)
    {
        if (_gameData == null) return;

        try
        {
            // Let DataService handle the save operations
            await _dataService.SaveData(_gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save experiment data: {ex.Message}");
        }
    }
    
}