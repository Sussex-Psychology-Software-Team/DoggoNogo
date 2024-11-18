using UnityEngine;

// Overall game state and flow: Coordinates between other controllers, and manages game phases (start, end)
// Generally, controllers manage game logic and connect Models with Views
public class GameController : MonoBehaviour, IGameState { //Singleton instance for global access
    public static GameController Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIController uiController;
    
    private GameData _gameData;
    private IDataService _dataService;
    private GamePhase _currentPhase;
    private bool _isGameActive;

    // IGameState properties
    public int CurrentTrialNumber => _gameData?.level1.Count ?? 0;
    public int TotalTrials => gameConfig.DefaultTrialCount;
    public int CurrentScore => _gameData?.gameStats.CurrentScore ?? 0;
    public GamePhase CurrentPhase
    {
        get => _currentPhase;
        set => _currentPhase = value;
    }

    // IGameState methods
    public void StartNewTrial()
    {
        if (CurrentTrialNumber >= TotalTrials)
        {
            EndGame();
            return;
        }

        double isi = CalculateIsi();
        _gameData.AddNewTrial(isi);
        GameEvents.GamePhaseChanged(GamePhase.TrialInProgress);
    }

    public void EndTrial(TrialResult result)
    {
        HandleTrialCompleted(result);
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
        SubscribeToEvents();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void StartGame()
    {
        _isGameActive = true;
        _gameData = new GameData();
        _gameData.metadata.InitializeWebVariables();
        
        Level1Controller.Instance.StartLevel();
        UIController.Instance.ShowGameScreen();
        
        GameEvents.GamePhaseChanged(GamePhase.Level1);
    }
    private void InitializeServices() {
        var webService = new WebService();
        _dataService = new DataService(webService);
        _gameData = new GameData();
    }

    private void SubscribeToEvents() {
        GameEvents.OnTrialCompleted += HandleTrialCompleted;
        GameEvents.OnGamePhaseChanged += HandleGamePhaseChanged;
    }
    
    public double GetCurrentThreshold()
    {
        return _gameData?.level1.Count > 0 
            ? _gameData.level1[^1].threshold 
            : gameConfig.InitialMedianRT;
    }

    public void OnLevelComplete()
    {
        EndGame();
    }
    
    private double CalculateIsi()
    {
        return UnityEngine.Random.Range(gameConfig.ISIRange.x, gameConfig.ISIRange.y);
    }

    private async void EndGame() {
        CurrentPhase = GamePhase.GameOver;
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
        await _dataService.SaveData(_gameData);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleTrialCompleted(TrialResult result)
    {
        if (!_isGameActive) return;

        DataController.Instance.SaveTrial(result);
        UIController.Instance.UpdateScore(result.TotalScore);
        
        if (result.ValidTrialCount >= gameConfig.DefaultTrialCount)
        {
            EndGame();
            return;
        }

        if (!result.ValidTrial)
        {
            Level1Controller.Instance.StartNewTrial();
        }
    }
    
    private void HandleGamePhaseChanged(GamePhase phase)
    {
        CurrentPhase = phase;
        _isGameActive = phase == GamePhase.Level1 || phase == GamePhase.TrialInProgress;
    
        switch (phase)
        {
            case GamePhase.Level1:
                DataController.Instance.Level1Started();
                break;
            case GamePhase.GameOver:
                DataController.Instance.Level1Ended();
                break;
        }
    }
    
    private void OnDestroy() {
        GameEvents.OnTrialCompleted -= HandleTrialCompleted;
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
    }
}