using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private void OnEnable() {
        GameEvents.OnScoreUpdated += UpdateScore;
    }

    private void OnDisable() {
        GameEvents.OnScoreUpdated -= UpdateScore;
    }

    public void UpdateScore(int newScore) {
        scoreText.text = $"Score: {newScore}";
    }
}