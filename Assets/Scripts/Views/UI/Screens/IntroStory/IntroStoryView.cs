using System;
using System.Collections;
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
    private Coroutine _storyCoroutine;
    
    private void Start()
    {
        StartCoroutine(InitializeStory());
    }

    // NOTE: In a try-catch block, yield return statements must be in the method body itself, not inside the try block.
    private IEnumerator InitializeStory()
    {
        try
        {
            _storyCoroutine = StartCoroutine(RunStorySequence());
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing story: {e.Message}\n{e.StackTrace}");
        }
        yield break;
    }
    
    private IEnumerator RunStorySequence()
    {
        // Fades in the story text with a duration of 1 second
        yield return StartCoroutine(GameController.Instance.Animations.FadeIn(storyText, 1f));

        // Start the main story sequence
        while (_currentChapter < 7)
        {
            yield return new WaitForSeconds(chapterDuration);
            yield return StartCoroutine(RunAdvanceChapter());
        }
    
        LoadGameScene();
    }
    
    private IEnumerator AdvanceChapter()
    {
        try
        {
            StartCoroutine(RunAdvanceChapter());
        }
        catch (Exception e)
        {
            Debug.LogError($"Error advancing chapter: {e.Message}\n{e.StackTrace}");
        }
        yield break;
    }
    
    private IEnumerator RunAdvanceChapter()
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
                yield return StartCoroutine(GameController.Instance.Animations.FadeIn(backgroundImage, 1f));
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
        }
    }
    
    private void Update()
    {
        HandleSkipInput();
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
            if (_storyCoroutine != null)
                StopCoroutine(_storyCoroutine);
            LoadGameScene();
        }
    }

    private static void LoadGameScene()
    {
        SceneManager.LoadScene("Simple RT");
    }
}