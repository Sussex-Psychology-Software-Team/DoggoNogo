[CreateAssetMenu(fileName = "GameConfig", menuName = "DoggoNogo/GameConfig")]
public class GameConfig : ScriptableObject {
    [SerializeField] private int defaultTrials = 60;
    [SerializeField] private float baseThreshold = 0.5f;
    [SerializeField] private float timeoutDuration = 5f;
    
    public int DefaultTrials => defaultTrials;
    public float BaseThreshold => baseThreshold;
    public float TimeoutDuration => timeoutDuration;
}