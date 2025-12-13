using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIs - Arraste aqui no Inspector")]
    public GameObject telaGameOverPrefab;
    public GameObject telaVitoriaPrefab;

    // Referências ATIVAS na cena atual
    private GameObject telaGameOverAtual;
    private GameObject telaVitoriaAtual;

    private string proximaCena;

    // ====== CHECKPOINT ======
    private Vector3 posicaoCheckpoint;
    private bool existeCheckpoint = false;

    // ===== SISTEMA DE INIMIGOS MORTOS =====
    public HashSet<string> inimigosMortos = new HashSet<string>();

    // ===== SISTEMA DE MOEDAS COLETADAS =====
    private int moedasTotais = 0;
    public HashSet<string> moedasColetadas = new HashSet<string>();

    // ===== SISTEMA DE CENOURAS COLETADAS =====
    public HashSet<string> cenourasColetadas = new HashSet<string>();

    // ===== MÉTODOS DE INIMIGOS =====
    public void RegistrarInimigoMorto(string id)
    {
        if (!inimigosMortos.Contains(id))
        {
            inimigosMortos.Add(id);
            Debug.Log($"🐍 Inimigo morto: {id}");
        }
    }

    public bool InimigoJaMorreu(string id)
    {
        return inimigosMortos.Contains(id);
    }

    // ===== MÉTODOS DE MOEDAS =====
    public void RegistrarMoedaColetada(string id)
    {
        if (!moedasColetadas.Contains(id))
        {
            moedasColetadas.Add(id);
            moedasTotais++;
            Debug.Log($"💰 Moeda coletada: {id} | Total: {moedasTotais}");
        }
    }

    public bool MoedaJaColetada(string id)
    {
        return moedasColetadas.Contains(id);
    }

    public void RemoverMoedas(int quantidade)
    {
        moedasTotais -= quantidade;
        if (moedasTotais < 0)
            moedasTotais = 0;

        Debug.Log($"💰 Moedas após remoção: {moedasTotais}");
    }

    public int ObterMoedasTotais()
    {
        return moedasTotais;
    }

    // ===== MÉTODOS DE CENOURAS =====
    public void RegistrarCenouraColetada(string id)
    {
        if (!cenourasColetadas.Contains(id))
        {
            cenourasColetadas.Add(id);
            Debug.Log($"🥕 Cenoura coletada: {id}");
        }
    }

    public bool CenouraJaColetada(string id)
    {
        return cenourasColetadas.Contains(id);
    }

    // ===== RESETAR FASE (SÓ QUANDO MORRE 3X) =====
    public void ResetarFaseCompleta()
    {
        inimigosMortos.Clear();
        moedasColetadas.Clear();
        cenourasColetadas.Clear();
        moedasTotais = 0;

        // ← CORRIGIDO: Reseta o checkpoint também!
        ResetarCheckpoint();

        Debug.Log("🔄 FASE RESETADA COMPLETAMENTE (incluindo checkpoint)");
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
        BuscarTelasNaCena();
    }

    private void BuscarTelasNaCena()
    {
        GameObject[] todosObjetos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in todosObjetos)
        {
            if (obj.scene.isLoaded)
            {
                if (obj.name == "TelaVitoria")
                {
                    telaVitoriaAtual = obj;
                    telaVitoriaAtual.SetActive(false);
                    Debug.Log("✅ TelaVitoria encontrada!");
                }

                if (obj.name == "TelaGameOver")
                {
                    telaGameOverAtual = obj;
                    telaGameOverAtual.SetActive(false);
                    Debug.Log("✅ TelaGameOver encontrada!");
                }
            }
        }
    }

    // ========= GAME OVER (SÓ QUANDO VIDA CHEGA A 0) =========
    public void MostrarTelaGameOver()
    {
        if (telaGameOverAtual == null)
            BuscarTelasNaCena();

        if (telaGameOverAtual != null)
        {
            telaGameOverAtual.SetActive(true);
            Time.timeScale = 0f;
            StartCoroutine(AguardarTeclaReiniciarFase());
        }
        else
        {
            Debug.LogError("❌ TelaGameOver não encontrada!");
        }
    }

    private IEnumerator AguardarTeclaReiniciarFase()
    {
        yield return null;

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;

        // ✅ Reseta tudo porque morreu 3x
        ResetarFaseCompleta();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ========= VITÓRIA =========
    public void MostrarTelaVitoria(string nextScene)
    {
        proximaCena = nextScene;

        if (telaVitoriaAtual == null)
            BuscarTelasNaCena();

        if (telaVitoriaAtual != null)
        {
            telaVitoriaAtual.SetActive(true);
            Time.timeScale = 0f;
            StartCoroutine(AguardarTeclaParaProximaFase());
        }
        else
        {
            Debug.LogError("❌ TelaVitoria não encontrada!");
        }
    }

    private IEnumerator AguardarTeclaParaProximaFase()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;

        // Ao passar de fase, limpa os dados da fase anterior
        ResetarCheckpoint();
        ResetarFaseCompleta();

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
        Debug.Log("Checkpoint resetado");
    }

    // Reseta TUDO ao voltar pro menu
    public void ResetarJogo()
    {
        ResetarCheckpoint();
        ResetarFaseCompleta();
        Debug.Log("🔄 Jogo resetado completamente");
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