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
    public Image image; // save reference to image
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
    // Surprised on slow
    public AudioSource dogSurprised; 


    // FUNCTIONS ********************************
    // Images
    public void GetSprite(int level){
        // Loops through sprites automatically
        int newLevel = level-1;
        if (newLevel < images.Length) {
            image.sprite = images[newLevel]; // Note first image just loaded automatically
            // Set new image to correct aspect ratio
            image.rectTransform.sizeDelta = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
        } else {
            Debug.LogError("Out of range error in NextSprite");
        }
    }

    public void TakeDamage(){ // Flicker and shake coroutine wrapper.
        // If not current running flicker and shake, do
        if(!takingDamage){
            startingX = transform.localPosition.x; // For shake
            yPosition = transform.localPosition.y; // Shake and jump
            StartCoroutine(ShakeRed());
        }
    }

    IEnumerator ShakeRed(){
        takingDamage = true; // Stop running script multiple times at once
        float endTime = Time.time + flickerDuration; // How long to run loops of function
        bool flickerToggle = false; // Turns colour flicker (consider shake too?) on and off repeatedly
        // Flickering function
        while (Time.time < endTime){
            image.color = flickerToggle ? Color.red : Color.white;
            // Apply 2D shaking effect
            transform.localPosition = transform.localPosition + new Vector3(UnityEngine.Random.Range(-shakeAmount, shakeAmount), UnityEngine.Random.Range(-shakeAmount, shakeAmount), 0);
            flickerToggle = !flickerToggle; // Flip state of colour flicker
            yield return new WaitForSeconds(flickerInterval);
        }
        // Return to original state
        image.color = Color.white; // White is equal to original colour if left unaltered
        transform.localPosition = new Vector3(startingX, yPosition, 0); // Y position also relevant to jump
        takingDamage = false; // Allow function to run again
    }

    // Audio
    public void whine(){ // Random dog whine
        System.Random rand = new System.Random(); // Random var
        if(rand.Next(2) == 1) dogWhine1.Play(); // .Next() gets new random number, (2) non-inclusive maximum int = 0 or 1
        else dogWhine2.Play();
    }

    public void Chew(){
        dogChew.Play();
    }

    public void Bark(){
        dogBark.Play();
    }

    public void Surprised(){
        dogSurprised.Play();
    }

    // Jumping Coroutine
    IEnumerator JumpCoroutine(){
        // While ascending
        while (ascending) {
            if (yPosition <= startingY + maxJumpHeight) {
                yPosition += jumpSpeed * Time.deltaTime;
            } else {
                ascending = false;
                descending = true;
            }

            UpdatePosition();
            yield return null; // Wait for the next frame
        }

        // While descending
        while (descending) {
            if (yPosition > startingY) {
                yPosition -= jumpSpeed * Time.deltaTime;
            } else {
                descending = false;
            }

            UpdatePosition();
            yield return null; // Wait for the next frame
        }
    }

    // Update the object position
    private void UpdatePosition(){
        transform.localPosition = new Vector3(transform.localPosition.x, yPosition, 0);
    }

    // Start the jump process
    public void StartJump(int jumpHeight) {
        if(!ascending && !descending){ //only one jump at a time
            startingY = transform.localPosition.y;
            yPosition = transform.localPosition.y; // Shake and jump
            maxJumpHeight = jumpHeight;
            ascending = true;
            StartCoroutine(JumpCoroutine());
        }
    }

    // UNITY ************************************
    // void Update(){
    //     // Trigger jump on pressing Down Arrow key
    //     if (Input.GetKeyDown(KeyCode.DownArrow)){
    //         startJump(40); // Example jump height of 3
    //     }
    // }

}
