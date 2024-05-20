using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public HealthBar scoreBar;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        scoreBar.SetMaxHealth(15); // figure out what this should be - obviously a maximum of the actual user score is somewhat needed.
        score = PlayerPrefs.GetInt("Score", score); //get local copy of player score
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(score);
        StartCoroutine(scoreAnimator());
    }

    IEnumerator scoreAnimator(){
        for (int s=0; s<=score; s++){
            scoreBar.SetHealth(s);
            yield return null;
        }
    }
}
