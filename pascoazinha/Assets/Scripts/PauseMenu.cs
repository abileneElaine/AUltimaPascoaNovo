using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

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
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Limpa checkpoint ao voltar pro menu
        if (GameManager.instance != null)
            GameManager.instance.ResetarCheckpoint();

        SceneManager.LoadScene("Menu"); // ← Ajuste o nome da sua cena de menu
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
        if (mixer == null) return;

        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", -10f);
        float volGeral = PlayerPrefs.GetFloat("VolumeGeral", -10f);

        mixer.SetFloat("VolumeMusica", volMusica);
        mixer.SetFloat("VolumeGeral", volGeral);

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
        }
    }

    public void AjustarVolumeGeral(float valor)
    {
        if (mixer != null)
        {
            mixer.SetFloat("VolumeGeral", valor);
            PlayerPrefs.SetFloat("VolumeGeral", valor);
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

        // Recarrega volumes
        CarregarConfiguracoesAudio();
    }
}