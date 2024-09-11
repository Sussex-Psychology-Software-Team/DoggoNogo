using System;
using UnityEngine;
using UnityEngine.UI; // Image namespace
using TMPro; // TextMeshProUGUI

public class Bone : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI feedbackText; // Defines top bound of bone position
    public Image dogImage;

    public void Hide(){
        image.enabled = false; // Hide bone
    }

    public void Show(){
        randomTransform(); // Change position
        image.enabled = true; // Show bone
    }

    public bool Hidden(){
        return image.enabled == false; // Is bone hidden
    }

    public void randomTransform(){
        // Position
        image.rectTransform.localPosition = randomPosition();
        // Rotation
        image.rectTransform.Rotate( new Vector3( 0, 0, UnityEngine.Random.Range(0, 360) ) );
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

    // For testing: (comment out TrialManager's Update() code as well)
    //void Update(){ 
    //     image.rectTransform.localPosition = randomPosition(); 
    // }
}
