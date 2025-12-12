using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [Header("Painéis")]
    public GameObject MenuDePausa;
    public GameObject PainelOpcoes;

    [Header("Audio")]
    public AudioMixer mixer;

    [Header("Sliders de Volume")]
    public Slider sliderMusica;
    public Slider sliderGeral;

    public bool isPaused;

    void Awake()
    {
        // Singleton - garante apenas um PauseMenu em todas as cenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Garante que os menus começam escondidos
        if (MenuDePausa != null)
            MenuDePausa.SetActive(false);

        if (PainelOpcoes != null)
            PainelOpcoes.SetActive(false);

        isPaused = false;

        // Carrega configurações de volume salvas
        CarregarConfiguracoesAudio();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        if (MenuDePausa != null)
        {
            MenuDePausa.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;

            // Garante que opções está fechado
            if (PainelOpcoes != null)
                PainelOpcoes.SetActive(false);

            // Atualiza sliders ao abrir o pause
            AtualizarSliders();
        }
    }

    public void Resume()
    {
        if (MenuDePausa != null)
        {
            MenuDePausa.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;

            // Fecha opções também
            if (PainelOpcoes != null)
                PainelOpcoes.SetActive(false);
        }
    }

    public void AbrirOpcoes()
    {
        if (MenuDePausa != null && PainelOpcoes != null)
        {
            MenuDePausa.SetActive(false);
            PainelOpcoes.SetActive(true);

            // Atualiza sliders ao abrir opções
            AtualizarSliders();
        }
    }

    public void FecharOpcoes()
    {
        if (MenuDePausa != null && PainelOpcoes != null)
        {
            PainelOpcoes.SetActive(false);
            MenuDePausa.SetActive(true);
        }
    }

    public void ReiniciarFase()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // As configurações de áudio serão reaplicadas automaticamente
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Limpa checkpoint ao voltar pro menu
        if (GameManager.instance != null)
            GameManager.instance.ResetarCheckpoint();

        SceneManager.LoadScene("Menu");
    }

    public void SairDoJogo()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }

    // ===== SISTEMA DE ÁUDIO =====

    void CarregarConfiguracoesAudio()
    {
        if (mixer == null)
        {
            Debug.LogWarning("⚠️ AudioMixer não foi atribuído no PauseMenu!");
            return;
        }

        // Carrega volumes salvos (padrão: -10dB)
        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", -10f);
        float volGeral = PlayerPrefs.GetFloat("VolumeGeral", -10f);

        // Aplica no mixer
        mixer.SetFloat("VolumeMusica", volMusica);
        mixer.SetFloat("VolumeGeral", volGeral);

        // Atualiza sliders
        AtualizarSliders();

        Debug.Log($"🔊 Áudio carregado - Música: {volMusica}dB | Geral: {volGeral}dB");
    }

    private void AtualizarSliders()
    {
        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", -10f);
        float volGeral = PlayerPrefs.GetFloat("VolumeGeral", -10f);

        if (sliderMusica != null)
            sliderMusica.value = volMusica;

        if (sliderGeral != null)
            sliderGeral.value = volGeral;
    }

    public void AjustarVolumeMusica(float valor)
    {
        if (mixer != null)
        {
            mixer.SetFloat("VolumeMusica", valor);
            PlayerPrefs.SetFloat("VolumeMusica", valor);
            PlayerPrefs.Save(); // ← Salva imediatamente
            Debug.Log($"🎵 Volume Música: {valor}dB");
        }
    }

    public void AjustarVolumeGeral(float valor)
    {
        if (mixer != null)
        {
            mixer.SetFloat("VolumeGeral", valor);
            PlayerPrefs.SetFloat("VolumeGeral", valor);
            PlayerPrefs.Save(); // ← Salva imediatamente
            Debug.Log($"🔊 Volume Geral: {valor}dB");
        }
    }

    // ===== RECONEXÃO AUTOMÁTICA ENTRE CENAS =====

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Busca os painéis na nova cena automaticamente
        if (MenuDePausa == null)
        {
            GameObject canvas = GameObject.Find("PauseCanvas");
            if (canvas != null)
            {
                Transform menuTransform = canvas.transform.Find("MenuDePausa");
                if (menuTransform != null)
                {
                    MenuDePausa = menuTransform.gameObject;
                    MenuDePausa.SetActive(false);
                }

                Transform opcoesTransform = canvas.transform.Find("PainelOpcoes");
                if (opcoesTransform != null)
                {
                    PainelOpcoes = opcoesTransform.gameObject;
                    PainelOpcoes.SetActive(false);
                }
            }
        }

        // Reseta estado ao mudar de cena
        isPaused = false;
        Time.timeScale = 1f;

        // IMPORTANTE: Recarrega e reaplica volumes
        CarregarConfiguracoesAudio();
    }
}