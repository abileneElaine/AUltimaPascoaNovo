using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public bool moverHorizontal = true;  // ✅ Marca pra mover na horizontal; desmarca pra mover na vertical
    public float distancia = 3f;         // Distância do movimento
    public float velocidade = 2f;        // Velocidade do movimento

    private Vector3 posicaoInicial;
    private Vector3 pontoDestino;
    private bool indo = true;

    void Start()
    {
        posicaoInicial = transform.position;

        if (moverHorizontal)
            pontoDestino = posicaoInicial + Vector3.right * distancia;
        else
            pontoDestino = posicaoInicial + Vector3.up * distancia;
    }

    void Update()
    {
        MoverPlataforma();
    }

    void MoverPlataforma()
    {
        // Define o ponto alvo
        Vector3 alvo = indo ? pontoDestino : posicaoInicial;

        // Move em direção ao alvo
        transform.position = Vector3.MoveTowards(transform.position, alvo, velocidade * Time.deltaTime);

        // Quando chegar em um dos extremos, inverte a direção
        if (Vector3.Distance(transform.position, alvo) < 0.05f)
        {
            indo = !indo;
        }
    }

    // Faz o jogador "andar junto" com a plataforma
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}