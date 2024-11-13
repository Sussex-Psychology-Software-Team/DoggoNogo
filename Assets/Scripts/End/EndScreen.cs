using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //for TextMeshProUGUI

public class EndScreen : MonoBehaviour
{
    // Asset references
    public HealthBar scoreBar;
    public TextMeshProUGUI percentileText; // displays score
    public ScoreBar scoreScript;
    public FullscreenManager fullscreenManager;

    

    // Score display functions
    void DisplayRelativeScore(){
        // Calculate %
        double threshold = GameController.Instance.GetCurrentThreshold();
        double zScore = Calculations.PercentileNormCDF(threshold); // Score under normal as %
        // Change text and Healthbar
        percentileText.text = "You completed the game, congratulations!\n\nYour reflexes were faster and more accurate than " + zScore.ToString("F0") + "% of people. Well done!\n\n<size=70%>You can now close the tab.";
        scoreScript.AnimateScore((int)zScore);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        // Send experimental data
        GameController.Instance.OnLevelComplete();
        fullscreenManager.ToggleFullscreen();
        // Clear trials ahead of repeat - probably just do this on a 'repeat' button listener
        //data.ClearTrials();
        DisplayRelativeScore();
    }
}
