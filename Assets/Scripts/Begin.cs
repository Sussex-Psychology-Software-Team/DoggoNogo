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
    public TMP_InputField nameInput;
    public GameObject warning;
    private string query;

    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
            //fullpage();
            //query = queryString("var"); //note if testing this include ?var=abc after URL
            //Debug.Log(query);
        #endif
        warning.SetActive(false); // false to hide, true to show
    }

    // Update is called once per frame
    void Update()
    {
        string name = nameInput.text;
        if(Input.GetKeyDown(KeyCode.Return) && !scene_triggered){
            if(String.IsNullOrWhiteSpace(name)){ //if name field empty or all whitespace, and warning not already showing to avoid sticking here
                warning.SetActive(true); // false to hide, true to show
                nameInput.ActivateInputField();
                Debug.Log("Name not entered correctly");
            } else {
                //strip leading/trailing whitespace
                char[] charsToTrim = {' ', '\''};
                string result = name.Trim(charsToTrim);
                //move to first scene
                scene_triggered = true;
                //gameObject.transform.localScale = Vector3.zero; // hide instructions
                PlayerPrefs.SetString("Name", name);
                SceneManager.LoadScene("Simple RT");
            }
        }
    }
}
