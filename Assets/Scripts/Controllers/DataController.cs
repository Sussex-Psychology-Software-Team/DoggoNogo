using System;
using UnityEngine;
using System.Threading.Tasks;

public class DataController : MonoBehaviour
{
    public static DataController Instance { get; private set; }
    
    [SerializeField] private GameConfig gameConfig;
    
    private GameData gameData;
    private IDataService dataService;
    private WebService webService;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeServices();
    }

    private void Start()
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        gameData.metadata.InitializeWebVariables();
    }

    private void InitializeServices()
    {
        webService = new WebService();
        dataService = new DataService(webService);
    }

    public int GetNTrialsFromQuery()
    {
        int defaultN = gameConfig.DefaultTrials;
        if (int.TryParse(gameData.metadata.l1n, out int l1n))
        {
            return l1n;
        }
        return defaultN;
    }

    public void NewTrial(double isi)
    {
        gameData.AddNewTrial(isi);
    }

    public void Level1Started()
    {
        gameData.ClearTrials();
        gameData.metadata.startL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.Level1);
    }

    public void StimuliShown(Dictionary<string, float> stimSpec)
    {
        gameData.CurrentTrial()?.SaveStimulus(stimSpec);
    }

    public void SaveTrial(double rt, string type, int score, int total, double threshold, bool validTrial, int validTrialCount)
    {
        gameData.CurrentTrial()?.SaveTrial(rt, type, score, total, threshold, validTrial, validTrialCount);
        GameEvents.ScoreUpdated(total);
    }

    public void Level1Ended()
    {
        gameData.metadata.endL1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameEvents.GamePhaseChanged(GamePhase.GameOver);
    }

    public double GetCurrentThreshold()
    {
        return (gameData.level1.Count > 0) ? gameData.CurrentTrial().threshold : gameConfig.BaseThreshold;
    }

    public async Task SendData()
    {
        gameData.metadata.end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        #if !UNITY_EDITOR && UNITY_WEBGL
            if (gameData.metadata.experimentID != "QUERY VAR NOT FOUND")
            {
                await SendDataToServer(gameData.metadata.experimentID, gameData.metadata.participantName);
            }
        #endif

        if (gameData.metadata.experimentID != "VSyXogVR8oTS")
        {
            await SendDataToServer("VSyXogVR8oTS", gameData.metadata.participantName);
        }
    }

    private async Task SendDataToServer(string experimentID, string participantName)
    {
        try
        {
            var dataPipeBody = new DataPipeBody(gameData, experimentID, participantName);
            string json = JsonUtility.ToJson(dataPipeBody);
            await dataService.SaveData(gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send data: {ex.Message}");
        }
    }
}