using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AjustarVolumePausa : MonoBehaviour
{
    public Slider SliderMusic;
    public AudioMixer mixer;
    private float MusicVolume;
    private const string VOLUME_KEY = "MusicVolume"; // Chave para salvar o volume

    void Start()
    {
        // Carrega o volume salvo (ou usa -20 como padrão se não houver nada salvo)
        MusicVolume = PlayerPrefs.GetFloat(VOLUME_KEY, -20f);

        // Aplica o volume carregado
        mixer.SetFloat("VolumeMusica", MusicVolume);
        SliderMusic.value = MusicVolume;

        // Adiciona um listener para salvar quando o slider mudar
        SliderMusic.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        MusicVolume = value;

        // Limita o volume mínimo
        if (MusicVolume <= -80)
        {
            MusicVolume = -80;
        }

        // Aplica no mixer
        mixer.SetFloat("VolumeMusica", MusicVolume);

        // Salva a configuração
        PlayerPrefs.SetFloat(VOLUME_KEY, MusicVolume);
        PlayerPrefs.Save(); // Garante que seja salvo imediatamente
    }

    void OnDestroy()
    {
        // Remove o listener quando o objeto for destruído
        SliderMusic.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}