using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    // SESSION-SPECIFIC: Should be NonSerialized
    [System.NonSerialized]
    public string sessionID;        // New for each session
    [System.NonSerialized]
    public string start;           // Current session start time
    [System.NonSerialized]
    public string end;            // Current session end time
    [System.NonSerialized]
    public string startL1;        // Current L1 start time
    [System.NonSerialized]
    public string endL1;         // Current L1 end time
    [System.NonSerialized]
    public string userAgentString; // Browser/system info for current session

    // EXPERIMENT CONFIGURATION: Can remain serialized
    public string experimentID;    // Study configuration identifier
    public string participantName; // Participant identifier
    public string studyName;      // Study name
    public string l1n;            // Language/condition parameter

    // Constructor
    public Metadata() {
        sessionID = Utility.GenerateRandomId(7) + DateTime.Now.ToString("ddMMyyyy");
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
