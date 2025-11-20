using UnityEngine;

public class MiniBossPatrulha : MonoBehaviour, IDamageable
{
    [Header("Referências")]
    public Transform moveDestination;     // destino da patrulha (igual à cobra)
    private Transform player;

    [Header("Configurações")]
    public float moveSpeed = 1.5f;
    public float attackRange = 1.2f;      // distância para atacar
    public int maxVida = 4;               // morre com 4 cenouras de gelo

    [Header("Dano")]
    public int danoAtaque = 1;            // dano dado pelo hitbox na animação

    private Vector2 _initialPosition;
    private Vector2 _target;
    private Vector2 _direction;
    private bool _returning = false;

    private int vidaAtual;
    private bool isAlive = true;

    private Animator anim;
    private SpriteRenderer sprite;

    private float originalScaleX;

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        vidaAtual = maxVida;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        _initialPosition = transform.position;
        _target = moveDestination.position;

        _direction = (_target - (Vector2)transform.position).normalized;

        originalScaleX = transform.localScale.x;
    }

    void Update()
    {
        if (!isAlive) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // --- ATACA SE O PLAYER ESTÁ PERTO ---
        if (distancia <= attackRange)
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("Attack");   // a animação vai chamar o dano
            OlharPara(player.position);
            return;
        }

        // --- PATRULHA SE O PLAYER ESTÁ LONGE ---
        Patrulhar();
    }

    // ------------------------------------------------------
    // SISTEMA DE PATRULHA – IGUAL O DA COBRA
    // ------------------------------------------------------
    void Patrulhar()
    {
        anim.SetBool("isWalking", true);

        if (!_returning)
        {
            if (Vector2.Distance(transform.position, _target) < 0.1f)
            {
                _returning = true;
                _direction = (_initialPosition - (Vector2)transform.position).normalized;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, _initialPosition) < 0.1f)
            {
                _returning = false;
                _direction = (_target - (Vector2)transform.position).normalized;
            }
        }

        OlharPara(transform.position + (Vector3)_direction);

        transform.position += (Vector3)_direction * moveSpeed * Time.deltaTime;
    }

    // ------------------------------------------------------
    // DEIXAR O BOSS VIRADO PARA O ALVO
    // ------------------------------------------------------
    void OlharPara(Vector3 alvo)
    {
        if ((alvo.x > transform.position.x && transform.localScale.x < 0) ||
            (alvo.x < transform.position.x && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    // ------------------------------------------------------
    // TOMAR DANO (SÓ DE CENOURA DE GELO)
    // ------------------------------------------------------
    public void TakeEnergy(int amount)
    {
        TakeDamage(amount);
    }

    public void TakeDamage(int dano)
    {
        if (!isAlive) return;

        vidaAtual -= dano;

        sprite.color = Color.cyan; // hit azul (gelo)
        Invoke("ResetColor", 0.15f);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void ResetColor()
    {
        sprite.color = Color.white;
    }

    void Morrer()
    {
        isAlive = false;
        anim.SetTrigger("Die");
        Destroy(gameObject, 1.5f);
    }

    // ------------------------------------------------------
    // CAUSAR DANO NA ANIMAÇÃO
    // ESTE MÉTODO É CHAMADO NO EVENTO DA ANIMAÇÃO "Attack"
    // ------------------------------------------------------
    public void HitPlayer()
    {
        if (!isAlive) return;

        Collider2D col = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (col != null)
        {
            col.GetComponent<IDamageable>()?.TakeEnergy(danoAtaque);
        }
    }

    // DEBUG DA ÁREA DE ATAQUE
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
