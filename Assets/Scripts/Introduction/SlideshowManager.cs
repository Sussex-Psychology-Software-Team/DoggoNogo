using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Image.sprite
using TMPro; //for TextMeshProUGUI
using UnityEngine.SceneManagement; // SceneManager.LoadScene

public class slideshowManager : MonoBehaviour
{
    public TextMeshProUGUI textBox; // Reference to text box
    public Image textBackground;

    public Image backgroundImage;
    public Sprite garden;
    public Image dog;

    public AudioSource metalDoor;
    public AudioSource dogWhine;

    public float timeBetweenImages = 3.0f;

    // Private globals
    string[] storyText = {
        "You are an investigator tasked with bringing down criminals that mistreat animals.",
        "During your last raid, your hear something.",
        "You decide to take him home, and name him…<br><br>  ",
        "Doggo",
        "Doggo is in need of urgent care and feeding.<br>Help him get as many bones as possible by pressing the <color=red>down arrow (↓) when the bone appears <color=white>as fast as possible."
    };
    float timer;
    int chapter = 0;
    bool videoFinished = false;

    void CheckChapterNumber(){
        chapter++;
        if(chapter == 11){ //Skip three chapters to allow participants to read instructions
            videoFinished = true;
        } else {
            ShowNextChapter();
        }
    }

    void ShowNextChapter(){
        // Swap media for each Chapter
        if(chapter == 1){
            metalDoor.Play(); // Play metal door sound
            textBox.text = storyText[1]; // "During your last raid, your hear something."

        } else if(chapter == 2){
            textBox.text = ""; // Get rid of text
            backgroundImage.color = Color.white; // Make white to show background Dog Pound image
            StartCoroutine(FadeIn(1f, backgroundImage));
            dogWhine.Play(); // Play dog whine

        } else if(chapter == 3){ 
            dog.enabled = true; // Show dog in jail

        } else if(chapter == 4){ 
            textBox.text = storyText[2]; // "You decide to take him home, and name him…",

        } else if(chapter == 5){ // Display Doggo's name
            dog.enabled = false; // Hide dog
            backgroundImage.color = Color.black; // Black Background
            textBox.fontSize = 100; // BOOM: Title card
            textBox.text = storyText[3]; // "Doggo"
            
        } else if(chapter == 6){ // Doggo in Garden
            textBox.text = ""; // Get rid of text
            backgroundImage.color = Color.white; // White to show background image
            backgroundImage.sprite = garden; // Change to Garden
            dog.enabled = true; // Show dog in garden
            StartCoroutine(FadeIn(1f, dog)); // Fade in dog image

        } else if(chapter == 7){ // Instructions
            textBox.fontSize = 90; // Smaller more readable text
            textBackground.color = new Color(1f, 1f, 1f, 1f); // Set alpha to 1 to show wooden background
            textBox.text = storyText[4]; // "Doggo is in need of urgent care and feeding.<br>Help him get as many bones as possible by pressing the down arrow (↓) as fast as possible."

        }

        // Reset timer
        timer = timeBetweenImages;
    }

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
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn(1f, textBox));
        timer = timeBetweenImages;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0.0f && !videoFinished){
            CheckChapterNumber();
        }

        if (videoFinished){
            SceneManager.LoadScene("Simple RT");
        }
    }
}
