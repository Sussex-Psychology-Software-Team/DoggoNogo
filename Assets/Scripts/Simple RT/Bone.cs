using System;
using UnityEngine;
using UnityEngine.UI; // Image namespace
using TMPro; // TextMeshProUGUI

public class Bone : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI feedbackText; // Defines top bound of bone position
    public Image dogImage;
    public Canvas canvas;

    public void Hide(){
        image.enabled = false; // Hide bone
    }

    public void Show(){
        image.enabled = true; // Hide bone
    }

    public bool Hidden(){
        return image.enabled == false; // Is bone hidden
    }

    public void randomTransform(){
        // Needs definition of left and right of screen, small box within window bounds.
        // // Scale between 0.53156 to 
        // x = Random.Range(-25, 26);
        // y = 5;
        // z = Random.Range(-25, 26);
        // Note 0,0 is bottom left

        // Rect bone = image.rectTransform.rect;
        // Texture texture = image.sprite.texture;      
        // X: Left : -780 to  -340 or Right: 270 to 780
        // Y: -400 to 200

        // Left or right side

        transform.position = randomPosition();
        //transform.position = new Vector3(100, 100, 0);
        Debug.Log(transform.position);
    }

    Vector2 randomPosition(){
        Rect boneRect = image.rectTransform.rect;
        RectTransform dogRectTransform = dogImage.rectTransform;

        // Get dog width and position in local space
        float dogWidth = dogRectTransform.rect.width;
        float dogLocationX = dogRectTransform.localPosition.x;

        // X value: decide between left and right, avoiding the dog
        float randomX = 0f;
        if (UnityEngine.Random.value < 0.5f){
            float leftBound = dogLocationX - (dogWidth / 2) - boneRect.width;
            randomX = UnityEngine.Random.Range(0f, leftBound); // Left side
        } else {
            float rightStart = dogLocationX + (dogWidth / 2) + boneRect.width;
            float rightBound = Screen.width - boneRect.width;
            randomX = UnityEngine.Random.Range(rightStart, rightBound); // Right side
        }
            

        // Y value: within the range below the score card
        float topY = feedbackText.rectTransform.localPosition.y;
        float randomY = UnityEngine.Random.Range(boneRect.height, topY);

        return new Vector2(randomX, randomY);
    }

    void Start(){
        
    }
}
