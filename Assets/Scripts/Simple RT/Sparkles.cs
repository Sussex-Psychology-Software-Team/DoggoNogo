using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkles : MonoBehaviour
{
    public ParticleSystem sparkles;

    public void Sparkle(){
        StartCoroutine(PlaySparkles(3.0f));  // 5 seconds
    }

    IEnumerator PlaySparkles(float duration){
        // Play the particle system
        sparkles.Play();

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Stop the particle system
        sparkles.Stop();
    }

}
