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
        scoreBar.SetMaxHealth(10); // figure out what this should be - obviously a maximum of the actual user score is somewhat needed.
        score = PlayerPrefs.GetInt("Score", 0); //get local copy of player score
        StartCoroutine(scoreAnimator());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(score);
    }

    IEnumerator scoreAnimator(){
        for (int s=0; s<=score; s++){ //note can make smoother by making SetHealth take a float.
            scoreBar.SetHealth(s);
            if(s==score){
                Debug.Log("YAY!");
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
