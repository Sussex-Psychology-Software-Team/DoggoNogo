using System;
using System.Runtime.InteropServices; // DllImport

public static class Utility
{
    [DllImport("__Internal")]
    private static extern IntPtr queryString(string variable);

    public static string GetQueryVariable(string variable){
        #if !UNITY_EDITOR && UNITY_WEBGL 
        //if(Application.platform == RuntimePlatform.WebGLPlayer){
            IntPtr ptr = queryString(variable);
            if (ptr != IntPtr.Zero){
                string result = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(ptr);
                return result;
            }
            return "QUERY VAR NOT FOUND";
        #endif
        return "NOT BROWSER";
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
