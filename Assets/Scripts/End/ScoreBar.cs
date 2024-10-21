using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBar : MonoBehaviour
{
    public AudioSource fillSound;
    public HealthBar healthBarScript;
    public EndDog endDogScript; 
    public Sparkles sparkles;
    
    // Animation control variables
    public float animationDuration = 3f; // Total duration in seconds
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Customizable easing curve


    public void AnimateScore(int zScore){
        FillSound();
        sparkles.Play();
        StartCoroutine(ScoreAnimator(zScore));
    }

    void FillSound(){
        fillSound.Play();
    }

    IEnumerator ScoreAnimator(int percentScore){
        float startTime = Time.time;
        float startValue = 0f;
        float endValue = percentScore;

        while (Time.time - startTime < animationDuration)
        {
            float progress = (Time.time - startTime) / animationDuration;
            float easedProgress = easeCurve.Evaluate(progress);
            
            float currentValue = Mathf.Lerp(startValue, endValue, easedProgress);
            healthBarScript.SetHealth(currentValue);
            
            yield return null;
        }

        // Ensure we end on the exact percentScore
        healthBarScript.SetHealth(percentScore);
        // Final dog animations and sound
        endDogScript.makeNoise();
        endDogScript.ToggleJump(false);
        sparkles.Stop();
    }
}
