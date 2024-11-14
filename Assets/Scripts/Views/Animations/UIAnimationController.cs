using UnityEngine;
using System.Threading.Tasks;

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
    
    private async Task Fade(GameObject target, float startAlpha, float endAlpha)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
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
