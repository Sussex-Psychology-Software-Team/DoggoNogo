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
        // Get previous healthbar maximum
        int previousMaximum = (int)currentHealthBar.GetMaxHealth(); // Minimum set to last healthbar's maximum
        // Fill current healthbar
        currentHealthBar.SetHealth(previousMaximum);
        // Change healthbar from array
        currentHealthBar = healthBarList[level-1];
        // Set min and max for new healthbar
        currentHealthBar.SetMinHealth(previousMaximum); // Healthbar minimum to last healthbar's maximum
        currentHealthBar.SetMaxHealth(scoreManager.GetNewTargetScore()); // Set healthbar maximum
    }

    // Start is called before the first frame update
    void Start() {
        currentHealthBar.SetMaxHealth(scoreManager.GetNewTargetScore());
    }
}
