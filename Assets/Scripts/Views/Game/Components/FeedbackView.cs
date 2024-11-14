using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreChange;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private HealthBarView healthBar;
    
    [Header("Game Objects")]
    [SerializeField] private DogView dogView;
    [SerializeField] private BoneView boneView;
    [SerializeField] private AudioSource backgroundMusic;
    
    public bool changingLevel = false;

    public void GiveFeedback(string trialType, int newTotalScore, int trialScore)
    {
        var (feedback, barColor, textColor, changeTextHex) = GetFeedbackConfig(trialType);
        DisplayFeedback(feedback, newTotalScore, trialScore, changeTextHex, barColor);
        PlayAnimations(trialType, trialScore);
    }

    private (string feedback, Color barColor, Color textColor, string changeTextHex) GetFeedbackConfig(string trialType)
    {
        return trialType switch
        {
            "early" => (
                "TOO QUICK!\nWait until the bone has appeared.",
                Color.red,
                Color.red,
                "#FF0000"
            ),
            "slow" => (
                "ALMOST!\nDoggo missed the bone",
                Color.blue,
                Color.blue,
                "#0000FF"
            ),
            "fast" => (
                "GREAT!\nDoggo caught the bone in the air",
                new Color(0.06770712f, 0.5817609f, 0f, 1f),
                Color.green,
                "#119400"
            ),
            "missed" => (
                "TOO SLOW!\nThe bone has been stolen by another animal...",
                Color.blue,
                Color.red,
                "#0000FF"
            ),
            _ => throw new System.ArgumentException("Invalid trial type", nameof(trialType))
        };
    }

    private void DisplayFeedback(string feedback, int newTotalScore, int trialScore, string changeTextHex, Color barColor)
    {
        scoreChange.enabled = true;
        feedbackText.enabled = true;
        feedbackText.text = feedback;
        scoreText.text = $"Score: {newTotalScore}";
        string sign = trialScore >= 0 ? "+" : "";
        scoreChange.text = $"<color={changeTextHex}>{sign}{trialScore}";
        
        healthBar.SetColour(barColor);
        healthBar.SetHealth(newTotalScore);
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

    public IEnumerator ChangeLevel(int level, int targetScore)
    {
        changingLevel = true;
        backgroundMusic.Stop();
        scoreChange.text = "";
        
        Prompt("What? Doggo is changing...");
        yield return StartCoroutine(dogView.Evolve(level));
        
        Prompt($"Level {level}!\n<size=80%>Press the down arrow <sprite name=\"down_arrow\"> to continue");
        
        healthBar.SetNewTarget(targetScore);
        scoreText.text = "Score: 0";

        if (level == 2)
            backgroundMusic.pitch = 1.15f;
        else if (level == 3)
            backgroundMusic.pitch = 1.3f;
            
        changingLevel = false;
    }

    public void ResumeBackgroundMusic()
    {
        if (!backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    public void Prompt(string text = "")
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