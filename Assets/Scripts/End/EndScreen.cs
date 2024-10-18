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

    // Average score distribution params
    public static double mean = 4000.0;
    public static double sd = 2000.0;

    // Score display functions
    void DisplayRelativeScore(){
        // Calculate %
        int score;
        if(DataManager.Instance.data.trials.Count > 0) score = DataManager.Instance.data.CurrentTrial().totalScore; //get copy of player score
        else score = (int)mean;

        Debug.Log(score);
        double zScore = PercentileNormCDF(score); // Score under normal as %
        Debug.Log(zScore);
        // Change text and Healthbar
        percentileText.text = "You completed the game, congratulations!\n\nYour reflexes were faster and more accurate than " + zScore.ToString("F0") + "% of people. Well done!\n\n<size=70%>But can you do better? Click Restart to try again, or Continue to move to the next part of the experiment.";
        scoreScript.AnimateScore((int)zScore);
    }



    static double PercentileNormCDF(int percentScore) { // Phi
        // Percentile function Polynomial approximation in c# - https://stackoverflow.com/questions/1662943/standard-normal-distribution-z-value-function-in-c-sharp
        double x = Convert.ToDouble(percentScore); // Convert score to double
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;
            
        // Save the sign of x
        x = (x - mean) / sd; // Normalise to z score
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Math.Abs(x) / Math.Sqrt(2.0); // Absolute X scaled by sqrt2
            
        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p*x); // Compute t
        double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x); // Polynomial approximation of error function
        
        //output
        double pecentile = 0.5 * (1.0 + sign*y);
        double percent = pecentile * 100;
        return percent;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Send experimental data
        DataManager.Instance.SendData();
        // Clear trials ahead of repeat - probably just do this on a 'repeat' button listener
        //data.ClearTrials();
        DisplayRelativeScore();
    }
}
