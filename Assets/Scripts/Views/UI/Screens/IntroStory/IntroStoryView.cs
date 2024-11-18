using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class IntroStoryView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Image textBackground;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image dogImage;
    [SerializeField] private Slider skipProgressSlider;
    
    [Header("Audio")]
    [SerializeField] private AudioSource metalDoorSound;
    [SerializeField] private AudioSource dogWhineSound;
    
    [Header("Settings")]
    [SerializeField] private float chapterDuration = 4.0f;
    [SerializeField] private float skipHoldDuration = 2.0f;
    
    private readonly string[] _storyChapters = {
        "You are an investigator tasked with bringing down criminals that mistreat animals.",
        "During your last raid, you hear something.",
        "You decide to take him home, and name him…\n\n",
        "Doggo"
    };

    private int _currentChapter;
    private float _chapterTimer;
    private readonly Stopwatch _skipTimer = new();
    private UIAnimationController _animationController;

    private void Start()
    {
        _animationController = GetComponent<UIAnimationController>();
        InitializeStory();
    }

    private async void InitializeStory()
    {
        try
        {
            // Fades in the story text with a duration of 1 second
            await _animationController.FadeGraphic(storyText, 1f);
        
            // Sets the chapter timer to the specified duration
            _chapterTimer = chapterDuration;
        }
        catch (Exception e)
        {
            // Log the exception for debugging purposes
            Debug.LogError($"Error initializing story: {e.Message}\n{e.StackTrace}");
        }
    }

    private void Update()
    {
        HandleSkipInput();
        UpdateChapter();
    }

    private void HandleSkipInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _skipTimer.Start();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _skipTimer.Reset();
        }

        skipProgressSlider.value = (float)(_skipTimer.Elapsed.TotalSeconds / skipHoldDuration);

        if (Input.GetKey(KeyCode.Escape) || _skipTimer.Elapsed.TotalSeconds >= skipHoldDuration)
        {
            LoadGameScene();
        }
    }

    private void UpdateChapter()
    {
        _chapterTimer -= Time.deltaTime;
        if (_chapterTimer <= 0f)
        {
            AdvanceChapter();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async void AdvanceChapter()
    {
        try
        {
            _currentChapter++;
        
            switch (_currentChapter)
            {
                case 2:
                    metalDoorSound.Play();
                    storyText.text = _storyChapters[1];
                    break;
            
                case 3:
                    storyText.text = "";
                    backgroundImage.color = Color.white;
                    await _animationController.FadeGraphic(backgroundImage, 1f);
                    dogWhineSound.Play();
                    break;
            
                case 4:
                    dogImage.enabled = true;
                    break;
            
                case 5:
                    storyText.text = _storyChapters[2];
                    break;
            
                case 6:
                    dogImage.enabled = false;
                    backgroundImage.color = Color.black;
                    storyText.fontSize = 100;
                    storyText.text = _storyChapters[3];
                    break;
                case 7:
                    LoadGameScene();
                    return;
            }

            _chapterTimer = chapterDuration;
        }
        catch (Exception e)
        {
            // Log the exception for debugging purposes
            Debug.LogError($"Error initializing story: {e.Message}\n{e.StackTrace}");
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Simple RT");
    }
}
