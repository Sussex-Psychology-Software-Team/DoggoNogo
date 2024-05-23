using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //for TextMeshProUGUI

public class EndScreen : MonoBehaviour
{
    public HealthBar scoreBar;

    private int score;
    public static double mean = 7.0;
    public static double sd = 2.0;
    public double max_health;
    public TextMeshProUGUI percentileText; // displays score
    
    // Start is called before the first frame update
    void Start()
    {
        max_health = mean + (sd*3);
        scoreBar.SetMaxHealth((int)max_health); // figure out what this should be - obviously a maximum of the actual user score is somewhat needed.
        score = PlayerPrefs.GetInt("Score", 0); //get local copy of player score
        StartCoroutine(scoreAnimator());
        string z = Phi(score);
        percentileText.text = "You scored better than " + z + "% of participants!";
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(score);
    }

    IEnumerator scoreAnimator(){
        for (int s=0; s<=score; s++){ //note can make smoother by making SetHealth take a float.
            scoreBar.SetHealth(s);
            if(s==score && s<max_health){
                //Debug.Log("YAY!");
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public static string Phi(int x_score) {
        //quantile/percentile function in c# - https://stackoverflow.com/questions/1662943/standard-normal-distribution-z-value-function-in-c-sharp
        double x = Convert.ToDouble(x_score);
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
        string trunc = percent.ToString("F2");
        return trunc;
    }
}
