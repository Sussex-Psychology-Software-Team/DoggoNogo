using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Intro to Level1
// do I need dog image or just dogView reference?
public class Level1IntroductionView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image dogImage;
    [SerializeField] private GameObject woodenSign;
    [SerializeField] private TextMeshProUGUI continueText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float swingDuration = 4f;
    [SerializeField] private float swingFrequency = 3f;
    [SerializeField] private float swingDampening = 3f;

    private bool _viewingInstructions = true; // Toggle to stop double presses of spacebar
    
    public void Initialize()
    {
        StartCoroutine(IntroSequence());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator IntroSequence()
    {
        yield return StartCoroutine(UIAnimationController.Instance.FadeIn(dogImage, fadeInDuration));
        woodenSign.SetActive(true); // technically should be handled by ViewManager - but here so it doesn't appear to early
        yield return StartCoroutine(Swing(woodenSign.GetComponent<RectTransform>()));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(UIAnimationController.Instance.FadeIn(continueText, 1f));
    }
    
    private void Update()
    {
        if (_viewingInstructions && Input.GetKeyDown(KeyCode.Space))
        {
            CompleteIntro();
        }
    }

    private void CompleteIntro()
    {
        _viewingInstructions = false;
        woodenSign.SetActive(false);
        Level1Events.IntroComplete(); // Triggers OnIntroComplete Event loaded in L1IntroController, 
    }
    
    // Swing Animation - move to separate location!
    private IEnumerator Swing(RectTransform uiElement)
    {
        float elapsed = 0f;
        float startAngle = 90f;
    
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / swingDuration;
        
            float angle = startAngle * 
                          Mathf.Exp(-swingDampening * normalizedTime) * 
                          Mathf.Cos(swingFrequency * Mathf.PI * normalizedTime);
        
            uiElement.localRotation = Quaternion.Euler(angle, 0f, 0f);
        
            yield return null;
        }
    
        uiElement.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
    
    
}