using UnityEngine;
using UnityEngine.SceneManagement;

// Overall game state and flow: Coordinates between other controllers, and manages game phases (start, end)
// Generally, controllers manage game logic and connect Models with Views

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    
    private GameData _gameData;
    private IDataService _dataService;
    private bool _isGameActive;

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
        SubscribeToEvents();
    }

    private void InitializeServices()
    {
        var webService = new WebService();
        _dataService = new DataService(webService);
        _gameData = new GameData();
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnExperimentComplete += HandleExperimentComplete;
        GameEvents.OnGamePhaseChanged += HandleGamePhaseChanged;
    }

    public void StartExperiment()
    {
        _isGameActive = true;
        _gameData = new GameData();
        _gameData.metadata.InitializeWebVariables();
    }

    private async void HandleExperimentComplete(ExperimentData experimentData)
    {
        if (!_isGameActive) return;
        
        _isGameActive = false;
        await _dataService.SaveData(_gameData);
        SceneManager.LoadScene("End");
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
    }

    private void HandleGamePhaseChanged(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Level1:
                _gameData.metadata.startL1 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;
            case GamePhase.GameOver:
                _gameData.metadata.endL1 = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnExperimentComplete -= HandleExperimentComplete;
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
    }
}