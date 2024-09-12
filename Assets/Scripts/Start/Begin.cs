using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager.LoadScene
using System.Runtime.InteropServices; //for DllImport
using TMPro; // grab text input field
using System; // String class

public class Begin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void fullpage();

    [DllImport("__Internal")]
    private static extern void queryString();

    private bool scene_triggered = false; //make sure next scene can only be loaded once
    private string query;

    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            //fullpage();
            query = queryString("var"); //note if testing this include ?var=abc after URL
            Debug.Log(query);
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && !scene_triggered){
                scene_triggered = true;
                SceneManager.LoadScene("Introduction");
        }
    }
}
