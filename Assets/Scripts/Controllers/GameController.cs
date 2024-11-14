using UnityEngine;

public class GameController : MonoBehaviour, IGameState {
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIController uiController;
    
    private GameData _gameData;
    private IDataService _dataService;
    private bool _isGameActive;

    public int CurrentTrialNumber => _gameData.level1.Count;
    public int TotalTrials => gameConfig.DefaultTrialCount;
    public int CurrentScore => _gameData.gameStats.CurrentScore;
    public GamePhase CurrentPhase { get; private set; }

    private void Awake() {
        InitializeServices();
        SubscribeToEvents();
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

    public void StartNewTrial() {
        if (CurrentTrialNumber >= TotalTrials) {
            EndGame();
            return;
        }

        double isi = CalculateIsi();
        _gameData.AddNewTrial(isi);
        GameEvents.GamePhaseChanged(GamePhase.TrialInProgress);
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