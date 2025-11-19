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

    // ========= GAME OVER =========
    public void MostrarTelaGameOver()
    {
        Time.timeScale = 0f;
        telaGameOver.SetActive(true);
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
        telaVitoria.SetActive(true);
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

    public void DefinirCheckpoint(Vector3 transformPosition)
    {
        throw new System.NotImplementedException();
    }
}