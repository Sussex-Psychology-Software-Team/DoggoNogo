using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public const string LandingScene = "Landing";
    public const string IntroScene = "Introduction";
    public const string Level1 = "Simple RT";
    public const string EndScene = "End";
    
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}