using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ativado && other.CompareTag("Player"))
        {
            ativado = true;

            // Salva posição no GameManager
            GameManager.instance.DefinirCheckpoint(transform.position);

            // Recupera vida do player
            PlayerVida vida = other.GetComponent<PlayerVida>();
            if (vida != null)
            {
                vida.coracoesAtuais = vida.totalCoracoes;
                vida.SendMessage("AtualizarCoracoes");
                Debug.Log("Corações restaurados!");
            }
        }
    }
}