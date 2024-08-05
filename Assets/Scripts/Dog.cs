using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Image.sprite

public class Dog : MonoBehaviour
{
    public Sprite[] images;
    private int i = 0;

    public void NextSprite(){
        // Loops through sprites automatically
        gameObject.GetComponent<Image>().sprite = images[++i];
    }
}
