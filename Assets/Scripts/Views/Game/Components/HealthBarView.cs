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

    private float _currentHealth;
    private float _targetHealth;
    private float _minHealth;
    private float _maxHealth;

    private void Awake()
    {
        ResetHealth();
    }

    public void SetNewTarget(int target)
    {
        _maxHealth = target;
        _minHealth = 0;
        ResetHealth();
    }

    public void SetHealth(float health)
    {
        _targetHealth = Mathf.Clamp(health, _minHealth, _maxHealth);
        
        // Smoothly animate to new health value
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateHealthChange());
        }
        else
        {
            _currentHealth = _targetHealth;
            UpdateFillAmount();
        }
    }

    public void SetColour(Color color)
    {
        fillImage.color = color;
    }

    private void ResetHealth()
    {
        _currentHealth = 0;
        _targetHealth = 0;
        UpdateFillAmount();
    }

    private void UpdateFillAmount()
    {
        float normalizedHealth = Mathf.InverseLerp(_minHealth, _maxHealth, _currentHealth);
        fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
    }

    private System.Collections.IEnumerator AnimateHealthChange()
    {
        float elapsed = 0f;
        float startHealth = _currentHealth;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Use smoothstep for more natural animation
            t = t * t * (3f - 2f * t);
            
            _currentHealth = Mathf.Lerp(startHealth, _targetHealth, t);
            UpdateFillAmount();
            
            yield return null;
        }

        _currentHealth = _targetHealth;
        UpdateFillAmount();
    }

    public float GetMinHealth() => _minHealth;
    public float GetMaxHealth() => _maxHealth;
}