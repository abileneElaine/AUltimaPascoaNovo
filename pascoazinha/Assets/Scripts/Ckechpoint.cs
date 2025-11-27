using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Checkpoint] Trigger entrou: " + other.name);

        if (ativado) 
        {
            Debug.Log("[Checkpoint] Já ativado, ignorando.");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[Checkpoint] Quem entrou não é Player.");
            return;
        }

        ativado = true;
        Vector3 savePos = transform.position;
        // salva um pouco acima para evitar spawn dentro do chão
        savePos += Vector3.up * 0.1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.DefinirCheckpoint(savePos);
        }
        else
        {
            Debug.LogError("[Checkpoint] GameManager.instance é null!");
        }

        PlayerVida vida = other.GetComponentInParent<PlayerVida>();
        if (vida != null)
        {
            vida.coracoesAtuais = vida.totalCoracoes;
            vida.SendMessage("AtualizarCoracoes", SendMessageOptions.DontRequireReceiver);
            Debug.Log("[Checkpoint] Corações restaurados!");
        }
    }
}