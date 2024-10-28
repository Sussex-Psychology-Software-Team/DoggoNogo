using UnityEngine;
using System.Runtime.InteropServices;

public class FullscreenManager : MonoBehaviour
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
