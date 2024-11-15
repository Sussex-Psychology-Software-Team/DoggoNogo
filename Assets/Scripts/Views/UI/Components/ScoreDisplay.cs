using TMPro;
using UnityEngine;

// Shows current score
// This is probably the text on screen for score in L1.
// not sure it really needs the onenable/disable stuff? Event shouldn't be firing anyway. 
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