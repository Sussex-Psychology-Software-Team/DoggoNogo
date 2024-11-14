using System.Diagnostics;
using UnityEngine;

public class Level1TrialController : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BoneView boneView;
    private readonly Stopwatch _timer = new();
    private double _currentTrialIsi;

    public void StartNewTrial()
    {
        boneView.Hide();
        _currentTrialIsi = UnityEngine.Random.Range(gameConfig.ISIRange.x, gameConfig.ISIRange.y);
        DataController.Instance.NewTrial(_currentTrialIsi);
        RestartTimer();
    }

    public void Update()
    {
        if (!_timer.IsRunning) return;

        if (_timer.Elapsed.TotalSeconds > _currentTrialIsi && boneView.Hidden())
        {
            var stimSpec = boneView.Show();
            DataController.Instance.StimuliShown(stimSpec);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || _timer.Elapsed.TotalSeconds > (_currentTrialIsi + gameConfig.MaxReactionTime))
        {
            EndTrial();
        }
    }

    private void EndTrial()
    {
        _timer.Stop();
        double rt = _timer.Elapsed.TotalSeconds - _currentTrialIsi;
        Level1Controller.Instance.ProcessTrialResult(rt);
    }

    private void RestartTimer()
    {
        _timer.Reset();
        _timer.Start();
    }
}