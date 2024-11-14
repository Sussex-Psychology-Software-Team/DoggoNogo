using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    
    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float animationDuration = 0.5f;

    private float currentHealth;
    private float targetHealth;
    private float minHealth;
    private float maxHealth;

    private void Awake()
    {
        ResetHealth();
    }

    public void SetNewTarget(int target)
    {
        maxHealth = target;
        minHealth = 0;
        ResetHealth();
    }

    public void SetHealth(float health)
    {
        targetHealth = Mathf.Clamp(health, minHealth, maxHealth);
        
        // Smoothly animate to new health value
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateHealthChange());
        }
        else
        {
            currentHealth = targetHealth;
            UpdateFillAmount();
        }
    }

    public void SetColour(Color color)
    {
        fillImage.color = color;
    }

    private void ResetHealth()
    {
        currentHealth = 0;
        targetHealth = 0;
        UpdateFillAmount();
    }

    private void UpdateFillAmount()
    {
        float normalizedHealth = Mathf.InverseLerp(minHealth, maxHealth, currentHealth);
        fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
    }

    private System.Collections.IEnumerator AnimateHealthChange()
    {
        float elapsed = 0f;
        float startHealth = currentHealth;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Use smoothstep for more natural animation
            t = t * t * (3f - 2f * t);
            
            currentHealth = Mathf.Lerp(startHealth, targetHealth, t);
            UpdateFillAmount();
            
            yield return null;
        }

        currentHealth = targetHealth;
        UpdateFillAmount();
    }

    public float GetMinHealth() => minHealth;
    public float GetMaxHealth() => maxHealth;
}