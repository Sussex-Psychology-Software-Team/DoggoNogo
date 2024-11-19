using UnityEngine;

// Level 1 main controller: Initializes components, High-level level management, Communicates with GameController
public class Level1Controller : MonoBehaviour
{
    public static Level1Controller Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;

    [Header("View References")]
    [SerializeField] private Level1UIController uiController;
    [SerializeField] private BoneView boneView;

    private GameData _gameData;
    private Level1Data _levelData;
    private Level1StageData _stageData;
    private Level1TrialController _trialController;
    private ReactionTimeProcessor _rtProcessor;
    private ScoreCalculator _scoreCalculator;
    private bool _isLevelActive;

    private void Awake() // initialised this controller and related processes
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeComponents();
    }

    private void Start()
    {
        if (!GameController.Instance) // Check GameController has been initialised
        {
            Debug.LogError("GameController not found - ensure GameManager is initialized");
        }
    }

    private void InitializeComponents()
    {
        _rtProcessor = new ReactionTimeProcessor(gameConfig);
        _scoreCalculator = new ScoreCalculator(gameConfig);
        _trialController = GetComponent<Level1TrialController>();
        _levelData = new Level1Data(gameConfig);
        _stageData = new Level1StageData(_gameData.metadata, gameConfig); // Pass metadata to grab l1n
        _isLevelActive = true;
        GameEvents.GamePhaseChanged(GamePhase.Level1);
    }

    public void StartLevel()
    {
        _levelData = new Level1Data(gameConfig);
        Level1Events.LevelStarted(); // Guess nothing happens if nothing attached?
        _trialController.StartNewTrial();
    }

    public void ProcessTrialResult(double reactionTime)
    {
        if (!_isLevelActive) return;

        string responseType = _rtProcessor.DetermineResponseType(reactionTime);
        int score = _scoreCalculator.CalculateScore(responseType, reactionTime);
        bool isValidTrial = responseType is "slow" or "fast";

        if (!isValidTrial)
        {
            Level1Events.InvalidResponse(responseType);
        }
        
        UpdateLevelData(isValidTrial, reactionTime);
        _levelData.currentScore += score;
        Level1Events.ScoreUpdated(_levelData.currentScore);

        var result = CreateTrialResult(reactionTime, responseType, score, isValidTrial);
        Level1Events.TrialCompleted(result);
        
        if (ShouldEndLevel())
        {
            EndLevel();
            return;
        }
        
        CheckStageProgress(result);
        
        if (!result.ValidTrial)
        {
            _trialController.StartNewTrial();
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckStageProgress(TrialResult result)
    {
        if (result.TotalScore < _stageData.CurrentTargetScore || !_stageData.AdvanceStage()) return;
        int newTargetScore = _stageData.CalculateTargetScore(
            _levelData.validTrialCount,
            gameConfig.MinScore
        );
            
        Level1Events.StageChanged(_stageData.CurrentStage, newTargetScore);
        _stageData.CurrentTargetScore = newTargetScore;
    }

    private void UpdateLevelData(bool isValidTrial, double reactionTime)
    {
        if (isValidTrial)
        {
            _levelData.validTrialCount++;
        }
        
        if (isValidTrial || reactionTime > gameConfig.MaxReactionTime)
        {
            _levelData.UpdateMedianRT(reactionTime);
        }
    }

    private TrialResult CreateTrialResult(double reactionTime, string responseType, int score, bool isValidTrial)
    {
        return new TrialResult
        {
            ReactionTime = reactionTime,
            ResponseType = responseType,
            TrialScore = score,
            TotalScore = _levelData.currentScore,
            Threshold = _rtProcessor.GetCurrentThreshold(),
            ValidTrial = isValidTrial,
            ValidTrialCount = _levelData.validTrialCount
        };
    }

    private bool ShouldEndLevel()
    {
        return _stageData.CurrentStage == _stageData.TotalStages && _levelData.currentScore >= _stageData.CurrentTargetScore;
    }

    private void EndLevel()
    {
        _isLevelActive = false;
        var experimentData = new ExperimentData
        {
            FinalScore = _levelData.currentScore,
            ValidTrials = _levelData.validTrialCount,
            FinalThreshold = _levelData.currentMedianRT
        };
        
        Level1Events.LevelComplete();
        GameEvents.ExperimentComplete(experimentData);
    }

    public int CurrentTrialNumber => _levelData.validTrialCount;
    public int TotalTrials => gameConfig.DefaultTrialCount;

    private void OnDestroy()
    {
        _isLevelActive = false;
    }
}