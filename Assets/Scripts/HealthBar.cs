using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public void SetMaxHealth(int health){
		slider.maxValue = health;
		fill.color = gradient.Evaluate(1f);
	}

	public float GetMaxHealth(){
		return slider.maxValue;
	}

    public void SetHealth(int health,  Color? c = null){
		slider.value = health;
		fill.color = c ?? gradient.Evaluate(slider.normalizedValue);
	}
}