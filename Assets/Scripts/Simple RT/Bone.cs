using System;
using System.Collections; // For IEnumerator
using UnityEngine;
using UnityEngine.UI; // Image namespace
using TMPro; // TextMeshProUGUI

public class Bone : MonoBehaviour
{
    // Refs
    public Image image;
    public TextMeshProUGUI feedbackText; // Defines top bound of bone position
    public Image dogImage;
    public AudioSource boneThrow; // Barking on early press

    // Hide
    public void Hide(){
        image.enabled = false; // Hide bone
    }

    public bool Hidden(){
        return image.enabled == false; // Is bone hidden
    }

    // Show
    public void Show(){
        randomTransform(); // Change position
        image.enabled = true; // Show bone
    }

    public void randomTransform(){
        RectTransform boneRectTransform = image.rectTransform;
        // Position
        boneRectTransform.localPosition = randomPosition();
        // Rotation - note just adds a 0-360 to whatever current rotation is, can create negatives
        boneRectTransform.Rotate(new Vector3( 0, 0, UnityEngine.Random.Range(0, 360) ));
        // Size
        float randomScale = UnityEngine.Random.Range(0.3f, 0.7f);
        boneRectTransform.localScale = new Vector3(randomScale,randomScale, 0f);
    }
    
    Vector2 randomPosition(){
        // Bone size
        RectTransform boneRectTransform = image.rectTransform;
        float boneOffset = (boneRectTransform.rect.width * boneRectTransform.localScale.x)/2;
        
        // Get dog width and position in local space
        RectTransform dogRectTransform = dogImage.rectTransform;
        float dogOffset = (dogRectTransform.rect.width * dogRectTransform.localScale.x)/2;
        float dogLocation = dogRectTransform.localPosition.x;

        // X value: decide between left and right, avoiding the dog
        float randomX = 0f;
        if (UnityEngine.Random.value < 0.5f){
            float leftStart = -Screen.width + boneOffset;
            float leftBound = dogLocation - dogOffset - boneOffset;
            randomX = UnityEngine.Random.Range(leftStart, leftBound); // Left side
        } else {
            float rightStart = dogLocation + dogOffset + boneOffset;
            float rightBound = Screen.width - boneOffset;
            randomX = UnityEngine.Random.Range(rightStart, rightBound); // Right side
        }
            
        // Y value: within the range below the score card
        float topY = feedbackText.rectTransform.localPosition.y;
        float randomY = UnityEngine.Random.Range(-Screen.height+boneOffset, topY-boneOffset);

        //float boneOffset = ((image.rectTransform.rect.width*image.rectTransform.localScale.x)/2);
        return new Vector2(randomX, randomY);//new Vector2(randomX, randomY);
    }

    // Throw
    public void Throw(){
        boneThrow.Play();
        StartShrink(1f, new Vector3(0, 0, 0), 360f);
    }

        // Public method to call for starting the shrink animation
    void StartShrink(float shrinkDuration, Vector3 finalScale, float spinSpeed) {
        // Start the shrinking coroutine
        StartCoroutine(Shrink(shrinkDuration, finalScale, spinSpeed));
    }

    // Coroutine to handle the shrinking over time
    IEnumerator Shrink(float duration, Vector3 targetScale, float spinSpeed){
        Vector3 initialScale = transform.localScale;
        float timer = 0f;
        while (timer < duration){
            // Increment the timer based on time passed
            timer += Time.deltaTime;
            // Calculate how much to shrink the scale based on the timer
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            // Rotate the object (spin around the Z-axis by default)
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            // Wait for the next frame before continuing the loop
            yield return null;
        }
        // Ensure the object reaches the target scale exactly at the end
        transform.localScale = targetScale;
    }

    // For testing: (comment out TrialManager's call to new trial in Start() as well)
    // Test animations
    // void Start(){
    //     Throw();
    // }

    // Test random locations
    //void Update(){ 
    //     image.rectTransform.localPosition = randomPosition(); 
    // }
}
