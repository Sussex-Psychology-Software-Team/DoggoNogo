using System;
using System.Collections; // For IEnumerator
using UnityEngine;
using UnityEngine.UI; // Image namespace
using TMPro; // TextMeshProUGUI

public class Bone : MonoBehaviour
{
    // Refs
    public Image image;
    public Image dogImage;
    public Canvas canvas;
    public TextMeshProUGUI feedbackText; // Defines top bound of bone position
    public GameObject scoreContainer;
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

    void randomTransform(){
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
        float boneOffset = (boneRectTransform.rect.width * boneRectTransform.localScale.x)/2f;
        
        // Get dog width and position in local space
        RectTransform dogRectTransform = dogImage.rectTransform;
        float dogOffset = (dogRectTransform.rect.width * dogRectTransform.localScale.x)/2f;
        float dogLocationX = dogRectTransform.localPosition.x;

        // Get Canvas bounds
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        // Debug.Log(canvasRectTransform.localScale.x);
        float xBound = canvasRectTransform.rect.width/2f;
        float yBound = canvasRectTransform.rect.height/2f;

        // X value: decide between left and right, avoiding the dog
        float randomX = 0f;
        if (UnityEngine.Random.value < 0.5f){
            float leftStart = -xBound + boneOffset;
            float leftBound = dogLocationX - dogOffset - boneOffset;
            randomX = UnityEngine.Random.Range(leftStart, leftBound); // Left side
        } else {
            float rightStart = dogLocationX + dogOffset + boneOffset;
            float rightBound = xBound - boneOffset;
            randomX = UnityEngine.Random.Range(rightStart, rightBound); // Right side
        }
            
        // Y value: within the range below the score card
        // Score card bottom
        RectTransform scoringRectTransform = scoreContainer.GetComponent<RectTransform>();
        float scoringOffset = (scoringRectTransform.rect.height * scoringRectTransform.localScale.y) /2f;
        float scoringBottom = scoringRectTransform.localPosition.y - scoringOffset;

        float randomY = UnityEngine.Random.Range(-yBound + boneOffset, scoringBottom-boneOffset);

        return new Vector2(randomX, randomY);//new Vector2(randomX, randomY);
    }

    // Throw
    public void Throw(){
        boneThrow.Play();
        StartShrink(1f, 360f);
    }

    // Starts shrink animation
    void StartShrink(float shrinkDuration, float spinSpeed) {
        // Start the shrinking coroutine
        StartCoroutine(Shrink(shrinkDuration, spinSpeed));
    }

    IEnumerator Shrink(float duration, float spinSpeed){
        Vector3 initialScale = transform.localScale;
        float timer = 0f;
        while (timer < duration){
            timer += Time.deltaTime; // Time passed
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer / duration); // Shrinking
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime); // Rotate
            yield return null; // Wait for next before continue
        }
        transform.localScale = Vector3.zero; // At end reaches target scale
    }

    // Move to dog animation
    public void Eat(){
        // Start animation
        Vector3 targetScale = new(0.1f, 0.1f, 0.1f);

        // Get target position
        RectTransform dogRectTransform = dogImage.rectTransform;
        // Get the pixel coordinates of mouth relative to rendered image scale
        float mouthXFromCentre = ((dogRectTransform.rect.width/2)-121f) * dogRectTransform.localScale.x; // Found float coordinate on photoshop
        float mouthYFromCentre = ((dogRectTransform.rect.height/2)-219f)* dogRectTransform.localScale.y;
        Vector3 targetPosition = new(dogRectTransform.localPosition.x - mouthXFromCentre, dogRectTransform.localPosition.y + mouthYFromCentre, 0);

        // Animate
        StartCoroutine(MoveBoneToMouth(targetPosition, targetScale, 0.3f));
    }

    IEnumerator MoveBoneToMouth(Vector3 targetPosition, Vector3 targetScale, float duration){
        // Setup
        Vector3 startPosition = image.rectTransform.localPosition; // Start position
        Vector3 startScale = image.rectTransform.localScale; // Start scale
        float elapsedTime = 0f; // Elapsed time since the start of the animation
        // Run
        while (elapsedTime < duration){
            // Interpolate between the start and target positions and scales
            image.rectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            image.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            // Increment enumerator
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait until the next frame
        }
        Hide();
    }

    // For testing: (comment out TrialManager's call to new trial in Start() as well)
    // Test animations
    // void Start(){
    // }

    // Test random locations
    // void Update(){ 
    // }
}
