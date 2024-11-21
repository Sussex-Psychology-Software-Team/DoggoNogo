using UnityEngine;

public class Level1IntroController : MonoBehaviour
{    
    private void OnEnable()
    {
        Level1Events.OnIntroComplete += HandleIntroComplete;
    }

    private void OnDisable()
    {
        Level1Events.OnIntroComplete -= HandleIntroComplete;
    }

    public void StartIntro()
    {
        Level1Events.IntroStarted();
    }

    private void HandleIntroComplete()
    {
        Level1Controller.Instance.StartLevel();
    }
}