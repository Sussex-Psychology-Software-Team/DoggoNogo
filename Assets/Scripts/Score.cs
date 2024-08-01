using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public HealthBar healthBar; //holds reference to coloured health bar
    public score = 0;

   // SCORE -------------------------------------
    public void Change(int change, string feedback){
        // set colours
        Color forest = new Color(0.06770712f, 0.5817609f, 0f, 1f); //colour of positive feedback text
        Color barColour;
        if(change<0){ //if score is being reduced
            barColour = Color.red;
            feedbackText.color = Color.red;
        } else if(change == 1){
            barColour = forest;
            feedbackText.color = Color.white;
        } else {
            barColour = forest;
            feedbackText.color = Color.white;
        }
        // Update UI 
        scoreText.text = "Score: " + newScore;
        feedbackText.text = feedback;
    }

    void ratchetScore(int change, Colour){
        // Update healthbar but comparing score to ratchet
        float maxHealth = healthBar.GetMaxHealth();
        float healthBarScore = (float)(score + change);
        
        //ratchet healthbar separately
        float health_bar_ratchet = maxHealth*(((float)stage-1f)/3f);
        if(healthBarScore < health_bar_ratchet){ 
            healthBarScore = health_bar_ratchet;
        }
        healthBar.SetHealth((int)healthBarScore, barColour);

        //compare new score to ratchet
        int newScore = this.score + change;
        if(newScore < score_ratchet[stage-1]){
            newScore = score_ratchet[stage-1];
        }
        // Update data
        DataManager.Instance.data.currentTrial().score = newScore; //take score out of global var soon
        score = newScore;

    }

    // PUT NEW SCORE CALCULATIONS IN HERE
    public void Get(double rt, double min_rt=.2, double max_rt=.6, double min_score=.1, double max_score = .2) {
        // Calculate score
        double final_score;
        string text;
        if(rt<max_rt){ // if not way too slow
            // calculate realtive score
            double clamped = Math.Clamp(rt, min_rt, max_rt); //clamp rt between ranges
            double relative = (clamped - min_rt) / (max_rt - min_rt); //normalise as proportion of range
            double reversed = (1 - relative); //reverse
            // score bonus
            double max_score_bonus = max_score - min_score;
            double score_bonus = reversed * max_score_bonus;
            double min_add = min_score + score_bonus;
            if (min_add > max_score) { min_add = max_score; } //clamp
            final_score = min_add*1000;
            text = "GREAT!\nDoggo caught the sausage!";
        } else { //if too slow
            final_score = 0;
            text = "Oh no!\nDoggo didn't get a sausage.";
        }
        Debug.Log(final_score);

        //finally handle changing the score
        changeScore((int)final_score, text);
    }

    // Calculate score where participant will enter into new stage of game
    public void Target(){
        //n_trials and n_trials_stage1 are defined up top
        int n_trials_stage2 = 0;
        if(stage==1){ n_trials_stage1 = DataManager.Instance.data.trials.Count;
        } else if(stage==2){ n_trials_stage2 = DataManager.Instance.data.trials.Count; }
        stage++;
        if(stage==4){
            endTask();
            return;
        }
        dog.ChangeSprite();
        int target = ((n_trials - (n_trials_stage1 + n_trials_stage2)) / (8-(2*stage)) )*100;
        if(target<500){ target=500; }
        score_ratchet[stage] = score_ratchet[stage-1]+target;
        float new_health = (float)score_ratchet[stage] * (1f/ (float)stage )*3f;
        healthBar.SetMaxHealth((int)new_health); //set healthbar maximum
        Debug.Log("New Max health: ");
        Debug.Log((1/stage)*3);
        Debug.Log(score_ratchet[stage]*(1/stage)*3);
    }

}
