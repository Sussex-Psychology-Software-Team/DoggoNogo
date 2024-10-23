using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource backgroundMusic;

    //note: same as an increase in speed: https://discussions.unity.com/t/how-i-can-change-the-speed-of-a-song-or-sound/6623/3
    public void Stop(){
        backgroundMusic.Stop();
    }

    public void Play(){
        backgroundMusic.Play();
    }

    public void ChangePitch(float pitch){
        backgroundMusic.pitch = pitch;
    }

    public bool Playing(){
        return backgroundMusic.isPlaying;
    }
}
