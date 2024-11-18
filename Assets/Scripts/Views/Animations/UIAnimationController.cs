using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    [SerializeField] private float defaultFadeDuration = 0.3f;

    public async Task FadeIn(GameObject target)
    {
        await Fade(target, 0f, 1f);
    }
    
    public async Task FadeOut(GameObject target)
    {
        await Fade(target, 1f, 0f);
    }

    public async Task MoveVertical(RectTransform rect, float targetY, float duration)
    {
        float startY = rect.anchoredPosition.y;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            float newY = Mathf.Lerp(startY, targetY, percent);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, newY);
            
            await Task.Yield();
        }

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, targetY);
    }

    public async Task FadeGraphic(Graphic graphic, float duration)
    {
        var color = graphic.color;
        color.a = 0;
        graphic.color = color;

        while (graphic.color.a < 1.0f)
        {
            color.a = Mathf.Min(color.a + (Time.deltaTime / duration), 1f);
            graphic.color = color;
            await Task.Yield();
        }
    }
    
    private async Task Fade(GameObject target, float startAlpha, float endAlpha)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = target.AddComponent<CanvasGroup>();

        float elapsed = 0;
        canvasGroup.alpha = startAlpha;

        while (elapsed < defaultFadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / defaultFadeDuration);
            await Task.Yield();
        }

        canvasGroup.alpha = endAlpha;
    }
}