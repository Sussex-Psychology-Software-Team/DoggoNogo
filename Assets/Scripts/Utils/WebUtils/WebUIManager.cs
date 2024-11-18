using System.Runtime.InteropServices;
using UnityEngine;

public class WebUIManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GoFullscreen();

    public void ToggleFullscreen()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
                    GoFullscreen();
        #endif
    }
}