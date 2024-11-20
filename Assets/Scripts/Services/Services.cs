public static class Services
{
    public static UIAnimationController Animation => GameController.Instance.Animations;
    
    // Can add other commonly accessed services
    public static DataController Data => DataController.Instance;
    // etc.
}