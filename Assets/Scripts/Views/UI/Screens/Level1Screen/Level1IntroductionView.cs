using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Intro to Level1
// do I need dog image or just dogView reference?
public class Level1IntroductionView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject instructions;
    [SerializeField] private RectTransform instructionsRect;
    [SerializeField] private GameObject scoreCard;
    [SerializeField] private Image dogImage;
    [SerializeField] private TextMeshProUGUI continueText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float swingDuration = 4f;
    [SerializeField] private float swingFrequency = 3f;
    [SerializeField] private float swingDampening = 3f;
    
    private UIAnimationController _animationController;
    private bool _viewingInstructions = true;
    private bool _allowContinue = false;
    

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private void Update()
    {
        if (_allowContinue && _viewingInstructions && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FadeIn(fadeInDuration, dogImage));
        instructions.SetActive(true);
        yield return StartCoroutine(Swing(instructionsRect));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(FadeIn(1f, continueText));
        _allowContinue = true;
    }

    private void StartGame()
    {
        instructions.SetActive(false);
        scoreCard.SetActive(true);
        _viewingInstructions = false;
        Level1Controller.Instance.StartLevel();
    }

    private IEnumerator FadeIn(float duration, Graphic graphic)
    {
        Color color = graphic.color;
        color.a = 0;
        graphic.color = color;

        while (graphic.color.a < 1.0f)
        {
            color.a = Mathf.Min(color.a + (Time.deltaTime / duration), 1f);
            graphic.color = color;
            yield return null;
        }
    }

    private IEnumerator Swing(RectTransform uiElement)
    {
        float elapsed = 0f;
        float startAngle = 90f;
        Vector3 originalRotation = uiElement.localEulerAngles;
        
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / swingDuration;
            
            float angle = startAngle * 
                         Mathf.Exp(-swingDampening * normalizedTime) * 
                         Mathf.Cos(swingFrequency * Mathf.PI * normalizedTime);
            
            uiElement.localRotation = Quaternion.Euler(
                originalRotation.x + angle,
                originalRotation.y,
                originalRotation.z
            );
            
            yield return null;
        }
        
        uiElement.localRotation = Quaternion.Euler(originalRotation);
    }
}