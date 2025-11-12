using UnityEngine;

public class Cobra : Inimigo
{
    [Header("Movimentação")]
    public float velocidade = 1f;
    public float distanciaPatrulha = 0.05f; // 5 cm
    private Vector2 pontoInicial;
    private bool indoDireita = true;

    [Header("Detecção e Ataque")]
    public float raioDeteccao = 1.5f;
    public float tempoEntreAtaques = 1.5f;
    public int danoAtaque = 5;

    private float tempoUltimoAtaque = 0f;
    private Transform jogador;
    private PlayerVida vidaJogador;

    void Awake()
    {
        pontoInicial = transform.position;
        jogador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jogador != null)
            vidaJogador = jogador.GetComponent<PlayerVida>();
    }

    void Update()
    {
        if (vidaAtual <= 0) return;

        if (JogadorProximo())
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time - tempoUltimoAtaque >= tempoEntreAtaques)
            {
                Atacar();
                tempoUltimoAtaque = Time.time;
            }
        }
        else
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        if (animator != null)
            animator.Play("Walk");

        float direcao = indoDireita ? 1 : -1;
        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        if (Vector2.Distance(transform.position, pontoInicial) > distanciaPatrulha)
        {
            indoDireita = !indoDireita;
            pontoInicial = transform.position;
            sprite.flipX = !sprite.flipX;
        }
    }

    bool JogadorProximo()
    {
        if (jogador == null) return false;
        return Vector2.Distance(transform.position, jogador.position) <= raioDeteccao;
    }

    void Atacar()
    {
        if (animator != null)
            animator.Play("Attack");

        if (vidaJogador != null && JogadorProximo())
        {
            vidaJogador.TomarDano(danoAtaque);
            Debug.Log("Cobra atacou o jogador!");
        }
    }

    public override void Morrer()
    {
        base.Morrer();
        if (animator != null)
            animator.Play("Morrer");
        rb.linearVelocity = Vector2.zero;
    }
}
