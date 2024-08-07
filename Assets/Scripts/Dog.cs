using System; // Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image.sprite

public class Dog : MonoBehaviour
{
    // Images
    public Sprite[] images; // Array of images of each evolution, increase on level change
    private int i = 0; // image number
    // Jumping
    public int maxJumpHeight = 40;
    public int jumpSpeed = 250;

    // Audio feeback
    public AudioSource dogBark; // Barking on early press
    public AudioSource dogChew; // Chewing on fast (i.e. valid) trial
    // Whining on miss (after stim dissapears). Choose random one
    public AudioSource dogWhine1; 
    public AudioSource dogWhine2;


    // FUNCTIONS ********************************
    // Images
    public void NextSprite(){
        // Loops through sprites automatically
        gameObject.GetComponent<Image>().sprite = images[++i]; // Note first image just loaded automatically
    }

    // Audio
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

    // Jumping
    float startingY; // Initial starting point - ground
    float yPosition;
    bool ascending = false;
    bool descending = false;

    void jump(){
        if(ascending){
            if(yPosition <= startingY+maxJumpHeight) yPosition += jumpSpeed * Time.deltaTime;
            else{
                ascending = false;
                descending = true;
            }
        } else if(descending){
            if(yPosition > startingY) yPosition -= jumpSpeed * Time.deltaTime;
            else descending = false;
        }
        // Change position
        transform.position = new Vector3(transform.position.x, yPosition, 0);
    }

    void Start(){
        startingY = transform.position.y;
        yPosition = transform.position.y;
    }

    void Update(){
        // Initiate jump on down arrow
        if(Input.GetKeyDown(KeyCode.DownArrow)) ascending = true;
        jump();
    }
}
