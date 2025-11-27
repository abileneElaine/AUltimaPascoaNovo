using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    
    private GameObject EncontrarObjetoMesmoDesativado(string nome)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform t in objs)
        {
            if (t.hideFlags == HideFlags.NotEditable || t.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (t.name == nome)
                return t.gameObject;
        }

        return null;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Encontra as telas mesmo se estiverem DESATIVADAS
        telaVitoria = EncontrarObjetoMesmoDesativado("TelaVitoria");
        telaGameOver = EncontrarObjetoMesmoDesativado("TelaGameOver");

        if (telaGameOver != null)
            telaGameOver.SetActive(false);

        if (telaVitoria != null)
            telaVitoria.SetActive(false);
    }


    // ========= GAME OVER =========
    public void MostrarTelaGameOver()
    {
        Time.timeScale = 0f;

        if (telaGameOver != null)
            telaGameOver.SetActive(true);
        else
            Debug.LogError("TelaGameOver não encontrada na cena!");

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

    public bool TemCheckpoint()
    {
        return existeCheckpoint;
    }

    public Vector3 ObterCheckpoint()
    {
        return posicaoCheckpoint;
    }
}
