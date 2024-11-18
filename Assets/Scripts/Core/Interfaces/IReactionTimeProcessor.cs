public interface IReactionTimeProcessor
{
    string DetermineResponseType(double rt);
    void UpdateMedianRT(double rt);
    double GetCurrentThreshold();
}