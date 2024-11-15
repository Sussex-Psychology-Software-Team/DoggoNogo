using System;
using System.Runtime.InteropServices;
using UnityEditor.Playables;
using UnityEngine;

// Metadata model - grabs url vars as well
[System.Serializable]
public partial class Metadata {
    // SESSION-SPECIFIC: Should be NonSerialized?
    public string sessionID = StringUtils.GenerateRandomId(7) + DateTime.Now.ToString("ddMMyyyy");        // New for each session
    public string start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");           // Current session start time
    public string end;            // Current session end time
    public string startL1;        // Current L1 start time
    public string endL1;         // Current L1 end time
    public string userAgentString; // Browser/system info for current session

    // EXPERIMENT CONFIGURATION: Can remain serialized?
    public string experimentID;    // Study configuration identifier
    public string participantName; // Participant identifier
    public string studyName;      // Study name
    public string l1n;            // Language/condition parameter

    // Constructor
    public void InitializeWebVariables(){
        // Otherwise datamanager starts on Awake() and UNITY_WEBGL is not true
        experimentID = WebUtils.GetQueryVariable(Constants.QueryParams.DataPipe);
        participantName = WebUtils.GetQueryVariable(Constants.QueryParams.ParticipantName);
        studyName = WebUtils.GetQueryVariable(Constants.QueryParams.StudyName);
        l1n = WebUtils.GetQueryVariable(Constants.QueryParams.Level1Trials);
        userAgentString = GetUserAgent();
    }

    private static string GetUserAgent() {
        #if UNITY_EDITOR
            return "EDITOR";
        #elif UNITY_WEBGL
            return userAgent();
        #else
            return "NO_UA";
        #endif
    }

    [DllImport("__Internal")]
    private static extern string userAgent();
}
