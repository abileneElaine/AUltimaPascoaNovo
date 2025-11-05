using UnityEngine;

public class Cenoura : MonoBehaviour
{
    [Header("Configurações da Cenoura")]
    public float lifetime = 3f;       // Tempo antes de se autodestruir
    public string tipo = "Normal";    // "Normal", "Fogo", "Gelo"

    [Header("Efeitos Visuais")]
    public GameObject efeitoFogo;
    public GameObject efeitoGelo;

    void Start()
    {
        // Instancia efeito ao criar a cenoura, se necessário
        if (tipo == "Fogo" && efeitoFogo != null)
        {
            Instantiate(efeitoFogo, transform.position + Vector3.up * 1f, Quaternion.identity);
        }
        else if (tipo == "Gelo" && efeitoGelo != null)
        {
            Instantiate(efeitoGelo, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        // Destrói a cenoura após o tempo definido
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Ground"))
        {
            AplicarEfeito(collision.gameObject);
            CriarEfeitoVisual(collision.contacts[0].point);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Ground"))
        {
            AplicarEfeito(other.gameObject);
            CriarEfeitoVisual(other.transform.position);
            Destroy(gameObject);
        }
    }

    void AplicarEfeito(GameObject alvo)
    {
        // Só aplica efeitos se for inimigo
        if (!alvo.CompareTag("Enemy")) return;

        Inimigo inimigo = alvo.GetComponent<Inimigo>();
        if (inimigo == null) return;

        switch (tipo)
        {
            case "Fogo":
                inimigo.TomarDano(10);
                inimigo.Queimar(2f, 3f); // duração e dano por segundo
                break;

            case "Gelo":
                inimigo.TomarDano(5);
                inimigo.Congelar(2f); // duração do congelamento
                break;

            default: // Normal
                inimigo.TomarDano(10);
                break;
        }
    }

    void CriarEfeitoVisual(Vector3 pos)
    {
        GameObject efeito = null;

        Debug.Log("CriarEfeitoVisual chamado para tipo: " + tipo);

        switch (tipo)
        {
            case "Fogo":
                if (efeitoFogo != null)
                    efeito = Instantiate(efeitoFogo, pos, Quaternion.identity);
                break;

            case "Gelo":
                if (efeitoGelo != null)
                    efeito = Instantiate(efeitoGelo, pos, Quaternion.identity);
                break;
        }

        if (efeito != null)
        {
            Destroy(efeito, 2f); // Destrói efeito após 2 segundos
        }
    }
}
