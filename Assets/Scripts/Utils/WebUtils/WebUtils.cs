using System;
using System.Runtime.InteropServices;

public static class WebUtils
{
    [DllImport("__Internal")]
    private static extern IntPtr queryString(string variable);

    [DllImport("__Internal")]
    private static extern string queryStringWhole();
    
    public static string GetQueryVariable(string variable)
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
                    IntPtr ptr = queryString(variable);
                    if (ptr != IntPtr.Zero)
                    {
                        string result = Marshal.PtrToStringAnsi(ptr);
                        Marshal.FreeHGlobal(ptr);
                        return result;
                    }
                    return "QUERY VAR NOT FOUND";
        #endif
        return "NOT BROWSER";
    }
    public static string GetQueryStringWhole()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
                    return queryStringWhole();
        #else
                return string.Empty;
        #endif
    }
}