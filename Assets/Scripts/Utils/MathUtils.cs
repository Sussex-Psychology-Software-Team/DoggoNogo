using System;

public static class MathUtils
{
    private static readonly double[] Coefficients = { 0.254829592, -0.284496736, 1.421413741, -1.453152027, 1.061405429 };
    private const double P = 0.3275911;

    public static double CalculatePercentileNormCDF(double x)
    {
        x = (x - Constants.Statistics.NormMean) / Constants.Statistics.NormSD;
        int sign = x < 0 ? -1 : 1;
        x = Math.Abs(x) / Math.Sqrt(2.0);
        
        double t = 1.0 / (1.0 + P * x);
        double y = 1.0 - CalculatePolynomial(t) * Math.Exp(-x * x);
    
        return 50.0 * (1.0 + sign * y);
    }

    private static double CalculatePolynomial(double t)
    {
        double result = Coefficients[4];
        for (int i = 3; i >= 0; i--)
        {
            result = result * t + Coefficients[i];
        }
        return result * t;
    }
}