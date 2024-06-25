using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Image.sprite

public class Dog : MonoBehaviour
{
    public Sprite[] images;
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeSprite(){
        // Loops through sprites automatically
         gameObject.GetComponent<Image>().sprite = images[++i];
    }
}
