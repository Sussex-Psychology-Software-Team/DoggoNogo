using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    public string id;
    public string userAgentString;
    public string start;
    public string end;
    public int retry;

    // Constructor
    public Metadata() {
        id = Utility.GenerateRandomId(24);
        userAgentString = GetUserAgent();
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        retry = 0;
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
    static extern string userAgent();
}
