using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main game screen - I think this is Level1 UI and needs to be updated with dog and bone views, and other methods
public class Level1UI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private FeedbackView feedbackView; // Feedback view to communicate through
    [SerializeField] private List<HealthBarView> healthBars; // List of health bars to flip through
    
    // Public methods for controller to call
    public void ShowHealthBar(int index)
    {
        for (int i = 0; i < healthBars.Count; i++)
        {
            healthBars[i].gameObject.SetActive(i == index);
        }
    }

    public void UpdateHealthBar(int index, float value)
    {
        healthBars[index].SetHealth(value);
    }

    public void ConfigureHealthBar(int index, float maxValue, Color color)
    {
        healthBars[index].SetMaxValue(maxValue);
        healthBars[index].SetColor(color);
    }

    public void DisplayTrialResult(TrialResult result)
    {
        feedbackView.DisplayTrialResult(result); // Use controller to 
    }

    public IEnumerator HandleLevelTransition(int newStage)
    {
        yield return StartCoroutine(feedbackView.HandleLevelChange(newStage));
    }
}