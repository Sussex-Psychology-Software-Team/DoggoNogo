using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds references to UI elements/prefabs - distinct from Level1UIController
    // Handles direct manipulation of UI elements
    // Manages view state transitions
    // Provides methods for modifying UI elements
    // Does not contain game logic or event handling

// Stuff here and in introduction view is called elsewhere.
public class Level1ViewManager : MonoBehaviour
{
    [Header("Introduction")]
    [SerializeField] private Level1IntroductionView introView;

    [Header("Score components")]
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private FeedbackView feedbackView;
    [SerializeField] private List<HealthBarView> healthBars;
    
    // Initial continue on Ready
    private bool _allowReadyContinue = false;
    public Level1IntroductionView GetIntroView() => introView;
    
    public void PromptAndAllowStartLevel() // Set this after intro signpost is hidden
    {
        scoreContainer.SetActive(true);
        _allowReadyContinue = true;
    }
    
    private void Update()
    {
        if (_allowReadyContinue && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _allowReadyContinue = false;
            StartLevel();
        }
    }
    
    private void StartLevel()
    {
        ClearInstructions();
        Level1Controller.Instance.StartLevel();
    }

    // Manage gameplay views
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
        feedbackView.DisplayTrialResult(result);
    }

    private void ClearInstructions()
    {
        feedbackView.SetPrompt("");
    }

    public IEnumerator HandleLevelTransition(int newStage)
    {
        yield return StartCoroutine(feedbackView.HandleLevelChange(newStage));
    }
}