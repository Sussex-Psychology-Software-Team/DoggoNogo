using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    public IEnumerator FadeIn(Graphic graphic, float duration=0.3f)
    {
        yield return StartCoroutine(Fade(graphic, 0f, 1f, duration));
    }

    public IEnumerator FadeOut(Graphic graphic, float duration=0.3f)
    {
        yield return StartCoroutine(Fade(graphic, 1f, 0f, duration));
    }
    
    public IEnumerator MoveVertical(RectTransform rect, float targetY, float duration)
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

        // Ensure we end up exactly at the target position
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, targetY);
    }

    private static IEnumerator Fade(Graphic graphic, float startAlpha, float endAlpha, float duration=0.3f)
    {
        var color = graphic.color;
        color.a = startAlpha;
        graphic.color = color;

        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            graphic.color = color;
            yield return null;
        }

        color.a = endAlpha;
        graphic.color = color;
    }
    
}