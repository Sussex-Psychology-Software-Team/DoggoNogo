using UnityEngine;

public class Level1Controller : MonoBehaviour
{
    public static Level1Controller Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private FeedbackView feedbackView;
    [SerializeField] private BoneView boneView;

    private Level1Data _levelData;
    private Level1ScoreController _scoreController;
    private Level1TrialController _trialController;
    private DataController _dataController;

    // Add properties for trial tracking
    public int CurrentTrialNumber => _levelData.validTrialCount;
    public int TotalTrials => gameConfig.DefaultTrialCount;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeControllers();
        _levelData = new Level1Data(gameConfig);
    }

    private void InitializeControllers()
    {
        _dataController = DataController.Instance;
        _scoreController = GetComponent<Level1ScoreController>();
        _trialController = GetComponent<Level1TrialController>();
    }

    public void StartLevel()
    {
        _dataController.Level1Started();
        _trialController.StartNewTrial();
    }

    public void ProcessTrialResult(double reactionTime)
    {
        var result = _scoreController.ProcessTrial(reactionTime, _levelData);
        _dataController.SaveTrial(result);
        feedbackView.GiveFeedback(result);
        
        // Trigger trial completed event
        GameEvents.TrialCompleted(result);
        
        if (result.ValidTrial || result.ResponseType == "missed")
        {
            _levelData.UpdateMedianRT(reactionTime);
        }

        if (!result.ValidTrial)
        {
            _trialController.StartNewTrial();
        }
    }
    
    public void StartNewTrial()
    {
        _trialController.StartNewTrial();
    }
}