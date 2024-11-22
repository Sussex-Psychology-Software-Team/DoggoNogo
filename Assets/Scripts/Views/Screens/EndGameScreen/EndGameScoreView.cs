using UnityEngine;
using UnityEngine.UI;

public class EndGameScoreView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private HealthBarView healthBar;
    [SerializeField] private EndGameDogView dogView;
    [SerializeField] private SparkleEffect sparkleEffect;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 3f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AudioSource fillSound;

    public void AnimateScore(int percentileScore)
    {
        fillSound.Play();
        sparkleEffect.PlayIndefinitely();
        StartCoroutine(AnimateScoreRoutine(percentileScore));
    }

    private System.Collections.IEnumerator AnimateScoreRoutine(int targetScore)
    {
        float startTime = Time.time;
        float startValue = 0f;

        while (Time.time - startTime < animationDuration)
        {
            float progress = (Time.time - startTime) / animationDuration;
            float easedProgress = easeCurve.Evaluate(progress);
            float currentValue = Mathf.Lerp(startValue, targetScore, easedProgress);
            
            healthBar.SetHealth(currentValue);
            yield return null;
        }

        // Ensure final value is exact
        healthBar.SetHealth(targetScore);
        
        // Final animations
        dogView.PlaySound();
        dogView.StopJumping();
        sparkleEffect.Stop();
    }
}