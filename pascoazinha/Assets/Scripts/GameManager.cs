using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIs")]
    public GameObject telaGameOver;
    public GameObject telaVitoria;

    private string proximaCena;

    // ====== CHECKPOINT ======
    private Vector3 posicaoCheckpoint;
    private bool existeCheckpoint = false;

    // ===== SISTEMA DE INIMIGOS MORTOS =====
    public HashSet<string> inimigosMortos = new HashSet<string>();

    public void RegistrarInimigoMorto(string id)
    {
        if (!inimigosMortos.Contains(id))
            inimigosMortos.Add(id);
    }

    public bool InimigoJaMorreu(string id)
    {
        return inimigosMortos.Contains(id);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        // Procura UIs automaticamente na cena nova
        if (telaVitoria == null)
        {
            telaVitoria = GameObject.Find("TelaVitoria");
        }
           
        if (telaGameOver == null)
        {
            telaGameOver = GameObject.Find("TelaGameOver");
        }
           
    }

    // ========= GAME OVER =========
    public void MostrarTelaGameOver()
    {

        if (telaGameOver != null)
            telaGameOver.SetActive(true);
        else
            Debug.LogError("TelaGameOver não encontrada na cena!");
        
        Time.timeScale = 0f;

        StartCoroutine(AguardarTeclaReiniciarFase());
    }

    private IEnumerator AguardarTeclaReiniciarFase()
    {
        yield return null;

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ========= VITÓRIA =========
    public void MostrarTelaVitoria(string nextScene)
    {
        proximaCena = nextScene;

        Time.timeScale = 0f;

        if (telaVitoria != null)
            telaVitoria.SetActive(true);
        else
            Debug.LogError("TelaVitoria não encontrada na cena!");

        StartCoroutine(AguardarTeclaParaProximaFase());
    }

    private IEnumerator AguardarTeclaParaProximaFase()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;
        SceneManager.LoadScene(proximaCena);
    }

    // ========= CHECKPOINT =========
    public void DefinirCheckpoint(Vector3 position)
    {
        posicaoCheckpoint = position;
        existeCheckpoint = true;
        Debug.Log("CHECKPOINT DEFINIDO: " + position);
    }
    
    public void ResetarCheckpoint()
    {
        existeCheckpoint = false;
        posicaoCheckpoint = Vector3.zero;
        Debug.Log("Checkpoint resetado para nova fase");
    }


    public bool TemCheckpoint()
    {
        return existeCheckpoint;
    }

    public Vector3 ObterCheckpoint()
    {
        return posicaoCheckpoint;
    }

}

   
