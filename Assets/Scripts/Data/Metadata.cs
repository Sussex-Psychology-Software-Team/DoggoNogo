using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    public string randomId;
    public string experimentID;
    public string userAgentString;
    public string start;
    public string end;

    // Constructor
    public Metadata() {
        randomId = Utility.GenerateRandomId(24);
        experimentID = Utility.GetQueryVariable("experimentID");
        userAgentString = GetUserAgent();
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
