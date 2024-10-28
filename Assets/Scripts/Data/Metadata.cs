using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    public string randomID;
    public string experimentID;
    public string participantName;
    public string studyName;
    public string l1n;
    public string userAgentString;
    public string start;
    public string end;
    public string trialsStart;
    public string trialsEnd;

    // Constructor
    public Metadata() {
        randomID = Utility.GenerateRandomId(24);
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public void InitializeWebVariables(){
        // Otherwise datamanager starts on Awake() and UNITY_WEBGL is not true
        experimentID = Utility.GetQueryVariable("datapipe");
        participantName = Utility.GetQueryVariable("p");
        studyName = Utility.GetQueryVariable("s");
        l1n = Utility.GetQueryVariable("l1n");
        userAgentString = GetUserAgent();
    }


    static string GetUserAgent() {
        #if UNITY_EDITOR
            return "EDITOR";
        #elif UNITY_WEBGL
            return userAgent();
        #else
            return "NO_UA";
        #endif
    }

    [DllImport("__Internal")]
    static extern string userAgent();
}
