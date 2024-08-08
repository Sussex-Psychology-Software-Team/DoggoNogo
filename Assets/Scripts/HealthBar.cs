using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public void SetMinHealth(int health){
		slider.minValue = health;
		fill.color = gradient.Evaluate(1f);
	}

	public void SetMaxHealth(int health){
		slider.maxValue = health;
		fill.color = gradient.Evaluate(1f);
	}

	public float GetMaxHealth(){
		return slider.maxValue;
	}

	public float GetMinHealth(){
		return slider.minValue;
	}

    public void SetHealth(float health){
		slider.value = health;
	}

	public void setColour(Color c){
		fill.color = c;
	}
}