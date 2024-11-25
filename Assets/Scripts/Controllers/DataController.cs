using System;
using System.Threading.Tasks;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    
    private GameData _gameData;
    private IDataService _dataService;
    
    // Public accessor for GameData with initialization check
    public GameData GameData 
    { 
        get
        {
            if (_gameData == null)
            {
                InitializeGameData();
            }
            return _gameData;
        }
    }

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
    
    // Save start and end times of levels
    public void SavePhaseTimeStamp(GamePhase phase)
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

    // Save the experimental data
    public async Task SaveExperimentData()
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