using UnityEngine;

// Level 1 main controller: Initializes components, High-level level management, Communicates with GameController
public class Level1Controller : MonoBehaviour
{
    public static Level1Controller Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private FeedbackView feedbackView;
    [SerializeField] private BoneView boneView;

    private Level1Data _levelData;
    private Level1TrialController _trialController;
    private ReactionTimeProcessor _rtProcessor;
    private ScoreCalculator _scoreCalculator;
    private DataController _dataController;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _rtProcessor = new ReactionTimeProcessor(gameConfig);
        _scoreCalculator = new ScoreCalculator(gameConfig);
        _trialController = GetComponent<Level1TrialController>();
        _dataController = DataController.Instance;
        _levelData = new Level1Data(gameConfig);
    }

    // Add this method
    public void StartLevel()
    {
        _levelData = new Level1Data(gameConfig);
        _trialController.StartNewTrial();
    }

    public void StartNewTrial()
    {
        _trialController.StartNewTrial();
    }

    public void ProcessTrialResult(double reactionTime)
    {
        string responseType = _rtProcessor.DetermineResponseType(reactionTime);
        int score = _scoreCalculator.CalculateScore(responseType, reactionTime);
        bool isValidTrial = responseType is "slow" or "fast";
        
        if (isValidTrial) _levelData.validTrialCount++;
        if (isValidTrial || responseType == "missed")
        {
            _rtProcessor.UpdateMedianRT(reactionTime);
        }

        var result = new TrialResult
        {
            ReactionTime = reactionTime,
            ResponseType = responseType,
            TrialScore = score,
            TotalScore = _levelData.currentScore,
            Threshold = _rtProcessor.GetCurrentThreshold(),
            ValidTrial = isValidTrial,
            ValidTrialCount = _levelData.validTrialCount
        };

        _dataController.SaveTrial(result);
        feedbackView.GiveFeedback(result);

        if (!result.ValidTrial)
        {
            _trialController.StartNewTrial();
        }
    }

    public int CurrentTrialNumber => _levelData.validTrialCount;
    public int TotalTrials => gameConfig.DefaultTrialCount;
}