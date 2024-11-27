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
    private bool _isStimulusShown;
    
    
    // Show and hide bone
    private void ShowStimulus()
    {
        var stimulusSpecifications = boneView.Show();
        _isStimulusShown = true;
        Level1Controller.Instance.SaveStimulusSpecifications(stimulusSpecifications);
    }

    private void HideStimulus()
    {
        boneView.Hide();
        _isStimulusShown = false;
    }
    
    // TRIAL SETUP
    public void StartNewTrial()
    {
        HideStimulus();
        _currentTrialIsi = Random.Range(gameConfig.ISIRange.x, gameConfig.ISIRange.y); // Note x and y are just the default names on Vector2s
        StartTimer();
    }

    private void StartTimer()
    {
        _timer.Reset();
        _timer.Start();
    }
    
    // END TRIAL
    private void EndTrial()
    {
        _timer.Stop();
        double reactionTime = _timer.Elapsed.TotalSeconds - _currentTrialIsi;
        Level1Controller.Instance.ProcessTrialResult(reactionTime);
    }
    
    private void OnDisable()
    {
        _timer.Stop();
    }
    
    // RUN TRIAL
    private void Update()
    {
        if (!_timer.IsRunning) return;
        if (!_isStimulusShown && _timer.Elapsed.TotalSeconds > _currentTrialIsi) {
            ShowStimulus();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || _timer.Elapsed.TotalSeconds > (_currentTrialIsi + gameConfig.MaxReactionTime)) {
            EndTrial();
        }
    }
}