using System;
using System.Runtime.InteropServices; // DllImport

public static class Utility
{
    [DllImport("__Internal")]
    static extern string queryString(string variable);

    public static string getQueryVariable(string variable){
        #if UNITY_EDITOR
            return "";
        #elif UNITY_WEBGL
            return queryString(variable);
        #else
            Debug.Log("No value found for variable: " + variable);
            return "";
        #endif
    }

    // Generates a random ID
    public static string GenerateRandomId(int size) {
        var rand = new System.Random();
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var randId = new char[size];
        for (var i = 0; i < size; i++){
            randId[i] = characters[rand.Next(characters.Length)];
        }
        return new string(randId);
    }
}
