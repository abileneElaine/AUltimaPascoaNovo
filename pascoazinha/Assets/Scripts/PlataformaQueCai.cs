using UnityEngine;
using System.Collections;

public class PlataformaQueCai : MonoBehaviour
{
    [Header("Configurações")]
    public float tempoAntesDeCair = 1f;      // Tempo que o player precisa ficar em cima antes de cair
    public float tempoParaVoltar = 3f;       // Tempo para a plataforma reaparecer
    public float velocidadeDeQueda = 5f;     // Velocidade da queda

    private Vector3 posicaoInicial;          // Posição original da plataforma
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;
    private bool caindo = false;

    void Start()
    {
        posicaoInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic; // Fica parada até cair
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Só reage se for o jogador e ele estiver em cima
        if (!caindo && collision.collider.CompareTag("Player"))
        {
            // Verifica se o player está realmente pisando (não batendo de lado)
            if (collision.contacts[0].normal.y < 0)
            {
                StartCoroutine(CairDepois());
            }
        }
    }

    IEnumerator CairDepois()
    {
        caindo = true;
        yield return new WaitForSeconds(tempoAntesDeCair);

        // A plataforma começa a cair
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;

        // Espera um pouco antes de sumir
        yield return new WaitForSeconds(0.7f);

        // Some (fica invisível e sem colisão)
        sprite.enabled = false;
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        transform.position = posicaoInicial; // volta pro lugar, mas ainda invisível

        // Espera o tempo para reaparecer
        yield return new WaitForSeconds(tempoParaVoltar);

        // Reativa tudo
        sprite.enabled = true;
        col.enabled = true;
        caindo = false;
    }
}
