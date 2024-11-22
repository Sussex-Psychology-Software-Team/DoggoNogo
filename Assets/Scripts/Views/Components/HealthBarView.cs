using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    
    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
        SetHealth(slider.value);
    }

    // Note consider a smoother animation on health changes
    public void SetHealth(float value)
    {
        slider.value = value;
    }

    public void SetColor(Color color)
    {
        fillImage.color = color;
    }
    
}