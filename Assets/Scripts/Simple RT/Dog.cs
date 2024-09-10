using System; // Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image.sprite

public class Dog : MonoBehaviour
{
    // Images
    public Sprite[] images; // Array of images of each evolution, increase on level change

    // Damage animation
    Image image; // save reference to image
    public float flickerDuration = 1.0f;
    public float flickerInterval = 0.2f;
    public float shakeAmount = 3.0f;
    bool takingDamage = false;
    // Jumping
    public int maxJumpHeight = 40;
    public int jumpSpeed = 250;
    float startingY; // Initial starting point - ground
    float startingX; // For putting back to original X position after shake
    float yPosition;
    bool ascending = false;
    bool descending = false;

    // Audio feeback
    public AudioSource dogBark; // Barking on early press
    public AudioSource dogChew; // Chewing on fast (i.e. valid) trial
    // Whining on miss (after stim dissapears). Choose random one
    public AudioSource dogWhine1; 
    public AudioSource dogWhine2;


    // FUNCTIONS ********************************
    // Images
    public void GetSprite(int level){
        // Loops through sprites automatically
        if ((level-1) < images.Length) {
            image.sprite = images[level-1]; // Note first image just loaded automatically
        } else {
            Debug.LogError("Out of range error in NextSprite");
        }
    }

    public void takeDamage(){ // Flicker and shake coroutine wrapper.
        if(!takingDamage) StartCoroutine(shakeRed()); // If not current running flicker and shake, do
    }

    IEnumerator shakeRed(){
        takingDamage = true; // Stop running script multiple times at once
        float endTime = Time.time + flickerDuration; // How long to run loops of function
        bool flickerToggle = false; // Turns colour flicker (consider shake too?) on and off repeatedly
        // Flickering function
        while (Time.time < endTime){
            image.color = flickerToggle ? Color.red : Color.white;
            // Apply 2D shaking effect
            transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-shakeAmount, shakeAmount), UnityEngine.Random.Range(-shakeAmount, shakeAmount), 0);
            flickerToggle = !flickerToggle; // Flip state of colour flicker
            yield return new WaitForSeconds(flickerInterval);
        }
        // Return to original state
        image.color = Color.white; // White is equal to original colour if left unaltered
        transform.position = new Vector3(startingX, yPosition, 0); // Y position also relevant to jump
        takingDamage = false; // Allow function to run again
    }

    // Jumping
    void jump(){ // Simple jump to avoid unity physics engine
        if(ascending){
            // // If not at max height increase
            if(yPosition <= startingY+maxJumpHeight) yPosition += jumpSpeed * Time.deltaTime; //Time.deltaTime ensures this is not FPS dependent as Update called very regularly
            else { // Else start descent
                ascending = false;
                descending = true;
            }
        } else if(descending){
            if(yPosition > startingY) yPosition -= jumpSpeed * Time.deltaTime; // If not grounded
            else descending = false; // If grounded stop descent
        }
        // Change position
        transform.position = new Vector3(transform.position.x, yPosition, 0);
    }

    public void startJump(int jumpHeight){
        maxJumpHeight = jumpHeight;
        ascending = true;
    }

    // Audio
    public void whine(){ // Random dog whine
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

    // UNITY ************************************
    void Start(){
        // Save starting position
        startingY = transform.position.y; // For jump
        startingX = transform.position.x; // For shake
        yPosition = transform.position.y; // Shake and jump
        // Image reference for sprite swap and colour
        image = gameObject.GetComponent<Image>();
    }

    void Update(){
        // Initiate jump on down arrow
        // if(Input.GetKeyDown(KeyCode.DownArrow)) ascending = true;
        if(ascending || descending) jump();
    }
}
