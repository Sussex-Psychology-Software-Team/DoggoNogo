using System;

public class Calculations
{
    // Average score distribution params
    public static double normMean = 0.30;
    public static double normSD = 0.0304;
    
    public static double PercentileNormCDF(double x) { // Phi
        // Percentile function Polynomial approximation in c# - https://stackoverflow.com/questions/1662943/standard-normal-distribution-z-value-function-in-c-sharp
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;
        
        // Save the sign of x
        x = (x - normMean) / normSD; // Normalise to z score
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
