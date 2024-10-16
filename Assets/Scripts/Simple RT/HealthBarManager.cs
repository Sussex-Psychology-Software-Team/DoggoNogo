using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public List<HealthBar> healthBarList; // List of all 3 healthbars
    public HealthBar currentHealthBar;
    public ScoreManager scoreManager; // For getting calculation of new total score

    // Switch healthbars
    public void SetNewHealthBar(int level, int targetScore){
        // Fill current healthbar
        currentHealthBar.SetHealth(currentHealthBar.GetMaxHealth());
        // Change healthbar from array
        currentHealthBar = healthBarList[level-1];
        // Set min and max for new healthbar
        currentHealthBar.SetMaxHealth(targetScore); // Set healthbar maximum
        currentHealthBar.SetColour(new Color(0.06770712f, 0.5817609f, 0f, 1f)); // "forest"
    }

    // Start is called before the first frame update
    void Start() {
        currentHealthBar.SetMaxHealth(scoreManager.GetNewTargetScore());
    }
}
