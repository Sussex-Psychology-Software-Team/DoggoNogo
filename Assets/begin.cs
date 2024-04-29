using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager.LoadScene
using TMPro; // grab text input field
using System; // String class
using UnityEditor; // DisplayDialog

public class begin : MonoBehaviour
{
    private bool pressed = false; //make sure next scene can only be loaded once
    public TMP_InputField name_input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string name = name_input.text;
        if(Input.GetKeyDown(KeyCode.Return) && !pressed){
            Debug.Log("Enter");
            if(String.IsNullOrWhiteSpace(name)){ //if name field empty or all whitespace
                EditorUtility.DisplayDialog("Name field empty", "Please enter a name", "Ok");
            } else {
                //strip leading/trailing whitespace
                char[] charsToTrim = {' ', '\''};
                string result = name.Trim(charsToTrim);
                //move to first scene
                pressed = true;
                //gameObject.transform.localScale = Vector3.zero; // hide instructions
                SceneManager.LoadScene("Simple RT");
            }
        }
    }
}
