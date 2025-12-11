using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AjustarVolumePausa : MonoBehaviour
{
    public Slider SliderMusic;
    public AudioMixer mixer;

    private float MusicVolume;
    
    void Start()
    {
        
        mixer.GetFloat("VolumeMusica", out MusicVolume);
 
      SliderMusic.value = MusicVolume;
    }

    void Update()
    {
        MusicVolume = SliderMusic.value;

        if (MusicVolume <= -20)
        {
            MusicVolume = -80;
        }

        mixer.SetFloat("VolumeMusica", MusicVolume);
    }
}
