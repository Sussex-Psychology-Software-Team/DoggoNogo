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

    public void StartLevel1()
    {
        if (!_isExperimentActive) return;
        SceneManager.LoadScene("Simple RT");
    }

    private void HandleGamePhaseChanged(GamePhase newPhase)
    {
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