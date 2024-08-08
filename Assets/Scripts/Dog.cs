using System; // Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image.sprite

public class Dog : MonoBehaviour
{
    // Images
    public Sprite[] images; // Array of images of each evolution, increase on level change
    int i = 0; // image number
    // Damage animation
    Image image; // save reference to image
    Color originalColour; // save reference to image colour
    Vector3 originalPosition;
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
    public void NextSprite(){
        // Loops through sprites automatically
        image.sprite = images[++i]; // Note first image just loaded automatically
    }

    public void takeDamage(){
        if(!takingDamage) StartCoroutine(shakeRed());
    }

    IEnumerator shakeRed(){
        takingDamage = true;
        float endTime = Time.time + flickerDuration;
        bool flickerToggle = false;
        // Flickering function
        while (Time.time < endTime){
            image.color = flickerToggle ? Color.red : originalColour;
            // Apply 2D shaking effect
            transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-shakeAmount, shakeAmount), UnityEngine.Random.Range(-shakeAmount, shakeAmount), 0);
            flickerToggle = !flickerToggle;
            yield return new WaitForSeconds(flickerInterval);
        }
        // Return to original colour
        image.color = originalColour; // or just Color.white?
        transform.position = new Vector3(startingX, yPosition, 0);
        takingDamage = false;
    }

    // Jumping
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

    // UNITY ************************************
    void Start(){
        // Save starting position
        startingY = transform.position.y;
        startingX = transform.position.x;
        yPosition = transform.position.y;
        // Save sprite colour
        image = gameObject.GetComponent<Image>();
        originalColour = image.color;
        originalPosition = transform.localPosition;
    }

    void Update(){
        // Initiate jump on down arrow
        if(Input.GetKeyDown(KeyCode.DownArrow)) ascending = true;
        jump();
    }
}
