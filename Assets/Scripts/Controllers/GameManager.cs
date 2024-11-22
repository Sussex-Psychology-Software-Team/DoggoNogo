using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Controller Prefabs")]
    [SerializeField] private GameController gameControllerPrefab;
    [SerializeField] private DataController dataControllerPrefab;
    [SerializeField] private UIAnimationController uiAnimationControllerPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeControllers();
    }

    private void InitializeControllers()
    {
        // Initialize in specific order to handle dependencies
        if (DataController.Instance == null)
        {
            Instantiate(dataControllerPrefab);
        }
        
        if (GameController.Instance == null)
        {
            Instantiate(gameControllerPrefab);
        }
        
        if (UIAnimationController.Instance == null)
        {
            Instantiate(uiAnimationControllerPrefab);
        }
    }

    public void StartGame()
    {
        GameController.Instance.StartExperiment();
    }
}