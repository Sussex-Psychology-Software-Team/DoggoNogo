using UnityEngine;
using UnityEngine.SceneManagement;

public class begin : MonoBehaviour
{
    private bool pressed = false; //make sure scene can only be loaded once

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("space") && !pressed){
            pressed = true;
            //gameObject.transform.localScale = Vector3.zero; // hide instructions
            SceneManager.LoadScene("Simple RT");
        }
    }
}
