using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetVolume(float volume)
    { }
   
    public void SetQuality(int QualityIndex)
        {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    public void SetFullscreen()
    {
        
    }
}
