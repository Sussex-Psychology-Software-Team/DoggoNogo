public class GameController : MonoBehaviour, IGameState {
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIController uiController;
    
    private GameData gameData;
    private IDataService dataService;
    private GamePhase currentPhase;

    public int CurrentTrialNumber => gameData.level1.Count;
    public int TotalTrials => gameConfig.DefaultTrials;
    public int CurrentScore => gameData.gameStats.CurrentScore;
    public GamePhase CurrentPhase => currentPhase;

    private void Awake() {
        InitializeServices();
        SubscribeToEvents();
    }

    private void InitializeServices() {
        var webService = new WebService();
        dataService = new DataService(webService);
        gameData = new GameData();
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

        double isi = CalculateISI();
        gameData.AddNewTrial(isi);
        GameEvents.GamePhaseChanged(GamePhase.TrialInProgress);
    }

    private async void EndGame() {
        currentPhase = GamePhase.GameOver;
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
        await dataService.SaveData(gameData);
    }

    private void OnDestroy() {
        GameEvents.OnTrialCompleted -= HandleTrialCompleted;
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
    }
}