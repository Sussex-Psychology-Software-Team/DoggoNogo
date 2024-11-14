using UnityEngine;

[System.Serializable]
public class FeedbackData
{
    public string feedbackText;
    public Color barColor;
    public Color textColor;
    public int scoreChange;
    public string responseType;
    public int totalScore;
    public string colorHex;

    public static FeedbackData FromTrialResult(TrialResult result)
    {
        var data = new FeedbackData
        {
            responseType = result.ResponseType,
            scoreChange = result.TrialScore,
            totalScore = result.TotalScore
        };

        (data.feedbackText, data.barColor, data.textColor, data.colorHex) = result.ResponseType switch
        {
            "early" => ("TOO QUICK!\nWait until the bone has appeared.", Color.red, Color.red, "#FF0000"),
            "slow" => ("ALMOST!\nDoggo missed the bone", Color.blue, Color.blue, "#0000FF"),
            "fast" => ("GREAT!\nDoggo caught the bone in the air", new Color(0.06770712f, 0.5817609f, 0f, 1f), Color.green, "#119400"),
            "missed" => ("TOO SLOW!\nThe bone has been stolen by another animal...", Color.blue, Color.red, "#0000FF"),
            _ => ("", Color.white, Color.white, "#FFFFFF")
        };

        return data;
    }
}