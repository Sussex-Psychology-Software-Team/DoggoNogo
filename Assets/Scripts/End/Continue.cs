using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices; //for DllImport
using UnityEngine.UI; //button

public class Continue : MonoBehaviour
{
    public Button button;
    public string url = "https://www.example.com";
    [DllImport("__Internal")]
    static extern string queryStringWhole();


    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ContinueExp);
    }

    void ContinueExp(){
        string query;
        #if !UNITY_EDITOR && UNITY_WEBGL
            query = queryStringWhole();
        #else
            query = "";
        #endif
        Application.OpenURL(url + query);
    }
}
