using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreChange;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Game Objects")]
    [SerializeField] private DogView dogView;
    [SerializeField] private BoneView boneView;
    [SerializeField] private AudioSource backgroundMusic;

    public bool changingLevel = false;

    private (string feedback, Color textColor, string changeTextHex) GetFeedbackConfig(string trialType)
    {
        return trialType switch
        {
            "early" => (
                "TOO QUICK!\nWait until the bone has appeared.",
                Color.red,
                "#FF0000"
            ),
            "slow" => (
                "ALMOST!\nDoggo missed the bone",
                Color.blue,
                "#0000FF"
            ),
            "fast" => (
                "GREAT!\nDoggo caught the bone in the air",
                Color.green,
                "#119400"
            ),
            "missed" => (
                "TOO SLOW!\nThe bone has been stolen by another animal...",
                Color.red,
                "#0000FF"
            ),
            _ => throw new System.ArgumentException("Invalid trial type", nameof(trialType))
        };
    }

    public void DisplayTrialResult(TrialResult result)
    {
        var feedbackConfig = GetFeedbackConfig(result.ResponseType);
        scoreChange.enabled = true;
        feedbackText.enabled = true;
        feedbackText.text = feedbackConfig.feedback;
        scoreText.text = $"Score: {result.TotalScore}";
        string sign = result.TrialScore >= 0 ? "+" : "";
        scoreChange.text = $"<color={feedbackConfig.changeTextHex}>{sign}{result.TrialScore}";
        
        PlayAnimations(result.ResponseType, result.TrialScore);
    }

    private void PlayAnimations(string trialType, int trialScore)
    {
        switch (trialType)
        {
            case "early":
                dogView.Bark();
                dogView.TakeDamage();
                dogView.StartJump(15);
                break;
            case "slow":
                dogView.Surprised();
                dogView.StartJump(30);
                break;
            case "fast":
                boneView.Eat();
                dogView.Chew();
                dogView.StartJump((int)(trialScore/2));
                break;
            case "missed":
                boneView.Throw();
                break;
        }
    }

    public IEnumerator HandleLevelChange(int level)
    {
        changingLevel = true;
        backgroundMusic.Stop();
        scoreChange.text = "";
        
        SetPrompt("What? Doggo is changing...");
        yield return StartCoroutine(dogView.Evolve(level));
        
        SetPrompt($"Level {level}!\n<size=80%>Press the down arrow <sprite name=\"down_arrow\"> to continue");

        backgroundMusic.pitch = level switch
        {
            2 => 1.15f,
            3 => 1.3f,
            _ => backgroundMusic.pitch
        };

        changingLevel = false;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ResumeBackgroundMusic()
    {
        if (!backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    public void SetPrompt(string text)
    {
        scoreChange.text = "";
        feedbackText.color = Color.white;
        feedbackText.text = text;
    }

    public void Hide()
    {
        scoreChange.enabled = false;
        feedbackText.enabled = false;
    }
}