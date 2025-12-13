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

    // ===== SISTEMA DE MOEDAS =====
    private int moedasTotais = 0; // Total acumulado de moedas

    public void RegistrarInimigoMorto(string id)
    {
        if (!inimigosMortos.Contains(id))
            inimigosMortos.Add(id);
    }

    public bool InimigoJaMorreu(string id)
    {
        return inimigosMortos.Contains(id);
    }

    // ===== MÉTODOS DE MOEDAS =====

    public void AdicionarMoeda()
    {
        moedasTotais++;
        Debug.Log($"💰 Total de moedas: {moedasTotais}");
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

    public void ResetarMoedas()
    {
        moedasTotais = 0;
        Debug.Log("💰 Moedas resetadas para 0");
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
        // BUSCA AS TELAS NA CENA ATUAL (incluindo objetos inativos)
        BuscarTelasNaCena();
    }

    private void BuscarTelasNaCena()
    {
        // Busca TODOS os GameObjects, incluindo inativos
        GameObject[] todosObjetos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in todosObjetos)
        {
            // Verifica se o objeto pertence à cena atual (não é prefab)
            if (obj.scene.isLoaded)
            {
                if (obj.name == "TelaVitoria")
                {
                    telaVitoriaAtual = obj;
                    telaVitoriaAtual.SetActive(false);
                    Debug.Log("✅ TelaVitoria encontrada na cena!");
                }

                if (obj.name == "TelaGameOver")
                {
                    telaGameOverAtual = obj;
                    telaGameOverAtual.SetActive(false);
                    Debug.Log("✅ TelaGameOver encontrada na cena!");
                }
            }
        }

        if (telaVitoriaAtual == null)
            Debug.LogWarning("⚠️ TelaVitoria não encontrada! Certifique-se de que existe um GameObject chamado 'TelaVitoria' na cena.");

        if (telaGameOverAtual == null)
            Debug.LogWarning("⚠️ TelaGameOver não encontrada! Certifique-se de que existe um GameObject chamado 'TelaGameOver' na cena.");
    }

    // ========= GAME OVER =========
    public void MostrarTelaGameOver()
    {
        // Se não encontrou, tenta buscar novamente
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
            Debug.LogError("❌ TelaGameOver não foi encontrada na cena!");
        }
    }

    private IEnumerator AguardarTeclaReiniciarFase()
    {
        yield return null;

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;

        // ✅ MOEDAS PERSISTEM ao morrer - NÃO reseta!

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ========= VITÓRIA =========
    public void MostrarTelaVitoria(string nextScene)
    {
        proximaCena = nextScene;

        // Se não encontrou, tenta buscar novamente
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
            Debug.LogError("❌ TelaVitoria não foi encontrada na cena!");
        }
    }

    private IEnumerator AguardarTeclaParaProximaFase()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        while (!Input.anyKeyDown)
            yield return null;

        Time.timeScale = 1f;

        // ✅ MOEDAS PERSISTEM ao passar de fase - NÃO reseta!
        // Apenas limpa checkpoint e inimigos
        ResetarCheckpoint();

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
        inimigosMortos.Clear();
        Debug.Log("Checkpoint resetado para nova fase");
    }

    // Método completo para resetar tudo ao voltar pro menu
    public void ResetarJogo()
    {
        ResetarCheckpoint();
        ResetarMoedas();
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