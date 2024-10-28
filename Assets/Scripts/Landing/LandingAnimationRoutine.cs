using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image
using TMPro; //for TextMeshProUGUI

public class LandingAnimationRoutine : MonoBehaviour
{
    public GameObject instructions;
    public TextMeshProUGUI instructionsText;
    public GameObject button;
    public RectTransform titleCardRect;
    public Image titleCardImage;

    IEnumerator MoveAnimation(RectTransform rect, float targetY, float duration)
    {
        float startY = rect.anchoredPosition.y;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            float newY = Mathf.Lerp(startY, targetY, percent);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, newY);
            
            yield return null;
        }

        // Set final position
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, targetY);
    }

    IEnumerator FadeIn(float seconds, Graphic graphic){
        // Graphic.color returns copy not reference so can't be set directly
        Color originalColor = graphic.color; // Get the current color
        originalColor.a = 0; // Set alpha to 0
        graphic.color = originalColor; // Put back in image

        // While alpha is not full
        while (graphic.color.a < 1.0f) {
            // Calculate new alpha
            originalColor.a = graphic.color.a + (Time.deltaTime / seconds); // Turn up Alpha by time elapsed
            graphic.color = originalColor; // put back in colour
            yield return null;
        }
    }

    IEnumerator AnimationChain(){
        yield return StartCoroutine(FadeIn(1f, titleCardImage));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoveAnimation(titleCardRect, 328f, 1f));
        instructions.SetActive(true);
        yield return StartCoroutine(FadeIn(1f, instructionsText));
        yield return new WaitForSeconds(2f);
        button.SetActive(true);
    }

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(AnimationChain());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
