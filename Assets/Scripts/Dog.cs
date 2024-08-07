using System; // Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image.sprite

public class Dog : MonoBehaviour
{
    public Sprite[] images; // Array of images of each evolution, increase on level change
    private int i = 0; // image number
    // Audio feeback
    public AudioSource dogBark; // Barking on early press
    public AudioSource dogChew; // Chewing on fast (i.e. valid) trial
    // Whining on miss (after stim dissapears). Choose random one
    public AudioSource dogWhine1; 
    public AudioSource dogWhine2;

    public void NextSprite(){
        // Loops through sprites automatically
        gameObject.GetComponent<Image>().sprite = images[++i]; // Note first image just loaded automatically
    }

    public void whine(){
        System.Random rand = new System.Random(); // Random var
        if(rand.Next(2) == 1) dogWhine1.Play(); // .Next() gets new random number, (2) non-inclusive maximum int = 0 or 1
        else dogWhine2.Play();
    }

    public void chew(){
        dogChew.Play();
    }

    public void bark(){
        dogBark.Play();
    }
}
