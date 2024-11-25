using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    private bool _isExperimentActive;

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

    private static void HandleGamePhaseChanged(GamePhase newPhase)
    {
        DataController.Instance.SavePhaseTimeStamp(newPhase);
    }

    private async void HandleExperimentComplete(ExperimentData experimentData)
    {
        if (!_isExperimentActive) return;
        
        _isExperimentActive = false;
        await DataController.Instance.SaveExperimentData();
        SceneManager.LoadScene("End");
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseChanged -= HandleGamePhaseChanged;
        GameEvents.OnExperimentComplete -= HandleExperimentComplete;
    }
}