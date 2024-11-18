using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandingScreenView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject instructions;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private RectTransform titleCardRect;
    [SerializeField] private Image titleCardImage;
    [SerializeField] private UIAnimationController animationController;

    private async void Start()
    {
        await PlayIntroAnimation();
    }

    private async Task PlayIntroAnimation()
    {
        await animationController.FadeGraphic(titleCardImage, 1f);
        await Task.Delay(1000); // 1 second
        await animationController.MoveVertical(titleCardRect, 328f, 1f);
        
        instructions.SetActive(true);
        await animationController.FadeGraphic(instructionsText, 1f);
        await Task.Delay(2000); // 2 seconds
        
        startButton.SetActive(true);
    }
}