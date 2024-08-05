using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //for TextMeshProUGUI

public class EndScreen : MonoBehaviour
{
    public HealthBar scoreBar;

    public static double mean = 4000.0;
    public static double sd = 1000.0;
    public TextMeshProUGUI percentileText; // displays score
    
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.sendData();
        int score = DataManager.Instance.data.lastTrial().totalScore; //get local copy of player score
        double percent = Phi(score);
        string z = percent.ToString("F2");
        percentileText.text = "You scored better than " + z + "% of participants!";
        StartCoroutine(scoreAnimator((int)percent));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(score);
    }

    IEnumerator scoreAnimator(int percent_score){
        for (int s=0; s<=percent_score; s++){ //note can make smoother by making SetHealth take a float.
            scoreBar.SetHealth(s);
            if(s==percent_score && s<100){
                //Debug.Log("YAY!");
            }
            yield return new WaitForSeconds(.01f);
        }
    }

    public static double Phi(int percent_score) {
        //quantile/percentile function in c# - https://stackoverflow.com/questions/1662943/standard-normal-distribution-z-value-function-in-c-sharp
        double x = Convert.ToDouble(percent_score);
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;
            
        // Save the sign of x
        x = (x - mean) / sd;
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Math.Abs(x) / Math.Sqrt(2.0);
            
        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p*x);
        double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x);
        
        //output
        double p_score = 0.5 * (1.0 + sign*y);
        double percent = p_score*100;
        return percent;
    }
}
