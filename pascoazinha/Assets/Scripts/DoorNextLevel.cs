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
        GameManager.instance.MostrarTelaVitoria(nextSceneName);
    }
}