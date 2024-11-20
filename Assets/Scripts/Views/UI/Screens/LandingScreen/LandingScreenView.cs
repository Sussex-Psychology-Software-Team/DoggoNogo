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

    private void Start()
    {
        StartCoroutine(PlayIntroAnimation());
    }

    private IEnumerator PlayIntroAnimation()
    {
        yield return StartCoroutine(GameController.Instance.Animations.FadeIn(titleCardImage, 1f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameController.Instance.Animations.MoveVertical(titleCardRect, 328f, 1f));
    
        instructions.SetActive(true);
        yield return StartCoroutine(GameController.Instance.Animations.FadeIn(instructionsText, 1f));
        yield return new WaitForSeconds(2f);
    
        startButton.SetActive(true);
    }

}