using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //button
using UnityEngine.SceneManagement; // SceneManager.LoadScene

public class Restart : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(restartExp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void restartExp(){
        SceneManager.LoadScene("Simple RT");
    }
}