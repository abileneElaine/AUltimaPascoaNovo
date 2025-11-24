using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelMenuInicial;
    public GameObject painelOpcoes;

    [Header("Audio")]
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderGeral;

    private void Start()
    {
        float volM = PlayerPrefs.GetFloat("VolumeMusica", -10f);
        float volG = PlayerPrefs.GetFloat("VolumeGeral", -10f);

        sliderMusica.value = volM;
        sliderGeral.value = volG;

        
        
        mixer.SetFloat("VolumeMusica", volM);
        mixer.SetFloat("VolumeGeral", volG);
    }

    public void AjustarVolumeMusica(float valor)
    {
        mixer.SetFloat("VolumeMusica", valor);
        PlayerPrefs.SetFloat("VolumeMusica", valor);
    }

    public void AjustarVolumeGeral(float valor)
    {
        mixer.SetFloat("VolumeGeral", valor);
        PlayerPrefs.SetFloat("VolumeGeral", valor);
    }

    public void Jogar()
    {
        SceneManager.LoadScene("CutsCene");
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}