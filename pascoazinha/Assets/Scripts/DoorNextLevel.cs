using UnityEngine;

public class DoorNextLevel : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Se ainda existe inimigo na cena, não passa
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            Debug.Log("Ainda há inimigos vivos!");
            return;
        }

        // Mostra a tela de vitória e aguarda tecla para próxima fase
        Vector3 minhaPosicao = new Vector3(-13f, 5f, 0f);
        GameManager.instance.DefinirCheckpoint(minhaPosicao);
        GameManager.instance.MostrarTelaVitoria(nextSceneName);
    }
}