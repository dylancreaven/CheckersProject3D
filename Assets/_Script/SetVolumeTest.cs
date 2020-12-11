using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SetVolumeTest : MonoBehaviour
{
      public AudioMixer mixer;
    public void SetVolume(float volume)
    {
        Debug.Log("volume: "+volume);
        mixer.SetFloat("MusicVol",Mathf.Log10(volume)*20);
    }
}
