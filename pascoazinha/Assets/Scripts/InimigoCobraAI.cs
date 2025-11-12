using UnityEngine;

public class InimigoCobraAI : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public Transform pontoA;
    public Transform pontoB;
    private Transform alvoAtual;
    private bool indoParaA = false;

    [Header("Combate")]
    public float distanciaAtaque = 1.5f;
    public float dano = 10f;
    public float tempoEntreAtaques = 1.5f;
    private float cooldownAtaque;

    [Header("Referências")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public Transform player;

    [Header("Vida")]
    public float vidaMaxima = 10f;
    private float vidaAtual;
    private bool morto = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
        alvoAtual = pontoB;
    }

    void Update()
    {
        if (morto) return;

        float distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaPlayer <= distanciaAtaque)
        {
            // Player perto -> atacar
            animator.SetBool("Walk", false);
            if (cooldownAtaque <= 0)
            {
                Atacar();
                cooldownAtaque = tempoEntreAtaques;
            }
        }
        else
        {
            // Player longe -> patrulhar
            Patrulhar();
        }

        if (cooldownAtaque > 0)
            cooldownAtaque -= Time.deltaTime;
    }

    void Patrulhar()
    {
        animator.SetBool("Walk", true);

        transform.position = Vector2.MoveTowards(transform.position,
            alvoAtual.position,
            velocidade * Time.deltaTime);

        // Verifica se chegou ao ponto
        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.05f)
        {
            // Troca de destino
            if (alvoAtual == pontoA)
            {
                alvoAtual = pontoB;
                indoParaA = false;
            }
            else
            {
                alvoAtual = pontoA;
                indoParaA = true;
            }

            // Vira na direção correta
            Virar();
        }
    }

    void Virar()
    {
        Vector3 escala = transform.localScale;
        if (indoParaA)
            escala.x = Mathf.Abs(escala.x); // direita
        else
            escala.x = -Mathf.Abs(escala.x); // esquerda
        transform.localScale = escala;
    }

    void Atacar()
    {
        animator.SetTrigger("Attack");
        Debug.Log("Cobra atacou o jogador!");
    }

    public void TomarDano(float dano)
    {
        if (morto) return;

        vidaAtual -= dano;
        if (vidaAtual <= 0)
            Morrer();
    }

    void Morrer()
    {
        morto = true;
        animator.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmos()
    {
        if (pontoA != null && pontoB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pontoA.position, pontoB.position);
        }
    }
}
