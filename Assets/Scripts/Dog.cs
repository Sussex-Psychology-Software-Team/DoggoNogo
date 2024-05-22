using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Image.sprite

public class Dog : MonoBehaviour
{
    public Sprite[] images;
    public Bone bone;
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(bone.score>1 && bone.score<=3){
            gameObject.GetComponent<Image>().sprite = images[0];
            //Debug.Log("IMG 0");
        } else if(bone.score>3 && bone.score<=5){
            gameObject.GetComponent<Image>().sprite = images[1];
            //Debug.Log("IMG 1");
        } else if(bone.score>5){
            gameObject.GetComponent<Image>().sprite = images[2];
            //Debug.Log("IMG 2");
        }
    }

    void ChangeSprite(){
        // Loops through sprites automatically
        if(i<images.Length){
            gameObject.GetComponent<Image>().sprite = images[i];
            i++;
        }
    }
}
