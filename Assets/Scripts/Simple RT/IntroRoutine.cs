using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image
using TMPro; //for TextMeshProUGUI

public class IntroRoutine : MonoBehaviour
{
    public bool viewingInstructions = true;
    public bool allowContinue = false;
    public GameObject instructions;
    public RectTransform instructionsRect;
    public GameObject scoreCard;
    public Image Dog;
    public TextMeshProUGUI continueText; // Reference to text box

    IEnumerator FadeIn(float seconds, Graphic graphic){
        // Graphic.color returns copy not reference so can't be set directly
        Color originalColor = graphic.color; // Get the current color
        originalColor.a = 0; // Set alpha to 0
        graphic.color = originalColor; // Put back in image

        // While alpha is not full
        while (graphic.color.a < 1.0f) {
            // Calculate new alpha
            originalColor.a = graphic.color.a + (Time.deltaTime / seconds); // Turn up Alpha by time elapsed
            graphic.color = originalColor; // put back in colour
            yield return null;
        }
    }

    IEnumerator Swing(RectTransform uiElement){
        float elapsed = 0f;
        float startAngle = 90f;
        Vector3 originalRotation = uiElement.localEulerAngles;
        
        // Animation parameters
        float duration = 4f;
        float frequency = 3f;     // How many oscillations
        float dampening = 3f;     // How quickly it settles
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / duration;
            
            // Calculate damped oscillation
            float angle = startAngle * Mathf.Exp(-dampening * normalizedTime) * 
                         Mathf.Cos(frequency * Mathf.PI * normalizedTime);
            
            // Apply rotation to UI element
            uiElement.localRotation = Quaternion.Euler(
                originalRotation.x + angle,
                originalRotation.y,
                originalRotation.z
            );
            
            yield return null;
        }
        
        // Ensure we end at original rotation plus exactly 0 on Z
        uiElement.localRotation = Quaternion.Euler(
            originalRotation.x,
            originalRotation.y,
            originalRotation.z
        );
    }

    IEnumerator CoroutineChain(){
        yield return StartCoroutine(FadeIn(2f, Dog));
        instructions.SetActive(true);
        yield return StartCoroutine(Swing(instructionsRect));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(FadeIn(1f, continueText));
        allowContinue = true;
    }

    void StartGame(){
        instructions.SetActive(false);
        scoreCard.SetActive(true);
        viewingInstructions = false;
        DataManager.Instance.Level1Started();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoroutineChain());
    }

    // Update is called once per frame
    void Update()
    {
        if(allowContinue && viewingInstructions && Input.GetKeyDown(KeyCode.Space)){
            StartGame();
        }
        
    }
}
