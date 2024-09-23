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
    // Average score distribution params
    public static double mean = 4000.0;
    public static double sd = 2000.0;

    
    // Start is called before the first frame update
    void Start()
    {
        // Send experimental data
        DataManager.Instance.sendData();
        // Clear trials ahead of repeat - probably just do this on a 'repeat' button listener
        //data.ClearTrials();
        displayRelativeScore();
    }

    // Score display functions
    void displayRelativeScore(){
        // Calculate %
        int score = DataManager.Instance.data.currentTrial().totalScore; //get copy of player score
        Debug.Log(score);
        double zScore = percentileNormCDF(score); // Score under normal as %
        Debug.Log(zScore);
        // Change text and Healthbar
        percentileText.text = "You scored better than " + zScore.ToString("F0") + "% of participants!";
        StartCoroutine(scoreAnimator((int)zScore));
    }

    IEnumerator scoreAnimator(int percentScore){
        for (int s=0; s<=percentScore; s++){ //note can make smoother by making SetHealth take a float.
            scoreBar.SetHealth(s);
            yield return new WaitForSeconds(.01f);
        }
    }

    static double percentileNormCDF(int percentScore) { // Phi
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
}
