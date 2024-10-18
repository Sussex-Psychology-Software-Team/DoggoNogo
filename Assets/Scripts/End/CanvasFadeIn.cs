using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 2.0f;

    void Start()
    {
        // Start with the canvas fully transparent
        canvasGroup.alpha = 0f;

        // Start fading in
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Lerp the alpha value from 0 to 1 over fadeDuration time
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);

            // Wait until the next frame
            yield return null;
        }

        // Ensure the alpha is set to 1 at the end
        canvasGroup.alpha = 1f;
    }
}
