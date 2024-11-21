using System.Diagnostics;
using UnityEngine;

// Defines trials in controller: Manages trial flow, Controls stimulus timing, Handles input detection, Manages ISI
// Essentially TrialManager from before...
public class Level1TrialController : MonoBehaviour
{
    // VARS
    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;

    [Header("View References")]
    [SerializeField] private BoneView boneView;

    private readonly Stopwatch _timer = new();
    private double _currentTrialIsi;
    private bool _isTrialActive;
    private bool _isStimulusShown;

    // FUNCTIONS
    
    // TRIAL SETUP
    public void StartNewTrial()
    {
        ResetTrial();
        _currentTrialIsi = GenerateIsi();
        Level1Events.NewTrialStarted(_currentTrialIsi);
        Level1Events.TrialStateChanged(TrialState.WaitingForStimulus);
        StartTimer();
    }
    
    private void ResetTrial()
    {
        _isTrialActive = true;
        _isStimulusShown = false;
        boneView.Hide();
    }

    private double GenerateIsi()
    {
        // Note x and y are just the default names on Vector2s
        return Random.Range(gameConfig.ISIRange.x, gameConfig.ISIRange.y);
    }
    
    private void StartTimer()
    {
        _timer.Reset();
        _timer.Start();
    }

    // DURING TRIAL
    private void HandleStimulusPresentation()
    {
        if (_timer.IsRunning && !_isStimulusShown && 
                _timer.Elapsed.TotalSeconds > _currentTrialIsi)
        {
            ShowStimulus();
        }
    }
    
    private void ShowStimulus()
    {
        var stimulusSpecifications = boneView.Show();
        _isStimulusShown = true;
        Level1Events.StimulusShown(stimulusSpecifications);
        Level1Events.TrialStateChanged(TrialState.WaitingForResponse);
    }
    
    private void HandleInput()
    {
        bool shouldEndTrial = Input.GetKeyDown(KeyCode.DownArrow) || 
                              _timer.Elapsed.TotalSeconds > (_currentTrialIsi + gameConfig.MaxReactionTime);

        if (shouldEndTrial)
        {
            EndTrial();
        }
    }

    // END TRIAL
    private void EndTrial()
    {
        if (!_isTrialActive) return;

        _isTrialActive = false;
        _timer.Stop();
        double reactionTime = CalculateReactionTime();
        // Perhaps remove events?
        Level1Events.ReactionTimeRecorded(reactionTime);
        Level1Events.TrialStateChanged(TrialState.Complete);
        Level1Controller.Instance.ProcessTrialResult(reactionTime);
    }
    
    private double CalculateReactionTime()
    {
        return _timer.Elapsed.TotalSeconds - _currentTrialIsi;
    }
    
    private void OnDisable()
    {
        _timer.Stop();
        _isTrialActive = false;
    }
    
    // RUN UPDATE
    private void Update()
    {
        if (!_isTrialActive) return;

        HandleStimulusPresentation();
        HandleInput();
    }
    
}