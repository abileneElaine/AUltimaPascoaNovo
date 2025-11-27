using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UIs (opcionais - se deixar vazio, o GameManager tenta encontrar por nome)")]
    public GameObject telaGameOver;
    public GameObject telaVitoria;

    private string proximaCena;

    // CHECKPOINT
    private Vector3 checkpointPos;
    private bool temCheckpoint = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Awake - singleton criado");
        }
        else
        {
            Debug.Log("[GameManager] Awake - instância duplicada destruída");
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    // ---------- GAME OVER ----------
    public void MostrarTelaGameOver()
    {
        Time.timeScale = 0f;
        
        

        // --- FIX CRUCIAL ---
        if (telaGameOver == null)
            telaGameOver = GameObject.Find("TelaGameOver");
        Debug.Log("AAAAAA: "+ telaGameOver.name);
        if (telaGameOver != null)
            telaGameOver.SetActive(true);
        else
            Debug.LogError("TelaGameOver NÃO ENCONTRADA NA CENA!");

        StartCoroutine(AguardarTeclaReiniciarFase());
    }


    private IEnumerator AguardarTeclaReiniciarFase()
    {
        yield return null;
        while (!Input.anyKeyDown)
            yield return null;

        // Reset no timescale ANTES do reload (importantíssimo)
        Time.timeScale = 1f;

        // recarrega cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ---------- VITÓRIA ----------
    public void MostrarTelaVitoria(string nextScene)
    {
        proximaCena = nextScene;
        Time.timeScale = 0f;
        EnsureVitoriaUI();
        if (telaVitoria != null) telaVitoria.SetActive(true);
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

    // ---------- CHECKPOINT API ----------
    public void DefinirCheckpoint(Vector3 pos)
    {
        checkpointPos = pos;
        temCheckpoint = true;
        Debug.Log("[GameManager] Checkpoint definido em: " + pos);
    }

    public bool TemCheckpoint()
    {
        return temCheckpoint;
    }

    public Vector3 ObterCheckpoint()
    {
        return checkpointPos;
    }

    // ---------- SCENE LOADED HANDLER ----------
    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        // Reencontra as telas de UI destruídas
        telaGameOver = GameObject.Find("TelaGameOver");
        telaVitoria = GameObject.Find("TelaVitoria");

        // APÓS reencontrar as UIs, reposiciona Player se precisar
        StartCoroutine(ReposicionarNoCheckpoint());
    }

    private IEnumerator ReposicionarNoCheckpoint()
    {
        if (!temCheckpoint) yield break;

        // Espera o Player existir na cena
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        // Move
        player.transform.position = checkpointPos;

        // Reseta vida
        PlayerVida pv = player.GetComponent<PlayerVida>();
        if (pv != null)
        {
            pv.vivo = true;   // <- ESSA LINHA É O SEGUNDO FIX IMPORTANTE
            pv.invencivel = false;
        }
    }


    // Espera o player existir na cena e o reposiciona com offset + reseta estado
    private IEnumerator MoverPlayerAoCheckpoint()
    {
        float timeout = 2f; // segundos máximos de espera
        float t = 0f;
        GameObject player = null;

        // espera até encontrar um objeto com tag "Player" por até timeout segundos
        while (player == null && t < timeout)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) break;
            t += Time.deltaTime;
            yield return null;
        }

        if (player == null)
        {
            Debug.LogWarning("[GameManager] Não encontrou Player após carregar cena (timeout).");
            yield break;
        }

        // aplica offset vertical pequeno para evitar spawn "dentro" do chão
        Vector3 target = checkpointPos + Vector3.up * 0.15f;
        player.transform.position = target;
        Debug.Log("[GameManager] Player reposicionado no checkpoint: " + target);

        // tenta restaurar o estado do Player (vida / flags)
        var vida = player.GetComponent<PlayerVida>();
        if (vida != null)
        {
            vida.coracoesAtuais = vida.totalCoracoes;
            vida.SendMessage("AtualizarCoracoes", SendMessageOptions.DontRequireReceiver);
            Debug.Log("[GameManager] PlayerVida restaurada no checkpoint.");
        }

        // chama OnRespawn para garantir que 'vivo' e invencibilidade sejam resetados
        player.SendMessage("OnRespawn", SendMessageOptions.DontRequireReceiver);

        // reabilita movement caso tenha sido desabilitado
        var mov = player.GetComponent<PlayerMovement>();
        if (mov != null && !mov.enabled)
            mov.enabled = true;
    }

    // ---- helpers para localizar UI se as referências forem nulas ----
    private void EnsureGameOverUI()
    {
        if (telaGameOver == null)
        {
            // tenta achar por nome "TelaGameOver" — ajuste se seu objeto tiver outro nome
            var go = GameObject.Find("TelaGameOver");
            if (go != null) telaGameOver = go;
        }
    }

    private void EnsureVitoriaUI()
    {
        if (telaVitoria == null)
        {
            var go = GameObject.Find("TelaVitoria");
            if (go != null) telaVitoria = go;
        }
    }
}
