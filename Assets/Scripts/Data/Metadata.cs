using System;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Metadata {
    public string id;
    public string name;
    public string userAgentString;
    public string start;
    public string end;
    public int retry;

    // Constructor
    public Metadata() {
        id = GenerateRandomId(24);
        userAgentString = GetUserAgent();
        start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        retry = 0;
    }

    private static string GenerateRandomId(int size) {
        var rand = new System.Random();
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var randId = new char[size];
        for (var i = 0; i < size; i++) {
            randId[i] = characters[rand.Next(characters.Length)];
        }
        return new string(randId);
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
