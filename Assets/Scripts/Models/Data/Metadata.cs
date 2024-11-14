using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    // SESSION-SPECIFIC: Should be NonSerialized
    public string sessionID = Utility.GenerateRandomId(7) + DateTime.Now.ToString("ddMMyyyy");        // New for each session
    public string start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");           // Current session start time
    public string end;            // Current session end time
    public string startL1;        // Current L1 start time
    public string endL1;         // Current L1 end time
    public string userAgentString; // Browser/system info for current session

    // EXPERIMENT CONFIGURATION: Can remain serialized
    public string experimentID;    // Study configuration identifier
    public string participantName; // Participant identifier
    public string studyName;      // Study name
    public string l1n;            // Language/condition parameter

    // Constructor
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
