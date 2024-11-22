using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    private bool _isExperimentActive;
    private GamePhase _currentPhase;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnGamePhaseChanged += HandleGamePhaseChanged;
        GameEvents.OnExperimentComplete += HandleExperimentComplete;
    }

    public void StartExperiment()
    {
        _isExperimentActive = true;
        SceneManager.LoadScene("Introduction");
    }

    private async void HandleExperimentComplete(ExperimentData experimentData)
    public void StartLevel1()
    {
        if (!_isGameActive) return;
        
        _isGameActive = false;
        await _dataService.SaveData(_gameData);
        SceneManager.LoadScene("End");
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
        if (!_isExperimentActive) return;
        SceneManager.LoadScene("Simple RT");
    }

    private void HandleGamePhaseChanged(GamePhase phase)
    private void HandleGamePhaseChanged(GamePhase newPhase)
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
        _currentPhase = newPhase;
        DataController.Instance.UpdateGamePhase(newPhase);
    }

    private async void HandleExperimentComplete(ExperimentData experimentData)
    {
        if (!_isExperimentActive) return;
        
        _isExperimentActive = false;
        await DataController.Instance.SaveExperimentData(experimentData);
        SceneManager.LoadScene("End");
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
        GameEvents.OnExperimentComplete -= HandleExperimentComplete;
    }
}