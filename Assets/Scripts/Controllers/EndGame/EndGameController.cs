using UnityEngine;

public class EndGameController : MonoBehaviour
{
    public static EndGameController Instance { get; private set; }

    [SerializeField] private GameConfig gameConfig;
    private EndGameScoreData _scoreData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeEndGame();
    }

    private void InitializeEndGame()
    {
        _scoreData = new EndGameScoreData();
        GameController.Instance.OnLevelComplete();
    }

    public float CalculatePercentileScore()
    {
        double threshold = GameController.Instance.GetCurrentThreshold();
        return (float)MathUtils.CalculatePercentileNormCDF(threshold);
    }
}