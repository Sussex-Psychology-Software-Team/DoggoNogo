using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroStoryView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Image textBackground;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image dogImage;
    [SerializeField] private Slider skipProgressSlider;
    
    [Header("Story Assets")]
    [SerializeField] private Sprite gardenSprite;
    
    [Header("Audio")]
    [SerializeField] private AudioSource metalDoorSound;
    [SerializeField] private AudioSource dogWhineSound;
    
    [Header("Settings")]
    [SerializeField] private float chapterDuration = 4.0f;
    [SerializeField] private float skipHoldDuration = 2.0f;
    
    private readonly string[] storyChapters = {
        "You are an investigator tasked with bringing down criminals that mistreat animals.",
        "During your last raid, you hear something.",
        "You decide to take him home, and name him…\n\n",
        "Doggo"
    };

    private int currentChapter;
    private float chapterTimer;
    private readonly Stopwatch skipTimer = new();
    private UIAnimationController _animationController;

    private void Start()
    {
        _animationController = GetComponent<UIAnimationController>();
        InitializeStory();
    }

    private async void InitializeStory()
    {
        await _animationController.FadeGraphic(storyText, 1f);
        chapterTimer = chapterDuration;
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
            skipTimer.Start();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            skipTimer.Reset();
        }

        skipProgressSlider.value = (float)(skipTimer.Elapsed.TotalSeconds / skipHoldDuration);

        if (Input.GetKey(KeyCode.Escape) || skipTimer.Elapsed.TotalSeconds >= skipHoldDuration)
        {
            LoadGameScene();
        }
    }

    private void UpdateChapter()
    {
        chapterTimer -= Time.deltaTime;
        if (chapterTimer <= 0f)
        {
            AdvanceChapter();
        }
    }

    private async void AdvanceChapter()
    {
        currentChapter++;
        if (currentChapter >= 6)
        {
            LoadGameScene();
            return;
        }

        switch (currentChapter)
        {
            case 1:
                metalDoorSound.Play();
                storyText.text = storyChapters[1];
                break;
            
            case 2:
                storyText.text = "";
                backgroundImage.color = Color.white;
                await _animationController.FadeGraphic(backgroundImage, 1f);
                dogWhineSound.Play();
                break;
            
            case 3:
                dogImage.enabled = true;
                break;
            
            case 4:
                storyText.text = storyChapters[2];
                break;
            
            case 5:
                dogImage.enabled = false;
                backgroundImage.color = Color.black;
                storyText.fontSize = 100;
                storyText.text = storyChapters[3];
                break;
        }

        chapterTimer = chapterDuration;
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Simple RT");
    }
}
