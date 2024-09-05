using System;
using UnityEngine;
using UnityEngine.UI; // Image namespace

public class Bone : MonoBehaviour
{
    public Image image;

    public void Hide(){
        image.enabled = false; // Hide bone
    }

    public void Show(){
        image.enabled = true; // Hide bone
    }

    public bool Hidden(){
        return image.enabled; // Is bone hidden
    }
}
