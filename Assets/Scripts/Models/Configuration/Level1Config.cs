using UnityEngine;

[CreateAssetMenu(fileName = "Level1Config", menuName = "DoggoNogo/Level1Config")]
public class Level1Config : ScriptableObject
{
    [Header("Level 1 Specific Settings")]
    [SerializeField] private int targetScore = 500;
    [SerializeField] private float evolveAnimationDuration = 2f;
    
    // Add any other Level 1 specific settings here
    
    // Public accessors
    public int TargetScore => targetScore;
    public float EvolveAnimationDuration => evolveAnimationDuration;
    
}